using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateWithFriends : MonoBehaviour {

    [System.Serializable]
    public class RepositionTarget {
        public Transform targetTransform;
        public float yPosition;
    }

    public int[] friendsId;
    public int facilityID;
    public bool notGenerateHomeObjectOnly;
    public bool forceGenerate;
    public bool dontCheckFacilityBook;
    public bool notForFDKaban;
    public RepositionTarget[] repositionTargets;
    public GameObject insteadActivateObj;
    public int overrideFDKabanID;

    protected virtual void InstantiatePrefab() {
        if (facilityID >= 0 && (dontCheckFacilityBook || CharacterDatabase.Instance.CheckFacilityEnabled(facilityID))) {
            // Instantiate(CharacterDatabase.Instance.GetFacility(facilityID), transform);
            GameObject objTemp = Instantiate(ItemDatabase.Instance.GetItemPrefab(ItemDatabase.facilityBottom + facilityID), transform);
            if (!notForFDKaban && PauseController.Instance) {
                PauseController.Instance.SetFacilityObj(overrideFDKabanID > 0 ? overrideFDKabanID : facilityID, objTemp);
            }
        } else {
            if (repositionTargets.Length > 0) {
                for (int i = 0; i < repositionTargets.Length; i++) {
                    if (repositionTargets[i].targetTransform) {
                        Vector3 posTemp = repositionTargets[i].targetTransform.position;
                        posTemp.y = repositionTargets[i].yPosition;
                        repositionTargets[i].targetTransform.position = posTemp;
                    }
                }
            }
            if (insteadActivateObj) {
                insteadActivateObj.SetActive(true);
            }
        }
    }

    protected virtual bool CheckCondition() {
        for (int i = 0; i < friendsId.Length; i++) {
            if (friendsId[i] >= 0 && friendsId[i] < GameManager.friendsMax && GameManager.Instance.save.friends[friendsId[i]] == 0) {
                return false;
            }
        }
        return true;
    }

	protected virtual void Start () {
		if (CharacterManager.Instance != null && (forceGenerate || (GameManager.Instance.save.config[GameManager.Save.configID_GenerateHomeObjects] == 0) == notGenerateHomeObjectOnly)) {
            if (CheckCondition()) {
                InstantiatePrefab();
            }
        }
	}
}
