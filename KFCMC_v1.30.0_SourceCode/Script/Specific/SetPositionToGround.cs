using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPositionToGround : MonoBehaviour {

    public Transform setTarget;
    public Transform rayPivot;
    public float delay = 0.5f;
    public float maxDistance = 10f;
    public LayerMask layerMask;
    public bool fixWorldRotation;
    public bool waitUntilDrop;
    public bool activateOnDropping;
    public Rigidbody checkRigidbody;

    private float elapsedTime;
    private bool dropped;
    private Ray ray;
    private RaycastHit hit;
    static readonly Vector3 vecDown = Vector3.down;
    static readonly Quaternion quaIden = Quaternion.identity;

    private void Awake() {
        ray = new Ray(rayPivot.position, vecDown);
        if (activateOnDropping) {
            setTarget.gameObject.SetActive(false);
        }
    }

    void Update() {
        if (elapsedTime >= delay) {
            if (waitUntilDrop && !dropped) {
                if (checkRigidbody && checkRigidbody.velocity.y < 0f) {
                    dropped = true;
                    if (activateOnDropping) {
                        setTarget.gameObject.SetActive(true);
                    }
                }
            }
            if (!waitUntilDrop || dropped) {
                ray.origin = rayPivot.position;
                if (Physics.Raycast(ray, out hit, maxDistance, layerMask, QueryTriggerInteraction.Ignore)) {
                    setTarget.position = hit.point;
                    if (fixWorldRotation) {
                        setTarget.rotation = quaIden;
                    }
                    if (!setTarget.gameObject.activeSelf) {
                        setTarget.gameObject.SetActive(true);
                    }
                } else if (setTarget.gameObject.activeSelf) {
                    setTarget.gameObject.SetActive(false);
                }
            }
        }
        elapsedTime += Time.deltaTime;
    }
}
