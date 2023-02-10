using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friends_Raccoon : FriendsBase {

    public Transform[] washPosPivot;
    public GameObject longBuffEffect;
    public Transform longBuffAttack;
    public Vector3 longBuffScale;

    const int washAD = 2;
    const int checkFriendsID = 30;
    // const float defaultAttackCost = 36f;
    // const float angryAttackCost = 24f;
    int stealFailedCount;
    bool isAngry;
    int defaultAttackFaceIndex;
    int angryAttackFaceIndex;
    int defaultSmileFaceIndex;
    int angrySmileFaceIndex;
    bool attackIsLong;
    static readonly Vector3 defaultScale = Vector3.one;

    protected override void Awake() {
        base.Awake();
        if (animatorForBattle) {
            chatAttackCount = 6;
        }
    }

    protected override void Start() {
        base.Start();
        if (animatorForBattle) {
            SetAngry(false);
        }
    }

    protected override void SetFaceIndex() {
        base.SetFaceIndex();
        if (fCon) {
            defaultAttackFaceIndex = faceIndex[(int)FaceName.Attack];
            angryAttackFaceIndex = fCon.GetFaceIndex("Attack2");
            defaultSmileFaceIndex = faceIndex[(int)FaceName.Smile];
            angrySmileFaceIndex = fCon.GetFaceIndex("Smile2");
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (actDistNum != 1 && !JudgeStamina(GetCost(CostType.Attack))) {
            actDistNum = 1;
        } else if (actDistNum != 0 && nowST >= GetMaxST() * staminaBorder && JudgeStamina(GetCost(CostType.Attack))) {
            actDistNum = 0;
        }
        if (isAngry && CharacterManager.Instance.GetFriendsExist(checkFriendsID, true)) {
            SetAngry(false);
        }
        if (attackDetection[washAD]) {
            if (CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Multi)) {
                attackDetection[washAD].multiHitInterval = 0.1f;
                attackDetection[washAD].multiHitMaxCount = 0;
            } else if (isSuperman) {
                attackDetection[washAD].multiHitInterval = 0.25f;
                attackDetection[washAD].multiHitMaxCount = 8;
            } else {
                attackDetection[washAD].multiHitInterval = 0.3333333f;
                attackDetection[washAD].multiHitMaxCount = 6;
            }
        }
        bool longBuffEnabled = CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Long);
        bool giraffeBeamEnabled = longBuffEnabled && GameManager.Instance.save.config[GameManager.Save.configID_ShowGiraffeBeam] >= 2;
        if (longBuffAttack && attackIsLong != longBuffEnabled) {
            attackIsLong = longBuffEnabled;
            if (attackIsLong) {
                longBuffAttack.localScale = longBuffScale;
            } else {
                longBuffAttack.localScale = defaultScale;
            }
        }
        if (longBuffEffect && longBuffEffect.activeSelf != giraffeBeamEnabled) {
            longBuffEffect.SetActive(giraffeBeamEnabled);
        }
        
    }

    public override void ResetGuts() {
        base.ResetGuts();
        stealFailedCount = 0;
        gutsRemain = gutsMax = CharacterManager.Instance.GetDifficultyEffect(CharacterManager.DifficultyEffect.ServalGuts);
        if (isAngry) {
            SetAngry(false);
        }
    }

    protected override float GetSTHealRate() {
        return base.GetSTHealRate() * (isAngry ? 2f : 1f);
    }

    public override void AppearAction() {
        if (CharacterManager.Instance.GetFriendsExist(checkFriendsID, true)) {
            chatKey_Appear = isRevive ? "TALK_RACCOON_REVIVE_SPECIAL" : "TALK_RACCOON_APPEAR_SPECIAL";
        } else {
            chatKey_Appear = isRevive ? "TALK_RACCOON_REVIVE" : "TALK_RACCOON_APPEAR";
        }
        base.AppearAction();
    }

    protected override void Start_Process_Dead() {
        if (CharacterManager.Instance.GetFriendsExist(checkFriendsID, true)) {
            chatKey_Dead = "TALK_RACCOON_DEAD_SPECIAL_00";
        } else if (isAngry) {
            chatKey_Dead = "TALK_RACCOON_DEAD_SPECIAL_01";
        } else {
            chatKey_Dead = "TALK_RACCOON_DEAD";
        }
        base.Start_Process_Dead();
    }

    public override void WinAction(bool gravityZero = false, float forceGroundedTimePlus = 0f) {
        if (CharacterManager.Instance.GetFriendsExist(checkFriendsID, true)) {
            chatKey_Win = "TALK_RACCOON_WIN_SPECIAL_00";
        } else if (isAngry) {
            chatKey_Win = "TALK_RACCOON_WIN_SPECIAL_01";
        } else {
            chatKey_Win = "TALK_RACCOON_WIN";
        }
        base.WinAction(gravityZero, forceGroundedTimePlus);
    }

    public void SetAngry(bool flag = true) {
        isAngry = flag;
        if (isAngry) {
            moveCost.attack = 60f * staminaCostRate;
            attackPower = 8.5f;
            knockPower = 30f;
            mesAtkMin = 3;
            mesAtkMax = 5;
            SetChat("TALK_RACCOON_SPECIAL", 40, 3f);
        } else {
            moveCost.attack = 90f * staminaCostRate;
            attackPower = 3f;
            knockPower = 10f;
            mesAtkMin = 0;
            mesAtkMax = 2;
        }
        if (fCon) {
            faceIndex[(int)FaceName.Attack] = isAngry ? angryAttackFaceIndex : defaultAttackFaceIndex;
            faceIndex[(int)FaceName.Smile] = isAngry ? angrySmileFaceIndex : defaultSmileFaceIndex;
        }
    }

    protected override void Attack() {
        base.Attack();
        float spRate = isSuperman ? 4f / 3f : 1f;
        if (isAngry) {
            AttackBase(2, 1f, 1f, GetCost(CostType.Attack), 60f / 30f / spRate, 60f / 30f / spRate, 1f, spRate, true, 15f);
        } else {
            float targetHeight = 0f;
            if (targetTrans) {
                targetHeight = (targetTrans.position.y - targetRadius) - trans.position.y;
            }
            int subType = (targetHeight >= 0.8f ? 1 : 0);
            if (attackDetection.Length > washAD) {
                attackDetection[washAD].transform.position = washPosPivot[subType].position;
            }
            if (AttackBase(subType, 1f, 1f, GetCost(CostType.Attack), 60f / 30f, 60f / 30f + 1f / spRate, 0.5f)) {
                SpecialStep(0.18f, 0.35f / spRate, 4f, 0f, 0f);
            }
        }
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        float spRate = isSuperman ? 4f / 3f : 1f;
        if (attackType < 2) {
            /*
            if (target && stateTime >= 0.5f && GetTargetDistance(true, true, true) > (attackType == 0 ? 0.13f * 0.13f : 0.26f * 0.26f)) {
                cCon.Move(trans.TransformDirection(vecForward) * 3f * spRate * deltaTimeMove);
            }
            */
            if (target && attackType < washPosPivot.Length) {
                ApproachTransformPivot(washPosPivot[attackType], 3f * spRate, 0.1f, 0.01f, true);
            }
        } else {
            if (!target || (target && GetTargetDistance(true, true, true) > 0.09f * 0.09f)) {
                cCon.Move(trans.TransformDirection(vecForward) * 6f * spRate * deltaTimeMove);
            }
        }
    }

    public override void HitAttackAdditiveProcessDD(ref DamageDetection targetDD) {
        if (!isAngry) {
            EnemyBase targetEnemyBase = targetDD.GetComponentInParent<EnemyBase>();
            if (targetEnemyBase) {
                stealFailedCount++;
                int stealProbability = 35 + Mathf.Clamp(stealFailedCount - 30, 0, 1000);
                if (Random.Range(0, 1000) < stealProbability) {
                    stealFailedCount = 0;
                    int itemID = targetEnemyBase.GetStealItemID();
                    if (itemID >= 0) {
                        targetEnemyBase.GiveItemForSteal(itemID, trans.position + vecUp * (attackType == 1 ? 0.9f : 0.4f));
                        targetEnemyBase.RegisterTargetHate(this, CharacterManager.Instance.GetNormalKnockAmount() * (targetEnemyBase.isBoss ? 4f : 1f));
                        EmitEffect(0);
                    }
                }
            }
        }
    }

}
