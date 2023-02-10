using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDetectionMustKnock : AttackDetectionPlayer {

    protected override bool SendDamage(ref DamageDetection damageDetection, ref Vector3 closestPoint, ref Vector3 direction) {
        bool answer = false;
        CharacterBase damageCBase = damageDetection.GetCharacterBase();
        if (damageCBase && (!damageCBase.isBoss || (damageCBase.mustKnockEffectiveEnabled && damageDetection.colorType == CharacterBase.damageColor_Effective))) {
            damageCBase.mustKnockBackFlag = true;
        }
        answer = base.SendDamage(ref damageDetection, ref closestPoint, ref direction);
        if (damageCBase) {
            damageCBase.mustKnockBackFlag = false;
        }
        return answer;
    }

}
