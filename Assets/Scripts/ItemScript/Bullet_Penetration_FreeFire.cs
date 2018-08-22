using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet_Penetration_FreeFire : NetworkBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<ItemUsage_FreeFire>().BulletPenetration();
            NetworkServer.Destroy(gameObject);
        }
    }
}
