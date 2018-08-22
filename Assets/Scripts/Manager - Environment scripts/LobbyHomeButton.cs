using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;


public class LobbyHomeButton : NetworkBehaviour {
    //public Button HomeButton;
    public GameObject Lobby;

    void Start()
    {
        Lobby = GameObject.Find("LobbyManager");
    }

    // Use this for initialization
    public void ExitToStartScene ()
    {
        //Need to clean sth here
        Network.Disconnect();
        Delay();

        Destroy(Lobby);
        Debug.Log("Destroy Lobby");
        Delay();

        Destroy(NetworkManager.singleton);
        Debug.Log("Destroy NetworkManager");
        Delay();

        SceneManager.LoadScene("Introduction",LoadSceneMode.Single);
        Debug.Log("To menu");
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.3f);
    }
}
