using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DeathMatchManager_TurnBased : NetworkBehaviour
{

    // List of players' health in the match
    public static List<SpaceshipHealth_TurnBased> spaceships = new List<SpaceshipHealth_TurnBased>();

    public static void AddSpaceship(SpaceshipHealth_TurnBased spaceship)
    {
        spaceships.Add(spaceship);
    }

    public static bool RemoveSpaceshipAndCheckWinner(SpaceshipHealth_TurnBased spaceship)
    {
        spaceships.Remove(spaceship);
        Debug.Log("Call RSACW, spaceships.count = " + spaceships.Count);
        if (spaceships.Count == 1)  // 1 tank left --> winner
            return true;
        return false;
    }

    public static SpaceshipHealth_TurnBased GetWinner()
    {
        return spaceships[0];
    }
}
