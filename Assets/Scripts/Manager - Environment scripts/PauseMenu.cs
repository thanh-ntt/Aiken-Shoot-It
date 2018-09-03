using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{ 
    public void LoadMenu()
    {
        //Do sth with the network here
        //Remove player,...
        SceneManager.LoadScene("Introduction");
    }
}       
