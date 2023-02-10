using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friends_Owl : FriendsBase {

    public float attackSpeed = 1f;
    public float dodgeRecovery;

    string[] chatKey_Dodge = new string[2];
    float defaultCConHeight;
    float defaultCConStepOffset;

    protected override void Awake() {
        base.Awake();
        if (animatorForBattle) {
            dodgeDistance = 6f;
            moveCost.attack = 16f / attackSpeed * staminaCostRate;
            // dodgeRecovery = (1f / attackSpeed) * 0.16f;
            mudToWalk = true;
            defaultCConHeight = cCon.height;
            defaultCConStepOffset = cCon.stepOffset;
        }
    }

    protected override void ChatKeyInit() {
        base.ChatKeyInit();
        chatKey_Dodge[0] = StringUtils.Format("{0}{1}_DODGE_00", talkHeader, talkName);
        chatKey_Dodge[1] = StringUtils.Format("{0}{1}_DODGE_01", talkHeader, talkName);
    }

    protected override void Attack() {
        base.Attack();
        if (attackSpeed <= 0f) {
            attackSpeed = 1f;
        }
        float spRate = (isSuperman ? 4f / 3f : 1f) * attackSpeed;
        float stiffTimeTemp = 16f / 30f / spRate;
        if (AttackBase(0, 1f, 1f, GetCost(CostType.Attack), stiffTimeTemp, stiffTimeTemp, 0f, spRate, true, 100f)) {
            CommonLockon();
            lockonRotSpeed = 10f;
            S_ParticlePlay(0);
            agent.enabled = false;
            cCon.height = 0.5f;
            cCon.stepOffset = 0.4f;
            gravityZeroTimeRemain = stiffTimeTemp;
            Vector3 targetPos = trans.position;
            float dist = 5f / attackSpeed;
            if (targetTrans) {
                targetPos = targetTrans.position;
                targetPos.y -= Random.Range(0.6f, 0.8f);
                if (GetTargetDistance(true, true, true) <= 1f) {
                    Vector2 randCircle = Random.insideUnitCircle * targetRadius;
                    targetPos.x += randCircle.x;
                    targetPos.z += randCircle.y;
                }
                dist = Mathf.Clamp(Vector3.Distance(trans.position, targetPos) + 2f, 5f / attackSpeed, 9f / attackSpeed);
            } else {
                targetPos = trans.position + trans.TransformDirection(vecForward);
            }
            Vector3 direction = (targetPos - trans.position).normalized;
            SetSpecialMove(direction != vecZero ? direction : trans.TransformDirection(vecForward), dist, stiffTimeTemp, EasingType.SineInOut);

            EmitEffect(0);
            //Test
            if (dodgeRemain < dodgePower) {
                dodgeRemain += dodgeRecovery;
                if (dodgeRemain > dodgePower) {
                    dodgeRemain = dodgePower;
                }
            }
        }
    }

    protected override void Update_Transition_Moves() {
        if (!groundedFlag && target && (command == Command.Default || command == Command.Free)) {
            float sqrDist = GetTargetDistance(true, false, true);
            bool attackCond = (sqrDist > MyMath.Square(agentActionDistance[actDistNum].attack.x) && sqrDist < MyMath.Square(agentActionDistance[actDistNum].attack.y) && JudgeStamina(GetCost(CostType.Attack)));
            if (attackCond && state != State.Attack && !GetSick(SickType.Stop)) {
                SetState(State.Attack);
            }
        }
        if (state != State.Attack) {
            base.Update_Transition_Moves();
        }
    }
    
    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (actDistNum == 0 && !JudgeStamina(GetCost(CostType.Attack))) {
            actDistNum = 1;
        } else if (actDistNum == 1 && nowST >= GetMaxST() * staminaBorder) {
            actDistNum = 0;
        }
        if (state != State.Attack){
            if (cCon.height != defaultCConHeight) {
                cCon.height = defaultCConHeight;
                cCon.stepOffset = defaultCConStepOffset;
            }
        }
    }

    protected override void Start_Process_Dodge() {
        base.Start_Process_Dodge();
        EmitEffect(1);
        if (GameManager.Instance.save.config[GameManager.Save.configID_ObscureFriends] < 3) {
            SetChat(chatKey_Dodge[Random.Range(0, 2)], 15);
        }
    }

}
