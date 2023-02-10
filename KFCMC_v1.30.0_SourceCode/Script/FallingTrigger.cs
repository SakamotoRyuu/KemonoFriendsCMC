using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingTrigger : MonoBehaviour {

    public GameObject effectPrefab;
    
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Enemy")) {
            EnemyBase eBase = other.GetComponent<EnemyBase>();
            if (eBase) {
                if (effectPrefab) {
                    Instantiate(effectPrefab, other.transform.position, Quaternion.identity);
                }
                eBase.SetFalling();
            }
        }
    }
}
