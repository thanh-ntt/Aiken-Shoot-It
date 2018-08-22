using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BulletController_FreeFire : NetworkBehaviour
{

    public float bulletLifeTime;
    public float damage;
    public float reductionRatio;
    public float bulletSpeed;
    public ItemUsage_FreeFire.EffectDelegate effectOnPlayer;

    public GameObject impactEffectPrefab;
    float age;

    void Start()
    {
        gameObject.GetComponent<Rigidbody2D>().velocity = bulletSpeed * transform.right;
    }

    // Bullets are updated by the server
    [ServerCallback]
    void Update()
    {
        age += Time.deltaTime;
        // Destroy on the network
        if (age > bulletLifeTime)
            NetworkServer.Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // Only server can control bullets
        if (!isServer)
            return;
        RpcImpactEffect(transform.position);

        if (other.gameObject.CompareTag("BulletBlock"))
        {
            other.gameObject.GetComponent<BlockBulletController>().ChangeColor();
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            if (effectOnPlayer != null)
                effectOnPlayer(other.gameObject);
            other.gameObject.GetComponent<SpaceshipHealth_FreeFire>().TakeDamage((int)damage);
            Destroy(gameObject);
        }
        damage *= reductionRatio;
    }

    [ClientRpc]
    void RpcImpactEffect(Vector3 position)
    {
        var impactEffect = Instantiate(impactEffectPrefab, position, Quaternion.identity);
        impactEffect.GetComponent<ParticleSystem>().Play();
        Destroy(impactEffect, 1f);
    }
}
