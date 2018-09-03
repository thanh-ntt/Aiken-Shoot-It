using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DeathMatchManager_FreeFire : NetworkBehaviour
{

    // List of players' health in the match
    static List<SpaceshipHealth_FreeFire> spaceships = new List<SpaceshipHealth_FreeFire>();

    public static void AddSpaceship(SpaceshipHealth_FreeFire spaceship)
    {
        spaceships.Add(spaceship);
    }

    public static bool RemoveSpaceshipAndCheckWinner(SpaceshipHealth_FreeFire spaceship)
    {
        spaceships.Remove(spaceship);

        if (spaceships.Count == 1)  // 1 tank left --> winner
            return true;
        return false;
    }

    public static SpaceshipHealth_FreeFire GetWinner()
    {
        if (spaceships.Count != 1)
            return null;
        SpaceshipHealth_FreeFire winner = spaceships[0];

        // Reset this script
        spaceships = new List<SpaceshipHealth_FreeFire>();

        return winner;
    }
}
