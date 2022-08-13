using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class PlayerAnimatorManager : MonoBehaviourPun {
    private Animator animator;

    [SerializeField] private float directionDampTime = 0.25f;

    void Start() {
        animator = GetComponent<Animator>();
        if (!animator) {
            Debug.LogError("PlayerAnimatorManager is missing Animator component.", this);
        }
    }

    void Update() {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true) {
            return;
        }
        
        if (!animator) return;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Base Layer.Run")) {
            if (Input.GetButtonDown("Fire2")) {
                animator.SetTrigger("Jump");
            }
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (v < 0) v = 0;
        animator.SetFloat("Speed", h * h + v * v);
        animator.SetFloat("Direction", h, directionDampTime, Time.deltaTime);
    }
}