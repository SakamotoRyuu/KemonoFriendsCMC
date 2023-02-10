using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOperationCanvas : MonoBehaviour {

    public Canvas canvas;
    public bool switchOnBecameVisible = true;
    public bool checkCanvasCulling;
    public bool isEnemyHP;
    protected Transform trans;
    protected Transform camT;

    private void Awake() {
        trans = transform;
        if (switchOnBecameVisible) {
            enabled = false;
            if (canvas) {
                canvas.enabled = false;
            }
        }
    }

    private void Start() {
        if (CameraManager.Instance) {
            CameraManager.Instance.SetMainCameraTransform(ref camT);
        } else {
            Camera mainCamera = Camera.main;
            if (mainCamera) {
                camT = mainCamera.transform;
            }
        }
    }
    
    protected virtual void Update () {
        if (camT && trans.rotation != camT.rotation) {
            trans.rotation = camT.rotation;
        }
        if (isEnemyHP && GameManager.Instance.save.config[GameManager.Save.configID_ShowEnemyHp] == 0 && canvas.enabled) {
            canvas.enabled = false;
        } else if (checkCanvasCulling && CanvasCulling.Instance && canvas.enabled != CanvasCulling.Instance.canvas[0].enabled) {
            canvas.enabled = CanvasCulling.Instance.canvas[0].enabled;
        }
	}

    void OnBecameInvisible() {
        if (switchOnBecameVisible) {
            enabled = false;
            if (canvas) {
                canvas.enabled = false;
            }
        }
    }
    void OnBecameVisible() {
        if (switchOnBecameVisible) {
            enabled = true;
            if (canvas) {
                canvas.enabled = checkCanvasCulling && CanvasCulling.Instance ? CanvasCulling.Instance.canvas[0].enabled : true;
            }
        }
    }

}
