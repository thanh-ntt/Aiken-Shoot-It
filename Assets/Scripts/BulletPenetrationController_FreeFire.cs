using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BulletPenetrationController_FreeFire : NetworkBehaviour
{

    public float bulletLifeTime;
    public float damage;
    public float bulletSpeed;
    public ItemUsage_FreeFire.EffectDelegate effectOnPlayer;

    public ParticleSystem impactEffectPrefab;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isServer)
            return;
        if (collision.gameObject.CompareTag("Player"))
        {
            RpcImpactEffect();
            collision.gameObject.GetComponent<SpaceshipHealth_FreeFire>().TakeDamage((int)damage);
            Destroy(gameObject);
        }
    }

    [ClientRpc]
    void RpcImpactEffect()
    {
        var impactEffect = Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
        impactEffect.Play();
        Destroy(impactEffect, 2f);
    }
}
