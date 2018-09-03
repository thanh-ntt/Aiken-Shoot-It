using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ItemEffect_TurnBased : NetworkBehaviour
{

    public ParticleSystem damageBoostEffect;
    public ParticleSystem hpCollectEffect;
    public ParticleSystem boostEnergyEffect;
    public ParticleSystem increaseVisionEffect;
    public ParticleSystem invisibleStartEffect;
    public ParticleSystem invisibleEndEffect;

    public void DamageBoostEffect()
    {
        if (isLocalPlayer)
            damageBoostEffect.Play();
    }

    public void HpCollectEffect()
    {
        if (isLocalPlayer)
            hpCollectEffect.Play();
    }

    public void BoostEnergyEffect()
    {
        if (isLocalPlayer)
            boostEnergyEffect.Play();
    }

    public void IncreaseVisionEffect()
    {
        if (isLocalPlayer)
            increaseVisionEffect.Play();
    }

    public void InvisibleEffect()
    {
        if (isLocalPlayer)
            invisibleStartEffect.Play();
    }

    public void VisibleEffect()
    {
        if (isLocalPlayer)
            invisibleEndEffect.Play();
    }
}
