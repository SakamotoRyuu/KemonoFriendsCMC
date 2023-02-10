using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class Event_SafePack : MonoBehaviour
{

    public GameManager.SecretType checkSecretType;
    public int friendsID = 1;
    public string dicKey;
    public GameObject balloonPrefab;
    public Vector3 balloonOffset;
    public GameObject activateTarget;
    public GameManager.SecretType newSecretType;

    bool secretCondition;
    bool opened;
    bool touched;
    int pauseWait;
    GameObject balloonInstance;
    Animator anim;
    AudioSource aSource;
    GameObject mapChip;
    bool actionTextEnabled;

    private void Start() {
        if (GameManager.Instance.GetSecret(checkSecretType)) {
            secretCondition = true;
            anim = GetComponent<Animator>();
            aSource = GetComponent<AudioSource>();
        } else {
            enabled = false;
        }
        mapChip = Instantiate(MapDatabase.Instance.prefab[MapDatabase.other], transform);
    }

    private void Update() {
        if (CharacterManager.Instance && PauseController.Instance) {
            if (PauseController.Instance.pauseGame) {
                pauseWait = 2;
            } else if (pauseWait > 0) {
                pauseWait--;
            }
            if (secretCondition && !opened && CharacterManager.Instance.GetFriendsExist(1, true)) {
                if (!balloonInstance) {
                    balloonInstance = Instantiate(balloonPrefab, transform.position + balloonOffset, transform.rotation, transform);
                }
                if (balloonInstance && !balloonInstance.activeSelf) {
                    balloonInstance.SetActive(true);
                }
                if (touched && pauseWait <= 0 && PauseController.Instance.pauseEnabled && GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit)) {
                    opened = true;
                    GameManager.Instance.SetSecret(newSecretType);
                    if (anim) {
                        anim.SetTrigger("Open");
                    }
                    if (aSource) {
                        aSource.Play();
                    }
                    if (activateTarget) {
                        activateTarget.SetActive(true);
                    }
                    if (mapChip) {
                        mapChip.SetActive(false);
                    }
                    if (!string.IsNullOrEmpty(dicKey)) {
                        CharacterManager.Instance.SetSpecialChat(dicKey, friendsID, -1);
                    }
                }
            } else if (balloonInstance && balloonInstance.activeSelf) {
                balloonInstance.SetActive(false);
            }

            bool actionTemp = (secretCondition && !opened && touched && pauseWait <= 0 && PauseController.Instance.pauseEnabled && CharacterManager.Instance.GetFriendsExist(1, true));
            if (actionTextEnabled != actionTemp) {
                CharacterManager.Instance.SetActionType(actionTemp ? CharacterManager.ActionType.Search : CharacterManager.ActionType.None, gameObject);
                actionTextEnabled = actionTemp;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("ItemGetter")) {
            touched = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("ItemGetter")) {
            touched = false;
        }
    }

}
