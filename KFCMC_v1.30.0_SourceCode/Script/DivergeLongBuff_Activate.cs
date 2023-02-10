using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivergeLongBuff_Activate : MonoBehaviour {

    public GameObject[] activateObj;
    public GameObject[] deactivateObj;
    public bool isPlayer;


    private void Awake() {
        if (GameManager.Instance.save.config[GameManager.Save.configID_ShowGiraffeBeam] >= (isPlayer ? 1 : 2) && CharacterManager.Instance && CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Long)) {
            for (int i = 0; i < activateObj.Length; i++) {
                if (activateObj[i]) {
                    activateObj[i].SetActive(true);
                }
            }
            for (int i = 0; i < deactivateObj.Length; i++) {
                if (deactivateObj[i]) {
                    deactivateObj[i].SetActive(false);
                }
            }
        }
    }
}
