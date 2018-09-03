using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TurnManager : NetworkBehaviour
{
    public static List<NetworkInstanceId> players = new List<NetworkInstanceId>();
    public static NetworkInstanceId curPlayer;

    public static float turnTime = 15.9f;

    static int curPlayerIdx = 0;

    public static void TogglePlayer()
    {
        curPlayer = players[curPlayerIdx % players.Count];
        curPlayerIdx++;
    }

    public static void AddPlayer(NetworkInstanceId id)
    {
        players.Add(id);
        if (players.Count == 1)
        {
            curPlayerIdx = 0;
            TogglePlayer();
        }
    }

    public static void ResetFields()
    {
        curPlayerIdx = 0;
        players = new List<NetworkInstanceId>();
        DeathMatchManager_TurnBased.spaceships = new List<SpaceshipHealth_TurnBased>();
    }
}
