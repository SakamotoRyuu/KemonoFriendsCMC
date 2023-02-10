using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Parkman : PlayerController {

    public GameObject parkmanAttackObj;
    public Transform cameraFacePivot;
    
    protected override void Start() {
        base.Start();
        attackedTimeRemainOnDamage = 100;
    }

    protected override void Update_TimeCount() {
        base.Update_TimeCount();
        attackedTimeRemain = 100;
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        nowST = GetMaxST();
    }

    protected override void Update_Targeting() {
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

    public override void WinAction(bool gravityZero = false, float forceGroundedTimePlus = 0f) {
        if (state != State.Dead) {
            ForceStopForEvent(3f);
            SetFaceSmile();
            if (gravityZero) {
                gravityZeroTimeRemain = 3f;
            }
            anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Smile]);
        }
    }

    public override void SupermanStart(bool effectEnable = true) {
        if (!isSuperman) {
            if (effectEnable) {
                CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.friendsYKStart, friendsId, true, true, friendsId >= 0);
                GameObject duringObj = CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.friendsYKDuring, friendsId, true, false, friendsId >= 0);
            }
            isSuperman = true;
            supermanTime = 0f;
            SupermanSetObj(true);
            SupermanSetMaterial(true);
            parkmanAttackObj.SetActive(true);
        }
    }
    
    public override void SupermanEnd(bool effectEnable = true) {
        if (isSuperman && effectEnable) {
            CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.friendsYKEnd, friendsId, true, true, false);
        }
        if (isSuperman) {
            isSuperman = false;
            supermanTime = 0f;
            SupermanSetObj(false);
            SupermanSetMaterial(false);
            S_ParticleStopAll();
            parkmanAttackObj.SetActive(false);
        }
    }

    public override void AddNowHP(int num, Vector3 position, bool showDamage = true, int colorType = 0, CharacterBase attacker = null) {
        showDamage = false;
        base.AddNowHP(num, position, showDamage, colorType, attacker);
    }

    public override void TakeDamage(int damage, Vector3 position, float knockAmount, Vector3 knockVector, CharacterBase attacker = null, int colorType = 0, bool penetrate = false) {
        gutsRemain = 0;
        damage = 5000;
        base.TakeDamage(damage, position, knockAmount, knockVector, attacker, colorType, penetrate);
    }

    protected override bool GetCanWildRelease() {
        return false;
    }    

    public override float GetMaxSpeed(bool isWalk = false, bool ignoreSuperman = false, bool ignoreSick = false, bool ignoreConfig = false) {
        return (isWalk ? walkSpeed : maxSpeed * (!ignoreSuperman && isSuperman ? superSpeedRate : 1f));
    }
    public override float GetAcceleration() {
        return acceleration * (isSuperman ? superAccelerationRate : 1f);
    }
    public override float GetAngularSpeed() {
        return angularSpeed * (isSuperman ? superAngularRate : 1f);
    }
    protected override void ChangeGoldenMaterial() { }
    protected override void PlayerJump() { }
    public override bool GetCanDodge() { return false; }
    public override bool GetCanQuick() { return false; }
    protected override void Attack() { }

}
