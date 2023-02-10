using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friends_Alpaca : FriendsBase {

    public Transform spitTrans;

    static readonly Vector3 defaultEuler = new Vector3(353f, 0f, 0f);
    
    protected override void Awake() {
        base.Awake();
        if (animatorForBattle) {
            moveCost.attack = 80;
            belligerent = false;
            separateFearRate = 8f;
        }
    }

    public override void ChangeActionDistance(bool isBossBattle) {
        if (!isBossBattle) {
            agentActionDistance[0].attack.y = 15f;
        } else {
            agentActionDistance[0].attack.y = 30f;
        }
    }

    protected override void Update() {
        base.Update();
        if (attackedTimeRemain <= 0 && JudgeStamina(GetCost(CostType.Attack)) && target) {
            EnemyBase targetEBase = target.GetComponentInParent<EnemyBase>();
            if (targetEBase && !targetEBase.GetSick(SickType.Acid)) {
                actDistNum = 0;
            } else {
                actDistNum = 1;
            }
        } else {
            actDistNum = 1;
        }
    }

    protected override void Attack() {
        base.Attack();
        float spRate = isSuperman ? 4f / 3f : 1f;
        AttackBase(0, 1f, 1f, GetCost(CostType.Attack), 1f / spRate, (1f + 1.5f) / spRate, 0.5f, 1f, true, 15f);
    }

    public override void AttackStart(int index) {
        Vector3 spitEuler = defaultEuler;
        if (targetTrans) {
            spitEuler.x = Mathf.Clamp(353f - ((GetTargetDistance(false, false, true) - 3f) * 2f), 346f, 353f);
        }
        spitTrans.localEulerAngles = spitEuler;
        base.AttackStart(index);
    }

}
