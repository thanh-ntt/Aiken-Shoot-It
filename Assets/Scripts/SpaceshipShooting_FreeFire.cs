using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Networking;

public class SpaceshipShooting_FreeFire : NetworkBehaviour
{

    public GameObject bulletPrefab;
    public GameObject bulletPenetrationPrefab;
    public float shootInterval;
    public Transform gunBarrel;

    public bool isPenetrationBullet;

    AudioSource shootSFX;
    public float curInterval;
    List<ItemUsage_FreeFire.EffectDelegate> effectDelegates;

    private void Start()
    {
        effectDelegates = new List<ItemUsage_FreeFire.EffectDelegate>();
        shootSFX = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;
        
        if (curInterval > 0)
            curInterval -= Time.deltaTime;
        if (CrossPlatformInputManager.GetButton("Fire") && curInterval <= 0)
        {
            if (isPenetrationBullet)
            {
                CmdSpawnPenetrationBullet();
                shootSFX.Play();
                curInterval = shootInterval;
                isPenetrationBullet = false;
                return;
            }
            CmdSpawnBullet();
            shootSFX.Play();
            curInterval = shootInterval;
        }
    }

    // Run server command to spawn bullet
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

    [Command]
    void CmdSpawnPenetrationBullet()
    {
        Vector3 bulletSpawnPosition = new Vector3(gunBarrel.position.x, gunBarrel.position.y, gunBarrel.position.z - 1f);

        Vector3 bulletSpawnDirection = gameObject.transform.rotation.eulerAngles
                                                 + new Vector3(0f, 0f, (90f - 40.4f));
        GameObject instance = Instantiate(bulletPenetrationPrefab, bulletSpawnPosition, Quaternion.Euler(bulletSpawnDirection)) as GameObject;

        NetworkServer.Spawn(instance);
    }

    public void RegisterEffect(ItemUsage_FreeFire.EffectDelegate effectDelegate)
    {
        effectDelegates.Add(effectDelegate);
    }
}
