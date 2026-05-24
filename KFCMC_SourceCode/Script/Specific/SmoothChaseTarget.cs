using UnityEngine;

public class SmoothChaseTarget : MonoBehaviour {

    public Vector3 offset;
    public float smoothTime;
    public Transform target;

    int state;
    Vector3 velocity;

    public void SetTarget(Transform target) {
        this.target = target;
    }

    public void RemoveTarget() {
        target = null;
    }

    private void Update() {
        if (target) {
            transform.position = Vector3.SmoothDamp(transform.position, target.position + offset, ref velocity, smoothTime);
        }
    }

}
