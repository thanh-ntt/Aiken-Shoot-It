using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BlockBulletController : NetworkBehaviour    {

	public void ChangeColor()
    {
        RpcChangeColor();
    }

    [ClientRpc]
    void RpcChangeColor()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
    }
}
