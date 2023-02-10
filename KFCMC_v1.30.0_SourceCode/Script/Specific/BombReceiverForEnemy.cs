using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombReceiverForEnemy : MonoBehaviour {

    public GameObject destroyTarget;
    public GameObject deadEffectPrefab;
    public Transform centerPivot;
    public float scale;
    public float force;
    public float radius;
    public int count;
    public int colorType;

    public void BootDeathEffect() {
        Instantiate(deadEffectPrefab, centerPivot.position, Quaternion.identity);
        GameObject pieceInstance;
        Vector3 pivot = centerPivot.position;
        Vector3 direction;
        Vector3 forceRandom;
        float mass = scale * scale * scale * 1000;
        float forceBias = mass * 500f * force;
        for (int i = 0; i < count; i++) {
            pieceInstance = ObjectPool.Instance.GetObject(colorType);
            if (pieceInstance) {
                direction = Random.insideUnitSphere;
                forceRandom = Random.insideUnitSphere;
                pieceInstance.transform.position = pivot + direction * radius;
                pieceInstance.GetComponent<CellienPiece>().SetParam(scale, mass, (direction * 0.9f + forceRandom * 0.1f) * forceBias, pivot);
            }
        }
    }

    public void DestroyWithEffect() {
        BootDeathEffect();
        if (destroyTarget) {
            Destroy(destroyTarget);
        } else {
            Destroy(gameObject);
        }
    }

}
