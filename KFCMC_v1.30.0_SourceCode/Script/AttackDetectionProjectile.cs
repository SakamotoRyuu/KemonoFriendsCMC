using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDetectionProjectile : AttackDetectionSick {

    [System.Serializable]
    public class MovingLimit {
        public bool enabled = false;
        public float limitSpeed = 1;
        public float minDuration = 0.1f;
        public float delay = 0f;
        public bool recycle = false;
        public bool disablizeOnKinematic;
    }    

    public float attackDuration = 5;
    public float destroyTime = 5;
    public bool ignoreParentMissing;
    public GameObject destroyTarget;
    public MovingLimit movingLimit;

    protected Rigidbody rigid;
    protected float elapsedTime;
    protected int shotProgress;
    protected float recycleTime = 0.1f;
    protected int kinematicCount;

    protected override void AwakeProcess() {
        base.AwakeProcess();
        if (movingLimit.enabled) {
            rigid = GetComponentInParent<Rigidbody>();
        }
        shotProgress = 0;
        elapsedTime = 0;
        isProjectile = true;
    }

    protected bool CheckDestroyCondition() {
        if (elapsedTime >= destroyTime || (!ignoreParentMissing && !parentCBase) || (movingLimit.enabled && !rigid) || (!ignoreParentMissing && parentCBase && parentCBase.IsThrowCancelling)) {
            if (shotProgress == 1) {
                DetectionEnd();
            }
            if (destroyTarget) {
                Destroy(destroyTarget);
                return true;
            } else if (transform.parent.gameObject) {
                Destroy(transform.parent.gameObject);
                return true;
            } else {
                Destroy(gameObject);
                return true;
            }
        }
        return false;
    }

    private void FixedUpdate() {
        elapsedTime += Time.fixedDeltaTime;
        if (!CheckDestroyCondition()) {
            switch (shotProgress) {
                case 0:
                    if (!movingLimit.enabled || (rigid && rigid.velocity.sqrMagnitude >= movingLimit.limitSpeed * movingLimit.limitSpeed && elapsedTime >= movingLimit.delay)) {
                        if (!attackEnabled) {
                            DetectionStart();
                        }
                        shotProgress = 1;
                        elapsedTime = 0f;
                    }
                    break;
                case 1:
                    if (movingLimit.disablizeOnKinematic && rigid && rigid.isKinematic) {
                        kinematicCount++;
                    } else if (kinematicCount > 0) {
                        kinematicCount = 0;
                    }
                    if (elapsedTime >= attackDuration || (movingLimit.enabled && elapsedTime >= movingLimit.minDuration && rigid && (rigid.velocity).sqrMagnitude < movingLimit.limitSpeed * movingLimit.limitSpeed) || kinematicCount > 1) {
                        DetectionEnd();
                        shotProgress = 2;
                        elapsedTime = 0f;
                    }
                    break;
                case 2:
                    if (movingLimit.recycle && elapsedTime >= recycleTime) {
                        shotProgress = 0;
                        elapsedTime -= recycleTime;
                        if (elapsedTime > oneFrameSec) {
                            elapsedTime = oneFrameSec;
                        }
                    }
                    break;
            }
        }
    }
    
}
