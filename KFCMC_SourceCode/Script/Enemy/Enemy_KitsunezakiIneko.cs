using UnityEngine;
using UnityStandardAssets.Cameras;

public class Enemy_KitsunezakiIneko : EnemyBaseBoss
{

    public float startMotionStiffTime;
    public Transform rootTransform;
    public Vector3 growScale;
    public GameObject[] objsOnSpawn;
    public GameObject[] objsOnGrow;
    public GameObject[] objsOnBattle;
    public Transform[] quakePivot;
    public Transform[] movePivot;
    public GameObject[] shieldObjs;
    public GameObject[] criticalObjs;
    public GameObject prefabOnDestroy;

    // Throw Swords
    public GameObject[] swordPrefabs;
    public Transform swordPivotX;
    public Transform swordFrom;
    public LookatTarget swordLookatTarget;
    public Transform swordNullTarget;
    public Transform swordLookTargetTrans;
    public float throwSwordSpeedMin;
    public float throwSwordSpeedMax;
    public float throwSwordSpeedUpDistance;
    public float throwSwordSpeedUpRate;

    // Throw Wave
    public float throwWaveSpeed;

    // Throw Fire
    public Transform throwFirePivotParent;
    public Transform throwFirePivotDefaultPosition;

    // Small Laser
    public GameObject smallLasersParent;
    private LaserOption[] smallLaserOptions;
    private RaycastToAdjustCapsuleCollider[] smallLaserRaycasters;
    private bool smallLaserEnabled;
    private FixEffectRotation smallLaserFixRotation;

    // BigLaser
    public LaserOption[] bigLaserOptions;
    public RaycastToAdjustCapsuleCollider[] bigLaserRaycasters;
    public LookatTarget bigLaserLookatTarget;
    public Transform bigLaserNullTarget;
    public float[] bigLaserLockonSpeed;
    private bool bigLaserEnabled;

    // Meteor
    public Transform meteorParent;

    // Danmaku
    public Transform[] danmakuPositionPivots;
    public Transform[] danmakuRotationPivots;
    public Transform[] danmakuChildRotationPivots;
    public GameObject[] danmakuFirstWeaknessAdditionalObjs;
    public GameObject[] danmakuSecondWeaknessAdditionalObjs;
    public float danmakuRotationSpeed;
    public float danmakuSinWaveSpeed;
    public ParticleSystem danmakuParticleSystem;
    public AudioSource danmakuAudio;
    public float[] danmakuEmitTimeAdd;
    private bool danmakuPositionEnabled;
    private bool danmakuRotationEnabled;
    private bool danmakuParticleEnabled;
    private Vector3 danmakuDefaultLocalPosition = new Vector3(0f, 1f, 0f);
    private Vector3 danmakuPositionVelocity;
    private float danmakuElapsedTime;
    private int danmakuParticleCount;
    private int danmakuAttackDetectionCountNow;
    private const float danmakuTeleportDistance = 1f;

    // Debug
    public int weakProgressOverride = -1;

    bool targetFound;
    int movingIndex = -1;
    float movingSpeedMul;
    int attackSave = -1;
    float attackSpeed = 1;
    bool attracted;
    int specialMoveCount;
    float specialMoveIntervalRemain;
    bool isGrown;
    int attackCount;
    DynamicBone[] dynamicBones;
    bool dynamicBoneEnabled = true;
    bool teleportToCenterReserved;
    bool teleportToEscapeReserved;
    bool healEffectEmitted = false;
    bool heavyKnocked = false;
    float teleportIntervalTimeRemain;
    const float teleportIntervalTimeMax = 16f;

    // Face
    protected FaceName fixFaceName;
    protected float fixFaceTimeRemain;
    protected int currentFaceIndex = -1;
    protected int[] faceIndex = new int[0];
    protected bool faceIndexInitialized;
    const string specialAttackFaceName = "Attack2";

    const float specialMoveIntervalMax = 20f;

    const int attackTypeSword = 0;
    const int attackTypeThunder = 1;
    const int attackTypeWave = 2;
    const int attackTypeFire = 3;
    const int attackTypeIce = 4;
    const int attackTypeSmallLaser = 5;
    const int attackTypeRing = 6;
    const int attackTypeBigLaser = 7;
    const int attackTypeMeteor = 8;
    const int attackTypeDanmaku = 9;

    const int attackDetectionIndexIce = 0;
    const int attackDetectionIndexSmallLaserReady = 3;
    const int attackDetectionIndexSmallLaser = 4;
    const int attackDetectionIndexBigLaserReady = 17;
    const int attackDetectionIndexBigLaser = 18;
    const int attackDetectionIndexDanmaku = 19;

    const int attackDetectionCountIce = 3;
    const int attackDetectionCountSmallLaser = 13;
    readonly int[] attackDetectionCountsDanmaku = { 6, 12, 18 };
    const int attackDetectionCountDanmakuAll = 18;

    const int effectIndexGrowReady = 0;
    const int effectIndexGrowStart = 1;
    const int effectIndexDeadSave = 2;
    const int effectIndexPrepareSword = 3;
    const int effectIndexPrepareThunder = 4;
    const int effectIndexReadyThunder = 5;
    const int effectIndexPrepareWave = 6;
    const int effectIndexReadyWave = 7;
    const int effectIndexPrepareFire = 8;
    const int effectIndexPrepareIce = 9;
    const int effectIndexIceReady = 10;
    const int effectIndexIceStart = 11;
    const int effectIndexIceEnd = 12;
    const int effectIndexIceBreak = 13;
    const int effectIndexPrepareSmallLaser = 14;
    const int effectIndexPrepareRing = 15;
    const int effectIndexBigLaserCountDown = 16;
    const int effectIndexPrepareMeteor = 17;
    const int effectIndexPrepareDanmaku = 18;
    const int effectIndexTeleport = 19;
    const int effectIndexBreakShiled = 20;
    const int effectIndexHealShield = 21;

    const int throwIndexSword = 0;
    const int throwIndexThunder = 19;
    const int throwIndexWave = 20;
    const int throwIndexFire = 29;
    const int throwIndexRing = 38;
    const int throwIndexMeteor = 40;

    const int throwMaxSword = 19;
    const int throwMaxWave = 9;
    const int throwMaxFire = 9;
    const int throwMaxRing = 2;
    const int throwMaxMeteor = 5;

    protected override void Awake()
    {
        base.Awake();
        actDistNum = 0;
        attackedTimeRemainOnDamage = 0.1f;
        //killByCriticalFailedKnockAmount = 4000f;
        //killByCriticalOnly = true;
        //checkGroundPivotHeight = 1f;
        //checkGroundTolerance = 2f;
        //checkGroundTolerance_Jumping = 2f;
        sandstarRawKnockEndurance = 1000000;
        sandstarRawKnockEnduranceLight = 5000;

        deadTimer = 105f / 30f;
        destroyOnDead = true;
        isAnimParamDetail = true;
        retargetingConditionTime = 4f;
        retargetingDecayMultiplier = 1f;

        spawnStiffTime = 0.05f;
        angryFixTime = 2f;
        attractionTime = 1.5f;
        confuseTime = 1.5f;
        attackWaitingLockonRotSpeed = 8;
        attackLockonDefaultSpeed = 20;

        stealedMax = 4;
        supermanEffectNumber = (int)EffectDatabase.id.enemyYK_Man;
        supermanAuraNumber = (int)EffectDatabase.id.enemyYK_Aura;
        dynamicBones = GetComponents<DynamicBone>();

        coreTimeMax = 18f;
        coreHideDenomi = 6f;

        smallLaserOptions = smallLasersParent.GetComponentsInChildren<LaserOption>();
        smallLaserRaycasters = smallLasersParent.GetComponentsInChildren<RaycastToAdjustCapsuleCollider>();
        smallLaserFixRotation = smallLasersParent.GetComponent<FixEffectRotation>();
        smallLaserFixRotation.enabled = false;
        SmallLaserCancel();

        for (int i = 0; i < attackDetectionCountDanmakuAll; i++)
        {
            attackDetection[attackDetectionIndexDanmaku + i].unleashed = true;
        }

        teleportIntervalTimeRemain = teleportIntervalTimeMax;

        SetCharacterOnSpawn();
    }

    protected override void Start()
    {
        base.Start();
        if (fCon)
        {
            SetFaceIndex();
        }
    }

    void CheckDynamicBone()
    {
        if (dynamicBones.Length > 0)
        {
            bool flag = (GameManager.Instance.save.config[GameManager.Save.configID_DynamicBone] >= 2);
            if (dynamicBoneEnabled != flag)
            {
                dynamicBoneEnabled = flag;
                for (int i = 0; i < dynamicBones.Length; i++)
                {
                    dynamicBones[i].enabled = dynamicBoneEnabled;
                }
            }
        }
    }

    private void CoreHide()
    {
        for (int i = 0; i < shieldObjs.Length; i++)
        {
            shieldObjs[i].SetActive(true);
        }
        for (int i = 0; i < criticalObjs.Length; i++)
        {
            criticalObjs[i].SetActive(false);
        }
        isCoreShowed = false;
        ResetKnockRemain();
    }

    private void CoreShow()
    {
        for (int i = 0; i < shieldObjs.Length; i++)
        {
            shieldObjs[i].SetActive(false);
        }
        for (int i = 0; i < criticalObjs.Length; i++)
        {
            criticalObjs[i].SetActive(true);
        }
        isCoreShowed = true;
        coreTimeRemain = coreTimeMax;
        coreShowHP = nowHP;
        coreHideConditionDamage = GetCoreHideConditionDamage();
        coreHideBorderStartHP = GetCoreHideBorder();
        healEffectEmitted = false;
        heavyKnocked = false;
        EmitEffect(effectIndexBreakShiled);
    }


    public override void SetSandstarRaw()
    {
        base.SetSandstarRaw();
        if (state != State.Dead)
        {
            coreHideDenomi = 12f;
            if (isCoreShowed)
            {
                CoreHide();
            }
        }
        quickAttackRadius = 1f;
        quickAttackReduceRate = 1f;
    }

    protected override void Update_StatusJudge()
    {
        base.Update_StatusJudge();
        if (GameManager.Instance.save.difficulty >= GameManager.difficultyVU || sandstarRawEnabled)
        {
            weakProgress = (nowHP <= GetMaxHP() / 3 ? 2 : nowHP <= GetMaxHP() * 2 / 3 ? 1 : 0);
        }
        else
        {
            weakProgress = (nowHP <= GetMaxHP() / 2 ? 1 : 0);
        }
        if (weakProgressOverride >= 0)
        {
            weakProgress = weakProgressOverride;
        }
        attractionTime = 4f - weakProgress;
        attracted = (decoySave == target);
        if (!sandstarRawEnabled)
        {
            maxSpeed = (weakProgress == 2 ? 18f : 12f);
        }
        else
        {
            maxSpeed = (weakProgress == 2 ? 36f : 24f);
        }
        acceleration = maxSpeed * 3;
        if (!targetFound)
        {
            mutekiTimeRemain = startMotionStiffTime;
        }
        if (state != State.Attack ||
            (attackType != attackTypeBigLaser && attackType != attackTypeMeteor && attackType != attackTypeDanmaku))
        {
            specialMoveIntervalRemain -= deltaTimeMove;
        }
        CheckDynamicBone();

        // Shield
        if (isCoreShowed && state != State.Dead)
        {
            coreTimeRemain -= deltaTimeCache * (coreTimeRemain >= 1f ? CharacterManager.Instance.riskyIncSqrt : 1f);
            if (!healEffectEmitted && coreTimeRemain < 1)
            {
                EmitEffect(effectIndexHealShield);
                healEffectEmitted = true;
            }
            if (coreTimeRemain < 0)
            {
                CoreHide();
            }
        }

        // Throw Swords
        if (targetTrans)
        {
            swordLookTargetTrans.position = MyMath.GetConsiderFallTargetPosition(targetTrans.position, swordPivotX.position, GetThrowSwordSpeed());
            swordLookatTarget.SetTarget(swordLookTargetTrans);
        }
        else
        {
            swordLookatTarget.SetTarget(swordNullTarget);
        }

        // Small Laser
        if (state != State.Attack && smallLaserEnabled)
        {
            SmallLaserCancel();
        }

        // Big Laser
        if (targetTrans)
        {
            bigLaserLookatTarget.SetTarget(targetTrans);
        }
        else
        {
            bigLaserLookatTarget.SetTarget(bigLaserNullTarget);
        }
        if (state != State.Attack && bigLaserEnabled)
        {
            BigLaserCancel();
        }

        // Danmaku
        if (state == State.Attack && attackType == attackTypeDanmaku)
        {
            Vector3 targetPosition = transform.position + danmakuDefaultLocalPosition;
            if (danmakuPositionEnabled && CharacterManager.Instance.playerSearchTarget && CharacterManager.Instance.playerSearchTarget.position.y > targetPosition.y)
            {
                targetPosition.y = CharacterManager.Instance.playerSearchTarget.position.y;
            }
            for (int i = 0; i < danmakuPositionPivots.Length; i++)
            {
                danmakuPositionPivots[i].position = Vector3.SmoothDamp(danmakuPositionPivots[i].position, targetPosition, ref danmakuPositionVelocity, 0.1f);
            }
            if (danmakuRotationEnabled)
            {
                for (int i = 0; i < danmakuRotationPivots.Length; i++)
                {
                    danmakuRotationPivots[i].localEulerAngles += Vector3.up * danmakuRotationSpeed * deltaTimeMove;
                }
                for (int i = 0; i < danmakuChildRotationPivots.Length; i++)
                {
                    danmakuChildRotationPivots[i].localEulerAngles = Vector3.up * 15f * Mathf.Sin(danmakuElapsedTime * danmakuSinWaveSpeed);
                }
                danmakuElapsedTime += deltaTimeMove;
            }
            if (danmakuParticleEnabled)
            {
                for (int i = 0; i < danmakuAttackDetectionCountNow; i++)
                {
                    //float danmakuTempTime = danmakuElapsedTime + danmakuEmitTimeAdd[i];
                    //bool isEmitting = danmakuTempTime >= 0 && (danmakuTempTime - Mathf.Floor(danmakuTempTime)) < 0.8f;
                    //if (sandstarRawEnabled)
                    //{
                    //    isEmitting = true;
                    //}
                    bool isEmitting = true;
                    if (attackDetection[attackDetectionIndexDanmaku + i].IsParticlesEmitting() != isEmitting)
                    {
                        if (isEmitting)
                        {
                            attackDetection[attackDetectionIndexDanmaku + i].ParticlesPlay();
                        }
                        else
                        {
                            attackDetection[attackDetectionIndexDanmaku + i].ParticlesStop();
                        }
                    }
                }
            }
            int currentParticleCount = danmakuParticleSystem.particleCount;
            if (currentParticleCount > danmakuParticleCount)
            {
                danmakuAudio.PlayOneShot(danmakuAudio.clip);
            }
            danmakuParticleCount = currentParticleCount;
        }
        if (danmakuParticleEnabled && state != State.Attack)
        {
            AttackEndDanmaku();
        }

        // Teleport
        if (teleportToCenterReserved)
        {
            TeleportToCenter();
            teleportToCenterReserved = false;
            teleportToEscapeReserved = false;
        }
        if (teleportToEscapeReserved)
        {
            TeleportToEscape();
            teleportToEscapeReserved = false;
        }
        if (state != State.Dead && battleStarted && CharacterManager.Instance.friendsCountSave > 0)
        {
            teleportIntervalTimeRemain -= deltaTimeCache * (12f + CharacterManager.Instance.friendsCountSave) / 12f * (weakProgress >= 2 ? 2f : weakProgress == 1 ? 1.5f : 1f);
            if (state == State.Damage && stateTime >= lightStiffTime * 0.5f && teleportIntervalTimeRemain <= 0)
            {
                teleportIntervalTimeRemain = teleportIntervalTimeMax;
                EmitEffect(effectIndexTeleport);
                teleportToEscapeReserved = true;
            }
        }
    }

    protected override void Update_Targeting()
    {
        base.Update_Targeting();
        // 戦闘形態への変身モーションを開始
        if (!targetFound && target)
        {
            disableControlTimeRemain = startMotionStiffTime;
            attackedTimeRemain = startMotionStiffTime;
            if (GetTargetDistance(true, true) < 9f * 9f || nowHP < GetMaxHP())
            {
                targetFound = true;
                anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.IdleType], 0);
                anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.IdleMotion]);
                actDistNum = 1;
                EmitEffect(effectIndexGrowReady);
            }
        }
    }

    // 通常形態へ変身
    void SetCharacterOnSpawn()
    {
        rootTransform.localScale = Vector3.one;
        for (int i = 0; i < objsOnGrow.Length; i++)
        {
            objsOnGrow[i].SetActive(false);
        }
        for (int i = 0; i < objsOnBattle.Length; i++)
        {
            objsOnBattle[i].SetActive(false);
        }
        for (int i = 0; i < objsOnSpawn.Length; i++)
        {
            objsOnSpawn[i].SetActive(true);
        }
        isGrown = false;
    }

    // 戦闘形態へ変身
    void SetCharacterOnBattle()
    {
        rootTransform.localScale = growScale;
        for (int i = 0; i < objsOnBattle.Length; i++)
        {
            objsOnBattle[i].SetActive(false);
        }
        for (int i = 0; i < objsOnSpawn.Length; i++)
        {
            objsOnSpawn[i].SetActive(false);
        }
        for (int i = 0; i < objsOnGrow.Length; i++)
        {
            objsOnGrow[i].SetActive(true);
        }
        isGrown = true;
    }

    public override void EmitEffectString(string type)
    {
        switch (type)
        {
            case "Spawn":
                if (!isGrown)
                {
                    SetCharacterOnBattle();
                }
                EmitEffect(effectIndexGrowStart);
                break;
            case "DeadSave":
                EmitEffect(effectIndexDeadSave);
                break;
            case "IceReady":
                EmitEffect(effectIndexIceReady);
                break;
            case "IceStart":
                EmitEffect(effectIndexIceStart);
                break;
            case "IceEnd":
                EmitEffect(effectIndexIceEnd);
                if (state == State.Attack)
                {
                    CameraManager.Instance.SetQuake(effect[effectIndexIceEnd].pivot.position, 12, 4, 0, 0, 1.5f, 12f, dissipationDistance_Boss);
                }
                break;
            case "IceBreak":
                EmitEffect(effectIndexIceBreak);
                break;
            case "BigLaserCountDown":
                EmitEffect(effectIndexBigLaserCountDown);
                break;
            case "PrepareMeteor":
                EmitEffect(effectIndexPrepareMeteor);
                break;
            case "PrepareDanmaku":
                EmitEffect(effectIndexPrepareDanmaku);
                break;
        }
    }

    void RestoreKnockDown()
    {
        //if (isRaw)
        //{
        //    fbStepMaxDist = 10f;
        //    fbStepTime = 1f;
        //    fbStepIgnoreY = true;
        //    SeparateFromTarget(10f);
        //}
    }

    void ActivateAgent()
    {
        agent.enabled = true;
    }

    void MovingEnd()
    {
        movingIndex = -1;
    }

    protected override void BattleStart()
    {
        base.BattleStart();
        actDistNum = 1;
        specialMoveIntervalRemain = specialMoveIntervalMax;
        specialMoveCount = 0;
        CoreHide();
        for (int i = 0; i < objsOnBattle.Length; i++)
        {
            objsOnBattle[i].SetActive(true);
        }
    }

    protected override void DamageCommonProcess()
    {
        base.DamageCommonProcess();
        if (!battleStarted)
        {
            BattleStart();
        }
    }

    protected override void KnockLightProcess()
    {
        base.KnockLightProcess();
        if (!isCoreShowed && state != State.Dead)
        {
            CoreShow();
        }
    }

    protected override void KnockHeavyProcess()
    {
        base.KnockHeavyProcess();
        heavyKnocked = true;
        if (isCoreShowed && coreTimeRemain > 1.25f)
        {
            coreTimeRemain = 1.25f;
        }
    }

    public override float GetKnocked()
    {
        return base.GetKnocked() * (!heavyKnocked && isCoreShowed && nowHP <= GetCoreHideBorder() ? 0 : 1);
    }

    protected override void Attack()
    {
        float spRate = (weakProgress >= 2 ? 1.2f : weakProgress == 1 ? 1.1f : 1f);
        attackLockonDefaultSpeed = 20 * spRate;
        movingIndex = -1;
        if (targetTrans)
        {
            int attackTemp;
            float sqrDistance = GetTargetDistance(true, true, false);
            if (!isGrown)
            {
                SetCharacterOnBattle();
            }
            if (!battleStarted)
            {
                BattleStart();
                attackSave = attackTemp = 0;
            }
            else if (specialMoveIntervalRemain <= 0f)
            {
                attackTemp = 7 + specialMoveCount % 3;
                specialMoveCount++;
                specialMoveIntervalRemain = specialMoveIntervalMax;
            }
            else
            {
                attackSave = attackTemp = MyMath.RandomRangeExclude(0, 7, attackSave);
            }
            // 攻撃回数が3の倍数のとき、逃走モードにする
            attackCount++;
            actDistNum = (attackCount % 3) == 0 ? 2 : 1;

            float intervalPlus = 0f;
            float runTimePlus = (actDistNum == 2 ? 1f : 0f) / (weakProgress >= 2 ? 1.5f : 1f);
            intervalPlus = runTimePlus + (weakProgress >= 2 ? Random.Range(0.1f, 0.15f) : weakProgress == 1 ? Random.Range(0.2f, 0.25f) : Random.Range(0.3f, 0.35f)) + (attracted ? 0.2f : 0);
            if (attackTemp == attackTypeMeteor)
            {
                if (weakProgress == 0)
                {
                    intervalPlus += (actDistNum == 2 ? 1f : 2f);
                }
                else if (weakProgress == 1)
                {
                    intervalPlus += (actDistNum == 2 ? 0f : 1f);
                }
            }
            if (attackTemp != attackTypeDanmaku)
            {
                if (CharacterManager.Instance.friendsCountSave > 0 && teleportIntervalTimeRemain <= 0)
                {
                    teleportIntervalTimeRemain = teleportIntervalTimeMax;
                    EmitEffect(effectIndexTeleport);
                    teleportToEscapeReserved = true;
                }
            }

            switch (attackTemp)
            {
                case attackTypeSword:
                    AttackBase(attackTypeSword, 1.1f, 1.7f, 0, 120f / 60f / spRate, 120f / 60f / spRate + intervalPlus, 0, spRate);
                    PrepareSword();
                    break;
                case attackTypeThunder:
                    AttackBase(attackTypeThunder, 1f, 1.1f, 0, 120f / 60f / spRate, 120f / 60f / spRate + intervalPlus, 0, spRate);
                    PrepareThunder();
                    break;
                case attackTypeWave:
                    AttackBase(attackTypeWave, 1.05f, 1.4f, 0, 120f / 60f / spRate, 120f / 60f / spRate + intervalPlus, 0, spRate);
                    PrepareWave();
                    break;
                case attackTypeFire:
                    AttackBase(attackTypeFire, 0.95f, 0.8f, 0, 120f / 60f / spRate, 120f / 60f / spRate + intervalPlus, 0, spRate);
                    PrepareFire();
                    break;
                case attackTypeIce:
                    AttackBase(attackTypeIce, 1.15f, 3.4f, 0, 120f / 60f / spRate, 120f / 60f / spRate + intervalPlus, 0, spRate);
                    PrepareIce();
                    break;
                case attackTypeSmallLaser:
                    AttackBase(attackTypeSmallLaser, 1f, 0.8f, 0, 150f / 60f / spRate, 150f / 60f / spRate + intervalPlus, 0, spRate);
                    PrepareSmallLaser();
                    break;
                case attackTypeRing:
                    AttackBase(attackTypeRing, 0.9f, 0.6f, 0, 115f / 60f / spRate, 115f / 60f / spRate + intervalPlus, 0, spRate);
                    PrepareRing();
                    break;
                case attackTypeBigLaser:
                    AttackBase(attackTypeBigLaser, 1.4f, 50f, 0, 285f / 60f, 285f / 60f + intervalPlus, 0);
                    PrepareBigLaser();
                    break;
                case attackTypeMeteor:
                    AttackBase(attackTypeMeteor, 1.4f, 50f, 0, 130f / 60f, 130f / 60f + intervalPlus, 0);
                    PrepareMeteor();
                    break;
                case attackTypeDanmaku:
                    AttackBase(attackTypeDanmaku, 1f, 0.8f, 0, 95f / 10f, 95f / 10f + intervalPlus, 0, 1, false);
                    PrepareDanmaku();
                    break;
            }
        }
    }

    protected override void AttackContinuous()
    {
        base.AttackContinuous();
        if (movingIndex >= 0 && movingIndex < movePivot.Length && movePivot[movingIndex] != null)
        {
            ApproachTransformPivot(movePivot[movingIndex], GetMinmiSpeed() * movingSpeedMul, 0.25f);
        }
        //switch (attackType)
        //{
        //    case attackTypeSword:
        //        if (swordSmoothRotEnabled && targetTrans && swordPivotY)
        //        {
        //            SmoothRotation(swordPivotY, GetTargetVector(true, true, false), lockonRotSpeed * 3.6f * Mathf.Min(GetLockonRotSpeedRate(), 2f), 1f / 6f, true);
        //        }
        //        break;
        //}
    }

    void PrepareSword()
    {
        EmitEffect(effectIndexPrepareSword);
    }

    void ThrowSwordReady()
    {
        if (state == State.Attack)
        {
            for (int i = 0; i < throwMaxSword; i++)
            {
                throwing.throwSettings[throwIndexSword + i].prefab = swordPrefabs[Random.Range(0, swordPrefabs.Length)];
                throwing.ThrowReady(throwIndexSword + i);
            }
        }
    }

    float GetThrowSwordSpeed()
    {
        if (targetTrans)
        {
            float distance = Vector3.Distance(swordPivotX.position, targetTrans.position);
            return Mathf.Clamp(throwSwordSpeedMin + (distance - throwSwordSpeedUpDistance) * throwSwordSpeedUpRate, throwSwordSpeedMin, throwSwordSpeedMax);
        }
        else
        {
            return throwSwordSpeedMin;
        }
    }

    void ThrowSwordStart()
    {
        if (state == State.Attack)
        {
            float speed = GetThrowSwordSpeed();
            for (int i = 0; i < throwMaxSword; i++)
            {
                throwing.throwSettings[throwIndexSword + i].velocity = speed;
                throwing.ThrowStart(throwIndexSword + i);
            }
        }
        LockonEnd();
    }

    void PrepareThunder()
    {
        EmitEffect(effectIndexPrepareThunder);
    }

    void ThrowReadyThunder()
    {
        if (state == State.Attack)
        {
            EmitEffect(effectIndexReadyThunder);
        }
    }

    void ThrowStartThunder()
    {
        if (state == State.Attack)
        {
            throwing.ThrowStart(throwIndexThunder);
        }
        LockonEnd();
    }

    void PrepareWave()
    {
        EmitEffect(effectIndexPrepareWave);
        EmitEffect(effectIndexReadyWave);
    }

    void ThrowStartWave()
    {
        for (int i = 0; i < throwMaxWave; i++)
        {
            throwing.throwSettings[throwIndexWave + i].velocity = throwWaveSpeed;
            throwing.ThrowStart(throwIndexWave + i);
        }
        LockonEnd();
    }

    void PrepareFire()
    {
        EmitEffect(effectIndexPrepareFire);
    }

    void ThrowStartFire()
    {
        if (targetTrans)
        {
            Vector3 targetPos = targetTrans.position;
            targetPos.y = transform.position.y;
            throwFirePivotParent.position = targetPos;
        }
        else
        {
            throwFirePivotParent.position = throwFirePivotDefaultPosition.position;
        }
        for (int i = 0; i < throwMaxFire; i++)
        {
            throwing.ThrowStart(throwIndexFire + i);
        }
        LockonEnd();
    }

    void PrepareIce()
    {
        EmitEffect(effectIndexPrepareIce);
    }

    void AttackStartIce()
    {
        for (int i = 0; i < attackDetectionCountIce; i++)
        {
            AttackStart(attackDetectionIndexIce + i);
        }
        LockonEnd();
    }

    void AttackEndIce()
    {
        for (int i = 0; i < attackDetectionCountIce; i++)
        {
            AttackEnd(attackDetectionIndexIce + i);
        }
    }

    void PrepareSmallLaser()
    {
        EmitEffect(effectIndexPrepareSmallLaser);
    }

    void SmallLaserCancel()
    {
        smallLaserEnabled = false;
        for (int i = 0; i < smallLaserOptions.Length; i++)
        {
            if (smallLaserOptions[i])
            {
                smallLaserOptions[i].CancelLaser();
            }
        }
        for (int i = 0; i < smallLaserRaycasters.Length; i++)
        {
            if (smallLaserRaycasters[i])
            {
                smallLaserRaycasters[i].Deactivate();
            }
        }
        smallLaserFixRotation.enabled = false;
    }

    void SmallLaserReady()
    {
        if (state == State.Attack)
        {
            smallLaserEnabled = true;
            if (targetTrans)
            {
                Vector3 targetPos = targetTrans.position;
                targetPos.y = smallLasersParent.transform.position.y;
                smallLasersParent.transform.LookAt(targetPos);
            }
            else
            {
                smallLasersParent.transform.localEulerAngles = Vector3.zero;
            }

            smallLaserFixRotation.SetRotation(smallLasersParent.transform.eulerAngles);
            smallLaserFixRotation.enabled = true;

            AttackStart(attackDetectionIndexSmallLaserReady);
            for (int i = 0; i < smallLaserOptions.Length; i++)
            {
                if (smallLaserOptions[i])
                {
                    smallLaserOptions[i].LightFlickeringChargeStart();
                }
            }
            for (int i = 0; i < smallLaserRaycasters.Length; i++)
            {
                if (smallLaserRaycasters[i])
                {
                    smallLaserRaycasters[i].hitEffectEnabled = false;
                    smallLaserRaycasters[i].Activate();
                }
            }
        }
    }

    void SmallLaserStart()
    {
        if (state == State.Attack)
        {
            smallLaserEnabled = true;
            AttackEnd(attackDetectionIndexSmallLaserReady);
            for (int i = 0; i < smallLaserOptions.Length; i++)
            {
                if (smallLaserOptions[i])
                {
                    smallLaserOptions[i].LightFlickeringChargeEnd();
                }
            }
            for (int i = 0; i < smallLaserRaycasters.Length; i++)
            {
                if (smallLaserRaycasters[i])
                {
                    smallLaserRaycasters[i].hitEffectEnabled = true;
                    smallLaserRaycasters[i].Activate();
                }
            }
            for (int i = 0; i < attackDetectionCountSmallLaser; i++)
            {
                AttackStart(attackDetectionIndexSmallLaser + i);
            }
        }
    }

    void SmallLaserEnd()
    {
        if (state == State.Attack)
        {
            smallLaserEnabled = false;
            for (int i = 0; i < smallLaserOptions.Length; i++)
            {
                if (smallLaserOptions[i])
                {
                    smallLaserOptions[i].LightFlickeringBlastEnd();
                }
            }
            for (int i = 0; i < smallLaserRaycasters.Length; i++)
            {
                if (smallLaserRaycasters[i])
                {
                    smallLaserRaycasters[i].Deactivate();
                }
            }
            for (int i = 0; i < attackDetectionCountSmallLaser; i++)
            {
                AttackEnd(attackDetectionIndexSmallLaser + i);
            }
            smallLaserFixRotation.enabled = false;
        }
    }

    void PrepareRing()
    {
        EmitEffect(effectIndexPrepareRing);
    }

    void ThrowStartRing()
    {
        for (int i = 0; i < throwMaxRing; i++)
        {
            throwing.ThrowStart(throwIndexRing + i);
        }
    }

    void PrepareBigLaser()
    {
        attackDetection[attackDetectionIndexBigLaser].multiHitInterval = 0.1f;
        if (isCoreShowed)
        {
            SuperarmorStart();
        }
    }

    void BigLaserCancel()
    {
        bigLaserEnabled = false;
        for (int i = 0; i < bigLaserOptions.Length; i++)
        {
            if (bigLaserOptions[i])
            {
                bigLaserOptions[i].CancelLaser();
            }
        }
        for (int i = 0; i < bigLaserRaycasters.Length; i++)
        {
            if (bigLaserRaycasters[i])
            {
                bigLaserRaycasters[i].Deactivate();
            }
        }
    }

    void BigLaserReady()
    {
        if (state == State.Attack)
        {
            bigLaserEnabled = true;
            AttackStart(attackDetectionIndexBigLaserReady);
            for (int i = 0; i < bigLaserOptions.Length; i++)
            {
                if (bigLaserOptions[i])
                {
                    bigLaserOptions[i].LightFlickeringChargeStart();
                }
            }
            for (int i = 0; i < bigLaserRaycasters.Length; i++)
            {
                if (bigLaserRaycasters[i])
                {
                    bigLaserRaycasters[i].hitEffectEnabled = false;
                    bigLaserRaycasters[i].Activate();
                }
            }
        }
    }

    void BigLaserStart()
    {
        if (state == State.Attack)
        {
            bigLaserEnabled = true;
            AttackEnd(attackDetectionIndexBigLaserReady);
            for (int i = 0; i < bigLaserOptions.Length; i++)
            {
                if (bigLaserOptions[i])
                {
                    bigLaserOptions[i].LightFlickeringChargeEnd();
                }
            }
            for (int i = 0; i < bigLaserRaycasters.Length; i++)
            {
                if (bigLaserRaycasters[i])
                {
                    bigLaserRaycasters[i].hitEffectEnabled = true;
                    bigLaserRaycasters[i].Activate();
                }
            }
            AttackStart(attackDetectionIndexBigLaser);
        }
    }

    void BigLaserEnd()
    {
        if (state == State.Attack)
        {
            bigLaserEnabled = false;
            for (int i = 0; i < bigLaserOptions.Length; i++)
            {
                if (bigLaserOptions[i])
                {
                    bigLaserOptions[i].LightFlickeringBlastEnd();
                }
            }
            for (int i = 0; i < bigLaserRaycasters.Length; i++)
            {
                if (bigLaserRaycasters[i])
                {
                    bigLaserRaycasters[i].Deactivate();
                }
            }
            AttackEnd(attackDetectionIndexBigLaser);
            LockonEnd();
            SuperarmorEnd();
        }
    }

    void BigLaserMultiHitEnd()
    {
        if (state == State.Attack)
        {
            attackDetection[attackDetectionIndexBigLaser].multiHitInterval = 0f;
        }
    }

    void BigLaserLockonSlowDown()
    {
        lockonRotSpeed = bigLaserLockonSpeed[weakProgress];
    }

    void PrepareMeteor()
    {
        if (isCoreShowed)
        {
            SuperarmorStart();
        }
    }

    void ThrowStartMeteor()
    {
        Vector3 targetPos;
        if (targetTrans)
        {
            targetPos = targetTrans.position;
            targetPos.y = transform.position.y;
        }
        else
        {
            targetPos = transform.position;
        }
        meteorParent.position = targetPos;
        for (int i = 0; i < throwMaxMeteor; i++)
        {
            throwing.ThrowStart(throwIndexMeteor + i);
        }
        LockonEnd();
        SuperarmorEnd();
    }

    void PrepareDanmaku()
    {
        danmakuPositionEnabled = true;
        danmakuRotationEnabled = false;
        danmakuParticleEnabled = false;
        for (int i = 0; i < danmakuPositionPivots.Length; i++)
        {
            danmakuPositionPivots[i].localPosition = danmakuDefaultLocalPosition;
        }
        for (int i = 0; i < danmakuRotationPivots.Length; i++)
        {
            danmakuRotationPivots[i].localEulerAngles = Vector3.zero;
        }
        danmakuPositionVelocity = Vector3.zero;
        danmakuElapsedTime = 0;
        for (int i = 0; i < danmakuChildRotationPivots.Length; i++)
        {
            danmakuChildRotationPivots[i].localEulerAngles = Vector3.zero;
        }
        for (int i = 0; i < danmakuFirstWeaknessAdditionalObjs.Length; i++)
        {
            danmakuFirstWeaknessAdditionalObjs[i].SetActive(weakProgress >= 1);
        }
        for (int i = 0; i < danmakuSecondWeaknessAdditionalObjs.Length; i++)
        {
            danmakuSecondWeaknessAdditionalObjs[i].SetActive(weakProgress >= 2);
        }
        danmakuAttackDetectionCountNow = attackDetectionCountsDanmaku[weakProgress];
        if ((GetCenterPointPosition() - transform.position).sqrMagnitude >= danmakuTeleportDistance * danmakuTeleportDistance)
        {
            EmitEffect(effectIndexTeleport);
            teleportToCenterReserved = true;
        }
        if (isCoreShowed)
        {
            SuperarmorStart();
        }
    }

    void AttackStartDanmaku()
    {
        for (int i = 0; i < danmakuAttackDetectionCountNow; i++)
        {
            attackDetection[attackDetectionIndexDanmaku + i].ParticlesClear();
            AttackStart(attackDetectionIndexDanmaku + i);
        }
        danmakuPositionEnabled = true;
        danmakuRotationEnabled = true;
        danmakuParticleEnabled = true;
        danmakuParticleCount = 0;
    }

    void AttackEndDanmaku()
    {
        for (int i = 0; i < danmakuAttackDetectionCountNow; i++)
        {
            attackDetection[attackDetectionIndexDanmaku + i].ParticlesStop();
        }
        danmakuPositionEnabled = false;
        danmakuRotationEnabled = false;
        danmakuParticleEnabled = false;
        SuperarmorEnd();
    }

    Vector3 GetCenterPointPosition()
    {
        GameObject[] centerPoints = GameObject.FindGameObjectsWithTag("CenterPoint");
        if (centerPoints.Length > 0)
        {
            return centerPoints[0].transform.position;
        }
        return transform.position;
    }

    void TeleportToCenter()
    {
        specialMoveDuration = 0f;
        destination = GetCenterPointPosition();
        agent.SetDestination(destination);
        Warp(destination, 0f, 0f);
    }

    void TeleportToEscape()
    {
        specialMoveDuration = 0f;
        destination = GetEscapeDestination(searchArea[0].GetTargetsAveragePosition(), evadeDistance);
        agent.SetDestination(destination);
        Warp(destination, 0f, 0f);
    }

    protected override void Update_Process_Dead()
    {
        base.Update_Process_Dead();
        if (stateTime > 4)
        {
            Destroy(gameObject);
        }
    }

    protected override void DeadProcess()
    {
        if (isLastOne)
        {
            if (prefabOnDestroy && StageManager.Instance.dungeonController)
            {
                Instantiate(prefabOnDestroy, trans.position, trans.rotation, StageManager.Instance.dungeonController.transform);
            }
        }
        base.DeadProcess();
    }

    protected virtual void SetFaceIndex()
    {
        if (fCon && !faceIndexInitialized)
        {
            faceIndex = new int[System.Enum.GetValues(typeof(FaceName)).Length];
            faceIndex[(int)FaceName.Idle] = fCon.GetFaceIndex(FaceName.Idle.ToString());
            faceIndex[(int)FaceName.Blink] = fCon.GetFaceIndex(FaceName.Blink.ToString());
            faceIndex[(int)FaceName.Idle1] = fCon.GetFaceIndex(FaceName.Idle1.ToString());
            faceIndex[(int)FaceName.Idle2] = fCon.GetFaceIndex(FaceName.Idle2.ToString());
            faceIndex[(int)FaceName.Attack] = fCon.GetFaceIndex(FaceName.Attack.ToString());
            faceIndex[(int)FaceName.Damage] = fCon.GetFaceIndex(FaceName.Damage.ToString());
            faceIndex[(int)FaceName.Dead] = fCon.GetFaceIndex(FaceName.Dead.ToString());
            faceIndex[(int)FaceName.Fear] = fCon.GetFaceIndex(FaceName.Fear.ToString());
            faceIndex[(int)FaceName.Jump] = fCon.GetFaceIndex(FaceName.Jump.ToString());
            faceIndex[(int)FaceName.Run] = fCon.GetFaceIndex(FaceName.Run.ToString());
            faceIndex[(int)FaceName.Refresh] = fCon.GetFaceIndex(FaceName.Refresh.ToString());
            faceIndex[(int)FaceName.Smile] = fCon.GetFaceIndex(FaceName.Smile.ToString());
            faceIndexInitialized = true;
        }
    }

    public void SetFixFace(FaceName newFace, float timer = 5f)
    {
        fixFaceName = newFace;
        fixFaceTimeRemain = timer;
    }

    protected virtual void SetFace(int faceIndex)
    {
        if (faceIndex == this.faceIndex[(int)FaceName.Attack] && attackType == attackTypeDanmaku)
        {
            fCon.SetFace(specialAttackFaceName);
        }
        else
        {
            fCon.SetFace(faceIndex, true);
        }
    }

    protected override void Update_FaceControl()
    {
        base.Update_FaceControl();
        AnimatorStateInfo animSI = anim.GetCurrentAnimatorStateInfo(0);
        if (fCon)
        {
            currentFaceIndex = fCon.CurrentFaceIndex;
            int hash = animSI.fullPathHash;
            if (hash == AnimHash.Instance.ID[(int)AnimHash.ParamName.StateFriendsIdle1])
            {
                float normalizedTime = animSI.normalizedTime;
                if (normalizedTime >= 0f && normalizedTime < 45f / 120f)
                {
                    if (currentFaceIndex != faceIndex[(int)FaceName.Blink])
                    {
                        SetFace(faceIndex[(int)FaceName.Blink]);
                    }
                }
                else if (normalizedTime < 95f / 120f)
                {
                    if (currentFaceIndex != faceIndex[(int)FaceName.Idle1])
                    {
                        SetFace(faceIndex[(int)FaceName.Idle1]);
                    }
                }
                else
                {
                    if (currentFaceIndex != faceIndex[(int)FaceName.Idle])
                    {
                        SetFace(faceIndex[(int)FaceName.Idle]);
                    }
                }
            }
            else if (fixFaceTimeRemain > 0f)
            {
                if (currentFaceIndex != faceIndex[(int)fixFaceName])
                {
                    SetFace(faceIndex[(int)fixFaceName]);
                }
            }
            else if (state == State.Damage)
            {
                if (currentFaceIndex != faceIndex[(int)FaceName.Damage])
                {
                    SetFace(faceIndex[(int)FaceName.Damage]);
                }
            }
            else if (state == State.Dead)
            {
                if (currentFaceIndex != faceIndex[(int)FaceName.Dead])
                {
                    SetFace(faceIndex[(int)FaceName.Dead]);
                }
            }
            else if (state == State.Attack)
            {
                if (currentFaceIndex != faceIndex[(int)FaceName.Attack])
                {
                    SetFace(faceIndex[(int)FaceName.Attack]);
                }
            }
            else if (nowSpeed > GetMaxSpeed(true))
            {
                if (currentFaceIndex != faceIndex[(int)FaceName.Run] && currentFaceIndex != faceIndex[(int)FaceName.Blink])
                {
                    SetFace(faceIndex[(int)FaceName.Run]);
                }
            }
            else if (GetAnySick())
            {
                if (currentFaceIndex != faceIndex[(int)FaceName.Refresh] && currentFaceIndex != faceIndex[(int)FaceName.Blink])
                {
                    SetFace(faceIndex[(int)FaceName.Refresh]);
                }
            }
            else
            {
                if (currentFaceIndex != faceIndex[(int)FaceName.Idle] && currentFaceIndex != faceIndex[(int)FaceName.Blink])
                {
                    SetFace(faceIndex[(int)FaceName.Idle]);
                }
            }
        }
        if (fixFaceTimeRemain > 0f)
        {
            fixFaceTimeRemain -= deltaTimeCache;
        }
    }

    void ChangeFaceCry()
    {

    }

    public override float GetOriginalSpeed()
    {
        return base.GetOriginalSpeed() * 0.75f;
    }

    public override void SetForDictionary(bool toSuperman, int layer, Transform dynamicBoneReferenceObject)
    {
        base.SetForDictionary(toSuperman, layer, dynamicBoneReferenceObject);
        SetCharacterOnBattle();
        CheckDynamicBone();
        for (int i = 0; i < dynamicBones.Length; i++)
        {
            dynamicBones[i].m_ReferenceObject = dynamicBoneReferenceObject;
        }
    }

    void ScrewStart()
    {
    }

    void ScrewStartSecond()
    {
    }

    void ScrewEnd()
    {
    }

}
