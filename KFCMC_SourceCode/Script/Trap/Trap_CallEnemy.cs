using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_CallEnemy : TrapBase {

    public GameObject effectPrefab;
    public Vector3 offset;    

    protected override void WorkEnter(int index) {
        if (effectPrefab != null) {
            Instantiate(effectPrefab, transform.position + offset, transform.rotation);
        }
        if (StageManager.Instance != null && StageManager.Instance.dungeonController != null) {
            StageManager.Instance.dungeonController.SummonEnemies(Random.Range(4, 7), -1);
        }
    }
}
