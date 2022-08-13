using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks {
    private string gameVersion = "0.0.1";
    private bool isConnecting;

    [SerializeField] private byte maxPlayersPerRoom = 4;
    [SerializeField] private GameObject controlPanel;
    [SerializeField] private GameObject progressLabel;

    void Awake() {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start() {
        Debug.Log("Game started.");

        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }

    public void Connect() {
        progressLabel.SetActive(true);
        controlPanel.SetActive(false);
        if (PhotonNetwork.IsConnected) {
            PhotonNetwork.JoinRandomRoom();
        } else {
            isConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    public override void OnConnectedToMaster() {
        Debug.Log("OnConnectedToMaster() invoked.");

        if (isConnecting) {
            PhotonNetwork.JoinRandomRoom();
            isConnecting = false;
        }
    }

    public override void OnDisconnected(DisconnectCause cause) {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);

        isConnecting = false;

        Debug.LogWarningFormat("OnDisconnected() invoked. Cause: {0}", cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
        Debug.Log("OnJoinRandomFailed() invoked. No random room available, creating a new one...");
        PhotonNetwork.CreateRoom(null, new RoomOptions{MaxPlayers = maxPlayersPerRoom});
    }

    public override void OnJoinedRoom() {
        Debug.Log("OnJoinedRoom() invoked. We are now in a room.");

        Debug.Log("We load the 'Room for 1' ");

        PhotonNetwork.LoadLevel("Room for 1");
    }
}
