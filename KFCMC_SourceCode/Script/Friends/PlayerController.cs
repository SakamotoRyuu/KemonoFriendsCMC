using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mebiustos.MMD4MecanimFaciem;
using Rewired;
using static ItemDatabase;

public class PlayerController : FriendsBase {

    [System.Serializable]
    public class JumpEffectSetting {
        public bool enabled;
        public AudioSource audioSource;
        public ParticleSystem[] particles;
        public AnimationWingVariation[] animationWings;
    }

    public float jumpPower = 8.3f;
    public Effect jumpEffect;
    public Effect quickEffect;
    public Transform lookAtTarget;
    public Transform pointer;
    public GameObject[] actionIcon;
    public GameObject grassDisplacer;
    public Transform cConCenterPivot;
    public GameObject wallBreaker;
    public GameObject wallBreakerSp;
    public JudgeFootMaterial judgeFootMaterial;
    public NavMeshAgent obstacleForGrounded;
    public GameObject justDodgePrefab;
    public GameObject justDodgeEffectPrefab;
    public Transform audioListener;
    public ChangeMatSet[] goldenMatSet;
    public GameObject[] photoExclude;
    public GameObject projectileNoticeObj;
    public Transform fpEyeRefer;
    public GameObject assistDodgeCounter;
    public bool sandstarAutoHeal;
    public bool skillPercentageEnabled;
    public bool isHyper;
    public GameObject sendDamageForContainerPrefab;
    public bool autoCalcComboAttackPoint;

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
    protected float coyoteTimeRemain;
    protected bool isAnimEnd = false;
    protected float landingTime = 0f;
    protected SphereCollider enemySphCol;
    protected CapsuleCollider enemyCapCol;
    protected bool climbEnabled = false;
    protected float jumpAttackPosSave;
    protected float jumpShortenTimeRemain;
    protected int quickJumpShortenReserved;
    protected float quickJumpedDisableDodgeTimeRemain; // クイックエスケープ直後にドッジステップが可能になる問題の対処
    protected float obstacleDisableTimeRemain;
    protected int displaceEnabled;
    protected float disableInputTimeRemain;
    protected bool boosterFlag;
    protected bool airJumpEnabled = true;
    protected Vector3 checkGroundBoxHalfExtents;
    protected float supermanCoolTimeRemain;
    protected float disableDodgeTimeRemain;
    protected Camera mainCamera;
    protected Transform camT;
    protected GameObject justDodgeInstance;
    protected float particleJustDodgeTimeRemain;
    protected float justDodgeAmountMultiplier = 1f;
    protected float groundedFixTimeRemain;
    protected bool attackingMoveEnabled;
    protected float attackingMoveReservedTimer;
    protected bool attackingDodgeEnabledForPlayer;
    protected float attackingDodgeReservedTimer;
    protected float projectileNoticeTimeRemain;
    
    protected float defCCRadius;
    protected float defCCHeight;
    protected float defCCStepOffset;
    protected float defCCSlopeLimit;
    protected bool climbCC;
    protected bool climbAnimFlag;
    protected LayerMask landingPointerLayer;

    protected const int actionClimbIndex = 0;

    protected const int resurrectionId = 57;

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
    protected const float landingTimeMax = 9f / 60f;
    protected float footstepJumpTimeRemain;
    protected bool dodgeCancelEnabled;
    protected bool dodgeInDodgeEnabled;
    protected bool dodgeCancelCounterAttack;
    protected bool setGravityOnDodge = true;
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
    protected const float justDodgeCounterTime = 0.275f;
    protected const float justDodgeIntervalMax = 0.5f;
    protected const float justDodgeIntervalDodgeInDodge = 0.25f;
    protected int justDodgeCounterAttackProcess = -1;

    //BattleAssist
    protected int assistIndexSave = -1;
    protected int attackTypeSave = -1;
    protected float assistTimeRemain;
    protected float assistDodgeTimeRemain;
    protected float assistJumpingMoveTimeRemain;
    protected float heightDistConditionTime;
    protected float heightAdjustTimer;
    protected bool forceWildReleaseOn;
    protected bool forceWildReleaseOff;
    protected bool forceDodge;
    protected int forceDodgeDir;

    //For Other Friends
    protected bool isAirAttacking;
    protected bool brakeOnAttacking = true;
    protected float hyperJumpedTimeRemain;
    protected GameObject hyperEffectInstance;
    protected bool skipHeavyKnockAnimationEnabled;
    protected float sandstarHealMultiplier = 1f;
    protected float sandstarHealMultiplierForAssist = 0.5f;
    static readonly Vector3 sendDamageForContainerPivot = new Vector3(0f, 0.65f, 0.3f);
    protected float sendDamageForContainerTimer;

    // Debug
    /*
    private double dbgSandstarStartTimeStamp;
    private double dbgSandstarEndTimeStamp;
    */

    protected const float JumpButtonReserveTime = 0.2f;
    protected const float AttackButtonReserveTime = 0.2f;
    protected const float CoyoteTime = 0.1f;

    // Combo Rank
    protected float comboRankAttackPoint;
    protected float comboRankCounterTimeRemain;
    protected int[] comboRankLastAttackTypes = new int[12];
    protected const int comboRankRedundantDefaultWeight = 4;

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
        superAttackRate = 1.5f;
        superKnockRate = 1.75f;
        sickHealSpeed = (IsHyper() || sandstarAutoHeal ? 2f : 1f);
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
        if (obstacleForGrounded) {
            obstacleForGrounded.updateUpAxis = false;
            obstacleForGrounded.updateRotation = false;
        }
        skipHeavyKnockAnimationEnabled = true;
        ResetComboRankRedundancy();
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
            CharacterManager.Instance.SetSkillPercentageEnabled(skillPercentageEnabled);
        }
        GetMainCamera();
    }

    public override void ResetGuts() {
        base.ResetGuts();
        if (CharacterManager.Instance) {
            gutsRemain = gutsMax = CharacterManager.Instance.GetDifficultyEffect(CharacterManager.DifficultyEffect.ServalGuts);
        }
    }

    public override void ResetStatus()
    {
        base.ResetStatus();
        if (justDodgeInstance != null)
        {
            Destroy(justDodgeInstance);
        }
        justDodgeIntervalRemain = 0.01f;
        fpVerticalValue = 0f;
        fpHorizontalValue = 0f;
        supermanCoolTimeRemain = 0f;
    }

    protected virtual void CheckBooster() { }
    
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
        UpdateTargetDirection();
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

        if (state == State.Dodge && dodgeCancelEnabled && inputMagMoment <= 0.7f)
        {
            dodgeInDodgeEnabled = true;
        }
        if (GetCanDodge()
            && (playerInput.GetButton(RewiredConsts.Action.Dodge) || forceDodge)
            && (inputMagMoment > 0.7f || forceDodge || (GameManager.Instance.save.config[GameManager.Save.configID_SimplifyDodgeCommand] != 0 && !(GetCanQuick() && landingTime <= landingTimeMax * 0.5f && lastJumpButtonTime < JumpButtonReserveTime)))
            && JudgeStamina(GetCost(CostType.Step))
            && quickJumpedDisableDodgeTimeRemain <= 0f) // クイックエスケープ直後でない
        {

            if (GetCanDodgeInDodge)
            {
                justDodgeIntervalRemain = 0f;
            }

            SetJustDodge(12f / 60f, CharacterManager.JustDodgeType.DodgeStep);
            if (forceDodge) {
                SideStep_ConsiderWall(forceDodgeDir, dodgeDistance, 0.3f);
            } else {
                Vector3 diff;
                if (inputMagMoment > 0.7f) {
                    diff = targetDirection.normalized;
                } else if (targetTrans != null) {
                    Vector3 diffTemp = GetEscapeDestination(targetTrans.position, dodgeDistance) - transform.position;
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
                SideStep_Vector(diff, dodgeDirType, dodgeDistance, 0.3f);
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

    protected virtual void OnDisable() {
        if (obstacleForGrounded) {
            obstacleForGrounded.enabled = false;
        }
    }

    public override void SetForItem() {
        base.SetForItem();
        if (audioListener) {
            audioListener.gameObject.SetActive(false);
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
        // dbg
        /*
        nowST = GetMaxST();
        if (Time.timeScale == 0)
        {
            CharacterManager.Instance.AddSandstar(-100);
            dbgSandstarStartTimeStamp = 0;
            dbgSandstarEndTimeStamp = 0;
        }
        if (dbgSandstarStartTimeStamp == 0 && CharacterManager.Instance.GetSandstar() > 0)
        {
            dbgSandstarStartTimeStamp = GameManager.Instance.time;
        }
        if (dbgSandstarEndTimeStamp == 0 && CharacterManager.Instance.GetSandstar() >= 5)
        {
            dbgSandstarEndTimeStamp = GameManager.Instance.time;
            Debug.Log(characterName + ":" + (dbgSandstarEndTimeStamp - dbgSandstarStartTimeStamp));
        }
        */
    }

    protected virtual void ChangeGoldenMaterial() {
        minmiGoldenSave = GameManager.Instance.minmiGolden;
        for (int i = 0; i < goldenMatSet.Length; i++) {
            SetForChangeMatSet(goldenMatSet[i], minmiGoldenSave);
        }
    }

    public virtual float GetStaminaBorder() {
        return Mathf.Max(
            GetCost(CostType.Attack) + (state == State.Jump ? 0f : GetCost(CostType.Jump)),
            GetCost(CostType.Skill)
            );
    }

    protected override GameObject SetMapChip()
    {
        GameObject mapChipObj = Instantiate(MapDatabase.Instance.prefab[MapDatabase.player], trans);
        mapChipControl = mapChipObj.GetComponent<MapChipControl>();
        return mapChipObj;
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
            if (assistDodgeTimeRemain > 0f && (state != State.Attack || attackingMoveEnabled || attackingDodgeEnabledForPlayer)) {
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
        if (hyperJumpedTimeRemain > 0f)
        {
            hyperJumpedTimeRemain -= deltaTimeCache;
        }
        if (sendDamageForContainerTimer > 0f)
        {
            if (state != State.Attack)
            {
                sendDamageForContainerTimer = 0f;
            }
            else
            {
                sendDamageForContainerTimer -= deltaTimeMove;
                if (sendDamageForContainerTimer <= 0f)
                {
                    InstantiateSendDamageForContainer();
                }
            }
        }
        // 地上にいる、かつジャンプ直後でないとき
        if (groundedFlag && landingTime <= 0)
        {
            // コヨーテタイムを再設定
            coyoteTimeRemain = CoyoteTime;
        }
        else if (coyoteTimeRemain > 0)
        {
            coyoteTimeRemain -= deltaTimeCache;
        }
        // クイックエスケープ直後にドッジステップが可能になる問題の対処
        if (quickJumpedDisableDodgeTimeRemain > 0f)
        {
            quickJumpedDisableDodgeTimeRemain -= deltaTimeCache;
        }
        // コンボランクのジャストドッジカウンターによる加点の残り時間
        if (comboRankCounterTimeRemain > 0f)
        {
            comboRankCounterTimeRemain -= deltaTimeCache;
        }
    }

    protected bool IsCoyote()
    {
        return groundedFlag || coyoteTimeRemain > 0;
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
            move.y -= (GetJumpPower() * 0.8f - (isScrewJumping ? jumpPower * 0.3542168f : jumpPower * 0.5228915f));
            jumpShortenTimeRemain = 0f;
        }
        if (targetTrans && assistJumpingMoveTimeRemain > 0f) {
            if (inputMagMoment < 0.1f) {
                float sqrDist = GetTargetDistance(true, true, true);                
                if (sqrDist > 0.3f * 0.3f) {
                    CommonLockon();
                    move += GetTargetVector(true, true, false) * GetMaxSpeed(false, false, false, true) * Mathf.Clamp01(0.5f + stateTime * 2f);
                    // cCon.Move(GetTargetVector(true, true, false) * GetMaxSpeed(false, false, false, true) * Mathf.Clamp01(0.5f + stateTime * 2f) * deltaTimeMove);
                }
            } else {
                assistJumpingMoveTimeRemain = 0f;
            }
        }
    }

    protected override void Update_Process_Attack()
    {
        base.Update_Process_Attack();
        if (brakeOnAttacking && nowSpeed != 0f)
        {
            nowSpeed = Mathf.Max(nowSpeed + GetAcceleration() * (-2f / 3f) * deltaTimeMove, 0f);
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
    protected override float GetSTHealRateChild_Jump() {        
        float rate = 0.03f;
        float speedTemp = Mathf.Abs(nowSpeed);
        float maxTemp = GetMaxSpeed(false, false, false, true);
        rate = Mathf.Lerp(0.03f, 0.0075f, Mathf.Clamp01(speedTemp / maxTemp));
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
        // AttackingMove
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
        // AttackingDodge
        if (state == State.Attack && attackingDodgeReservedTimer > 0f)
        {
            attackingDodgeReservedTimer -= deltaTimeMove;
            if (attackingDodgeReservedTimer <= 0f)
            {
                attackingDodgeEnabledForPlayer = true;
            }
        }
        if (attackingDodgeEnabledForPlayer && state != State.Attack)
        {
            attackingDodgeEnabledForPlayer = false;
            attackingDodgeReservedTimer = 0f;
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
            isAirAttacking = false;
        }
        if (state != State.Dodge)
        {
            dodgeCancelEnabled = false;
            dodgeInDodgeEnabled = false;
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
        // Combo Rank
        if (CharacterManager.Instance && (state == State.Damage || state == State.Dead))
        {
            CharacterManager.Instance.ResetComboPoint();
        }
        if (!CharacterManager.Instance.IsComboContinuing)
        {
            ResetComboRankRedundancy();
        }
    }

    protected virtual float GetDefCCStepOffset()
    {
        return defCCStepOffset;
    }

    protected virtual float GetDefCCSlopeLimit()
    {
        return defCCSlopeLimit;
    }

    protected override void Update_MoveControl() {
        if (fixMoveTimeRemain > 0f) {
            fixMoveTimeRemain -= deltaTimeCache;
            move = fixMoveVector;
        }
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
            float stepOffsetTarget = GetDefCCStepOffset();
            float slopeLimitTarget = GetDefCCSlopeLimit();
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

    protected virtual void BattleAssist() { }

    protected override void Attack() {
        base.Attack();
        dodgeCancelCounterAttack = dodgeCancelEnabled;
        attackingMoveEnabled = false;
        attackingMoveReservedTimer = 0f;
        attackingDodgeEnabledForPlayer = false;
        attackingDodgeReservedTimer = 0f;
        isAnimStopped = false;
        isAnimEnd = false;
        isPlasma = (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Plasma) != 0);
        specialMoveDuration = 0f;

        BattleAssist();
        AttackBody();
        if (dodgeCancelEnabled && t_JustDodgeCounterTimeRemain > 0f) {
            comboRankCounterTimeRemain = 1f;
            if (TrophyManager.Instance)
            {
                TrophyManager.Instance.CheckTrophy(TrophyManager.t_JustDodgeCounter, true);
            }
        }
        else
        {
            comboRankCounterTimeRemain = 0f;
        }
    }

    protected virtual void AttackBody()
    {

    }

    protected float GetOverlookRate() {
        if (state == State.Jump) {
            if (GameManager.Instance.save.config[GameManager.Save.configID_Overlook] != 0 && targetTrans) {                
                Vector3 transPos = trans.position;
                Vector3 tempPos = targetTrans.position;
                tempPos.y = transPos.y;
                if ((tempPos - transPos).sqrMagnitude < 400f) {
                    float dist = Vector3.Distance(tempPos, transPos) - 10f;
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

    protected virtual void SetJumpAnim()
    {
        if (nowSpeed > GetMaxSpeed(true))
        {
            anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.JumpType], jumpType = 1);
        }
        else
        {
            anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.JumpType], jumpType = 0);
        }
        isScrewJumping = false;
    }

    protected virtual void PlayerJump() {
        nowST -= GetCost(CostType.Jump);
        lastJumpButtonTime = 100f;
        ReleaseAttackDetections();
        attackProcess = 0;
        attackType = -1;
        bothHandAttacking = false;
        anim.ResetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Landing]);
        anim.ResetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Jump]);
        if (rideTarget) {
            RemoveRide();
        }
        SetJumpAnim();
        move.y = GetJumpPower() * (GetSick(SickType.Mud) ? 0.25f : 1);
        EmitEffectClass(jumpEffect);
        SetJustDodge(9f / 60f, CharacterManager.JustDodgeType.Jump);
        SetMutekiTime(11f / 60f);
        landingTime = landingTimeMax;
        coyoteTimeRemain = 0;
        jumpShortenTimeRemain = 0.1f;
        if (attackedTimeRemain > 0.01f) {
            attackedTimeRemain = 0.01f;
        }
        SetState(State.Jump);
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

    protected virtual bool IsHyperJumpEnabled()
    {
        return (isHyper && isSuperman && hyperJumpedTimeRemain <= 0f);
    }

    protected virtual void ImpactAttack() { }

    protected virtual void PlayerControl_BattleAssist() { }

    protected virtual void PlayerControl_CheckScrew() { }

    protected virtual void PlayerControl_CheckBooster() { }

    protected bool CheckJump_Grounded()
    {
        if (landingTime <= landingTimeMax * 0.5f &&
            lastJumpButtonTime < JumpButtonReserveTime &&
            GetCanControl_Input() &&
            IsCoyote() &&
            !GetSick(SickType.Mud))
        {
            if (GetCanQuick() && playerInput.GetButton(RewiredConsts.Action.Dodge) && JudgeStamina(GetCost(CostType.Quick)))
            {
                QuickTurn();
                lastJumpButtonTime = 100f;
                attackProcess = 0;
                return true;
            }
            else if (JudgeStamina(GetCost(CostType.Jump)))
            {
                PlayerJump();
                return true;
            }
        }
        return false;
    }

    protected bool CheckJump_Airborne()
    {
        if (!checkPaused &&
            lastJumpButtonTime < JumpButtonReserveTime &&
            GetCanControl_Input() &&
            (state == State.Jump || (state == State.Attack && attackingMoveEnabled)) &&
            airJumpEnabled &&
            !GetSick(SickType.Mud) &&
            CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Jump) != 0 &&
            JudgeStamina(GetCost(CostType.Jump))
            )
        {
            PlayerJump();
            SpaceJumpBoots();
            airJumpEnabled = false;
            return true;
        }
        return false;
    }

    protected override void Update_PlayerControl()
    {
        base.Update_PlayerControl();
        bool saveClimbEnabled = climbEnabled;
        if (controlEnabled)
        {
            boosterFlag = false;
            axisInput.x = playerInput.GetAxis(RewiredConsts.Action.Horizontal);
            axisInput.y = playerInput.GetAxis(RewiredConsts.Action.Vertical);
        }
        else
        {
            axisInput.x = 0f;
            axisInput.y = 0f;
        }

        CharacterMovement();
        ImpactAttack();

        if (!controlEnabled)
        {
            return;
        }

        PlayerControl_BattleAssist();

        if (trans.eulerAngles.x != 0f || trans.eulerAngles.z != 0f)
        {
            eulerVec.y = trans.eulerAngles.y;
            trans.eulerAngles = eulerVec;
        }
        if (GetCanWildRelease())
        {
            if (!isSuperman && CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.WildRelease) != 0 && (playerInput.GetButtonDown(RewiredConsts.Action.Wild_Release) || forceWildReleaseOn) && supermanTime >= 0.5f && CharacterManager.Instance.GetSandstar() >= 3)
            {
                CharacterManager.Instance.SupermanStart();
            }
            else if (isSuperman && (playerInput.GetButtonDown(RewiredConsts.Action.Wild_Release) || forceWildReleaseOff) && supermanTime >= 0.5f)
            {
                CharacterManager.Instance.AddSandstar(-1f, true);
                CharacterManager.Instance.ResetWR(true);
            }
        }
        forceWildReleaseOn = false;
        forceWildReleaseOff = false;
        if (isSuperman && state == State.Dead)
        {
            CharacterManager.Instance.ResetWR(true);
        }
        PlayerControl_CheckScrew();
        if (state != State.Jump && landingTime > 0)
        {
            landingTime -= deltaTimeMove;
        }

        climbEnabled = CheckClimbEnable();
        if (climbEnabled != saveClimbEnabled)
        {
            if (actionIcon.Length > actionClimbIndex && actionIcon[actionClimbIndex])
            {
                actionIcon[actionClimbIndex].SetActive(climbEnabled);
            }
        }
        bool hyperJumpEnabled = IsHyperJumpEnabled();
        if (!checkPaused && playerInput.GetButtonDown(RewiredConsts.Action.Jump))
        {
            if (climbEnabled && !playerInput.GetButtonDown(RewiredConsts.Action.Special))
            {
                ClimbStart(true);
            }
            else
            {
                lastJumpButtonTime = 0f;
            }
        }

        // 地上ジャンプ
        if (!CheckJump_Grounded() &&
            !hyperJumpEnabled)
        {
            // 空中ジャンプ
            CheckJump_Airborne();
        }

        // 空中ジャンプ可能フラグを立てるかチェック
        if (state != State.Jump && IsCoyote())
        {
            airJumpEnabled = true;
        }

        // ハイパージャンプ
        if (disableControlTimeRemain <= 0f && hyperJumpEnabled && lastJumpButtonTime < JumpButtonReserveTime && JudgeStamina(GetCost(CostType.Jump)))
        {
            PlayerJump();
            airJumpEnabled = false;
        }

        lastJumpButtonTime += deltaTimeCache * (state == State.Attack ? 0.5f : state == State.Damage || state == State.Dodge ? 0.75f : 1f);

        if (!checkPaused && (playerInput.GetButtonDown(RewiredConsts.Action.Attack) || (GameManager.Instance.save.config[GameManager.Save.configID_SimplifySkillCommand] != 0 && playerInput.GetButtonDown(RewiredConsts.Action.Special))))
        {
            lastAttackButtonTime = 0f;
        }
        if (GetCanControl_Input() && lastAttackButtonTime < AttackButtonReserveTime && attackedTimeRemain <= 0 && JudgeStamina(GetCost(CostType.Attack)))
        {
            if (state != State.Jump || stateTime > 0.2f)
            {
                lastAttackButtonTime = 100f;
                SetState(State.Attack);
            }
        }
        lastAttackButtonTime += deltaTimeCache;
        if (state != State.Attack && state != State.Climb && gravityMultiplier > 1)
        {
            gravityMultiplier = 1;
        }
        PlayerControl_CheckBooster();
        if (CameraManager.Instance)
        {
            float olRate = GetOverlookRate();
            CameraManager.Instance.heightBias = olRate > 0f ? 40f * olRate : 0f;
            CameraManager.Instance.distanceBias = olRate > 0f && move.y > 0f ? Mathf.Min(0.5f, move.y * 0.1f) * olRate : 0f;
        }
        if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Gather) != 0 && ((playerInput.GetButton(RewiredConsts.Action.Dodge) && playerInput.GetButtonDown(RewiredConsts.Action.Special)) || (playerInput.GetButtonDown(RewiredConsts.Action.Dodge) && playerInput.GetButton(RewiredConsts.Action.Special))))
        {
            CharacterManager.Instance.WarpToPlayerPosAll(true, true, true);
            CharacterManager.Instance.CheckTrophy_Gather_ForPlayer();
        }
    }

    protected virtual void SpaceJumpBoots() { }

    protected virtual bool LightningBolt() {
        return false;
    }

    protected virtual void ThrowWave() { }

    protected virtual void ThrowSpin() { }

    public override void SupermanEnd(bool effectEnable = true) {
        if (isSuperman) {
            supermanCoolTimeRemain = 0.6f;
        }
        base.SupermanEnd(effectEnable);
        CharacterManager.Instance.UpdateSandstarReady();
    }

    protected virtual bool IsAttackStartEnabled(int index)
    {
        return state != State.Dodge && state != State.Jump;
    }

    public override void AttackStart(int index) {
        if (IsAttackStartEnabled(index)) {
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

    protected override void Start_Process_Dodge()
    {
        base.Start_Process_Dodge();
        // ドッジステップ時、Y軸速度を地上扱いにリセット
        if (setGravityOnDodge)
        {
            move.y = Physics.gravity.y;
        }
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
        return (GetCanControl() || (dodgeCancelEnabled && state == State.Dodge && stateTime >= justDodgeCounterTime) || rideTarget) && disableInputTimeRemain <= 0;
    }
    public override bool GetCanDodge() {
        return (base.GetCanDodge() || GetCanDodgeInDodge) && disableControlTimeRemain <= 0 && IsCoyote() && CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Dodge) != 0 && (state != State.Attack || attackingMoveEnabled || attackingDodgeEnabledForPlayer) && disableDodgeTimeRemain <= 0f;
    }
    protected bool GetCanDodgeInDodge
    {
        get
        {
            return dodgeInDodgeEnabled && state == State.Dodge && stateTime >= justDodgeCounterTime;
        }
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
        return jumpPower + (CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.Jump) != 0 ? 0.8f : 0f);
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
            t_JustDodgeCounterTimeRemain = 0.5f - stateTime;
        }
        nowST += maxSTTemp * 0.5f * amount;
        mutekiTimeRemain += 0.5f * amount * (CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.Muteki) != 0 ? 1.2f : 1f);
        if (nowST > maxSTTemp) {
            nowST = maxSTTemp;
        }
        if (justDodgeCounterAttackProcess >= 0)
        {
            attackProcess = justDodgeCounterAttackProcess;
            attackedTimeRemain = -attackProcessResetTime + (0.5f - stateTime);
        }
        // Combo Rank
        CharacterManager.Instance.AddComboPoint(0);
        ResetComboRankRedundancy();
    }

    void SetJustDodge(float timer, CharacterManager.JustDodgeType type)
    {
        dodgeCancelEnabled = false;
        dodgeInDodgeEnabled = false;
        if (GameManager.Instance.save.config[GameManager.Save.configID_DisableJustDodge] == 0) {
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

    public virtual void Event_PlayerJumping(float height, float moveVelocity) {
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
        anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.JumpType], jumpType = 0);
        isScrewJumping = false;
        landingTime = 9f / 60f;
        jumpShortenTimeRemain = 0.1f;
        SetState(State.Jump);
        gravityMultiplier = 1f;
        fixMoveTimeRemain = 0.5f;
        fixMoveVector = new Vector3(0f, moveVelocity, 0f);
    }

    public virtual void Event_SetHyperJumpEffectVolume(float volume) { }

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

    public virtual bool IsHyper()
    {
        return isHyper;
    }

    protected virtual void QuickTurn()
    {
        SetJustDodge(15f / 60f, CharacterManager.JustDodgeType.QuickEscape);
        if (attackedTimeRemain > 0.01f)
        {
            attackedTimeRemain = 0.01f;
        }
        jumpShortenTimeRemain = 0.1f;
        quickJumpShortenReserved = 1;
        float angle = 180f;
        anim.ResetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Landing]);
        SetState(State.Dodge);
        nowST -= GetCost(CostType.Quick);
        SetMutekiTime(0.5f);
        if (targetTrans)
        {
            Vector3 targetPos = targetTrans.position;
            targetPos.y = trans.position.y;
            Vector3 dir = (trans.position - targetPos).normalized;
            Vector3 forwardTemp = trans.TransformDirection(vecForward);
            angle = Vector3.Angle(forwardTemp, dir) * (Vector3.Cross(forwardTemp, dir).y < 0f ? -1f : 1f);
        }
        //if (angle != 0f)
        //{
        //    SetSpecialRotate(angle, 0.0008f * Mathf.Abs(angle), QuickJump);
        //}
        //else
        //{
        //    QuickJump();
        //}
        SetSpecialRotate(angle, 0.075f, QuickJump);
    }

    protected virtual void QuickJump() {
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
        quickJumpedDisableDodgeTimeRemain = 0.1f;
        EmitEffectClass(quickEffect);
    }

    protected virtual void ThrowSlash() { }

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

    public virtual void Event_SetHyperJumpEffectEnabled(bool enabled) { }

    protected virtual void AttackStartAir()
    {
        if (!groundedFlag)
        {
            assistJumpingMoveTimeRemain = 0f;
            obstacleDisableTimeRemain = 1f;
            if (move.y > 0f)
            {
                move.y *= 0.4f;
            }
        }
        isAirAttacking = !groundedFlag;
    }

    protected virtual void AttackContinuousAir(float gravity = 2f)
    {
        if (isAirAttacking)
        {
            gravityMultiplier = gravity;
            if (FootstepManager.Instance)
            {
                FootstepManager.Instance.SetActionType(0);
                footstepJumpTimeRemain = 0.4f;
            }
            lockonRotSpeed = Mathf.Clamp(20f - stateTime * 50f, 10f, 20f);
            obstacleDisableTimeRemain = 1f;
            if ((stateTime >= 0.2f && groundedFlag) || stateTime > 3f || (stateTime >= 0.6f && Time.timeScale > 0f && trans.position.y >= jumpAttackPosSave))
            {
                lockonRotSpeed = 8f;
                obstacleDisableTimeRemain = GetAttackStiffRemain * 0.5f;
                nowSpeed = 0;
                gravityMultiplier = 1;
                isAirAttacking = false;
            }
            if (Time.timeScale > 0f)
            {
                jumpAttackPosSave = trans.position.y;
            }
        }
    }

    public float GetJustDodgeAmountMultiplier => justDodgeAmountMultiplier;

    public virtual void SetHyper(bool activate = true)
    {
        if (isHyper != activate)
        {
            isHyper = activate;
            sandstarAutoHeal = activate;
            if (activate)
            {
                superAttackRate *= 2;
                superKnockRate *= 2;
                maxHP *= 2;
                maxST *= 2;
                knockEndurance *= 2;
                knockRecovery *= 2;
                knockPower *= 2;
                attackPower *= 2;
                if (!hyperEffectInstance)
                {
                    hyperEffectInstance = CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.friendsHyper, -1, true, false, false);
                }
            }
            else
            {
                superAttackRate /= 2;
                superKnockRate /= 2;
                maxHP /= 2;
                maxST /= 2;
                knockEndurance /= 2;
                knockRecovery /= 2;
                knockPower /= 2;
                attackPower /= 2;
                if (hyperEffectInstance)
                {
                    Destroy(hyperEffectInstance);
                }
            }
            nowHP = GetMaxHP();
            nowST = GetMaxST();
        }
    }

    public float GetEyeHeight { 
        get
        {
            return fpEyeRefer ? fpEyeRefer.position.y - trans.position.y : 0f;
        }
    }

    protected virtual void PlayHyperEffect() { }

    protected override void SkipHeavyKnockAnimation()
    {
        if (skipHeavyKnockAnimationEnabled && state == State.Damage && isDamageHeavy)
        {
            AnimatorStateInfo animState = anim.GetCurrentAnimatorStateInfo(0);
            anim.Play(0, 0, animState.normalizedTime + (30f / 107f));
        }
    }

    public float GetSandstarHealMultiplierForAttack { get { return sandstarHealMultiplier; } }

    public float GetSandstarHealMultiplierForAssist { get { return sandstarHealMultiplierForAssist; } }

    protected void InstantiateSendDamageForContainer()
    {
        if (sendDamageForContainerPrefab)
        {
            Instantiate(sendDamageForContainerPrefab, trans.position + trans.TransformVector(sendDamageForContainerPivot), quaIden);
        }
    }

    protected void ReserveSendDamageForContainer(float timer)
    {
        if (sendDamageForContainerPrefab)
        {
            sendDamageForContainerTimer = timer;
        }
    }

    public override void AddComboPoint(int colorType)
    {
        float point = comboRankAttackPoint;
        if (colorType == damageColor_Hard || colorType == damageColor_HardBack)
        {
            point *= 0.25f;
        }
        if (comboRankCounterTimeRemain > 0)
        {
            point += 20;
            comboRankCounterTimeRemain = 0;
        }
        comboRankAttackPoint = 0;
        int itemID = CharacterManager.Instance.AddComboPoint(point);
        if (itemID >= 0)
        {
            ItemDatabase.Instance.GiveItem(itemID, trans.position + Vector3.up * faceHeight, 5, -1, -1, -1, StageManager.Instance.dungeonController.itemSettings.container, 0);
            CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.comboHeal, -1, false, true);
        }
    }

    protected override void SetComboRankAttackPoint(int type, float point, int weight = comboRankRedundantDefaultWeight)
    {
        int redundantCount = 0;
        for (int i = 0; i < comboRankLastAttackTypes.Length; i++)
        {
            if (type == comboRankLastAttackTypes[i])
            {
                redundantCount++;
            }
        }
        if (redundantCount > 0)
        {
            point = point * 4f / (4f + redundantCount);
        }
        for (int i = comboRankLastAttackTypes.Length - 1; i >= weight; i--)
        {
            comboRankLastAttackTypes[i] = comboRankLastAttackTypes[i - weight];
        }
        for (int i = comboRankLastAttackTypes.Length - 1 - weight; i >= 0; i--)
        {
            comboRankLastAttackTypes[i] = type;
        }
        comboRankAttackPoint = point;
    }

    protected void ResetComboRankRedundancy()
    {
        for (int i = 0; i < comboRankLastAttackTypes.Length; i++)
        {
            comboRankLastAttackTypes[i] = -1;
        }
    }

    protected override void ConsumeStaminaForAttack(int type, float cost)
    {
        base.ConsumeStaminaForAttack(type, cost);
        if (autoCalcComboAttackPoint)
        {
            float point = cost / (1f + CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.CostAttack));
            SetComboRankAttackPoint(type, point, comboRankRedundantDefaultWeight);
        }
    }

}
