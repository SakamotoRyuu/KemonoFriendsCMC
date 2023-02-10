using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivergeLongBuff_Scale : MonoBehaviour {
    
    public Vector3 longScale;

    private void Awake() {
        if (CharacterManager.Instance && CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Long)) {
            transform.localScale = longScale;
            AttackDetection attackDetection = GetComponent<AttackDetection>();
            if (attackDetection) {
                attackDetection.selectTopDamageRate = true;
            } else {
                AttackDetection[] attackDetectionArray = GetComponentsInChildren<AttackDetection>();
                for (int i = 0; i < attackDetectionArray.Length; i++) {
                    attackDetectionArray[i].selectTopDamageRate = true;
                }
            }
        }
    }

}
