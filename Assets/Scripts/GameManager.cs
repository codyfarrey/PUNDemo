using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks {

    public static GameManager Instance;

    public GameObject playerPrefab;

    void Start() {
        Instance = this;

        if (playerPrefab == null) {
             Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'",this);
        } else {
            if (PlayerManager.localPlayerInstance == null) {
                 Debug.LogFormat("We are Instantiating LocalPlayer from {0}", Application.loadedLevelName);
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f,5f,0f), Quaternion.identity, 0);
            } else {
                 Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }
           
        }
    }

    public void LeaveRoom() {
        PhotonNetwork.LeaveRoom();
    }

    public void LoadArena() {
        if (!PhotonNetwork.IsMasterClient) {
            Debug.LogError("Trying to load level but we are not the master client!");
        }

        Debug.LogFormat("Loading Level: Room for {0}...", PhotonNetwork.CurrentRoom.PlayerCount);
        PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public override void OnLeftRoom() {
        Debug.Log("OnLeftRoom() invoked. Leaving room... Going back to Launcher scene.");
        SceneManager.LoadScene(0);
    }
    
    public override void OnPlayerEnteredRoom(Player other) {
        Debug.LogFormat("OnPlayerEnteredRoom(): {0}", other.NickName);

        if (PhotonNetwork.IsMasterClient) {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);

            LoadArena();
        }
    }

    public override void OnPlayerLeftRoom(Player other) {
        Debug.LogFormat("OnPlayerLeftRoom(): {0}", other.NickName);

        if (PhotonNetwork.IsMasterClient) {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);

            LoadArena();
        }
    }


}