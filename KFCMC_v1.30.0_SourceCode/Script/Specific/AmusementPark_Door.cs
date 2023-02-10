using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AmusementPark_Door : MonoBehaviour {

    public Transform doorTransform;
    public Collider[] rideTrigger;
    public Vector3 closeEuler;
    public Vector3 openEuler;
    public float moveTime;
    public bool isOpened;

    private int pauseWait;
    private bool entering;
    private float moveTimeRemain;
    private const string targetTag = "ItemGetter";

    private void Update() {
        if (PauseController.Instance) {
            if (PauseController.Instance.pauseGame) {
                pauseWait = 2;
            } else if (pauseWait > 0) {
                pauseWait--;
            }
            if (PauseController.Instance.returnToLibraryProcessing) {
                enabled = false;
            }
            if (moveTimeRemain > 0f) {
                moveTimeRemain -= Time.deltaTime;
            }
            if (entering && moveTimeRemain <= 0f && pauseWait <= 0 && PauseController.Instance.pauseEnabled && GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit)) {
                ControlDoor(!isOpened);
            }
        }
    }

    public void ControlDoor(bool toOpen) {
        if (toOpen != isOpened && moveTimeRemain <= 0f) {
            doorTransform.DOLocalRotate(toOpen ? openEuler : closeEuler, moveTime);
            isOpened = toOpen;
            moveTimeRemain = moveTime + 0.025f;
            for (int i = 0; i < rideTrigger.Length; i++) {
                if (rideTrigger[i]) {
                    rideTrigger[i].enabled = toOpen;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(targetTag)) {
            entering = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag(targetTag)) {
            entering = false;
        }
    }

}
