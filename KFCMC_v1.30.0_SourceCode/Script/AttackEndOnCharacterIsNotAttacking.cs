using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEndOnCharacterIsNotAttacking : MonoBehaviour {

    public AttackDetection attackDetection;
    CharacterBase cBase;

    void Awake() {
        cBase = GetComponentInParent<CharacterBase>();
    }

    private void Start() {
        if (!cBase) {
            cBase = GetComponentInParent<CharacterBase>();
        }
    }

    void Update() {
        if (cBase && attackDetection && attackDetection.attackEnabled && !cBase.IsAttacking()) {
            attackDetection.DetectionEnd();
        }
    }

}
