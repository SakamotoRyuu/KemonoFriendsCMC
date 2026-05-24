using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateObject : MonoBehaviour {

    public GameObject[] targetObject;

    public void Activate(bool flag) {
        for (int i = 0; i < targetObject.Length; i++) {
            if (targetObject[i] && targetObject[i].activeSelf != flag) {
                targetObject[i].SetActive(flag);
            }
        }
    }

}
