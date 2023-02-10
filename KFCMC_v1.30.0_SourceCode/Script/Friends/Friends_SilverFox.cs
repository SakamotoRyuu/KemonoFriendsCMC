using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friends_SilverFox : FriendsBase {

    public GameObject obstaclePrefab;

    int throwIndex = 0;
    float eventTimeRemain;
    int eventFaceIndex = -1;
    float obstacleResetTimeRemain = 0f;
    GameObject obstacleInstance;

    protected override void Awake() {
        base.Awake();
        if (animatorForBattle) {
            attackedTimeRemainOnDamage = 3f;
            moveCost.attack = 80;
            belligerent = false;
        }
    }

    void ThrowReadySpecial() {
        if (throwing) {
            int throwRand = Random.Range(0, 100);
            if (throwRand < 60) {
                throwIndex = 0;
            } else if (throwRand < 85) {
                throwIndex = 1;
            } else if (throwRand < 98) {
                throwIndex = 2;
            } else {
                throwIndex = 3;
                TrophyManager.Instance.CheckTrophy(TrophyManager.t_SilverFox, true);
            }
            throwing.ThrowReady(throwIndex);
        }
    }

    void ThrowStartSpecial() {
        if (throwing) {
            throwing.ThrowStart(throwIndex);
        }
    }

    public void SetDance() {
        eventFaceIndex = fCon.GetFaceIndex("Event");
        eventTimeRemain = 240f / 30f;
        AttackBase(1, 0f, 0f, 0f, 180f / 30f, 180f / 30f, 0f, 1f, false);
        obstacleResetTimeRemain = 5f;
        gravityZeroTimeRemain = 5.5f;
        agent.enabled = false;
        obstacleInstance = Instantiate(obstaclePrefab, transform);
    }

    protected override void Update_TimeCount() {
        base.Update_TimeCount();
        if (obstacleResetTimeRemain > 0f) {
            obstacleResetTimeRemain -= deltaTimeCache;
            if (obstacleResetTimeRemain <= 0f) {
                if (obstacleInstance) {
                    Destroy(obstacleInstance);
                }
                if (agent && !agent.enabled) {
                    agent.enabled = true;
                }
            } else {
                if (agent && agent.enabled) {
                    agent.enabled = false;
                }
            }
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (actDistNum == 0 && !JudgeStamina(GetCost(CostType.Attack))) {
            actDistNum = 1;
        } else if (actDistNum == 1 && nowST >= GetMaxST() * staminaBorder) {
            actDistNum = 0;
        }
    }

    void EventFace2() {
        eventFaceIndex = fCon.GetFaceIndex("Event2");
    }

    void EventEffect() {
        EmitEffect(0);
    }

    protected override void Update_FaceControl() {
        if (eventTimeRemain > 0f) {
            eventTimeRemain -= deltaTimeCache;
            if (fCon && eventFaceIndex != -1 && fCon.CurrentFaceIndex != eventFaceIndex) {
                SetFace(eventFaceIndex);
            }
        } else {
            base.Update_FaceControl();
        }
    }

    protected override void Attack() {
        base.Attack();
        float spRate = (isSuperman ? 4f / 3f : 1f) * 2f;
        throwing.target = targetTrans;
        if (AttackBase(0, 1f, 1f, GetCost(CostType.Attack), 82f / 30f / spRate, (82f / 30f + 1.6f) / spRate, 0f, spRate)) {
            if (targetTrans && GetTargetDistance(true, true, true) < 4.5f * 4.5f) {
                fbStepMaxDist = 2.5f;
                fbStepTime = 30f / 30f / spRate;
                SeparateFromTarget(4.5f);
            }
        }
    }
}
