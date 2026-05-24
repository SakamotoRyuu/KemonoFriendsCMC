using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonCharacterAttackDetectionParticle : NonCharacterAttackDetection {

    public ParticleSystem attackParticle;
    List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

    void OnParticleCollision(GameObject other) {
        if (other.CompareTag(targetTag)) {
            DamageDetection targetDD = other.GetComponent<DamageDetection>();
            if (targetDD) {
                int targetID = targetDD.characterId;
                if (!idList.Contains(targetID)) {
                    idList.Add(targetID);
                    Vector3 closestPoint;
                    int numCollisionEvents = attackParticle.GetCollisionEvents(targetDD.gameObject, collisionEvents);
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
                        closestPoint = collisionEvents[answerIndex].intersection;
                    } else {
                        closestPoint = Vector3.Lerp(targetDD.transform.position, transform.position, 0.5f);
                    }
                    Vector3 direction = targetDD.transform.position - transform.position;
                    MyMath.NormalizeXZ(ref direction);
                    WorkEnter(targetDD, closestPoint, direction);
                }
            }
        }
    }

}
