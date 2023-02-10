using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSetParentNull : MonoBehaviour
{

    public float timer;

    private void Update() {
        timer -= Time.deltaTime;
        if (timer <= 0f) {
            if (transform.parent != null) {
                transform.SetParent(null);
            }
            enabled = false;
        }
    }

}
