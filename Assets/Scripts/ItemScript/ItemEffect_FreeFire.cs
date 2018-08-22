using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ItemEffect_FreeFire : NetworkBehaviour
{ 
    public ParticleSystem damageBoost;
    public ParticleSystem stunEffect;
    public ParticleSystem boostFireRateEffect;
    public ParticleSystem silenceEffect;
    public ParticleSystem hpCollectEffect;
    public ParticleSystem invisibleStartEffect;
    public ParticleSystem invisibleEndEffect;

    public void DamageBoostEffect()
    {
        RpcDamageBoostEffect();
    }

    public void StunEffect()
    {
        RpcStunEffect();
    }

    public void BoostFireRateEffect()
    {
        RpcBoostFireRateEffect();
    }

    public void SilenceEffect()
    {
        RpcSilenceEffect();
    }

    public void HpCollectEffect()
    {
        RpcHpCollectEffect();
    }

    public void InvisibleEffect()
    {
        RpcInvisibleEffect();
    }

    public void VisibleEffect()
    {
        RpcVisibleEffect();
    }

    [ClientRpc]
    void RpcDamageBoostEffect()
    {
        damageBoost.Play();
    }

    [ClientRpc]
    void RpcStunEffect()
    {
        stunEffect.Play();
    }

    [ClientRpc]
    void RpcBoostFireRateEffect()
    {
        boostFireRateEffect.Play();
    }

    [ClientRpc]
    void RpcSilenceEffect()
    {
        silenceEffect.Play();
    }

    [ClientRpc]
    void RpcHpCollectEffect()
    {
        hpCollectEffect.Play();
    }

    [ClientRpc]
    void RpcInvisibleEffect()
    {
        invisibleStartEffect.Play();
    }

    [ClientRpc]
    void RpcVisibleEffect()
    {
        invisibleEndEffect.Play();
    }
}
