using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOperationSpriteRenderer : MonoBehaviour {

    public SpriteRenderer spriteRenderer;
    public bool switchOnBecameVisible;
    public bool checkCanvasCulling;
    public bool isEnemyHP;
    protected Transform trans;
    protected Transform camT;

    private void Awake() {
        trans = transform;
        if (switchOnBecameVisible) {
            enabled = false;
            if (spriteRenderer) {
                spriteRenderer.enabled = false;
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

    protected virtual void Update() {
        if (camT && trans.rotation != camT.rotation) {
            trans.rotation = camT.rotation;
        }
        if (isEnemyHP && GameManager.Instance.save.config[GameManager.Save.configID_ShowEnemyHp] == 0 && spriteRenderer.enabled) {
            spriteRenderer.enabled = false;
        } else if (checkCanvasCulling && CanvasCulling.Instance && spriteRenderer.enabled != CanvasCulling.Instance.canvas[0].enabled) {
            spriteRenderer.enabled = CanvasCulling.Instance.canvas[0].enabled;
        }
    }

    void OnBecameInvisible() {
        if (switchOnBecameVisible) {
            enabled = false;
            if (spriteRenderer) {
                spriteRenderer.enabled = false;
            }
        }
    }
    void OnBecameVisible() {
        if (switchOnBecameVisible) {
            enabled = true;
            if (spriteRenderer) {
                spriteRenderer.enabled = checkCanvasCulling && CanvasCulling.Instance ? CanvasCulling.Instance.canvas[0].enabled : true;
            }
        }
    }
}
