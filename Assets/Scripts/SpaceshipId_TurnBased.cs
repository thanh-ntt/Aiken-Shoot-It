using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpaceshipId_TurnBased : NetworkBehaviour
{
    private NetworkInstanceId playerId;
    private bool isEnabled; // Avoid multiple Rpc calls

    // Use this for initialization
    void Start()
    {
        isEnabled = false;
        playerId = GetComponent<NetworkIdentity>().netId;

        if (isServer)
        {
            if (TurnManager.players.Count == 2)
                TurnManager.ResetFields();
            TurnManager.AddPlayer(playerId);
            RpcDisablePlayer();
        }
    }

    // Only Server are able to call RPC Enable/Disable player
    [ServerCallback]
    void Update()
    {
        if (TurnManager.curPlayer.Equals(playerId) && !isEnabled)
            RpcEnablePlayer();
        else if (!TurnManager.curPlayer.Equals(playerId) && isEnabled)
            RpcDisablePlayer();
    }

    [ClientRpc]
    void RpcEnablePlayer()
    {
        if (isLocalPlayer)
        {
            gameObject.GetComponent<SpaceshipController_TurnBased>().EnableMoving();
            gameObject.GetComponent<SpaceshipShooting_TurnBased>().EnableShooting();
        }
        isEnabled = true;
    }

    [ClientRpc]
    void RpcDisablePlayer()
    {
        if (isLocalPlayer)
        {
            gameObject.GetComponent<SpaceshipController_TurnBased>().canMove = false;
            gameObject.GetComponent<SpaceshipShooting_TurnBased>().isTurn = false;
        }
        isEnabled = false;
    }
}
