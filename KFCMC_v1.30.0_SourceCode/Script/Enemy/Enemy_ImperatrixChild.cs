using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_ImperatrixChild : EnemyBase {

    public Enemy_ImperatrixMundi parentEnemy;
    public LaserOption laserOption;
    public RaycastToAdjustCapsuleCollider raycaster;
    public BoxCollider cubeCollider;
    public SearchTargetReference searchTargetReference;
    public int attackPowerSandstarRaw;
    public int maxHPSandstarRaw;
    public int maxHPAmusement;
    public AttackDetectionSoundFade[] adjustVolumeTarget;
    public float[] adjustVolumeMax;

    bool laserEnabled;
    bool sandstarRawEnabled;
    static readonly float[] volumeArray = new float[12] {
        1f,
        0.7647961f,
        0.6537689f,
        0.5849131f,
        0.5365392f,
        0.5f,
        0.4710548f,
        0.4473393f,
        0.4274138f,
        0.4103431f,
        0.3954889f,
        0.3823980f
    };


    protected override void Awake() {
        base.Awake();
        roveInterval = -1;
        attackWaitingLockonRotSpeed = 0;
        attackLockonDefaultSpeed = 0;
        LaserCancel();
        SetDropRate(0);
        dropExpDisabled = true;
        mapChipSize = 2;
        isBoss = true;
    }

    public void SetSandstarRaw() {
        if (!sandstarRawEnabled) {
            sandstarRawEnabled = true;
            SupermanSetMaterial(true);
            SetSupermanEffect(true);
            attackPower = attackPowerSandstarRaw;
            nowHP = maxHP = maxHPSandstarRaw;
        }
    }

    public override void SetForAmusement(Amusement_Mogisen amusement) {
        isForAmusement = true;
        nowHP = maxHP = maxHPAmusement;
    }

    public void SetSearchTargetReference(GameObject targetObj) {
        if (searchTargetReference && targetObj) {
            searchTargetReference.referObj = targetObj;
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        knockRemain = knockEndurance;
        knockRemainLight = knockEnduranceLight;
        if (state != State.Attack && laserEnabled) {
            LaserCancel();
        }
        if (laserEnabled && targetTrans) {
            SmoothPosition(laserOption.transform, cubeCollider.ClosestPoint(targetTrans.position), 5f, 0.2f, false);
            SmoothRotation(laserOption.transform, (targetTrans.position - laserOption.transform.position).normalized, 5f, 0.2f, false);
        }
        if (Event_LastBattleSecond.Instance && Event_LastBattleSecond.Instance.eventNow && attackedTimeRemain < 2f) {
            attackedTimeRemain = 2f;
        }
    }

    public override void SetLevel(int newLevel, bool effectFlag = false, bool isLevelUp = true, int variableLevel = 0) { }

    protected override void LoadEnemyCanvas() { }

    void LaserCancel() {
        laserEnabled = false;
        laserOption.CancelLaser();
        raycaster.Deactivate();
    }

    void LaserReady() {
        laserEnabled = true;
        if (targetTrans) {
            laserOption.transform.position = cubeCollider.ClosestPoint(targetTrans.position);
            laserOption.transform.LookAt(targetTrans.position);
        } else {
            laserOption.transform.position = GetCenterPosition();
            laserOption.transform.localRotation = quaIden;
        }
        laserOption.LightFlickeringChargeStart();
        raycaster.hitEffectEnabled = false;
        raycaster.Activate();
        AttackStart(0);
    }

    void LaserStart() {
        laserEnabled = true;
        laserOption.LightFlickeringChargeEnd();
        raycaster.hitEffectEnabled = true;
        raycaster.Activate();
        AttackEnd(0);
        AttackStart(1);
    }

    void LaserEnd() {
        laserEnabled = false;
        laserOption.LightFlickeringBlastEnd();
        raycaster.Deactivate();
        AttackEnd(1);
    }

    public override int GetMaxHP() {
        return maxHP;
    }

    protected override void Attack() {
        base.Attack();
        bool targetIsNear = (targetTrans && (cubeCollider.ClosestPoint(targetTrans.position) - targetTrans.position).sqrMagnitude < 3f * 3f);
        int attackTemp = 0;
        if (targetIsNear) {
            attackTemp = Random.Range(0, 2);
        }
        switch (attackTemp) {
            case 0:
                AttackBase(0, 1f, 0.8f, 0, 150f / 60f, 150f / 60f + Random.Range(0.3f, 0.4f), 0, 1, false);
                break;
            case 1:
                AttackBase(1, 1.1f, 1.2f, 0, 120f / 60f, 120f / 60f + Random.Range(0.3f, 0.4f), 0, 1, false);
                break;
        }
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        if (attackType == 0 && parentEnemy) {
            int count = Mathf.Clamp(parentEnemy.GetLaserAttackingCount(), 1, volumeArray.Length) - 1;
            float volumeTemp = volumeArray[count];
            for (int i = 0; i < adjustVolumeTarget.Length; i++) {
                if (adjustVolumeTarget[i]) {
                    adjustVolumeTarget[i].endVolume = adjustVolumeMax[i] * volumeTemp;
                }
            }
        }
    }

    public bool IsLaserAttacking() {
        return laserEnabled;
    }

    public void SetAttackedTimeRemain(float param) {
        attackedTimeRemain = param;
    }

}
