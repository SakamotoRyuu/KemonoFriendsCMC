using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Raccoon : PlayerController
{

    public Transform[] washPosPivot;
    public GameObject longBuffEffect;
    public Transform longBuffAttack;
    public Vector3 longBuffScale;

    const int washAD = 2;
    const int checkFriendsID = 30;
    // const float defaultAttackCost = 36f;
    // const float angryAttackCost = 24f;
    int stealFailedCount;
    int defaultAttackFaceIndex;
    int angryAttackFaceIndex;
    int defaultSmileFaceIndex;
    int angrySmileFaceIndex;
    bool attackIsLong;
    bool isAngry;
    static readonly Vector3 defaultScale = Vector3.one;

    protected override void Awake()
    {
        base.Awake();
        moveCost.skill = 60f * staminaCostRate;
        moveCost.attack = 90f * staminaCostRate;
        sandstarHealMultiplier = justDodgeAmountMultiplier = 1f; // 6 damages in 1 attack
        if (attackPower > 0)
        {
            sandstarHealMultiplier *= 10f / attackPower;
        }
    }

    protected override void Start()
    {
        base.Start();
        SetAngry(false);
    }

    protected override void SetFaceIndex()
    {
        base.SetFaceIndex();
        if (fCon)
        {
            defaultAttackFaceIndex = faceIndex[(int)FaceName.Attack];
            angryAttackFaceIndex = fCon.GetFaceIndex("Attack2");
            defaultSmileFaceIndex = faceIndex[(int)FaceName.Smile];
            angrySmileFaceIndex = fCon.GetFaceIndex("Smile2");
        }
    }

    protected override void Update_StatusJudge()
    {
        base.Update_StatusJudge();
        if (isAngry && CharacterManager.Instance.GetFriendsExist(checkFriendsID, true))
        {
            SetAngry(false);
        }
        if (attackDetection[washAD])
        {
            if (CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Multi))
            {
                attackDetection[washAD].multiHitInterval = 0.1f;
                attackDetection[washAD].multiHitMaxCount = 0;
            }
            else if (isSuperman)
            {
                attackDetection[washAD].multiHitInterval = 0.25f;
                attackDetection[washAD].multiHitMaxCount = 8;
            }
            else
            {
                attackDetection[washAD].multiHitInterval = 0.3333333f;
                attackDetection[washAD].multiHitMaxCount = 6;
            }
        }
        bool longBuffEnabled = CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Long);
        bool giraffeBeamEnabled = longBuffEnabled && GameManager.Instance.save.config[GameManager.Save.configID_ShowGiraffeBeam] >= 2;
        if (longBuffAttack && attackIsLong != longBuffEnabled)
        {
            attackIsLong = longBuffEnabled;
            if (attackIsLong)
            {
                longBuffAttack.localScale = longBuffScale;
            }
            else
            {
                longBuffAttack.localScale = defaultScale;
            }
        }
        if (longBuffEffect && longBuffEffect.activeSelf != giraffeBeamEnabled)
        {
            longBuffEffect.SetActive(giraffeBeamEnabled);
        }

    }

    public override void ResetGuts()
    {
        base.ResetGuts();
        gutsRemain = gutsMax = CharacterManager.Instance.GetDifficultyEffect(CharacterManager.DifficultyEffect.ServalGuts);
    }

    public override void ResetStatus()
    {
        base.ResetStatus();
        stealFailedCount = 0;
        if (isAngry)
        {
            SetAngry(false);
        }
    }

    public void SetAngry(bool flag = true)
    {
        isAngry = flag;
        if (fCon)
        {
            faceIndex[(int)FaceName.Attack] = isAngry ? angryAttackFaceIndex : defaultAttackFaceIndex;
            faceIndex[(int)FaceName.Smile] = isAngry ? angrySmileFaceIndex : defaultSmileFaceIndex;
        }
    }

    protected override void AttackBody()
    {
        float spRate = isSuperman ? 4f / 3f : 1f;
        if (playerInput.GetButton(RewiredConsts.Action.Special))
        {
            AttackBase(2, 8.5f / 3f, 30f / 10f, GetCost(CostType.Skill), 60f / 30f / spRate, 60f / 30f / spRate, 1f, spRate, true, 15f);
            attackingMoveReservedTimer = 1f / 30f / spRate;
            AttackStartAir();
            nowSpeed = Mathf.Min(nowSpeed, 9f);
        }
        else
        {
            float targetHeight = 0f;
            if (targetTrans)
            {
                targetHeight = (targetTrans.position.y - targetRadius) - trans.position.y;
            }
            int subType = (targetHeight >= 0.8f ? 1 : 0);
            if (attackDetection.Length > washAD)
            {
                attackDetection[washAD].transform.position = washPosPivot[subType].position;
            }
            if (AttackBase(subType, 1f, 1f, GetCost(CostType.Attack), 60f / 30f, 60f / 30f, 1f))
            {
                if (!playerInput.GetButton(RewiredConsts.Action.Dodge))
                {
                    SpecialStep(0.18f, 0.35f / spRate, 4f, 0f, 0f);
                }
                attackingMoveReservedTimer = 1f / 30f;
                AttackStartAir();
                nowSpeed = Mathf.Min(nowSpeed, 9f);
            }
        }
    }

    protected override void AttackContinuous()
    {
        base.AttackContinuous();
        float spRate = isSuperman ? 4f / 3f : 1f;
        if (attackType < 2)
        {
            if (target && attackType < washPosPivot.Length && !playerInput.GetButton(RewiredConsts.Action.Dodge))
            {
                ApproachTransformPivot(washPosPivot[attackType], 3f * spRate, 0.1f, 0.01f, true);
            }
        }
        else
        {
            if (!target || (target && GetTargetDistance(true, true, true) > 0.09f * 0.09f))
            {
                if (!playerInput.GetButton(RewiredConsts.Action.Dodge))
                {
                    move += trans.TransformDirection(vecForward) * 6f * spRate;
                }
            }
        }
    }

    public override void HitAttackAdditiveProcessDD(ref DamageDetection targetDD)
    {
        if (attackType != 2)
        {
            EnemyBase targetEnemyBase = targetDD.GetComponentInParent<EnemyBase>();
            if (targetEnemyBase)
            {
                stealFailedCount++;
                int stealProbability = 35 + Mathf.Clamp(stealFailedCount - 30, 0, 1000);
                if (Random.Range(0, 1000) < stealProbability)
                {
                    stealFailedCount = 0;
                    int itemID = targetEnemyBase.GetStealItemID();
                    if (itemID >= 0)
                    {
                        targetEnemyBase.GiveItemForSteal(itemID, trans.position + vecUp * (attackType == 1 ? 0.9f : 0.4f));
                        targetEnemyBase.RegisterTargetHate(this, CharacterManager.Instance.GetNormalKnockAmount() * (targetEnemyBase.isBoss ? 4f : 1f));
                        EmitEffect(0);
                    }
                }
            }
        }
    }
}
