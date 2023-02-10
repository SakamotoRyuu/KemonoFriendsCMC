using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTimeScaleOnEnable : MonoBehaviour {

    public float newTimeScale;

    private void OnEnable() {
        Time.timeScale = newTimeScale;
    }

}
