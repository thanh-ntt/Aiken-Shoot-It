using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Networking;

public class SpaceshipHealth_TurnBased : NetworkBehaviour
{

    public int maxHealth = 100;
    public ParticleSystem killParticle;
    public GameObject HealthBarPrefab;
    public float canvasHeightAbovePlayer;

    AudioSource Explosion;
    public GameObject HealthBarInstance;
    TextMeshProUGUI informationText;
    int health;

    // Use this for initialization
    void Start()
    {
        health = maxHealth;
        informationText = GameObject.Find("Buttons").GetComponentInChildren<TextMeshProUGUI>();
        HealthBarInstance = Instantiate(HealthBarPrefab, transform.position + Vector3.up * canvasHeightAbovePlayer, Quaternion.identity);
        HealthBarInstance.GetComponentInChildren<Image>().fillAmount = maxHealth;
        Explosion = GetComponentsInChildren<AudioSource>()[1];

        // If this is the server, tell DeathMatchManager that this spaceship spawned
        if (isServer)
            DeathMatchManager_TurnBased.AddSpaceship(this);
    }

    private void Update()
    {
        if (!isLocalPlayer && CrossPlatformInputManager.GetButton("QuitButton"))
            CmdDieInstantly();
    }

    [Command]
    void CmdDieInstantly()
    {
        RpcDied();

        if (DeathMatchManager_TurnBased.RemoveSpaceshipAndCheckWinner(this))
        {
            SpaceshipHealth_TurnBased spaceship = DeathMatchManager_TurnBased.GetWinner();
            spaceship.RpcWon();

            // Back to lobby in 2 seconds
            Invoke("BackToLobby", 2f);
        }
    }

    void FixedUpdate()
    {
        HealthBarInstance.transform.position = transform.position + Vector3.up * canvasHeightAbovePlayer;
    }

    public void TakeDamage(int amount)
    {
        if (!isServer || health <= 0)
            return;
        
        health -= amount;
        RpcUpdateHealthBar((float)health / (float)maxHealth);

        if (health <= 0)
        {
            health = 0;
            // Call a method on all instances of this object (Remote procedure calls)
            RpcDied();

            if (DeathMatchManager_TurnBased.RemoveSpaceshipAndCheckWinner(this))
            {
                SpaceshipHealth_TurnBased spaceship = DeathMatchManager_TurnBased.GetWinner();
                spaceship.RpcWon();

                // Back to lobby in 2 seconds
                Invoke("BackToLobby", 2f);
            }

            // There may be "hurt / bullet collision" effects, which can be
            // added after this line
            // Return prevents those from running
            return;
        }
        // Hurt effect here
    }

    [ClientRpc]
    public void RpcUpdateHealthBar(float amount)
    {
        HealthBarInstance.GetComponentInChildren<Image>().fillAmount = amount;
    }

    // Every instances of this object, on everyone's device, die.
    [ClientRpc]
    void RpcDied()
    {
        // Run killParticle effects
        killParticle.transform.SetParent(null);
        killParticle.transform.position = transform.position;
        killParticle.gameObject.SetActive(true);
        killParticle.time = 0;
        killParticle.Play();
        Explosion.enabled = true;
        Explosion.Play();

        GetComponent<SpriteRenderer>().enabled = false;

        if (isLocalPlayer)
        {
            informationText.text = "Game Over";

            // Disable spaceship functions
            GetComponent<SpaceshipController_TurnBased>().canMove = false;
            GetComponent<SpaceshipShooting_TurnBased>().isTurn = false;
        }
    }

    [ClientRpc]
    public void RpcWon()
    {
        // LocalPlayer <=> Winner, since everyone else is dead
        if (isLocalPlayer)
        {
            informationText.text = "You Won!";
        }
    }

    void BackToLobby()
    {
        FindObjectOfType<NetworkLobbyManager>().ServerReturnToLobby();
    }
}
