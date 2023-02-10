using UnityEngine;

[ExecuteInEditMode]
public class DepthOn : MonoBehaviour {

    void Awake() {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth | DepthTextureMode.MotionVectors;
    }
}