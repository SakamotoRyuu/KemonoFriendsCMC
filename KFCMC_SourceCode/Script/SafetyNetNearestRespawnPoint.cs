using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafetyNetNearestRespawnPoint : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            Vector3 pos = other.transform.position;
            GameObject[] respawns = GameObject.FindGameObjectsWithTag("Respawn");
            GameObject[] passages = GameObject.FindGameObjectsWithTag("Passage");
            float sqrDistMinR = float.MaxValue;
            int minIndexR = -1;
            float sqrDistMinP = float.MaxValue;
            int minIndexP = -1;
            for (int i = 0; i < respawns.Length; i++) {
                float sqrDistTemp = (respawns[i].transform.position - pos).sqrMagnitude;
                if (sqrDistTemp < sqrDistMinR) {
                    sqrDistMinR = sqrDistTemp;
                    minIndexR = i;
                }
            }
            for (int i = 0; i < passages.Length; i++) {
                float sqrDistTemp = (passages[i].transform.position - pos).sqrMagnitude;
                if (sqrDistTemp < sqrDistMinP) {
                    sqrDistMinP = sqrDistTemp;
                    minIndexP = i;
                }
            }
            if (sqrDistMinR <= sqrDistMinP) {
                if (minIndexR >= 0) {
                    other.transform.position = respawns[minIndexR].transform.position;
                }
            } else {
                if (minIndexP >= 0) {
                    other.transform.position = passages[minIndexP].transform.position;
                }
            }
        }
    }

}
