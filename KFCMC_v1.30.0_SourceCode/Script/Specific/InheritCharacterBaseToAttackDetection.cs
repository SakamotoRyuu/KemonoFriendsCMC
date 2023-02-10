using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InheritCharacterBaseToAttackDetection : MonoBehaviour {

    public AttackDetection targetAD;
    public bool autoSetOnAwake = true;

    private void Awake() {
        if (autoSetOnAwake) {
            CharacterBase parentCharacterBase = GetComponentInParent<CharacterBase>();
            if (parentCharacterBase) {
                SetCharacterBase(parentCharacterBase);
            }
        }
    }

    public void SetCharacterBase(CharacterBase parentCharacterBase) {
        if (targetAD) {
            targetAD.SetParentCharacterBase(parentCharacterBase);
        }
    }

}
