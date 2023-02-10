using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XftWeapon;

public class Friends_RedFox : FriendsBase {
    
    public XWeaponTrail jumpTrail;

    bool jumpTrailEnabled = false;
    bool accurateAimFlag;

    protected override void Start() {
        base.Start();
        if (animatorForBattle) {
            jumpTrail.Init();
            jumpTrail.Deactivate();
            jumpTrailEnabled = false;
            // moveCost.attack = 16f;
            moveCost.attack = 62f * staminaCostRate;
            specialMoveDirectionAdjustEnabled = false;
        }
    }

    public override void SetForItem() {
        jumpTrail.gameObject.SetActive(false);
        base.SetForItem();
    }

    void FoxJumpStart() {
        if (enabled) {
            LockonEnd();
            if (accurateAimFlag) {
                PerfectLockon();
            }
            jumpTrail.Activate();
            jumpTrailEnabled = true;
            EmitEffect(accurateAimFlag ? 1 : 0);
            SetSpecialMove(trans.TransformDirection(vecForward), attackType >= 4 ? 6f : attackType == 3 ? 7f : 8f, 28f / 30f, EasingType.Linear);
        }
    }

    void FoxJumpEnd() {
        if (enabled) {
            if (jumpTrailEnabled) {
                jumpTrail.StopSmoothly(0.1f);
                jumpTrailEnabled = false;
            }
            SuperarmorEnd();
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (state == State.Attack && accurateAimFlag && specialMoveDuration > 0f && targetTrans) {
            float distTemp = GetTargetDistance(false, true, false);
            if (distTemp < 0.05f) {
                specialMoveDirection = vecZero;
            } else {
                specialMoveDirection = GetTargetVector(true, true);
                if (distTemp < 0.125f) {
                    specialMoveDirection *= distTemp * 8f;
                }
            }
        }
        if (state != State.Attack) {
            if (jumpTrailEnabled) {
                FoxJumpEnd();
            }
        }
        if (actDistNum == 0 && !JudgeStamina(GetCost(CostType.Attack))) {
            actDistNum = 1;
        } else if (actDistNum == 1 && nowST >= GetMaxST() * staminaBorder) {
            actDistNum = 0;
        }
    }

    protected override void Attack() {
        base.Attack();
        int attackTemp = 0;
        accurateAimFlag = (Random.Range(0, 100) < (isSuperman ? 50 : 25));
        if (targetTrans) {
            if (targetTrans.position.y > trans.position.y + 2.15f) {
                attackTemp = 4;
            } else if (targetTrans.position.y > trans.position.y + 1.7f) {
                attackTemp = 3;
            } else if (targetTrans.position.y > trans.position.y + 1.35f) {
                attackTemp = 2;
            } else if (targetTrans.position.y > trans.position.y + 0.9f) {
                attackTemp = 1;
            }
        }
        if (AttackBase(attackTemp, 1, 1, GetCost(CostType.Attack), 62f / 30f + (isSuperman ? -0.3f : 0f), 62f / 30f + (isSuperman ? 0.2f : 0.5f), 0f, 1f, true, accurateAimFlag ? 20 : 15)) {
            S_ParticlePlay(0);
            if (accurateAimFlag) {
                SuperarmorStart();
            }
        }
    }

}
