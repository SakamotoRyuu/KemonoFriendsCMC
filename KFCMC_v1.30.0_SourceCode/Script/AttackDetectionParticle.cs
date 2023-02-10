using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDetectionParticle : AttackDetectionSick {

    public bool forgetDamageReceiver = false;
    public int attackParticleIndex = 0;

    List<ParticleCollisionEvent> collisionEvents;

    protected override void StartProcess() {
        base.StartProcess();
        isProjectile = true;
        unleashed = true;
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    protected override void Update() {
        if (forgetDamageReceiver) {
            for (int i = 0; i < damageReceiver.Length; i++) {
                damageReceiver[i].characterId = notExistID;
            }
        }
        base.Update();
    }

    protected override Vector3 GetClosestPoint(ref DamageDetection targetDD) {
        if (particles.Length > attackParticleIndex) {
            int numCollisionEvents = particles[attackParticleIndex].GetCollisionEvents(targetDD.gameObject, collisionEvents);
            if (numCollisionEvents > 0) {
                float minDist = float.MaxValue;
                Transform targetTrans = targetDD.transform;
                int answerIndex = 0;
                for (int i = 0; i < numCollisionEvents; i++) {
                    float tempDist = (collisionEvents[i].intersection - targetTrans.position).sqrMagnitude;
                    if (tempDist < minDist) {
                        answerIndex = i;
                        tempDist = minDist;
                    }
                }
                return collisionEvents[answerIndex].intersection;
            }
        }
        return base.GetClosestPoint(ref targetDD);
    }

    public override void WorkEnter(GameObject other) {
        if (attackEnabled) {
            targetDD = other.GetComponent<DamageDetection>();
            if (targetDD) {
                bool justDodged = false;
                CharacterBase targetCBase = targetDD.GetCharacterBase();
                if (targetCBase != null && targetCBase.isPlayer) {
                    PlayerController targetPCon = targetCBase.GetComponent<PlayerController>();
                    if (targetPCon) {
                        justDodged = targetPCon.ReceiveParticleJustDodge(this);
                    }
                }
                if (!justDodged) {
                    ConsiderHit(ref targetDD, true);
                    if (relationIndex >= 0 && parentCBase) {
                        parentCBase.ConsiderHitAttackDetection(relationIndex, ref targetDD);
                    }
                }
            }
        }
    }

    void OnParticleCollision(GameObject other) {
        if (!isEffectOnly && (parentCBase || independenceOnAwake) && other.CompareTag(targetTag)) {
            WorkEnter(other);
        }
    }

}
