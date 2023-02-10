using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootDeathEffector : MonoBehaviour {

    public EnemyBase.EnemyDeath ed;
    public bool isFullColor;
    public bool isHemisphere;
    public float upperBias;

    private void Awake() {
        if (isFullColor) {
            for (int i = 1; i < 6; i++) {
                ed.colorNum = i;
                BootDeathEffect(ed);
            }
        } else {
            BootDeathEffect(ed);
        }
    }

    private void BootDeathEffect(EnemyBase.EnemyDeath enemyDeath) {
        GameObject pieceInstance;
        Vector3 pivot = (enemyDeath.pivot != null ? enemyDeath.pivot.position : transform.position);
        Vector3 direction;
        Vector3 forceRandom;
        float mass = enemyDeath.scale * enemyDeath.scale * enemyDeath.scale * 1000;
        float forceBias = mass * enemyDeath.force * 500f;
        int appropriateNum = ObjectPool.Instance.GetAppropriateNum(enemyDeath.numOfPieces);
        for (int i = 0; i < appropriateNum; i++) {
            pieceInstance = ObjectPool.Instance.GetObject(enemyDeath.colorNum);
            if (pieceInstance) {
                direction = Random.insideUnitSphere;
                forceRandom = Random.insideUnitSphere;
                if (isHemisphere) {
                    if (direction.y < 0f) {
                        direction.y *= -1;
                    }
                }
                if (upperBias != 0f) {
                    direction += Vector3.up * upperBias;
                    direction.Normalize();
                }
                pieceInstance.transform.position = pivot + direction * enemyDeath.radius;
                pieceInstance.GetComponent<CellienPiece>().SetParam(enemyDeath.scale, mass, (direction * 0.9f + forceRandom * 0.1f) * forceBias, pivot);
            } else {
                break;
            }
        }
    }

}
