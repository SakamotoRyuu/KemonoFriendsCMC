using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friends_CampoFlicker : FriendsBase {

    Vector3 playerPosTemp;
    
    protected override void Awake() {
        base.Awake();
        if (animatorForBattle) {
            attackedTimeRemainOnDamage = 2f;
            moveCost.attack = 100;
            belligerent = false;
            mudToWalk = true;
            separateFearRate = 8f;
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        float sqrPlayerDist = 0f;
        if (playerTrans) {
            playerPosTemp = playerTrans.position;
            playerPosTemp.y = trans.position.y;
            sqrPlayerDist = (playerPosTemp - trans.position).sqrMagnitude;
        }
        if (nowST >= moveCost.attack * 0.7f) {
            if (sqrPlayerDist <= 3f * 3f) {
                actDistNum = 2;
            } else {
                actDistNum = 1;
            }
        }
        if (nowST <= moveCost.attack * 0.4f) {
            actDistNum = 0;
        }
    }

    public override void ChangeActionDistance(bool isBossBattle) {
        if (!isBossBattle) {
            agentActionDistance[2].attack.y = 20f;
        } else {
            agentActionDistance[2].attack.y = 30f;
        }
    }

    public override void SetFieldBuff(FieldBuffType type) {
        if (fieldBuffRemainTime.Length > (int)type) {
            fieldBuffRemainTime[(int)type] = 0f;
        }
    }

    protected override void Attack() {
        base.Attack();
        float spRate = isSuperman ? 4f / 3f : 1f;
        AttackBase(0, 1, 1, GetCost(CostType.Attack), 67f / 30f / spRate, (67f / 30f + 2f) / spRate, 0f, spRate, false);
    }
}
