using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable {
    [SerializeField]
    private GameObject beams;

    [SerializeField]
    public GameObject playerUIPrefab;

    public float Health = 1f;

    public static GameObject localPlayerInstance;

    bool isFiring;

    void Awake() {
        if (beams == null) {
            Debug.LogError("<Color=Red><a>Missing</a></Color> Beams Reference.", this);
        } else {
            beams.SetActive(false);
        }

        if (photonView.IsMine) {
            PlayerManager.localPlayerInstance = this.gameObject;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    void Start() {
        CameraWork cameraWork = this.gameObject.GetComponent<CameraWork>();

        if (cameraWork != null) {
            if (photonView.IsMine) {
                cameraWork.OnStartFollowing();
            }
        } else {
            Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
        }

        #if UNITY_5_4_OR_NEWER
        // Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        #endif

        if (playerUIPrefab != null) {
            GameObject uiGO = Instantiate(playerUIPrefab);
            uiGO.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        } else {
            Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
        }
    }

    void Update() {
        if (photonView.IsMine) {
            ProcessInputs();
        }

        if (Health <= 0f) {
            GameManager.Instance.LeaveRoom();
        }

        if (beams != null && isFiring != beams.activeInHierarchy) {
            beams.SetActive(isFiring);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (!photonView.IsMine) {
            return;
        }

        if (!other.name.Contains("Beam")) {
            return;
        }

        Health -= 0.1f;
    }

    void OnTriggerStay(Collider other) {
        if (!photonView.IsMine) {
            return;
        }

        if (!other.name.Contains("Beam")) {
            return;
        }

        Health -= 0.1f * Time.deltaTime;
    }

    #if !UNITY_5_4_OR_NEWER
    /// <summary>See CalledOnLevelWasLoaded. Outdated in Unity 5.4.</summary>
    void OnLevelWasLoaded(int level) {
        this.CalledOnLevelWasLoaded(level);
    }
    #endif


    void CalledOnLevelWasLoaded(int level) {
        // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
        if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
        {
            transform.position = new Vector3(0f, 5f, 0f);
        }

        GameObject uiGO = Instantiate(playerUIPrefab);
        uiGO.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext(isFiring);
            stream.SendNext(Health);
        } else {
            this.isFiring = (bool)stream.ReceiveNext();
            this.Health = (float)stream.ReceiveNext();
        }
    }

    #if UNITY_5_4_OR_NEWER
    public override void OnDisable() {
        // Always call the base to remove callbacks
        base.OnDisable ();
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    #endif

    private void ProcessInputs() {
        if (Input.GetButtonDown("Fire1")) {
            if (!isFiring) isFiring = true;
        }

        if (Input.GetButtonUp("Fire1")) {
            if (isFiring) isFiring = false;
        }
    }

    #if UNITY_5_4_OR_NEWER
    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode) {
        this.CalledOnLevelWasLoaded(scene.buildIndex);
    }
    #endif


}