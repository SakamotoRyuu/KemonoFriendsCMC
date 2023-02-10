using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckMultiHitIntervalForBuff : MonoBehaviour {
    
    public float multiHitBuffInterval = 0.1f;
    public int multiHitBuffMaxCount;
    AttackDetection attackDetection;
    float multiHitDefaultInterval;
    int multiHitDefaultMaxCount;

    private void Awake() {
        attackDetection = GetComponent<AttackDetection>();
        if (attackDetection) {
            multiHitDefaultInterval = attackDetection.multiHitInterval;
            multiHitDefaultMaxCount = attackDetection.multiHitMaxCount;
            CheckMulti();
        }
    }

    void CheckMulti() {
        if (attackDetection && CharacterManager.Instance) {
            if (CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Multi)) {
                attackDetection.multiHitInterval = multiHitBuffInterval;
                attackDetection.multiHitMaxCount = multiHitBuffMaxCount;
            } else {
                attackDetection.multiHitInterval = multiHitDefaultInterval;
                attackDetection.multiHitMaxCount = multiHitDefaultMaxCount;
            }
        }
    }

    void Update() {
        CheckMulti();
    }
}
