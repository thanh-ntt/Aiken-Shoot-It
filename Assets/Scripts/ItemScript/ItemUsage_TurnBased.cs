using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ItemUsage_TurnBased : NetworkBehaviour
{
    [Range(0, 100)] public int healthPotionAmount;
    public float boostDmgRatio;

    public delegate void EffectDelegate(GameObject gameObject);

    public void HealthPotion()
    {
        GetComponent<SpaceshipHealth_TurnBased>().TakeDamage(-healthPotionAmount);
        GetComponent<ItemEffect_TurnBased>().HpCollectEffect();
    }

    public void BoostEnergy()
    {
        GetComponent<SpaceshipController_TurnBased>().BoostEnergy();
        GetComponent<ItemEffect_TurnBased>().BoostEnergyEffect();
    }

    public void BulletBoostDmg()
    {
        GetComponent<SpaceshipShooting_TurnBased>().RegisterEffect(BoostDamage);
        GetComponent<ItemEffect_TurnBased>().DamageBoostEffect();
    }

    public void Invisible()
    {
        if (isLocalPlayer)
        {
            CmdInvisible();
            GetComponent<ItemEffect_TurnBased>().InvisibleEffect();
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
            GetComponent<SpaceshipHealth_TurnBased>().HealthBarInstance.GetComponentInChildren<Image>().enabled = false;
            GetComponent<SpaceshipController_TurnBased>().energyBar.GetComponentInChildren<Image>().enabled = false;
            StartCoroutine(Invisibling());
        }
        else if (isLocalPlayer)
        {
            SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < sprites.Length; i++)
                sprites[i].color = new Color(1F, 1F, 1F, 0.4F);
            Debug.Log("Blurring");
        }
    }

    IEnumerator Invisibling()
    {
        yield return new WaitForSeconds(30f);
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].enabled = true;
            sprites[i].color = new Color(1F, 1F, 1F, 1F);
        }
        GetComponent<SpaceshipHealth_TurnBased>().HealthBarInstance.GetComponentInChildren<Image>().enabled = true;
        GetComponent<SpaceshipController_TurnBased>().energyBar.GetComponentInChildren<Image>().enabled = true;
    }

    IEnumerator WaitInvisible()
    {
        yield return new WaitForSeconds(29.8f);
        GetComponent<ItemEffect_TurnBased>().VisibleEffect();
    }

    public void IncreaseVision()
    {
        GetComponentInChildren<Light>().range *= 3;
        Invoke("DecreaseVision", 1f);
        GetComponent<ItemEffect_TurnBased>().IncreaseVisionEffect();
    }

    void DecreaseVision()
    {
        GetComponentInChildren<Light>().range /= 3;
    }

    void BoostDamage(GameObject bullet)
    {
        bullet.GetComponent<BulletController_TurnBased>().damage *= boostDmgRatio;
    }
}
