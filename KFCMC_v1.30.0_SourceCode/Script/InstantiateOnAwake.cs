using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateOnAwake : MonoBehaviour {

    [System.Serializable]
    public class GroundSettings {
        public bool checkGroundEnabled = false;
        public LayerMask layerMask;
        public float offset = 0.3f;
        public float maxDistance = 0.6f;
    }

    public GameObject prefab;
    public GroundSettings groundSettings;

    private void Awake() {
        if (prefab) {
            if (!groundSettings.checkGroundEnabled) {
                Instantiate(prefab, transform);
            } else {
                Ray ray = new Ray(transform.position + Vector3.up * groundSettings.offset, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, groundSettings.maxDistance, groundSettings.layerMask, QueryTriggerInteraction.Ignore)) {
                    Instantiate(prefab, hit.point, transform.rotation, transform);
                } else {
                    Instantiate(prefab, transform.position + Vector3.down * (groundSettings.maxDistance - groundSettings.offset), transform.rotation, transform);
                }
            }
        }
    }

}
