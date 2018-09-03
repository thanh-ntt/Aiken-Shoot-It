using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ItemUsage_FreeFire : NetworkBehaviour {

    [Range(0, 100)] public int healthPotionAmount;
    [Range(0, 20)] public int healingAmount;
    public float boostDmgRatio;
    public float bulletStunTime;
    public float bulletSilenceTime;
    public float fireRateTime;
    public float fireRateRatio;
    public float invisibleTime;

    public delegate void EffectDelegate(GameObject gameObject);

    bool isHealing;

	public void HealthPotion()
    {
        GetComponent<SpaceshipHealth_FreeFire>().TakeDamage(-healthPotionAmount);
        if (isServer)
            GetComponent<ItemEffect_FreeFire>().HpCollectEffect();
    }

    public void StartHealing()
    {
        isHealing = true;
        Invoke("Healing", 0.1f);
    }

    public void StopHealing()
    {
        isHealing = false;
    }

    public void BulletBoostDmg()
    {
        GetComponent<SpaceshipShooting_FreeFire>().RegisterEffect(BoostDamage);
        if (isServer)
            GetComponent<ItemEffect_FreeFire>().DamageBoostEffect();
    }

    public void BulletStun()
    {
        if (isServer)
            RpcBulletStun();
    }

    [ClientRpc]
    void RpcBulletStun()
    {
        GetComponent<SpaceshipShooting_FreeFire>().RegisterEffect(RegisterStunEffect);
    }

    public void BoostFireRate()
    {
        GetComponent<SpaceshipShooting_FreeFire>().shootInterval /= fireRateRatio;
        GetComponent<SpaceshipShooting_FreeFire>().curInterval /= fireRateRatio;
        StartCoroutine(BoostingFireRate());
        if (isServer)
            GetComponent<ItemEffect_FreeFire>().BoostFireRateEffect();
    }

    public void BulletSilence()
    {
        if (isServer)
            RpcBulletSilence();
    }

    [ClientRpc]
    void RpcBulletSilence()
    {
        GetComponent<SpaceshipShooting_FreeFire>().RegisterEffect(RegisterSilenceEffect);
    }

    public void BulletPenetration()
    {
        GetComponent<SpaceshipShooting_FreeFire>().isPenetrationBullet = true;
    }

    public void Invisible()
    {
        CmdInvisible();
        if (isServer)
        {
            GetComponent<ItemEffect_FreeFire>().InvisibleEffect();
            StartCoroutine(WaitInvisible());
        }
    }

    [Command]
    void CmdInvisible()
    {
        RpcInvisible();
    }

    [ClientRpc]
    void RpcInvisible()
    {
        if (!isLocalPlayer)
        {
            SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < sprites.Length; i++)
                sprites[i].enabled = false;
            GetComponent<SpaceshipHealth_FreeFire>().HealthBarInstance.GetComponentInChildren<Image>().enabled = false;
            GetComponent<SpaceshipController_FreeFire>().energyBar.GetComponentInChildren<Image>().enabled = false;
            //StartCoroutine(Invisibling());
        }
        else if(isLocalPlayer)
        {
            SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < sprites.Length; i++)
                sprites[i].color = new Color(1F, 1F, 1F, 0.4F);
        }
        StartCoroutine(Invisibling());
    }

    void Healing()
    {
        if (isHealing)
        {
            GetComponent<SpaceshipHealth_FreeFire>().TakeDamage(-1);
            Invoke("Healing", (1f/healingAmount));
        }
    }

    void BoostDamage(GameObject bullet)
    {
        bullet.GetComponent<BulletController_FreeFire>().damage *= boostDmgRatio;
    }

    void RegisterStunEffect(GameObject bullet)
    {
        bullet.GetComponent<BulletController_FreeFire>().effectOnPlayer = StunEffect;
    }

    void RegisterSilenceEffect(GameObject bullet)
    {
        bullet.GetComponent<BulletController_FreeFire>().effectOnPlayer = SilenceEffect;
    }

    void StunEffect(GameObject spaceship)
    {
        
        if (isServer)
        {
            spaceship.GetComponent<ItemEffect_FreeFire>().StunEffect();
            RpcCallingStunEffect(spaceship);
        }
    }

    [ClientRpc]
    void RpcCallingStunEffect(GameObject spaceship)
    {
        spaceship.GetComponent<SpaceshipController_FreeFire>().canMove = false;
        spaceship.GetComponent<SpaceshipShooting_FreeFire>().enabled = false;
        StartCoroutine(Stunning(spaceship));
    }

    void SilenceEffect(GameObject spaceship)
    {
        
        if (isServer)
        {
            spaceship.GetComponent<ItemEffect_FreeFire>().SilenceEffect();
            RpcCallingSilenceEffect(spaceship);
        }
    }

    [ClientRpc]
    void RpcCallingSilenceEffect(GameObject spaceship)
    {
        spaceship.GetComponent<SpaceshipShooting_FreeFire>().enabled = false;
        StartCoroutine(Silencing(spaceship));
    }

    IEnumerator Stunning(GameObject spaceship)
    {
        yield return new WaitForSeconds(bulletStunTime);
        spaceship.GetComponent<SpaceshipController_FreeFire>().canMove = true;
        spaceship.GetComponent<SpaceshipShooting_FreeFire>().enabled = true;
    }

    IEnumerator Silencing(GameObject spaceship)
    {
        yield return new WaitForSeconds(bulletSilenceTime);
        spaceship.GetComponent<SpaceshipShooting_FreeFire>().enabled = true;
    }

    IEnumerator BoostingFireRate()
    {
        yield return new WaitForSeconds(fireRateTime);
        GetComponent<SpaceshipShooting_FreeFire>().shootInterval *= fireRateRatio;
    }

    IEnumerator Invisibling()
    {
        yield return new WaitForSeconds(invisibleTime);
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].enabled = true;
            sprites[i].color = new Color(1F,1F,1F,1F);  
        }
        GetComponent<SpaceshipHealth_FreeFire>().HealthBarInstance.GetComponentInChildren<Image>().enabled = true;
        GetComponent<SpaceshipController_FreeFire>().energyBar.GetComponentInChildren<Image>().enabled = true;
    }

    IEnumerator WaitInvisible()
    {
        yield return new WaitForSeconds(invisibleTime - 0.2f);
        GetComponent<ItemEffect_FreeFire>().VisibleEffect();
    }
}
