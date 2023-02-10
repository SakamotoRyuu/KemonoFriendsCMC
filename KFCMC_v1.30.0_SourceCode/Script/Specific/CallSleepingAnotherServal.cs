using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallSleepingAnotherServal : MonoBehaviour {
    
    public int facilityID;
    public Transform cullingPivotTrans;
    public LineChecker lineChecker;

    private Transform trans;
    private GameObject characterObj;
    private Transform cameraTrans;
    const int conditionProgress = 11;
    const int conditionFriendsID = 31;
    const float invisibleDistance = 50f;
    const float visibleDistance = 15f;

    private void Awake() {
        trans = transform;
        if (GameManager.Instance.save.progress >= conditionProgress) {
            // characterObj = Instantiate(CharacterDatabase.Instance.GetFacility(facilityID), transform);
            characterObj = Instantiate(ItemDatabase.Instance.GetItemPrefab(ItemDatabase.facilityBottom + facilityID), transform);
            Animator anim = characterObj.GetComponent<Animator>();
            if (anim) {
                anim.keepAnimatorControllerStateOnDisable = true;
            }
        }
    }

    private void Start() {
        if (CameraManager.Instance) {
            CameraManager.Instance.SetMainCameraTransform(ref cameraTrans);
        }
    }

    private void Update() {
        if (characterObj) {
            bool exist = CharacterManager.Instance.GetFriendsExist(conditionFriendsID, false);
            if (characterObj.activeSelf == exist) {
                characterObj.SetActive(!exist);
            }
        }
    }

    protected virtual void Culling() {
        if (characterObj && cameraTrans) {
            bool answer = true;
            float sqrDist = (cameraTrans.position - (cullingPivotTrans ? cullingPivotTrans.position : trans.position)).sqrMagnitude;
            if (sqrDist > invisibleDistance * invisibleDistance) {
                answer = false;
            }
            if (answer && sqrDist > visibleDistance * visibleDistance && lineChecker) {
                answer = lineChecker.reach;
            }
            if (characterObj.activeSelf != answer) {
                characterObj.SetActive(answer);
            }
        }
    }

}
