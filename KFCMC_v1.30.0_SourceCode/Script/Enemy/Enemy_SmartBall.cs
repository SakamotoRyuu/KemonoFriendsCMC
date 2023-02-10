using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_SmartBall : EnemyBase {

    public GameObject[] criticalPoints;
    public Transform rayFrom;
    public GameObject wallBreaker;
    
    bool quakeFlag = false;
    int criticalIndex = -1;
    int attackSave = -1;
    float continuousQuakeInterval;
    static readonly float[] knockRestoreArray = new float[] { 1f, 1f, 1.25f, 1.5f, 2f };
    static readonly float[] stiffPlusArray = new float[] { 0.9f, 0.9f, 0.6f, 0.4f, 0.3f };

    protected void SetCriticalPoint() {
        int indexTemp = criticalIndex;
        for (int i = 0; i < 10 && criticalIndex == indexTemp; i++) {
            indexTemp = Random.Range(0, criticalPoints.Length);
        }
        for (int i = 0; i < criticalPoints.Length; i++) {
            if (criticalPoints[i]) {
                criticalPoints[i].SetActive(i == indexTemp);
            }
        }
        criticalIndex = indexTemp;
    }

    protected override void Awake() {
        base.Awake();
        SetCriticalPoint();
    }

    protected void AttackMove_Back(int frames) {
        if (state == State.Attack) {
            SetSpecialMove(trans.TransformDirection(Vector3.back), 0.95f * frames / 30f, frames / 60f, EasingType.SineInOut);
        }
    }

    protected void AttackMove_Bounce(int frames) {
        if (state == State.Attack) {
            SetSpecialMove(trans.TransformDirection(Vector3.back), 2f * frames / 60f, frames / 60f, EasingType.SineOut);
        }
    }

    protected void AttackMove_Forward(int frames) {
        if (state == State.Attack) {
            SetSpecialMove(trans.TransformDirection(Vector3.forward), 12.5f * frames / 60f, frames / 60f, EasingType.SineIn);
        }
    }

    protected void AttackMove_ForwardLong(int frames) {
        if (state == State.Attack) {
            SetSpecialMove(trans.TransformDirection(Vector3.forward), 18f * frames / 60f, frames / 60f, EasingType.SineIn);
        }
    }

    protected void AttackMove_Jump(int frames) {
        if (state == State.Attack && targetTrans) {
            SetSpecialMove(GetTargetVector(true, true), GetTargetDistance(false, true, false), frames / 60f, EasingType.SineOut);
        }
    }
    
    void QuakeAttack() {
        if (state == State.Attack) {
            CameraManager.Instance.SetQuake(trans.position, 10, 4, 0, 0, 1.5f, 2f, dissipationDistance_Normal);
        }
    }

    void SetQuakeFlag(int flag) {
        quakeFlag = (flag != 0);
        continuousQuakeInterval = 0f;
    }
    
    void ChangeCriticalPoint() {
        if (level >= 3) {
            SetCriticalPoint();
        }
    }

    protected override void SetLevelModifier() {
        if (wallBreaker) {
            if (level >= 2) {
                wallBreaker.SetActive(true);
            } else {
                wallBreaker.SetActive(false);
            }
        }
        knockRestoreSpeed = knockRestoreArray[Mathf.Clamp(level, 0, knockRestoreArray.Length - 1)];
        if (anim) {
            anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], knockRestoreSpeed);
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (state != State.Attack) {
            quakeFlag = false;
        }
        if (state == State.Attack && quakeFlag) {
            continuousQuakeInterval -= deltaTimeCache;
            if (continuousQuakeInterval <= 0f) {
                continuousQuakeInterval = 0.03f;
                if (attackType == 3) {
                    CameraManager.Instance.SetQuake(trans.position, 4, 4, 0, 0.05f, 0f, 2.5f, dissipationDistance_Normal * 1.5f);
                } else {
                    CameraManager.Instance.SetQuake(trans.position, 3, 4, 0, 0.05f, 0f, 1.5f, dissipationDistance_Normal);
                }
            }
        }
    }

    protected override void DamageCommonProcess() {
        base.DamageCommonProcess();
        if (!fixKnockAmount && nowHP <= 0 && CharacterManager.Instance.GetFriendsExist(21, false) && attackerCB == CharacterManager.Instance.friends[21].fBase) {
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_SmartBallRedFox, true);
        }
    }

    protected override void Attack() {
        base.Attack();
        resetAgentRadiusOnChangeState = true;
        int attackTemp = 0;
        if (level >= 2) {
            attackTemp = Random.Range(0, level);
            if (attackTemp == attackSave) {
                attackTemp = Random.Range(0, level);
            }
        }
        attackSave = attackTemp;
        float stiffPlus = stiffPlusArray[Mathf.Clamp(level - attackTemp, 0, stiffPlusArray.Length - 1)];
        if (IsSuperLevel) {
            stiffPlus = 0f;
        }
        switch (attackTemp) {
            case 0:
                AttackBase(0, 1, 1.4f, 0, 150f / 60f + stiffPlus, 150f / 60f + stiffPlus + GetAttackInterval(1.5f));
                break;
            case 1:
                AttackBase(1, 1.1f, 1.7f, 0, 150f / 60f + stiffPlus, 150f / 60f + stiffPlus + GetAttackInterval(1.5f, -1));
                agent.radius = 0.05f;
                break;
            case 2:
                AttackBase(2, 1, 1.4f, 0, 230f / 60f + stiffPlus, 230f / 60f + stiffPlus + GetAttackInterval(1.5f, -2));
                break;
            case 3:
                AttackBase(3, 1.2f, 2.5f, 0, 180f / 60f + stiffPlus, 180f / 60f + stiffPlus + GetAttackInterval(1.5f, -3));
                SuperarmorStart();
                break;
        }
    }

    public override void EmitEffectString(string type) {
        base.EmitEffectString(type);
        if (type == "End") {
            if (Physics.Raycast(new Ray(rayFrom.position, rayFrom.TransformDirection(vecForward)), 1.5f, fieldLayerMask, QueryTriggerInteraction.Ignore)) {
                EmitEffect(attackType == 3 ? 6 : 1);
            } else {
                EmitEffect(attackType == 3 ? 7 : 2);
            }
        }
    }

}
