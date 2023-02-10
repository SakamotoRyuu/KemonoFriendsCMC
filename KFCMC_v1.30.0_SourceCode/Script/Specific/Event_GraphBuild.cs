using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_GraphBuild : MonoBehaviour {

    [System.Serializable]
    public class EffectSet {
        public GameObject prefab;
        public Vector3 position;
        public float scale;
    }
    
    public EffectSet effectSet;
    public GameObject[] activateObj;
    public GameObject[] deactivateObj;
    public bool useCeil;
    public float ceilHeight;
    public EffectSet recollapseEffect;

    [System.NonSerialized]
    public bool usedFlag;
    [System.NonSerialized]
    public bool rebreakedFlag;

    const float characterHeight = 1.4f;

    private void Awake() {
        if (StageManager.Instance) {
            StageManager.Instance.graphBuildNowFloor = this;
        }
    }

    public bool Execute() {
        bool answer = false;
        if (!usedFlag) {
            usedFlag = true;
            answer = true;
            if (effectSet.prefab) {
                GameObject effectInstance = Instantiate(effectSet.prefab, effectSet.position, Quaternion.identity, StageManager.Instance && StageManager.Instance.dungeonController ? StageManager.Instance.dungeonController.transform : null);
                if (effectSet.scale != 1) {
                    effectInstance.transform.localScale = Vector3.one * effectSet.scale;
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
            if (useCeil) {
                CharacterManager.Instance.SetPositionHeightMaxAll(ceilHeight - characterHeight);
            }
        }
        return answer;
    }

    public void Recollapse() {
        if (usedFlag) {
            usedFlag = false;
            for (int i = 0; i < activateObj.Length; i++) {
                if (activateObj[i] != null) {
                    activateObj[i].SetActive(false);
                }
            }
            if (recollapseEffect.prefab) {
                Instantiate(recollapseEffect.prefab, recollapseEffect.position, Quaternion.identity, StageManager.Instance && StageManager.Instance.dungeonController ? StageManager.Instance.dungeonController.transform : null);
            }
        }
    }

}
