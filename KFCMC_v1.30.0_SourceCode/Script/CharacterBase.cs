using UnityEngine;
using UnityEngine.AI;
using Mebiustos.MMD4MecanimFaciem;

public class CharacterBase : MonoBehaviour {

    [System.Serializable]
    public class Effect {
        public GameObject prefab;
        public Transform pivot;
        public Vector3 offset;
        public bool parenting;
        public GameObject instance;
    }

    [System.Serializable]
    public class ChangeMatSet {
        public Renderer changeMatRenderer;
        public int materialIndex;
        public Material defaultMaterial;
        public Material specialMaterial;
        public bool bringRenderQueue = true;
    }

    [System.Serializable]
    public class SupermanSettings {
        public GameObject[] obj;
        public ChangeMatSet[] mats;
        [System.NonSerialized]
        public bool isSpecial;
    }

    [System.Serializable]
    public class AgentActionDistance {
        public Vector2 attack;
        public Vector2 chase;
        public Vector2 escape;
    }

    public enum Command {
        Default, Evade, Ignore, Free
    };

    public enum CostType {
        Run, Attack, Step, Quick, Jump, Skill
    };

    public enum SickType {
        Mud, Fire, Poison, Ice, Acid, Slow, Stop, Frightened
    };

    public string characterName;
    public int maxHP = 100;
    public float maxST = 100;
    public float knockEndurance = 100;
    public float knockEnduranceLight = 30;
    public float knockRecovery = 25;
    public float knockPower = 50;
    public float attackPower = 10;
    public float defensePower = 10;
    public float dodgePower = 8f;
    public AttackDetection[] attackDetection;
    public SearchArea[] searchArea;
    public GameObject[] searchTarget;

    public float maxSpeed = 9;
    public float walkSpeed = 3;
    public float acceleration = 18f;
    public float angularSpeed = 180;
    public AgentActionDistance[] agentActionDistance;
    public Transform centerPivot;

    public float lightKnockTime = 0.5f;
    public float lightKnockMove = 1;
    public float lightStiffTime = 0.5f;
    public float heavyKnockTime = 0.5f;
    public float heavyKnockMove = 2;
    public float heavyStiffTime = 2.8f;
    public float quickAttackRadius = -1f;
    public float faceHeight;
    public bool throwingEnabled;
    public bool throwingCancelOnDamage;
    public Effect[] effect;
    public SupermanSettings supermanSettings;

    [System.NonSerialized]
    public int characterId;
    [System.NonSerialized]
    public GameObject target;
    [System.NonSerialized]
    public Transform targetTrans;
    [System.NonSerialized]
    public bool forceDefaultDifficulty;
    [System.NonSerialized]
    public bool isPlayer;
    [System.NonSerialized]
    public bool isEnemy;
    [System.NonSerialized]
    public bool isBoss;
    [System.NonSerialized]
    public bool isSuperman;
    [System.NonSerialized]
    public bool isItem;
    [System.NonSerialized]
    public bool slidingFlag;
    [System.NonSerialized]
    public bool mustKnockBackFlag;
    [System.NonSerialized]
    public bool mustKnockEffectiveEnabled;

    protected Transform trans;
    protected Animator anim;
    protected CharacterController cCon;
    protected NavMeshAgent agent;
    protected FaciemController fCon;
    protected delegate void VoidEvent();

    protected enum State { Spawn, Wait, Chase, Escape, Dodge, Attack, Damage, Dead, Jump, Climb };
    protected State state;
    protected Command command;
    protected Ray ray = new Ray();
    protected RaycastHit raycastHit = new RaycastHit();
    protected NavMeshHit navMeshHit = new NavMeshHit();
    protected float deltaTimeMove;
    protected float deltaTimeCache;
    protected int nowHP;
    protected float nowST;
    protected bool consumeStamina;
    protected float knockRemain;
    protected float knockRemainLight;
    protected bool cannotKnockDown;
    protected Vector3 knockDirection;
    protected int actDistNum;
    protected float totalTime;
    protected float stateTime;
    protected Vector3 destination;
    protected float mutekiTimeRemain;
    protected float attackedTimeRemain;
    protected float attackedTimeRemainReduceMultiplier = 1f;
    protected GameObject playerObj;
    protected Transform playerTrans;
    protected GameObject attackerObj;
    protected CharacterBase attackerCB;
    protected int attackProcess = 0;
    protected bool notResetAttackProcessOnDamage;
    protected int attackType = 0;
    protected float attackPowerMultiplier = 1;
    protected float defensePowerMultiplier = 1;
    protected float knockPowerMultiplier = 1;
    protected bool arrived = true;
    protected float roveInterval = 8f;
    protected bool agentPathReserved;
    protected bool isJumpingAttack = false;
    protected bool canRun = true;
    protected bool groundedFlag = true;
    protected LayerMask fieldLayerMask;
    protected LayerMask checkGroundedLayerMask;
    protected float checkGroundPivotHeight = 0.2f;
    protected float checkGroundTolerance = 0.8f;
    protected float checkGroundTolerance_Jumping = 0.5f;
    protected bool groundedFlag_ForAgent = false;
    protected float minMoveDistance_Default = 0.002f;
    protected float minMoveDistance_Special = 0.001f;
    protected float nowSpeed;
    protected float rotSpeed;
    protected float lastRotateY;
    protected Vector3 move;
    protected float gravityMultiplier = 1f;
    protected bool isDamageHeavy = false;
    protected float attackBiasValue;
    protected bool quickJumping = false;
    protected int jumpType;
    protected float rovingTime;
    protected float idlingTime;
    protected float idlingTimeCondition = 10f;
    protected bool isLockon = false;
    protected float lockonRotSpeed = 10;
    protected bool isSuperarmor = false;
    protected bool isAnimStopped = false;
    protected float targetRadius;
    protected float targetFixingTimeRemain;
    protected float timeScale = 1f;
    protected bool belligerent = true;
    protected float disableControlTimeRemain;
    protected float gravityZeroTimeRemain;
    protected float supermanTime;
    protected bool destroyOnDead = true;
    protected float deadTimer;
    protected float knockRestoreSpeed = 1f;
    protected bool mudToWalk = false;
    protected bool attackEffectEnabled = true;
    protected float staminaTreatAsAttackTimeRemain;
    protected bool setAttackIntervalToStaminaDontHeal = false;
    protected bool resetAgentRadiusOnChangeState = false;
    protected bool resetAgentHeightOnChangeState = false;
    protected bool animatorForBattle = true;
    protected float disableAttackStartTimeRemain;
    protected bool anyAttackEnabled = false;
    protected bool forceIgnoreFlag;
    protected float atttackIntervalSave;
    protected float lastKnockedSave;

    protected float attackStiffTime = 0.5f;
    protected Vector3 specialMoveDirection = Vector3.zero;
    protected float specialMoveDistance;
    protected float specialMoveDuration;
    protected EasingType specialMoveEasingType;
    protected VoidEvent specialMoveCompleteEvent;
    protected float specialMoveLastPoint;
    protected float specialMoveElapsedTime;
    protected Vector3 specialMoveVectorTemp;
    protected float specialMovePointTemp;
    protected bool specialMoveDirectionAdjustEnabled;

    protected float destinationUpdateInterval = 0.3f;
    protected int level;

    protected float watchoutTime = 4;
    protected float lockTargetTime = 2;
    protected float attackProcessResetTime = 0.75f;
    protected float attackLockonDefaultSpeed = 10;
    protected float attackWaitingLockonRotSpeed = 4;
    protected float commonLockonTowardsMultiplier = 1f;
    protected float lightMutekiTime = 0;
    protected float heavyMutekiTime = 0;
    protected float lightKnockRecoveryRate = 1f;
    protected float stoppingDistanceWait = 2.5f;
    protected float stoppingDistanceBattle = 1f;
    protected float stoppingDistanceRove = 1f;
    protected float defaultAgentRadius = 0.2f;
    protected float defaultAgentHeight = 1.4f;
    protected float agentResetTime;
    protected bool enableContinuousLightKnock;

    protected float stopTurnRotSpeed = 135;
    protected float spawnStiffTime = 1;
    protected float jumpSaveTime = 0.15f;
    protected float dodgeStiffTime = 0.6f;
    protected float climbStiffTime = 1;

    protected float superAttackRate = 1.5f;
    protected float superDefenseRate = 1.25f;
    protected float superKnockRate = 1.25f;
    protected float superKnockedRate = 2f;
    protected float superSpeedRate = 1.25f;
    protected float superAngularRate = 1.25f;
    protected float superAccelerationRate = 2;
    protected float superSTRate = 1.5f;
    protected float superCostRate = 2f / 3f;
    protected bool superAttackSuperarmor = true;

    protected int sideStepDirection = 0;

    protected float fbStepTime = 0.25f;
    protected float fbStepMaxDist = 4f;
    protected bool fbStepIgnoreY = true;
    protected bool fbStepConsiderRadius = false;
    protected EasingType fbStepEaseType = EasingType.SineInOut;

    protected LayerMask wallLayerMask;

    protected int colorTypeDamage = 0;
    protected int lastDamagedColorType = -1;
    protected const float costRunBaseSpeed = 9f;

    public const int damageColor_Friends = 0;
    public const int damageColor_Heal = 1;
    public const int damageColor_Enemy = 2;
    public const int damageColor_Critical = 3;
    public const int damageColor_Back = 4;
    public const int damageColor_Effective = 5;
    public const int damageColor_Hyper = 6;
    public const int damageColor_Hard = 7;
    public const int damageColor_HardBack = 8;

    protected float attackedTimeRemainOnDamage = 1;
    protected float attackedTimeRemainOnRestoreKnockDown;
    protected bool isAnimParamDetail = false;
    protected bool isAnimIdleEnabled = false;
    protected int idleType = 0;
    protected bool disablizeAgentOnJump = false;
    protected bool enableFriendsEffect = false;
    protected bool damageReportEnabled = false;
    protected int lightKnockCount;
    protected int heavyKnockCount;
    protected float quickAttackReduceRate = 1f;
    protected bool notAffectedByRisky;
    protected float attackStiffTimeSave;
    protected float attackIntervalSave;
    protected Transform rideTarget;
    protected bool rideResetCheck;
    protected bool rideTimeLimitEnabled;
    protected float rideTimeLimitDistance;
    protected Transform rideReleasePoint;
    protected bool rideContinueOnDamage;

    protected Throwing throwing;
    protected GameObject decoySave;
    public enum AttractionType { PaperPlane, IbisSong };

    protected const int sickMax = 8;
    protected float[] sickRemainTime = new float[sickMax];
    protected float[] sickEffectInterval = new float[sickMax];
    protected GameObject[] sickEffectInstance = new GameObject[sickMax];
    protected float sickHealSpeed = 1;
    protected int fireDamageRate = 1;

    protected class AnimatorParameters {
        public bool move;
        public bool run;
        public bool refresh;
        public bool slowDown;
        public float speed;
        public float rotSpeed;
        public float walkSpeed;
        public float animSpeed = 1;
        public bool deadSpecial;
    }
    protected AnimatorParameters animParam = new AnimatorParameters();

    protected class MoveCost {
        public float run = 4;
        public float attack = 6;
        public float step = 8;
        public float quick = 12;
        public float jump = 12;
        public float skill = 16;
    }
    protected MoveCost moveCost = new MoveCost();
    protected const int idleTypeMax = 2;
    protected const float evadeDistance = 15f;
    protected const float evadeSqrDist = 225f;

    public class TargetHate {
        public CharacterBase cBase;
        public int characterID;
        public float param;
        public double lastDamagedTimeStamp;
        public float dampingTimeRemain;
        public TargetHate() {
            this.cBase = null;
            this.characterID = -1;
            this.param = 0f;
            this.lastDamagedTimeStamp = 0f;
            this.dampingTimeRemain = 0f;
        }
    }

    protected TargetHate[] targetHates;
    protected bool targetHateEnabled;
    protected bool targetHateInitialized;
    protected const float targetHateUpdateInterval = 0.2f;
    protected const float targetHateDamping = 0.9440875f;
    protected const double targetHateClearTime = 7.5;
    protected const float targetHatePlayerBias = 4f;

    protected const int healStarID = 352;
    protected const int sandstarBlockID = 354;

    protected float dodgeRemain;
    protected float dodgeDamageHealMax;
    protected float dodgeCost = 2f;
    protected bool attackingDodgeEnabled;
    protected float dodgeDistance = 4f;
    protected float dodgeMutekiTime;

    protected static readonly Vector3 vecZero = Vector3.zero;
    protected static readonly Vector3 vecUp = Vector3.up;
    protected static readonly Vector3 vecDown = Vector3.down;
    protected static readonly Vector3 vecForward = Vector3.forward;
    protected static readonly Vector3 vecBack = Vector3.back;
    protected static readonly Quaternion quaIden = Quaternion.identity;
    protected static readonly Vector3[] escapeDirection = new Vector3[] {
        new Vector3(1f, 0f, 0f),
        new Vector3(0.9238795f, 0f, 0.3826835f),
        new Vector3(0.7071068f, 0f, 0.7071068f),
        new Vector3(0.3826835f, 0f, 0.9238795f),
        new Vector3(0f, 0f, 1f),
        new Vector3(-0.3826835f, 0f, 0.9238795f),
        new Vector3(-0.7071068f, 0f, 0.7071068f),
        new Vector3(-0.9238795f, 0f, 0.3826835f),
        new Vector3(-1f, 0f, 0f),
        new Vector3(-0.9238795f, 0f, -0.3826835f),
        new Vector3(-0.7071068f, 0f, -0.7071068f),
        new Vector3(-0.3826835f, 0f, -0.9238795f),
        new Vector3(0f, 0f, -1f),
        new Vector3(0.3826835f, 0f, -0.9238795f),
        new Vector3(0.7071068f, 0f, -0.7071068f),
        new Vector3(0.9238795f, 0f, -0.3826835f)
    };

    public virtual int Level {
        get {
            return 0;
        }
        set {
        }
    }


    protected virtual void Awake() {
        trans = transform;
        anim = GetComponent<Animator>();
        cCon = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
        fCon = GetComponent<FaciemController>();
        animatorForBattle = SetAnimatorDefaultParam();
        if (animatorForBattle) {            
            attackerObj = target = null;
            move = vecZero;
            lastRotateY = trans.localEulerAngles.y;
            fieldLayerMask = LayerMask.GetMask("Field", "SecondField");
            if (agent) {
                defaultAgentRadius = agent.radius;
                defaultAgentHeight = agent.height;
                Update_MoveControl_ChildAgentSpeed();
                agent.acceleration = GetAcceleration();
                agent.angularSpeed = GetAngularSpeed();
            }
            mutekiTimeRemain = 0.5f;
            wallLayerMask = LayerMask.GetMask("Field", "InvisibleWall", "SecondField");
            if (throwingEnabled) {
                throwing = GetComponent<Throwing>();
            }
            idlingTimeCondition = Random.Range(9f, 11f);
            SetState(State.Spawn);
        }
    }

    protected virtual void Start() {
        if (CharacterManager.Instance) {
            characterId = CharacterManager.Instance.IssueID();
            playerObj = CharacterManager.Instance.playerObj;
            playerTrans = CharacterManager.Instance.playerTrans;
        }
        ResetHP();
        SetMapChip();
        if (targetHateEnabled && !targetHateInitialized) {
            InitTargetHate();
        }
        lastRotateY = trans.localEulerAngles.y;
    }

    protected virtual void OnEnable() {
        if (animatorForBattle) {
            InitializeOnEnable();
            disableControlTimeRemain = 0.25f;
            gravityZeroTimeRemain = 0.25f;
        }
    }

    protected virtual void OnDestroy() {
        if (animatorForBattle) {
            ReleaseAttackDetections();
            specialMoveDuration = 0f;
            move = vecZero;
        }
    }

    /*
    void CalcEscapeDirection() {
        escapeDirection = new Vector3[16];
        Vector3 vecTemp = vecZero;
        for (int i = 0; i < 16; i++) {
            vecTemp.x = Mathf.Cos(Mathf.PI * 2 / 16 * i);
            vecTemp.z = Mathf.Sin(Mathf.PI * 2 / 16 * i);
            escapeDirection[i] = vecTemp;
            MyMath.NormalizeXZ(ref escapeDirection[i]);
        }
    }
    */

    protected void LookNoWallDirection(float rayDistance, float heightOffset, LayerMask rayLayerMask) {
        Vector3 pivotPos = trans.position;
        pivotPos.y += heightOffset;
        float maxSqrDist = -1f;
        int maxIndex = -1;
        ray.origin = pivotPos;
        int[] shuffleIndex = new int[escapeDirection.Length];
        for (int i = 0; i < shuffleIndex.Length; i++) {
            shuffleIndex[i] = i;
        }
        for (int i = shuffleIndex.Length - 1; i > 0; i--) {
            int r = Random.Range(0, i + 1);
            int temp = shuffleIndex[i];
            shuffleIndex[i] = shuffleIndex[r];
            shuffleIndex[r] = temp;
        }
        for (int i = 0; i < shuffleIndex.Length; i++) {
            ray.direction = escapeDirection[shuffleIndex[i]];
            if (Physics.Raycast(ray, out raycastHit, rayDistance, rayLayerMask, QueryTriggerInteraction.Collide)) {
                float sqrDist = (raycastHit.point - pivotPos).sqrMagnitude;
                if (sqrDist > maxSqrDist) {
                    maxSqrDist = sqrDist;
                    maxIndex = shuffleIndex[i];
                }
            } else {
                maxSqrDist = rayDistance * rayDistance;
                maxIndex = shuffleIndex[i];
                break;
            }
        }
        if (maxIndex >= 0) {
            trans.forward = escapeDirection[maxIndex];
        }
    }
    protected virtual bool SetAnimatorDefaultParam() {
        if (anim) {
            anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.AnimSpeed], 1);
            anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.AttackSpeed], 1);
            anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], knockRestoreSpeed);
        }
        return true;
    }

    protected virtual void SetMapChip() {
    }

    public virtual void ResetHP() {
        nowHP = GetMaxHP();
        nowST = GetMaxST();
        knockRemain = knockEndurance;
        knockRemainLight = knockEnduranceLight;
    }

    public void InitializeOnEnable() {
        ResetTriggerOnDamage();
        AnimControl_Reset();
        SupermanEnd(false);
        ClearSick();
    }

    public Vector3 GetCenterPosition() {
        if (centerPivot) {
            return centerPivot.position;
        } else if (cCon) {
            return trans.position + cCon.center;
        } else {
            return trans.position + vecUp * 0.5f;
        }
    }

    protected virtual void Update_GetDelta() {
        deltaTimeCache = Time.deltaTime;
        timeScale = GetSick(SickType.Slow) ? 0.5f : 1f;
        deltaTimeMove = deltaTimeCache * timeScale;
        if (playerObj == null) {
            playerObj = CharacterManager.Instance.playerObj;
            playerTrans = CharacterManager.Instance.playerTrans;
        }
    }

    protected virtual void Update_TimeCount() {
        totalTime += deltaTimeCache;
        stateTime += deltaTimeMove;
        mutekiTimeRemain -= deltaTimeMove;
        if (state == State.Attack || state == State.Jump && attackedTimeRemainReduceMultiplier == 1f) {
            attackedTimeRemain -= deltaTimeMove;
        } else {
            attackedTimeRemain -= deltaTimeMove * attackedTimeRemainReduceMultiplier;
        }
        if (quickAttackRadius > 0f && attackedTimeRemain > 0f && state != State.Attack && targetTrans) {
            float sqrDist = GetTargetDistance(true, true, false);
            if (sqrDist < quickAttackRadius * quickAttackRadius) {
                attackedTimeRemain -= deltaTimeCache * quickAttackReduceRate;
                if (attackedTimeRemain > 1f) {
                    attackedTimeRemain = 1f;
                }
            }
        }
        disableControlTimeRemain -= deltaTimeMove;
        gravityZeroTimeRemain -= deltaTimeCache;
        targetFixingTimeRemain -= deltaTimeCache;
        supermanTime += deltaTimeCache;
        if (!isPlayer && rideTimeLimitEnabled && rideTarget && (disableControlTimeRemain <= 0f || gravityZeroTimeRemain <= 0f)) {
            if (isEnemy || command == Command.Free || CharacterManager.Instance.GetBetweenPlayerDistance(trans.position, true, false) >= rideTimeLimitDistance * rideTimeLimitDistance) {
                RemoveRide(true);
            } else {
                if (disableControlTimeRemain < 0.5f) {
                    disableControlTimeRemain = 0.5f;
                }
                if (gravityZeroTimeRemain < 0.5f) {
                    gravityZeroTimeRemain = 0.5f;
                }
            }
        }
        if (staminaTreatAsAttackTimeRemain > 0f) {
            staminaTreatAsAttackTimeRemain -= deltaTimeMove;
        }
        if (disableAttackStartTimeRemain > 0f) {
            disableAttackStartTimeRemain -= deltaTimeCache;
        }
        if (attackProcess != 0 && attackedTimeRemain < -attackProcessResetTime) {
            attackProcess = 0;
        }
        if (state == State.Wait) {
            rovingTime += deltaTimeCache;
        } else if (state != State.Wait && state != State.Jump && state != State.Spawn) {
            rovingTime = 0f;
        }
        if (state != State.Spawn && state != State.Wait) {
            for (int i = 0; i < searchArea.Length; i++) {
                searchArea[i].SetWatchout(watchoutTime);
            }
        }
        if (knockRecovery > 0f) {
            float heavyEnduranceTemp = GetHeavyKnockEndurance();
            float lightEnduranceTemp = GetLightKnockEndurance();
            if (knockRemain != heavyEnduranceTemp) {
                knockRemain += knockRecovery * deltaTimeMove;
                if (knockRemain > heavyEnduranceTemp) {
                    knockRemain = heavyEnduranceTemp;
                }
            }
            if (knockRemainLight != lightEnduranceTemp) {
                knockRemainLight += knockRecovery * lightKnockRecoveryRate * deltaTimeMove;
                if (knockRemainLight > lightEnduranceTemp) {
                    knockRemainLight = lightEnduranceTemp;
                }
            }
        }
        for (int i = 0; i < sickRemainTime.Length; i++) {
            if (sickRemainTime[i] > 0) {
                sickRemainTime[i] -= deltaTimeCache * sickHealSpeed;
            }
            if (sickEffectInterval[i] > 0) {
                sickEffectInterval[i] -= deltaTimeCache;
            }
        }
        if (anim.speed != timeScale) {
            anim.speed = timeScale;
        }
    }

    protected virtual void Update_Targeting() {
        if (targetHateEnabled) {
            UpdateTargetHate();
            if (targetFixingTimeRemain <= 0f) {
                SetLockTargetToTargetHate();
            }
        }
        for (int i = 0; i < searchArea.Length; i++) {
            if (searchArea[i] && searchArea[i].enabled) {
                target = searchArea[i].GetNowTarget();
                if (target) {
                    targetTrans = searchArea[i].GetNowTargetTransform();
                    targetRadius = searchArea[i].GetNowTargetRadius();
                    break;
                }
            }
        }
        if (!target) {
            targetTrans = null;
            targetRadius = 0f;
        }
    }

    protected virtual float GetCheckGroundTolerance() {
        if (state == State.Jump || (state == State.Attack && isJumpingAttack)) {
            return checkGroundTolerance_Jumping;
        } else {
            return checkGroundTolerance;
        }
    }

    protected virtual void Update_StatusJudge() {
        if (slidingFlag) {
            groundedFlag_ForAgent = groundedFlag = false;
        } else {
            float tolerance = GetCheckGroundTolerance();
            groundedFlag = CheckGrounded(tolerance);
            if (agent) {
                groundedFlag_ForAgent = CheckNearNavMesh(tolerance);
            } else {
                groundedFlag_ForAgent = groundedFlag;
            }
        }
        if (nowST < 0f) {
            nowST = 0f;
        }
        if (nowST < 1f) {
            canRun = false;
        }
        if (!canRun && nowST >= GetCost(CostType.Run) * 2) {
            canRun = true;
        }
        if (agent) {
            if (totalTime >= 0.2f && state != State.Jump && state != State.Dead && (state != State.Attack || !isJumpingAttack)) {
                if (groundedFlag_ForAgent) {
                    if (!agent.enabled && gravityZeroTimeRemain <= 0f) {
                        agent.enabled = true;
                    } else if (!agent.isOnNavMesh) {
                        agent.enabled = false;
                    }
                } else {
                    if (groundedFlag && gravityZeroTimeRemain <= 0f) {
                        // Agent Restart
                        agentResetTime += deltaTimeCache;
                        if (agentResetTime >= 1f) {
                            agentResetTime = 0f;
                            agent.enabled = false;
                            agent.enabled = true;
                        }
                    } else {
                        agentResetTime = 0f;
                    }
                }
            }
            if (agent.enabled && agent.isOnNavMesh && state == State.Jump) {
                if (agent.isOnOffMeshLink) {
                    agentPathReserved = true;
                } else if (!groundedFlag_ForAgent) {
                    agent.enabled = false;
                    agentPathReserved = false;
                }
            }
        }
        if (cCon) {
            if (GetCanControl() && state != State.Jump) {
                if (cCon.minMoveDistance != minMoveDistance_Default) {
                    cCon.minMoveDistance = minMoveDistance_Default;
                }
            } else {
                if (cCon.minMoveDistance != minMoveDistance_Special) {
                    cCon.minMoveDistance = minMoveDistance_Special;
                }
            }
        }
    }

    public virtual void SetAgentEnabled(bool toEnable) {
        if (agent) {
            agent.enabled = toEnable;
        }
    }

    protected virtual float GetTargetDistance(bool isSqrDist = true, bool ignoreY = true, bool considerTargetRadius = false) {
        float dist = 0f;
        if (targetTrans) {
            Vector3 targetPos = targetTrans.position;
            if (ignoreY) {
                targetPos.y = trans.position.y;
            }
            if (considerTargetRadius) {
                if ((targetPos - trans.position).sqrMagnitude <= targetRadius * targetRadius) {
                    return 0f;
                } else {
                    targetPos += (trans.position - targetPos).normalized * targetRadius;
                }
            }
            if (isSqrDist) {
                dist = (targetPos - trans.position).sqrMagnitude;
            } else {
                dist = Vector3.Distance(targetPos, trans.position);
            }
        }
        return dist;
    }

    protected virtual float GetTargetHeight(bool considerTargetRadius = false) {
        float dist = 0f;
        if (targetTrans) {
            dist = targetTrans.position.y - trans.position.y;
            if (considerTargetRadius) {
                if (dist > 0f) {
                    dist = Mathf.Max(dist - targetRadius, 0f);
                } else if (dist < 0f) {
                    dist = Mathf.Min(dist + targetRadius, 0f);
                }
            }
        }
        return dist;
    }

    protected virtual Vector3 GetTargetVector(bool ignoreY = true, bool normalize = true, bool reverse = false) {
        if (targetTrans) {
            Vector3 answer;
            if (reverse) {
                answer = trans.position - targetTrans.position;
            } else {
                answer = targetTrans.position - trans.position;
            }
            if (normalize) {
                if (ignoreY) {
                    MyMath.NormalizeXZ(ref answer);
                } else {
                    MyMath.Normalize(ref answer);
                }
            } else {
                if (ignoreY) {
                    answer.y = 0f;
                }
            }
            return answer;
        }
        return vecZero;
    }

    protected virtual bool CheckSickEffect(SickType sickType, int databaseIndex, bool isCenter = false) {
        int effectIndex = (int)sickType;
        if (effectIndex >= 0) {
            if (GetSick(sickType)) {
                if (effectIndex < sickEffectInstance.Length && !sickEffectInstance[effectIndex] && EffectDatabase.Instance) {
                    sickEffectInstance[effectIndex] = Instantiate(EffectDatabase.Instance.prefab[databaseIndex], isCenter && centerPivot ? centerPivot : trans);
                    return true;
                }
            } else if (effectIndex < sickEffectInstance.Length && sickEffectInstance[effectIndex]) {
                Destroy(sickEffectInstance[effectIndex]);
            }
        }
        return false;
    }

    protected virtual void Update_Sick() {
        CheckSickEffect(SickType.Fire, (int)EffectDatabase.id.sickFire, false);
        CheckSickEffect(SickType.Poison, (int)EffectDatabase.id.sickPoison, true);
        CheckSickEffect(SickType.Ice, (int)EffectDatabase.id.sickIce, false);
        CheckSickEffect(SickType.Acid, (int)EffectDatabase.id.sickAcid, true);
        CheckSickEffect(SickType.Slow, (int)EffectDatabase.id.sickSlow, true);
        CheckSickEffect(SickType.Stop, (int)EffectDatabase.id.sickStop, true);
        CheckSickEffect(SickType.Frightened, (int)EffectDatabase.id.sickFrightened, true);
        if (GetSick(SickType.Fire) && sickEffectInterval[(int)SickType.Fire] <= 0f) {
            sickEffectInterval[(int)SickType.Fire] += 0.5f;
            if (nowHP > 1 && fireDamageRate > 0) {
                AddNowHP(Mathf.Min(nowHP - 1, StageManager.Instance.GetSlipDamage(isEnemy) * fireDamageRate) * (-1), GetCenterPosition(), true, colorTypeDamage);
            }
        }
        if (GetSick(SickType.Poison) && sickEffectInterval[(int)SickType.Poison] <= 0f) {
            sickEffectInterval[(int)SickType.Poison] += 2f;
            if (nowHP > 1) {
                AddNowHP(Mathf.Min(nowHP - 1, StageManager.Instance.GetSlipDamage(isEnemy)) * (-1), GetCenterPosition(), true, colorTypeDamage);
            }
        }
        if (GetSick(SickType.Ice)) {
            /*
            if (sickEffectInterval[(int)SickType.Ice] <= 0) {
                sickEffectInterval[(int)SickType.Ice] += 2f;
                if (nowHP > 1) {
                    AddNowHP(Mathf.Min(nowHP - 1, StageManager.Instance.GetSlipDamage(isEnemy)) * (-1), GetCenterPosition(), true, colorTypeDamage);
                }
            }
            */
            
            nowST -= (StageManager.Instance.GetSlipDamage(isEnemy) * 1.25f + 7.5f * CharacterManager.Instance.GetDifficultyEffect(CharacterManager.DifficultyEffect.SlipDamage)) * 6f * deltaTimeCache;
            if (nowST < 0) {
                nowST = 0;
            }
        }
        sickHealSpeed = CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.SickHeal);
    }

    protected virtual void Update_Transition_Spawn() {
        if (stateTime > spawnStiffTime) {
            SetState(State.Wait);
        }
    }

    protected virtual void Update_Transition_Wait() {
        Update_Transition_Moves();
    }

    protected virtual void Update_Transition_Chase() {
        Update_Transition_Moves();
    }

    protected virtual void Update_Transition_Escape() {
        Update_Transition_Moves();
    }

    protected virtual bool AttackConditionAdditive() {
        return true;
    }

    protected virtual void Update_Transition_Moves() {
        if (!groundedFlag) {
            move.y = 0;
            anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.JumpType], jumpType = 0);
            SetState(State.Jump);
        } else if (!isPlayer) {
            if (target && command != Command.Ignore && !forceIgnoreFlag) {
                float sqrDist = Mathf.Max(GetTargetDistance(true, false, true), 0.0001f);
                if (command != Command.Evade) {
                    if (actDistNum < agentActionDistance.Length) {
                        bool attackCond = (sqrDist > MyMath.Square(agentActionDistance[actDistNum].attack.x) && sqrDist < MyMath.Square(agentActionDistance[actDistNum].attack.y) && JudgeStamina(GetCost(CostType.Attack)) && disableControlTimeRemain <= 0f);
                        bool chaseCond = (sqrDist > MyMath.Square(agentActionDistance[actDistNum].chase.x) && sqrDist < MyMath.Square(agentActionDistance[actDistNum].chase.y));
                        bool escapeCond = (sqrDist > MyMath.Square(agentActionDistance[actDistNum].escape.x) && sqrDist < MyMath.Square(agentActionDistance[actDistNum].escape.y));
                        if (attackCond && state != State.Attack && attackedTimeRemain < 0 && !GetSick(SickType.Stop) && AttackConditionAdditive()) {
                            SetState(State.Attack);
                        } else if (chaseCond) {
                            if (state != State.Chase) {
                                SetState(State.Chase);
                            }
                        } else if (escapeCond) {
                            if (state != State.Escape) {
                                SetState(State.Escape);
                            }
                        } else {
                            if (belligerent) {
                                if (!escapeCond && attackCond && attackedTimeRemain >= 0) {
                                    lockonRotSpeed = attackWaitingLockonRotSpeed;
                                    if (lockonRotSpeed > 0) {
                                        CommonLockon();
                                    }
                                } else if (state == State.Chase && !chaseCond && !escapeCond) {
                                    SetState(State.Wait);
                                }
                            } else {
                                if (state != State.Wait && !escapeCond) {
                                    SetState(State.Wait);
                                }
                            }
                        }
                    }
                } else {
                    if (state != State.Escape && sqrDist <= evadeSqrDist) {
                        SetState(State.Escape);
                    } else if (state != State.Wait && sqrDist > evadeSqrDist) {
                        SetState(State.Wait);
                    }
                }
            } else if (state != State.Wait) {
                SetState(State.Wait);
            }
            if (agent) {
                if (state == State.Wait || state == State.Jump) {
                    if (!isEnemy && command == Command.Free) {
                        if (agent.stoppingDistance != stoppingDistanceRove) {
                            agent.stoppingDistance = stoppingDistanceRove;
                        }
                    } else {
                        if (agent.stoppingDistance != stoppingDistanceWait) {
                            agent.stoppingDistance = stoppingDistanceWait;
                        }
                    }
                } else {
                    if (agent.stoppingDistance != stoppingDistanceBattle) {
                        agent.stoppingDistance = stoppingDistanceBattle;
                    }
                }
            }
        }
        if (disablizeAgentOnJump) {
            disablizeAgentOnJump = false;
        }
    }

    protected virtual void Update_Transition_Dodge() {
        if (stateTime > dodgeStiffTime) {
            SetState(State.Wait);
        }
    }

    protected virtual void Update_Transition_Attack() {
        if (stateTime > attackStiffTime) {
            SetState(State.Wait);
        }
    }

    protected virtual void Update_Transition_Damage() {
        if ((!isDamageHeavy && stateTime >= GetLightStiffTime()) || (isDamageHeavy && stateTime >= GetHeavyStiffTime())) {
            if (attackedTimeRemainOnRestoreKnockDown > 0f && isDamageHeavy && attackedTimeRemain < attackedTimeRemainOnRestoreKnockDown) {
                attackedTimeRemain = attackedTimeRemainOnRestoreKnockDown;
            }
            isDamageHeavy = false;
            ResetTriggerOnDamage();
            SetState(State.Wait);
        } else {
            if (isDamageHeavy) {
                knockRemain = GetHeavyKnockEndurance();
            }
            if (!enableContinuousLightKnock || isDamageHeavy) {
                knockRemainLight = GetLightKnockEndurance();
            }
        }
    }

    protected virtual void Update_Transition_Dead() {
        ResetTriggerOnDamage();
    }

    protected virtual void Update_Transition_Jump() {
        if (stateTime > (isJumpingAttack ? 0.7f : 0.5f) && groundedFlag && (GetCanControl() || disableControlTimeRemain > 0f)) {
            nowSpeed = 0;
            anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Landing]);
            SetState(State.Wait);
            if (agent) {
                agent.enabled = true;
            }
        }
    }

    protected virtual void Update_Transition_Climb() {
    }

    protected void Update_Transition() {
        if (Time.timeScale > 0f) {
            switch (state) {
                case State.Spawn:
                    Update_Transition_Spawn();
                    break;
                case State.Wait:
                    Update_Transition_Wait();
                    break;
                case State.Chase:
                    Update_Transition_Chase();
                    break;
                case State.Escape:
                    Update_Transition_Escape();
                    break;
                case State.Dodge:
                    Update_Transition_Dodge();
                    break;
                case State.Attack:
                    Update_Transition_Attack();
                    break;
                case State.Damage:
                    Update_Transition_Damage();
                    break;
                case State.Dead:
                    Update_Transition_Dead();
                    break;
                case State.Jump:
                    Update_Transition_Jump();
                    break;
                case State.Climb:
                    Update_Transition_Climb();
                    break;
            }
        }
    }

    protected virtual void Update_PlayerControl() {
    }

    protected virtual void Start_Process_Spawn() {
        if (agent) {
            if (agent && agent.pathStatus != NavMeshPathStatus.PathInvalid) {
                agent.ResetPath();
            }
            agent.enabled = false;
        }
    }

    protected virtual void Update_Process_Spawn() {
    }

    protected virtual void Start_Process_Wait() {
        ReleaseAttackDetections();
        if (agentPathReserved) {
            agentPathReserved = false;
        } else if (agent && agent.enabled && agent.hasPath) {
            agent.ResetPath();
        }
    }

    protected virtual void GetRoveDestination() {
        destination = StageManager.Instance.dungeonController.GetRespawnPos();
    }

    protected virtual void Rove() {
        if (agent && StageManager.Instance && StageManager.Instance.dungeonController) {
            if (arrived) {
                if (roveInterval > 0f && rovingTime >= roveInterval) {
                    if (agent.pathStatus != NavMeshPathStatus.PathInvalid) {
                        GetRoveDestination();
                        agent.SetDestination(destination);
                        arrived = false;
                        rovingTime = 0f;
                    }
                }
            } else if ((trans.position - agent.destination).sqrMagnitude < MyMath.Square(agent.stoppingDistance)) {
                arrived = true;
                rovingTime = 0f;
            }
        }
    }

    protected virtual void Update_Process_Wait() {
        if (agent && !agent.enabled && groundedFlag_ForAgent && gravityZeroTimeRemain <= 0f) {
            agent.enabled = true;
        }
        Rove();
    }

    protected virtual void Start_Process_Chase() {
        stateTime = destinationUpdateInterval;
    }

    protected virtual void Update_Process_Chase() {
        if (agent && targetTrans) {
            if (stateTime >= destinationUpdateInterval) {
                stateTime = 0;
                if (agent.pathStatus != NavMeshPathStatus.PathInvalid) {
                    float sqrDist = GetTargetDistance(true, false, true);
                    if (sqrDist < MyMath.Square(agentActionDistance[actDistNum].chase.x)) {
                        agent.ResetPath();
                    } else {
                        destination = targetTrans.position;
                        agent.SetDestination(destination);
                    }
                }
            }
        }
    }

    protected virtual void Start_Process_Escape() {
        stateTime = destinationUpdateInterval;
    }

    protected virtual bool IsSeparated() {
        return false;
    }

    protected virtual Vector3 GetEscapeDestination(Vector3 escapeFrom, float distance) {
        Vector3 pivot = trans.position;
        Vector3 answer = trans.position;
        float distMax = 0;
        bool limitEnabled = (!isEnemy && !isPlayer && command != Command.Free && !IsSeparated() && CharacterManager.Instance.playerTrans);
        bool isBossFloor = (limitEnabled ? StageManager.Instance && StageManager.Instance.isBossFloor : false);
        pivot.y += 0.4f;
        for (int i = 0; i < escapeDirection.Length; i++) {
            Vector3 direction = trans.TransformDirection(escapeDirection[i]);
            Vector3 point = pivot + direction * distance;
            ray.origin = pivot;
            ray.direction = direction;
            if (Physics.Raycast(ray, out raycastHit, evadeDistance, wallLayerMask, QueryTriggerInteraction.Collide)) {
                point = raycastHit.point;
            }

            if (limitEnabled) {
                float distTemp = Vector3.Distance(point, escapeFrom);
                float limitDist = (isBossFloor ? CharacterManager.evadeLimitDistanceBoss : CharacterManager.evadeLimitDistance);
                float distBP = CharacterManager.Instance.GetBetweenPlayerDistance(point, false, true);
                if (distBP > limitDist) {
                    distTemp -= (distBP - limitDist);
                }
                if (distTemp > distMax) {
                    distMax = distTemp;
                    point.y = trans.position.y;
                    answer = point;
                }
            } else {
                float sqrDistTemp = (point - escapeFrom).sqrMagnitude;
                if (sqrDistTemp > distMax) {
                    distMax = sqrDistTemp;
                    point.y = trans.position.y;
                    answer = point;
                }
            }
        }
        return answer;
    }

    protected virtual void Update_Process_Escape() {
        if (agent && agent.enabled && targetTrans && stateTime > destinationUpdateInterval) {
            stateTime = 0;
            if (agent.pathStatus != NavMeshPathStatus.PathInvalid) {
                if (GetTargetDistance(true, false, true) > (command == Command.Evade ? evadeDistance * evadeDistance : MyMath.Square(agentActionDistance[actDistNum].escape.y))) {
                    agent.ResetPath();
                } else {
                    destination = GetEscapeDestination(searchArea[0].GetTargetsAveragePosition(), evadeDistance);
                    agent.SetDestination(destination);
                }
            }
        }
    }

    protected virtual void Start_Process_Dodge() { }
    protected virtual void Update_Process_Dodge() {
        if (anyAttackEnabled) {
            ReleaseAttackDetections();
        }
    }

    protected virtual void Start_Process_Attack() {
        Attack();
    }
    protected virtual void Update_Process_Attack() {
        AttackContinuous();
    }

    protected virtual void Start_Process_Damage() { }
    protected virtual void Update_Process_Damage() {
        if (anyAttackEnabled) {
            ReleaseAttackDetections();
        }
    }

    protected virtual void Start_Process_Dead() {
        ResetTriggerOnDamage();
        if (throwing && throwingCancelOnDamage) {
            if (isEnemy) {
                throwing.ThrowCancelAll(false);
            } else {
                throwing.ThrowCancelAll(true);
            }
        }
        if (searchTarget.Length > 0) {
            SetSearchTargetActive(false);
        }
        anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.Dead], true);
        if (deadTimer <= 0) {
            deadTimer = -1;
            DeadProcess();
        }
    }

    protected virtual void Update_Process_Dead() {
        if (deadTimer > 0 && deadTimer < 1000 && deadTimer <= stateTime) {
            deadTimer = -1;
            DeadProcess();
        }
    }

    protected virtual void DeadProcess() {
        if (destroyOnDead) {
            Destroy(gameObject);
        }
    }

    protected virtual void Start_Process_Jump() {
        anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Jump]);
        if (disablizeAgentOnJump && agent && agent.enabled) {
            agent.enabled = false;
        }
    }

    protected virtual void Update_Process_Jump() { }
    protected virtual void Update_Process_Climb() { }

    protected virtual void Update_MoveControl_ChildAgentSpeed() {
        float speedTemp = (canRun && state != State.Wait ? GetMaxSpeed(false) : GetMaxSpeed(true));
        if (agent.speed != speedTemp) {
            agent.speed = speedTemp;
        }
    }

    protected virtual void MoveControlChild_Gravity() {
        if (gravityZeroTimeRemain <= 0f) {
            if (agent) {
                if (state == State.Spawn || (agent.enabled && groundedFlag_ForAgent && groundedFlag && agent.isOnNavMesh)) {
                    move.y = 0f;
                } else {
                    move.y += Physics.gravity.y * deltaTimeMove * gravityMultiplier;
                }
            } else {
                if (groundedFlag && move.y < Physics.gravity.y) {
                    move.y = Physics.gravity.y;
                } else {
                    move.y += Physics.gravity.y * deltaTimeMove * gravityMultiplier;
                }
            }
        }
    }

    protected virtual void Update_MoveControl() {
        Update_SpecialMove();
        if (slidingFlag) {
            slidingFlag = false;
            Vector3 originTemp = trans.position;
            originTemp.y += 0.5f;
            if (Physics.SphereCast(originTemp, cCon.radius, vecDown, out raycastHit, 1f, checkGroundedLayerMask, QueryTriggerInteraction.Ignore)) {
                Vector3 groundNormal = raycastHit.normal;
                if (groundNormal.y > 0f && 30f < Vector3.Angle(groundNormal, vecUp)) {
                    move.x += groundNormal.x * 20f;
                    // move.y -= groundNormal.y * 10f * deltaTimeCache;
                    move.z += groundNormal.z * 20f;
                }
            }
        }
        if (agent) {
            if (agent.pathStatus != NavMeshPathStatus.PathInvalid) {
                bool stopCond = !GetCanControl();
                bool speedZeroCond = (!GetCanMove() && state != State.Spawn);
                if (agent.isStopped != stopCond) {
                    agent.isStopped = stopCond;
                }
                if (speedZeroCond) {
                    if (agent.speed != 0f) {
                        agent.speed = 0f;
                    }
                    if (agent.velocity != vecZero) {
                        agent.velocity = vecZero;
                    }
                } else {
                    Update_MoveControl_ChildAgentSpeed();
                }
                float accelerationTemp = GetAcceleration();
                if (agent.acceleration != accelerationTemp) {
                    agent.acceleration = GetAcceleration();
                }
                float angularSpeedTemp = GetAngularSpeed();
                if (agent.angularSpeed != angularSpeedTemp) {
                    agent.angularSpeed = angularSpeedTemp;
                }
            }
            nowSpeed = agent.velocity.magnitude;
            nowSpeed = nowSpeed < 0.1f ? 0 : nowSpeed > agent.speed ? agent.speed : nowSpeed;
            float eulerAnglesYTemp = trans.localEulerAngles.y;
            if (lastRotateY > eulerAnglesYTemp + 180f) {
                lastRotateY -= 360f;
            } else if (lastRotateY < eulerAnglesYTemp - 180f) {
                lastRotateY += 360f;
            }
            if (deltaTimeMove > 0 && angularSpeed > 0) {
                rotSpeed = Mathf.Clamp((eulerAnglesYTemp - lastRotateY) / (deltaTimeMove * angularSpeed), -1f, 1f);
            } else {
                rotSpeed = 0f;
            }
            lastRotateY = eulerAnglesYTemp;
        }
        if (!isEnemy) {
            if (rideTarget) {
                nowSpeed = 0f;
                rotSpeed = 0f;
                if (state == State.Wait || (rideContinueOnDamage && state == State.Damage)) {
                    trans.SetPositionAndRotation(rideTarget.position, rideTarget.rotation);
                } else {
                    RemoveRide();
                }
            } else if (rideResetCheck) {
                rideResetCheck = false;
                Vector3 eulerTemp = trans.localEulerAngles;
                if (eulerTemp.x != 0f || eulerTemp.z != 0f) {
                    eulerTemp.x = 0f;
                    eulerTemp.z = 0f;
                    trans.localEulerAngles = eulerTemp;
                }
            }
        }
        MoveControlChild_Gravity();
        if (move != vecZero && deltaTimeMove > 0f) {
            cCon.Move(move * deltaTimeMove);
        }
    }

    protected virtual void STControlChild_STConsume() {
        if (nowSpeed > GetMaxSpeed(true, false, false, true) && deltaTimeMove > 0f) {
            nowST -= GetCost(CostType.Run) * deltaTimeMove;
        }
        if (nowST < 0f) {
            nowST = 0f;
        }
    }

    protected virtual void STControlChild_STHeal() {
        if (deltaTimeCache > 0f) {
            nowST += GetSTHealRate() * deltaTimeCache;
        }
        if (nowST > GetMaxST()) {
            nowST = GetMaxST();
        }
    }

    protected virtual void STControlChild_DodgePowerHeal() {
        float healRate = GetDodgeHealRate();
        if (healRate > 0f && dodgeRemain < dodgePower) {
            dodgeRemain += dodgePower * healRate * deltaTimeCache;
            if (dodgeRemain > dodgePower) {
                dodgeRemain = dodgePower;
            }
        }
    }

    protected virtual void Update_STControl() {
        if (state != State.Attack && !(state == State.Jump && isJumpingAttack) && isSuperarmor) {
            SuperarmorEnd();
        }
        if (consumeStamina) {
            STControlChild_STConsume();
            STControlChild_STHeal();
        } else {
            nowST = GetMaxST();
        }
        if (isEnemy && dodgePower > 0f) {
            STControlChild_DodgePowerHeal();
        }
    }

    protected virtual void AnimControl_Reset() {
        animParam = new AnimatorParameters();
        anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.Move], animParam.move);
        anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.Run], animParam.run);
        anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.Refresh], animParam.refresh);
        anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.Speed], animParam.speed);
        anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.RotSpeed], animParam.rotSpeed);
        anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.WalkSpeed], animParam.walkSpeed);
        anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.AnimSpeed], animParam.animSpeed);
    }

    protected virtual void UpdateAC_Move() {
        bool temp = nowSpeed > 0.1f || nowSpeed < -0.1f;
        if (animParam.move != temp) {
            animParam.move = temp;
            anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.Move], animParam.move);
        }
    }

    protected virtual void UpdateAC_AnimSpeed() {
        float temp = (GetSick(SickType.Mud) ? 0.5f : 1);
        if (animParam.animSpeed != temp) {
            animParam.animSpeed = temp;
            anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.AnimSpeed], animParam.animSpeed);
        }
    }

    protected virtual void UpdateAC_Refresh() {
        if (nowSpeed == 0f && rotSpeed == 0f) {
            if (nowST < GetMaxST() * 0.3f && !animParam.refresh && !rideTarget) {
                animParam.refresh = true;
                anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.Refresh], animParam.refresh);
            } else if ((nowST >= GetMaxST() && animParam.refresh) || rideTarget) {
                animParam.refresh = false;
                anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.Refresh], animParam.refresh);
            }
        } else {
            if (animParam.refresh) {
                animParam.refresh = false;
                anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.Refresh], animParam.refresh);
            }
        }
    }

    protected virtual void UpdateAC_Speed() {
        float temp = GetOriginalSpeed();
        if (animParam.speed != temp) {
            animParam.speed = temp;
            anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.Speed], animParam.speed);
        }
    }

    protected virtual void UpdateAC_Run() {
        bool temp = GetOriginalSpeed() > GetMaxSpeed(true, false, false, true);
        if (mudToWalk && GetSick(SickType.Mud)) {
            temp = false;
        }
        if (animParam.run != temp) {
            animParam.run = temp;
            anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.Run], animParam.run);
        }
    }

    protected virtual void UpdateAC_RotSpeed() {
        float temp = LinearEasing(animParam.rotSpeed, rotSpeed, 4f);
        if (animParam.rotSpeed != temp) {
            animParam.rotSpeed = temp;
            anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.RotSpeed], animParam.rotSpeed);
        }
    }

    protected virtual void UpdateAC_WalkSpeed() {
        float targetParam = Mathf.Clamp(GetOriginalSpeed() / walkSpeed, -1f, 1f);
        float temp = LinearEasing(animParam.walkSpeed, targetParam, Mathf.Abs(targetParam) >= Mathf.Abs(animParam.walkSpeed) ? 4f : 2f);
        if (animParam.walkSpeed != temp) {
            animParam.walkSpeed = temp;
            anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.WalkSpeed], animParam.walkSpeed);
        }
    }

    protected virtual void UpdateAC_Idle() {
        if (Time.timeScale > 0f && !rideTarget) {
            if (groundedFlag && GetCanControl() && !target && nowSpeed > -0.1f && nowSpeed < 0.1f && rotSpeed > -0.1f && rotSpeed < 0.1f) {
                idlingTime += deltaTimeMove;
                if (idlingTime > idlingTimeCondition) {
                    int restingMotion = GameManager.Instance.save.config[GameManager.Save.configID_RestingMotion];
                    if (restingMotion != 1) {
                        idlingTime = 0f;
                        idlingTimeCondition = Random.Range(9f, 11f);
                        if (restingMotion == 2) {
                            idleType = 0;
                        } else if (restingMotion == 3) {
                            idleType = 1;
                        } else {
                            idleType = Random.Range(0, idleTypeMax);
                        }
                        anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.IdleType], idleType);
                        anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.IdleMotion]);
                    }
                }
            } else {
                idlingTime = 0f;
                if (idleType >= 0) {
                    idleType = -1;
                    anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.IdleType], idleType);
                }
            }
        }
    }


    protected virtual void Update_AnimControl() {
        UpdateAC_Move();
        UpdateAC_AnimSpeed();
        if (isAnimParamDetail) {
            UpdateAC_Refresh();
            UpdateAC_Speed();
            UpdateAC_Run();
            UpdateAC_RotSpeed();
            UpdateAC_WalkSpeed();
        }
        if (isAnimIdleEnabled) {
            UpdateAC_Idle();
        }
    }

    protected virtual void Update_FaceControl() {
    }

    protected virtual void Update_Process() {
        switch (state) {
            case State.Spawn:
                Update_Process_Spawn();
                break;
            case State.Wait:
                Update_Process_Wait();
                break;
            case State.Chase:
                Update_Process_Chase();
                break;
            case State.Escape:
                Update_Process_Escape();
                break;
            case State.Dodge:
                Update_Process_Dodge();
                break;
            case State.Attack:
                Update_Process_Attack();
                break;
            case State.Damage:
                Update_Process_Damage();
                break;
            case State.Dead:
                Update_Process_Dead();
                break;
            case State.Jump:
                Update_Process_Jump();
                break;
            case State.Climb:
                Update_Process_Climb();
                break;
        }
    }

    protected virtual void Update() {
        if (CharacterManager.Instance) {
            Update_GetDelta();
            Update_StatusJudge();
            Update_Sick();
            Update_Targeting();
            Update_Transition();
            Update_PlayerControl();
            Update_Process();
            Update_MoveControl();
            Update_STControl();
            Update_AnimControl();
            Update_FaceControl();
            Update_TimeCount();
        }
    }

    protected virtual bool GetCanControl() {
        return !(disableControlTimeRemain > 0 || state == State.Spawn || state == State.Dodge || state == State.Attack || state == State.Damage || state == State.Dead || state == State.Climb);
    }

    public virtual bool GetCanMove() {
        return !(disableControlTimeRemain > 0 || state == State.Spawn || state == State.Dodge || state == State.Damage || state == State.Dead || state == State.Climb);
    }

    public virtual bool GetCanTakeDamage(bool penetrate = false) {
        return (nowHP > 0 && state != State.Dead && (penetrate || (mutekiTimeRemain <= 0f && state != State.Climb)));
    }
    public virtual bool GetIsMuteki() {
        return mutekiTimeRemain > 0f;
    }
    public virtual bool GetCanDodge() {
        return (state != State.Dodge && state != State.Damage && state != State.Dead && state != State.Climb && (attackingDodgeEnabled || state != State.Attack) && (mudToWalk || !GetSick(SickType.Mud)));
    }
    public virtual bool DodgeChallenge() {
        // return dodgeRemain > 0f && dodgePower > 0f && GetCanDodge() && Random.value <= dodgeRemain / dodgePower;
        return dodgeRemain > 0f && dodgePower > 0f && GetCanDodge() && Random.value <= dodgeRemain * 0.1f;
    }

    protected virtual void SetAttackPowerMultiplier(float pMul = 1f) {
        attackPowerMultiplier = pMul;
    }

    protected virtual void SetKnockPowerMultiplier(float kMul = 1f) {
        knockPowerMultiplier = kMul;
    }

    protected bool AttackBase(int type, float pMul, float kMul, float cost = 0, float stiff = 0.5f, float interval = 0.5f, float moveSpeedMultiplier = 1, float animSpeedMultiplier = 1, bool lockonOnStart = true, float lockonSpeed = -1) {
        if (!isPlayer || JudgeStamina(cost)) {
            attackType = type;
            anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.AttackSpeed], animSpeedMultiplier);
            anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.AttackType], attackType);
            anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Attack]);
            attackPowerMultiplier = pMul;
            knockPowerMultiplier = kMul;
            nowST -= cost;
            if (nowST < 0f) {
                nowST = 0f;
            }
            if (consumeStamina && cost > 0 && GetMaxST() < cost) {
                float powerdownRate = GetMaxST() / cost;
                attackPowerMultiplier *= powerdownRate;
                knockPowerMultiplier *= powerdownRate;
            }
            attackStiffTime = stiff;
            attackedTimeRemain = interval;
            attackIntervalSave = interval;
            if (setAttackIntervalToStaminaDontHeal) {
                staminaTreatAsAttackTimeRemain = interval;
            }
            if (moveSpeedMultiplier != 1f) {
                if (agent) {
                    if (isEnemy && moveSpeedMultiplier > 0f) {
                        agent.velocity *= moveSpeedMultiplier / (GameManager.Instance.minmiPurple ? 3f : notAffectedByRisky ? 1f : CharacterManager.Instance.riskyIncSqrt);
                    } else {
                        agent.velocity *= moveSpeedMultiplier;
                    }
                } else {
                    nowSpeed *= moveSpeedMultiplier;
                }
            }
            isLockon = lockonOnStart;
            lockonRotSpeed = (lockonSpeed >= 0 ? lockonSpeed : attackLockonDefaultSpeed);
            isSuperarmor = (isSuperman && superAttackSuperarmor);
            return true;
        } else {
            return false;
        }
    }

    protected virtual void Attack() {
    }

    protected virtual void AttackContinuous() {
        if (isLockon) {
            CommonLockon();
        }
    }

    protected virtual void Jump(float power) {
        if (state != State.Dead) {
            gravityMultiplier = 1f;
            move.y = power;
            disablizeAgentOnJump = true;
            SetState(State.Jump);
        }
    }

    protected virtual void ChangeStateReset() {
        if (quickJumping) {
            quickJumping = false;
        }
        if (resetAgentRadiusOnChangeState && agent.radius != defaultAgentRadius) {
            agent.radius = defaultAgentRadius;
        }
        if (resetAgentHeightOnChangeState && agent.height != defaultAgentHeight) {
            agent.height = defaultAgentHeight;
        }
    }

    protected void SetState(State targetState) {
        if (state != State.Dead) {
            stateTime = 0f;
            state = targetState;
            ChangeStateReset();
            StartState();
        }
    }

    protected virtual void StartState() {
        switch (state) {
            case State.Spawn:
                Start_Process_Spawn();
                break;
            case State.Wait:
                Start_Process_Wait();
                break;
            case State.Chase:
                Start_Process_Chase();
                break;
            case State.Escape:
                Start_Process_Escape();
                break;
            case State.Dodge:
                Start_Process_Dodge();
                break;
            case State.Attack:
                Start_Process_Attack();
                break;
            case State.Damage:
                Start_Process_Damage();
                break;
            case State.Dead:
                Start_Process_Dead();
                break;
            case State.Jump:
                Start_Process_Jump();
                break;
        }
    }

    protected virtual bool CommonLockon() {
        if (targetTrans && !GetSick(SickType.Stop)) {
            Vector3 diff = GetTargetVector(true, false);
            if (diff.sqrMagnitude > 0.01f) {
                float step = lockonRotSpeed * GetLockonRotSpeedRate() * deltaTimeCache;
                if (step > 0f) {
                    Quaternion targetRot = Quaternion.LookRotation(diff);
                    Quaternion nowRot = Quaternion.LookRotation(Vector3.RotateTowards(trans.TransformDirection(vecForward), diff, step * 0.15f * commonLockonTowardsMultiplier, 0f));
                    trans.rotation = Quaternion.Slerp(nowRot, targetRot, step * 0.45f);
                }
                return true;
            }
        }
        return false;
    }

    public void SpecificLockon(Vector3 position) {
        Vector3 diff = position - trans.position;
        diff.y = 0f;
        if (diff.sqrMagnitude > 0.01f) {
            float step = 10f * deltaTimeCache;
            Quaternion targetRot = Quaternion.LookRotation(diff);
            Quaternion nowRot = Quaternion.LookRotation(Vector3.RotateTowards(trans.TransformDirection(vecForward), diff, step * 0.15f * commonLockonTowardsMultiplier, 0f));
            trans.rotation = Quaternion.Slerp(nowRot, targetRot, step * 0.45f);
        }
    }

    protected void PerfectLockon() {
        if (targetTrans && !GetSick(SickType.Stop)) {
            Vector3 diff = GetTargetVector(true, false);
            if (diff.sqrMagnitude > 0.01f) {
                trans.rotation = Quaternion.LookRotation(diff);
            }
        }
    }

    protected void LockonStart() {
        isLockon = true;
    }
    protected void LockonEnd() {
        isLockon = false;
    }
    public virtual void CallBackGiveDamage(CharacterBase damageTaker, int damageNum) {
    }
    public static float CalcDamage(float attack, float defense) {
        if (attack < defense * 0.75f) {
            return attack / defense * (4f / 9f) * attack;
        } else {
            return attack - defense * 0.5f;
        }
    }
    public virtual float GetAttackNoEffected() {
        return attackPower;
    }
    public virtual float GetAttack(bool ignoreMultiplier = false) {
        return attackPower * (isSuperman ? superAttackRate : 1) * (GetSick(SickType.Frightened) ? 0.8f : 1) * (ignoreMultiplier ? 1 : attackPowerMultiplier);
    }
    public virtual float GetAttackPowerMultiplier() {
        return attackPowerMultiplier;
    }
    public virtual float GetDefenseNoEffected() {
        return defensePower;
    }
    public virtual float GetDefense(bool ignoreMultiplier = false) {
        return defensePower * (isSuperman ? superDefenseRate : 1) * (GetSick(SickType.Acid) ? 0.25f : 1) * (ignoreMultiplier ? 1 : defensePowerMultiplier);
    }
    public virtual float GetKnock(bool ignoreMultiplier = false) {
        return knockPower * (ignoreMultiplier ? 1 : knockPowerMultiplier) * (isSuperman ? superKnockRate : 1f);
    }
    public virtual float GetKnocked() {
        return 1f * (isSuperman ? superKnockedRate : 1f);
    }
    public virtual int GetNowHP() {
        return nowHP;
    }
    public virtual float GetNowST() {
        return nowST;
    }
    public virtual int GetMaxHP() {
        return maxHP;
    }
    public virtual int GetMaxHPNoEffected() {
        return maxHP;
    }
    public virtual float GetMaxST() {
        return maxST;
    }
    public virtual float GetMaxSTNoEffected() {
        return maxST;
    }
    public void SetNowHP(int num) {
        nowHP = num;
        if (num > 0 && state == State.Dead) {
            state = State.Spawn;
        }
    }
    public void SetNowST(float num) {
        nowST = num;
    }

    public virtual bool GetRunning() {
        return (nowSpeed > GetMaxSpeed(true, false, false, true));
    }
    public virtual bool GetWalking() {
        return (groundedFlag && nowSpeed <= GetMaxSpeed(true, false, false, true));
    }
    public virtual bool GetIdling() {
        return (groundedFlag && nowSpeed > -0.1f && nowSpeed < 0.1f && rotSpeed > -0.1f && rotSpeed < 0.1f);
    }
    protected virtual float GetSTHealRateChild_Normal() {
        return 0.15f;
    }
    protected virtual float GetSTHealRateChild_Attack() {
        return 0f;
    }
    protected virtual float GetSTHealRateChild_Jump() {
        return 0f;
    }
    protected virtual float GetSTHealRate() {
        float temp = 0f;
        if (groundedFlag && (state == State.Dodge || state == State.Attack || staminaTreatAsAttackTimeRemain > 0f)) {
            temp = GetSTHealRateChild_Attack();
        } else if (groundedFlag && (GetCanControl() || state == State.Damage || rideTarget)) {
            temp = GetSTHealRateChild_Normal();
        } else if (state == State.Jump && stateTime >= 0.3f && !isJumpingAttack) {
            temp = GetSTHealRateChild_Jump();
        }
        if (!isEnemy && GameManager.Instance.minmiBlue && temp > 0.025f) {
            temp = 0.025f + (temp - 0.025f) * 0.2f;
        }
        if (temp != 0f) {
            return (GetMaxST() * temp) * (isSuperman ? superSTRate : 1f) * (GetSick(SickType.Ice) ? 0f : 1f);
        } else {
            return 0f;
        }
    }
    protected virtual float GetDodgeHealRate() {
        if (state == State.Dodge) {
            return 0f;
        } else {
            return 0.1f;
        }
    }
    public float GetSickSpeedRate() {
        return (sickRemainTime[(int)SickType.Mud] > 0f ? 1f / 3f : 1f) * (sickRemainTime[(int)SickType.Slow] > 0f ? 0.5f : 1f) * (sickRemainTime[(int)SickType.Stop] > 0f ? 0f : 1f);
    }
    public virtual float GetMaxSpeed(bool isWalk = false, bool ignoreSuperman = false, bool ignoreSick = false, bool ignoreConfig = false) {
        return (isWalk ? walkSpeed : maxSpeed)
            * (isSuperman && !ignoreSuperman ? superSpeedRate : 1f)
            * (ignoreSick ? 1f : GetSickSpeedRate());
    }
    public virtual float GetJumpPower() {
        return 8.3f;
    }
    public virtual float GetOriginalSpeed() {
        float rate = (GetSick(SickType.Mud) ? 1f / 3f : 1f) * (GetSick(SickType.Slow) ? 0.5f : 1f);
        if (rate == 1f) {
            return nowSpeed;
        } else {
            return nowSpeed / rate;
        }
    }
    public virtual float GetAcceleration() {
        return acceleration * (isSuperman ? superAccelerationRate : 1f) * (GetSick(SickType.Ice) ? 0.25f : 1f) * (GetSick(SickType.Stop) ? 5f : 1f);
    }
    public virtual float GetAngularSpeed() {
        return angularSpeed * (isSuperman ? superAngularRate : 1f);
    }
    public virtual float GetLockonRotSpeedRate() {
        return (GetSick(SickType.Mud) ? 0.2f : 1f) * (GetSick(SickType.Slow) ? 0.5f : 1f);
    }
    protected virtual float GetHeavyStiffTime() {
        return knockRestoreSpeed == 1f ? heavyStiffTime : knockRestoreSpeed > 0f ? heavyStiffTime / knockRestoreSpeed : float.MaxValue;
    }
    protected virtual float GetLightStiffTime() {
        return knockRestoreSpeed == 1f ? lightStiffTime : knockRestoreSpeed > 0f ? lightStiffTime / knockRestoreSpeed : float.MaxValue;
    }
    public virtual float GetLightKnockEndurance() {
        return knockEnduranceLight;
    }
    public virtual float GetHeavyKnockEndurance() {
        return knockEndurance;
    }
    public virtual float GetDodgeRemain() {
        return dodgeRemain;
    }
    public float GetDodgePower() {
        return dodgePower;
    }

    public virtual void HitAttackAdditiveProcess() { }

    public virtual void HitAttackAdditiveProcessDD(ref DamageDetection targetDD) { }

    public virtual void AddNowHP(int num, Vector3 position, bool showDamage = true, int colorType = 0, CharacterBase attacker = null) {
        if (nowHP > 0 && num != 0) {
            nowHP += num;
            if (showDamage) {
                if (!(((!isEnemy && !isPlayer) || (attacker && !attacker.isEnemy && !attacker.isPlayer)) && GameManager.Instance.save.config[GameManager.Save.configID_ObscureFriends] >= 3)) {
                    CharacterManager.Instance.ShowDamage(ref position, num >= 0 ? num : -num, colorType, isPlayer || (attacker == null && isEnemy) || (attacker && attacker.isPlayer));
                }
                lastDamagedColorType = colorType;
            }
            if (nowHP > GetMaxHP()) {
                nowHP = GetMaxHP();
            } else if (nowHP < 0) {
                nowHP = 0;
            }
        }
    }

    protected virtual void DamageCommonProcess() { }

    protected virtual void KnockLightProcess() {
        if (!isSuperarmor) {
            isDamageHeavy = false;
            knockRemainLight = GetLightKnockEndurance();
            lightKnockCount++;
            if (lightKnockTime > 0 && lightKnockMove > 0) {
                SetSpecialMove(knockDirection, lightKnockMove, lightKnockTime, EasingType.QuadOut);
            }
            if (GetLightStiffTime() > 0) {
                anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockLight]);
                SetState(State.Damage);
                ResetTriggerOnDamage();
                disableAttackStartTimeRemain = 0.2f;
            }
        }
    }

    protected virtual void KnockHeavyProcess() {
        isDamageHeavy = true;
        knockRemain = GetHeavyKnockEndurance();
        knockRemainLight = GetLightKnockEndurance();
        if (heavyMutekiTime > 0f) {
            SetMutekiTime(heavyMutekiTime);
        }
        heavyKnockCount++;
        if (move.y > 0f) {
            move.y = 0f;
        }
        if (heavyKnockTime > 0 && heavyKnockMove > 0) {
            SetSpecialMove(knockDirection, heavyKnockMove, heavyKnockTime, EasingType.QuadOut);
        }
        if (GetHeavyStiffTime() > 0) {
            anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockHeavy]);
            SetState(State.Damage);
            ResetTriggerOnDamage();
            disableAttackStartTimeRemain = 0.2f;
        }
    }

    protected virtual void JudgeResurrection() {
    }

    protected virtual void ReportDamage(CharacterBase attacker, int damage) {
        CharacterManager.Instance.damageRecord.time = GameManager.Instance.time;
        CharacterManager.Instance.damageRecord.attacker = attacker;
        CharacterManager.Instance.damageRecord.receiver = this;
    }

    protected virtual void DodgePowerDamageHeal(int damage) {
        if (dodgeRemain < dodgeDamageHealMax) {
            dodgeRemain += dodgePower * damage / Mathf.Max(GetMaxHP(), 1f);
            if (dodgeRemain > dodgeDamageHealMax) {
                dodgeRemain = dodgeDamageHealMax;
            }
        }
    }

    public virtual void TakeDamage(int damage, Vector3 position, float knockAmount, Vector3 knockVector, CharacterBase attacker = null, int colorType = 0, bool penetrate = false) {
        if (GetCanTakeDamage(penetrate)) {
            attackerCB = attacker;
            if (attacker) {
                attackerObj = attacker.gameObject;
                attacker.CallBackGiveDamage(this, damage);
                if (damageReportEnabled) {
                    ReportDamage(attacker, damage);
                }
            }
            float knockedRateTemp = GetKnocked();
            if (knockedRateTemp == 1f) {
                knockRemain -= knockAmount;
                knockRemainLight -= knockAmount;
                lastKnockedSave = knockAmount;
            } else if (knockedRateTemp >= 1000f) {
                lastKnockedSave = 0f;            
            } else if (knockedRateTemp > 0f) {
                float knockTemp = knockAmount / knockedRateTemp;
                knockRemain -= knockTemp;
                knockRemainLight -= knockTemp;
                lastKnockedSave = knockTemp;
            } else {
                knockRemain = 0f;
                knockRemainLight = 0f;
                lastKnockedSave = knockAmount;
            }
            if (cannotKnockDown) {
                if (knockRemain < 1f) {
                    knockRemain = 1f;
                }
                if (knockRemainLight < 1f) {
                    knockRemainLight = 1f;
                }
            }
            knockDirection = knockVector;
            AddNowHP(-damage, position, true, colorType, attacker);
            if (isPlayer) {
                JudgeResurrection();
            }
            DamageCommonProcess();
            if (nowHP > 0) {
                if (dodgePower >= 1f && dodgePower < 100f && damage > 0) {
                    DodgePowerDamageHeal(damage);
                }
                if (attacker || attackerObj) {
                    for (int i = 0; i < searchArea.Length; i++) {
                        if (searchArea[i]) {
                            if (lockTargetTime > 0 && targetFixingTimeRemain <= 0f) {
                                if (attacker) {
                                    searchArea[i].SetLockTargetFromCharacter(attacker, lockTargetTime);
                                } else {
                                    searchArea[i].SetLockTarget(attackerObj, lockTargetTime);
                                }
                            }
                            if (watchoutTime > 0f) {
                                searchArea[i].SetWatchout(watchoutTime);
                            }
                        }
                    }
                }
                if (knockRemain > 0f) {
                    if (lightMutekiTime > 0f) {
                        SetMutekiTime(lightMutekiTime);
                    }
                    if ((mustKnockBackFlag || knockRemainLight <= 0f) && !(state == State.Damage && isDamageHeavy && stateTime < GetHeavyStiffTime())) {
                        KnockLightProcess();
                    }
                } else {
                    KnockHeavyProcess();
                }
            } else {
                SetState(State.Dead);
            }
        }
    }

    public void SetSearchTargetActive(bool flag = true) {
        for (int i = 0; i < searchTarget.Length; i++) {
            if (searchTarget[i]) {
                searchTarget[i].SetActive(flag);
            }
        }
    }

    public bool GetSearchTargetActive() {
        for (int i = 0; i < searchTarget.Length; i++) {
            if (searchTarget[i] && searchTarget[i].activeSelf) {
                return true;
            }
        }
        return false;
    }

    public GameObject[] GetSearchTarget() {
        return searchTarget;
    }

    public virtual bool CheckGrounded(float tolerance = 0.5f) {
        if (cCon.isGrounded) {
            return true;
        } else if (move.y <= 0.1f) {
            ray.origin = trans.position + vecUp * checkGroundPivotHeight;
            ray.direction = vecDown;
            if (Physics.Raycast(ray, tolerance, checkGroundedLayerMask, QueryTriggerInteraction.Ignore)) {
                return true;
            }
        }
        return false;
    }

    public virtual bool CheckNearNavMesh(float maxDistance = 0.5f) {
        if (NavMesh.SamplePosition(trans.position, out navMeshHit, maxDistance, NavMesh.AllAreas)) {
            return true;
        }
        return false;
    }

    protected virtual void SideStep_Move(Vector3 dir, float distance = 5f) {
        SetSpecialMove(dir, distance, dodgeStiffTime, EasingType.SineInOut);
    }

    protected virtual void SideStep_Head(int direction, float mutekiTime = 0f, bool changeState = true) {
        SetMutekiTime(mutekiTime);
        if (dodgePower < 100f) {
            dodgeRemain -= dodgeCost;
        }
        if (consumeStamina) {
            nowST -= GetCost(CostType.Step);
        }
        if (changeState) {
            SetState(State.Dodge);
            ReleaseAttackDetections();
            if (throwing && throwingCancelOnDamage) {
                throwing.ThrowCancelAll(true);
            }
            anim.ResetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Landing]);
            anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.SideStep]);
            anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.StepDirection], direction);
        }
        sideStepDirection = direction;
    }

    public virtual void SideStep(int direction, float distance = 5f, float mutekiTime = 0f, bool changeState = true) {
        SideStep_Head(direction, mutekiTime, changeState);
        Vector3 dirVector = vecZero;
        switch (direction) {
            case -1:
                dirVector = trans.TransformDirection(Vector3.left);
                break;
            case 0:
                dirVector = trans.TransformDirection(Vector3.back);
                break;
            case 1:
                dirVector = trans.TransformDirection(Vector3.right);
                break;
            case 2:
                dirVector = trans.TransformDirection(Vector3.forward);
                break;
        }
        SideStep_Move(dirVector, distance);
    }

    public virtual void SideStep_Vector(Vector3 moveDir, int animDir, float distance = 5f, float mutekiTime = 0f, bool changeState = true) {
        SideStep_Head(animDir, mutekiTime, changeState);
        SideStep_Move(moveDir, distance);
    }

    public virtual void SideStep_ConsiderWall(int direction, float distance = 5f, float mutekiTime = 0f, bool changeState = true) {
        Vector3 dirVector = vecZero;
        switch (direction) {
            case -1:
                dirVector = trans.TransformDirection(Vector3.left);
                break;
            case 0:
                dirVector = trans.TransformDirection(Vector3.back);
                break;
            case 1:
                dirVector = trans.TransformDirection(Vector3.right);
                break;
            case 2:
                dirVector = trans.TransformDirection(Vector3.forward);
                break;
        }
        ray.origin = GetCenterPosition();
        ray.direction = dirVector;
        if (Physics.Raycast(ray, distance * 0.8f, wallLayerMask, QueryTriggerInteraction.Collide)) {
            switch (direction) {
                case -1:
                    direction = 1;
                    break;
                case 0:
                    direction = 2;
                    break;
                case 1:
                    direction = -1;
                    break;
                case 2:
                    direction = 0;
                    break;
            }
        }
        SideStep(direction, distance, mutekiTime, changeState);
    }

    public virtual void CounterDodge(int dodgeDir, bool changeState = true) {
        SideStep_ConsiderWall(dodgeDir, dodgeDistance, dodgeMutekiTime, changeState);
    }

    protected virtual bool JudgeStamina(float staminaCost) {
        return (nowST >= staminaCost || nowST >= GetMaxST());
    }

    public virtual void CounterAttack(GameObject attackerObject = null, bool isProjectile = false) {
        this.attackerObj = attackerObject;
    }

    public virtual void SpecialStep(float distanceToTarget, float stepTime = 0.25f, float maxDist = 4f, float minDist = 0f, float defaultDist = 0f, bool ignoreY = true, bool considerTargetRadius = true, EasingType easingType = EasingType.SineInOut, bool directionAdjustEnabled = false) {
        Vector3 targetPos = trans.position;
        float dist = defaultDist;
        float speedRate = isEnemy && GameManager.Instance.minmiPurple ? 3f : 1f;
        if (targetTrans) {
            dist = Mathf.Clamp(GetTargetDistance(false, ignoreY, considerTargetRadius) - distanceToTarget, minDist, maxDist * speedRate);
            targetPos = targetTrans.position;
        } else {
            targetPos = trans.position + trans.TransformDirection(vecForward);
        }
        if (ignoreY) {
            targetPos.y = trans.position.y;
        }
        if (dist > 0f) {
            SetSpecialMove((targetPos - trans.position).normalized, dist, stepTime, easingType, null, directionAdjustEnabled);
        }
    }

    public virtual void StepToTarget(float distanceToTarget) {
        if (targetTrans) {
            float dist = GetTargetDistance(false, fbStepIgnoreY, fbStepConsiderRadius);
            float speedRate = isEnemy && GameManager.Instance.minmiPurple ? 3f : 1f;
            if (dist > distanceToTarget) {
                SetSpecialMove(GetTargetVector(true, true, false), Mathf.Min(dist - distanceToTarget, fbStepMaxDist * speedRate), fbStepTime, fbStepEaseType);
            }
        }
    }

    public virtual void SeparateFromTarget(float distanceToTarget) {
        if (targetTrans) {
            float dist = GetTargetDistance(false, fbStepIgnoreY, fbStepConsiderRadius);
            float speedRate = isEnemy && GameManager.Instance.minmiPurple ? 3f : 1f;
            if (dist < distanceToTarget) {
                SetSpecialMove(GetTargetVector(true, true, true), Mathf.Min(distanceToTarget - dist, fbStepMaxDist * speedRate), fbStepTime, fbStepEaseType);
            }
        }
    }

    public virtual void ApproachOrSeparate(float distanceToTarget) {
        if (targetTrans) {
            float dist = GetTargetDistance(false, fbStepIgnoreY, fbStepConsiderRadius);
            float speedRate = isEnemy && GameManager.Instance.minmiPurple ? 3f : 1f;
            if (dist > distanceToTarget) {
                SetSpecialMove(GetTargetVector(true, true, false), Mathf.Min(dist - distanceToTarget, fbStepMaxDist * speedRate), fbStepTime, fbStepEaseType);
            } else if (dist < distanceToTarget) {
                SetSpecialMove(GetTargetVector(true, true, true), Mathf.Min(distanceToTarget - dist, fbStepMaxDist * speedRate), fbStepTime, fbStepEaseType);
            }
        }
    }

    public virtual void ForwardStep(float distanceToTarget) {
        if (targetTrans) {
            float dist = GetTargetDistance(false, fbStepIgnoreY, fbStepConsiderRadius);
            float speedRate = isEnemy && GameManager.Instance.minmiPurple ? 3f : 1f;
            if (dist > distanceToTarget) {
                SetSpecialMove(trans.TransformDirection(vecForward), Mathf.Min(dist - distanceToTarget, fbStepMaxDist * speedRate), fbStepTime, fbStepEaseType);
            }
        }
    }

    public virtual void BackStep(float distanceToTarget) {
        if (targetTrans) {
            float dist = GetTargetDistance(false, fbStepIgnoreY, fbStepConsiderRadius);
            float speedRate = isEnemy && GameManager.Instance.minmiPurple ? 3f : 1f;
            if (dist < distanceToTarget) {
                SetSpecialMove(trans.TransformDirection(vecBack), Mathf.Min(distanceToTarget - dist, fbStepMaxDist * speedRate), fbStepTime, fbStepEaseType);
            }
        }
    }

    public virtual void ForwardOrBackStep(float distanceToTarget) {
        if (targetTrans) {
            float dist = GetTargetDistance(false, fbStepIgnoreY, fbStepConsiderRadius);
            float speedRate = isEnemy && GameManager.Instance.minmiPurple ? 3f : 1f;
            if (dist > distanceToTarget) {
                SetSpecialMove(trans.TransformDirection(vecForward), Mathf.Min(dist - distanceToTarget, fbStepMaxDist * speedRate), fbStepTime, fbStepEaseType);
            } else if (dist < distanceToTarget) {
                SetSpecialMove(trans.TransformDirection(vecBack), Mathf.Min(distanceToTarget - dist, fbStepMaxDist * speedRate), fbStepTime, fbStepEaseType);
            }
        }
    }

    protected virtual float GetCost(CostType type) {
        float answer = 0f;
        switch (type) {
            case CostType.Run:
                answer = moveCost.run;
                break;
            case CostType.Attack:
                answer = moveCost.attack;
                break;
            case CostType.Step:
                answer = moveCost.step;
                break;
            case CostType.Quick:
                answer = moveCost.quick;
                break;
            case CostType.Jump:
                answer = moveCost.jump;
                break;
            case CostType.Skill:
                answer = moveCost.skill;
                break;
        }
        return answer * (isSuperman ? superCostRate : 1f);
    }

    public void ConsiderHitAttackDetection(int index, ref DamageDetection targetDD) {
        if (index >= 0 && index < attackDetection.Length && attackDetection[index]) {
            attackDetection[index].ConsiderHit(ref targetDD, false);
        }
    }

    protected float LinearEasing(float start, float end, float speed) {
        if (start < end) {
            start += deltaTimeMove * speed;
            if (start > end) {
                start = end;
            }
        } else if (start > end) {
            start -= deltaTimeMove * speed;
            if (start < end) {
                start = end;
            }
        }
        return start;
    }

    public virtual void ForceAttackStart(int index) {
        if (index < attackDetection.Length && attackDetection[index]) {
            attackDetection[index].DetectionStart(attackEffectEnabled);
            anyAttackEnabled = true;
        }
    }

    public virtual void AttackStart(int index) {
        if (enabled && state != State.Damage && disableAttackStartTimeRemain <= 0f && index < attackDetection.Length && attackDetection[index]) {
            attackDetection[index].DetectionStart(attackEffectEnabled);
            anyAttackEnabled = true;
        }
    }

    public virtual void AttackEnd(int index) {
        if (enabled && index < attackDetection.Length && attackDetection[index]) {
            attackDetection[index].DetectionEnd(attackEffectEnabled);
        }
    }

    void ActivateXWeaponTrails(int index) {
        if (enabled && state != State.Damage && disableAttackStartTimeRemain <= 0f && index < attackDetection.Length && attackDetection[index]) {
            attackDetection[index].ActivateXWeaponTrails();
            anyAttackEnabled = true;
        }
    }

    protected virtual void ReleaseAttackDetections() {
        if (anyAttackEnabled) {
            for (int i = 0; i < attackDetection.Length; i++) {
                if (attackDetection[i] && attackDetection[i].attackEnabled && !attackDetection[i].unleashed) {
                    attackDetection[i].DetectionEnd(false);
                }
            }
            anyAttackEnabled = false;
        }
    }

    protected virtual void ResetTriggerOnDamage() {
        if (anim) {
            anim.ResetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.QuickTurn]);
            anim.ResetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.SideStep]);
            anim.ResetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Jump]);
            anim.ResetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Landing]);
            anim.ResetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Attack]);
            if (isAnimIdleEnabled) {
                if (rideTarget && (!rideContinueOnDamage || nowHP <= 0)) {
                    RemoveRide();
                }
                anim.ResetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.IdleMotion]);
                if (idleType >= 0 && !rideTarget) {
                    idleType = -1;
                    anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.IdleType], idleType);
                }
            }
        }
        ReleaseAttackDetections();
        if (attackedTimeRemain > attackedTimeRemainOnDamage) {
            attackedTimeRemain = attackedTimeRemainOnDamage;
        }
        if (!notResetAttackProcessOnDamage) {
            attackProcess = 0;
        }
        if (throwing && throwingCancelOnDamage) {
            throwing.ThrowCancelAll(true);
        }
    }

    public void SuperarmorStart() {
        isSuperarmor = true;
    }

    public virtual void SuperarmorEnd() {
        isSuperarmor = false;
    }

    protected void SetForChangeMatSet(ChangeMatSet matSet, bool flag) {
        if (matSet.changeMatRenderer) {
            Material[] matsTemp = matSet.changeMatRenderer.materials;
            if (matSet.materialIndex < matsTemp.Length) {
                if (flag) {
                    matsTemp[matSet.materialIndex] = matSet.specialMaterial;
                    if (matSet.bringRenderQueue) {
                        matsTemp[matSet.materialIndex].renderQueue = matSet.defaultMaterial.renderQueue;
                    }
                } else {
                    matsTemp[matSet.materialIndex] = matSet.defaultMaterial;
                }
                matSet.changeMatRenderer.materials = matsTemp;
            }
        }
    }

    public virtual void SupermanSetMaterial(bool flag) {
        for (int i = 0; i < supermanSettings.mats.Length; i++) {
            SetForChangeMatSet(supermanSettings.mats[i], flag);
        }
        supermanSettings.isSpecial = flag;
    }

    public virtual void SupermanSetObj(bool flag) {
        for (int i = 0; i < supermanSettings.obj.Length; i++) {
            if (supermanSettings.obj[i]) {
                supermanSettings.obj[i].SetActive(flag);
            }
        }
    }

    public virtual void SupermanStart(bool effectEnable = true) {
        if (!isSuperman) {
            isSuperman = true;
            supermanTime = 0f;
            SupermanSetObj(true);
            SupermanSetMaterial(true);
        }
    }

    public virtual void SupermanEnd(bool effectEnable = true) {
        if (isSuperman) {
            isSuperman = false;
            supermanTime = 0f;
            SupermanSetObj(false);
            SupermanSetMaterial(false);
            S_ParticleStopAll();
        }
    }

    public void ChangeSpeed(float vMul) {
        if (agent) {
            agent.velocity *= vMul;
        } else {
            nowSpeed *= vMul;
        }
    }

    protected void SetSpecialMove(Vector3 direction, float distance, float duration, EasingType easingType, VoidEvent completeEvent = null, bool directionAdjustEnabled = false) {
        if (!GetSick(SickType.Stop)) {
            specialMoveDirection = direction;
            specialMoveDistance = distance;
            specialMoveDuration = duration;
            specialMoveEasingType = easingType;
            specialMoveLastPoint = 0f;
            specialMoveElapsedTime = 0f;
            specialMoveCompleteEvent = completeEvent;
            specialMoveDirectionAdjustEnabled = directionAdjustEnabled;
        } else {
            specialMoveDuration = 0f;
        }
    }

    protected void Update_SpecialMove() {
        if (specialMoveElapsedTime < specialMoveDuration && specialMoveDuration > 0f && deltaTimeMove > 0f) {
            if (specialMoveDirectionAdjustEnabled && targetTrans) {
                Vector3 targetPos = targetTrans.position;
                targetPos.y = trans.position.y;
                specialMoveDirection = (targetPos - trans.position).normalized;
            }
            specialMoveElapsedTime += deltaTimeMove;
            specialMovePointTemp = Easing.GetEasing(specialMoveEasingType, specialMoveElapsedTime, specialMoveDuration, 0f, 1f);
            specialMoveVectorTemp = specialMoveDirection * ((specialMovePointTemp - specialMoveLastPoint) * specialMoveDistance);
            if (specialMoveVectorTemp != vecZero) {
                cCon.Move(specialMoveVectorTemp);
            }
            specialMoveLastPoint = specialMovePointTemp;
            if (specialMoveElapsedTime >= specialMoveDuration && specialMoveCompleteEvent != null) {
                specialMoveCompleteEvent.Invoke();
                specialMoveCompleteEvent = null;
            }
        }
    }

    public void EmitEffectObject(GameObject effectPrefab) {
        if (effectPrefab) {
            Instantiate(effectPrefab, trans.position, quaIden);
        }
    }

    public void EmitEffectObjectParenting(GameObject effectPrefab) {
        if (effectPrefab) {
            Instantiate(effectPrefab, trans);
        }
    }    

    public virtual void EmitEffect(int index) {
        if (enabled && index >= 0 && index < effect.Length && effect[index].prefab) {
            if (effect[index].pivot) {
                effect[index].instance = Instantiate(effect[index].prefab, effect[index].pivot.position + effect[index].offset, effect[index].pivot.rotation, effect[index].parenting ? effect[index].pivot : null);
            } else {
                effect[index].instance = Instantiate(effect[index].prefab, trans.position + effect[index].offset, trans.rotation, effect[index].parenting ? trans : null);
            }
            if (!isEnemy && !isPlayer && CharacterManager.Instance) {
                float obscureRate = CharacterManager.Instance.GetObscureRateAudio();
                if (obscureRate < 1f) {
                    AudioSource[] audioSources = effect[index].instance.GetComponentsInChildren<AudioSource>();
                    for (int i = 0; i < audioSources.Length; i++) {
                        audioSources[i].volume *= obscureRate;
                    }
                }
            }
        }
    }

    public void EmitEffectClass(Effect effect) {
        if (effect.prefab) {
            if (effect.pivot) {
                effect.instance = Instantiate(effect.prefab, effect.pivot.position + effect.offset, effect.pivot.rotation, effect.parenting ? effect.pivot : null);
            } else {
                effect.instance = Instantiate(effect.prefab, trans.position + effect.offset, trans.rotation, effect.parenting ? trans : null);
            }
            if (!isEnemy && !isPlayer && CharacterManager.Instance) {
                float obscureRate = CharacterManager.Instance.GetObscureRateAudio();
                if (obscureRate < 1f) {
                    AudioSource[] audioSources = effect.instance.GetComponentsInChildren<AudioSource>();
                    for (int i = 0; i < audioSources.Length; i++) {
                        audioSources[i].volume *= obscureRate;
                    }
                }
            }
        }
    }

    public virtual void EmitEffectString(string type) {
    }

    public void DestroyEffect(int index) {
        if (index >= 0 && index < effect.Length && effect[index].instance) {
            Destroy(effect[index].instance);
            effect[index].instance = null;
        }
    }

    public void AnimationStopTiming() {
        isAnimStopped = true;
    }
    public void AnimationStopCancel() {
        isAnimStopped = false;
    }

    public virtual void ResetAgent() {
        if (agent && agent.enabled && agent.isOnNavMesh) {
            agent.ResetPath();
        }
        ResetTriggerOnDamage();
        specialMoveDuration = 0f;
        move = vecZero;
        nowSpeed = 0f;
        rotSpeed = 0f;
    }

    public void AgentOffOn() {
        if (agent && agent.enabled) {
            agent.enabled = false;
            agent.enabled = true;
        }
    }

    public virtual void SetForItem() {
        for (int i = 0; i < attackDetection.Length; i++) {
            if (attackDetection[i]) {
                attackDetection[i].gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < searchArea.Length; i++) {
            if (searchArea[i]) {
                searchArea[i].gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < searchTarget.Length; i++) {
            if (searchTarget[i]) {
                searchTarget[i].SetActive(false);
            }
        }
        if (agent) {
            agent.enabled = false;
        } else {
            NavMeshAgent agentTemp = GetComponent<NavMeshAgent>();
            if (agentTemp) {
                agentTemp.enabled = false;
            }
        }
        if (cCon) {
            cCon.enabled = false;
        } else {
            CharacterController cConTemp = GetComponent<CharacterController>();
            if (cConTemp) {
                cConTemp.enabled = false;
            }
        }
        isItem = true;
        enabled = false;
    }

    public bool GetSick(SickType sickType) {
        return sickRemainTime[(int)sickType] > 0;
    }

    public bool GetAnySick() {
        for (int i = 0; i < sickRemainTime.Length; i++) {
            if (sickRemainTime[i] > 0) {
                return true;
            }
        }
        return false;
    }

    public virtual void SetSick(SickType sickType, float duration, AttackDetection attacker = null) {
        if (duration > 0 && sickRemainTime[(int)sickType] <= 0) {
            sickEffectInterval[(int)sickType] = Mathf.Clamp(sickEffectInterval[(int)sickType], 0f, 2f);
        }
        if (sickType == SickType.Stop && duration > 0) {
            specialMoveDuration = 0f;
        }
        if (duration <= 0 || duration > sickRemainTime[(int)sickType]) {
            sickRemainTime[(int)sickType] = duration;
        }
        if (targetHateEnabled && attacker && CharacterManager.Instance) {
            RegisterTargetHate(attacker.parentCBase, CharacterManager.Instance.GetNormalKnockAmount() * (sickType == SickType.Stop ? 4f : 2f));
        }
    }

    public void ClearSick() {
        for (int i = 0; i < sickRemainTime.Length; i++) {
            sickRemainTime[i] = 0;
        }
    }

    public virtual void ForceStopForEvent(float time) {
        ResetAgent();
        disableControlTimeRemain = time;
        mutekiTimeRemain = time;
    }

    public virtual void ReleaseStopForEvent() {
        disableControlTimeRemain = 0f;
        mutekiTimeRemain = 0f;
    }

    public virtual void Warp(Vector3 position, float disableControlTime, float gravityZeroTime) {
        if (state == State.Climb) {
            SetState(State.Wait);
        }
        for (int i = 0; i < searchArea.Length; i++) {
            searchArea[i].SetUnlockTarget();
        }
        if (agent && agent.enabled) {
            if (agent.pathStatus != NavMeshPathStatus.PathInvalid) {
                agent.ResetPath();
            }
            agent.enabled = false;
        }
        if (disableControlTimeRemain < disableControlTime) {
            disableControlTimeRemain = disableControlTime;
        }
        if (gravityZeroTimeRemain < gravityZeroTime) {
            gravityZeroTimeRemain = gravityZeroTime;
        }
        if (!isJumpingAttack) {
            move = vecZero;
        }
        trans.position = position;
        if (rideTarget && (rideTarget.position - trans.position).sqrMagnitude > 2f * 2f) {
            RemoveRide();
        }
    }

    public void ReleaseWarpStan() {
        disableControlTimeRemain = 0f;
        gravityZeroTimeRemain = 0f;
    }

    public void RandomWarp(Vector3 offset, bool nearGoal = false, string targetTag = "Respawn", float disableControlTime = 0f, float gravityZeroTime = 0f) {
        if (StageManager.Instance && StageManager.Instance.dungeonController) {
            Vector3 warpPos = StageManager.Instance.dungeonController.GetRespawnPos(targetTag);
            /*
            if (searchFar) {
                Vector3 pivotPos = trans.position;
                float maxDist = (warpPos - pivotPos).sqrMagnitude;
                for (int i = 0; i < 8; i++) {
                    Vector3 tempPos = StageManager.Instance.dungeonController.GetRespawnPos(targetTag);
                    float tempDist = (tempPos - pivotPos).sqrMagnitude;
                    if (tempDist > maxDist) {
                        warpPos = tempPos;
                        maxDist = tempDist;
                    }
                }
            }
            */
            if (nearGoal) {
                GameObject goalInstance = StageManager.Instance.dungeonController.GetGoalInstance();
                if (goalInstance) {
                    Vector3 goalPos = goalInstance.transform.position;
                    float minDist = (warpPos - goalPos).sqrMagnitude;
                    for (int i = 0; i < 15; i++) {
                        Vector3 tempPos = StageManager.Instance.dungeonController.GetRespawnPos(targetTag);
                        float tempDist = (tempPos - goalPos).sqrMagnitude;
                        if (tempDist < minDist) {
                            warpPos = tempPos;
                            minDist = tempDist;
                        }
                    }
                }
            }
            Warp(warpPos + offset, disableControlTime, gravityZeroTime);
        }
    }

    public virtual void BlownAway(bool disableControl = true, float height = 5f, bool nearGoal = false, string targetTag = "Respawn") {
        if (disableControl) {
            disableControlTimeRemain = 0.5f;
            gravityZeroTimeRemain = 0.5f;
        } else {
            disableControlTimeRemain = 0f;
            gravityZeroTimeRemain = 0f;
        }
        move.y = 0f;
        RandomWarp(vecUp * height, nearGoal, targetTag, disableControlTimeRemain, gravityZeroTimeRemain);
    }

    public void DamageNeedle() {
        if (CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.Needle) == 0) {
            TakeDamage(Mathf.Min(nowHP - 1, (int)Mathf.Ceil(CharacterManager.Instance.GetPlayerMaxHPNoEffected() * CharacterManager.Instance.GetDifficultyEffect(CharacterManager.DifficultyEffect.NeedleDamage) * (CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.ReduceSlipDamage) != 0 ? 0.75f : 1f) / Mathf.Max(sickHealSpeed, 1f))), GetCenterPosition(), 50, vecZero, null, 0);
        }
    }

    protected void Continuous_Approach(float baseSpeed = 10f, float brakeDistance = 0.5f, float stopDistance = 0.01f, bool ignoreY = true, bool considerRadius = false, float brakingSpeedRate = 0.5f) {
        if (targetTrans && deltaTimeMove > 0f && !GetSick(SickType.Stop)) {
            float sqrDist = GetTargetDistance(true, ignoreY, considerRadius);
            if (sqrDist > stopDistance) {
                float speed = baseSpeed;
                float brakeStopDiff = brakeDistance - stopDistance;
                if (GetSick(SickType.Mud)) {
                    speed *= 1f / 3f;
                } else if (!GetSick(SickType.Slow) && brakeStopDiff > 0f && sqrDist < brakeDistance * brakeDistance) {
                    float dist = Vector3.Distance(trans.position, targetTrans.position);
                    speed *= Mathf.Lerp(brakingSpeedRate, 1f, (dist - stopDistance) / brakeStopDiff);
                }
                cCon.Move(GetTargetVector(true, true) * speed * deltaTimeMove);
            }
        }
    }

    protected void ApproachTransformPivot(Transform pivot, float baseSpeed = 10f, float brakeDistance = 0.5f, float stopDistance = 0.01f, bool ignoreY = true, float brakingSpeedRate = 0.5f) {
        if (targetTrans && pivot && deltaTimeMove > 0f && !GetSick(SickType.Stop)) {
            Vector3 targetPosTemp = targetTrans.position;
            Vector3 pivotPosTemp = pivot.position;
            if (ignoreY) {
                pivotPosTemp.y = targetPosTemp.y = 0f;
            }
            float sqrDist = (targetPosTemp - pivotPosTemp).sqrMagnitude;
            if (sqrDist > stopDistance * stopDistance) {
                float speed = baseSpeed;
                float brakeStopDiff = brakeDistance - stopDistance;
                if (GetSick(SickType.Mud)) {
                    speed *= 1f / 3f;
                } else if (!GetSick(SickType.Slow) && brakeStopDiff > 0f && sqrDist < brakeDistance * brakeDistance) {
                    float dist = Vector3.Distance(targetPosTemp, pivotPosTemp);
                    speed *= Mathf.Lerp(brakingSpeedRate, 1f, (dist - stopDistance) / brakeStopDiff);
                }
                cCon.Move((targetPosTemp - pivotPosTemp).normalized * speed * deltaTimeMove);
            }
        }
    }

    protected void SetTransformPositionToGround(Transform origin, Transform setTarget, float tolerance) {
        if (origin && setTarget) {
            Vector3 originPos = origin.position;
            ray.origin = originPos;
            ray.direction = vecDown;
            if (Physics.Raycast(ray, out raycastHit, tolerance, fieldLayerMask, QueryTriggerInteraction.Ignore)) {
                setTarget.position = raycastHit.point;
            } else {
                originPos.y -= 0.5f;
                setTarget.position = originPos;
            }
        }
    }

    public bool IsAttacking() {
        return (state == State.Attack);
    }

    public float GetNowSpeed() {
        return nowSpeed;
    }

    public void SetCommand(Command newCommand) {
        command = newCommand;
        if (command == Command.Free && (state == State.Wait || state == State.Jump || state == State.Spawn) && roveInterval >= 1f) {
            rovingTime = roveInterval - 1f;
        }
    }

    public Command GetCommand() {
        return command;
    }

    public bool GetAutoGatherEnabled() {
        return (command != Command.Free && !rideTarget);
    }

    public void InitTargetHate() {
        if (!targetHateInitialized) {
            targetHateInitialized = true;
            targetHates = new TargetHate[64];
            for (int i = 0; i < targetHates.Length; i++) {
                targetHates[i] = new TargetHate();
            }
        }
    }

    protected void UpdateTargetHate() {
        if (targetHateEnabled) {
            for (int i = 0; i < targetHates.Length; i++) {
                if (targetHates[i].cBase) {
                    if (GameManager.Instance.time >= targetHates[i].lastDamagedTimeStamp + targetHateClearTime) {
                        targetHates[i].cBase = null;
                        targetHates[i].characterID = -1;
                    } else {
                        targetHates[i].dampingTimeRemain -= deltaTimeCache;
                        if (targetHates[i].dampingTimeRemain <= 0f) {
                            targetHates[i].param *= targetHateDamping;
                            targetHates[i].dampingTimeRemain = targetHateUpdateInterval;
                        }
                    }
                } else {
                    targetHates[i].characterID = -1;
                }
            }
        }
    }

    protected void SetLockTargetToTargetHate() { 
        if (targetHateEnabled) {
            int index = -1;
            float maxParam = 0f;
            for (int i = 0; i < targetHates.Length; i++) {
                if (targetHates[i].characterID != -1 && targetHates[i].cBase && targetHates[i].param > maxParam) {
                    maxParam = targetHates[i].param;
                    index = i;
                }
            }
            if (index >= 0) {
                for (int i = 0; i < searchArea.Length; i++) {
                    searchArea[i].SetLockTargetFromCharacter(targetHates[index].cBase, lockTargetTime);
                }
            }
        }
    }

    public void ClearTargetHate() {
        if (targetHateEnabled) {
            if (!targetHateInitialized) {
                InitTargetHate();
            }
            for (int i = 0; i < targetHates.Length; i++) {
                targetHates[i].cBase = null;
                targetHates[i].characterID = -1;
                targetHates[i].param = 0;
                targetHates[i].lastDamagedTimeStamp = 0;
                targetHates[i].dampingTimeRemain = 0;
            }
        }
    }

    public void RegisterTargetHate(CharacterBase attacker, float param) {
        if (targetHateEnabled && attacker) {
            if (!targetHateInitialized) {
                InitTargetHate();
            }
            int id = attacker.characterId;
            bool find = false;
            if (attacker.characterId == CharacterManager.Instance.GetPlayerID()) {
                param *= targetHatePlayerBias;
            }
            for (int i = 0; i < targetHates.Length; i++) {
                if (id == targetHates[i].characterID) {
                    find = true;
                    targetHates[i].param += param;
                    targetHates[i].lastDamagedTimeStamp = GameManager.Instance.time;
                    break;
                }
            }
            if (!find) {
                for (int i = 0; i < targetHates.Length; i++) {
                    if (targetHates[i].characterID == -1) {
                        targetHates[i].cBase = attacker;
                        targetHates[i].characterID = id;
                        targetHates[i].param = param;
                        targetHates[i].lastDamagedTimeStamp = GameManager.Instance.time;
                        targetHates[i].dampingTimeRemain = targetHateUpdateInterval;
                        break;
                    }
                }
            }
        }
    }

    public bool GetTargetExist() {
        return target != null;
    }

    public GameObject GetNowTarget() {
        return target;
    }
    public Transform GetNowTargetTrans() {
        return targetTrans;
    }
    public float GetNowTargetRadius() {
        return targetRadius;
    }

    protected virtual void ParticlePlay(int index) {
        if (index >= 0 && index < attackDetection.Length && attackDetection[index]) {
            attackDetection[index].ParticlesPlay();
            anyAttackEnabled = true;
        }
    }
    protected virtual void ParticleStopAll() {
        for (int i = 0; i < attackDetection.Length; i++) {
            if (attackDetection[i]) {
                attackDetection[i].ParticlesStop();
            }
        }
    }
    protected virtual void S_ParticlePlay(int index) {
        if (isSuperman && index >= 0 && index < attackDetection.Length && attackDetection[index]) {
            attackDetection[index].S_ParticlesPlay();
            anyAttackEnabled = true;
        }
    }
    protected virtual void S_ParticleStopAll() {
        for (int i = 0; i < attackDetection.Length; i++) {
            if (attackDetection[i]) {
                attackDetection[i].S_ParticlesStop();
            }
        }
    }
    public void LookAtIgnoreY(Vector3 position) {
        Vector3 posTemp = position;
        posTemp.y = transform.position.y;
        transform.LookAt(posTemp);
    }
    public virtual void SetMutekiTime(float num) {
        if (num > mutekiTimeRemain) {
            mutekiTimeRemain = num;
        }
    }
    protected void ResetKnockRemain() {
        knockRemainLight = GetLightKnockEndurance();
        knockRemain = GetHeavyKnockEndurance();
    }
    public bool IsDead {
        get {
            return state == State.Dead;
        }
    }
    public bool IsJumping {
        get {
            return state == State.Jump;
        }
    }
    public virtual bool IsThrowCancelling {
        get {
            return state == State.Dead;
        }
    }

    protected void SmoothRotation(Transform rotateTransform, Vector3 targetVector, float speed, float towardsMul = 1f, bool ignoreY = false) {
        float step = speed * deltaTimeCache;
        if (step > 0f) {
            if (ignoreY) {
                targetVector.y = 0f;
            }
            Quaternion targetRot = Quaternion.LookRotation(targetVector);
            Quaternion nowRot = Quaternion.LookRotation(Vector3.RotateTowards(rotateTransform.TransformDirection(vecForward), targetVector, step * 0.25f * towardsMul, 0f));
            rotateTransform.rotation = Quaternion.Slerp(nowRot, targetRot, step * 0.125f);
        }
    }

    protected void SmoothPosition(Transform moveTransform, Vector3 targetPos, float speed, float towardsMul = 1f, bool ignoreY = false) {
        float step = speed * deltaTimeCache;
        if (step > 0f) {
            if (ignoreY) {
                targetPos.y = moveTransform.position.y;
            }
            Vector3 posTemp = Vector3.MoveTowards(moveTransform.position, targetPos, step * towardsMul);
            moveTransform.position = Vector3.Lerp(posTemp, targetPos, step * 0.125f);
        }
    }

    public void SetLockTargetExternal(CharacterBase attacker, float lockTargetTime) {
        for (int i = 0; i < searchArea.Length; i++) {
            if (searchArea[i]) {
                if (attacker) {
                    searchArea[i].SetLockTargetFromCharacter(attacker, lockTargetTime);
                }
                if (watchoutTime > 0f) {
                    searchArea[i].SetWatchout(watchoutTime);
                }
            }
        }
    }

    protected void SetLayerChildren(GameObject obj, int layer) {
        obj.layer = layer;
        Transform children = obj.GetComponentInChildren<Transform>();
        if (children.childCount == 0) {
            return;
        }
        foreach (Transform ob in children) {
            SetLayerChildren(ob.gameObject, layer);
        }
    }

    public bool RideEnabled() {
        return !isEnemy && GameManager.Instance.save.config[GameManager.Save.configID_SittingAction] >= (isPlayer ? 1 : 2);
    }

    public void SetRide(Transform ridePoint, float stoppingTime, int motionType = 2, bool timeLimitEnabled = false, float timeLimitDistance = 5f, Transform releasePoint = null, bool continueOnDamage = false) {
        if (ridePoint && !isEnemy) {
            nowSpeed = 0;
            rotSpeed = 0f;
            if (state == State.Jump) {
                anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Landing]);
            }
            if (state != State.Wait) {
                SetState(State.Wait);
            }
            if (isPlayer) {
                stoppingTime = float.MaxValue;
            }
            groundedFlag = true;
            Warp(ridePoint.position, stoppingTime, stoppingTime);            
            rideTarget = ridePoint;
            trans.SetPositionAndRotation(rideTarget.position, rideTarget.rotation);
            rideResetCheck = true;
            rideTimeLimitEnabled = timeLimitEnabled;
            rideTimeLimitDistance = timeLimitDistance;
            rideReleasePoint = releasePoint;
            rideContinueOnDamage = continueOnDamage;
            idleType = motionType;
            if (anim) {
                anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.IdleType], idleType);
            }
        }
    }

    public virtual void RemoveRide(bool replaceToReleasePoint = false) {
        if (rideTarget && !isEnemy && AnimHash.Instance) {
            if (rideReleasePoint) {
                if (replaceToReleasePoint && (rideReleasePoint.position - trans.position).sqrMagnitude < 3f * 3f) {
                    trans.position = rideReleasePoint.position;
                }
                rideReleasePoint = null;
            }
            ReleaseWarpStan();
            Vector3 vecTemp = transform.localEulerAngles;
            vecTemp.x = 0f;
            vecTemp.z = 0f;
            transform.localEulerAngles = vecTemp;
            transform.localScale = Vector3.one;
            idleType = -1;
            if (anim) {
                anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.IdleType], idleType);
            }
            rideTarget = null;
            rideResetCheck = false;
            rideTimeLimitEnabled = false;
        }
    }

    public void RemoveRideSpecificTarget(Transform ridePoint) {
        if (rideTarget == ridePoint) {
            RemoveRide();
        }
    }

    public virtual void Attraction(GameObject decoy, AttractionType type, bool lockForce = false, float targetFixingTime = 0f) { }

    public virtual int GetEnemyID() {
        return -1;
    }

    public virtual void EscapeFromTrap(Vector3 position, SickType sickType, float radius) { }

    public GameObject GetNowDecoy => decoySave;    

    public float GetKnockRemainHeavy => knockRemain;    

    public float GetKnockRemainLight => knockRemainLight;

    public int GetStateInt => (int)state;
    
    public float GetAttackStiffTime => attackStiffTime;

    public float GetAttackStiffRemain => (state == State.Attack ? Mathf.Max(attackStiffTime - stateTime, 0f) : 0f);

    public float GetAttackedTimeRemain => Mathf.Max(attackedTimeRemain, 0f);

    public float GetAttackIntervalSave => attackIntervalSave;

    public float GetLastKnocked => lastKnockedSave;

    public bool IsRiding => (rideTarget != null);

}
