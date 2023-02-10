using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombTrigger_CheckStay : BombTrigger {

    private void OnTriggerStay(Collider other) {
        if (!entered && duration >= triggerDelay && (string.IsNullOrEmpty(targetTag) || other.CompareTag(targetTag)) && (!restrictLayerEnabled || ((1 << other.gameObject.layer) & conditionLayer) != 0)) {
            DamageDetection ddTemp = other.GetComponent<DamageDetection>();
            if (ddTemp && !ddTemp.isNotCharacter) {
                Burst();
            }
        }
    }

}
