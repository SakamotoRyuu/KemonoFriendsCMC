using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friends_Penguin : FriendsBase {

    public float attackSpeed = 1.2f;
    public bool isFast = false;
    
    float pppExistTime = 0;
    bool refresh = false;
    const string chatKey_PPP = "TALK_PPP";

    protected override void Awake() {
        base.Awake();
        if (animatorForBattle) {
            if (attackSpeed > 0) {
                if (isFast) {
                    moveCost.attack = 18f / attackSpeed * staminaCostRate;
                } else {
                    moveCost.attack = 33f / attackSpeed * staminaCostRate;
                }
            }
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (!refresh && nowST < GetCost(CostType.Attack)) {
            refresh = true;
            actDistNum = 1;
        } else if (refresh && nowST >= GetMaxST() * staminaBorder) {
            refresh = false;
            actDistNum = 0;
        }
        if (CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.PPP) != 0) {
            if (pppExistTime < 3f) {
                pppExistTime += deltaTimeCache;
                if (pppExistTime >= 3f) {
                    SetChat(chatKey_PPP, 35);
                }
            }
        } else {
            pppExistTime = 0f;
        }
    }

    protected override void Attack() {
        base.Attack();
        float spRate = isSuperman ? 4f / 3f : 1f;
        float spTemp = spRate * attackSpeed;
        int dir = 0;
        float frames = isFast ? 18f : 33f;
        if (isFast) {
            dir = attackProcess;
        } else {
            if (targetTrans) {
                if (Vector3.Cross(trans.TransformDirection(vecForward), targetTrans.position - trans.position).y >= -1) {
                    //Right
                    dir = 0;
                } else {
                    //Left
                    dir = 1;
                }
            }
        }
        if (AttackBase((isFast ? 2 : 0) + dir, 1f, 1f, GetCost(CostType.Attack), frames / 30f / spTemp, frames / 30f / spTemp, 1, spTemp)) {
            S_ParticlePlay(dir);
            if (!refresh) {
                SpecialStep(0.35f, 0.25f / spRate, 4f, 0f, 0f, true, true, EasingType.SineInOut, true);
            }
        }
        attackProcess = (attackProcess + 1) % 2;
    }

}
