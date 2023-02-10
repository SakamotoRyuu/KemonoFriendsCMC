using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Alexandrium : EnemyBase {

    public LaserOption[] laserOption;
    public RaycastToAdjustCapsuleCollider[] raycaster;

    int laserMax = 1;
    int attackSave = -1;
    bool laserEnabled = false;
    static readonly float[] lockonSpeedArray = new float[] { 2.5f, 2.5f, 3.25f, 4.225f, 5.5f };

    protected override void Awake() {
        base.Awake();
        attackedTimeRemainOnRestoreKnockDown = 0.5f;
        attackLockonDefaultSpeed = lockonSpeedArray[0];
        LaserCancel();
    }

    protected override void SetLevelModifier() {
        attackLockonDefaultSpeed = lockonSpeedArray[Mathf.Clamp(level, 0, lockonSpeedArray.Length - 1)];
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (state != State.Attack && laserEnabled) {
            LaserCancel();
        }
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
        for (int i = 0; i < laserOption.Length && i < laserMax; i++) {
            if (laserOption[i]) {
                laserOption[i].LightFlickeringChargeStart();
            }
        }
        for (int i = 0; i < raycaster.Length && i < laserMax; i++) {
            if (raycaster[i]) {
                raycaster[i].hitEffectEnabled = false;
                raycaster[i].Activate();
            }
        }
    }

    void LaserStart() {
        laserEnabled = true;
        for (int i = 0; i < laserOption.Length && i < laserMax; i++) {
            if (laserOption[i]) {
                laserOption[i].LightFlickeringChargeEnd();
            }
        }
        for (int i = 0; i < raycaster.Length && i < laserMax; i++) {
            if (raycaster[i]){
                raycaster[i].hitEffectEnabled = true;
                raycaster[i].Activate();
            }
        }
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
    }
    
    int GetAttackIndex() {
        return Random.Range(0, level >= 4 ? 6 : level == 3 ? 5 : level == 2 ? 4 : 3);
    }

    void AttackStartForLaser() {
        for (int i = 1; i < attackDetection.Length && i < laserMax + 1; i++) {
            AttackStart(i);
        }
    }

    void AttackEndForLaser() {
        for (int i = 1; i < attackDetection.Length && i < laserMax + 1; i++) {
            AttackEnd(i);
        }
    }

    protected override void Attack() {
        int attackRandom = GetAttackIndex();
        if (attackRandom == attackSave) {
            attackRandom = GetAttackIndex();
        }
        if (attackRandom <= 3) {
            laserMax = 1;
        } else if (attackRandom == 4) {
            laserMax = 2;
        } else {
            laserMax = 3;
        }
        AttackBase(attackRandom, 1f, 0.8f, 0, 150f / 60f, 150f / 60f + GetAttackInterval(1.5f, laserMax * -1), 0f);
        if (level >= 3) {
            SuperarmorStart();
        }
        attackSave = attackRandom;
    }
}
