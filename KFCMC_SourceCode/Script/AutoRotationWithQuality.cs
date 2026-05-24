using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotationWithQuality : MonoBehaviour {

    public Vector3 rotSpeed;
    public int conditionQuality = 1;
    Vector3 eulerTemp;

    private void Start() {
        eulerTemp = transform.localEulerAngles;
    }

    private void Update() {
        if (GameManager.Instance && GameManager.Instance.save.config[GameManager.Save.configID_QualityLevel] >= conditionQuality) {
            float delta = Time.deltaTime;
            if (rotSpeed.x != 0f) {
                eulerTemp.x += rotSpeed.x * delta;
                if (eulerTemp.x > 180f) {
                    eulerTemp.x -= 360f;
                }
            }
            if (rotSpeed.y != 0f) {
                eulerTemp.y += rotSpeed.y * delta;
                if (eulerTemp.y > 180f) {
                    eulerTemp.y -= 360f;
                }
            }
            if (rotSpeed.z != 0f) {
                eulerTemp.z += rotSpeed.z * delta;
                if (eulerTemp.z > 180f) {
                    eulerTemp.z -= 360f;
                }
            }
            transform.localEulerAngles = eulerTemp;
        }
    }
}
