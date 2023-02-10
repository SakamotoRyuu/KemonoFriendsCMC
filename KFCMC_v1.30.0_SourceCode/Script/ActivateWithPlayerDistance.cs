using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateWithPlayerDistance : MonoBehaviour {

    public GameObject activateTarget;
    public float conditionDistance = 65f;
    public bool ignoreY = true;
    public float conditionCameraHeight = 500f;
    public bool checkDefeatEnemyCondition;
    public bool activateOnClimbing;

    Transform trans;
    Transform playerTrans;
    Vector3 playerPos;
    Transform camT;

    private void Awake() {
        trans = transform;
    }

    private void Start() {
        if (CharacterManager.Instance && CharacterManager.Instance.playerTrans) {
            playerTrans = CharacterManager.Instance.playerTrans;
        }
        if (conditionCameraHeight < 100f) {
            if (CameraManager.Instance) {
                CameraManager.Instance.SetMainCameraTransform(ref camT);
            } else {
                Camera mainCamera = Camera.main;
                if (mainCamera) {
                    camT = mainCamera.transform;
                }
            }
        }
    }

    void LateUpdate () {
        if (playerTrans) {
            playerPos = playerTrans.position;
            if (ignoreY) {
                playerPos.y = trans.position.y;
            }
            bool active = (conditionDistance >= 200 || (activateOnClimbing && CharacterManager.Instance && CharacterManager.Instance.isClimbing) || (playerPos - trans.position).sqrMagnitude <= conditionDistance * conditionDistance);
            if (conditionCameraHeight < 100f && active) {
                active = (camT && camT.position.y - trans.position.y <= conditionCameraHeight);
            }
            if (checkDefeatEnemyCondition && StageManager.Instance && StageManager.Instance.dungeonController && StageManager.Instance.dungeonController.GetDefeatMissionCompleted() == false) {
                active = false;
            }
            if (activateTarget && activateTarget.activeSelf != active) {
                activateTarget.SetActive(active);
            }
        }
	}
}
