using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friends_Fennec : FriendsBase {

    const int checkFriendsID = 29;
    //const float defaultAttackCost = 12f;
    // const float angryAttackCost = 12f;
    bool isAngry;
    int defaultAttackFaceIndex;
    int angryAttackFaceIndex;
    int defaultSmileFaceIndex;
    int angrySmileFaceIndex;
    int defaultDeadFaceIndex;
    int angryDeadFaceIndex;
    int attackSave;

    protected override void Awake() {
        base.Awake();
        if (animatorForBattle) {
            chatAttackCount = 4;
            chatDamageCount = 6;
        }
    }

    protected override void Start() {
        base.Start();
        if (animatorForBattle) {
            SetAngry(false);
        }
    }

    public override void ChangeActionDistance(bool isBossBattle) {
        if (!isBossBattle) {
            agentActionDistance[0].attack.y = 15f;
            agentActionDistance[0].chase.y = 15f;
        } else {
            agentActionDistance[0].attack.y = 20f;
            agentActionDistance[0].chase.y = 30f;
        }
    }

    protected override void SetFaceIndex() {
        base.SetFaceIndex();
        if (fCon) {
            defaultAttackFaceIndex = faceIndex[(int)FaceName.Attack];
            angryAttackFaceIndex = fCon.GetFaceIndex("Attack2");
            defaultSmileFaceIndex = faceIndex[(int)FaceName.Smile];
            angrySmileFaceIndex = fCon.GetFaceIndex("Smile2");
            defaultDeadFaceIndex = faceIndex[(int)FaceName.Dead];
            angryDeadFaceIndex = fCon.GetFaceIndex("Dead2");
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (isAngry) {
            if (actDistNum != 2 && !JudgeStamina(GetCost(CostType.Attack))) {
                actDistNum = 2;
            } else if (actDistNum != 1 && nowST >= GetMaxST() * staminaBorder) {
                actDistNum = 1;
            }
        } else {
            if (actDistNum != 2 && !JudgeStamina(GetCost(CostType.Attack))) {
                actDistNum = 2;
            } else if (actDistNum != 0 && nowST >= GetMaxST() * staminaBorder) {
                actDistNum = 0;
            }
        }
        if (isAngry && CharacterManager.Instance.GetFriendsExist(checkFriendsID, true)) {
            SetAngry(false);
        }
    }

    protected override float GetSTHealRate() {
        return base.GetSTHealRate() * (isAngry ? 2f : 1f);
    }

    public override void ResetGuts() {
        base.ResetGuts();
        if (isAngry) {
            SetAngry(false);
        }
    }

    public override void AppearAction() {
        if (CharacterManager.Instance.GetFriendsExist(checkFriendsID, true)) {
            chatKey_Appear = isRevive ? "TALK_FENNEC_REVIVE" : "TALK_FENNEC_APPEAR_SPECIAL";
        } else {
            chatKey_Appear = isRevive ? "TALK_FENNEC_REVIVE" : "TALK_FENNEC_APPEAR";
        }
        base.AppearAction();
    }

    protected override void Start_Process_Dead() {
        if (CharacterManager.Instance.GetFriendsExist(checkFriendsID, true)) {
            chatKey_Dead = "TALK_FENNEC_DEAD_SPECIAL_00";
        } else if (isAngry) {
            chatKey_Dead = "TALK_FENNEC_DEAD_SPECIAL_01";
        } else {
            chatKey_Dead = "TALK_FENNEC_DEAD";
        }
        base.Start_Process_Dead();
    }

    public override void WinAction(bool gravityZero = false, float forceGroundedTimePlus = 0f) {
        if (CharacterManager.Instance.GetFriendsExist(checkFriendsID, true)) {
            chatKey_Win = "TALK_FENNEC_WIN_SPECIAL_00";
        } else if (isAngry) {
            chatKey_Win = "TALK_FENNEC_WIN_SPECIAL_01";
        } else {
            chatKey_Win = "TALK_FENNEC_WIN";
        }
        base.WinAction(gravityZero, forceGroundedTimePlus);
    }

    public void SetAngry(bool flag = true) {
        isAngry = flag;
        if (isAngry) {
            moveCost.attack = 35f * staminaCostRate;
            attackPower = 10f;
            knockPower = 40f;
            attackSave = 3;
            mesAtkMin = 3;
            mesAtkMax = 3;
            mesDmgLtMin = 4;
            mesDmgLtMax = 4;
            mesDmgHvMin = 5;
            mesDmgHvMax = 5;
            SetChat("TALK_FENNEC_SPECIAL", 40, 3f);
        } else {
            moveCost.attack = 46f * staminaCostRate;
            attackPower = 8f;
            knockPower = 30f;
            mesAtkMin = 0;
            mesAtkMax = 2;
            mesDmgLtMin = 0;
            mesDmgLtMax = 2;
            mesDmgHvMin = 1;
            mesDmgHvMax = 3;
        }
        if (fCon) {
            faceIndex[(int)FaceName.Attack] = isAngry ? angryAttackFaceIndex : defaultAttackFaceIndex;
            faceIndex[(int)FaceName.Smile] = isAngry ? angrySmileFaceIndex : defaultSmileFaceIndex;
            faceIndex[(int)FaceName.Dead] = isAngry ? angryDeadFaceIndex : defaultDeadFaceIndex;
        }
    }

    void MoveAttack() {
        float spRate = isSuperman ? 4f / 3f : 1;
        SpecialStep(0.3f, 0.25f / spRate, 4f, 0.05f, 0.4f);
    }

    void MoveAttack1() {
        float spRate = isSuperman ? 4f / 3f : 1;
        SpecialStep(0.3f, 0.25f / spRate, 4f, 0f, 0.4f);
    }

    void MoveAttack2() {
        float spRate = isSuperman ? 4f / 3f : 1;
        SpecialStep(0.25f, 0.25f / spRate, 4f, 0.05f, 0.4f);
    }

    void MoveAttack3() {
        float spRate = isSuperman ? 4f / 3f : 1;
        SpecialStep(0.3f, 0.25f / spRate, 4f, 0.05f, 0.4f);
    }

    void MoveSeparate() {
        float spRate = isSuperman ? 4f / 3f : 1;
        fbStepMaxDist = 3f;
        fbStepTime = 12f / 30f / spRate;
        SeparateFromTarget(7f);
    }

    void MoveEscape() {
        if (targetTrans) {
            float dist = GetTargetDistance(false, true, true);
            if (dist < 7f) {
                float spRate = isSuperman ? 4f / 3f : 1;
                Vector3 escapeDestination = GetEscapeDestination(searchArea[0].GetTargetsAveragePosition(), 12f);
                escapeDestination.y = trans.position.y;
                SetSpecialMove((escapeDestination - trans.position).normalized, Mathf.Clamp(7f - dist, 0f, 4f), 16f / 30f / spRate, EasingType.SineOut);
            }
        }
    }

    void SetCombo2() {
        SetAttackPowerMultiplier(1.6f);
        SetAttackPowerMultiplier(2f);
    }

    protected override void Attack() {
        base.Attack();
        float spRate = isSuperman ? 4f / 3f : 1;
        if (isAngry) {
            int attackRand = Random.Range(1, 4);
            if ((attackSave == 1 && attackRand == 1) || (attackSave == 3 && attackRand == 3)) {
                attackRand = 2;
            }
            spRate *= 1.05f;
            if (attackRand == 1) {
                if (AttackBase(1, 1.2f, 1.25f, GetCost(CostType.Attack) * (26f / 35f), 34f / 30f / 1.25f / spRate, 34f / 30f / 1.25f / spRate, 0.5f, 1.25f * spRate)) {
                    S_ParticlePlay(2);
                    S_ParticlePlay(3);
                    SuperarmorStart();
                    MoveAttack1();
                }
            } else if (attackRand == 2) {
                if (AttackBase(2, 0.9f, 0.7f, GetCost(CostType.Attack) * (19f / 35f), 20f / 30f / spRate, 20f / 30f / spRate, 1f, spRate)) {
                    S_ParticlePlay(0);
                    S_ParticlePlay(1);
                    MoveAttack2();
                }
            } else {
                if (AttackBase(3, 1.2f, 1.25f, GetCost(CostType.Attack), 46f / 30f / 1.25f / spRate, 46f / 30f / 1.25f / spRate, 1f, 1.25f * spRate)) {
                    S_ParticlePlay(2);
                    MoveAttack3();
                }
            }
            attackSave = attackRand;
        } else {
            if (AttackBase(0, 1f, 1f, GetCost(CostType.Attack), 46f / 30f / spRate, 46f / 30f / spRate, 0f, spRate)) {
                MoveSeparate();
            }
        }
    }
}
