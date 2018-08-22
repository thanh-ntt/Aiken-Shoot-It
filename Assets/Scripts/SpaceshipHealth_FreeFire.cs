using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Networking;


public class SpaceshipHealth_FreeFire : NetworkBehaviour
{
    public int maxHealth = 100;
    public ParticleSystem killParticle;
    public GameObject HealthBarPrefab;
    public float canvasHeightAbovePlayer;

    public GameObject HealthBarInstance;
    TextMeshProUGUI informationText;
    int health;

    //public Canvas QuitButton;
    
    // Use this for initialization
    void Start()
    {
        health = maxHealth;
        informationText = GameObject.Find("Buttons").GetComponentInChildren<TextMeshProUGUI>();
        HealthBarInstance = Instantiate(HealthBarPrefab, transform.position + Vector3.up * canvasHeightAbovePlayer, Quaternion.identity);
        HealthBarInstance.GetComponentInChildren<Image>().fillAmount = 1f;  

        // If this is the server, tell DeathMatchManager that this spaceship spawned
        if (isServer)
            DeathMatchManager_FreeFire.AddSpaceship(this);
    }

    void Update()
    {
        if (isLocalPlayer && CrossPlatformInputManager.GetButton("QuitButton"))
        {
            CmdDieInstantly();
            Invoke("BackToLobby", 2f);
        }
    }

    [Command]
    void CmdDieInstantly()
    {
        RpcDied();
        if (DeathMatchManager_FreeFire.RemoveSpaceshipAndCheckWinner(this))
        {
            SpaceshipHealth_FreeFire spaceship = DeathMatchManager_FreeFire.GetWinner();
            spaceship.RpcWon();

            // Back to lobby in 2 seconds
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

        if (health > maxHealth)
        {
            health = maxHealth;
            return;
        }
        RpcUpdateHealthBar((float)health / (float)maxHealth);

        if (health <= 0)
        {
            health = 0;
            // Call a method on all instances of this object (Remote procedure calls)

            RpcDied();
            if (DeathMatchManager_FreeFire.RemoveSpaceshipAndCheckWinner(this))
            {
                SpaceshipHealth_FreeFire spaceship = DeathMatchManager_FreeFire.GetWinner();
                spaceship.RpcWon();

                // Back to lobby in 2 seconds
                Invoke("BackToLobby", 2f);
            }
            return;
        }
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

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponentsInChildren<SpriteRenderer>()[1].enabled = false;
        Destroy(GetComponent<SpaceshipController_FreeFire>().energyBar);
        GetComponent<SpaceshipController_FreeFire>().canMove = false;
        GetComponent<SpaceshipShooting_FreeFire>().enabled = false;
        GetComponent<PolygonCollider2D>().enabled = false;
        GetComponent<NetworkTransform>().enabled = false;

        if (isLocalPlayer)
        {
            informationText.text = "Game Over";
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
