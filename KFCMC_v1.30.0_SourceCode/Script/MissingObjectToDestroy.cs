using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissingObjectToDestroy : MonoBehaviour {

    GameObject checkTarget;
    bool checking = false;

    void Update() {
        if (checking && !checkTarget) {
            Destroy(gameObject);
        }
    }

    public void SetGameObject(GameObject target) {
        checkTarget = target;
        checking = true;
    }
}
