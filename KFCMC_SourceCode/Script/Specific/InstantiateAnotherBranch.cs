using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateAnotherBranch : MonoBehaviour {

    public GameObject normalPrefab;
    public GameObject anotherPrefab;

    private void Awake() {
        if (GameManager.Instance && GameManager.Instance.IsPlayerAnother) {
            if (anotherPrefab) {
                Instantiate(anotherPrefab, transform);
            }
        } else {
            if (normalPrefab) {
                Instantiate(normalPrefab, transform);
            }
        }
    }

}
