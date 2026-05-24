using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 攻撃がヒットしたとき、親キャラクターにそれを通知する
/// </summary>
public class AttackDetectionNoticeToParent : AttackDetection
{
    protected override bool SendDamage(ref DamageDetection damageDetection, ref Vector3 closestPoint, ref Vector3 direction)
    {
        bool hitFlag = base.SendDamage(ref damageDetection, ref closestPoint, ref direction);
        if (hitFlag && parentCBase)
        {
            parentCBase.ReceiveNoticeForHitAttack(damageDetection);
        }
        return hitFlag;
    }
}
