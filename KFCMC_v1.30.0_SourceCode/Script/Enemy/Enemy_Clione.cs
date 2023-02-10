using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;

public class Enemy_Clione : EnemyBase {

    public Transform quakePivot;
    public DamageDetection criticalDD;
    public GameObject[] guardedObj;
    public GameObject shieldObj;
    public Transform throwPivot;
    public LookatTarget lookatTarget;
    public Transform nullTarget;

    int attackSave = -1;
    float guardKnockRemain;
    bool[] t_PPPAttacked = new bool[5];
    bool t_GotTrophy;
    const int effThrowReady = 2;
    const int effThrowStart = 3;
    const int effShieldBreak = 4;
    static readonly float[] damageRateArray = new float[5] { 4f, 4f, 3.833333f, 3.666666f, 3.5f };

    protected override void SetLevelModifier() {
        if (criticalDD) {
            criticalDD.damageRate = damageRateArray[Mathf.Clamp(level, 0, damageRateArray.Length - 1)];
        }
        ActivateGuard(level >= 4);
        attackSave = -1;
    }

    protected override void Awake() {
        base.Awake();
        attackWaitingLockonRotSpeed = 1f;
    }

    protected override void Start() {
        base.Start();
        if (TrophyManager.Instance) {
            t_GotTrophy = TrophyManager.Instance.IsTrophyHad(TrophyManager.t_ClionePPP);
        }
    }

    void MoveAttack0() {
        SpecialStep(2.2f, 20f / 60f, 6f, 0f, 0f);
    }

    void MoveAttack1() {
        SpecialStep(0f, 30f / 60f, 10f, 0f, 0f, true, false, EasingType.SineOut);
    }

    void MoveAttack2() {
        fbStepTime = 0.3f;
        fbStepMaxDist = 3f;
        BackStep(6f);
    }

    void MoveSeparate() {
        fbStepMaxDist = 0.4f;
        fbStepMaxDist = 3.5f;
        SeparateFromTarget(6f);
    }

    void NeedleReady() {
        if (state == State.Attack) {
            throwPivot.localEulerAngles = vecZero;
            lookatTarget.enabled = true;
            if (targetTrans) {
                lookatTarget.SetTarget(targetTrans);
            } else {
                lookatTarget.SetTarget(nullTarget);
            }
            for (int i = 1; i < 8; i++) {
                throwing.ThrowReady(i);
            }
            EmitEffect(effThrowReady);
        }
    }

    void NeedleStart() {
        if (state == State.Attack) {
            for (int i = 1; i < 8; i++) {
                throwing.ThrowStart(i);
            }
            EmitEffect(effThrowStart);
        }
        lookatTarget.enabled = false;
    }

    void QuakeAttack() {
        if (state == State.Attack) {
            CameraManager.Instance.SetQuake(quakePivot.position, 6, 4, 0, 0, 1.5f, 2f, dissipationDistance_Normal);
        }
    }

    int GetAttackType() {
        int max = (level >= 2 ? 4 : 3);
        if (targetTrans) {
            float sqrDist = (targetTrans.position - trans.position).sqrMagnitude;
            if (sqrDist > 6f * 6f) {
                return Random.Range(2, max);
            } else if (sqrDist > 3f * 3f) {
                return Random.Range(0, max);
            } else {
                return Random.Range(0, 3);
            }
        }
        return Random.Range(0, max);
    }

    void ActivateGuard(bool flag) {
        for (int i = 0; i < guardedObj.Length; i++) {
            if (guardedObj[i] != null) {
                guardedObj[i].SetActive(!flag);
            }
        }
        shieldObj.SetActive(flag);
        if (flag) {
            guardKnockRemain = 1200;
        } else {
            guardKnockRemain = -1;
        }
    }

    public void ReceiveGuardKnock(float knockAmount) {
        if (guardKnockRemain > 0f) {
            guardKnockRemain -= knockAmount;
            if (guardKnockRemain <= 0f) {
                ActivateGuard(false);
                EmitEffect(effShieldBreak);
            }
        }
    }

    protected override void DamageCommonProcess() {
        base.DamageCommonProcess();
        if (!fixKnockAmount && !t_GotTrophy && attackerCB && CharacterManager.Instance.GetFriendsExist(16) && CharacterManager.Instance.GetFriendsExist(17) && CharacterManager.Instance.GetFriendsExist(18) && CharacterManager.Instance.GetFriendsExist(19) && CharacterManager.Instance.GetFriendsExist(20)) {
            for (int i = 0; i < t_PPPAttacked.Length; i++) {
                if (!t_PPPAttacked[i] && attackerCB == CharacterManager.Instance.friends[16 + i].fBase) {
                    t_PPPAttacked[i] = true;
                }
            }
        }
    }

    protected override void DeadProcess() {
        base.DeadProcess();
        if (!t_GotTrophy) {
            bool answer = true;
            for (int i = 0; i < t_PPPAttacked.Length; i++) {
                if (t_PPPAttacked[i] == false || CharacterManager.Instance.GetFriendsExist(16 + i, true) == false) {
                    answer = false;
                    break;
                }
            }
            if (answer) {
                CharacterManager.Instance.CheckTrophy_PPP();
            }
        }
    }

    public override void TakeDamageFixKnock(int damage, Vector3 position, float knockAmount, Vector3 knockVector, CharacterBase attacker = null, int colorType = 0, bool penetrate = false) {
        base.TakeDamageFixKnock(damage, position, knockAmount, knockVector, attacker, colorType, penetrate);
        ReceiveGuardKnock(knockAmount);
    }

    public override void SetSick(SickType sickType, float duration, AttackDetection attacker = null) {
        base.SetSick(sickType, duration, attacker);
        if (sickType == SickType.Fire && duration > 0f) {
            ReceiveGuardKnock(10000);
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (state != State.Attack && lookatTarget && lookatTarget.enabled) {
            lookatTarget.enabled = false;
        }
        if (lookatTarget.enabled) {
            if (targetTrans) {
                lookatTarget.SetTarget(targetTrans);
            } else {
                lookatTarget.SetTarget(nullTarget);
            }
        }
    }

    protected override void Attack() {
        base.Attack();
        resetAgentRadiusOnChangeState = true;
        int attackTemp = GetAttackType();
        if (attackSave == attackTemp) {
            attackTemp = GetAttackType();
        }
        attackSave = attackTemp;
        switch (attackTemp) {
            case 0:
                AttackBase(0, 1f, 0.8f, 0, 80f / 60f, 80f / 60f + GetAttackInterval(1.5f));
                break;
            case 1:
                AttackBase(1, 1.05f, 1.5f, 0, 110f / 60f, 110f / 60f + GetAttackInterval(1.5f), 0);
                agent.radius = 0.05f;
                break;
            case 2:
                AttackBase(2, 0.95f, 1.1f, 0, 80f / 60f + (IsSuperLevel ? 0f : 1f), 80f / 60f + GetAttackInterval(2f));
                break;
            case 3:
                AttackBase(3, 1.1f, 1.4f, 0, 105f / 60f, 105f / 60f + GetAttackInterval(1.5f), 0);
                MoveSeparate();
                break;
        }
    }

}
