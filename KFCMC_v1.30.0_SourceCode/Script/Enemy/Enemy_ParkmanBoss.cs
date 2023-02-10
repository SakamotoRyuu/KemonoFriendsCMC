using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_ParkmanBoss : EnemyBase {
    
    public Transform movePivot;
    public Amusement_Parkman eventParent;
    float waitTimeRemain;
    bool lostTargetFlag;

    protected override void Awake() {
        base.Awake();
        destinationUpdateInterval = 0.2f;
        mapChipSize = 2;
        waitTimeRemain = 15f;
        roveInterval = 100f;
        stoppingDistanceBattle = 1f;
        stoppingDistanceWait = 1f;
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
            if (agent && StageManager.Instance && StageManager.Instance.dungeonController && agent.pathStatus != NavMeshPathStatus.PathInvalid) {
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
    }

    public override void EmitEffectString(string type) {
        base.EmitEffectString(type);
        switch (type) {
            case "Roar2":
                EmitEffect(0);
                if (effect[0].pivot) {
                    CameraManager.Instance.SetQuake(effect[0].pivot.position, 5, 8, 0, 0, 1f, 4f, dissipationDistance_Boss);
                }
                break;
        }
    }

    void MoveAttack() { }    

    public override void SetLevel(int newLevel, bool effectFlag = false, bool isLevelUp = true, int variableLevel = 0) { }

    protected override void LoadEnemyCanvas() { }

    public override void AddNowHP(int num, Vector3 position, bool showDamage = true, int colorType = 0, CharacterBase attacker = null) {
        showDamage = false;
        base.AddNowHP(num, position, showDamage, colorType, attacker);
    }

    public override void TakeDamage(int damage, Vector3 position, float knockAmount, Vector3 knockVector, CharacterBase attacker = null, int colorType = 0, bool penetrate = false) {
        // Disabled
    }

    public override void TakeDamageFixKnock(int damage, Vector3 position, float knockAmount, Vector3 knockVector, CharacterBase attacker = null, int colorType = 0, bool penetrate = false) {
        BootDeathEffect(ed);
        if (eventParent) {
            eventParent.DefeatEnemy(GetCenterPosition());
        }
        Destroy(gameObject);
    }

    protected override void Attack() {
        AttackBase(0, 1, 2.5f, 0, 90f / 60f, 90f / 60f + 1.5f, 0f, 1f, true, 20f);
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        ApproachTransformPivot(movePivot, GetMaxSpeed() * 1.75f, 0.3f, 0.15f, true);
    }

    public override float GetMaxSpeed(bool isWalk = false, bool ignoreSuperman = false, bool ignoreSick = false, bool ignoreConfig = false) {
        return maxSpeed * (GameManager.Instance.minmiPurple ? 1.5f : 1f);
    }

    public override float GetAcceleration() {
        return acceleration * (GameManager.Instance.minmiPurple ? 1.5f : 1f);
    }

    public override float GetAngularSpeed() {
        return angularSpeed * (GameManager.Instance.minmiPurple ? 1.5f : 1f);
    }


}
