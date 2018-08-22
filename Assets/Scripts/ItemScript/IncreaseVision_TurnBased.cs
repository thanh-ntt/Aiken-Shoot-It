using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class IncreaseVision_TurnBased : NetworkBehaviour {

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<ItemUsage_TurnBased>().IncreaseVision();
            NetworkServer.Destroy(gameObject);
        }
    }
}
