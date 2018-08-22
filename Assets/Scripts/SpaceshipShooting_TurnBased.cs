using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Networking;

public class SpaceshipShooting_TurnBased : NetworkBehaviour
{

    public GameObject bulletPrefab;
    public Transform gunBarrel;
    public bool isTurn;

    AudioSource shootSFX;
    Light playerLight;
    List<ItemUsage_TurnBased.EffectDelegate> effectDelegates;

    TextMeshProUGUI turnTimeText;
    float curTurnTime;
    float turnTime;
    bool canShoot;

    void Start()
    {
        turnTimeText = GameObject.Find("Time").GetComponentInChildren<TextMeshProUGUI>();
        turnTime = TurnManager.turnTime;
        curTurnTime = turnTime;
        shootSFX = GetComponent<AudioSource>();

        effectDelegates = new List<ItemUsage_TurnBased.EffectDelegate>();
        playerLight = GetComponentInChildren<Light>();
        if (isLocalPlayer)
            playerLight.enabled = true;
        else
            playerLight.enabled = false;
    }

    void Update()
    {
        if (!isLocalPlayer || !isTurn)
            return;
        
        curTurnTime -= Time.deltaTime;
        if (curTurnTime < 15.7)
            turnTimeText.text = "Time Left: " + (int)curTurnTime;

        if (CrossPlatformInputManager.GetButton("EndTurn") && isTurn)
        {
            isTurn = false;
            TogglePlayer();
        }

        if (curTurnTime <= 0)
            TogglePlayer();

        if (canShoot && CrossPlatformInputManager.GetButton("Fire"))
        {
            canShoot = false;
            CmdSpawnBullet();
            shootSFX.Play();
            if (!GetComponent<SpaceshipController_TurnBased>().canMove)
                TogglePlayer();
        }
    }

    public void EnableShooting()
    {
        isTurn = true;
        canShoot = true;
    }

    void TogglePlayer()
    {
        canShoot = false;
        curTurnTime = turnTime;
        turnTimeText.text = "Enemy's Turn";
        CmdTogglePlayer();
    }

    [Command]
    void CmdSpawnBullet()
    {
        Vector3 bulletSpawnPosition = new Vector3(gunBarrel.position.x, gunBarrel.position.y, gunBarrel.position.z - 1f);

        Vector3 bulletSpawnDirection = gameObject.transform.rotation.eulerAngles
                                                 + new Vector3(0f, 0f, (90f - 40.4f));
        GameObject instance = Instantiate(bulletPrefab, bulletSpawnPosition, Quaternion.Euler(bulletSpawnDirection)) as GameObject;

        if (effectDelegates.Count > 0)
        {
            effectDelegates[0](instance);
            effectDelegates.RemoveAt(0);
        }

        NetworkServer.Spawn(instance);
    }

    public void RegisterEffect(ItemUsage_TurnBased.EffectDelegate effectDelegate)
    {
        effectDelegates.Add(effectDelegate);
    }

    [Command]
    void CmdTogglePlayer()
    {
        TurnManager.TogglePlayer();
    }
}
