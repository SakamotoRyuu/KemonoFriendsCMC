using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDetectionSick : AttackDetection {

    public CharacterBase.SickType sickType;
    public float sickTime = -1;
    public int probability = 0;

    protected override bool SendDamage(ref DamageDetection damageDetection, ref Vector3 closestPoint, ref Vector3 direction) {
        if (base.SendDamage(ref damageDetection, ref closestPoint, ref direction)) {
            if (probability >= 100 || Random.Range(0, 100) < probability) {
                if (probability >= 10000) {
                    damageDetection.ReceiveSick(CharacterBase.SickType.Poison, 10, this);
                    damageDetection.ReceiveSick(CharacterBase.SickType.Fire, 10, this);
                    damageDetection.ReceiveSick(CharacterBase.SickType.Acid, 10, this);
                    damageDetection.ReceiveSick(CharacterBase.SickType.Slow, 10, this);
                } else {
                    damageDetection.ReceiveSick(sickType, sickTime, this);
                }
            }
            return true;
        }
        return false;
    }

}
