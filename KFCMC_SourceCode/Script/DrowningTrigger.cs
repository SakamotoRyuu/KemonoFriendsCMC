using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrowningTrigger : MonoBehaviour {

    public GameObject effectPrefab;
    public bool isLava;
    public bool isFalling;

    FriendsBase fBase;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            fBase = other.gameObject.GetComponent<FriendsBase>();
            if (fBase && !fBase.GetDrowning()) {
                if ((isFalling || !fBase.GetDrownTolerance()) && effectPrefab) {
                    Instantiate(effectPrefab, other.transform.position, Quaternion.identity, other.transform);
                }
                fBase.SetDrowning(transform.position.y, isLava, isFalling);
            }
        }
    }
}
