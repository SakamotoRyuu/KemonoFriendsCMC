using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookToTargetFromPlayer : MonoBehaviour {
    
    Transform fromTrans;
    GameObject toObj;
    GameObject objSave;
    Transform toTrans;
    Transform trans;
    bool overThresholdFlag;
    float restTimeRemain;
    float heightDistRateSave;
    Vector3 targetPosSave = Vector3.zero;
    float targetStoppingTime;

    static readonly float[] speedArray = new float[21] { 0f, 0.5f, 1.0f, 1.5f, 2.0f, 2.5f, 3.130589f, 3.794118f, 4.492306f, 5.226965f, 6.0f, 6.813413f, 7.669317f, 8.569929f, 9.517585f, 10.51474f, 11.56398f, 12.66804f, 13.82976f, 15.05217f, 16.33843f };
    static readonly Vector3 vecForward = Vector3.forward;
    static readonly float[] suppressArray = new float[11] { 0f, 1f, 1.333333f, 1.666666f, 2f, 2.333333f, 2.666666f, 3f, 3.333333f, 3.666666f, 4f };

    private void Awake() {
        trans = transform;
    }

    private void LateUpdate() {
        if (GameManager.Instance && CharacterManager.Instance) {
            if (Time.timeScale == 0f) {
                restTimeRemain = 0f;
            } else {
                restTimeRemain -= Time.unscaledDeltaTime;
            }
            if (SceneChange.Instance && SceneChange.Instance.GetIsProcessing) {
                overThresholdFlag = true;
                restTimeRemain = 0f;
            }
            int speedIndex = GameManager.Instance.save.config[GameManager.Save.configID_CameraTurningSpeed];
            if (speedIndex > 0 && speedIndex < speedArray.Length) {
                if (!fromTrans && CharacterManager.Instance) {
                    fromTrans = CharacterManager.Instance.playerLookAt;
                }
                toObj = CharacterManager.Instance.GetPlayerPotentialTarget();
                if (toObj != objSave) {
                    if (toObj) {
                        toTrans = toObj.transform;
                    } else {
                        toTrans = null;
                    }
                    objSave = toObj;
                }
                if (fromTrans) {
                    float deltaTimeCache = Time.deltaTime;
                    Vector3 fromPosition = fromTrans.position;
                    Vector3 toPosition;
                    if (toTrans) {
                        toPosition = toTrans.position;
                    } else {
                        toPosition = fromPosition + fromTrans.TransformDirection(vecForward);
                    }
                    CameraManager.Instance.SetOverlook(Mathf.Clamp01((fromPosition.y - 1f - toPosition.y) * 0.3f));
                    float heightDistRateTemp = Mathf.Clamp01((toPosition.y - (fromPosition.y + 3f)) * 0.25f);
                    if (heightDistRateSave != heightDistRateTemp) {
                        if (heightDistRateTemp > heightDistRateSave) {
                            heightDistRateSave = Mathf.MoveTowards(heightDistRateSave, heightDistRateTemp, deltaTimeCache * 6f);
                        } else {
                            heightDistRateSave = Mathf.MoveTowards(heightDistRateSave, heightDistRateTemp, deltaTimeCache);
                        }
                    }
                    CameraManager.Instance.heightDistRate = heightDistRateSave;
                    toPosition.y = fromPosition.y;
                    float suppressTemp = suppressArray[Mathf.Clamp(GameManager.Instance.save.config[GameManager.Save.configID_SuppressCameraTurning], 0, suppressArray.Length - 1)];
                    float distance = Vector3.Distance(toPosition, fromPosition);
                    if (toTrans == null || Vector3.Distance(targetPosSave, toPosition) <= deltaTimeCache * 0.1f) {
                        targetStoppingTime = Mathf.Clamp(targetStoppingTime + deltaTimeCache, 0f, 4f);
                    } else {
                        targetStoppingTime = 0f;
                    }
                    if (targetStoppingTime > 0.25f) {
                        suppressTemp = Mathf.Clamp(suppressTemp - (targetStoppingTime - 0.25f) * (suppressTemp + 7) * 0.4f, Mathf.Clamp01((suppressTemp - 0.5f) * 0.4f), suppressTemp);
                    }
                    if (distance > 0.3162277f) {
                        Quaternion targetDir = Quaternion.LookRotation(toPosition - fromPosition);
                        float angle = Quaternion.Angle(trans.rotation, targetDir);
                        float speed = speedArray[speedIndex];
                        if (angle > 30f) {
                            speed *= 30f / angle;
                        }
                        if (suppressTemp <= 0) {
                            overThresholdFlag = true;
                            restTimeRemain = 0f;
                        } else {
                            if (angle > suppressTemp * 90f - distance * 18f) {
                                if (restTimeRemain <= 0f) {
                                    overThresholdFlag = true;
                                }
                            } else if (angle <= suppressTemp * 15f) {
                                if (overThresholdFlag) {
                                    restTimeRemain = suppressTemp * 0.4f;
                                }
                                overThresholdFlag = false;
                            }
                        }
                        if (overThresholdFlag) {
                            trans.rotation = Quaternion.Slerp(trans.rotation, targetDir, speed * deltaTimeCache);
                        }
                    }
                    targetPosSave = toPosition;
                }
            }
        }
    }
}
