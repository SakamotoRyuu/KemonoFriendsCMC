using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandstarHealPoint : MonoBehaviour {

    public GameObject healEffect;
    bool healing;
    float effectInterval = 1f;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && CharacterManager.Instance && other.gameObject == CharacterManager.Instance.playerObj) {
            healing = true;
            if (effectInterval >= 0.5f) {
                Instantiate(healEffect, other.transform.position, other.transform.rotation, other.transform);
                effectInterval = 0f;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player") && CharacterManager.Instance && other.gameObject == CharacterManager.Instance.playerObj) {
            healing = false;
        }
    }

    private void Update() {
        if (healing) {
            CharacterManager.Instance.AddSandstar(Time.deltaTime, true);
        }
        if (effectInterval < 1f) {
            effectInterval += Time.deltaTime;
        }
    }

}
