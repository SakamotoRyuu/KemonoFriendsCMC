using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonDamageDummyAttack : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("PlayerDamageDetection")) {
            DamageDetection damageDetection = other.GetComponent<DamageDetection>();
            if (damageDetection) {
                damageDetection.NonDamageDodge(transform.position);
            }
        }
    }

}
