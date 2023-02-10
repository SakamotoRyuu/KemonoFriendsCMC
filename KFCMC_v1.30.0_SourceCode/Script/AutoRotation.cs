using UnityEngine;

public class AutoRotation : MonoBehaviour {

    public Vector3 rotSpeed;
    public bool always = false;
    Transform trans;
    Vector3 eulerTemp;

    private void Start() {
        trans = transform;
        eulerTemp = trans.localEulerAngles;
    }

    private void Update() {
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
        trans.localEulerAngles = eulerTemp;
    }

    void OnBecameInvisible() {
        if (!always) {
            enabled = false;
        }
    }
    void OnBecameVisible() {
        if (!always) {
            enabled = true;
        }
    }
}
