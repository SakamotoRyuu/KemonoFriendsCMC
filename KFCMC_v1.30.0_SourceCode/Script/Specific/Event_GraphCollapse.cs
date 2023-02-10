using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_GraphCollapse : MonoBehaviour
{

    [System.Serializable]
    public class CollapseSet {
        public GameObject targetObj;
        public Vector3 offset;
        public bool checkChildCount;
        public GameObject effectPrefab;
        public float scale;
    }

    [System.Serializable]
    public class FriendsTalkSet {
        public int friendsID;
        public string dicKey;
        public bool setFaceEnabled;
        public FriendsBase.FaceName faceName;
    }

    public int conditionProgress;
    public CollapseSet[] collapseSet;
    public GameObject[] activateObj;
    public GameObject[] deactivateObj;
    public Transform[] setOnGroundTransform;
    public FriendsTalkSet[] friendsTalkSet;
    public bool collapsedFlag;

    private void Awake() {
        if (StageManager.Instance) {
            StageManager.Instance.graphCollapseNowFloor = this;
        }
    }

    public bool Collapse(bool effectTalkEnabled = true) {
        bool answer = false;
        if (!collapsedFlag && GameManager.Instance.save.progress >= conditionProgress) {
            collapsedFlag = true;
            if (effectTalkEnabled) {
                for (int i = 0; i < collapseSet.Length; i++) {
                    if (collapseSet[i].targetObj != null && collapseSet[i].targetObj.activeSelf && (!collapseSet[i].checkChildCount || collapseSet[i].targetObj.transform.childCount > 0)) {
                        if (collapseSet[i].effectPrefab) {
                            GameObject effectInstance = Instantiate(collapseSet[i].effectPrefab, collapseSet[i].targetObj.transform.position + collapseSet[i].offset, Quaternion.identity);
                            if (collapseSet[i].scale != 1) {
                                effectInstance.transform.localScale = Vector3.one * collapseSet[i].scale;
                            }
                        }
                        collapseSet[i].targetObj.SetActive(false);
                        answer = true;
                    }
                }
            }
            for (int i = 0; i < deactivateObj.Length; i++) {
                if (deactivateObj[i] != null) {
                    deactivateObj[i].SetActive(false);
                }
            }
            for (int i = 0; i < activateObj.Length; i++) {
                if (activateObj[i] != null) {
                    activateObj[i].SetActive(true);
                }
            }
            if (setOnGroundTransform.Length > 0) {
                StartCoroutine(DelayMethod(1));
            }
        }
        if (effectTalkEnabled && answer && friendsTalkSet.Length > 0) {
            for (int i = 0; i < friendsTalkSet.Length; i++) {
                if (friendsTalkSet[i].friendsID >= 0 && !string.IsNullOrEmpty(friendsTalkSet[i].dicKey) && CharacterManager.Instance.GetFriendsExist(friendsTalkSet[i].friendsID)) {
                    float chatTimer = MessageUI.Instance.GetSpeechAppropriateTime(TextManager.Get(friendsTalkSet[i].dicKey).Length);
                    CharacterManager.Instance.SetSpecialChat(friendsTalkSet[i].dicKey, friendsTalkSet[i].friendsID, chatTimer);
                    if (friendsTalkSet[i].setFaceEnabled) {
                        CharacterManager.Instance.SetFaceSpecificFriend(friendsTalkSet[i].friendsID, friendsTalkSet[i].faceName, chatTimer);
                    }
                }
            }
        }
        return answer;
    }

    private IEnumerator DelayMethod(int delayFrameCount) {
        for (int i = 0; i < delayFrameCount; i++) {
            yield return null;
        }
        if (setOnGroundTransform.Length > 0) {
            Ray ray = new Ray(Vector3.zero, Vector3.down);
            LayerMask fieldLayer = LayerMask.GetMask("Field", "SecondField");
            for (int i = 0; i < setOnGroundTransform.Length; i++) {
                if (setOnGroundTransform[i] != null) {
                    RaycastHit hit;
                    ray.origin = setOnGroundTransform[i].position + Vector3.up * 0.5f;
                    if (Physics.Raycast(ray, out hit, 5f, fieldLayer, QueryTriggerInteraction.Ignore)) {
                        setOnGroundTransform[i].position = hit.point;
                    }
                }
            }
        }
    }

}
