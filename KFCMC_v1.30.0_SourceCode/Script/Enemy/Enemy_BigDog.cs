using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_BigDog : EnemyBaseBoss {

    public bool isReaper;
    public AudioSource[] footAudioSource;
    public AudioSource slimeAudioSource;
    public AudioSource[] growlIdleAudioSource;
    public AudioSource[] growlAttackAudioSource;
    public Transform[] movePivot;
    public Transform[] quakePivot;
    public GameObject[] scarObj;
    public GameObject coreObj;
    public Transform shirome;
    public Transform kurome;
    public Transform heightPivot;
    public Transform[] bombPoints;
    public GameObject eventController;
    public Transform[] eventCameraPivot;
    public Transform[] playerRestartPivot;
    public Renderer charRenderer;
    public ScrollTexture charRendScroll;
    public EnemyDeath[] finalFormEffect;
    public GameObject finalFormNavMesh;
    public GameObject[] finalFormActivate;
    public GameObject[] finalFormDeactivate;
    public Transform[] summonPivot;
    public int defeatLightingNumber = -1;
    public int defeatAmbientNumber = -1;
    public bool animOverrideEnabled;
    [System.Serializable]
    public class AnimOverride {
        public Transform bone;
        public bool posEnable;
        public Vector3 localPos;
        public bool rotEnable;
        public Vector3 localRot;
    }
    public AnimOverride[] animOverrides;

    int moveIndex = -1;
    int attackSave = -1;
    int footSoundIndex = -1;
    float attackSpeed = 1f;
    int throwCount;
    int throwMax;
    int throwIndex = -1;
    float throwStartStateTime;
    int bombType;
    int missileReadyCount;
    int missileStartCount;
    int missileMax;
    float missileTimeRemain;
    int bombMax;
    int footMovedIndex = -1;
    int moveDirectionHash;
    int knockDirectionHash;
    int scarCondition;
    int[] scarDamage = new int[4];
    int scarCount;
    float slimeAudioCurrentVolume;
    float slimeAudioCurrentVelocity;
    float growlIdleTimeRemain;
    float growlAttackTimeRemain;
    int eventState;
    bool eventEnabledAttack;
    float restartEventTimeRemain;
    float kabanTalkEventTimer;
    bool blockReadiedFlag;
    bool finalFormFlag;
    int finalFormLayer;
    int scarIndexSave;
    float lastDamagedTime;
    Vector3 smoothKuromeVelocity;
    /*
    float healReserveTimeRemain;
    float healProcessTimeRemain;
    */
    float bigBombTimeRemain;
    float blockTimeRemain;
    bool damageBias;
    bool animFinalizedFlag;
    bool allFriendsInformationReserved;
    bool weakChangedFlag;
    bool collapseReserved;

    const int throwFreeBlock = 2;
    const int throwMissileBase = 3;
    const int throwBigBombBase = 11;
    const int throwBombBase = 12;
    const int effMove= 0;
    const int effVertical = 4;
    const int effHorizontal = 8;
    const int effLegImpact = 12;
    const int effBlockReady = 17;
    const int effBlockStart = 19;
    const int effSuperPress = 21;
    const int effDoubleMove = 22;
    const int effDoubleVertical = 24;
    const int effDoubleImpact = 26;
    const int effFaceJump = 28;
    const int effFaceImpact = 29;
    const int effTailMove = 30;
    const int effBigBombReady = 33;
    const int effBombReady = 34;
    const int effLegBreak = 44;
    const int effKnockLight = 48;
    const int effKnockLightFF = 52;
    const int effEventAttack = 53;
    const int effEscapeReady = 54;
    const int effSuperBreak = 55;
    const int effStartFinalForm = 56;
    const int effKnockHeavy = 57;
    const int effBlockReadyFF = 58;
    const int effBlockStartFF = 59;
    const int effDeadSave = 60;
    const int effHeal = 61;
    const int effReaperDefeat = 62;
    
    const int dropID_EX = 97;
    const int rewardMinmiShiftNum = 3;
    const int summonID = 3;
    const int summonLevel = 4;

    struct BombDist {
        public int index;
        public int sqrDist;
        public BombDist(int index, int sqrDist) {
            this.index = index;
            this.sqrDist = sqrDist;
        }
    }
    List<BombDist> bombDistList;

    const float blockSpeedMin = 22f;
    const float blockSpeedUpBorderDistance = 30f;
    const float blockSpeedUpRate = 0.4f;

    static readonly float[] moveSpeedArray = new float[10] { 1.0f, 1.0f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f, 1.0f, 1.0f, 1.5f };
    static readonly float[] moveScaleArray = new float[10] { 1.0f, 1.0f, 1.5f, 1.5f, 1.5f, 1.5f, 1.0f, 1.0f, 1.0f, 1.0f };
    static readonly Vector3 vecRight = Vector3.right;
    static readonly Vector3 vecRand = new Vector3(2f, 2f, 2f);

    protected override void Awake() {
        base.Awake();
        if (isReaper) {
            destroyOnDead = false;
            changeMusicEnabled = false;
            roveInterval = 8f;
        }
        attackedTimeRemainOnDamage = 2f;
        deadTimer = 3f;
        knockEndurance = 1000000;
        sandstarRawKnockEndurance = 1000000;
        if (isReaper) {
            knockEnduranceLight = 12000;
            sandstarRawKnockEnduranceLight = 12000;
        } else {
            knockEnduranceLight = 18000;
            sandstarRawKnockEnduranceLight = 18000;
        }
        stoppingDistanceBattle = 10f;
        attackWaitingLockonRotSpeed = 0f;
        sandstarRawLockonSpeed = attackLockonDefaultSpeed = 3f;
        attackedTimeRemainReduceRadius = 30f;
        moveDirectionHash = Animator.StringToHash("MoveDirection");
        knockDirectionHash = Animator.StringToHash("KnockDirection");
        finalFormLayer = anim.GetLayerIndex("FinalForm");
        bombDistList = new List<BombDist>(bombPoints.Length);
        cannotDoubleKnockDown = true;
    }

    protected override void Start() {
        base.Start();
        ResetScar();
        bool restartEnabled = false;
        GameObject[] eventObj = GameObject.FindGameObjectsWithTag("GameController");
        if (eventObj != null && eventObj.Length > 0) {
            for (int i = 0; i < eventObj.Length; i++) {
                Event_BigDog eventTemp = eventObj[i].GetComponent<Event_BigDog>();
                if (eventTemp != null) {
                    BattleRestart(eventTemp);
                    restartEnabled = true;
                    break;
                }
            }
        }
        if (isForAmusement) {
            SetForNoEvent();
        } else if (!restartEnabled) {
            GameObject[] dungeonObj = GameObject.FindGameObjectsWithTag("Dungeon");
            if (dungeonObj != null && dungeonObj.Length > 0) {
                for (int i = 0; i < dungeonObj.Length; i++) {
                    HoldPrefab holdPrefabTemp = dungeonObj[i].GetComponent<HoldPrefab>();
                    if (holdPrefabTemp != null) {
                        SetForNoEvent();
                        break;
                    }
                }
            }
        }
    }

    protected override void UpdateAC_AnimSpeed() {
        float temp = Mathf.Clamp(GetMaxSpeed() / 9f, 0.5f, 10f);
        if (animParam.animSpeed != temp) {
            animParam.animSpeed = temp;
            anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.AnimSpeed], animParam.animSpeed);
        }
    }

    protected override void SetLevelModifier() {
        if (isReaper) {
            dropItem[0] = dropID_EX;
            SetDropRate(10000);
        }
    }

    public override void SetSandstarRaw() {
        base.SetSandstarRaw();
        ResetScar();
        if (finalFormFlag) {
            SetFinalStatus();
        }
        if (eventState >= 3) {
            // defensePower = 50;
            defensePower = 0;
        }
        lastDamagedTime = 0f;
        notAffectedByRisky = false;
    }

    void ResetScar() {
        for (int i = 0; i < scarDamage.Length; i++) {
            scarDamage[i] = 0;
            scarObj[i].SetActive(false);
        }
        scarCount = 0;
        scarCondition = Mathf.RoundToInt(nowHP * (isForAmusement ? 0.075f : 0.125f));
        coreObj.SetActive(false);
    }    

    void MoveStart(int index) {
        moveIndex = index;
    }

    void MoveEnd() {
        moveIndex = -1;
        LockonEnd();
    }

    void MoveAttack12() {
        LockonEnd();
        fbStepTime = 45f / 60f;
        fbStepMaxDist = 12f;
        fbStepIgnoreY = true;
        fbStepEaseType = EasingType.SineInOut;
        ForwardOrBackStep(1.5f);
    }

    void SwingStart(int index) {
        EmitEffect(effMove + index);
        ParticlePlay(index);
    }

    void SwingVertical(int index) {
        EmitEffect(effVertical + index);
    }

    void SwingHorizontal(int index) {
        EmitEffect(effHorizontal + index);
    }

    void DoubleSwing() {
        EmitEffect(effDoubleVertical);
        EmitEffect(effDoubleVertical + 1);
    }

    void TailSwing() {
        EmitEffect(effTailMove + 1);
    }

    void FaceJump() {
        EmitEffect(effFaceJump);
    }

    void Impact(int index) {
        if (index == 0) {
            if (!isItem) {
                FootSound();
                CameraManager.Instance.SetQuake(quakePivot[index].position, 4, 4, 0, 0, 1f, 10f, dissipationDistance_Boss);
            }
        } else if (index >= 1 && index <= 5) {
            if (state == State.Attack) {
                EmitEffect(effLegImpact + (index - 1));
                CameraManager.Instance.SetQuake(quakePivot[index].position, 12, 4, 0, 0, 1.5f, 4f, dissipationDistance_Boss);
            }
        } else if (index == 6) {
            if (state == State.Attack) {
                EmitEffect(effSuperPress);
                CameraManager.Instance.SetQuake(quakePivot[index].position, 16, 4, 0, 0.5f, 2f, 8f, dissipationDistance_Boss * 2f);
            }
        } else if (index == 7) {
            if (state == State.Attack) {
                EmitEffect(effDoubleImpact);
                EmitEffect(effDoubleImpact + 1);
                CameraManager.Instance.SetQuake(quakePivot[index].position, 14, 4, 0, 0, 1.5f, 4f, dissipationDistance_Boss);
            }
        } else if (index == 8) {
            if (state == State.Attack) {
                EmitEffect(effFaceImpact);
                CameraManager.Instance.SetQuake(quakePivot[index].position, 16, 4, 0, 0.5f, 2f, 8f, dissipationDistance_Boss * 2f);
            }
        } else if (index == 9) {
            if (state == State.Attack) {
                EmitEffect(effTailMove + 2);
                CameraManager.Instance.SetQuake(quakePivot[index].position, 12, 4, 0, 0, 1.5f, 4f, dissipationDistance_Boss);
            }
        }
    }

    void FootSound() {
        if (footSoundIndex < 0) {
            footSoundIndex = Random.Range(0, footAudioSource.Length);
        } else {
            footSoundIndex = (footSoundIndex + Random.Range(1, footAudioSource.Length)) % footAudioSource.Length;
        }
        footAudioSource[footSoundIndex].Play();
    }

    void SetFootMovedIndex(int index) {
        footMovedIndex = index;
    }

    float GetThrowVelocity(int index, float minSpeed, float borderDistance, float speedUpRate) {
        if (throwing && targetTrans && index >= 0 && index < throwing.throwSettings.Length) {
            float distance = Vector3.Distance(targetTrans.position, throwing.throwSettings[index].from.transform.position);
            if (distance > borderDistance) {
                return minSpeed + (distance -borderDistance) * speedUpRate;
            }
        }
        return minSpeed;
    }

    void SearchThrowPivotPos(int index, Vector3 avoidPos, float avoidDist) {
        float condHeight = trans.position.y + 1f;
        float minDist = float.MaxValue;
        int minIndex = Random.Range(0, bombPoints.Length);
        float avoidSqrDist = avoidDist * avoidDist;
        if (targetTrans) {
            Vector3 targetPos = targetTrans.position;
            for (int i = 0; i < bombPoints.Length; i++) {
                if (bombPoints[i].position.y >= condHeight) {
                    float sqrDistTemp = (targetPos - bombPoints[i].position).sqrMagnitude;
                    if (sqrDistTemp < minDist && (avoidPos - bombPoints[i].position).sqrMagnitude > avoidSqrDist) {
                        minDist = sqrDistTemp;
                        minIndex = i;
                    }
                }
            }
        }
        throwing.throwSettings[index].from.transform.position = bombPoints[minIndex].position;
    }

    void ThrowReadyBlock(int index) {
        if (!blockReadiedFlag) {
            blockReadiedFlag = true;
            throwMax = Mathf.Clamp(6 + weakProgress, 6, 10);
            throwCount = 0;
            throwing.ThrowReady(index);
        }
    }

    void ThrowStartBlock(int index) {
        if (blockReadiedFlag && state == State.Attack) {
            if (finalFormFlag) {
                index = throwFreeBlock;
            }
            throwIndex = index;
            throwStartStateTime = stateTime;
            throwing.throwSettings[index].velocity = GetThrowVelocity(index, blockSpeedMin, blockSpeedUpBorderDistance, blockSpeedUpRate);
            throwing.throwSettings[index].randomDirection = vecZero;
            throwing.throwSettings[index].randomForceRate = 0f;
            throwing.throwSettings[index].angularVelocity = Random.insideUnitSphere * 20f;
            throwing.ThrowStart(index);
            throwCount++;
            throwing.throwSettings[index].randomDirection = vecRand * 0.9f;
            throwing.throwSettings[index].randomForceRate = 0.25f;
            if (!finalFormFlag) {
                EmitEffect(effBlockStart + index);
            } else {
                EmitEffect(effBlockStartFF);
            }
        }
    }
    
    void MissileReady() {
        bombType = 0;
        missileReadyCount = 0;
        missileStartCount = 0;
        missileMax = (weakProgress >= 3 ? 8 : weakProgress >= 1 ? 6 : 4);
        missileTimeRemain = 1f;
    }

    void BombReady(int type) {
        bombType = type;
        if (bombType == 1) {
            SearchThrowPivotPos(throwBigBombBase, targetTrans ? targetTrans.position : trans.position, 1.8f);
            throwing.ThrowReady(throwBigBombBase);
            EmitEffect(effBigBombReady);
        } else if (bombType == 2) {
            bombMax = Mathf.Clamp(6 + weakProgress, 6, 10);
            if (targetTrans) {
                float condHeight = trans.position.y + 1f;
                Vector3 targetPos = targetTrans.position;
                bombDistList.Clear();
                BombDist bombDistTemp = new BombDist(0, 0);
                for (int i = 0; i < bombPoints.Length; i++) {
                    if (bombPoints[i].position.y >= condHeight) {
                        float sqrDist = (targetPos - bombPoints[i].position).sqrMagnitude;
                        if (sqrDist >= 1.8f * 1.8f) {
                            bombDistTemp.index = i;
                            bombDistTemp.sqrDist = (int)sqrDist;
                            bombDistList.Add(bombDistTemp);
                        }
                    }
                }
                bombDistList.Sort((a, b) => a.sqrDist - b.sqrDist);
                for (int i = 0; i < bombMax; i++) {
                    throwing.throwSettings[throwBombBase + i].from.transform.position = bombPoints[bombDistList[i].index].position;
                    throwing.ThrowReady(throwBombBase + i);
                    EmitEffect(effBombReady + i);
                }
            } else {
                for (int i = 0; i < bombMax; i++) {
                    throwing.throwSettings[throwBombBase + i].from.transform.position = bombPoints[Random.Range(0, bombPoints.Length)].position;
                    throwing.ThrowReady(throwBombBase + i);
                    EmitEffect(effBombReady + i);
                }
            }
        }
    }

    void BombStart() {
        if (state == State.Attack) {
            if (bombType == 1) {
                throwing.throwSettings[throwBigBombBase].velocity = GetThrowVelocity(throwBigBombBase, 5f, 10f, 0.2f);
                throwing.ThrowStart(throwBigBombBase);
            } else if (bombType == 2) {
                for (int i = 0; i < bombMax; i++) {
                    throwing.throwSettings[throwBombBase + i].velocity = GetThrowVelocity(throwBombBase + i, 10f, 10f, 0.4f);
                    throwing.ThrowStart(throwBombBase + i);
                }
            }
        }
    }

    void SummonDebris(Vector3 pivotPos) {
        Vector3 posTemp = trans.position;
        if (NavMesh.SamplePosition(pivotPos, out NavMeshHit hit, 1.0f, NavMesh.AllAreas)) { 
            posTemp = hit.position;
        }
        Enemy_Debris eBase = StageManager.Instance.dungeonController.SummonSpecificEnemy(summonID, summonLevel, posTemp).GetComponent<Enemy_Debris>();
        if (eBase) {
            eBase.LookAtIgnoreY(CharacterManager.Instance.playerTrans.position);
            if (CharacterManager.Instance.pCon) {
                eBase.RegisterTargetHate(CharacterManager.Instance.pCon, 40f);
            }
            eBase.dropExpDisabled = true;
            eBase.SetForEvent();
        }
    }

    public void ReceiveScarDamage(int index, int damage, bool effectEnabled = true) {
        if (!isReaper && index >= 0 && index < scarDamage.Length && defensePower < 100) {
            scarIndexSave = index;
            scarDamage[index] += damage;
            if (scarDamage[index] >= scarCondition && !scarObj[index].activeSelf) {
                if (effectEnabled) {
                    EmitEffect(effLegBreak + index);
                }
                int debrisDifficulty = 0;
                if (GameManager.Instance.save.difficulty >= GameManager.difficultyEN) {
                    debrisDifficulty++;
                }
                if (GameManager.Instance.save.difficulty >= GameManager.difficultyVU) {
                    debrisDifficulty++;
                }
                if (sandstarRawEnabled) {
                    debrisDifficulty++;
                }
                if (debrisDifficulty >= 1 && CharacterManager.Instance.GetFriendsCostSum() > CharacterManager.riskyCost && index < summonPivot.Length && summonPivot[index]) {
                    if (debrisDifficulty >= 3) {
                        for (int i = 0; i < summonPivot.Length; i++) {
                            SummonDebris(summonPivot[i].position);
                        }
                    } else if (debrisDifficulty >= 2 || scarCount >= 3) {
                        SummonDebris(summonPivot[index].position);
                    }
                }
                knockRemainLight = 0f;
                if (searchTarget[index]) {
                    searchTarget[index].SetActive(false);
                }
                scarObj[index].SetActive(true);
                scarCount++;
                if (scarCount >= 4 && !finalFormFlag) {
                    knockRemain = knockEndurance = 1750;
                    for (int i = 0; i < 4; i++) {
                        if (searchTarget[i]) {
                            searchTarget[i].SetActive(true);
                        }
                    }
                }
            }
        }
    }

    protected override void UpdateAC_Move() {
        bool temp = nowSpeed > 0.1f || nowSpeed < -0.1f;
        if (animParam.move == false && temp == true) {
            anim.SetInteger(moveDirectionHash, footMovedIndex == 0 ? 1 : 0);
        }
        base.UpdateAC_Move();
    }

    protected override void KnockLightProcess() {
        if (!isSuperarmor){
            if (!isReaper && eventState == 0) {
                eventState = 1;
                retargetingToPlayer = true;
                retargetingConditionTime = 0.001f;
            }
            if (finalFormFlag) {
                EmitEffect(effKnockLightFF);
                CameraManager.Instance.SetQuake(transform.position, 4, 4, 0, 0, 1.5f, 8f);
                anim.SetInteger(knockDirectionHash, 0);
            } else {
                EmitEffect(effKnockLight + scarIndexSave);
                CameraManager.Instance.SetQuake(movePivot[2 + scarIndexSave].position, 4, 4, 0, 0, 1.5f, 8f);
                anim.SetInteger(knockDirectionHash, scarIndexSave);
            }
            paperPlaneTolerance = 0;
            confuseTolerance = 0;
        }
        base.KnockLightProcess();
    }

    void EventStart() {
        eventState = 2;
        knockEnduranceLight = 1000000;
        mutekiTimeRemain = 5f;
        dontTargetingTimeRemain = 0f;
        targetFixingTimeRemain = 0f;
        decoySave = null;
        confuseTime = 0f;
        attractionTime = 0f;
        angryAttractionTime = 0f;
        angryFixTime = 0f;
        SuperarmorStart();
        MoveStart(6);
        throwing.ThrowCancelAll(false);
        if (enemyCanvasLoaded) {
            for (int i = 0; i < enemyCanvasChildObject.Length; i++) {
                enemyCanvasChildObject[i].SetActive(false);
            }
        }
        PauseController.Instance.pauseEnabled = false;
        BGM.Instance.StopFade(2f);
        CameraManager.Instance.SetEventCameraFollowTarget(eventCameraPivot[0], 4f, 1f, 8f);
        CharacterManager.Instance.pCon.ForceStopForEvent(4f);
        CharacterManager.Instance.BossTimeStop();
    }

    void ChangeMatForFinalForm() {
        System.Array.Resize(ref charRendScroll.scrollTextureSettings, 1);
        Material[] matsTemp = charRenderer.materials;
        System.Array.Resize(ref matsTemp, 3);
        charRenderer.materials = matsTemp;
    }

    void SetFinalStatus() {
        knockRemain = knockEndurance = 1000000;
        attackLockonDefaultSpeed = 0f;
        attackWaitingLockonRotSpeed = 0f;
        sandstarRawLockonSpeed = 0f;
        maxSpeed = 0f;
        walkSpeed = 0f;
        angularSpeed = 0f;
        attackedTimeRemainOnDamage = 3f;
        killByCriticalOnly = true;
        actDistNum = 3;
    }

    void StartFinalForm() {
        finalFormFlag = true;
        for (int i = 0; i < finalFormEffect.Length; i++) {
            BootDeathEffect(finalFormEffect[i]);
        }
        for (int i = 0; i < 4 && i < searchTarget.Length; i++) {
            if (searchTarget[i] != null) {
                searchTarget[i].SetActive(false);
            }
        }
        for (int i = 0; i < scarObj.Length; i++) {
            if (scarObj[i] != null) {
                scarObj[i].SetActive(false);
            }
        }
        for (int i = 0; i < finalFormActivate.Length; i++) {
            if (finalFormActivate[i] != null) {
                finalFormActivate[i].SetActive(true);
            }
        }
        for (int i = 0; i < finalFormDeactivate.Length; i++) {
            if (finalFormDeactivate[i] != null) {
                finalFormDeactivate[i].SetActive(false);
            }
        }
        ChangeMatForFinalForm();
        SetFinalStatus();
        PlayGrowlAudio(3);
        EmitEffect(effStartFinalForm);
    }

    protected override void KnockHeavyProcess() {
        if (!finalFormFlag) {
            base.KnockHeavyProcess();
            StartFinalForm();
        }
    }

    void KnockDownEffect() {
        coreObj.SetActive(true);
        EmitEffect(effKnockHeavy);
        if (!isItem) {
            CameraManager.Instance.SetQuake(transform.position, 8, 4, 0, 0.5f, 2f, 8f, 200f);
        }
    }

    void SetFinalForm() {
        anim.SetLayerWeight(finalFormLayer, 1);
        Instantiate(finalFormNavMesh, transform);
        animFinalizedFlag = true;
    }

    void KuromeMove() {
        Vector3 targetPos = vecZero;
        targetPos.z = kurome.localPosition.z;
        if (targetTrans) {
            Vector3 diff = (shirome.InverseTransformPoint(targetTrans.position) - shirome.localPosition).normalized;
            targetPos.x = Mathf.Clamp(diff.x * 0.25f, -0.2f, 0.2f);
            targetPos.y = Mathf.Clamp(diff.y * 0.25f, -0.2f, 0.2f);
        }
        kurome.localPosition = Vector3.SmoothDamp(kurome.localPosition, targetPos, ref smoothKuromeVelocity, 1f);
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        KuromeMove();
        if (knockEndurance >= 100000) {
            knockRemain = knockEndurance;
        }
        if (actDistNum == 0 && target != null) {
            BattleStart();
        }
        weakProgress = 0;
        float maxHPTemp = GetMaxHP();
        if (maxHPTemp > 0) {
            float nowHPTemp = nowHP;
            if (damageBias) {
                nowHPTemp -= maxHPTemp * 0.1666667f;
            }
            float damageRate = (maxHPTemp - nowHPTemp) / maxHPTemp * (notAffectedByRisky ? Mathf.Clamp(CharacterManager.Instance.riskyIncSqrt, 1f, 2f) : CharacterManager.Instance.riskyIncrease);
            weakProgress = Mathf.Clamp((int)(damageRate * (GameManager.Instance.save.difficulty >= GameManager.difficultyVU || sandstarRawEnabled ? 5f : 4f)), 0, 4);
        }
        if (sandstarRawEnabled) {
            weakProgress += 1;
        }
        float slimeTargetVolume = 0f;
        float maxSpeedTemp = GetMaxSpeed();
        if (state == State.Attack && stateTime < attackStiffTime * 0.8f) {
            slimeTargetVolume = 0.8f;
        } else if (state == State.Damage) {
            slimeTargetVolume = 0.6f;
        } else if (nowSpeed > 0.1f && maxSpeedTemp > 0.1f) {
            slimeTargetVolume = 0.1f + Mathf.Clamp01(nowSpeed / maxSpeedTemp) * 0.3f;
        } else if (actDistNum >= 1) {
            slimeTargetVolume = 0.1f;
        }
        slimeAudioCurrentVolume = Mathf.SmoothDamp(slimeAudioCurrentVolume, slimeTargetVolume, ref slimeAudioCurrentVelocity, 0.5f);
        if (slimeAudioSource.volume != slimeAudioCurrentVolume) {
            slimeAudioSource.volume = slimeAudioCurrentVolume;
        }
        growlIdleTimeRemain -= deltaTimeCache;
        growlAttackTimeRemain -= deltaTimeCache;
        if (actDistNum != 0 && state != State.Attack && growlIdleTimeRemain < 0f && growlAttackTimeRemain < 0f) {
            int index = Random.Range(0, growlIdleAudioSource.Length);
            growlIdleAudioSource[index].Play();
            growlIdleTimeRemain = growlIdleAudioSource[index].clip.length + Random.Range(1f, 4f);
        }
        if (state == State.Attack && bombType == 0) {
            missileTimeRemain -= deltaTimeMove;
            int tempReadyMax = Mathf.Min((int)((1f - missileTimeRemain) * 2f * missileMax), missileMax);
            while (missileReadyCount < tempReadyMax) {
                throwing.ThrowReady(throwMissileBase + missileReadyCount);
                missileReadyCount++;
            }
            int tempStartMax = Mathf.Min((int)((0.5f - missileTimeRemain) * 2f * missileMax), missileMax);
            while (missileStartCount < tempStartMax) {
                throwing.ThrowStart(throwMissileBase + missileStartCount);
                missileStartCount++;
            }
        }
        if (battleStarted && lastDamagedTime < 20f) {
            lastDamagedTime += deltaTimeCache;
        }
        if (state != State.Attack && anyAttackEnabled) {
            ReleaseAttackDetections();
        }
        if (finalFormFlag && state != State.Damage) {
            if (!coreObj.activeSelf) {
                coreObj.SetActive(true);
            }
            if (!animFinalizedFlag) {
                SetFinalForm();
            }
        }
        if (bigBombTimeRemain > 0f) {
            bigBombTimeRemain -= deltaTimeCache;
        }
        if (blockTimeRemain > 0f) {
            blockTimeRemain -= deltaTimeCache;
        }
        if (collapseReserved && StageManager.Instance.graphCollapseNowFloor) {
            StageManager.Instance.graphCollapseNowFloor.Collapse(false);
            collapseReserved = false;
        }
        if (restartEventTimeRemain > 0f) {
            attackedTimeRemain = 4f;
            restartEventTimeRemain -= deltaTimeCache;
            if (restartEventTimeRemain <= 0f) {
                eventState = 4;
                EmitEffect(effSuperBreak);
                scarIndexSave = 0;
                mutekiTimeRemain = 0f;
                anim.SetInteger(knockDirectionHash, scarIndexSave);
                CharacterManager.Instance.pCon.Event_EscapeFromBigDog_02(playerRestartPivot[1].position, playerRestartPivot[1].rotation);
                CharacterManager.Instance.BossTimeStart();
                TakeDamageFixKnock(Mathf.RoundToInt(GetMaxHP() * (6f / Mathf.Clamp(CharacterManager.Instance.GetFriendsCostSum(), 12f, 60f))), playerRestartPivot[0].position, 50000, transform.TransformDirection(vecBack), CharacterManager.Instance.pCon, 3, true);
                ResetScar();
                attackedTimeRemain = 4f;
                if (blackMinmiIDRank > 0) {
                    attackedTimeRemain += blackMinmiIDRank * 0.4f;
                }
                kabanTalkEventTimer = 1f;
                if (MessageUI.Instance) {
                    if (allFriendsInformationReserved) {
                        MessageUI.Instance.SetMessage(TextManager.Get("MESSAGE_ALLFRIENDS"), MessageUI.time_Important, MessageUI.panelType_Information, MessageUI.slotType_Friends);
                        allFriendsInformationReserved = false;
                    }
                    MessageUI.Instance.SetMessage(TextManager.Get("MESSAGE_DEFENSE_ZERO"), MessageUI.time_Important);
                }
            }
        }
        if (kabanTalkEventTimer > 0f) {
            kabanTalkEventTimer -= deltaTimeCache;
            if (kabanTalkEventTimer <= 0f) {
                if (CharacterManager.Instance.GetFriendsExist(1, true)) {
                    MessageBackColor friendsMBC = CharacterManager.Instance.GetMessageBackColor(1);
                    if (friendsMBC != null) {
                        MessageUI.Instance.SetMessageOptional(TextManager.Get("EVENT_KABAN_12_3"), friendsMBC.color1, friendsMBC.color2, friendsMBC.twoToneType, 101, -1, MessageUI.panelType_Speech, MessageUI.slotType_Speech, true);
                        MessageUI.Instance.SetMessageLog(TextManager.Get("EVENT_KABAN_12_3"), 101);
                    }
                }
            }
        }
        if (eventState == 2) {
            CharacterManager.Instance.pCon.SpecificLockon(trans.position);
        }
    }

    protected override void BattleStart() {
        base.BattleStart();
        anim.SetTrigger("BattleStart");
        attackedTimeRemain = isForAmusement ? 1f : 4f;
        lastDamagedTime = 0f;
        if (blackMinmiIDRank > 0) {
            attackedTimeRemain += blackMinmiIDRank * 0.4f;
        }
        actDistNum = 1;
    }

    protected override void BattleEnd() {
        base.BattleEnd();
        if (isReaper && StageManager.Instance) {
            StageManager.Instance.DefeatReaper();
        }
    }

    protected override void DamageCommonProcess() {
        base.DamageCommonProcess();
        if (!battleStarted) {
            BattleStart();
        }
        lastDamagedTime = 0f;
    }

    public override void TakeDamage(int damage, Vector3 position, float knockAmount, Vector3 knockVector, CharacterBase attacker = null, int colorType = 0, bool penetrate = false) {
        if (!isReaper && !weakChangedFlag && attacker == CharacterManager.Instance.pCon && !attacker.isSuperman && knockAmount < 720f) {
            knockAmount = Mathf.Clamp(knockAmount * 1.5f, 0f, 720f);
        }
        base.TakeDamage(damage, position, knockAmount, knockVector, attacker, colorType, penetrate);
    }

    void PlayGrowlAudio(int index) {
        if (growlAttackTimeRemain < 0f) {
            growlAttackAudioSource[index].Play();
            growlAttackTimeRemain = growlIdleAudioSource[index].clip.length;
        }
    }

    float GetInterval() {
        if (finalFormFlag) {
            return Random.Range(1.8f, 2.2f);
        } else if (lastDamagedTime > 10f && targetTrans && GetTargetDistance(true, true, false) > 20f * 20f && weakProgress >= 1) {
            return 3f;
        } else if (weakProgress >= 4) {
            return Random.Range(0.3f, 0.6f);
        } else if (weakProgress == 3) {
            return Random.Range(0.6f, 0.9f);
        } else if (weakProgress == 2) {
            return Random.Range(0.9f, 1.2f);
        } else if (weakProgress == 1) {
            return 2f;
        } else {
            return 4f;
        }
    }
    
    int GetAttackType() {
        int attackTemp = 0;
        float sqrDist = 0f;
        bool tailEnabled = false;
        bool nearEnabled = false;
        bool blockOnly = false;
        bool eventReadied = false;
        if (targetTrans) {
            sqrDist = GetTargetDistance(true, true, false);
            float angle = Vector3.Angle(trans.TransformDirection(vecForward), targetTrans.position - trans.position);
            if (!finalFormFlag) {
                if (angle >= 150 && sqrDist >= 4f * 4f && sqrDist <= 20f * 20f) {
                    tailEnabled = true;
                }
                if (targetTrans.position.y < heightPivot.position.y && sqrDist <= 20f * 20f) {
                    nearEnabled = true;
                }
                if (isReaper && sqrDist > 50f * 50f) {
                    blockOnly = true;
                }
            }
        }
        if (nearEnabled) {
            attackTemp = Random.Range(0, 11);
            if (attackTemp == 7 && !finalFormFlag && blockTimeRemain > 0f) {
                attackTemp = Random.Range(0, 10);
                if (attackTemp >= 7) {
                    attackTemp += 1;
                }
            }
            if (tailEnabled && (attackTemp == 0 || attackTemp == 1 || attackTemp == 7)) {
                attackTemp = 6;
            } else if (!tailEnabled && attackTemp == 6) {
                attackTemp = Random.Range(0, 10);
                if (attackTemp >= 6) {
                    attackTemp += 1;
                }
            }
        } else {
            attackTemp = Random.Range(7, 11);
            if (tailEnabled && attackTemp == 7) {
                attackTemp = 6;
            }
        }
        if (eventState == 1 && nearEnabled && targetTrans == CharacterManager.Instance.playerSearchTarget){
            attackTemp = 3; //EVENT
            eventReadied = true;
        }
        if (attackTemp == 9 && bigBombTimeRemain > 0f) {
            attackTemp = 10;
        }
        if (blockOnly) {
            attackTemp = 7;
        }
        if (attackSave < 0 && !eventReadied) {
            attackTemp = 7;
        }
        return attackTemp;
    }

    protected override void Attack() {
        base.Attack();
        float lockonSpeedTemp = attackLockonDefaultSpeed;
        if (finalFormFlag) {
            attackSpeed = 1f;
        } else {
            if (weakProgress >= 4) {
                attackSpeed = 1.2f;
                lockonSpeedTemp *= 1.2f;
            } else if (weakProgress >= 3) {
                attackSpeed = 1.1f;
                lockonSpeedTemp *= 1.1f;
            } else {
                attackSpeed = 1f;
            }
        }
        eventEnabledAttack = false;
        blockReadiedFlag = false;
        moveIndex = -1;
        throwIndex = -1;
        bombType = -1;
        int attackTemp = GetAttackType();
        if (attackTemp == attackSave) {
            attackTemp = GetAttackType();
        }
        attackSave = attackTemp;
        int rightLeft = Random.Range(0, 2);
        int forwardBack = 0;
        if (targetTrans && (attackTemp == 0 || attackTemp == 1 || attackTemp == 2 || attackTemp == 7)) {
            float angle = Vector3.Cross(trans.TransformDirection(vecForward), targetTrans.position - trans.position).y;
            if (angle > 0.001f) { //Right
                rightLeft = 0;
            } else if (angle < -0.001f) { //Left
                rightLeft = 1;
            }
        }
        if (targetTrans && attackTemp == 2) {
            float angle = Vector3.Cross(trans.TransformDirection(vecRight), targetTrans.position - trans.position).y;
            if (angle > 0f) {
                forwardBack = 2;
            }
        }
        eventEnabledAttack = (attackTemp <= 6);
        if (attackTemp == 0) {
            if (AttackBase(0 + rightLeft, 1.08f, 1.7f, 0, 105f / 60f / attackSpeed, 105f / 60f / attackSpeed + GetInterval(), 0f, attackSpeed, true, lockonSpeedTemp)) {
                MoveStart(0);
                SwingStart(rightLeft);
            }
        } else if (attackTemp == 1) {
            if (AttackBase(2 + rightLeft, 1.04f, 1.4f, 0, 95f / 60f / attackSpeed, 95f / 60f / attackSpeed + GetInterval(), 0f, attackSpeed, true, lockonSpeedTemp)) {
                MoveStart(1);
                SwingStart(rightLeft);
            }
        } else if (attackTemp == 2) {
            if (AttackBase(4 + forwardBack + rightLeft, 1.12f, 2.1f, 0, 55f / 60f / attackSpeed, 55f / 60f / attackSpeed + GetInterval(), 0f, attackSpeed, false)) {
                MoveStart(2 + forwardBack + rightLeft);
                SwingStart(forwardBack + rightLeft);
            }
        } else if (attackTemp == 3) {
            if (eventState == 1 && targetTrans == CharacterManager.Instance.playerSearchTarget) {
                if (AttackBase(10, 1.25f, 5f, 0f, 240f / 60f / 0.6f, 240f / 60f / 0.6f + 4f, 0f, 0.6f, true, 10f)) {
                    PlayGrowlAudio(1);
                    EventStart();
                }
            } else {
                if (AttackBase(10, 1.25f, 5f, 0f, 240f / 60f / attackSpeed, 240f / 60f / attackSpeed + GetInterval(), 0f, attackSpeed, true, lockonSpeedTemp * 1.5f)) {
                    PlayGrowlAudio(1);
                }
            }
        } else if (attackTemp == 4) {
            if (AttackBase(11, 1.12f, 2.1f, 0f, 115f / 60f / attackSpeed, 115f / 60f / attackSpeed + GetInterval(), 0f, attackSpeed, true, lockonSpeedTemp)) {
                MoveStart(7);
                EmitEffect(effDoubleMove);
                EmitEffect(effDoubleMove + 1);
                ParticlePlay(0);
                ParticlePlay(1);
            }
        } else if (attackTemp == 5) {
            if (AttackBase(12, 1.25f, 5f, 0f, 245f / 60f / attackSpeed, 245f / 60f / attackSpeed + GetInterval(), 0f, attackSpeed, true, lockonSpeedTemp * 1.25f)) {
                // MoveStart(8);
                PlayGrowlAudio(2);
            }
        } else if (attackTemp == 6) {
            if (AttackBase(13, 1.08f, 1.7f, 0f, 80f / 60f / attackSpeed, 80f / 60f / attackSpeed + GetInterval(), 0f, attackSpeed, false)) {
                MoveStart(9);
                EmitEffect(effTailMove);
                ParticlePlay(10);
            }
        } else if (attackTemp == 7) {
            if (AttackBase(8 + rightLeft, 1f, 1.1f, 0, 105f / 60f, 105f / 60f + GetInterval(), 0f, 1f, !finalFormFlag, lockonSpeedTemp)) {
                PlayGrowlAudio(0);
                if (finalFormFlag) {
                    SearchThrowPivotPos(throwFreeBlock, targetTrans ? targetTrans.position : trans.position, 1.8f);
                    EmitEffect(effBlockReadyFF);
                    ThrowReadyBlock(throwFreeBlock);
                } else {
                    blockTimeRemain = 12f;
                    EmitEffect(effBlockReady + rightLeft);
                } 
            }
        } else if (attackTemp == 8){
            if (AttackBase(14, 1f, 1.1f, 0, 80f / 60f, 80f / 60f + GetInterval(), 0f, 1f, false)) {
                MissileReady();
                if (finalFormFlag) {
                    PlayGrowlAudio(1);
                }
            }
        } else if (attackTemp == 9) {
            if (AttackBase(14, 1.2f, 5f, 0, 80f / 60f, 80f / 60f + GetInterval(), 0f, 1f, false)) {
                BombReady(1);
                PlayGrowlAudio(3);
                bigBombTimeRemain = 12f;
            }
        } else if (attackTemp == 10) {
            if (AttackBase(14, 1f, 1.1f, 0, 80f / 60f, 80f / 60f + GetInterval(), 0f, 1f, false)) {
                BombReady(2);
                if (finalFormFlag) {
                    PlayGrowlAudio(2);
                }
            }
        }
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        if (moveIndex >= 0 && moveIndex < movePivot.Length && movePivot[moveIndex]) {
            float speedRateTemp = (moveIndex < moveSpeedArray.Length ? moveSpeedArray[moveIndex] : 1f);
            float scaleRateTemp = (moveIndex < moveScaleArray.Length ? moveScaleArray[moveIndex] : 1f);
            ApproachTransformPivot(movePivot[moveIndex], Mathf.Clamp(GetMinmiSpeed() * speedRateTemp, 0f, 13.5f), 0.5f * scaleRateTemp, 0.1f, true);
        }
        if (throwIndex >= 0 && throwIndex < throwing.throwSettings.Length && state == State.Attack) {
            int tempMax = Mathf.Clamp((int)((stateTime - throwStartStateTime) * 6f * throwMax), 0, throwMax);
            for (int i = throwCount; i < tempMax; i++) {
                throwing.throwSettings[throwIndex].angularVelocity = Random.insideUnitSphere * 20f;
                if (!GetSick(SickType.Stop)) {
                    throwing.ThrowStart(throwIndex);
                }
                throwCount++;
            }
            if (throwCount >= throwMax) {
                // throwing.ThrowCancel(throwIndex, true);
                throwIndex = -1;
            }
        }
    }

    public override void AttackStart(int index) {
        if (attackType != 10 || eventState != 2) {
            base.AttackStart(index);
        }
    }

    protected override void ParticlePlay(int index) {
        if (attackType != 10 || eventState != 2) {
            base.ParticlePlay(index);
        }
    }

    protected override void Start_Process_Wait() {
        base.Start_Process_Wait();
        if (finalFormFlag) {
            actDistNum = 3;
        } else {
            if (actDistNum != 0) {
                if (targetTrans && actDistNum == 1 && Random.Range(0, 100) < 25 && GetTargetDistance(true, true, false) <= 7f * 7f) {
                    actDistNum = 2;
                } else {
                    actDistNum = 1;
                }
            }
        }
    }

    void EventAttackEffect() {
        if (eventState == 2) {
            EmitEffect(effEventAttack);
        }
    }

    void SetBlackCurtain() {
        if (eventState == 2) {
            PauseController.Instance.SetBlackCurtain(1f, false);
            if (slimeAudioSource) {
                slimeAudioSource.Stop();
            }
        }
    }

    void AbsorbEvent() {
        if (eventState == 2) {
            PauseController.Instance.SetBlackCurtain(1f, false);
            PauseController.Instance.pauseEnabled = true;
            Event_BigDog eventBigDog = Instantiate(eventController).GetComponent<Event_BigDog>();
            eventBigDog.SetBigDogSave(GetMaxHP() - nowHP,  sandstarRawEnabled);
        }
    }

    void ChangeToWeakStatus() {
        knockEnduranceLight = 4000;
        sandstarRawKnockEnduranceLight = 5000;
        defensePower = 0;
        weakChangedFlag = true;
    }

    public void SetForNoEvent() {
        ChangeToWeakStatus();
        eventState = 3;
        damageBias = true;
    }

    public void BattleRestart(Event_BigDog eventController) {
        int totalDamageSave = 0;
        bool sandstarRawSave = false;
        eventController.BigDogRestart(ref totalDamageSave, ref sandstarRawSave);
        eventState = 3;
        ChangeToWeakStatus();
        if (sandstarRawSave) {
            StageManager.Instance.ActivateSandstarRaw();
        }
        if (eventController.allFriendsActivateFlag && !sandstarRawEnabled) {
            notAffectedByRisky = true;
        }
        allFriendsInformationReserved = eventController.allFriendsActivateFlag;
        nowHP -= totalDamageSave;
        restartEventTimeRemain = 3f;
        mutekiTimeRemain = 3.5f;
        disableControlTimeRemain = 3.5f;
        attackedTimeRemain = 5f;
        if (blackMinmiIDRank > 0) {
            attackedTimeRemain += blackMinmiIDRank * 0.4f;
        }
        attackSave = 3;
        EmitEffect(effEscapeReady);
        CharacterManager.Instance.pCon.Event_EscapeFromBigDog_01(3f, playerRestartPivot[0].position, playerRestartPivot[0].rotation);
        CharacterManager.Instance.WarpToCirclePosAll(trans.position, 10f);
        CameraManager.Instance.SetControlStyle(0);
        CameraManager.Instance.SetEventCameraFollowTarget(eventCameraPivot[1], 4f, 0f, 6f);
        CharacterManager.Instance.BossTimeStop();
        collapseReserved = eventController.collapsedFlag;
    }

    protected override void Start_Process_Dead() {
        EmitEffect(effDeadSave);
        base.Start_Process_Dead();
    }

    protected override void Update_Process_Dead() {
        base.Update_Process_Dead();
        if (!destroyOnDead && stateTime >= 7f) {
            Destroy(gameObject);
        }
    }

    protected override void DeadProcess() {
        if (isReaper) {
            if (!finalFormFlag) {
                ChangeMatForFinalForm();
            }
            GameManager.Instance.save.minmi |= (1 << rewardMinmiShiftNum);
            EmitEffect(effReaperDefeat);
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_DefeatBigDogEX, true);
        } else {
            if (isLastOne) {
                GameManager.Instance.save.SetClearStage(StageManager.Instance.stageNumber);
                if (defeatLightingNumber >= 0) {
                    LightingDatabase.Instance.SetLighting(defeatLightingNumber);
                    CharacterManager.Instance.SetPlayerLightActive();
                }
                if (defeatAmbientNumber >= 0) {
                    StageManager.Instance.dungeonController.ambientNumber = defeatAmbientNumber;
                }
                GameObject[] enemyObjs = GameObject.FindGameObjectsWithTag("Enemy");
                if (enemyObjs.Length > 1) {
                    for (int i = 0; i < enemyObjs.Length; i++) {
                        EnemyBase eBaseTemp = enemyObjs[i].GetComponent<EnemyBase>();
                        if (eBaseTemp && !eBaseTemp.isBoss && eBaseTemp != this) {
                            eBaseTemp.ForceDeath();
                        }
                    }
                }
            }
        }
        base.DeadProcess();
    }

    public override float GetMaxSpeed(bool isWalk = false, bool ignoreSuperman = false, bool ignoreSick = false, bool ignoreConfig = false) {
        if (GameManager.Instance.minmiPurple) {
            return base.GetMaxSpeed(isWalk, ignoreSuperman, ignoreSick, ignoreConfig);
        } else {
            return (isWalk ? walkSpeed : maxSpeed) * (ignoreSick ? 1f : GetSickSpeedRate());
        }
    }
    public override float GetAcceleration() {
        if (GameManager.Instance.minmiPurple) {
            return base.GetAcceleration();
        } else {
            return acceleration * (GetSick(SickType.Ice) ? 0.25f : 1f) * (GetSick(SickType.Stop) ? 5f : 1f);
        }
    }
    public override float GetAngularSpeed() {
        return angularSpeed;
    }
    public override float GetLockonRotSpeedRate() {
        return (GetSick(SickType.Mud) ? 0.2f : 1f) * (GetSick(SickType.Slow) ? 0.5f : 1f);
    }    

    private void LateUpdate() {
        if (animFinalizedFlag && animOverrideEnabled && (state != State.Dead || stateTime < 0.5f)) {
            for (int i = 0; i < animOverrides.Length; i++) {
                if (animOverrides[i].bone) {
                    if (animOverrides[i].posEnable) {
                        animOverrides[i].bone.localPosition = animOverrides[i].localPos;
                    }
                    if (animOverrides[i].rotEnable) {
                        animOverrides[i].bone.localEulerAngles = animOverrides[i].localRot;
                    }
                }
            }
        }
    }

}
