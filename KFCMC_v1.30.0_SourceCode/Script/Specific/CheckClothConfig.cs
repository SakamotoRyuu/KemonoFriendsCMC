using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckClothConfig : MonoBehaviour {

    public Cloth cloth;
    public DynamicBone[] dynamicBone;
    public bool isPlayer;
    public bool onStartOnly;

    bool clothFlag;
    bool dynamicBoneFlag;

    void ClothCheck() {
        bool flagTemp = (GameManager.Instance && GameManager.Instance.save.config[GameManager.Save.configID_ClothSimulation] >= (isPlayer ? 1 : 2));
        if (clothFlag != flagTemp) {
            clothFlag = flagTemp;
            cloth.enabled = clothFlag;
        }
    }

    void DynamicBoneCheck() {
        bool flagTemp = (GameManager.Instance && GameManager.Instance.save.config[GameManager.Save.configID_DynamicBone] >= (isPlayer ? 1 : 2));
        if (dynamicBoneFlag != flagTemp) {
            dynamicBoneFlag = flagTemp;
            for (int i = 0; i < dynamicBone.Length; i++) {
                dynamicBone[i].enabled = dynamicBoneFlag;
            }
        }
    }

    void Start() {
        if (cloth) {
            clothFlag = cloth.enabled;
            ClothCheck();
        }
        if (dynamicBone.Length > 0) {
            dynamicBoneFlag = dynamicBone[0].enabled;
            DynamicBoneCheck();
        }
        if (onStartOnly) {
            enabled = false;
        }
    }
    
    void Update() {
        if (cloth) {
            ClothCheck();
        }
        if (dynamicBone.Length > 0) {
            DynamicBoneCheck();
        }
    }
}
