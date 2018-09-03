using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

    public void LoadFreeFire()
    {
        SceneManager.LoadScene("NetworkLobby (Free Fire)", LoadSceneMode.Single);
    }

    public void LoadTurnBase()
    {
        SceneManager.LoadScene("NetworkLobby (Turn-based)", LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
