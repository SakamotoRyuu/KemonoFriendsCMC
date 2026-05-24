using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateAnotherBranch_Position : MonoBehaviour {

    public GameObject normalPrefab;
    public GameObject anotherPrefab;
    public Vector3 position;
    public Vector3 eulerAngles;

    private void Awake() {
        if (GameManager.Instance && GameManager.Instance.IsPlayerAnother) {
            if (anotherPrefab) {
                Instantiate(anotherPrefab, position, Quaternion.Euler(eulerAngles), transform);
            }
        } else {
            if (normalPrefab) {
                Instantiate(normalPrefab, position, Quaternion.Euler(eulerAngles), transform);
            }
        }
    }
}
