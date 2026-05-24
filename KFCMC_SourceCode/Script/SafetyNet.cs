using UnityEngine;

public class SafetyNet : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            GameObject[] respawns = GameObject.FindGameObjectsWithTag("Respawn");
            if (respawns.Length > 0) {
                int minIndex = 0;
                float minDist = float.MaxValue;
                Vector3 otherPos = other.transform.position;
                for (int i = 0; i < respawns.Length; i++) {
                    float sqrDist = (otherPos - respawns[i].transform.position).sqrMagnitude;
                    if (sqrDist < minDist) {
                        minDist = sqrDist;
                        minIndex = i;
                    }
                }
                other.transform.position = respawns[minIndex].transform.position;
            } else {
                other.transform.position = Vector3.up;
            }
        }
    }
}
