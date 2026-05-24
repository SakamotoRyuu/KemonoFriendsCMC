using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_Volcano : MonoBehaviour
{

    public Transform callPivot;
    public Transform breakPivot;
    public GameObject breakEffect;
    public GameObject activateTarget;
    public GameObject destroyTarget;
    public GameObject callEnemyEffect;

    float elapsedTime;
    int state;
    int enemyCount;

    private void Update() {
        if (state == 1) {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= 0.1f) {
                elapsedTime = 0f;
                if (StageManager.Instance.dungeonController && StageManager.Instance.dungeonController.EnemyCount(true) <= 0) {
                    if (enemyCount >= 4) {
                        state = 2;
                    } else {
                        Instantiate(callEnemyEffect, callPivot.position, callPivot.rotation);
                        StageManager.Instance.SetActiveEnemies(true, enemyCount);
                        StageManager.Instance.SetEnemiesLockonPlayer(enemyCount);
                        enemyCount++;
                    }
                }
            }
        } else if (state == 2) {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= 0.7f) {
                if (destroyTarget) {
                    if (destroyTarget.activeSelf) {
                        if (CameraManager.Instance) {
                            CameraManager.Instance.SetQuake(breakPivot.position, 15, 4, 0f, 0.25f, 1.75f, 100f, 200f);
                        }
                        Instantiate(breakEffect, breakPivot.position, breakPivot.rotation);
                    }
                    Destroy(destroyTarget);
                }
                if (activateTarget) {
                    activateTarget.SetActive(true);
                }
                state = 3;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (state == 0 && other.CompareTag("ItemGetter")) {
            state = 1;
        }
    }

}
