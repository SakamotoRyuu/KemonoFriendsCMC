using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friends_Ibis : FriendsBase {

    protected override void Awake() {
        base.Awake();
        if (animatorForBattle) {
            attackedTimeRemainOnDamage = 3.5f;
            moveCost.attack = 100;
            belligerent = false;
            mudToWalk = true;
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

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (CharacterManager.Instance && CharacterManager.Instance.GetFriendsInjured(friendsId)) {
            actDistNum = 0;
        } else {
            actDistNum = 1;
        }
    }

    protected override void Attack() {
        base.Attack();
        float spRate = 0.8f;
        AttackBase(0, 1, 1, GetCost(CostType.Attack), 50f / 30f / spRate, (50f / 30f + 1.6f) / spRate, 0f, spRate, false); 
    }
    
}
