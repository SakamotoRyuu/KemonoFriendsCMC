using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mebiustos.MMD4MecanimFaciem;
using Rewired;

public class PlayerController : FriendsBase {

    [System.Serializable]
    public class JumpEffectSetting {
        public bool enabled;
        public AudioSource audioSource;
        public ParticleSystem[] particles;
        public AnimationWingVariation[] animationWings;
    }

    public Transform lookAtTarget;
    public Transform pointer;
    public GameObject[] actionIcon;
    public GameObject grassDisplacer;
    public Transform cConCenterPivot;
    public Transform pilePivot;
    public GameObject wallBreaker;
    public GameObject wallBreakerSp;
    public GameObject pileAttackChecker;
    public JudgeFootMaterial judgeFootMaterial;
    public NavMeshAgent obstacleForGrounded;
    public Transform antimatterPivot;
    public GameObject antimatterPrefab;
    public GameObject justDodgePrefab;
    public GameObject justDodgeEffectPrefab;
    public Transform audioListener;
    public GameObject[] footstepDetectionObj;
    public ChangeMatSet[] goldenMatSet;
    public GameObject[] photoExclude;
    public bool isHyper;
    public bool sandstarAutoHeal;
    public JumpEffectSetting hyperJumpEffect;
    public GameObject projectileNoticeObj;
    public Transform fpEyeRefer;
    public GameObject assistDodgeCounter;

    [System.NonSerialized]
    public Transform climbTarget;
    [System.NonSerialized]
    public bool controlEnabled = true;
    [System.NonSerialized]
    public float justDodgeIntervalRemain;

    protected Player playerInput;
    protected Vector2 axisInput;
    protected Transform climbTargetEnd;
    protected Transform climbMother;
    protected Quaternion climbLookQuaternion;
    protected float inputMagMoment;
    protected float inputMagInertia;
    protected float lastAttackButtonTime;
    protected float lastJumpButtonTime;
    protected bool isAnimEnd = false;
    protected float landingTime = 0f;
    protected SphereCollider enemySphCol;
    protected CapsuleCollider enemyCapCol;
    protected bool climbEnabled = false;
    protected float jumpAttackPosSave;
    protected float jumpShortenTimeRemain;
    protected int quickJumpShortenReserved;
    protected float obstacleDisableTimeRemain;
    protected int displaceEnabled;
    protected float disableInputTimeRemain;
    protected bool boosterFlag;
    protected bool airJumpEnabled = true;
    protected Collider[] _pileAttackCheckerCollider;
    protected CheckTriggerStay _pileAttackCheckerTrigger;
    protected bool _pileAttackCheckerEnabled = true;
    protected Vector3 checkGroundBoxHalfExtents;
    protected float supermanCoolTimeRemain;
    protected float disableDodgeTimeRemain;
    protected Camera mainCamera;
    protected Transform camT;
    protected GameObject justDodgeInstance;
    protected float particleJustDodgeTimeRemain;
    protected bool dodgeCancelEnabled;
    protected float groundedFixTimeRemain;
    protected float hyperJumpedTimeRemain;
    protected bool attackingMoveEnabled;
    protected float attackingMoveReservedTimer;
    protected float projectileNoticeTimeRemain;
    
    protected float defCCRadius;
    protected float defCCHeight;
    protected float defCCStepOffset;
    protected float defCCSlopeLimit;
    protected bool climbCC;
    protected bool climbAnimFlag;
    protected LayerMask landingPointerLayer;

    protected const int rightDet = 0;
    protected const int leftDet = 1;
    protected const int impactDet = 2;
    protected const int screwDet = 3;
    protected const int boosterDet = 4;
    protected const int bootsLeftDet = 5;
    protected const int bootsRightDet = 6;
    protected const int actionClimbIndex = 0;

    protected const int effectJump = 0;
    protected const int effectPile = 1;
    protected const int effectSpin = 2;
    protected const int effectSpinPlasma = 3;
    protected const int effectBolt = 4;
    protected const int effectPileEnd = 5;
    protected const int effectSlash = 6;
    protected const int effectQuick = 7;

    protected const int resurrectionId = 57;
    protected const int waveNormalThrowIndex = 0;
    protected const int wavePlasmaThrowIndex = 1;
    protected const int spinNormalThrowIndex = 2;
    protected const int spinPlasmaThrowIndex = 3;
    protected const int boltThrowIndex = 4;
    protected const int judgementThrowIndex = 5;
    protected const int slashRightThrowIndex = 6;
    protected const int slashLeftThrowIndex = 7;

    protected float turnSpeedMultiplier = 1f;
    protected Vector3 targetDirection;
    protected Vector3 lookDirection;
    protected Quaternion freeRotation;
    protected float velocity;
    protected float fixMoveTimeRemain;
    protected Vector3 fixMoveVector;
    protected int targetMissingCount = 0;
    protected bool bothHandAttacking = false;
    protected GameObject playerLightInstance;
    protected float pileDuration = 0f;
    protected bool isSkillAttacking = false;
    protected bool isScrewJumping = false;
    protected float impactAttackDuration;
    protected bool checkPaused = false;
    protected Vector3 attackOffsetNormal = new Vector3(0f, 0f, 1.2f);
    protected Vector3 attackOffsetBothHand = new Vector3(-0.22f, 0f, 1.2f);
    protected float[] costArray = new float[5];
    protected Vector3 eulerVec = Vector3.zero;
    protected bool isPlasma;
    protected bool minmiGoldenSave = false;
    protected bool photoSave;
    protected int lightTypeSave;
    protected RuntimeAnimatorController animConSave;
    protected bool animConIsChanging;
    protected float bothHandAttackMul = 1.6f;
    protected float bothHandKnockMul = 2.5f;
    protected EnemyBase targetEnemyBase;
    protected GameObject targetObjSave = null;
    protected float targetRadiusSave;
    protected float ridingTime;
    protected Quaternion firstPersonRotationDamp;
    protected const float judgementAttackSandstarCost = -2f;
    protected const float impactAttackConditionSpeed = 16.875f;
    protected const int resurrectionFriendsID = 31;
    protected const int pileAttackIndex = 5;
    protected const float landingTimeMax = 9f / 60f;
    protected float footstepJumpTimeRemain;
    static readonly Vector2 vec2zero = Vector2.zero;
    //Trophy
    protected float t_JustDodgeCounterTimeRemain;
    protected int t_JudgementDefeatCount;
    protected CharacterManager.JustDodgeType t_NowJustDodgeType;
    protected float fpEyeForward;
    protected float fpEyeForwardVel;
    protected float fpEyeHeight;
    protected float fpEyeHeightVel;
    protected float fpHorizontalValue;
    protected float fpVerticalValue;
    protected int dodgeDirType;
    protected const float justDodgeIntervalMax = 0.5f;

    //BattleAssist
    protected int assistIndexSave = -1;
    protected int attackTypeSave = -1;
    protected float assistTimeRemain;
    protected float assistDodgeTimeRemain;
    protected float assistJumpingMoveTimeRemain;
    protected float heightDistConditionTime;
    protected float heightAdjustTimer;
    protected float waveAttackedTimeRemain;
    protected bool[] attackConditions = new bool[5];
    protected bool forcePile;
    protected bool forceSpin;
    protected bool forceWave;
    protected bool forceBolt;
    protected bool forceScrew;
    protected bool forceWildReleaseOn;
    protected bool forceWildReleaseOff;
    protected bool forceDodge;
    protected int forceDodgeDir;


    protected override void Awake() {
        base.Awake();
        isPlayer = true;
        landingPointerLayer = LayerMask.GetMask("Default", "Field", "EnemyCollision", "EnemyProjectile", "SecondField", "ThirdField");
        defCCRadius = cCon.radius;
        defCCHeight = cCon.height;
        defCCStepOffset = cCon.stepOffset;
        defCCSlopeLimit = cCon.slopeLimit;
        watchoutTime = 0f;
        lockTargetTime = 0f;
        if (isHyper) {
            superAttackRate = 3f;
            superKnockRate = 3.5f;
        } else {
            superAttackRate = 1.5f;
            superKnockRate = 1.75f;
        }
        sickHealSpeed = (isHyper || sandstarAutoHeal ? 2f : 1f);
        superSTRate = 1.5f;
        dodgeStiffTime = 0.6f;
        dodgeDistance = 4.5f;
        dodgeMutekiTime = 0.3f;
        dodgeRemain = dodgePower = 1000f;
        attackLockonDefaultSpeed = 16f;
        deadTimer = 999999;
        climbCC = false;
        searchArea[0].isPlayer = true;
        lightMutekiTime = lightStiffTime + 0.3f;
        heavyMutekiTime = heavyStiffTime + 0.6f;
        setAttackIntervalToStaminaDontHeal = true;
        moveCost.run = 5f;
        moveCost.attack = 6;
        moveCost.step = 6;
        moveCost.quick = 10;
        moveCost.jump = 10;
        moveCost.skill = 16;
        checkGroundBoxHalfExtents = new Vector3(0.08f, 0.08f, 0.08f);
        if (pileAttackChecker) {
            _pileAttackCheckerCollider = pileAttackChecker.GetComponents<Collider>();
            _pileAttackCheckerTrigger = pileAttackChecker.GetComponent<CheckTriggerStay>();
            _pileAttackCheckerEnabled = true;
            PileAttackCheckerActivate(false);
        }
        if (obstacleForGrounded) {
            obstacleForGrounded.updateUpAxis = false;
            obstacleForGrounded.updateRotation = false;
        }
    }

    protected override void Start() {
        base.Start();
        playerInput = GameManager.Instance.playerInput;
        spawnStiffTime = 0.5f;
        lastAttackButtonTime = 100f;
        lastJumpButtonTime = 100f;
        for (int i = 0; i < actionIcon.Length; i++) {
            actionIcon[i].SetActive(false);
        }
        if (CharacterManager.Instance) {
            CharacterManager.Instance.staminaBorder = GetStaminaBorder();
        }
        GetMainCamera();
    }

    public override void ResetGuts() {
        base.ResetGuts();
        if (CharacterManager.Instance) {
            gutsRemain = gutsMax = CharacterManager.Instance.GetDifficultyEffect(CharacterManager.DifficultyEffect.ServalGuts);
            CharacterManager.Instance.SetPlayerGutsRate(1f);
        }
        if (justDodgeInstance != null) {
            Destroy(justDodgeInstance);
        }
        justDodgeIntervalRemain = 0.01f;
        fpVerticalValue = 0f;
        fpHorizontalValue = 0f;
        supermanCoolTimeRemain = 0f;
    }

    protected override void ShowGuts() {
        base.ShowGuts();
        if (CharacterManager.Instance && gutsMax > 0f) {
            CharacterManager.Instance.SetPlayerGutsRate(gutsRemain >= gutsMax ? 1f : gutsRemain <= 0f ? 0f : gutsRemain / gutsMax);
        }
    }

    protected void PileAttackCheckerActivate(bool flag) {
        if (_pileAttackCheckerEnabled != flag) {
            if (_pileAttackCheckerCollider[0]) {
                for (int i = 0; i < _pileAttackCheckerCollider.Length; i++) {
                    _pileAttackCheckerCollider[i].enabled = flag;
                }
            }
            if (_pileAttackCheckerTrigger) {
                _pileAttackCheckerTrigger.enabled = flag;
            }
            _pileAttackCheckerEnabled = flag;
        }
    }

    void CheckBooster() {
        if (GameManager.Instance.save.config[GameManager.Save.configID_ShowArms] > 0 && attackDetection[boosterDet] && nowSpeed >= costRunBaseSpeed * Mathf.Clamp01(1f + GameManager.Instance.save.config[GameManager.Save.configID_FriendsRunningSpeed] * 0.01f) && disableControlTimeRemain <= 0f && !rideTarget) {
            boosterFlag = true;
            if (!attackDetection[boosterDet].attackEnabled) {
                AttackStart(boosterDet);
            }
        }
    }
    
    bool GetPushRunButton() {
        bool answer = true;
        switch (GameManager.Instance.save.config[GameManager.Save.configID_UseRunButton]) {
            case 0:
                answer = true;
                break;
            case 1:
                answer = playerInput.GetButton(RewiredConsts.Action.Run);
                break;
            case 2:
                answer = !playerInput.GetButton(RewiredConsts.Action.Run);
                break;
        }
        return answer;
    }

    void CheckInputMag() {
        inputMagMoment = Mathf.Clamp01(Mathf.Sqrt(axisInput.x * axisInput.x + axisInput.y * axisInput.y));
        if (inputMagMoment >= 0.98f && inputMagMoment < 1f) {
            inputMagMoment = 1f;
        }
        if (inputMagMoment > inputMagInertia) {
            inputMagInertia = inputMagMoment;
        } else if (inputMagMoment < inputMagInertia) {
            inputMagInertia -= deltaTimeCache * (groundedFlag ? 4f : 1.5f);
            if (inputMagMoment > inputMagInertia) {
                inputMagInertia = inputMagMoment;
            }
        }
    }

    void CharacterMovement() {
        CheckInputMag();
        if (GetCanControl()) {
            bool pushRunButton = GetPushRunButton();
            float accel = 0f;
            float walkSp = GetMaxSpeed(true);
            float runSpMax = GetMaxSpeed(false);
            float runSpInput = GetSick(SickType.Ice) ? runSpMax : Mathf.Max(runSpMax * (quickJumping ? 1f : inputMagInertia), walkSp);
            if ((inputMagMoment < 0.1f || ((!canRun || !pushRunButton) && nowSpeed > walkSp)) && !quickJumping) {
                accel = -1f;
            } else if (inputMagMoment >= 0.1f) {
                accel = 1f;
            }
            UpdateTargetDirection();

            if (groundedFlag) {
                if (accel > 0f) {
                    accel *= Mathf.Lerp(GetAcceleration(), GetAcceleration() / 4, (nowSpeed - runSpMax * 0.5f) / (runSpMax * 0.5f));
                } else {
                    accel *= GetAcceleration() * 2;
                }
            } else {
                accel *= GetAcceleration() * (2f / 3f);
            }
            nowSpeed += accel * deltaTimeMove;
            nowSpeed = Mathf.Clamp(nowSpeed, 0f, !canRun || !pushRunButton ? walkSp : runSpInput);

            if (groundedFlag && accel > 0) {
                CheckBooster();
            }
        } else {
            lookDirection = trans.TransformDirection(vecForward);
        }

        if (GetCanDodge() && (playerInput.GetButton(RewiredConsts.Action.Dodge) || forceDodge) && (inputMagMoment > 0.7f || forceDodge || (GameManager.Instance.save.config[GameManager.Save.configID_SimplifyDodgeCommand] != 0 && !(GetCanQuick() && landingTime <= landingTimeMax * 0.5f && lastJumpButtonTime < 0.15f))) && JudgeStamina(GetCost(CostType.Step))) {
            
            SetJustDodge(12f / 60f, CharacterManager.JustDodgeType.DodgeStep);
            if (forceDodge) {
                SideStep_ConsiderWall(forceDodgeDir, 4.5f, 0.3f);
            } else {
                Vector3 diff;
                if (inputMagMoment > 0.7f) {
                    diff = targetDirection.normalized;
                } else if (targetTrans != null) {
                    Vector3 diffTemp = GetEscapeDestination(targetTrans.position, 4.5f) - transform.position;
                    diffTemp.y = 0f;
                    diff = diffTemp.normalized;
                } else {
                    diff = camT.TransformDirection(vecBack);
                }
                var axis = Vector3.Cross(trans.TransformDirection(vecForward), diff);
                var angle = Vector3.Angle(trans.TransformDirection(vecForward), diff) * (axis.y < 0 ? -1 : 1);
                dodgeDirType = 0;
                if (angle <= -45 && angle >= -135) {
                    dodgeDirType = -1;
                } else if (angle >= 45 && angle <= 135) {
                    dodgeDirType = 1;
                } else if (angle >= -45 && angle <= 45) {
                    dodgeDirType = 2;
                }
                SideStep_Vector(diff, dodgeDirType, 4.5f, 0.3f);
            }
            attackProcess = 0;
            quickJumpShortenReserved = 0;
            if (attackedTimeRemain > 0.01f) {
                attackedTimeRemain = 0.01f;
            }
            assistDodgeTimeRemain = 0f;
        } else if (GetCanControl() && axisInput != vec2zero && targetDirection.sqrMagnitude > 0.01f) {
            lookDirection = targetDirection.normalized;
            freeRotation = Quaternion.LookRotation(lookDirection, transform.up);
            eulerVec.y = freeRotation.eulerAngles.y;
            trans.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(eulerVec), (groundedFlag ? 10 : 5) * turnSpeedMultiplier * deltaTimeMove);
        }
        forceDodge = false;
        move += lookDirection * nowSpeed;
    }

    protected bool GetMainCamera() {
        if (mainCamera == null) {
            if (CameraManager.Instance) {
                CameraManager.Instance.SetMainCamera(ref mainCamera);
                CameraManager.Instance.SetMainCameraTransform(ref camT);
            } else {
                mainCamera = Camera.main;
                if (mainCamera) {
                    camT = mainCamera.transform;
                }
            }
        }
        return mainCamera != null;
    }

    public virtual void UpdateTargetDirection() {
        if (camT || GetMainCamera()) {
            turnSpeedMultiplier = 1f;
            var forward = camT.TransformDirection(vecForward);
            forward.y = 0f;
            var right = camT.right;            
            targetDirection = axisInput.x * right + axisInput.y * forward;
        }
    }

    protected override void OnEnable() {
        base.OnEnable();
        if (!isItem) {
            CheckWeapon();
            CheckGrassDisplacer();
            if (GameManager.Instance.minmiGolden != minmiGoldenSave) {
                ChangeGoldenMaterial();
            }
            if (obstacleForGrounded) {
                obstacleForGrounded.enabled = false;
            }
            if (CharacterManager.Instance) {
                CharacterManager.Instance.playerCameraPosition = trans.position;
            }
            if (StageManager.Instance && StageManager.Instance.dungeonController) {
                for (int i = 0; i < searchArea.Length; i++) {
                    searchArea[i].priorityEffectRate = (StageManager.Instance.dungeonController.isPlayerLockonRateMax ? 1f : 0.01f);
                }
            }
        }
        inputMagMoment = inputMagInertia = 0f;
    }

    protected void OnDisable() {
        if (obstacleForGrounded) {
            obstacleForGrounded.enabled = false;
        }
    }

    public override void SetForItem() {
        base.SetForItem();
        if (audioListener) {
            audioListener.gameObject.SetActive(false);
        }
        for (int i = 0; i < footstepDetectionObj.Length; i++) {
            if (footstepDetectionObj[i]) {
                footstepDetectionObj[i].SetActive(false);
            }
        }
    }

    protected void CheckGrassDisplacer() {
        if (displaceEnabled != GameManager.Instance.save.config[GameManager.Save.configID_DisplaceGrass]) {
            displaceEnabled = GameManager.Instance.save.config[GameManager.Save.configID_DisplaceGrass];
            if (grassDisplacer) {
                grassDisplacer.SetActive(displaceEnabled != 0);
            }
        }
    }
    
    protected override void Update() {
        base.Update();
        if (!controlEnabled) {
            if (CheckWeapon()) {
                SetMultiHitBuff(CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Multi));
            }
            CheckGrassDisplacer();
            disableInputTimeRemain = 0.025f;
        } else {
            if (checkWeaponPreservedFlag) {
                if (CheckWeapon()) {
                    SetMultiHitBuff(CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Multi));
                }
            }
            GameManager.Instance.MeasurePlayTime();
        }
        if (!isItem) {
            if (CharacterManager.Instance) {
                /*
                if (GetCanControl() && nowSpeed != 0f) {
                    CharacterManager.Instance.playerCameraPosition = trans.position + transform.TransformDirection(vecForward) * nowSpeed * 0.04f;
                } else {
                    CharacterManager.Instance.playerCameraPosition = trans.position;
                }
                */
                CharacterManager.Instance.playerCameraPosition = trans.position;
            }
            if (GameManager.Instance.minmiGolden != minmiGoldenSave) {
                ChangeGoldenMaterial();
            }
        }
    }

    protected virtual void ChangeGoldenMaterial() {
        minmiGoldenSave = GameManager.Instance.minmiGolden;
        for (int i = 0; i < goldenMatSet.Length; i++) {
            SetForChangeMatSet(goldenMatSet[i], minmiGoldenSave);
        }
    }

    protected float GetPileCost() {
        return GetCost(CostType.Skill) * (1f + CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.SpeedRank) * 0.125f);
    }

    protected float GetSpinCost() {
        return GetCost(CostType.Skill) * (isPlasma ? 1.5625f : 1.25f);
    }

    protected float GetWaveCost() {
        return GetCost(CostType.Skill) * (isPlasma ? 1.25f : 0.75f);
    }

    protected float GetBoltCost() {
        return GetCost(CostType.Skill) * 1.875f;
    }

    public float GetStaminaBorder() {
        costArray[0] = GetCost(CostType.Attack) + (state == State.Jump ? 0f : GetCost(CostType.Jump));        
        if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Pile) != 0) {
            costArray[1] = GetPileCost();
        }
        if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Spin) != 0) {
            costArray[2] = GetSpinCost();
        }
        if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Wave) != 0) {
            costArray[3] = GetWaveCost();
        }
        if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Bolt) != 0 || CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Judgement) != 0) {
            costArray[4] = GetBoltCost() + (state == State.Jump ? 0f : GetCost(CostType.Jump));
        }
        float tempMax = costArray[0];
        for (int i = 1; i < costArray.Length; i++) {
            if (costArray[i] > tempMax) {
                tempMax = costArray[i];
            }
        }
        return tempMax;
    }

    protected override void SetMapChip() {
        mapChipControl = Instantiate(MapDatabase.Instance.prefab[MapDatabase.player], trans).GetComponent<MapChipControl>();
    }    

    protected override void Update_TimeCount() {
        base.Update_TimeCount();
        if (disableInputTimeRemain > 0f) {
            disableInputTimeRemain -= deltaTimeMove;
        }
        if (supermanCoolTimeRemain > 0f) {
            supermanCoolTimeRemain -= deltaTimeCache;
        }
        if (disableDodgeTimeRemain > 0f) {
            disableDodgeTimeRemain -= deltaTimeCache;
        }
        if (groundedFixTimeRemain > 0f) {
            groundedFixTimeRemain -= deltaTimeCache;
        }
        if (hyperJumpedTimeRemain > 0f) {
            hyperJumpedTimeRemain -= deltaTimeCache;
        }
        if (jumpShortenTimeRemain > 0f) {
            jumpShortenTimeRemain -= deltaTimeCache;
        }
        if (footstepJumpTimeRemain > 0f) {
            footstepJumpTimeRemain -= deltaTimeCache;
        }
        if (justDodgeIntervalRemain > 0f) {
            justDodgeIntervalRemain -= deltaTimeCache;
        }
        if (GameManager.Instance.save.config[GameManager.Save.configID_BattleAssist] != 0) {
            if (assistTimeRemain > 0f) {
                assistTimeRemain -= deltaTimeCache;
            }
            if (assistDodgeTimeRemain > 0f && (state != State.Attack || attackingMoveEnabled)) {
                assistDodgeTimeRemain -= deltaTimeCache;
            }
            if (assistJumpingMoveTimeRemain > 0f) {
                assistJumpingMoveTimeRemain -= deltaTimeCache;
            }
        } else {
            assistTimeRemain = 0f;
            assistDodgeTimeRemain = 0f;
            assistJumpingMoveTimeRemain = 0f;
        }
        if (rideTarget) {
            ridingTime += deltaTimeCache;
        } else {
            ridingTime = 0f;
        }
    }

    protected override void Update_Process_Wait() {
    }

    protected override void Update_Process_Dodge() {
        base.Update_Process_Dodge();
        if (quickJumpShortenReserved == 1 && jumpShortenTimeRemain > 0f && !playerInput.GetButton(RewiredConsts.Action.Jump)) {
            quickJumpShortenReserved = 2;
        }
    }

    protected override void Update_Targeting() {
        if (controlEnabled && playerInput.GetButtonDown(RewiredConsts.Action.Aim)) {
            CharacterManager.Instance.autoAim = (CharacterManager.Instance.autoAim + 1) % 2;
            if (CharacterManager.Instance.autoAim == 0) {
                searchArea[0].SetUnlockTarget();
                UISE.Instance.Play(UISE.SoundName.aimoff);
                if (MessageUI.Instance) {
                    MessageUI.Instance.DestroyMessageSlotType(MessageUI.slotType_LockonEnabled);
                    MessageUI.Instance.SetMessage(TextManager.Get("MESSAGE_AIMOFF"), MessageUI.time_Default, MessageUI.panelType_Information, MessageUI.slotType_LockonDisabled);
                }
            } else {
                UISE.Instance.Play(UISE.SoundName.aimon);
                if (MessageUI.Instance) {
                    MessageUI.Instance.DestroyMessageSlotType(MessageUI.slotType_LockonDisabled);
                    MessageUI.Instance.SetMessage(TextManager.Get("MESSAGE_AIMON"), MessageUI.time_Default, MessageUI.panelType_Information, MessageUI.slotType_LockonEnabled);
                }
            }
        }
        if (CharacterManager.Instance.autoAim != 0) {
            base.Update_Targeting();
            if (target) {
                if (controlEnabled && playerInput.GetButtonDown(RewiredConsts.Action.Change_Target)) {
                    if (searchArea[0].isLocking) {
                        searchArea[0].ChangeTarget();
                        base.Update_Targeting();
                    }
                    searchArea[0].SetUnlockTimer(9999999);
                    UISE.Instance.Play(UISE.SoundName.move);
                }
            }
        } else {
            target = null;
            targetTrans = null;
            targetRadius = 0f;
        }
        if (target) {
            targetMissingCount = 0;
        } else {
            targetMissingCount++;
        }
        if (target != targetObjSave || targetRadius != targetRadiusSave) {
            targetObjSave = target;
            targetRadiusSave = targetRadius;
            if (target != null) {
                SearchTargetReference reference = target.GetComponent<SearchTargetReference>();
                if (reference != null && reference.referObj != null) {
                    CharacterManager.Instance.SetPlayerTarget(reference.referObj, reference.referRadius);
                } else {
                    CharacterManager.Instance.SetPlayerTarget(target, targetRadius);
                }
            } else {
                CharacterManager.Instance.SetPlayerTarget(target, targetRadius);
            }
        }
        if (target) {
            ActivateTargetGroupCamera(true);
        } else if (targetMissingCount >= 3) { 
            ActivateTargetGroupCamera(false);
        }
    }

    void ActivateTargetGroupCamera(bool flag) {
        if (CameraManager.Instance) {
            if (flag && GameManager.Instance.save.config[GameManager.Save.configID_CameraTargeting] == 2 || (GameManager.Instance.save.config[GameManager.Save.configID_CameraTargeting] == 1 && searchArea[0].isLocking)) {
                CameraManager.Instance.SetActiveTargetGroupCamera(true);
            } else {
                CameraManager.Instance.SetActiveTargetGroupCamera(false);
            }
        }
    }

    protected override void Update_Process_Jump() {
        base.Update_Process_Jump();
        if (FootstepManager.Instance) {
            FootstepManager.Instance.SetActionType(0);
            footstepJumpTimeRemain = 0.3f;
        }
        if (jumpShortenTimeRemain > 0f && move.y > 0f && !playerInput.GetButton(RewiredConsts.Action.Jump)) {
            move.y -= (GetJumpPower() * 0.8f - (isScrewJumping ? 2.94f : 4.34f));
            jumpShortenTimeRemain = 0f;
        }
        if (targetTrans && assistJumpingMoveTimeRemain > 0f) {
            if (inputMagMoment < 0.1f) {
                float sqrDist = GetTargetDistance(true, true, true);                
                if (sqrDist > 0.3f * 0.3f) {
                    CommonLockon();
                    cCon.Move(GetTargetVector(true, true, false) * GetMaxSpeed(false, false, false, true) * Mathf.Clamp01(0.5f + stateTime * 2f) * deltaTimeMove);
                }
            } else {
                assistJumpingMoveTimeRemain = 0f;
            }
        }
    }

    protected override void Update_Transition_Climb() {
        base.Update_Transition_Climb();
        if (stateTime > climbStiffTime) {
            SetMutekiTime(0.1f);
            gravityMultiplier = 1f;
            nowSpeed = 1f;
            anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.Climb], false);
            climbAnimFlag = false;
            climbTarget = null;
            climbTargetEnd = null;
            SetState(State.Wait);
        }
    }

    protected override void Update_Process_Climb() {
        base.Update_Process_Climb();
        nowSpeed = 0f;
        gravityMultiplier = 0f;
        trans.rotation = Quaternion.Slerp(trans.rotation, climbLookQuaternion, 5f * deltaTimeCache);
        if (climbTargetEnd && (climbTargetEnd.position - cConCenterPivot.position).sqrMagnitude < 0.2f * 0.2f) {
            ClimbChain();
        }
    }

    protected override void STControlChild_STConsume() {
        STConsumeChild_Drown();
        if (nowSpeed > GetMaxSpeed(true, false, false, true) && GetCanControl()) {
            nowST -= GetCost(CostType.Run) * (nowSpeed / costRunBaseSpeed) * deltaTimeMove * (groundedFlag ? 1f : 3f / 4f);
        }
        if (nowST < 0) {
            nowST = 0;
        }
    }

    public override bool GetIdling() {
        return (base.GetIdling() && landingTime <= 0f);
    }
    public override bool GetWalking() {
        return (base.GetWalking() && landingTime <= 0f);
    }
    protected override float GetSTHealRateChild_Normal() {
        float rate = 3f / 15f;
        float walkTemp = Mathf.Max(GetMaxSpeed(true, false, false, true), 1f);
        if (GetIdling() || state == State.Damage) {
            rate = 40f / 15f;
        } else if (GetWalking()) {
            rate = Mathf.Lerp(40f / 15f, 25f / 15f, nowSpeed / walkTemp);
        } else if (landingTime <= 0f) {
            rate = Mathf.Lerp(25f / 15f, 3f / 15f, (nowSpeed - walkTemp) / (walkTemp * 1.466667f));
        }
        return base.GetSTHealRateChild_Normal() * rate;
    }
    protected override float GetSTHealRateChild_Attack() {
        return base.GetSTHealRateChild_Attack() + (isSkillAttacking || isAnimStopped ? 0f : 0.03f);
    }
    protected override float GetSTHealRateChild_Jump() {        
        float rate = 0.03f;
        float speedTemp = Mathf.Abs(nowSpeed);
        float maxTemp = GetMaxSpeed(false, false, false, true);
        if (attackDetection.Length > screwDet && attackDetection[screwDet] && attackDetection[screwDet].attackEnabled) {
            rate = 0f;
        } else if (speedTemp > 0f && maxTemp > 0f) {
            // rate *= Mathf.Clamp01((maxTemp - speedTemp) / maxTemp);
            rate = Mathf.Lerp(0.03f, 0.0075f, Mathf.Clamp01(speedTemp / maxTemp));
        }
        return rate;
    }

    protected override void Update_Sick() {
        int nowHPSave = nowHP;
        base.Update_Sick();
        if (nowHP < nowHPSave) {
            int gutsBorder = CharacterManager.Instance.GetGutsBorder();
            if (nowHPSave >= gutsBorder && nowHP < gutsBorder) {
                CharacterManager.Instance.SetHPAlert();
            }
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (isJumpingAttack && state != State.Attack && state != State.Jump) {
            isJumpingAttack = false;
        }
        if (state == State.Attack && attackingMoveReservedTimer > 0f) {
            attackingMoveReservedTimer -= deltaTimeMove;
            if (attackingMoveReservedTimer <= 0f) {
                attackingMoveEnabled = true;
            }
        }
        if (attackingMoveEnabled && state != State.Attack) {
            attackingMoveEnabled = false;
            attackingMoveReservedTimer = 0f;
        }
        if (obstacleForGrounded) {
            bool groundedTemp = groundedFlag;
            if (obstacleDisableTimeRemain > 0f) {
                if (state == State.Attack || state == State.Jump) {
                    obstacleDisableTimeRemain -= deltaTimeMove;
                    groundedTemp = false;
                } else {
                    obstacleDisableTimeRemain = -1f;
                }
            }
            if (obstacleForGrounded.enabled != groundedTemp) {
                obstacleForGrounded.enabled = groundedTemp;
            }
        }
        if (wallBreaker) {
            if (!wallBreaker.activeSelf && isSuperman && groundedFlag && nowSpeed >= GetMaxSpeed() && CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.WallBreak) != 0) {
                wallBreaker.SetActive(true);
            } else if (wallBreaker.activeSelf) {
                wallBreaker.SetActive(false);
            }
        }
        if (wallBreakerSp) {
            if (!wallBreakerSp.activeSelf && groundedFlag && GetCanControl() && nowSpeed >= GetMaxSpeed()) {
                wallBreakerSp.SetActive(true);
            } else if (wallBreakerSp.activeSelf) {
                wallBreakerSp.SetActive(false);
            }
        }
        if (state != State.Attack && isAnimStopped) {
            isAnimStopped = false;
            anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.AttackSpeed], 1f);
        }
        if (_pileAttackCheckerEnabled && state != State.Attack) {
            PileAttackCheckerActivate(false);
        }
        if (CharacterManager.Instance && StageManager.Instance) {
            float referenceHeight = 0f;
            if (StageManager.Instance.dungeonController) {
                referenceHeight = StageManager.Instance.dungeonController.referenceHeight;
            }
            if (state == State.Climb && trans.position.y >= referenceHeight + 3.2f) {
                CharacterManager.Instance.isClimbing = true;
            } else if (state != State.Climb && trans.position.y < referenceHeight + 3.2f) {
                CharacterManager.Instance.isClimbing = false;
            }
            if (StageManager.Instance.mapActivateFlag == 0 && CharacterManager.Instance.isClimbing && CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.MapCamera) != 0) {
                StageManager.Instance.SetAllMapChipActivate(true);
            } else if (StageManager.Instance.mapActivateFlag == 1 && (!CharacterManager.Instance.isClimbing || CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.MapCamera) == 0)) {
                StageManager.Instance.DeactivateTemporalChip();
            }
        }
        if (state != State.Attack) {
            isSkillAttacking = false;
        }
        if (dodgeCancelEnabled && state != State.Dodge) {
            dodgeCancelEnabled = false;
        }
        if (t_JustDodgeCounterTimeRemain > 0f) {
            t_JustDodgeCounterTimeRemain -= deltaTimeCache;
        }
        if ((state == State.Wait && GetRunning()) || state == State.Dodge) {
            if (obstacleForGrounded.radius != 0.35f) {
                obstacleForGrounded.radius = 0.35f;
            }
        } else {
            if (obstacleForGrounded.radius != 0.25f) {
                obstacleForGrounded.radius = 0.25f;
            }
        }
        if (particleJustDodgeTimeRemain > 0f) {
            particleJustDodgeTimeRemain -= deltaTimeCache;
        }
        if (sandstarAutoHeal && state != State.Dead && !isSuperman && !IsSupermanCooltime()) {
            CharacterManager.Instance.AddSandstar(deltaTimeCache * 0.1666667f);
        }
        if (hyperJumpEffect.enabled && state != State.Jump && groundedFlag) {
            SetAnimationWing(0);
        }
        if (projectileNoticeTimeRemain > 0f) {
            if (projectileNoticeObj && projectileNoticeObj.activeSelf == false) {
                projectileNoticeObj.SetActive(true);
            }
            projectileNoticeTimeRemain -= deltaTimeCache;
        } else {
            if (projectileNoticeObj && projectileNoticeObj.activeSelf == true) {
                projectileNoticeObj.SetActive(false);
            }
        }
        if (!isItem && footstepDetectionObj.Length > 0 && footstepDetectionObj[0].activeSelf == rideTarget) {
            for (int i = 0; i < footstepDetectionObj.Length; i++) {
                footstepDetectionObj[i].SetActive(!rideTarget);
            }
        }
        if (fpHorizontalValue != 0f && rideTarget == null) {
            fpHorizontalValue = 0f;
        }
        if (quickJumpShortenReserved != 0 && state != State.Dodge) {
            quickJumpShortenReserved = 0;
        }
        if (assistDodgeCounter && assistDodgeCounter.activeSelf != (assistDodgeTimeRemain > 0f)) {
            assistDodgeCounter.SetActive(assistDodgeTimeRemain > 0f);
        }
        if (assistJumpingMoveTimeRemain > 0f && state != State.Attack && state != State.Jump) {
            assistJumpingMoveTimeRemain = 0f;
        }
    }

    protected override void Update_MoveControl() {
        base.Update_MoveControl();
        if (!GetCanMove()) {
            nowSpeed = 0f;
        }
        if (FootstepManager.Instance) {
            if (state != State.Jump && footstepJumpTimeRemain <= 0f) {
                FootstepManager.Instance.SetActionType(nowSpeed > GetMaxSpeed(true) ? 1 : 2);
            }
        }
        if (pointer) {
            ray.origin = trans.position;
            ray.direction = vecDown;
            if (GameManager.Instance.save.config[GameManager.Save.configID_LandingPoint] != 0 && state == State.Jump && Physics.Raycast(ray, out raycastHit, 30f, landingPointerLayer, QueryTriggerInteraction.Ignore)) {
                pointer.position = raycastHit.point + vecUp * 0.01f;
                if (!pointer.gameObject.activeSelf) {
                    pointer.gameObject.SetActive(true);
                }
            } else {
                if (pointer.gameObject.activeSelf) {
                    pointer.gameObject.SetActive(false);
                }
            }
        }
        if (state != State.Climb && !climbCC) {
            float stepOffsetTarget = defCCStepOffset;
            float slopeLimitTarget = defCCSlopeLimit;
            if (state == State.Attack && attackType == pileAttackIndex) {
                stepOffsetTarget = 0.6f;
                slopeLimitTarget = 60;
            }
            if (cCon.stepOffset != stepOffsetTarget) {
                cCon.stepOffset = Mathf.Min(cCon.height, stepOffsetTarget);
            }
            if (cCon.slopeLimit != slopeLimitTarget) {
                cCon.slopeLimit = slopeLimitTarget;
            }
        }
        if (state != State.Climb) {
            if (climbAnimFlag) {
                gravityMultiplier = 1f;
                anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.Climb], false);
                climbAnimFlag = false;
                climbTarget = null;
                climbTargetEnd = null;
            }
            if (climbCC) {
                cCon.radius += defCCRadius * deltaTimeCache * 4;
                cCon.height += defCCHeight * deltaTimeCache * 4;
                cCon.stepOffset = Mathf.Min(cCon.height, defCCStepOffset);
                if (cCon.radius >= defCCRadius) {
                    cCon.radius = defCCRadius;
                    cCon.height = defCCHeight;
                    cCon.stepOffset = defCCStepOffset;
                    climbCC = false;
                }
            }
        }
        if (fixMoveTimeRemain > 0f) {
            fixMoveTimeRemain -= deltaTimeCache;
            move = fixMoveVector;
        }
    }

    protected override void LateUpdate() {
        base.LateUpdate();
        if (CharacterManager.Instance) {
            if (searchArea[0].isLocking) {
                searchArea[0].ShowTarget(true, 1);
            } else {
                if (GameManager.Instance.save.config[GameManager.Save.configID_Target] != 0 && CharacterManager.Instance.autoAim != 0) {
                    searchArea[0].ShowTarget(true, 0);
                } else {
                    searchArea[0].ShowTarget(false);
                }
            }
        }
        if (PauseController.Instance && PauseController.Instance.IsPhotoPausing != photoSave) {
            photoSave = PauseController.Instance.IsPhotoPausing;
            for (int i = 0; i < photoExclude.Length; i++) {
                if (photoExclude[i]) {
                    photoExclude[i].SetActive(!photoSave);
                }
            }
        }
        checkPaused = (Time.timeScale == 0f);
    }
    
    public override bool CheckGrounded(float tolerance = 0.5f) {
        if (groundedFixTimeRemain > 0f) {
            return true;
        } else {
            bool answer = base.CheckGrounded(tolerance);
            if (!answer && move.y <= 0.1f) {
                if (Physics.BoxCast(trans.position + vecUp * (checkGroundPivotHeight), checkGroundBoxHalfExtents, vecDown, Quaternion.Euler(trans.TransformDirection(vecForward)), tolerance, checkGroundedLayerMask, QueryTriggerInteraction.Ignore)) {
                    answer = true;
                }
            }
            return answer;
        }
    }

    protected override void SetMultiHitBuff(bool flag) {
        if (flag) {
            for (int i = 0; i < 2; i++) {
                if (attackDetection[i] && (attackDetection[i].multiHitInterval == 0 || attackDetection[i].multiHitInterval > multiHitBuffInterval)) {
                    attackDetection[i].multiHitInterval = multiHitBuffInterval;
                }
            }
        } else {
            for (int i = 0; i < 2; i++) {
                if (attackDetection[i] && attackDetection[i].multiHitInterval != defaultMultiHitInterval[i]) {
                    attackDetection[i].multiHitInterval = defaultMultiHitInterval[i];
                }
            }
        }
        isMultiBuff = flag;
    }

    protected virtual void AttackBody() {
        float spRate = isSuperman ? 4f / 3f : 1;
        bool lockonEnabled = (CharacterManager.Instance.autoAim != 0 || searchArea[0].isLocking);
        if (!groundedFlag) {
            if ((CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Bolt) != 0 || (isSuperman && CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Judgement) != 0)) && (playerInput.GetButton(RewiredConsts.Action.Special) || forceBolt) && JudgeStamina(GetBoltCost())) {
                if (AttackBase(8, 1, 1, GetBoltCost(), 5f, 15f / 30f / spRate, 0.2f, 1, lockonEnabled, 20f)) {
                    S_ParticlePlay(0);
                    attackProcess = 1;
                    attackBiasValue = 0;
                    isSkillAttacking = true;
                    assistJumpingMoveTimeRemain = 0f;
                    obstacleDisableTimeRemain = 1f;
                    SuperarmorStart();
                    EmitEffect(effectBolt);
                    if (move.y > -0.25f) {
                        move.y = -0.25f;
                    }
                }
            } else {
                if (AttackBase(4, 1, 2, GetCost(CostType.Attack), 5f, 15f / 30f / spRate, 0.2f, 1, lockonEnabled, 20f)) {
                    S_ParticlePlay(0);
                    attackProcess = 1;
                    attackBiasValue = 0;
                    isSkillAttacking = false;
                    assistJumpingMoveTimeRemain = 0f;
                    obstacleDisableTimeRemain = 1f;
                    if (move.y > -0.25f) {
                        move.y = -0.25f;
                    }
                }
            }
        } else {
            if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Pile) != 0 && ((playerInput.GetButton(RewiredConsts.Action.Special) && axisInput.y > 0.5f) || forcePile) && JudgeStamina(GetPileCost())) {
                if (AttackBase(5, 1.5f, 1.5f, GetPileCost(), 15f / 30f / spRate, 15f / 30f / spRate, 0, spRate, lockonEnabled, 20f)) {
                    PerfectLockon();
                    S_ParticlePlay(0);
                    attackBiasValue = 0;
                    attackProcess = 1;
                    PileAttackCheckerActivate(true);
                    pileDuration = 0f;
                    isSkillAttacking = true;
                    EmitEffect(effectPile);
                }
            } else if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Wave) != 0 && (playerInput.GetButton(RewiredConsts.Action.Dodge) || (playerInput.GetButton(RewiredConsts.Action.Special) && axisInput.y < -0.5f) || forceWave) && JudgeStamina(GetWaveCost())) {
                spRate = isSuperman ? 4f / 3f : 10f / 9f;
                if (AttackBase(7, 1f, 1f, GetWaveCost(), 24f / 30f / spRate, 24f / 30f / spRate, 0, spRate, lockonEnabled, 24f)) {
                    S_ParticlePlay(0);
                    attackProcess = 1;
                    isSkillAttacking = true;
                    attackingMoveReservedTimer = 18f / 30f / spRate;
                }
            } else if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Spin) != 0 && (playerInput.GetButton(RewiredConsts.Action.Special) || forceSpin) && JudgeStamina(GetSpinCost())) {
                attackBiasValue = 0;
                const float spinSp = 3f / 4f;
                if (AttackBase(6, 1f, 1f, GetSpinCost(), 20f / 30f / spinSp / spRate, 20f / 30f / spinSp / spRate, 0.5f, spinSp * spRate, false)) {
                    S_ParticlePlay(0);
                    S_ParticlePlay(1);
                    attackProcess = 0;
                    isSkillAttacking = true;
                    SuperarmorStart();
                    EmitEffect(effectSpin);
                    attackingMoveReservedTimer = 18f / 30f / spRate;
                }
            } else if (attackProcess == 0) {
                if (AttackBase(0, 1, 1, GetCost(CostType.Attack), 10f / 30f / spRate, 10f / 30f / spRate, 1f, spRate, lockonEnabled)) {
                    S_ParticlePlay(0);
                    attackProcess = 1;
                    isSkillAttacking = false;
                    attackingMoveReservedTimer = 7f / 30f / spRate;
                    if (!playerInput.GetButton(RewiredConsts.Action.Dodge)) {
                        SpecialStep(0.4f, 0.25f / spRate, 1f, 0f, 0.4f, true, true, EasingType.SineInOut, true);
                    }
                }
            } else if (attackProcess == 1) {
                if (AttackBase(1, 1, 1, GetCost(CostType.Attack), 10f / 30f / spRate, 10f / 30f / spRate, 1f, spRate, lockonEnabled)) {
                    S_ParticlePlay(1);
                    attackProcess = (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Combo) != 0 ? 2 : 0);
                    isSkillAttacking = false;
                    attackingMoveReservedTimer = 7f / 30f / spRate;
                    if (!playerInput.GetButton(RewiredConsts.Action.Dodge)) {
                        SpecialStep(0.4f, 0.25f / spRate, 1f, 0f, 0.4f, true, true, EasingType.SineInOut, true);
                    }
                }
            } else if (attackProcess == 2) {
                if (AttackBase(2, 1, 0.75f, GetCost(CostType.Attack) * (7f / 6f), 12f / 30f / spRate, 12f / 30f / spRate, 1f, spRate, lockonEnabled)) {
                    S_ParticlePlay(0);
                    S_ParticlePlay(1);
                    attackProcess = 3;
                    isSkillAttacking = false;
                    attackingMoveReservedTimer = 7f / 30f / spRate;
                    if (!playerInput.GetButton(RewiredConsts.Action.Dodge)) {
                        SpecialStep(0.4f, 0.25f / spRate, 1f, 0f, 0.4f, true, true, EasingType.SineInOut, true);
                    }
                }
            } else if (attackProcess == 3) {
                if (AttackBase(3, 1f, 0.75f, GetCost(CostType.Attack) * (7f / 6f), 14f / 30f / spRate, 14f / 30f / spRate, 1f, spRate, lockonEnabled)) {
                    S_ParticlePlay(0);
                    S_ParticlePlay(1);
                    attackProcess = 0;
                    isSkillAttacking = false;
                    attackingMoveReservedTimer = 7f / 30f / spRate;
                    if (!playerInput.GetButton(RewiredConsts.Action.Dodge)) {
                        SpecialStep(0.4f, 0.25f / spRate, 1f, 0f, 0.4f, true, true, EasingType.SineInOut, true);
                    }
                }
            }
        }
        if (attackType == 3) {
            if (attackDetection[rightDet] && attackDetection[leftDet]) {
                attackDetection[rightDet].relationIndex = leftDet;
                attackDetection[leftDet].relationIndex = rightDet;
                bothHandAttacking = true;
                attackDetection[rightDet].offset = attackOffsetBothHand;
                attackDetection[leftDet].offset = attackOffsetNormal;
            }
        } else {
            if (attackDetection[rightDet] && attackDetection[leftDet]) {
                attackDetection[rightDet].relationIndex = -1;
                attackDetection[leftDet].relationIndex = -1;
                attackDetection[rightDet].offset = attackOffsetNormal;
                attackDetection[leftDet].offset = attackOffsetNormal;
            }
            bothHandAttacking = false;
        }
        attackTypeSave = attackType;
    }

    protected void BattleAssist() {
        forcePile = false;
        forceSpin = false;
        forceWave = false;
        forceBolt = false;
        forceScrew = false;
        if (assistTimeRemain > 0f && targetTrans) {
            int assistIndex = -1;
            float sqrDist = GetTargetDistance(true, true, true);
            bool canJump = !GetSick(SickType.Mud);
            if (groundedFlag) {
                if (attackProcess == 0 || (attackProcess == 1 && attackTypeSave != 0)) {
                    int assistMin = playerInput.GetButton(RewiredConsts.Action.Special) ? 1 : 0;
                    if (sqrDist >= 6f * 6f || (sqrDist >= 4f * 4f && Random.Range(0, 100) < 75)) {
                        int randTemp = Random.Range(0, 2);
                        if (randTemp == 0) {
                            assistIndex = 2;
                        } else {
                            assistIndex = 4;
                        }
                    } else if (heightDistConditionTime >= 0.4f && Random.Range(0, 100) < 75 && canJump) {
                        assistIndex = 1;
                    } else if (searchArea[0].IsBesieged(isSuperman ? 4f : 3f)) {
                        assistIndex = (Random.Range(0, 100) < 50 ? 1 : 3);
                    } else {
                        assistIndex = Random.Range(assistMin, 5);
                        if (assistIndex == assistIndexSave) {
                            assistIndex = Random.Range(assistMin, 5);
                        }
                        if (assistIndex == 1 && !canJump) {
                            assistIndex = Random.Range(assistMin, 4);
                            if (assistIndex >= 1) {
                                assistIndex += 1;
                            }
                        }
                    }

                    for (int i = 0; i < attackConditions.Length; i++) {
                        attackConditions[i] = true;
                    }
                    if (!JudgeStamina(GetCost(CostType.Jump) + GetCost(CostType.Run) + GetCost(CostType.Attack))) {
                        attackConditions[1] = false;
                    }
                    if (!JudgeStamina(GetPileCost()) || sqrDist <= 2f * 2f || CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Pile) == 0) {
                        attackConditions[2] = false;
                    }
                    if (!JudgeStamina(GetSpinCost()) || CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Spin) == 0) {
                        attackConditions[3] = false;
                    }
                    if (!JudgeStamina(GetWaveCost()) || (sqrDist < MyMath.Square(1.5f + waveAttackedTimeRemain * 0.5f) && Random.Range(0, 100) < 75) || CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Wave) == 0) {
                        attackConditions[4] = false;
                    }
                    for (int i = 0; i < 5 && attackConditions[assistIndex] == false; i++) {
                        assistIndex = Random.Range(assistMin, 5);
                    }
                    if (attackConditions[assistIndex] == false) {
                        assistIndex = 0;
                    }
                }

                assistIndexSave = assistIndex;

                if (assistIndex == 1) {
                    lastJumpButtonTime = 0f;
                    heightAdjustTimer = 0f;
                    if (inputMagMoment < 0.1f) {
                        assistJumpingMoveTimeRemain = 0.9f;
                    }
                    if (sqrDist < 1.2f * 1.2f && CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Screw) != 0) {
                        forceScrew = true;
                    }
                } else if (assistIndex == 2) {
                    forcePile = true;
                } else if (assistIndex == 3) {
                    forceSpin = true;
                } else if (assistIndex == 4) {
                    forceWave = true;
                }
            } else {
                if (JudgeStamina(GetBoltCost())) {
                    forceBolt = true;
                }
            }
        }
    }

    protected override void Attack() {
        base.Attack();
        attackingMoveEnabled = false;
        attackingMoveReservedTimer = 0f;
        isAnimStopped = false;
        isAnimEnd = false;
        isPlasma = (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Plasma) != 0);
        specialMoveDuration = 0f;

        BattleAssist();
        AttackBody();
        if (dodgeCancelEnabled && t_JustDodgeCounterTimeRemain > 0f && TrophyManager.Instance) {
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_JustDodgeCounter, true);
        }
    }

    protected float GetOverlookRate() {
        if (state == State.Jump) {
            if (GameManager.Instance.save.config[GameManager.Save.configID_Overlook] != 0 && targetTrans) {
                Vector3 tempPos = targetTrans.position;
                tempPos.y = trans.position.y;
                if ((tempPos - trans.position).sqrMagnitude < 400f) {
                    float dist = Vector3.Distance(tempPos, trans.position) - 10f;
                    if (dist < 0f) {
                        dist = 0f;
                    }
                    return Mathf.Lerp(1f, 0.25f, dist * 0.1f);
                }
            }
            return 0.25f;
        }
        return 0f;
    }

    void PileAttackEnd() {
        float spRate = isSuperman ? 4f / 3f : 1;
        attackStiffTime = Mathf.Max(stateTime, 0.1f) + (5f / 30f) / spRate;
        if (attackedTimeRemain < (7f / 30f) / spRate) {
            attackedTimeRemain = (7f / 30f) / spRate;
        }
        isAnimEnd = true;
        isAnimStopped = false;
        EmitEffect(effectPileEnd);
        PileAttackCheckerActivate(false);
    }

    protected override void ScrewStart() {
        AttackStart(screwDet);
    }

    protected override void ScrewEnd() {
        AttackEnd(screwDet);
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        float addValue = 0f;
        float spRate = isSuperman ? 4f / 3f : 1;
        switch (attackType) {
            case 0:
            case 1:
            case 2:
            case 3:
                gravityMultiplier = 1f;
                if (nowSpeed > GetMaxSpeed(true, false, false, true)) {
                    nowSpeed -= GetAcceleration() * deltaTimeMove;
                }
                break;
            case 4:
            case 8:
                gravityMultiplier = 3.8f;
                attackBiasValue = Mathf.Min(attackBiasValue, move.y);
                if (attackType == 4) {
                    addValue = Mathf.Max(0f, (attackBiasValue + 5f) * (-0.1f));
                    attackPowerMultiplier = 1.25f + addValue * (isSuperman ? 1.5f : 1f);
                    knockPowerMultiplier = 1.25f + addValue * 2f;
                    if (FootstepManager.Instance) {
                        FootstepManager.Instance.SetActionType(0);
                        footstepJumpTimeRemain = 0.4f;
                    }
                } else {
                    addValue = Mathf.Max(0f, (attackBiasValue + 8f) * (-0.1f));
                    attackPowerMultiplier = 1.25f + addValue * (isSuperman ? 1.5f : 1f);
                    knockPowerMultiplier = 1f + addValue * 2f;
                    if (FootstepManager.Instance) {
                        FootstepManager.Instance.SetActionType(0);
                        footstepJumpTimeRemain = 0.7f;
                    }
                }
                lockonRotSpeed = Mathf.Clamp(20f - stateTime * 50f, 8f, 20f);
                if ((stateTime >= 0.2f && groundedFlag) || stateTime > 3f || (stateTime >= 0.6f && Time.timeScale > 0f && trans.position.y >= jumpAttackPosSave)) {
                    float plusStiff = 4f / 30f;
                    float plusInterval = 4f / 30f;
                    if (attackType == 8) {
                        LightningBolt();
                        plusStiff = 12f / 30f;
                        plusInterval = 16f / 30f;
                    }
                    lockonRotSpeed = 8f;
                    attackType *= -1;
                    attackProcess = 1;
                    attackingMoveReservedTimer = plusStiff;
                    // attackStiffTime = stateTime + plusStiff;
                    attackStiffTime = stateTime + plusInterval;
                    obstacleDisableTimeRemain = plusStiff * 0.5f;
                    attackedTimeRemain = plusInterval;
                    nowSpeed = 0;
                    gravityMultiplier = 1;
                } else {
                    attackStiffTime = stateTime + 1;
                    obstacleDisableTimeRemain = 1f;
                }
                anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.AttackSpeed], (attackType != 4 && attackType != 8) || !isAnimStopped ? spRate : 0);
                if (Time.timeScale > 0f) {
                    jumpAttackPosSave = trans.position.y;
                }
                break;
            case pileAttackIndex: {
                    gravityMultiplier = 1f;
                    float hyperRate = (isHyper ? 1.5f : 1f);
                    float maxSpTemp = GetMaxSpeed(false, false, false, true) * hyperRate;
                    float pileMaxSpeed = maxSpTemp * 1.3f + 8f;
                    // float pileAttackAddValueMax = 1.6f + CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.SpeedRank) * 0.4f + (CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Speed) ? 1f : 0f) + (CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.Speed) != 0f ? 0.1f : 0f);

                    if (!isAnimEnd) {
                        if (stateTime > 0.9f) {
                            PileAttackEnd();
                        } else if (_pileAttackCheckerTrigger && _pileAttackCheckerEnabled && _pileAttackCheckerTrigger.stayFlag) {
                            PileAttackEnd();
                        } else if (nowSpeed > 0f && targetTrans && GetTargetDistance(true, true, false) < Mathf.Clamp(MyMath.Square(nowSpeed * 0.02f), 0.01f, 0.36f)) {
                            float targetHeight = GetTargetHeight(true);
                            if (targetHeight > 1.2f || targetHeight <= 0f) {
                                PileAttackEnd();
                            }
                        }
                    }
                    if (!isAnimEnd) {
                        if (nowSpeed < pileMaxSpeed * 0.4f) {
                            nowSpeed = pileMaxSpeed * 0.4f;
                        }
                        if (nowSpeed < pileMaxSpeed) {
                            nowSpeed += pileMaxSpeed * hyperRate * deltaTimeMove;
                            if (nowSpeed > pileMaxSpeed) {
                                nowSpeed = pileMaxSpeed;
                            }
                        }
                        // attackStiffTime = Mathf.Max(stateTime, 0.1f) + (5f / 30f) / spRate;
                        attackStiffTime = Mathf.Max(stateTime, 0.1f, deltaTimeMove) + (7f / 30f) / spRate;
                        if (attackedTimeRemain < (7f / 30f) / spRate) {
                            attackedTimeRemain = (7f / 30f) / spRate;
                        }
                        attackingMoveReservedTimer = (5f / 30f) / spRate;
                        attackProcess = 1;
                    } else {
                        nowSpeed = 0f;
                    }
                    ignoreMegatonCoin = true;
                    float maxSpPower = GetMaxSpeed(false, false, false, true) * hyperRate;
                    ignoreMegatonCoin = false;
                    attackBiasValue = Mathf.Clamp01(Mathf.Max(0f, pileDuration - (isHyper && isSuperman ? 0.1f : 0.2f)) * 2.5f);
                    attackPowerMultiplier = Mathf.Max(1.5f, 1.5f + (maxSpPower - 2.25f) / 4.5f * attackBiasValue);
                    knockPowerMultiplier = Mathf.Max(1.5f, 1.5f + (maxSpPower - 3.375f) * 4f / 9f * attackBiasValue);
                    if (!isAnimStopped) {
                        lockonRotSpeed = 20f * Mathf.Max(1f, pileMaxSpeed / 19.7f) * hyperRate * hyperRate;
                    } else if (!isAnimEnd) {
                        lockonRotSpeed = (20f - Mathf.Clamp(nowSpeed / pileMaxSpeed * 6f, 0f, 6f)) * Mathf.Max(1f, pileMaxSpeed / 19.7f) * hyperRate * hyperRate;
                    } else {
                        lockonRotSpeed = 0f;
                    }
                    if (!isAnimEnd) {
                        pileDuration += deltaTimeMove * hyperRate;
                    }
                    anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.AttackSpeed], isAnimEnd || !isAnimStopped ? spRate : 0f);
                }
                break;
        }
    }

    public override void HitAttackAdditiveProcess() {
        base.HitAttackAdditiveProcess();
        if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Antimatter) != 0) {
            for (int i = 0; i < 7; i++) {
                Instantiate(antimatterPrefab, antimatterPivot);
            }
        }
    }

    protected void PlayHyperEffect() {
        if (isHyper && isSuperman && hyperJumpEffect.enabled) {
            if (hyperJumpEffect.audioSource) {
                if (hyperJumpEffect.audioSource.isPlaying) {
                    hyperJumpEffect.audioSource.Stop();
                }
                hyperJumpEffect.audioSource.Play();
            }
            for (int i = 0; i < hyperJumpEffect.particles.Length; i++) {
                if (hyperJumpEffect.particles[i]) {
                    hyperJumpEffect.particles[i].Play();
                }
            }
        }
    }

    void SetAnimationWing(int index) {
        for (int i = 0; i < hyperJumpEffect.animationWings.Length; i++) {
            if (hyperJumpEffect.animationWings[i]) {
                hyperJumpEffect.animationWings[i].ChangeRotation(index);
            }
        }
    }

    protected virtual void PlayerJump() {
        nowST -= GetCost(CostType.Jump);
        lastJumpButtonTime = 100f;
        hyperJumpedTimeRemain = 0.4f;
        ReleaseAttackDetections();
        attackProcess = 0;
        attackType = -1;
        bothHandAttacking = false;
        anim.ResetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Landing]);
        anim.ResetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Jump]);
        if (rideTarget) {
            RemoveRide();
        }
        if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Screw) != 0 && (playerInput.GetButton(RewiredConsts.Action.Special) || forceScrew) && JudgeStamina(GetCost(CostType.Skill) * 0.625f)) {
            nowST -= GetCost(CostType.Skill) * 0.625f;
            anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.JumpType], jumpType = 2);
            attackPowerMultiplier = 1f;
            knockPowerMultiplier = 1f;
            isScrewJumping = true;
            if (hyperJumpEffect.enabled) {
                SetAnimationWing(1);
            }
        } else {
            if (attackDetection.Length > screwDet && attackDetection[screwDet] && attackDetection[screwDet].attackEnabled) {
                AttackEnd(screwDet);
            }
            if (nowSpeed > GetMaxSpeed(true)) {
                anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.JumpType], jumpType = 1);
            } else {
                anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.JumpType], jumpType = 0);
            }
            isScrewJumping = false;
            if (hyperJumpEffect.enabled) {
                SetAnimationWing(0);
            }
        }
        move.y = GetJumpPower() * (GetSick(SickType.Mud) ? 0.25f : 1);
        EmitEffect(effectJump);
        SetJustDodge(9f / 60f, CharacterManager.JustDodgeType.Jump);
        SetMutekiTime(11f / 60f);
        landingTime = landingTimeMax;
        jumpShortenTimeRemain = 0.1f;
        if (attackedTimeRemain > 0.01f) {
            attackedTimeRemain = 0.01f;
        }
        SetState(State.Jump);
        PlayHyperEffect();
    }

    public void Teleport() {
        GameObject[] teleportPoints = GameObject.FindGameObjectsWithTag("TeleportPoint");
        int answer = -1;
        if (teleportPoints.Length > 0) {
            float sqrDistMax = 0f;
            for (int i = 0; i < teleportPoints.Length; i++) {
                float sqrDistTemp = (teleportPoints[i].transform.position - trans.position).sqrMagnitude;
                if (sqrDistTemp > sqrDistMax) {
                    sqrDistMax = sqrDistTemp;
                    answer = i;
                }
            }
            if (answer >= 0) {
                Warp(teleportPoints[answer].transform.position + vecUp * 0.1f, 0f, 0f);
            }
        }
        if (answer < 0) {
            BlownAway(false, 0.1f, true, "Respawn");
        }
    }

    protected virtual bool GetCanWildRelease() {
        return !((disableControlTimeRemain > 0f && !rideTarget) || disableInputTimeRemain > 0f || state == State.Spawn || state == State.Dead);
    }

    protected override void Update_PlayerControl() {
        base.Update_PlayerControl();
        move.x = 0f;
        move.z = 0f;        
        bool saveClimbEnabled = climbEnabled;
        if (controlEnabled) {
            boosterFlag = false;
            axisInput.x = playerInput.GetAxis(RewiredConsts.Action.Horizontal);
            axisInput.y = playerInput.GetAxis(RewiredConsts.Action.Vertical);
        } else {
            axisInput.x = 0f;
            axisInput.y = 0f;
        }

        /*
        if (GameManager.Instance.save.moveStyle == 0) {
            CharacterMovement();
        } else {
            CharacterMovement_Classic();
        }
        */
        CharacterMovement();

        //Ram Attack
        if (Time.timeScale > 0f && attackDetection.Length > impactDet && attackDetection[impactDet]) {
            float conditionSpeed = impactAttackConditionSpeed * GameManager.Instance.megatonCoinSpeedMul;
            if (GetCanControl() && groundedFlag && (state == State.Wait || state == State.Chase || state == State.Escape)) {
                if (nowSpeed >= conditionSpeed - 0.01f) {
                    impactAttackDuration = Mathf.Clamp(impactAttackDuration + deltaTimeCache, 0f, 0.1f);
                } else if (nowSpeed < conditionSpeed - 0.5f && nowSpeed > walkSpeed) {
                    impactAttackDuration = Mathf.Clamp(impactAttackDuration - deltaTimeCache, 0f, 0.1f);
                } else if (nowSpeed <= walkSpeed) {
                    impactAttackDuration = 0f;
                }
            } else {
                impactAttackDuration = 0f;
            }
            if (impactAttackDuration > 0f) {
                float plusMul = Mathf.Clamp(nowSpeed / conditionSpeed, 1f, 10f);
                attackDetection[impactDet].attackRate = 1.5f * plusMul;
                attackDetection[impactDet].knockRate = 2.4f * plusMul;
                if (!attackDetection[impactDet].attackEnabled) {
                    AttackStart(impactDet);
                }
            } else {
                if (attackDetection[impactDet].attackEnabled) {
                    AttackEnd(impactDet);
                }
            }
        }

        if (!controlEnabled) {
            return;
        }

        //BattleAssist
        if (state != State.Jump) {
            heightAdjustTimer += deltaTimeCache;
            if (heightAdjustTimer > 10f) {
                heightAdjustTimer = 10f;
            }
        }
        if (targetTrans) {
            float heightDist = targetTrans.position.y - targetRadius - trans.position.y;
            if (heightDist >= (1.2f - heightAdjustTimer * 0.04f)) {
                heightDistConditionTime += deltaTimeCache;
            } else {
                heightDistConditionTime -= deltaTimeCache;
            }
            heightDistConditionTime = Mathf.Clamp01(heightDistConditionTime);
        } else {
            heightDistConditionTime = 0f;
        }
        if (waveAttackedTimeRemain > 0f) {
            waveAttackedTimeRemain = Mathf.Clamp(waveAttackedTimeRemain - deltaTimeCache, 0f, 10f);
        }
        if (state != State.Dead && !checkPaused && GameManager.Instance.save.difficulty <= GameManager.difficultyNT && GameManager.Instance.save.config[GameManager.Save.configID_BattleAssist] != 0 && playerInput.GetButtonDown(RewiredConsts.Action.Attack) && !playerInput.GetButton(RewiredConsts.Action.Special)) {
            assistTimeRemain = 0.2f;
            if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Dodge) != 0) {
                assistDodgeTimeRemain = 0.4f;
            }
            if (!isSuperman && CharacterManager.Instance.GetSandstarIsMax() && targetTrans && (CharacterManager.Instance.isBossBattle || CharacterManager.Instance.GetActionEnemyCount() >= 4)) {
                forceWildReleaseOn = true;
            } else if (isSuperman && !targetTrans && !CharacterManager.Instance.isBossBattle && CharacterManager.Instance.GetActionEnemyCount() <= 0) {
                forceWildReleaseOff = true;
            }
        }

        if (trans.eulerAngles.x != 0f || trans.eulerAngles.z != 0f) {
            eulerVec.y = trans.eulerAngles.y;
            trans.eulerAngles = eulerVec;
        }
        if (GetCanWildRelease()) {
            if (!isSuperman && CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.WildRelease) != 0 && (playerInput.GetButtonDown(RewiredConsts.Action.Wild_Release) || forceWildReleaseOn) && supermanTime >= 0.5f && CharacterManager.Instance.GetSandstar() >= 3) {
                CharacterManager.Instance.SupermanStart();
            } else if (isSuperman && (playerInput.GetButtonDown(RewiredConsts.Action.Wild_Release) || forceWildReleaseOff) && supermanTime >= 0.5f) {
                CharacterManager.Instance.AddSandstar(-1f, true);
                CharacterManager.Instance.ResetWR(true);
            }
        }
        forceWildReleaseOn = false;
        forceWildReleaseOff = false;
        if (isSuperman && state == State.Dead) {
            CharacterManager.Instance.ResetWR(true);
        }
        if (attackDetection.Length > screwDet && attackDetection[screwDet] && attackDetection[screwDet].attackEnabled) {
            if (attackedTimeRemain < 0) {
                attackedTimeRemain = 0;
            }
            if (state != State.Jump) {
                AttackEnd(screwDet);
            }
        }
        if (state != State.Jump && landingTime > 0) {
            landingTime -= deltaTimeMove;
        }

        climbEnabled = CheckClimbEnable();
        if (climbEnabled != saveClimbEnabled) {
            if (actionIcon.Length > actionClimbIndex && actionIcon[actionClimbIndex]) {
                actionIcon[actionClimbIndex].SetActive(climbEnabled);
            }
        }
        bool hyperJumpEnabled = (isHyper && isSuperman && hyperJumpEffect.enabled && hyperJumpedTimeRemain <= 0f);
        if (!checkPaused && playerInput.GetButtonDown(RewiredConsts.Action.Jump)) {
            if (climbEnabled && !playerInput.GetButtonDown(RewiredConsts.Action.Special)) {
                ClimbStart(true);
            } else if (!hyperJumpEnabled && GetCanControl_Input() && state == State.Jump && airJumpEnabled && !GetSick(SickType.Mud) && CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Jump) != 0 && JudgeStamina(GetCost(CostType.Jump))) {
                PlayerJump();
                if (GameManager.Instance.save.config[GameManager.Save.configID_ShowArms] > 0 && attackDetection[bootsLeftDet] && attackDetection[bootsRightDet]) {
                    AttackEnd(bootsLeftDet);
                    AttackEnd(bootsRightDet);
                    AttackStart(bootsLeftDet);
                    AttackStart(bootsRightDet);
                }
                airJumpEnabled = false;
            } else {
                lastJumpButtonTime = 0f;
            }
        }
        if (state != State.Jump && groundedFlag) {
            airJumpEnabled = true;
        }
        if (GetCanControl_Input() && state != State.Jump && groundedFlag && !GetSick(SickType.Mud)) {
            airJumpEnabled = true;
            if (landingTime <= landingTimeMax * 0.5f && lastJumpButtonTime < 0.15f) {
                if (GetCanQuick() && playerInput.GetButton(RewiredConsts.Action.Dodge) && JudgeStamina(GetCost(CostType.Quick))) {
                    QuickTurn();
                    lastJumpButtonTime = 100f;
                    attackProcess = 0;
                } else if (JudgeStamina(GetCost(CostType.Jump))) {
                    PlayerJump();
                }
            }
        }
        if (disableControlTimeRemain <= 0f && hyperJumpEnabled && hyperJumpedTimeRemain <= 0f && lastJumpButtonTime < 0.15f && JudgeStamina(GetCost(CostType.Jump))) {
            PlayerJump();
            airJumpEnabled = false;
        }

        lastJumpButtonTime += deltaTimeCache * (state == State.Attack ? 0.5f : state == State.Damage || state == State.Dodge ? 0.75f : 1f);

        if (!checkPaused && (playerInput.GetButtonDown(RewiredConsts.Action.Attack) || (GameManager.Instance.save.config[GameManager.Save.configID_SimplifySkillCommand] != 0 && playerInput.GetButtonDown(RewiredConsts.Action.Special)))) {
            lastAttackButtonTime = 0f;
        }
        if (GetCanControl_Input() && lastAttackButtonTime < 0.2f && attackedTimeRemain <= 0 && JudgeStamina(GetCost(CostType.Attack))) {
            if (state != State.Jump || stateTime > 0.2f) {
                lastAttackButtonTime = 100f;
                SetState(State.Attack);
            }
        }
        lastAttackButtonTime += deltaTimeCache;
        if (state != State.Attack && state != State.Climb && gravityMultiplier > 1) {
            gravityMultiplier = 1;
        }
        if (attackDetection[boosterDet] && !boosterFlag && attackDetection[boosterDet].attackEnabled) {
            AttackEnd(boosterDet);
        }
        if (CameraManager.Instance) {
            float olRate = GetOverlookRate();
            CameraManager.Instance.heightBias = olRate > 0f ? 40f * olRate : 0f;
            CameraManager.Instance.distanceBias = olRate > 0f && move.y > 0f ? Mathf.Min(0.5f, move.y * 0.1f) * olRate : 0f;
        }
        if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Gather) != 0 && ((playerInput.GetButton(RewiredConsts.Action.Dodge) && playerInput.GetButtonDown(RewiredConsts.Action.Special)) || (playerInput.GetButtonDown(RewiredConsts.Action.Dodge) && playerInput.GetButton(RewiredConsts.Action.Special)))) {
            CharacterManager.Instance.WarpToPlayerPosAll(true, true, true);
            CharacterManager.Instance.CheckTrophy_Gather_ForPlayer();
        }
    }

    bool LightningBolt() {
        if (isSuperman && CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Judgement) != 0 && playerInput.GetButton(RewiredConsts.Action.Special) && (playerInput.GetButton(RewiredConsts.Action.Attack) || GameManager.Instance.save.config[GameManager.Save.configID_SimplifySkillCommand] != 0)) {
            throwing.ThrowStart(judgementThrowIndex);
            CharacterManager.Instance.AddSandstar(judgementAttackSandstarCost, true);
            t_JudgementDefeatCount = 0;
            return true;
        } else {
            throwing.ThrowStart(boltThrowIndex);
            return false;
        }
    }

    protected virtual void ThrowWave() {
        if (!isItem) {
            if (isPlasma) {
                throwing.ThrowStart(wavePlasmaThrowIndex);
            } else {
                throwing.ThrowStart(waveNormalThrowIndex);
            }
        }
    }

    protected virtual void ThrowSpin() {
        if (!isItem) {
            if (isPlasma) {
                throwing.ThrowReady(spinPlasmaThrowIndex);
            } else {
                throwing.ThrowReady(spinNormalThrowIndex);
            }
        }
    }

    public override void SupermanEnd(bool effectEnable = true) {
        if (isSuperman) {
            supermanCoolTimeRemain = 0.6f;
        }
        base.SupermanEnd(effectEnable);
        CharacterManager.Instance.UpdateSandstarReady();
    }

    public override void AttackStart(int index) {
        if (bothHandAttacking && index == leftDet) {
            attackEffectEnabled = false;
        } else {
            attackEffectEnabled = true;
        }
        if (attackType == 3 && bothHandAttacking) {
            attackPowerMultiplier = bothHandAttackMul;
            knockPowerMultiplier = bothHandKnockMul;
        }
        if (state != State.Dodge && (state != State.Jump || (index != rightDet && index != leftDet))) {
            base.AttackStart(index);
        } else {
            AttackEnd(index);
        }
    }

    protected override void JudgeResurrection() {
        if (nowHP <= 0) {
            if (PauseController.Instance && PauseController.Instance.gameOverDisabled) {
                CharacterManager.Instance.PlayerDied(false);
            } else if (GameManager.Instance.save.NumOfSpecificItems(resurrectionId) > 0) {
                nowHP = GetMaxHP();
                SetMutekiTime(3f);
                CharacterManager.Instance.Heal(0, 100, (int)EffectDatabase.id.itemCrystal_Y, true, true, true);
                CharacterManager.Instance.ResetST();
                GameManager.Instance.save.RemoveItem(resurrectionId);
                CharacterManager.Instance.UseItemCountIncrement();
            } else if (CharacterManager.Instance.friendsEffectDisabled == 0 && CharacterManager.Instance.GetFriendsExist(resurrectionFriendsID, true)) {
                nowHP = GetMaxHP();
                SetMutekiTime(3f);
                CharacterManager.Instance.SacrificeOnPlayerResurrection();
                CharacterManager.Instance.Heal(9999, 100, (int)EffectDatabase.id.sacrifice, false, true, true);
                CharacterManager.Instance.ResetST();
            } else if (StageManager.Instance && StageManager.Instance.stageNumber == StageManager.infernoStageId) {
                CharacterManager.Instance.PlayerDied(false);
            } else { 
                CharacterManager.Instance.PlayerDied(true);
            }
        }
    }

    protected override void KnockLightProcess() {
        base.KnockLightProcess();
        GameManager.Instance.SetVibration(0.7f, 0.25f, true);
        if (!isSuperarmor && GetLightStiffTime() > 0) {
            disableDodgeTimeRemain = GetLightStiffTime() + 0.1f;
        }
    }

    protected override void KnockHeavyProcess() {
        base.KnockHeavyProcess();
        GameManager.Instance.SetVibration(1f, 0.45f, true);
        if (GetHeavyStiffTime() > 0) {
            disableDodgeTimeRemain = GetHeavyStiffTime() + 0.1f;
        }
    }

    protected override void DamageCommonProcess() {
        base.DamageCommonProcess();
        int gutsBorder = CharacterManager.Instance.GetGutsBorder();

        //ItemAutomaticUse
        if (GameManager.Instance.save.config[GameManager.Save.configID_ItemAutomaticUse] != 0 && nowHP >= 1 && nowHP < gutsBorder) {
            bool[] haveFlag = new bool[PauseController.itemHealIndex.Length];
            int itemID = -1;
            for (int i = 0; i < haveFlag.Length; i++) {
                haveFlag[i] = GameManager.Instance.save.HaveSpecificItem(PauseController.itemHealIndex[i]);
                if (haveFlag[i]) {
                    int healTemp = CharacterManager.Instance.PreCalcHealAmount(PauseController.itemHealAmounts[i], PauseController.itemHealPercents[i], true);
                    if (nowHP + healTemp >= gutsBorder) {
                        itemID = PauseController.itemHealIndex[i];
                        break;
                    }
                }
            }
            if (itemID == -1) {
                for (int i = 0; i < haveFlag.Length; i++) {
                    if (haveFlag[i]) {
                        itemID = PauseController.itemHealIndex[i];
                        break;
                    }
                }
            }
            if (itemID >= 0) {
                PauseController.Instance.ItemAutomaticUse(itemID);
            }
        }

        if (nowHP >= 1 && nowHP < gutsBorder) {
            CharacterManager.Instance.SetHPAlert();
        }
        justDodgeIntervalRemain = 0.4f;
        assistDodgeTimeRemain = 0f;
        CharacterManager.Instance.CheckTrophy_ResetCountByDamage();
    }

    protected override void Start_Process_Dead() {
        base.Start_Process_Dead();
        GameManager.Instance.SetVibration(1f, 0.8f, true);
    }

    protected bool ClimbStart(bool isStart) {
        bool findCheck = false;
        if (climbTarget) {
            if (isStart) {
                climbMother = climbTarget;
            }
            int index = -1;
            float distance = float.MaxValue;
            int childCount = climbTarget.childCount;
            if (childCount > 0) {
                for (int i = 0; i < childCount; i++) {
                    float distTemp = (climbTarget.GetChild(i).position - cConCenterPivot.position).sqrMagnitude;
                    if (distTemp < distance) {
                        distance = distTemp;
                        index = i;
                    }
                }
            }
            if (index >= 0) {
                climbTargetEnd = climbTarget.GetChild(index);
                findCheck = true;
            } else {
                ClimbChainTarget climbChainTarget = climbTarget.GetComponent<ClimbChainTarget>();
                if (climbChainTarget && climbChainTarget.target) {
                    climbTargetEnd = climbChainTarget.target;
                    findCheck = true;
                }
            }
            if (findCheck) {
                ClimbLookTarget climbLookHold = climbTarget.GetComponent<ClimbLookTarget>();
                Vector3 toPos = climbTarget.position;
                if (climbLookHold) {
                    toPos = climbLookHold.target.position;
                } else if (climbMother){
                    toPos = climbMother.position;
                }
                toPos.y = cConCenterPivot.position.y;
                climbLookQuaternion = Quaternion.LookRotation(toPos - cConCenterPivot.position);
                distance = Vector3.Distance(climbTargetEnd.position, cConCenterPivot.position);
                float time = distance * 0.25f + 0.1f;
                climbStiffTime = time + 0.05f;
                nowSpeed = 0f;
                move = vecZero;
                anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.Climb], true);
                climbAnimFlag = true;
                cCon.radius = 0.02f;
                cCon.height = 0.02f;
                cCon.stepOffset = 0.02f;
                climbCC = true;
                SetSpecialMove((climbTargetEnd.position - cConCenterPivot.position).normalized, distance * 1.2f, time, EasingType.Linear, ClimbChain);
                SetState(State.Climb);
            }
        }
        return findCheck;
    }

    protected void ClimbChain() {
        climbTarget = climbTargetEnd;
        if (!ClimbStart(false)) {
            specialMoveDuration = 0f;
        }
    }

    protected bool CheckClimbEnable() {
        if (GetCanControl_Input() && climbTarget && GameManager.Instance.save.config[GameManager.Save.configID_TreeClimbingAction] != 0) {
            if (Vector3.Angle((climbTarget.position - trans.position).normalized, trans.TransformDirection(vecForward)) <= 50f) {
                return true;
            } else {
                return false;
            }
        } else {
            return false;
        }
    }

    protected override bool GetCanControl() {
        if (state == State.Attack && attackingMoveEnabled && disableControlTimeRemain <= 0) {
            return true;
        } else {
            return base.GetCanControl();
        }
    }

    protected virtual bool GetCanControl_Input() {
        return (GetCanControl() || (dodgeCancelEnabled && state == State.Dodge && stateTime >= dodgeStiffTime * 0.5f) || rideTarget) && disableInputTimeRemain <= 0;
    }
    public override bool GetCanDodge() {
        return base.GetCanDodge() && disableControlTimeRemain <= 0 && groundedFlag && state != State.Jump && CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Dodge) != 0 && (state != State.Attack || attackingMoveEnabled) && disableDodgeTimeRemain <= 0f;
    }

    public virtual bool GetCanQuick() {
        return base.GetCanDodge() && CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Escape) != 0;
    }    

    public bool IsSupermanCooltime() {
        return (supermanCoolTimeRemain > 0f);
    }    

    public override float GetMaxSpeed(bool isWalk = false, bool ignoreSuperman = false, bool ignoreSick = false, bool ignoreConfig = false) {
        return base.GetMaxSpeed(isWalk, ignoreSuperman, ignoreSick, ignoreConfig) * (groundedFlag ? 1f : 0.9f);
    }

    public override float GetJumpPower() {
        return 8.3f + (CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.Jump) != 0 ? 0.8f : 0f);
    }

    public void ResetFootMaterial() {
        if (judgeFootMaterial) {
            judgeFootMaterial.ResetFixing();
        }
    }

    public void SetPlayerLightActive(bool flag, int type) {
        if (LightingDatabase.Instance) {
            if (flag) {
                if (playerLightInstance && type != lightTypeSave) {
                    Destroy(playerLightInstance);
                    playerLightInstance = null;
                }
                if (!playerLightInstance) {
                    playerLightInstance = Instantiate(LightingDatabase.Instance.playerLightPrefab[type], trans);
                    lightTypeSave = type;
                }
            } else {
                if (playerLightInstance) {
                    Destroy(playerLightInstance);
                    playerLightInstance = null;
                }
                lightTypeSave = -1;
            }
        }
    }

    public override void HealForJustDodge(float amount) {
        float maxSTTemp = GetMaxST();
        dodgeCancelEnabled = true;
        justDodgeIntervalRemain = justDodgeIntervalMax;
        if (state == State.Dodge) {
            t_JustDodgeCounterTimeRemain = 0.3f;
        }
        nowST += maxSTTemp * 0.5f * amount;
        mutekiTimeRemain += 0.5f * amount * (CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.Muteki) != 0 ? 1.2f : 1f);
        if (nowST > maxSTTemp) {
            nowST = maxSTTemp;
        }
    }

    void SetJustDodge(float timer, CharacterManager.JustDodgeType type) {
        if (GameManager.Instance.save.config[GameManager.Save.configID_DisableJustDodge] == 0) {
            dodgeCancelEnabled = false;
            if (justDodgeInstance != null) {
                Destroy(justDodgeInstance);
            }
            particleJustDodgeTimeRemain = timer;
            justDodgeInstance = Instantiate(justDodgePrefab, trans.position, quaIden);
            justDodgeInstance.GetComponent<AutoDestroy>().life = timer;
            JustDodgeTrigger jdTrigger = justDodgeInstance.GetComponent<JustDodgeTrigger>();
            jdTrigger.justDodgeType = type;
            jdTrigger.parentPlayer = this;
            t_NowJustDodgeType = type;
        }
    }

    public bool ReceiveParticleJustDodge(AttackDetection attacker = null) {
        if (particleJustDodgeTimeRemain > 0f) {
            particleJustDodgeTimeRemain = -1f;
            Instantiate(justDodgeEffectPrefab, trans.position, quaIden);
            CharacterManager.Instance.ShowJustDodge(t_NowJustDodgeType, attacker);
            return true;
        }
        return false;
    }

    protected override void ReportDamage(CharacterBase attacker, int damage) {
        base.ReportDamage(attacker, damage);
        CharacterManager.Instance.playerDamageSum += damage;
        CharacterManager.Instance.playerDamageCount++;
        if (GameManager.Instance.save.stageReport.Length >= GameManager.stageReportMax) {
            GameManager.Instance.save.stageReport[GameManager.stageReport_DamageSum] += damage;
            GameManager.Instance.save.stageReport[GameManager.stageReport_DamageCount]++;
        }
    }

    public override void SupermanStart(bool effectEnable = true) {
        base.SupermanStart(effectEnable);
        if (effectEnable) {
            PlayHyperEffect();
        }
    }

    public void Event_EscapeFromBigDog_01(float time, Vector3 pos, Quaternion rot) {
        ForceStopForEvent(time);
        cCon.enabled = false;
        gravityZeroTimeRemain = time;
        trans.SetPositionAndRotation(pos, rot);
        PauseController.Instance.pauseEnabled = false;
        CharacterManager.Instance.AddSandstar(-100, true);
    }

    public void Event_EscapeFromBigDog_02(Vector3 pos, Quaternion rot) {
        disableControlTimeRemain = 0.6f;
        gravityZeroTimeRemain = 0.6f;
        groundedFixTimeRemain = 0.6f;
        trans.SetPositionAndRotation(pos, rot);
        cCon.enabled = true;
        PauseController.Instance.pauseEnabled = true;
        isJumpingAttack = true;
        AttackBase(0, 1f, 1f, 0, 0.6f, 0.6f, 1f, 1f, false);
        SetSpecialMove(trans.TransformDirection(vecForward), 1f, 0.5f, EasingType.SineInOut);
        CharacterManager.Instance.AddSandstar(100, true);
    }

    public void Event_PlayerJumping(float height, float moveVelocity) {
        Vector3 posTemp = trans.position;
        posTemp.y += height;
        trans.position = posTemp;
        for (int i = 0; i < CharacterManager.Instance.friends.Length; i++) {
            if (CharacterManager.Instance.friends[i].trans) {
                posTemp = CharacterManager.Instance.friends[i].trans.position;
                posTemp.y += height;
                CharacterManager.Instance.friends[i].trans.position = posTemp;
            }
        }
        lastJumpButtonTime = 100f;
        anim.ResetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Landing]);
        anim.ResetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Jump]);
        if (attackDetection.Length > screwDet && attackDetection[screwDet] && attackDetection[screwDet].attackEnabled) {
            AttackEnd(screwDet);
        }
        anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.JumpType], jumpType = 0);
        isScrewJumping = false;
        landingTime = 9f / 60f;
        jumpShortenTimeRemain = 0.1f;
        SetState(State.Jump);
        gravityMultiplier = 1f;
        fixMoveTimeRemain = 0.5f;
        fixMoveVector = new Vector3(0f, moveVelocity, 0f);
    }

    public void SetAnotherAnimatorController(bool flag, RuntimeAnimatorController newAnimCon) {
        if (animConIsChanging != flag) {
            animConIsChanging = flag;
            if (flag) {
                if (newAnimCon != null) {
                    if (animConSave == null) {
                        animConSave = anim.runtimeAnimatorController;
                    }
                    anim.runtimeAnimatorController = newAnimCon;
                }
            } else {
                if (animConSave != null && anim.runtimeAnimatorController != animConSave) {
                    anim.runtimeAnimatorController = animConSave;
                    fixFaceTimeRemain = 0f;
                }
            }
        }
    }

    public void Event_LastBattleSecond() {
        AttackBase(0, 1f, 1f, 0, 1f, 1f, 1f, 1f, false);
    }

    public void SetAttackSpeedExternal(float param) {
        anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.AttackSpeed], param);
    }

    protected void QuickTurn() {
        SetJustDodge(15f / 60f, CharacterManager.JustDodgeType.QuickEscape);
        if (attackedTimeRemain > 0.01f) {
            attackedTimeRemain = 0.01f;
        }
        hyperJumpedTimeRemain = 0.4f;
        jumpShortenTimeRemain = 0.1f;
        quickJumpShortenReserved = 1;
        if (hyperJumpEffect.enabled) {
            SetAnimationWing(0);
        }
        float angle = 180f;
        anim.ResetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Landing]);
        SetState(State.Dodge);
        nowST -= GetCost(CostType.Quick);
        SetMutekiTime(0.5f);
        if (targetTrans) {
            Vector3 targetPos = targetTrans.position;
            targetPos.y = trans.position.y;
            Vector3 dir = (trans.position - targetPos).normalized;
            Vector3 forwardTemp = trans.TransformDirection(vecForward);
            angle = Vector3.Angle(forwardTemp, dir) * (Vector3.Cross(forwardTemp, dir).y < 0f ? -1f : 1f);
        }
        if (angle != 0f) {
            SetSpecialRotate(angle, 0.0008f * Mathf.Abs(angle), QuickJump);
        } else {
            QuickJump();
        }
    }

    protected void QuickJump() {
        landingTime = landingTimeMax;
        anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.JumpType], jumpType = 3);
        anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Jump]);
        anim.ResetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.QuickTurn]);
        anim.ResetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Landing]);
        SetState(State.Jump);
        quickJumping = true;
        isScrewJumping = false;
        nowSpeed = GetMaxSpeed();
        move.y = GetJumpPower();
        if ((jumpShortenTimeRemain > 0f || quickJumpShortenReserved == 2) && move.y > 0f && !playerInput.GetButton(RewiredConsts.Action.Jump)) {
            move.y -= (GetJumpPower() * 0.8f - 4.34f);
            jumpShortenTimeRemain = 0f;
        }
        quickJumpShortenReserved = 0;
        EmitEffect(effectQuick);
        if (hyperJumpEffect.enabled) {
            SetAnimationWing(0);
            PlayHyperEffect();
        }
    }

    /*
    protected void SpaceQuick() {
        SetJustDodge(12f / 60f, CharacterManager.JustDodgeType.QuickEscape);
        if (attackedTimeRemain > 0.01f) {
            attackedTimeRemain = 0.01f;
        }
        hyperJumpedTimeRemain = 0.4f;
        nowST -= GetCost(CostType.Quick);
        SetMutekiTime(dodgeMutekiTime);
        anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.JumpType], jumpType = 3);
        anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Jump]);
        anim.ResetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.QuickTurn]);
        anim.ResetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Landing]);
        SetState(State.Jump);
        quickJumping = true;
        nowSpeed = GetMaxSpeed();
        move.y = GetJumpPower();
        EmitEffect(effectQuick);
        if (hyperJumpEffect.enabled) {
            SetAnimationWing(0);
            PlayHyperEffect();
        }
    }
    */

    protected virtual void ThrowSlash() {
        if (isHyper && isSuperman) {
            EmitEffect(effectSlash);
            throwing.ThrowStart(slashRightThrowIndex);
            throwing.ThrowStart(slashLeftThrowIndex);
        }
    }

    public void SetFixMoveVector(Vector3 moveVector, float time) {
        fixMoveTimeRemain = time;
        fixMoveVector = moveVector;
    }

    protected void SpinMoveStart() { }

    protected void SpinMoveEnd() { }

    public void SetProjectileNotice() {
        projectileNoticeTimeRemain = 0.02f;
        if (projectileNoticeObj && projectileNoticeObj.activeSelf == false) {
            projectileNoticeObj.SetActive(true);
        }
    }

    protected override bool JudgeStamina(float staminaCost) {
        bool answer = base.JudgeStamina(staminaCost);
        if (!answer) {
            CharacterManager.Instance.SetStaminaAlert();
        }
        return answer;
    }

    public void CheckTrophy_Judgement() {
        t_JudgementDefeatCount++;
        if (t_JudgementDefeatCount >= 5) {
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_Judgement, true);
        }
    }

    public bool CheckTrophy_IsScrewAttacking() {
        return state == State.Jump && isScrewJumping;
    }

    public bool GetTalkEnabled() {
        return !rideTarget || ridingTime < 1f;
    }

    public Vector3 GetFirstPersonPosition() {
        if (rideTarget) {
            fpEyeHeight = Mathf.SmoothDamp(fpEyeHeight, (idleType == 3 ? 0.5f : 0.8f) / 1.2f * cCon.height, ref fpEyeHeightVel, 0.1f);
        } else {
            if (state == State.Damage && isDamageHeavy) {
                fpEyeHeight = Mathf.SmoothDamp(fpEyeHeight, Mathf.Min(0.9f * cCon.height, fpEyeRefer.position.y + 0.08f), ref fpEyeHeightVel, 0.1f);
            } else if (state == State.Attack && (attackType == 4 || attackType == -4)) {
                fpEyeHeight = Mathf.SmoothDamp(fpEyeHeight, 0.9f / 1.2f * cCon.height, ref fpEyeHeightVel, 0.1f);
            } else if (state == State.Attack && (attackType == 8 || attackType == -8)) {
                fpEyeHeight = Mathf.SmoothDamp(fpEyeHeight, Mathf.Min(0.9f * cCon.height, fpEyeRefer.position.y - 0.06f), ref fpEyeHeightVel, 0.1f);
            } else if (state == State.Jump && isScrewJumping) {
                fpEyeHeight = Mathf.SmoothDamp(fpEyeHeight, 0.8f, ref fpEyeHeightVel, 0.1f);
            } else if (state == State.Dodge) { 
                fpEyeHeight = Mathf.SmoothDamp(fpEyeHeight, fpEyeRefer.position.y, ref fpEyeHeightVel, 0.1f);
            } else { 
                fpEyeHeight = Mathf.SmoothDamp(fpEyeHeight, (0.9f - Mathf.Clamp((nowSpeed - 3f) * 0.015f, 0f, 0.15f)) * cCon.height, ref fpEyeHeightVel, 0.1f);
            }
        }
        if (state == State.Attack) {
            if (attackType >= 0 && attackType <= 3) {
                fpEyeForward = Mathf.SmoothDamp(fpEyeForward, 0.2f, ref fpEyeForwardVel, 0.15f);
            } else if (attackType == 4 || attackType == -4) {
                fpEyeForward = Mathf.SmoothDamp(fpEyeForward, 0.36f, ref fpEyeForwardVel, 0.15f);
            } else if (attackType == 8) {
                fpEyeForward = Mathf.SmoothDamp(fpEyeForward, 0.04f, ref fpEyeForwardVel, 0.15f);
            } else {
                fpEyeForward = Mathf.SmoothDamp(fpEyeForward, 0.08f, ref fpEyeForwardVel, 0.1f);
            }
        } else if (state == State.Damage) {
            Vector3 referPos = fpEyeRefer.position;
            Vector3 transPos = trans.position;
            referPos.y = transPos.y;
            if (Vector3.Angle(trans.TransformDirection(vecForward), referPos - transPos) > 90f) {
                fpEyeForward = Mathf.SmoothDamp(fpEyeForward, Vector3.Distance(referPos, transPos) * -1f + 0.08f, ref fpEyeForwardVel, 0.1f);
            } else {
                fpEyeForward = Mathf.SmoothDamp(fpEyeForward, 0.08f, ref fpEyeForwardVel, 0.1f);
            }
        } else if (state == State.Jump) {
            if (jumpType == 1 && stateTime < 0.4f) {
                fpEyeForward = Mathf.SmoothDamp(fpEyeForward, 0.36f, ref fpEyeForwardVel, 0.15f);
            } else if (jumpType == 2) {
                Vector3 referPos = fpEyeRefer.position;
                Vector3 transPos = trans.position;
                referPos.y = transPos.y;
                float angle = Vector3.Angle(trans.TransformDirection(vecForward), referPos - transPos);
                fpEyeForward = Mathf.SmoothDamp(fpEyeForward, Vector3.Distance(referPos, transPos) * (angle < 90f ? 1f : -1f) + 0.08f, ref fpEyeForwardVel, 0.1f);
            } else if (jumpType == 3) {
                fpEyeForward = Mathf.SmoothDamp(fpEyeForward, Mathf.Clamp(0.5f - stateTime * 0.4f, 0.1f, 0.3f), ref fpEyeForwardVel, 0.1f);
            } else { 
                fpEyeForward = Mathf.SmoothDamp(fpEyeForward, jumpType == 1 && stateTime < 0.4f ? 0.36f : 0.08f, ref fpEyeForwardVel, 0.15f);
            }
        } else if (state == State.Dodge) {
            Vector3 referPos = fpEyeRefer.position;
            Vector3 transPos = trans.position;
            referPos.y = transPos.y;
            float angle = Vector3.Angle(trans.TransformDirection(vecForward), referPos - transPos);
            fpEyeForward = Mathf.SmoothDamp(fpEyeForward, Vector3.Distance(referPos, transPos) * (angle < 90f ? 1f : -1f) + (dodgeDirType == 0 || dodgeDirType == 2 ? 0.12f : 0.08f), ref fpEyeForwardVel, 0.1f);
        } else { 
            fpEyeForward = Mathf.SmoothDamp(fpEyeForward, Mathf.Clamp(0.08f + (nowSpeed - 3f) * 0.012f, 0.08f, 0.2f), ref fpEyeForwardVel, 0.1f);
        }
        return trans.position + trans.TransformDirection(vecUp) * fpEyeHeight + trans.TransformDirection(vecForward) * fpEyeForward;
    }

    public Quaternion GetFirstPersonRotation() {
        Vector3 charaEuler = trans.eulerAngles;
        if (charaEuler.x > 180) {
            charaEuler.x -= 360f;
        }
        if ((state == State.Jump || (state == State.Attack && !groundedFlag)) && targetTrans && targetTrans.position.y < trans.position.y && GetTargetHeight(true) < 0f) {
            Vector3 lookEuler = Quaternion.LookRotation((targetTrans.position + targetRadius * vecUp) - (trans.position + cCon.height * 0.9f * vecUp)).eulerAngles;
            charaEuler.x = Mathf.Clamp(lookEuler.x * 0.9f, 0f, 80f);
        } else if (state == State.Damage) {
            Vector3 lookEuler = fpEyeRefer.eulerAngles;
            charaEuler.x = Mathf.Clamp(lookEuler.x >= 180f ? lookEuler.x - 360f : lookEuler.x, -80f, 80f);
        }
        charaEuler.y += fpHorizontalValue;
        charaEuler.x += fpVerticalValue;
        if (!rideTarget) {
            charaEuler.x = Mathf.Clamp(charaEuler.x, -80f, 80f);
        }
        Quaternion quaTemp = Quaternion.Euler(charaEuler);
        if (quaTemp != firstPersonRotationDamp) {
            firstPersonRotationDamp = Quaternion.RotateTowards(firstPersonRotationDamp, quaTemp, 360f * deltaTimeCache);
            firstPersonRotationDamp = Quaternion.Slerp(firstPersonRotationDamp, quaTemp, 4.5f * deltaTimeCache);
        }
        return firstPersonRotationDamp;
    }

    public void SetFirstPersonHV(float horizontal, float vertical) {
        if (rideTarget) {
            fpHorizontalValue += horizontal;
            if (fpHorizontalValue > 180f) {
                fpHorizontalValue -= 360f;
            } else if (fpHorizontalValue < -180f) {
                fpHorizontalValue += 360f;
            }
        } else {
            Vector3 eulerTemp = trans.localEulerAngles;
            eulerTemp.y += horizontal;
            trans.localEulerAngles = eulerTemp;
        }
        fpVerticalValue = Mathf.Clamp(fpVerticalValue + vertical, -80f, 80f);
    }

    public void ResetFirstPersonHV() {
        fpHorizontalValue = 0f;
        fpVerticalValue = 0f;
    }

    public override void SetSick(SickType sickType, float duration, AttackDetection attacker = null) {
        //ItemAutomaticUse
        if (GameManager.Instance.save.config[GameManager.Save.configID_ItemAutomaticUse] != 0) {
            if ((sickType == SickType.Poison || sickType == SickType.Acid || sickType == SickType.Slow) && !CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Antidote) && GameManager.Instance.save.HaveSpecificItem(PauseController.itemAntidoteID)) {
                PauseController.Instance.ItemAutomaticUse(PauseController.itemAntidoteID);
            }
        }
        base.SetSick(sickType, duration, attacker);
    }

    public override void CounterDodge(int dodgeDir, bool changeState = true) {
        if (!GetIsMuteki()) {
            forceDodge = true;
            forceDodgeDir = dodgeDir;
        }
    }

}
