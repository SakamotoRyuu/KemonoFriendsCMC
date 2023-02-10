using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveObjectOnEnable : MonoBehaviour {

    public GameObject[] targetObject;
    public bool inverseFlag;

    private void OnEnable() {
        for (int i = 0; i < targetObject.Length; i++) {
            if (targetObject[i]) {
                targetObject[i].SetActive(!inverseFlag);
            }
        }
    }

}
