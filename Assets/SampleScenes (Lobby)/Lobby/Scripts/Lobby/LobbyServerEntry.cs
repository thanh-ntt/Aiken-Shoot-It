using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using System.Collections;

namespace Prototype.NetworkLobby
{
    public class LobbyServerEntry : MonoBehaviour 
    {
        public Text serverInfoText;
        public Text slotInfo;
        public Button joinButton;

		public void Populate(MatchInfoSnapshot match, LobbyManager lobbyManager, Color c)
		{
            string RoomModeTag = "NULL";
            int LobbyForGameMode = 0;
            if(match.name.EndsWith("1"))
            {
                LobbyForGameMode = 1;
                RoomModeTag = "FREE FIRE";
            }
            else if(match.name.EndsWith("2"))
            {
                LobbyForGameMode = 2;
                RoomModeTag = "TURN BASE";
            }            
            serverInfoText.text = RoomModeTag + " - " + match.name;
            serverInfoText.text = serverInfoText.text.Remove(serverInfoText.text.Length - 1);

            slotInfo.text = match.currentSize.ToString() + "/" + match.maxSize.ToString(); ;

            NetworkID networkID = match.networkId;
            
            joinButton.onClick.RemoveAllListeners();
            joinButton.onClick.AddListener(() => { JoinMatch(networkID, lobbyManager); });
            if(LobbyForGameMode != FindObjectOfType<LobbyManager>().GameMode)
            {
                joinButton.enabled = false;
                //Destroy(joinButton);
            }
            else
            {
                joinButton.enabled = true;
            }

            GetComponent<Image>().color = c;
        }

        void JoinMatch(NetworkID networkID, LobbyManager lobbyManager)
        {
			lobbyManager.matchMaker.JoinMatch(networkID, "", "", "", 0, 0, lobbyManager.OnMatchJoined);
			lobbyManager.backDelegate = lobbyManager.StopClientClbk;
            lobbyManager._isMatchmaking = true;
            lobbyManager.DisplayIsConnecting();
        }
    }
}