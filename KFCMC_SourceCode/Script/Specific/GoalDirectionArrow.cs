using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalDirectionArrow : MonoBehaviour {

    public Canvas canvas;
    public RectTransform goalRect;
    public RectTransform shortcutRect;
    public Image goalImage;
    public Image shortcutImage;

    GameObject goalObj;
    GameObject shortcutObj;
    Transform camT;

    private void Start() {
        if (CameraManager.Instance) {
            CameraManager.Instance.SetMainCameraTransform(ref camT);
        } else {
            Camera camTemp = Camera.main;
            if (camTemp) {
                camT = camTemp.transform;
            }
        }
    }

    void Update () {
        bool flag = false;
        if (StageManager.Instance && camT && CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.GoalDirection) != 0) {
            goalObj = StageManager.Instance.GetGoalInstance();
            shortcutObj = StageManager.Instance.GetShortcutInstance();
            if (goalObj || shortcutObj) {
                flag = true;
                Vector3 fromPos = camT.position;
                float cameraEulerY = camT.localEulerAngles.y;
                if (goalObj) {
                    Vector3 toPos = goalObj.transform.position;
                    float angle = Mathf.Atan2(toPos.x - fromPos.x, toPos.z - fromPos.z) * Mathf.Rad2Deg;
                    goalRect.localRotation = Quaternion.Euler(0f, 0f, cameraEulerY - angle);
                    if (!goalImage.enabled) {
                        goalImage.enabled = true;
                    }
                } else {
                    if (goalImage.enabled) {
                        goalImage.enabled = false;
                    }
                }
                if (shortcutObj) {
                    Vector3 toPos = shortcutObj.transform.position;
                    float angle = Mathf.Atan2(toPos.x - fromPos.x, toPos.z - fromPos.z) * Mathf.Rad2Deg;
                    shortcutRect.localRotation = Quaternion.Euler(0f, 0f, cameraEulerY - angle);
                    if (!shortcutImage.enabled) {
                        shortcutImage.enabled = true;
                    }
                } else {
                    if (shortcutImage.enabled) {
                        shortcutImage.enabled = false;
                    }
                }
            }
        }
        if (flag != canvas.enabled) {
            canvas.enabled = flag;
        }		
	}
}
