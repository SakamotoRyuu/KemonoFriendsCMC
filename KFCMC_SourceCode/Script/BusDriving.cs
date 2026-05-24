using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusDriving : MonoBehaviour {

    [System.Serializable]
    public class AxleInfo {
        public WheelCollider wheel;
        public Transform model;
        public bool motor; //駆動輪か?
        public bool steering; //ハンドル操作をしたときに角度が変わるか？
    }

    public List<AxleInfo> axleInfos;
    public float maxMotorTorque;
    public float maxSteeringAngle;

    private Vector3 vecTemp;

    private void Update() {
        if (Time.timeScale > 0f && PauseController.Instance && !PauseController.Instance.pauseGame && PauseController.Instance.pauseEnabled) {
            float deltaTimeCache = Time.deltaTime;
            foreach (AxleInfo axleInfo in axleInfos) {
                vecTemp = axleInfo.model.localEulerAngles;
                vecTemp.x += axleInfo.wheel.rpm / 60 * 360 * deltaTimeCache;
                if (axleInfo.steering) {
                    vecTemp.y = axleInfo.wheel.steerAngle - vecTemp.z;
                }
                axleInfo.model.localEulerAngles = vecTemp;
            }
        }
    }

    private void FixedUpdate() {
        if (Time.timeScale > 0f && PauseController.Instance && !PauseController.Instance.pauseGame && PauseController.Instance.pauseEnabled) {
            float motor = maxMotorTorque * GameManager.Instance.playerInput.GetAxis(RewiredConsts.Action.Vertical);
            float steering = maxSteeringAngle * GameManager.Instance.playerInput.GetAxis(RewiredConsts.Action.Horizontal);
            foreach (AxleInfo axleInfo in axleInfos) {
                if (axleInfo.steering) {
                    axleInfo.wheel.steerAngle = steering;
                }
                if (axleInfo.motor) {
                    axleInfo.wheel.motorTorque = motor;
                }
            }
        }
    }
}