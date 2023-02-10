using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_B2 : EnemyBaseBoss {
    
    public Transform quakePressAttackPivot;
    public Transform quakeHeavyKnockPivot;
    public bool isRaw;
    public ParticleSystem weakParticle;

    int attackSave = -1;
    float attackSpeed = 1;
    float knockedDif = 1f;
    float knockStopStateTime = 0f;
    int knockMirrorAnimHash;
    bool quakeFlag;
    float continuousQuakeInterval;

    protected override void Awake() {
        base.Awake();
        deadTimer = 3;
        attackWaitingLockonRotSpeed = 1f;
        if (GameManager.Instance.save.difficulty <= GameManager.difficultyNT) {
            knockedDif = 0.5f;
            coreTimeMax = 10f;
        } else if (GameManager.Instance.save.difficulty == GameManager.difficultyVU) {
            knockedDif = 0.75f;
            coreTimeMax = 9f;
        } else if (GameManager.Instance.save.difficulty >= GameManager.difficultyEN) {
            knockedDif = 1f;
            coreTimeMax = 8f;
        }
        if (isRaw) {
            attackedTimeRemainOnDamage = 0.1f;
        } else {
            attackedTimeRemainOnDamage = 2f;
        }
        coreHideDenomi = 5.5f;
        sandstarRawKnockEndurance = 3500;
        sandstarRawKnockEnduranceLight = 700;
        checkGroundPivotHeight = 1f;
        checkGroundTolerance = 2f;
        checkGroundTolerance_Jumping = 2f;
        killByCriticalOnly = true;
        coreShowHP = GetMaxHP();
        knockMirrorAnimHash = Animator.StringToHash("KnockMirror");
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (!battleStarted && target) {
            BattleStart();
        }
        if (GameManager.Instance.save.difficulty >= GameManager.difficultyVU || sandstarRawEnabled) {
            weakProgress = (nowHP <= GetMaxHP() / 3 ? 2 : nowHP <= GetMaxHP() * 2 / 3 ? 1 : 0);
        } else {
            weakProgress = (nowHP <= GetMaxHP() / 2 ? 1 : 0);
        }
        attackSpeed = (weakProgress == 2 ? 1.2f : 1);
        if (coreTimeRemain > 0f) {
            if (state == State.Damage) {
                stateTime = knockStopStateTime;
                coreTimeRemain -= deltaTimeCache * (nowHP > 1 ? CharacterManager.Instance.riskyIncSqrt : 1f);
                knockRemain = knockEndurance;
                knockRemainLight = knockEnduranceLight;
                if (nowHP <= GetCoreHideBorder()) {
                    coreTimeRemain = -1f;
                }
                if (coreTimeRemain <= 0f) {
                    knockRestoreSpeed = 1f;
                    attackedTimeRemain = 0f;
                    anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], 1f);
                }
            } else {
                coreTimeRemain = 0f;
            }
        }
        if (state != State.Attack) {
            quakeFlag = false;
        }
        if (state == State.Attack && quakeFlag) {
            continuousQuakeInterval -= deltaTimeCache;
            if (continuousQuakeInterval <= 0f) {
                continuousQuakeInterval = 0.03f;
                CameraManager.Instance.SetQuake(GetCenterPosition(), 4, 8, 0, 0.05f, 0f, 4f, dissipationDistance_Boss);
            }
        }
        bool weakParticleFlag = !(state == State.Dead || (state == State.Damage && isDamageHeavy) || coreTimeRemain > 0f);
        if (weakParticle.isPlaying != weakParticleFlag) {
            if (weakParticleFlag) {
                weakParticle.Play();
            } else {
                weakParticle.Stop();
            }
        }
    }

    public void QuakeAttack() {
        quakeFlag = true;
        continuousQuakeInterval = 0f;
    }

    public void QuakePress() {
        if (state == State.Attack) {
            CameraManager.Instance.SetQuake(quakePressAttackPivot.position, 14, 4, 0, 0, 1.5f, 4f, dissipationDistance_Boss);
        }
    }

    public void QuakeHeavyKnock() {
        if (state == State.Damage) {
            CameraManager.Instance.SetQuake(quakeHeavyKnockPivot.position, 5, 4, 0, 0, 1.5f, 4f, dissipationDistance_Boss);
        }
    }

    public override float GetKnocked() {
        return base.GetKnocked() * knockedDif;
    }

    void QuakeEnd() {
        quakeFlag = false;
        if (state == State.Attack) {
            CameraManager.Instance.SetQuake(GetCenterPosition(), 4, 8, 0, 0, 1f, 4f, dissipationDistance_Boss);
        }
    }

    public override float GetLockonRotSpeedRate() {
        float multiplier = 1f;
        if (state != State.Attack && targetTrans) {
            Quaternion targetDir = Quaternion.LookRotation(GetTargetVector(true, false, false));
            float angle = Quaternion.Angle(trans.rotation, targetDir);
            if (angle <= 25f) {
                multiplier = 0.25f;
            }
        }
        return base.GetLockonRotSpeedRate() * multiplier;
    }

    protected void MoveAttack1() {
        if (state == State.Attack) {
            float distance = 0;
            if (targetTrans) {
                distance = Vector3.Distance(targetTrans.position, trans.position) + 16;
            }
            SetSpecialMove(trans.TransformDirection(vecForward), Mathf.Clamp(distance, 30f, 100f), 120f / 60f / attackSpeed, EasingType.SineInOut);
        }
    }

    protected void MoveAttack2() {
        if (state == State.Attack) {
            float distance = 0;
            Vector3 targetTemp = trans.position;
            if (targetTrans) {
                distance = Vector3.Distance(targetTrans.position, trans.position) + 8;
                targetTemp = targetTrans.position;
                targetTemp.y = trans.position.y;
            }
            SetSpecialMove((targetTemp - trans.position).sqrMagnitude < 4f * 4f ? trans.TransformDirection(vecForward) : (targetTemp - trans.position).normalized, Mathf.Clamp(distance, 10f, 30f), 90f / 60f, EasingType.SineInOut);
        }
    }

    protected void MoveAttack3() {
        if (state == State.Attack) {
            float distance = 0;
            if (targetTrans) {
                distance = Vector3.Distance(targetTrans.position, trans.position) + 4;
            }
            SetSpecialMove(trans.TransformDirection(vecForward), Mathf.Clamp(distance, 4f, 12f), 40f / 60f, EasingType.SineInOut);
        }
    }

    protected void MoveAttack3_2() {
        if (state == State.Attack) {
            SetSpecialMove(trans.TransformDirection(vecForward), 4, 60f / 60f, EasingType.SineInOut);
        }
    }

    protected override void KnockLightProcess() {
        if (state != State.Damage) {
            base.KnockLightProcess();
            knockRestoreSpeed = 1f;
            anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], 1f);
        }
    }

    protected override void KnockHeavyProcess() {
        if (state != State.Damage || !isDamageHeavy) {
            if (playerTrans) {
                anim.SetBool(knockMirrorAnimHash, Vector3.Cross(trans.TransformDirection(vecForward), playerTrans.position - trans.position).y < 0); // Left = True
            }
            base.KnockHeavyProcess();
            knockRestoreSpeed = 1f;
            anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], 1f);
            coreShowHP = nowHP;
            coreHideConditionDamage = GetCoreHideConditionDamage();
            agent.radius = 0.1f;
            agent.height = 0.1f;
        }
    }

    public override void TakeDamage(int damage, Vector3 position, float knockAmount, Vector3 knockVector, CharacterBase attacker = null, int colorType = 0, bool penetrate = false) {
        cannotKnockDown = (colorType == damageColor_Back || colorType == damageColor_Hard || colorType == damageColor_HardBack);
        base.TakeDamage(damage, position, knockAmount, knockVector, attacker, colorType, penetrate);
    }

    public override void SetSandstarRaw() {
        base.SetSandstarRaw();
        if (state != State.Dead) {
            coreShowHP = GetMaxHP();
            coreHideConditionDamage = 0;
            coreHideDenomi = 8f;
            knockedDif = 1f;
            coreTimeMax = 3f;
            if (coreTimeRemain > 0.01f) {
                coreTimeRemain = 0.01f;
            }
        }
    }

    void KnockStop() {
        knockRestoreSpeed = 0f;
        coreTimeRemain = coreTimeMax;
        anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], 0f);
        knockStopStateTime = stateTime;
    }

    public override void SetSick(SickType sickType, float duration, AttackDetection attacker = null) {
        base.SetSick(sickType, duration, attacker);
        if (sickType == SickType.Acid && attacker && CharacterManager.Instance.GetFriendsExist(6, false) && attacker.parentCBase == CharacterManager.Instance.friends[6].fBase) {
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_Alpaca, true);
        }
    }

    protected override void Attack() {
        resetAgentRadiusOnChangeState = true;
        resetAgentHeightOnChangeState = true;
        if (targetTrans) {
            float sqrDist = GetTargetDistance(true, true, false);
            int attackTemp;
            if (attackSave < 0 || (sqrDist > 30 * 30 && Random.Range(0, 100) < 75)) {
                attackTemp = 0;
            } else if (sqrDist < 4 * 4 && targetTrans.position.y > trans.position.y + 1 && Random.Range(0, 100) < 75) {
                attackTemp = 3;
            } else {
                int max = sqrDist < 10 * 10 ? 4 : 3;
                attackTemp = Random.Range(0, max);
                if (attackTemp == attackSave) {
                    attackTemp = Random.Range(0, max);
                }
            }
            attackSave = attackTemp;
            float intervalPlus = 0f;
            if (isRaw) {
                intervalPlus = (weakProgress == 2 ? Random.Range(0.3f, 0.5f) : weakProgress == 1 ? Random.Range(0.4f, 0.6f) : Random.Range(0.5f, 0.7f));
            } else if (sandstarRawEnabled) {
                intervalPlus = (weakProgress == 2 ? Random.Range(0.4f, 0.6f) : weakProgress == 1 ? Random.Range(0.6f, 0.9f) : Random.Range(0.9f, 1.2f));
            } else {
                intervalPlus = (weakProgress == 2 ? Random.Range(0.5f, 1.2f) : weakProgress == 1 ? Random.Range(1f, 1.7f) : Random.Range(1.5f, 2.2f));
            }
            switch (attackTemp) {
                case 0:
                    AttackBase(0, 1.1f, 2.1f, 0, 240f / 60f / attackSpeed, 240f / 60f / attackSpeed + intervalPlus, 1, attackSpeed);
                    break;
                case 1:
                    AttackBase(weakProgress == 2 ? 4 :1, 1.05f, 1.2f, 0, 105f / 60f, 105f / 60f + 1 + intervalPlus, 1, 1, true, attackLockonDefaultSpeed * 0.3f);
                    break;
                case 2:
                    AttackBase(weakProgress == 2 ? 5 : 2, 1.05f, 1.2f, 0, 105f / 60f, 105f / 60f + 1 + intervalPlus, 1, 1, true, attackLockonDefaultSpeed * 0.5f);
                    fbStepTime = 0.5f;
                    fbStepMaxDist = 4f;
                    BackStep(6);
                    break;
                case 3:
                    AttackBase(3, 1f, 1.1f, 0, (isRaw ? 245f : 185f) / 60f / attackSpeed, (isRaw ? 240f : 180f) / 60f / attackSpeed + intervalPlus, 1, attackSpeed, true, attackLockonDefaultSpeed * 0.5f);
                    break;
            }
        }
    }

    public void CheckTrophy_NeedleAttacking(AttackDetection attacker) {
        if (state == State.Attack && attackType == 3 && stateTime >= 0.25f && stateTime < 0.75f && attacker == attackDetection[3]) {
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_B2Needle, true);
        }
    }

}
