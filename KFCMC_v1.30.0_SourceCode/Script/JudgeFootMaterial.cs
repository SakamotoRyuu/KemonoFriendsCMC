using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgeFootMaterial : MonoBehaviour {
    
    public int materialType = -1;

    private int fixingIndex = -1;
    private GameObject[] fixingObj = new GameObject[8];

    private void OnTriggerEnter(Collider other) {
        if (FootstepManager.Instance && other.isTrigger) {
            int mTemp = FootstepManager.Instance.JudgeType(other.gameObject);
            if (mTemp >= 0) {
                bool exist = false;
                for (int i = 0; i < fixingObj.Length; i++) {
                    if (fixingObj[i] == other.gameObject) {
                        exist = true;
                        break;
                    }
                }
                if (!exist) {
                    for (int i = 0; i < fixingObj.Length; i++) {
                        if (fixingObj[i] == null) {
                            fixingObj[i] = other.gameObject;
                            fixingIndex = i;
                            materialType = mTemp;
                            break;
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (FootstepManager.Instance && other.isTrigger) {
            int existIndex = -1;
            for (int i = 0; i < fixingObj.Length; i++) {
                if (fixingObj[i] == other.gameObject) {
                    fixingObj[i] = null;
                    existIndex = i;
                    break;
                }
            }
            if (existIndex >= 0 && existIndex == fixingIndex) {
                int mTemp = -1;
                for (int i = fixingObj.Length - 1; i >= 0; i--) {
                    if (fixingObj[i] != null) {
                        mTemp = FootstepManager.Instance.JudgeType(fixingObj[i]);
                        if (mTemp >= 0) {
                            fixingIndex = i;
                            materialType = mTemp;
                            break;
                        }
                    }
                }
                if (mTemp < 0) {
                    fixingIndex = -1;
                    materialType = -1;
                }
            }            
        }
    }

    private void Update() {
        if (fixingIndex >= 0 && fixingObj[fixingIndex] == null) {
            int mTemp = -1;
            for (int i = fixingObj.Length - 1; i >= 0; i--) {
                if (fixingObj[i] != null) {
                    mTemp = FootstepManager.Instance.JudgeType(fixingObj[i]);
                    if (mTemp >= 0) {
                        fixingIndex = i;
                        materialType = mTemp;
                        break;
                    }
                }
            }
            if (mTemp < 0) {
                fixingIndex = -1;
                materialType = -1;
            }
        }
    }

    public void ResetFixing() {
        for (int i = 0; i < fixingObj.Length; i++) {
            fixingObj[i] = null;
        }
        fixingIndex = -1;
        materialType = -1;
    }
}
