using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friends_Margay : FriendsBase {
    
    protected override void Awake() {
        base.Awake();
        if (animatorForBattle) {
            attackedTimeRemainOnDamage = 70f / 30f;
            moveCost.attack = 70;
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

    protected override void Attack() {
        base.Attack();
        float spRate = (isSuperman ? 4f / 3f : 1f) * 0.8f;
        AttackBase(0, 1, 1, GetCost(CostType.Attack), 70f / 30f / spRate, (70f / 30f + 1.2f) / spRate, 0f, spRate, false);
    }
}
