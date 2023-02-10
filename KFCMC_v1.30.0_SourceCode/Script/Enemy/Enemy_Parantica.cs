using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Parantica : EnemyBase {
    
    public Transform throwFromFollowTarget;
    private int attackSave = -1;
    private bool teleportFlag;
    private int saveTeleportHP;

    private const int effectTeleport = 4;

    protected override void Awake() {
        base.Awake();
        attackedTimeRemainOnRestoreKnockDown = 0.5f;
        attackedTimeRemainOnDamage = 0.8f;
    }

    protected override void SetLevelModifier() {
        dodgeRemain = dodgePower = 5f;
        saveTeleportHP = GetMaxHP();
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (targetTrans && state == State.Attack) {
            throwFromFollowTarget.position = new Vector3(targetTrans.position.x, trans.position.y, targetTrans.position.z);
        }
        if (teleportFlag && (level <= 2 || state != State.Damage)) {
            teleportFlag = false;
        }
        if (targetTrans && searchTarget[0] && state == State.Chase && attackedTimeRemain < 0.1f && GetTargetDistance(true, true, false) > 20f * 20f && Physics.Linecast(targetTrans.position, searchTarget[0].transform.position, fieldLayerMask)) {
            attackedTimeRemain = 0.2f;
        }
    }

    void KnockTeleportReady() {
        if (level >= 3) {
            if (isDamageHeavy || nowHP <= saveTeleportHP - (GetMaxHP() / (level >= 4 ? 3 : 2))) {
                saveTeleportHP = nowHP;
                EmitEffect(effectTeleport);
                teleportFlag = true;
            }
        }
    }

    void KnockTeleportStart() {
        if (teleportFlag) {
            specialMoveDuration = 0f;
            destination = GetEscapeDestination(searchArea[0].GetTargetsAveragePosition(), evadeDistance);
            agent.SetDestination(destination);
            Warp(destination, 0f, 0f);
            teleportFlag = false;
            if (attackedTimeRemain < 1f) {
                attackedTimeRemain = 1f;
            }
        }
    }

    protected override void Attack() {
        base.Attack();
        int attackTemp = Random.Range(0, level >= 4 ? 4 : 3);
        int maxHP = GetMaxHP();
        float intervalPlus = GetAttackInterval(nowHP <= maxHP / 2 ? 0.8f : 1.2f, 0, 0f);
        if (attackTemp == attackSave) {
            attackTemp = Random.Range(0, 3);
        }
        attackSave = attackTemp;
        if (attackTemp <= 2) {
            int airBias = 0;
            if (level >= 2 && Random.Range(0, 100) < 50) {
                airBias = 3;
            }
            AttackBase(attackTemp + airBias, 1f, 1f, 0f, 85f / 60f, 85f / 60f + intervalPlus, 0.5f, 1f, true, 15f);
        } else {
            AttackBase(6, 1f, 1f, 0, 120f / 60f, 120f / 60f + intervalPlus, 0.5f, 1f, true, 15f);
        }
    }

    protected override void KnockLightProcess() {
        base.KnockLightProcess();
        if (!isSuperarmor) {
            SideStep_ConsiderWall(Random.Range(-1, 2), 5f, 0f, false);
        }
    }

    protected override void KnockHeavyProcess() {
        base.KnockHeavyProcess();
        SideStep_ConsiderWall(Random.Range(-1, 2), 5f, 0f, false);
    }

}
