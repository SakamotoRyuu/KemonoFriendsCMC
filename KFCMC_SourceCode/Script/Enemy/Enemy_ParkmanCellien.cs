using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_ParkmanCellien : EnemyBase {

    public GameObject ddObj;
    public int matColor;
    public Renderer parkmanRenderer;
    public int parkmanMatIndex;
    public Material[] strongMat;
    public Material weakMat;
    public AudioPlaySpecifyTime audioTime;
    public Transform movePivot;
    public Transform quakePivot;
    public Amusement_Parkman eventParent;
    public float waitTimeRemain;

    bool movingFlag;
    bool isWeak;
    bool isDefeated;
    bool forceCheckFlag;
    bool lostTargetFlag;
    Material[] parkmanMatsArray;

    protected override void Awake() {
        base.Awake();
        destinationUpdateInterval = 0.2f;
        parkmanMatsArray = parkmanRenderer.materials;
        stoppingDistanceBattle = 1f;
        stoppingDistanceWait = 1f;
    }

    protected override void Start() {
        base.Start();
        roveInterval = (waitTimeRemain > 0f ? 20f : 0.01f);
        isWeak = false;
        forceCheckFlag = true;
        if (audioTime) {
            audioTime.startTime = matColor;
        }
    }

    protected override void Update_Targeting() {
        bool targetExistSave = (target != null);
        base.Update_Targeting();
        if (targetExistSave && target == null) {
            lostTargetFlag = true;
        }
    }

    protected override void Rove() {
        if (lostTargetFlag) {
            if (!isWeak && agent && StageManager.Instance && StageManager.Instance.dungeonController && agent.pathStatus != NavMeshPathStatus.PathInvalid) {
                destination = StageManager.Instance.dungeonController.GetRespawnPosClosest(CharacterManager.Instance.playerTrans.position);
                agent.SetDestination(destination);
                arrived = false;
                stateTime = 0f;
            }
            lostTargetFlag = false;
        } else {
            base.Rove();
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (waitTimeRemain > 0f) {
            waitTimeRemain -= deltaTimeCache;
        }
        roveInterval = (waitTimeRemain > 0f ? 100f : 0.01f);
        if (!isDefeated && CharacterManager.Instance.pCon && waitTimeRemain <= 0f) {
            if (CharacterManager.Instance.pCon.isSuperman != isWeak || forceCheckFlag) {
                isWeak = CharacterManager.Instance.pCon.isSuperman;
                forceCheckFlag = false;
                if (isWeak) {
                    SetSick(SickType.Slow, 10000f);
                    ddObj.SetActive(true);
                    actDistNum = 1;
                    parkmanMatsArray[parkmanMatIndex] = weakMat;
                    searchArea[0].SetWatchout(999999f);
                } else {
                    ClearSick();
                    ddObj.SetActive(false);
                    actDistNum = 0;
                    parkmanMatsArray[parkmanMatIndex] = strongMat[matColor];
                    searchArea[0].SetNotWatchout();
                }
                if (attackedTimeRemain < 1f) {
                    attackedTimeRemain = 1f;
                }
                parkmanRenderer.materials = parkmanMatsArray;
            }
        }
        if (isWeak && anyAttackEnabled) {
            ResetTriggerOnDamage();
        }
    }

    void MoveAttack0() {
        movingFlag = true;
    }

    void MoveEnd() {
        movingFlag = false;
    }
    
    void QuakeAttack() {
        CameraManager.Instance.SetQuake(quakePivot.position, 8, 4, 0, 0, 1.5f, 2f, dissipationDistance_Normal);
    }

    public override void SetLevel(int newLevel, bool effectFlag = false, bool isLevelUp = true, int variableLevel = 0) { }

    protected override void LoadEnemyCanvas() { }

    public override void AddNowHP(int num, Vector3 position, bool showDamage = true, int colorType = 0, CharacterBase attacker = null) {
        showDamage = false;
        base.AddNowHP(num, position, showDamage, colorType, attacker);
    }

    public override void TakeDamage(int damage, Vector3 position, float knockAmount, Vector3 knockVector, CharacterBase attacker = null, int colorType = 0, bool penetrate = false) {
        base.TakeDamage(damage, position, knockAmount, knockVector, attacker, colorType, penetrate);
        BootDeathEffect(ed);
        if (eventParent) {
            eventParent.DefeatEnemy(GetCenterPosition());
        }
        isDefeated = true;
        Destroy(gameObject);
    }

    public override void AttackStart(int index) {
        if (!isWeak) {
            base.AttackStart(index);
        }
    }

    public override float GetMaxSpeed(bool isWalk = false, bool ignoreSuperman = false, bool ignoreSick = false, bool ignoreConfig = false) {
        return maxSpeed * (isWeak ? 0.3f : 1f) * (GameManager.Instance.minmiPurple ? 1.5f : 1f);
    }

    public override float GetAcceleration() {
        return acceleration * (isWeak ? 0.5f : 1f) * (GameManager.Instance.minmiPurple ? 1.5f: 1f);
    }

    public override float GetAngularSpeed() {
        return angularSpeed * (isWeak ? 0.3f : 1f) * (GameManager.Instance.minmiPurple ? 1.5f : 1f);
    }

    protected override void Attack() {
        resetAgentRadiusOnChangeState = true;
        movingFlag = false;
        AttackBase(0, 1, 1.4f, 0, 150f / 60f, 150f / 60f + 1.5f, 0.5f, 1f, true, 20f);
        agent.radius = 0.1f;
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        if (movingFlag) {
            ApproachTransformPivot(movePivot, GetMaxSpeed() * 1.75f, 0.3f, 0.15f, true);
        }
    }

}
