using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineCheckToChangeMaterial : MonoBehaviour {

    public LayerMask layerMask;
    public GameObject changeTarget;
    public Material reachingMaterial;
    public Material notReachMaterial;
    public int changeMatIndex;
    public bool reach;

    Transform focusTrans;
    Transform camT;
    float timeRemain = 0f;
    Renderer targetRend;
    Material[] targetMaterials;
    RaycastHit hit;
    static readonly Vector3 focusOffset = Vector3.up;
    const float interval = 0.125f;

    bool GetCameraTrans() {
        if (CameraManager.Instance) {
            CameraManager.Instance.SetMainCameraTransform(ref camT);
        } else {
            Camera mainCamera = Camera.main;
            if (mainCamera) {
                camT = mainCamera.transform;
            }
        }
        return camT != null;
    }

    private void Start() {
        GetCameraTrans();
        focusTrans = CharacterManager.Instance.playerTrans;
        if (changeTarget) {
            targetRend = changeTarget.GetComponent<Renderer>();
            targetMaterials = targetRend.materials;
        }
    }

    private void Update() {
        timeRemain -= Time.deltaTime;
        if (timeRemain <= 0f && camT && focusTrans && changeTarget) {
            bool reachTemp = !Physics.Linecast(focusTrans.position + focusOffset, camT.position, out hit, layerMask, QueryTriggerInteraction.Ignore);
            if (reach != reachTemp) {
                reach = reachTemp;
                targetMaterials[changeMatIndex] = reach ? reachingMaterial : notReachMaterial;
                targetRend.materials = targetMaterials;
            }
            timeRemain = interval;
        }
    }
}
