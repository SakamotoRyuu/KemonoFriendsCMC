using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangeOnEnable : MonoBehaviour {

    public string sceneName = "Play";
    bool loadedFlag;

    private void OnEnable() {
        if (GameManager.Instance && !loadedFlag) {
            loadedFlag = true;
            GameManager.Instance.LoadScene(sceneName);
        }
    }

}
