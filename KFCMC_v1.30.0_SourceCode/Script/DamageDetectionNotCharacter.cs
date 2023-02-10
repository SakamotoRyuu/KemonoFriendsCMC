using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDetectionNotCharacter : DamageDetection {

    protected override void Awake() {
        isNotCharacter = true;
    }

    protected override void Start() {
        if (CharacterManager.Instance) {
            characterId = CharacterManager.Instance.IssueID();
        }
    }

    public override bool ReceiveDamage(ref Vector3 effectPosition, int damage, float knockAmount, ref Vector3 knockVector, AttackDetection attackDetection = null, bool penetrate = false, int overrideColorType = -1) {
        if (attackDetection != null && attackDetection.parentCBase == CharacterManager.Instance.pCon && overrideColorType < 0) {
            if (hitEffect != null) {
                Instantiate(hitEffect[hitEffectNum], effectPosition, Quaternion.identity);
            }
            WorkDamage(damage, knockAmount * knockedRate, knockVector);
        }
        return true;
    }

    public virtual void WorkDamage(int damage, float knockAmount, Vector3 knockVector) {
    }

}
