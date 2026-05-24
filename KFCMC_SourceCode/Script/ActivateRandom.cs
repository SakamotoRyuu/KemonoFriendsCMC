using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateRandom : MonoBehaviour {

    public GameObject[] targetObj;

    private void Awake() {
        if (targetObj.Length > 0) {
            int index = Random.Range(0, targetObj.Length);
            if (targetObj[index]) {
                targetObj[index].SetActive(true);
            }
        }
    }

}
