using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetGroupFollow : MonoBehaviour {

    public Transform followPlayer;
    public Transform targetGroup;
    public GameObject inputEnabledCheckRefer;

    private Transform trans;
    private float rotateTimeRemain;
    private Vector3 posTemp = Vector3.zero;
    private Vector3 rotTemp = Vector3.zero;
    private Vector2 axisInput = Vector2.zero;
    private float anglePlus;
    private float angleSave;
    private float heightPlus;
    private float heightSave;

    private void Awake() {
        trans = transform;
    }

    void Update() {
        if (followPlayer && CameraManager.Instance && GameManager.Instance) {
            float yAngle = targetGroup.localEulerAngles.y;
            if (inputEnabledCheckRefer && inputEnabledCheckRefer.activeSelf) {
                axisInput.x = CameraManager.Instance.GetCameraHorizontal();
                axisInput.y = CameraManager.Instance.GetCameraVertical();
                if (axisInput.x != 0f) {
                    anglePlus = Mathf.Clamp(anglePlus + axisInput.x * 21f * GameManager.Instance.save.config[GameManager.Save.configID_CameraSensitivity] * Time.deltaTime, -105f, 105f);
                    angleSave = anglePlus;
                    rotateTimeRemain = 5f;
                }
                if (axisInput.y != 0f) {
                    heightPlus = Mathf.Clamp(heightPlus + axisInput.y * 0.4f * GameManager.Instance.save.config[GameManager.Save.configID_CameraSensitivity] * Time.deltaTime, -2f, 2f);
                    heightSave = heightPlus;
                    rotateTimeRemain = 5f;
                }
            }
            if (rotateTimeRemain > 0f) {
                rotateTimeRemain -= Time.deltaTime;
                if (GameManager.Instance.playerInput.GetButton(RewiredConsts.Action.Camera_Reset)) {
                    rotateTimeRemain = 0f;
                }
                if (rotateTimeRemain > 0f && rotateTimeRemain < 2f) {
                    anglePlus = Easing.SineInOut(rotateTimeRemain, 2f, 0f, angleSave);
                    heightPlus = Easing.SineInOut(rotateTimeRemain, 2f, 0f, heightSave);
                } else if (rotateTimeRemain <= 0f) {
                    anglePlus = 0f;
                    heightPlus = 0f;
                }
            }
            posTemp.x = followPlayer.position.x;
            posTemp.y = Mathf.Max(followPlayer.position.y, targetGroup.position.y - 1f);
            posTemp.z = followPlayer.position.z;
            rotTemp.y = yAngle;
            if (rotateTimeRemain > 0f) {
                posTemp.y += heightPlus;
                rotTemp.y += anglePlus;
            }
            trans.SetPositionAndRotation(posTemp, Quaternion.Euler(rotTemp));
        }
    }
}
