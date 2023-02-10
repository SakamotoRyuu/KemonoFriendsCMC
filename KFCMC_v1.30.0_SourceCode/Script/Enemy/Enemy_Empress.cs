using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Empress : EnemyBaseBoss
{

    [System.Serializable]
    public class NearestPointSettings {
        public Transform setTarget;
        public Transform pivotA;
        public Transform pivotB;
    }

    public GameObject[] startActivateObj;
    public GameObject[] startDeactivateObj;
    public Transform earthRotX;
    public Transform earthRotY;
    public Transform earthPosZ;
    public EnemyDeath earthBreak;
    public GameObject[] normalDD;
    public GameObject[] criticalDD;
    public ParticleSystem[] livingParticle;
    public Color normalParticleColor;
    public Color criticalParticleColor;
    public SearchTargetPriority bodyPriority;
    public Transform eyePivot;
    public Transform nullTarget;
    public Transform attackPivotFlexible;
    public Transform attackPivotHorizontal;
    public Transform attackPivotPosXZ;
    public NearestPointSettings[] rotateNearPointSettings;
    public Transform[] quakePivot;
    public LaserOption[] laserOption;
    public RaycastToAdjustCapsuleCollider[] raycaster;
    public int bgmNumberBattleAnother;
    public ChangeMatSet[] anotherMat;
    public GameObject cervalBallPrefab;
    public GameObject crackPivot;
    public DamageDetection[] rawKnockChangeDD;
    public DamageDetection[] rawKnockEffectiveDD;
    public DamageDetection damageDetectionEarth;
    public Transform searchReference;
    public Vector2 searchRefHeightRange;
    public GameObject[] lightObj;
    public GameObject eventBackImagePrefab;
    public Renderer bodyRenderer;
    public Material[] bodyMaterials;
    public GameObject[] firstContactWalls;

    int nearAttackSave;
    int[] farAttackSave = new int[3];
    bool nearFarSwitch;
    bool attracted;
    float battleReadyTimer;
    int eventProgress;
    bool healEffectEmitted;
    ParticleSystem.MinMaxGradient[] parColor = new ParticleSystem.MinMaxGradient[2];
    ParticleSystem.MainModule[] parMain = new ParticleSystem.MainModule[2];
    float earthRotSpeed;
    float earthRotTime;
    bool lookingFlag;
    bool lookingIsSlow;
    bool heavyKnocked;
    Vector3 earthPosSave;
    int throwReadyPointer;
    int throwStartPointer;
    bool throwRushEnabled;
    float throwRushInterval;
    bool laserEnabled = false;
    float laserAttackedTimeRemain = 0f;
    bool eyeFastEnabled;
    int eventState;
    int particleFlagSave = -1;
    GameObject cervalBallInstance;
    Vector3 smoothVelocity;
    GameObject eventBackImageInstance;
    bool bodyMaterialChanged;

    const int throwB2Start = 0;
    const int throwB2End = 12;
    const int throwRedGrowlStart = 12;
    const int throwRedGrowlEnd = 21;
    const int throwAkulaStart = 21;
    const int throwAkulaEnd = 33;
    const int throwCeckyBeastStart = 33;
    const int throwGlaucusStart = 35;
    const int throwGlaucusEnd = 37;
    const int throwBombStart = 37;
    const int throwBombEnd = 46;
    const int throwBombBig = 46;
    
    const int effBattleReady = 0;
    const int effCoreShow = 1;
    const int effCoreHide = 2;
    const int effKnockLight = 3;
    const int effKnockHeavy = 4;
    const int effDeadSave = 5;
    const int effCylinderReady = 6;
    const int effCylinderStart = 7;
    const int effCylinderEnd = 8;
    const int effSwingDownReady = 9;
    const int effSwingDownStart = 10;
    const int effSwingDownEnd = 11;
    const int effBowlingStart = 12;
    const int effBowlingEnd = 13;
    const int effStampReadyPar = 14;
    const int effStampReadySE = 15;
    const int effStampStart = 16;
    const int effStampImpact = 17;
    const int effStampEnd = 18;
    const int effCrossDownReady = 19;
    const int effCrossDownStart = 20;
    const int effCrossDownEnd = 21;
    const int effOpenDownReady = 22;
    const int effOpenDownStart = 23;
    const int effOpenDownEnd = 24;
    const int effRotateReady = 25;
    const int effRotateStart = 26;
    const int effRotateEnd = 27;
    const int effLineStart = 28;
    const int effLineEnd = 29;
    const int effComplexStart = 30;
    const int effComplexEnd1 = 31;
    const int effComplexEnd2 = 32;
    const int effClapReady = 33;
    const int effClapStart = 34;
    const int effClapEnd = 35;
    const int effBossReady = 36;
    const int effAlligatorClipsStart = 37;
    const int effAlligatorClipsEnd = 38;
    const int effProjectileReady = 39;
    const int effB2Start = 40;
    const int effRedGrowlReady = 41;
    const int effGlaucusReady = 42;
    const int effSnowTowerReady = 43;
    const int effSnowTowerStart = 44;
    const int effSnowTowerEnd = 45;
    const int effSnowTowerBreak = 46;
    const int effBigDogReadyPar = 47;
    const int effBigDogReadySE = 48;
    const int effBigDogStart = 49;
    const int effLaserCountDown = 50;
    const int effEvent1 = 51;
    const int effEvent2 = 52;

    const int quakeHorizontal = 0;
    const int quakeXZPos = 1;
    const int quakeCenter = 2;

    const int attackLaserDummy = 28;
    const int attackLaserBody = 29;

    const int parHQ = 0;
    const int parLQ = 1;
    
    const float criticalDDKnockRaw = 7.5f;
    const float criticalDDKnockUsual = 15f;
    const float earthDDKnockEasy = 7.5f;
    const float earthDDKnockNormal = 5f;

    static readonly Vector3 vecHalf = new Vector3(0.5f, 0.5f, 0.5f);
    static readonly Vector3 vecBombRand = new Vector3(1.5f, 1.5f, 1.5f);

    protected override void Awake() {
        base.Awake();
        for (int i = 0; i < parMain.Length; i++) {
            parMain[i] = livingParticle[i].main;
            parColor[i] = parMain[i].startColor;
        }
        spawnStiffTime = 100000f;
        deadTimer = 100f;
        destroyOnDead = false;
        actDistNum = 0;
        attackLockonDefaultSpeed = 0f;
        attackedTimeRemainOnDamage = 1.5f;
        attackedTimeRemainReduceOnAngry = 2f;
        attackWaitingLockonRotSpeed = 0f;
        checkGroundPivotHeight = 1f;
        checkGroundTolerance = 2f;
        checkGroundTolerance_Jumping = 2f;
        retargetingConditionTime = 0.001f;
        retargetingDecayMultiplier = 1f;
        sandstarRawLockonSpeed = 0f;
        sandstarRawKnockEndurance = 1000000;
        sandstarRawKnockEnduranceLight = 30000;
        sandstarRawMaxSpeed = 0f;
        sandstarRawAcceleration = 0f;
        LaserCancel();
        CoreHide();
        cannotDoubleKnockDown = true;
        catchupExpDisabled = true;
        isCoreShowed = false;
        healEffectEmitted = false;
        killByCriticalOnly = true;
        coreTimeRemain = 0f;
        coreTimeMax = 30f;
        coreHideDenomi = 10f;
        mapChipSize = 3;
        supermanEffectNumber = (int)EffectDatabase.id.enemyYK_LastBoss;
        supermanAuraNumber = (int)EffectDatabase.id.enemyYK_AuraBigSuper;
        for (int i = 0; i < startActivateObj.Length; i++) {
            if (startActivateObj[i]) {
                startActivateObj[i].SetActive(false);
            }
        }
        for (int i = 0; i < startDeactivateObj.Length; i++) {
            if (startDeactivateObj[i]) {
                startDeactivateObj[i].SetActive(true);
            }
        }
        if (GameManager.Instance.IsPlayerAnother && StageManager.Instance.dungeonController && StageManager.Instance.dungeonController.bossMusicNumber < 0) {
            bgmNumberBattle = bgmNumberBattleAnother;
        }
        SetCriticalDDKnockedRate();
    }

    protected override void Start() {
        base.Start();
        if (!isItem) {
            firstContactWalls[0].SetActive(!isForAmusement);
            firstContactWalls[1].SetActive(isForAmusement);
        }
        if (!isItem && LightingDatabase.Instance && LightingDatabase.Instance.IsNight == false) {
            ChangeBodyMaterial(2);
        }
    }

    void AnimStopSpawn() {
        if (eventProgress == 0) {
            anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.AnimSpeed], 0);
            eventProgress = 1;
        }
    }

    void SetParticleColor(bool isCritical) {
        for (int i = 0; i < parMain.Length; i++) {
            parColor[i].color = (isCritical ? criticalParticleColor : normalParticleColor);
            parMain[i].startColor = parColor[i];
        }
    }

    private void CoreHide() {
        for (int i = 0; i < normalDD.Length; i++) {
            if (normalDD[i]) {
                normalDD[i].SetActive(true);
            }
        }
        for (int i = 0; i < criticalDD.Length; i++) {
            if (criticalDD[i]) {
                criticalDD[i].SetActive(false);
            }
        }
        if (bodyPriority) {
            bodyPriority.priority = 0;
        }
        if (searchTarget[1]) {
            searchTarget[1].SetActive(true);
        }
        isCoreShowed = false;
        earthPosSave.z = 7f;
        earthPosZ.localPosition = earthPosSave;
        earthPosZ.gameObject.SetActive(true);
        ResetKnockRemain();
        SetParticleColor(false);
    }

    private void CoreShow() {
        for (int i = 0; i < normalDD.Length; i++) {
            if (normalDD[i]) {
                normalDD[i].SetActive(false);
            }
        }
        for (int i = 0; i < criticalDD.Length; i++) {
            if (criticalDD[i]) {
                criticalDD[i].SetActive(true);
            }
        }
        if (bodyPriority) {
            bodyPriority.priority = 50;
        }
        isCoreShowed = true;
        ResetKnockRemain();
        BootDeathEffect(earthBreak);
        earthPosZ.gameObject.SetActive(false);
        coreTimeRemain = coreTimeMax;
        coreShowHP = nowHP;
        coreHideConditionDamage = GetCoreHideConditionDamage();
        healEffectEmitted = false;
        heavyKnocked = false;
        EmitEffect(effCoreShow);
        SetParticleColor(true);
    }

    protected override void BootDeathEffect(EnemyDeath enemyDeath) {
        for (int i = 1; i < 6; i++) {
            enemyDeath.colorNum = i;
            base.BootDeathEffect(enemyDeath);
            enemyDeath.deadEffect.prefab = null;
        }
    }

    protected override void SetLevelModifier() {
        base.SetLevelModifier();
        if (!GameManager.Instance.IsPlayerAnother) {
            exp = 0;
        }
    }

    void SetCriticalDDKnockedRate() {
        bool rawFlag = sandstarRawEnabled && !isForAmusement;
        for (int i = 0; i < rawKnockChangeDD.Length; i++) {
            if (rawKnockChangeDD[i]) {
                rawKnockChangeDD[i].knockedRate = rawFlag ? criticalDDKnockRaw : criticalDDKnockUsual; ;
            }
        }
        for (int i = 0; i < rawKnockEffectiveDD.Length; i++) {
            if (rawKnockEffectiveDD[i]) {
                rawKnockEffectiveDD[i].knockedRate = criticalDDKnockUsual;
                rawKnockEffectiveDD[i].colorType = rawFlag ? damageColor_Hyper : damageColor_Critical;
            }
        }
        if (damageDetectionEarth) {
            damageDetectionEarth.knockedRate = rawFlag || GameManager.Instance.save.difficulty >= GameManager.difficultyVU ? earthDDKnockNormal : earthDDKnockEasy;
        }
    }

    public override void SetSandstarRaw() {
        base.SetSandstarRaw();
        if (state != State.Dead && isCoreShowed) {
            CoreHide();
        }
        if (!GameManager.Instance.IsPlayerAnother) {
            exp = 0;
        }
        SetCriticalDDKnockedRate();
    }

    protected override void Update_Targeting() {
        base.Update_Targeting();
        Vector3 targetRefPos = trans.position;
        if (CharacterManager.Instance.playerTrans) {
            targetRefPos.y = CharacterManager.Instance.playerTrans.position.y + 1.5f;
        } else {
            targetRefPos.y = trans.position.y + 5f;
        }
        targetRefPos.y = Mathf.Clamp(targetRefPos.y, trans.position.y + searchRefHeightRange.x, trans.position.y + searchRefHeightRange.y);
        searchReference.position = Vector3.SmoothDamp(searchReference.position, targetRefPos, ref smoothVelocity, 0.4f);
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (!battleStarted) {
            stateTime = 0f;
            if (battleReadyTimer > 0f) {
                battleReadyTimer -= deltaTimeCache;
                if (battleReadyTimer <= 0f) {
                    if (Event_LastBattle.Instance) {
                        Event_LastBattle.Instance.BattleReady(this);
                        BGM.Instance.Play(bgmNumberBattle);
                        Ambient.Instance.Play(-1);
                        changeMusicEnabled = false;
                        bgmReplayOnEnd = false;
                    } else {
                        BattleStart();
                    }
                }
            }
            if (eventProgress == 1 && targetTrans && GetTargetDistance(true, true, false) < 25f * 25f) {
                EmitEffect(effBattleReady);
                anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.AnimSpeed], 1);
                battleReadyTimer = 4.5f;
                eventProgress = 2;
            }
        }
        if (GameManager.Instance.save.difficulty >= GameManager.difficultyVU || sandstarRawEnabled) {
            weakProgress = (nowHP <= GetMaxHP() / 3 ? 2 : nowHP <= GetMaxHP() * 2 / 3 ? 1 : 0);
        } else {
            weakProgress = (nowHP <= GetMaxHP() / 2 ? 1 : 0);
        }
        if (state != State.Attack && laserEnabled) {
            LaserCancel();
        }
        laserAttackedTimeRemain -= deltaTimeMove;
        attractionTime = 6f - weakProgress;
        confuseTime = 3f - weakProgress * 0.5f;
        attackedTimeRemainOnDamage = 1.5f - weakProgress * 0.25f;
        attracted = (decoySave == target);
        if (isCoreShowed) {
            if (state != State.Dead) {
                coreTimeRemain -= deltaTimeCache * (coreTimeRemain >= 1f ? CharacterManager.Instance.riskyIncSqrt : 1f);
                if (!healEffectEmitted && coreTimeRemain < 1.25f) {
                    EmitEffect(effCoreHide);
                    healEffectEmitted = true;
                }
                if (coreTimeRemain < 0) {
                    CoreHide();
                }
            }
        } else {
            if (earthPosSave.z < 20f && !(state == State.Damage && isDamageHeavy)) {
                earthPosSave.z = Mathf.Clamp(earthPosSave.z + deltaTimeCache * 2f, 7f, 20f);
                earthPosZ.localPosition = earthPosSave;
            }
        }
        if (battleStarted) {
            if (earthRotX && earthRotY && Time.timeScale > 0f) {
                float rotSpeedMax = 1f;
                if (enemyCanvasLoaded && (enemyCanvasChildObject[(int)EnemyCanvasChild.paperPlane].activeSelf || enemyCanvasChildObject[(int)EnemyCanvasChild.margayVoice].activeSelf)) {
                    rotSpeedMax = 0.8f;
                }
                if (state == State.Spawn || state == State.Dead || state == State.Attack) {
                    earthRotSpeed = Mathf.Clamp(earthRotSpeed - deltaTimeMove * 2f, Mathf.Clamp01(0.1f * attackedTimeRemainReduceMultiplier), rotSpeedMax);
                } else {
                    earthRotSpeed = Mathf.Clamp(earthRotSpeed + deltaTimeMove * 2f, Mathf.Clamp01(0.1f * attackedTimeRemainReduceMultiplier), rotSpeedMax);
                }
                if (earthRotSpeed > 0f) {
                    float rotSpeedMul = (weakProgress >= 2 ? 1.666667f : weakProgress == 1 ? 1.333333f : 1f);
                    earthRotTime += earthRotSpeed * rotSpeedMul * 0.25f * deltaTimeMove;
                    if (earthRotTime >= 2f) {
                        earthRotTime -= 2f;
                    }
                    Vector3 earthRotXTemp = vecZero;
                    earthRotXTemp.x = Mathf.Clamp(Mathf.Sin(earthRotTime * Mathf.PI) * 7.5f + 1.25f, -8f, 8f);
                    earthRotX.localEulerAngles = earthRotXTemp;
                    Vector3 earthRotYTemp = earthRotY.localEulerAngles;
                    earthRotYTemp.y -= earthRotSpeed * rotSpeedMul * (GameManager.Instance.save.difficulty >= GameManager.difficultyVU ? 25f : 20f) * deltaTimeMove;
                    if (earthRotYTemp.y <= -180f) {
                        earthRotYTemp.y += 360f;
                    }
                    earthRotY.localEulerAngles = earthRotYTemp;
                }
            }
            if (eyePivot) {
                if (targetTrans) {
                    Vector3 targetPos = targetTrans.position;
                    targetPos.y += 0.2f;
                    if (state == State.Attack && eyeFastEnabled) {
                        SmoothRotation(eyePivot, targetPos - GetCenterPosition(), (weakProgress >= 2 ? 1.6f : weakProgress == 1 ? 1.3f : 1f) * (lookingIsSlow ? (sandstarRawEnabled ? 1.5f : 1f) : 2f), 0.625f);
                    } else {
                        SmoothRotation(eyePivot, targetPos - GetCenterPosition(), 1f, 0.5f);
                    }
                } else if (nullTarget) {
                    SmoothRotation(eyePivot, nullTarget.position - GetCenterPosition(), 0.5f, 0.5f);
                }
            }
        }
        if (Time.timeScale > 0) {
            int particleFlagTemp = (QualitySettings.GetQualityLevel() <= 1 ? parLQ : parHQ);
            if (particleFlagTemp != particleFlagSave) {
                particleFlagSave = particleFlagTemp;
                for (int i = 0; i < livingParticle.Length; i++) {
                    if (livingParticle[i].gameObject.activeSelf != (i == particleFlagTemp)) {
                        livingParticle[i].gameObject.SetActive(i == particleFlagTemp);
                    }
                }
            }
        }
    }

    public void QuakeAttack(int pivotIndex, float radius = 15f) {
        if (pivotIndex < quakePivot.Length && quakePivot[pivotIndex] && state == State.Attack) {
            CameraManager.Instance.SetQuake(quakePivot[pivotIndex].position, 10, 4, 0, 0, 1.5f, radius, 200f);
        }
    }

    public void QuakeKnockHeavy() {
        if (!isItem) {
            CameraManager.Instance.SetQuake(trans.position, 5, 4, 0, 0f, 3f, 15f, 200f);
        }
    }

    public void QuakeKnockLight() {
        if (!isItem) {
            CameraManager.Instance.SetQuake(trans.position, 5, 4, 0, 0, 1.5f, 15f, 200f);
        }
    }

    void ChangeBodyMaterial(int matType) {
        if (!bodyMaterialChanged) {
            Material[] mats = bodyRenderer.materials;
            mats[0] = bodyMaterials[matType];
            bodyRenderer.materials = mats;
            bodyMaterialChanged = true;
        }
    }

    protected override void BattleStart() {
        base.BattleStart();
        spawnStiffTime = 0f;
        actDistNum = 1;
        attackedTimeRemain = 2f;
        laserAttackedTimeRemain = 24f;
        if (blackMinmiIDRank > 0) {
            attackedTimeRemain += blackMinmiIDRank * 0.4f;
        }
        for (int i = 0; i < startActivateObj.Length; i++) {
            if (startActivateObj[i]) {
                startActivateObj[i].SetActive(true);
            }
        }
        for (int i = 0; i < startDeactivateObj.Length; i++) {
            if (startDeactivateObj[i]) {
                startDeactivateObj[i].SetActive(false);
            }
        }
        ChangeBodyMaterial(1);
    }

    public void BattleStartExternal() {
        BattleStart();
    }

    public override void SetForDictionary(bool toSuperman, int layer) {
        base.SetForDictionary(toSuperman, layer);
        ChangeBodyMaterial(2);
    }

    public override void TakeDamage(int damage, Vector3 position, float knockAmount, Vector3 knockVector, CharacterBase attacker = null, int colorType = 0, bool penetrate = false) {
        if (battleStarted) {
            base.TakeDamage(damage, position, knockAmount, knockVector, attacker, colorType, penetrate);
        }
    }

    protected override void DamageCommonProcess() {
        base.DamageCommonProcess();
        if (actDistNum != 1) {
            BattleStart();
        }
    }

    protected override void KnockLightProcess() {
        base.KnockLightProcess();
        if (!isCoreShowed && state != State.Dead) {
            CoreShow();
        }
        if (!isSuperarmor) {
            QuakeKnockLight();
            EmitEffect(effKnockLight);
        }
    }

    protected override void KnockHeavyProcess() {
        base.KnockHeavyProcess();
        heavyKnocked = true;
        if (isCoreShowed && coreTimeRemain > 1.5f) {
            coreTimeRemain = 1.5f;
        }
        QuakeKnockHeavy();
        EmitEffect(effKnockHeavy);
        TakeDamageFixKnock(GetMaxHP() / 20, GetCenterPosition(), 0, vecZero, CharacterManager.Instance.pCon, damageColor_Critical, true);
    }

    public override float GetKnocked() {
        return base.GetKnocked() * (!heavyKnocked && isCoreShowed && nowHP <= GetCoreHideBorder() ? 0 : 1);
    }

    protected override void Attack() {
        base.Attack();
        int attackTemp;
        if (!nearFarSwitch) {
            attackTemp = nearAttackSave;
            nearAttackSave = (nearAttackSave + 1) % 12;
        } else {
            int max = (laserAttackedTimeRemain <= 0f ? 21 : 20);
            attackTemp = Random.Range(12, max);
            for (int i = 0; i < farAttackSave.Length; i++) {
                if (attackTemp == farAttackSave[i]) {
                    attackTemp = Random.Range(12, max);
                    break;
                }
            }
            if (laserAttackedTimeRemain <= -10f && Random.Range(0, 100) < 50) {
                attackTemp = 20;
            }
            farAttackSave[0] = attackTemp;
        }
        nearFarSwitch = !nearFarSwitch;
        float intervalPlus = 2f / (weakProgress >= 2 ? 2f : weakProgress == 1 ? 1.414214f : 1f);
        float attackSpeed = 1f;
        if (sandstarRawEnabled) {
            attackSpeed = (weakProgress >= 2 ? 1.2f : 1f);
        }
        lookingFlag = true;
        lookingIsSlow = false;
        eyeFastEnabled = false;
        throwRushEnabled = false;
        throwRushInterval = 0f;
        throwReadyPointer = 0;
        throwStartPointer = 0;
        Vector3 targetPos = vecForward;
        if (targetTrans) {
            targetPos = targetTrans.position;
            targetPos.y += 0.2f;
        } else if (nullTarget) {
            targetPos = nullTarget.position;
        }
        attackPivotFlexible.LookAt(targetPos);
        targetPos.y = attackPivotHorizontal.position.y;
        attackPivotHorizontal.LookAt(targetPos);
        targetPos.y = trans.position.y;
        attackPivotPosXZ.position = targetPos;
        switch (attackTemp) {
            case 0:
                AttackBase(0, 1.05f, 3.4f, 0, 240f / 60f / attackSpeed, 240f / 60f / attackSpeed + intervalPlus, 0f, attackSpeed, false);
                EmitEffect(effCylinderReady);
                break;
            case 1:
                AttackBase(1, 1.05f, 3.4f, 0, 260f / 60f / attackSpeed, 260f / 60f / attackSpeed + intervalPlus, 0f, attackSpeed, false);
                EmitEffect(effCylinderReady);
                break;
            case 2:
                AttackBase(2, 1.05f, 3.4f, 0, 240f / 60f / attackSpeed, 240f / 60f / attackSpeed + intervalPlus, 0f, attackSpeed, false);
                EmitEffect(effCylinderReady);
                break;
            case 3:
                AttackBase(3, 1.05f, 3.4f, 0, 240f / 60f / attackSpeed, 240f / 60f / attackSpeed + intervalPlus, 0f, attackSpeed, false);
                EmitEffect(effStampReadyPar);
                EmitEffect(effStampReadySE);
                break;
            case 4:
                AttackBase(4, 1.05f, 3.4f, 0, 260f / 60f / attackSpeed, 260f / 60f / attackSpeed + intervalPlus, 0f, attackSpeed, false);
                EmitEffect(effCylinderReady);
                break;
            case 5:
                AttackBase(5, 1.05f, 3.4f, 0, 260f / 60f / attackSpeed, 260f / 60f / attackSpeed + intervalPlus, 0f, attackSpeed, false);
                EmitEffect(effCylinderReady);
                break;
            case 6:
                AttackBase(6, 1.05f, 3.4f, 0, 430f / 60f / attackSpeed, 430f / 60f / attackSpeed + intervalPlus, 0f, attackSpeed, false);
                EmitEffect(effCylinderReady);
                break;
            case 7:
                AttackBase(7, 1.05f, 3.4f, 0, 240f / 60f / attackSpeed, 240f / 60f / attackSpeed + intervalPlus, 0f, attackSpeed, false);
                EmitEffect(effCylinderReady);
                break;
            case 8:
                AttackBase(8, 1.05f, 3.4f, 0, 290f / 60f / attackSpeed, 290f / 60f / attackSpeed + intervalPlus, 0f, attackSpeed, false);
                EmitEffect(effCylinderReady);
                break;
            case 9:
                AttackBase(9, 1.05f, 3.4f, 0, 260f / 60f / attackSpeed, 260f / 60f / attackSpeed + intervalPlus, 0f, attackSpeed, false);
                EmitEffect(effCylinderReady);
                break;
            case 10:
                AttackBase(10, 1.05f, 3.4f, 0, 290f / 60f / attackSpeed, 290f / 60f / attackSpeed + intervalPlus, 0f, attackSpeed, false);
                EmitEffect(effCylinderReady);
                break;
            case 11:
                AttackBase(11, 1.2f, 5.0f, 0, 240f / 60f / attackSpeed, 240f / 60f / attackSpeed + intervalPlus, 0f, attackSpeed, false);
                EmitEffect(effCylinderReady);
                break;
            case 12:
                AttackBase(12, 1.1f, 3.4f, 0, 210f / 60f / attackSpeed, 210f / 60f / attackSpeed + intervalPlus, 0f, attackSpeed, false);
                EmitEffect(effBossReady);
                break;
            case 13:
                AttackBase(13, 1f, 1.7f, 0, 130f / 60f / attackSpeed, 130f / 60f / attackSpeed + intervalPlus, 0f, attackSpeed, false);
                EmitEffect(effProjectileReady);
                ThrowReadyB2();
                break;
            case 14:
                AttackBase(14, 1f, 1.4f, 0, 240f / 60f / attackSpeed, 240f / 60f / attackSpeed + intervalPlus, 0f, attackSpeed, false);
                EmitEffect(effProjectileReady);
                break;
            case 15:
                AttackBase(15, 1f, 1.4f, 0, 200f / 60f / attackSpeed, 200f / 60f / attackSpeed + intervalPlus, 0f, attackSpeed, false);
                EmitEffect(effProjectileReady);
                ThrowReadyAkula();
                break;
            case 16:
                AttackBase(16, 1f, 1.4f, 0, 170f / 60f / attackSpeed, 170f / 60f / attackSpeed + intervalPlus, 0f, attackSpeed, false);
                EmitEffect(effProjectileReady);
                ThrowReadyCeckyBeast();
                break;
            case 17:
                AttackBase(17, 0.95f, 1.1f, 0, 190f / 60f / attackSpeed, 190f / 60f / attackSpeed + intervalPlus, 0f, attackSpeed, false);
                EmitEffect(effProjectileReady);
                break;
            case 18:
                AttackBase(18, 1.1f, 3.4f, 0, 210f / 60f / attackSpeed, 210f / 60f / attackSpeed + intervalPlus, 0f, attackSpeed, false);
                EmitEffect(effBossReady);
                break;
            case 19:
                AttackBase(19, 1f, 1.1f, 0, 180f / 60f / attackSpeed, 180f / 60f / attackSpeed + intervalPlus, 0f, attackSpeed, false);
                EmitEffect(effProjectileReady);
                break;
            case 20:
                AttackBase(20, 1.4f, 50f, 0, 370f / 60f, 370f / 60f + intervalPlus, 0f, 1f, false);
                EmitEffect(effBossReady);
                eyeFastEnabled = true;
                lookingIsSlow = false;
                laserAttackedTimeRemain = 24f;
                attackDetection[attackLaserBody].multiHitInterval = 0.1f;
                break;
        }
    }

    void LaserMultiHitEnd() {
        if (state == State.Attack) {
            attackDetection[attackLaserBody].multiHitInterval = 0f;
        }
    }    

    void LookingEnd() {
        lookingFlag = false;
    }

    void LookingSlowDown() {
        lookingIsSlow = true;
    }

    void EyeFastEnd() {
        eyeFastEnabled = false;
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        if (lookingFlag) {
            Vector3 targetPos = vecForward;
            if (targetTrans) {
                targetPos = targetTrans.position;
                targetPos.y += 0.2f;
            } else if (nullTarget) {
                targetPos = nullTarget.position;
            }
            switch (attackType) {
                case 0:
                case 10:
                case 11:
                    SmoothRotation(attackPivotFlexible, targetPos - attackPivotFlexible.position, 2f);
                    break;
                case 2:
                    SmoothRotation(attackPivotFlexible, targetPos - attackPivotFlexible.position, 2f, 0.25f);
                    break;
                case 14:
                    SmoothRotation(attackPivotFlexible, targetPos - attackPivotFlexible.position, 3.2f, 0.5f);
                    break;
                case 16:
                    SmoothRotation(attackPivotFlexible, targetPos - attackPivotFlexible.position, 3.2f, 0.5f);
                    break;
                case 3:
                    SmoothPosition(attackPivotPosXZ, targetPos, 10f, 1f, true);
                    break;
                case 6:
                    if (CharacterManager.Instance.playerAudioListener) {
                        Vector3 pointP = CharacterManager.Instance.playerAudioListener.position;
                        for (int i = 0; i < rotateNearPointSettings.Length; i++) {
                            if (rotateNearPointSettings[i].setTarget && rotateNearPointSettings[i].pivotA && rotateNearPointSettings[i].pivotB) {
                                rotateNearPointSettings[i].setTarget.position = PerpendicularFootPoint(rotateNearPointSettings[i].pivotA.position, rotateNearPointSettings[i].pivotB.position, pointP);
                            }
                        }
                    }
                    break;
                case 7:
                case 8:
                case 9:
                case 12:
                    SmoothRotation(attackPivotHorizontal, targetPos - attackPivotHorizontal.position, 2f, 1f, true);
                    break;
                case 17:
                    SmoothRotation(attackPivotHorizontal, targetPos - attackPivotHorizontal.position, 4f * (lookingIsSlow ? 0.5f : 1f), 1f, true);
                    break;
                case 18:
                case 19:
                    SmoothRotation(attackPivotHorizontal, targetPos - attackPivotHorizontal.position, 3.2f * (lookingIsSlow ? 0.5f : 1f), 1f, true);
                    break;
            }
        }
        if (throwRushEnabled) {
            throwRushInterval += deltaTimeMove;
            switch (attackType) {
                case 15:
                    if (throwRushInterval >= 5f / 60f && throwAkulaStart + throwStartPointer < throwAkulaEnd) {
                        throwRushInterval -= 5f / 60f;
                        AdjustThrowSpeed(throwAkulaStart + throwStartPointer, 30f, 30f, 0.4f);
                        throwing.ThrowStart(throwAkulaStart + throwStartPointer);
                        throwStartPointer++;
                    }
                    break;
                case 16:
                    if (throwRushInterval >= 5f / 60f && throwStartPointer < 12) {
                        throwRushInterval -= 5f / 60f;
                        int indexTemp = throwCeckyBeastStart + throwStartPointer % 2;
                        AdjustThrowSpeed(indexTemp, 11f, 22f, 0.4f);
                        if (throwStartPointer < 2) {
                            throwing.throwSettings[indexTemp].randomDirection = vecZero;
                        } else {
                            throwing.throwSettings[indexTemp].randomDirection = vecHalf;
                        }
                        throwing.ThrowStart(indexTemp);
                        if (throwStartPointer < 10) {
                            throwing.ThrowReady(indexTemp);
                        }
                        throwStartPointer++;
                    }
                    break;
            }
        }
    }

    public override void EmitEffectString(string type) {
        if (state == State.Attack) {
            switch (type) {
                case "CylinderStart":
                    EmitEffect(effCylinderStart);
                    break;
                case "CylinderEnd":
                    EmitEffect(effCylinderEnd);
                    break;
                case "SwingDownReady":
                    EmitEffect(effSwingDownReady);
                    break;
                case "SwingDownStart":
                    EmitEffect(effSwingDownStart);
                    break;
                case "SwingDownEnd":
                    EmitEffect(effSwingDownEnd);
                    QuakeAttack(quakeHorizontal, 30f);
                    break;
                case "BowlingStart":
                    EmitEffect(effBowlingStart);
                    break;
                case "BowlingEnd":
                    EmitEffect(effBowlingEnd);
                    break;
                case "StampStart":
                    EmitEffect(effStampStart);
                    break;
                case "StampImpact":
                    EmitEffect(effStampImpact);
                    QuakeAttack(quakeXZPos, 15f);
                    break;
                case "StampEnd":
                    EmitEffect(effStampEnd);
                    break;
                case "CrossDownReady":
                    EmitEffect(effCrossDownReady);
                    break;
                case "CrossDownStart":
                    EmitEffect(effCrossDownStart);
                    break;
                case "CrossDownEnd":
                    EmitEffect(effCrossDownEnd);
                    QuakeAttack(quakeXZPos, 25f);
                    break;
                case "OpenDownReady":
                    EmitEffect(effOpenDownReady);
                    break;
                case "OpenDownStart":
                    EmitEffect(effOpenDownStart);
                    break;
                case "OpenDownEnd":
                    EmitEffect(effOpenDownEnd);
                    QuakeAttack(quakeHorizontal, 30f);
                    break;
                case "RotateReady":
                    EmitEffect(effRotateReady);
                    break;
                case "RotateStart":
                    // EmitEffect(effRotateStart);
                    break;
                case "RotateEnd":
                    EmitEffect(effRotateEnd);
                    break;
                case "LineStart":
                    EmitEffect(effLineStart);
                    break;
                case "EngageReady":
                    EmitEffect(effSwingDownReady);
                    break;
                case "EngageStart":
                    EmitEffect(effCylinderStart);
                    break;
                case "EngageEnd":
                    EmitEffect(effCylinderEnd);
                    break;
                case "ComplexStart":
                    EmitEffect(effComplexStart);
                    break;
                case "ComplexEnd1":
                    EmitEffect(effComplexEnd1);
                    break;
                case "ComplexEnd2":
                    EmitEffect(effComplexEnd2);
                    QuakeAttack(quakeHorizontal, 20f);
                    break;
                case "ClapReady":
                    EmitEffect(effClapReady);
                    break;
                case "ClapStart":
                    EmitEffect(effClapStart);
                    break;
                case "ClapEnd":
                    EmitEffect(effClapEnd);
                    break;
                case "BossReady":
                    EmitEffect(effBossReady);
                    break;
                case "AlligatorClipsStart":
                    EmitEffect(effAlligatorClipsStart);
                    break;
                case "AlligatorClipsEnd":
                    EmitEffect(effAlligatorClipsEnd);
                    QuakeAttack(quakeCenter, 25f);
                    break;
                case "RedGrowlReady":
                    EmitEffect(effRedGrowlReady);
                    break;
                case "GlaucusReady":
                    EmitEffect(effGlaucusReady);
                    break;
                case "SnowTowerReady":
                    EmitEffect(effSnowTowerReady);
                    break;
                case "SnowTowerStart":
                    EmitEffect(effSnowTowerStart);
                    break;
                case "SnowTowerEnd":
                    EmitEffect(effSnowTowerEnd);
                    QuakeAttack(quakeHorizontal, 20f);
                    break;
                case "SnowTowerBreak":
                    EmitEffect(effSnowTowerBreak);
                    break;
                case "LaserCountDown":
                    EmitEffect(effLaserCountDown);
                    break;
            }
        }
    }

    void ThrowReadyB2() {
        if (state == State.Attack) {
            for (int i = throwB2Start; i < throwB2End; i++) {
                throwing.ThrowReady(i);
            }
        }
    }

    void ThrowStartB2() {
        if (state == State.Attack) {
            for (int i = throwB2Start; i < throwB2End; i++) {
                throwing.ThrowStart(i);
            }
            EmitEffect(effB2Start);
        }
    }

    void ThrowReadyRedGrowl() {
        if (state == State.Attack) {
            if (throwRedGrowlStart + throwReadyPointer < throwRedGrowlEnd) {
                throwing.ThrowReady(throwRedGrowlStart + throwReadyPointer);
                throwReadyPointer++;
            }
        }
    }

    void ThrowStartRedGrowl() {
        if (state == State.Attack) {
            if (throwRedGrowlStart + throwStartPointer < throwRedGrowlEnd) {
                AdjustThrowSpeed(throwRedGrowlStart + throwStartPointer, 14f, 28f, 0.4f);
                throwing.ThrowStart(throwRedGrowlStart + throwStartPointer);
                throwStartPointer++;
            }
        }
    }

    void ThrowReadyAkula() {
        if (state == State.Attack) {
            for (int i = throwAkulaStart; i < throwAkulaEnd; i++) {
                throwing.ThrowReady(i);
            }
        }
    }

    void ThrowStartRush() {
        if (state == State.Attack) {
            throwRushEnabled = true;
            throwRushInterval = 0f;
        }
    }

    void ThrowReadyCeckyBeast() {
        if (state == State.Attack) {
            for (int i = 0; i < 2; i++) {
                throwing.ThrowReady(throwCeckyBeastStart + i);
            }
        }
    }

    void ThrowStartGlaucus() {
        if (state == State.Attack) {
            for (int i = throwGlaucusStart; i < throwGlaucusEnd; i++) {
                throwing.ThrowStart(i);
            }
        }
    }

    void ThrowReadyBomb() {
        if (state == State.Attack) {
            for (int i = throwBombStart; i < throwBombEnd; i++) {
                throwing.ThrowReady(i);
                effect[effBigDogReadyPar].pivot = throwing.throwSettings[i].from.transform;
                EmitEffect(effBigDogReadyPar);
            }
            throwing.ThrowReady(throwBombBig);
            effect[effBigDogReadyPar].pivot = throwing.throwSettings[throwBombBig].from.transform;
            EmitEffect(effBigDogReadyPar);
            EmitEffect(effBigDogReadySE);
        }
    }

    void ThrowStartBomb() {
        if (state == State.Attack) {
            int accurateIndex = Random.Range(throwBombStart, throwBombEnd);
            for (int i = throwBombStart; i < throwBombEnd; i++) {
                if (i == accurateIndex) {
                    throwing.throwSettings[i].randomDirection = vecZero;
                } else {
                    throwing.throwSettings[i].randomDirection = vecBombRand;
                }
                AdjustThrowSpeed(i, 10f, 10f, 0.4f);
                throwing.ThrowStart(i);
            }
            AdjustThrowSpeed(throwBombBig, 10f, 5f, 0.2f);
            throwing.ThrowStart(throwBombBig);
            EmitEffect(effBigDogStart);
        }
    }

    void AdjustThrowSpeed(int throwIndex, float borderDist, float minSpeed, float multiRate) {
        if (throwIndex < throwing.throwSettings.Length) {
            if (targetTrans) {
                float distance = Vector3.Distance(throwing.throwSettings[throwIndex].from.transform.position, targetTrans.position);
                if (distance <= borderDist) {
                    throwing.throwSettings[throwIndex].velocity = minSpeed;
                } else {
                    throwing.throwSettings[throwIndex].velocity = minSpeed + (distance - borderDist) * multiRate;
                }
            } else {
                throwing.throwSettings[throwIndex].velocity = minSpeed;
            }
        }
    }

    Vector3 PerpendicularFootPoint(Vector3 a, Vector3 b, Vector3 p) {
        return a + Vector3.Project(p - a, b - a);
    }

    void LaserCancel() {
        laserEnabled = false;
        for (int i = 0; i < laserOption.Length; i++) {
            if (laserOption[i]) {
                laserOption[i].CancelLaser();
            }
        }
        for (int i = 0; i < raycaster.Length; i++) {
            if (raycaster[i]) {
                raycaster[i].Deactivate();
            }
        }
    }

    void LaserReady() {
        laserEnabled = true;
        for (int i = 0; i < laserOption.Length; i++) {
            if (laserOption[i]) {
                laserOption[i].LightFlickeringChargeStart();
            }
        }
        for (int i = 0; i < raycaster.Length; i++) {
            if (raycaster[i]) {
                raycaster[i].hitEffectEnabled = false;
                raycaster[i].Activate();
            }
        }
        AttackStart(attackLaserDummy);
    }

    void LaserStart() {
        laserEnabled = true;
        for (int i = 0; i < laserOption.Length; i++) {
            if (laserOption[i]) {
                laserOption[i].LightFlickeringChargeEnd();
            }
        }
        for (int i = 0; i < raycaster.Length; i++) {
            if (raycaster[i]) {
                raycaster[i].hitEffectEnabled = true;
                raycaster[i].Activate();
            }
        }
        AttackEnd(attackLaserDummy);
        AttackStart(attackLaserBody);
    }

    void LaserEnd() {
        laserEnabled = false;
        for (int i = 0; i < laserOption.Length; i++) {
            if (laserOption[i]) {
                laserOption[i].LightFlickeringBlastEnd();
            }
        }
        for (int i = 0; i < raycaster.Length; i++) {
            if (raycaster[i]) {
                raycaster[i].Deactivate();
            }
        }
        AttackEnd(attackLaserBody);
    }

    protected override void Start_Process_Dead() {
        base.Start_Process_Dead();
        EmitEffect(effDeadSave);
        if (!isCoreShowed) {
            CoreShow();
        }
        if (isLastOne) {
            // CharacterManager.Instance.ShowBossResult(enemyID, sandstarRawEnabled);
            eventState = 1;
            deadTimer = 10000f;
            destroyOnDead = false;
            bgmReplayOnEnd = false;
            deathEffectEnabled = false;
            winActionEnabled = false;
            DeadProcess();
        } else {
            eventState = 0;
            deadTimer = 3.5f;
            destroyOnDead = true;
        }
    }

    protected override void Update_Process_Dead() {
        base.Update_Process_Dead();
        switch (eventState) {
            case 1:
                if (stateTime >= 0.5f) {
                    EmitEffect(effEvent1);
                    BGM.Instance.StopFade(2f);
                    CameraManager.Instance.SetQuake(transform.position, 2, 8, 2f, 5f, 2f, 100f, 200f);
                    CharacterManager.Instance.pCon.ForceStopForEvent(10f);
                    CharacterManager.Instance.pCon.SupermanEnd(false);
                    CharacterManager.Instance.StopFriends();
                    eventState++;
                    break;
                }
                break;
            case 2:
                if (stateTime >= 4f) {
                    if (GameManager.Instance.IsPlayerAnother) {
                        for (int i = 0; i < anotherMat.Length; i++) {
                            SetForChangeMatSet(anotherMat[i], true);
                        }
                        if (lightObj[0]) {
                            lightObj[0].SetActive(false);
                        }
                        if (lightObj[1]) {
                            lightObj[1].SetActive(true);
                        }
                        BootDeathEffect(ed);
                        anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.Dead], false);
                        anim.SetTrigger("DeadEvent");
                        if (effect[effDeadSave].instance) {
                            Destroy(effect[effDeadSave].instance);
                        }
                        if (effect[effEvent1].instance) {
                            Destroy(effect[effEvent1].instance);
                        }
                        if (supermanEffectActivated) {
                            SetSupermanEffect(false);
                        }
                        for (int i = 0; i < startActivateObj.Length; i++) {
                            if (startActivateObj[i]) {
                                startActivateObj[i].SetActive(false);
                            }
                        }
                        PauseController.Instance.pauseEnabled = false;
                        CameraManager.Instance.CancelQuake();
                        CameraManager.Instance.SetQuake(transform.position, 8, 4, 0, 0, 1.5f, 50, 200f);
                        CharacterManager.Instance.ForceStopForEventAll(30f);
                        if (cervalBallPrefab && CharacterManager.Instance.playerTrans) {
                            Vector3 playerPos = CharacterManager.Instance.playerTrans.position;
                            playerPos.y = trans.position.y;
                            Vector3 direction = (playerPos - trans.position).normalized;
                            cervalBallInstance = Instantiate(cervalBallPrefab, trans.position + direction * 7f + Vector3.up * 3f, Quaternion.identity);
                            Rigidbody ballRigid = cervalBallInstance.GetComponent<Rigidbody>();
                            if (ballRigid) {
                                ballRigid.isKinematic = true;
                            }
                            if (crackPivot) {
                                crackPivot.transform.LookAt(cervalBallInstance.transform.position);
                                crackPivot.SetActive(true);
                            }
                        }
                        eventState = 10;
                    } else {
                        if (effect[effDeadSave].instance) {
                            Destroy(effect[effDeadSave].instance);
                        }
                        if (effect[effEvent1].instance) {
                            Destroy(effect[effEvent1].instance);
                        }
                        CameraManager.Instance.CancelQuake();
                        PauseController.Instance.SetBlackCurtain(1f, false);
                        eventState++;
                    }
                }
                break;
            case 3:
                if (stateTime >= 4.5f) {
                    EmitEffect(effEvent2);
                    if (eventBackImagePrefab) {
                        eventBackImageInstance = Instantiate(eventBackImagePrefab, PauseController.Instance.blackCurtain.canvas.transform);
                        eventBackImageInstance.transform.SetAsLastSibling();
                    }
                    eventState++;
                }
                break;
            case 4:
                if (stateTime >= 10.5f) {
                    if (eventBackImageInstance) {
                        Destroy(eventBackImageInstance);
                    }
                    eventState++;
                }
                break;
            case 5:
                if (stateTime >= 11f) {
                    if (Event_LastBattle.Instance) {
                        PauseController.Instance.SetBlackCurtain(1f, false);
                        Event_LastBattle.Instance.MoveNextBattle(sandstarRawEnabled);
                    } else {
                        StageManager.Instance.MoveStage(0, 0);
                        PauseController.Instance.SetBlackCurtain(0f, false);
                        Destroy(gameObject);
                    }
                    eventState++;
                }
                break;
            case 6:
                break;
            case 10:
                if (stateTime >= 7f) {
                    if (cervalBallInstance) {
                        Rigidbody ballRigid = cervalBallInstance.GetComponent<Rigidbody>();
                        if (ballRigid) {
                            ballRigid.isKinematic = false;
                            Vector3 direction = (cervalBallInstance.transform.position - trans.position).normalized;
                            ballRigid.AddForce(direction * 3f, ForceMode.VelocityChange);
                        }
                    }
                    eventState++;
                }
                break;
            case 11:
                if (stateTime >= 10f) {
                    if (cervalBallInstance) {
                        Destroy(cervalBallInstance);
                    }
                    if (Event_LastBattleAnother.Instance) {
                        Event_LastBattleAnother.Instance.BattleEnd();
                    } else {
                        PauseController.Instance.ReturnLibraryExternal();
                    }
                    eventState++;
                }
                break;
            case 12:
                break;
        }
    }

}
