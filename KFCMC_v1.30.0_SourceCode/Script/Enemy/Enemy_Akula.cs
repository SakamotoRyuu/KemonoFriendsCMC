using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;

public class Enemy_Akula : EnemyBaseBoss {

    public GameObject cellienCore;
    public GameObject coreDetection;
    public Transform corePivot;
    public LookatTarget lookatTarget;
    public Transform nullTarget;

    public Transform follower;
    public Projector followerProjector;
    public Material followerBrightMat;
    public Material followerDarkMat;
    public OnTriggerByPlayerCheck onCheck;
    public OnTriggerByPlayerCheck followerOnCheck;
    public Transform[] followerAppearPoint;
    public GameObject followerDD;

    LaserOption laserOption;
    float followerRestRemain = 0;
    float bigBombRestRemain = 0;
    int followThrowCount;
    float followThrowTimeRemain;
    int attackSave = -1;
    bool healEffectEmitted = false;
    int followerState = -1;
    float followerResetTimer;
    
    const float throwSpeedMin = 30f;
    const float throwSpeedUpBorderDistance = 60f;
    const float throwSpeedUpRate = 0.4f;
    static readonly float[] followThrowBorder = new float[] { 17f / 60f, 11f / 60f, 5f / 60f };
    static readonly float[] eyeSpeed = new float[] { 1.8f, 1.6f, 1.4f };

    protected override void Awake() {
        base.Awake();
        deadTimer = 3;
        actDistNum = 0;
        attackedTimeRemainOnDamage = 0.5f;
        attackWaitingLockonRotSpeed = 0;
        gravityMultiplier = 0;
        winActionGravityZero = true;
        winActionForceGroundedTimePlus = 1.5f;
        CoreHide();
        isCoreShowed = false;
        healEffectEmitted = false;
        coreTimeRemain = 0f;
        coreTimeMax = 18f;
        // coreHideDenomi = 6.5f;
        coreHideDenomi = 5.5f;
        if (lookatTarget) {
            lookatTarget.SetTarget(nullTarget);
        }
        laserOption = GetComponent<LaserOption>();
        sandstarRawKnockEndurance = 5000;
        sandstarRawKnockEnduranceLight = 5000;
        killByCriticalOnly = true;
    }    

    private void CoreHide() {
        coreDetection.SetActive(false);
        cellienCore.SetActive(false);
        isCoreShowed = false;
        if (state != State.Dead) {
            searchTarget[0].SetActive(true);
        }
        ResetKnockRemain();
    }

    private void CoreShow() {
        cellienCore.SetActive(true);
        coreDetection.SetActive(true);
        isCoreShowed = true;
        coreTimeRemain = coreTimeMax;
        coreShowHP = nowHP;
        coreHideConditionDamage = GetCoreHideConditionDamage();
        healEffectEmitted = false;
        EmitEffect(0);
        searchTarget[0].SetActive(false);
    }

    public override void SetSandstarRaw() {
        base.SetSandstarRaw();
        if (state != State.Dead) {
            coreHideDenomi = 8f;
        }
    }

    void ThrowStartChangeVelocity(int index) {
        if (throwing && index >= 0 && index < throwing.throwSettings.Length) {
            if (targetTrans) {
                float distance = Vector3.Distance(targetTrans.position, throwing.throwSettings[index].from.transform.position);
                if (distance > throwSpeedUpBorderDistance) {
                    throwing.throwSettings[index].velocity = throwSpeedMin + (distance - throwSpeedUpBorderDistance) * throwSpeedUpRate;
                } else {
                    throwing.throwSettings[index].velocity = throwSpeedMin;
                }
            } else {
                throwing.throwSettings[index].velocity = throwSpeedMin;
            }
            throwing.ThrowStart(index);
        }
    }

    protected override void KnockHeavyProcess() {
        base.KnockHeavyProcess();
        if (!isCoreShowed && state != State.Dead) {
            CoreShow();
            if (CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Knock)) {
                TrophyManager.Instance.CheckTrophy(TrophyManager.t_AkulaKnock, true);
            }
        }
        laserOption.CancelLaser();
    }

    protected override void Start_Process_Dead() {
        base.Start_Process_Dead();
        laserOption.CancelLaser();
    }

    protected override void DeadProcess() {
        base.DeadProcess();
        CharacterManager.Instance.SetFriendsAgentEnabled(false, 2);
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        gravityMultiplier = 0;
        if (!battleStarted && target) {
            BattleStart();
        }
        if (GameManager.Instance.save.difficulty >= GameManager.difficultyVU || sandstarRawEnabled) {
            weakProgress = (nowHP <= GetMaxHP() / 3 ? 2 : nowHP <= GetMaxHP() * 2 / 3 ? 1 : 0);
        } else {
            weakProgress = (nowHP <= GetMaxHP() / 2 ? 1 : 0);
        }
        if (state != State.Attack && laserOption && (laserOption.isLightCharging || laserOption.isBlasting)) {
            laserOption.CancelLaser();
        }
        if (lookatTarget) {
            if (targetTrans) {
                lookatTarget.SetTarget(targetTrans);
            } else {
                lookatTarget.SetTarget(nullTarget);
            }
            bool notStopSick = !GetSick(SickType.Stop);
            if (lookatTarget.enabled != notStopSick) {
                lookatTarget.enabled = notStopSick;
            }
            lookatTarget.SetFollowSpeed(Mathf.Max(eyeSpeed[weakProgress] * (sandstarRawEnabled ? 0.5f : 1f) * Mathf.Min(GameManager.Instance.minmiPurple ? 1f / 3f : 1f , CharacterManager.Instance.riskyDecSqrt) * (GetSick(SickType.Slow) ? 2f : 1f) , 0.1f));
        }
        if (isCoreShowed && state != State.Dead) {
            if (coreTimeRemain > 1 && nowHP <= GetCoreHideBorder()) {
                coreTimeRemain = 1;
            }
            coreTimeRemain -= deltaTimeCache * (coreTimeRemain >= 1f && nowHP > 1 ? CharacterManager.Instance.riskyIncSqrt : 1f);
            if (!healEffectEmitted && coreTimeRemain < 1) {
                EmitEffect(1);
                healEffectEmitted = true;
            }
            if (coreTimeRemain < 0) {
                CoreHide();
            }
            knockRemain = knockEndurance;
        } else {
            knockRemainLight = knockEnduranceLight;
        }
        if (followerResetTimer > 0f) {
            followerResetTimer -= deltaTimeCache;
            if (followerResetTimer <= 0f) {
                followerRestRemain = (8f - weakProgress * 2f) * (sandstarRawEnabled ? 0.5f : 1f);
                followerProjector.gameObject.SetActive(false);
            }
        }
        if (followerRestRemain > 0) {
            followerRestRemain -= deltaTimeCache;
            if (followerProjector.gameObject.activeSelf) {
                followerProjector.gameObject.SetActive(false);
            }
        } else {
            bool followingEnabled = followerResetTimer <= 0f && (GetCanControl() || (state == State.Attack && attackType != 6 && !throwing.GetAnyReady() && stateTime > 0.2f));
            Vector3 targetPosition = CharacterManager.Instance.playerTrans.position;
            targetPosition.y = trans.position.y;
            float sqrDist = (targetPosition - follower.position).sqrMagnitude;
            if (followingEnabled) {
                if (followerProjector.gameObject.activeSelf && followerOnCheck.flag) {
                    state = State.Attack;
                    AttackBase(7, 1, 1.7f, 0, 30f / 60f, 60f / 60f);
                    followThrowCount = 0;
                    followThrowTimeRemain = 0.5f;
                    followerRestRemain = (8f - weakProgress * 2f) * (sandstarRawEnabled ? 0.5f : 1f);
                    followerProjector.gameObject.SetActive(false);
                } else {
                    if (onCheck.flag) {
                        if (!followerProjector.gameObject.activeSelf) {
                            Vector3 appPos = followerAppearPoint[0].position;
                            if ((followerAppearPoint[1].position - targetPosition).sqrMagnitude > (appPos - targetPosition).sqrMagnitude) {
                                appPos = followerAppearPoint[1].position;
                            }
                            follower.position = appPos;
                            followerOnCheck.flag = false;
                            followerProjector.gameObject.SetActive(true);
                        }
                        float speed = (sqrDist < 18f * 18f ? 2.4f : 15f) * (weakProgress >= 2 ? 3.4f / 2.4f : weakProgress == 1 ? 2.8f / 2.4f : 1f);
                        if (sandstarRawEnabled) {
                            speed *= 2f;
                        }
                        if (GetSick(SickType.Stop)) {
                            speed = 0f;
                        } else if (GetSick(SickType.Slow)) {
                            speed *= 0.5f;
                        }
                        speed *= Mathf.Max(GameManager.Instance.minmiPurple ? 3f : 1f, CharacterManager.Instance.riskyIncSqrt);
                        if (sqrDist > 0.01f) {
                            follower.position += (targetPosition - follower.position).normalized * speed * deltaTimeCache;
                        }
                    }
                }
            }
            if (sqrDist < 18f * 18f) {
                if (followingEnabled) {
                    SetFollowerState(0);
                } else {
                    SetFollowerState(1);
                }
            } else {
                SetFollowerState(2);
            }
        }
        bool ddFlag = (followerProjector.gameObject.activeSelf && followerProjector.enabled);
        if (followerDD.activeSelf != ddFlag) {
            followerDD.SetActive(ddFlag);
        }
        bigBombRestRemain -= deltaTimeCache;
        if (followThrowTimeRemain > 0f) {
            followThrowTimeRemain -= deltaTimeCache;
            if (followThrowCount < followThrowBorder.Length && followThrowTimeRemain <= followThrowBorder[followThrowCount]) {
                followThrowCount++;
                throwing.ThrowStart(12);
            }
        }
    }

    void SetFollowerState(int newState) {
        if (followerState != newState) {
            followerState = newState;
            if (followerState == 0) {
                followerProjector.material = followerBrightMat;
                if (!followerProjector.enabled) {
                    followerProjector.enabled = true;
                }
            } else if (followerState == 1) {
                followerProjector.material = followerDarkMat;
                if (!followerProjector.enabled) {
                    followerProjector.enabled = true;
                }
            } else {
                if (followerProjector.enabled) {
                    followerProjector.enabled = false;
                }
            }
        }
    }

    protected override void BattleStart() {
        base.BattleStart();
        actDistNum = 1;
    }

    protected override bool CommonLockon() {
        return false;
    }

    public override bool CheckGrounded(float tolerance = 0.5f) {
        return true;
    }
    
    protected override void Attack() {
        base.Attack();
        int typeMax = bigBombRestRemain > 0 ? 3 : 4;
        int attackTemp = 1;
        if (attackSave >= 0) {
            attackTemp = Random.Range(0, typeMax);
            if (attackSave == attackTemp) {
                attackTemp = Random.Range(0, typeMax);
            }
        }
        attackSave = attackTemp;
        float intervalPlus = 0f;
        if (sandstarRawEnabled) {
            intervalPlus = (weakProgress == 2 ? Random.Range(1.1f, 1.4f) : weakProgress == 1 ? Random.Range(1.25f, 1.7f) : Random.Range(1.4f, 2.0f)) * (isCoreShowed ? 0.75f : 1f);
        } else {
            intervalPlus = (weakProgress == 2 ? Random.Range(1.3f, 1.6f) : weakProgress == 1 ? Random.Range(1.45f, 1.9f) : Random.Range(1.6f, 2.2f)) * (isCoreShowed ? 0.75f : 1f);
        }
        int childType = Random.Range(0, weakProgress + 1);
        switch (attackTemp) {
            case 0://Missile
                AttackBase(0 + childType, 1f, 1f, 0, 60f / 60f, 60f / 60f + intervalPlus + childType * 0.8f);
                break;
            case 1://Block
                AttackBase(3 + childType, 1f, 1f, 0, 60f / 60f, 60f / 60f + intervalPlus + childType * 0.8f);
                break;
            case 2:
                AttackBase(6, 1.3f, 50f, 0, 190f / 60f, 190f / 60f + intervalPlus);
                break;
            case 3:
                AttackBase(8, 1f, 1f, 0, 45f / 60f, 45f / 60f + intervalPlus);
                bigBombRestRemain = 6f;
                break;
        }
        SuperarmorStart();
    }

    public void SetFollowerResetTimer() {
        followerResetTimer = 1f;
    }

}
