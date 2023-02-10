using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Rewired;
using UnityEngine.PostProcessing;
using DG.Tweening;

public class CameraManager : SingletonMonoBehaviour<CameraManager> {
    
    // int lastStyle = -1;

    //ControlStyle - 0
    public Camera mainCamera;
    public GameObject[] camObj;
    public Transform[] camTrans;
    public GameObject eventCamera;
    public GameObject photoCamera;
    public CinemachineVirtualCamera cmvc_Empty;
    public CinemachineFreeLook cmvc_FreeLook;
    public CinemachineVirtualCamera cmvc_TargetGroup;
    public CinemachineVirtualCamera cmvc_Event;
    public CinemachineVirtualCamera cmvc_Photo;
    public CinemachineVirtualCamera cmvc_FirstPerson;
    public CinemachineTargetGroup targetGroup;
    public CinemachineImpulseSource impulseSource;
    public Transform followPlayerForTG;
    public Transform followPlayerForFL;
    public FixYAxisValue fixYAxisValue;
    public CinemachineImpulseListener[] impulseListeners;
    
    public const float playerRadius = 1f;
    public const float playerWeight = 2f;
    public const float targetWeight = 1f;

    //ControlStyle - 1
    public GameObject mainCamObj;

    public Transform target;
    public Vector3 offset;
    public float xSpeed = 10;
    public float ySpeed = 5;
    public float zSpeed = 10;
    public float rotSpeed = 10;

    public Transform playerSpeedPivot;
    public Vector2 adjustAxisSpeedRange;
    public float adjustAxisSmoothTime;

    [System.NonSerialized]
    public float heightBias;
    [System.NonSerialized]
    public float distanceBias;
    [System.NonSerialized]
    public float smallRate;
    [System.NonSerialized]
    public bool flexibleClippingNear;
    [System.NonSerialized]
    public float heightDistRate;
    [System.NonSerialized]
    public bool photoMoveInfinity;
    
    Player playerInput;
    Mouse mouse;
    Transform mainCamTrans;
    float direction;
    float distance;
    float height;
    float yAxisClassic = 0.5f;
    float saveTargetAngle = 0f;
    Vector3 targetPosition;
    Vector3 wantedPosition;
    Vector3 currentPosition;

    float distanceToTarget;
    PostProcessingProfile ppb_profile;
    InstantiatePostProcessingProfile instPPP;
    DepthOfFieldModel.Settings dofm_settings;

    LayerMask clippingLayerMask;
    RaycastHit clippingLayerHit;

    float deltaTimeCache;
    CinemachineTransposer cmTargetGroupTransposer;
    float targetOverlookRate = 0;
    float targetRadius;
    float nowOverlookRate = 0;
    GameObject stopFollow;
    GameObject stopLookAt;
    bool targetGroupCameraEnabled = true;
    float eventTimer = 0f;
    int horizontalDampingIndexSave = -1;
    int reserveCameraActivate;
    Transform eventCameraFollowTarget;
    Vector3 lookAtPrePosition;
    float clipParamChangeInterval;
    float freelookAxisChangedTimeRemain;
    float freelookAxisVelocity;
    float freelookAxisRunningTime;

    CinemachineVirtualCamera[] cmFreeLook_Rig = new CinemachineVirtualCamera[3];
    CinemachineComposer[] cmFreeLook_Composer = new CinemachineComposer[3];
    CinemachineGroupComposer cmTargetGroup_Composer;
    CinemachineBrain cmBrain;

    private const int emIndex = 0;
    private const int flIndex = 1;
    private const int tgIndex = 2;
    private const float distanceMin = 0.8f;
    private const float distanceDef = 1.75f;
    private const float distanceMax = 2.5f;
    private const float heightMin = 0f;
    private const float heightMax = 80f;
    private const float heightBiassmall = -16f;
    private const float distanceBiassmall = -0.5f;
    
    private Ray rayTemp = new Ray();
    private float[] horizontalDamping = new float[21] { 2.5f, 1.91f, 1.46f, 1.12f, 0.85f, 0.65f, 0.5f, 0.33f, 0.21f, 0.1f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
    private float[] blendTime = new float[21] { 3f, 2.8f, 2.6f, 2.4f, 2.2f, 2f, 1.8f, 1.6f, 1.4f, 1.2f, 1f, 0.95f, 0.9f, 0.85f, 0.8f, 0.75f, 0.7f, 0.65f, 0.6f, 0.55f, 0.5f };
    private float[] photoSpeedArray = new float[] { 2.0f, 0.5f, 20.0f, 8.0f };
    private Vector2 axisInput = Vector2.zero;
    private bool photoMoveY;
    private int photoSpeedIndex;
    private float clippingNearMin;
    private float photoMoveContinuousTime;
    private float photoMoveMultiplier = 1f;

    static readonly Vector3 vecOne = Vector3.one;
    static readonly Vector3 vecZero = Vector3.zero;
    static readonly Vector3 vecRight = Vector3.right;
    static readonly Vector3 vecUp = Vector3.up;
    static readonly Vector3 vecDown = Vector3.down;
    static readonly Vector3 vecForward = Vector3.forward;
    static readonly Vector3 vecBack = Vector3.back;
    static readonly private Vector3 followOffset0 = new Vector3(0, 4.6f, -10f);
    static readonly private Vector3 followOffset1 = new Vector3(0, 8.6f, -6f);

    void Start() {
        playerInput = GameManager.Instance.playerInput;
        mouse = playerInput.controllers.Mouse;
        mainCamTrans = mainCamObj.transform;
        SetPlayerToFirst();
        ResetCamera();
        UpdateFOV();
        clippingLayerMask = LayerMask.GetMask("Field", "SecondField");
        SetDefaultAmount();
        heightBias = 0;
        smallRate = 0f;
        SetFreeLookSpeed(GameManager.Instance.save.config[GameManager.Save.configID_CameraSensitivity]);
        instPPP = mainCamObj.GetComponent<InstantiatePostProcessingProfile>();
        cmTargetGroupTransposer = cmvc_TargetGroup.GetCinemachineComponent<CinemachineTransposer>();
        cmBrain = mainCamObj.GetComponent<CinemachineBrain>();
        stopFollow = new GameObject("StopFollow");
        stopLookAt = new GameObject("StopLookAt");
        for (int i = 0; i < 3; i++) {
            cmFreeLook_Rig[i] = cmvc_FreeLook.GetRig(i);
            cmFreeLook_Composer[i] = cmFreeLook_Rig[i].GetCinemachineComponent<CinemachineComposer>();
        }
        cmTargetGroup_Composer = cmvc_TargetGroup.GetCinemachineComponent<CinemachineGroupComposer>();
        SetHorizontalDamping(GameManager.Instance.save.config[GameManager.Save.configID_CameraTurningSpeed]);
        CheckDepthTextureMode();
        CancelQuake();
    }

    public bool SetMainCamera(ref Camera camera) {
        if (mainCamera) {
            camera = mainCamera;
            return true;
        } else {
            Camera mainTemp = Camera.main;
            if (mainTemp) {
                mainCamera = mainTemp;
                mainCamObj = mainTemp.gameObject;
                mainCamTrans = mainCamObj.transform;
                return true;
            }
        }
        return false;
    }

    public bool SetMainCameraGameObject(ref GameObject camObj) {
        if (mainCamObj) {
            camObj = mainCamObj;
            return true;
        } else {
            Camera mainTemp = Camera.main;
            if (mainTemp) {
                mainCamObj = mainTemp.gameObject;
                mainCamTrans = mainCamObj.transform;
                camObj = mainCamObj;
                return true;
            }
        }
        return false;
    }

    public bool SetMainCameraTransform(ref Transform camT) {
        if (mainCamTrans) {
            camT = mainCamTrans;
            return true;
        } else if (mainCamObj) {
            mainCamTrans = mainCamObj.transform;
            camT = mainCamTrans;
            return true;
        } else {
            Camera mainTemp = Camera.main;
            if (mainTemp) {
                mainCamObj = mainTemp.gameObject;
                mainCamTrans = mainCamObj.transform;
                camT = mainCamTrans;
                return true;
            }
        }
        return false;
    }

    public void SetHorizontalDamping(int index) {
        if (index < 0) {
            index = 5;
            stopFollow.transform.SetPositionAndRotation(CharacterManager.Instance.playerTrans.position, CharacterManager.Instance.playerTrans.rotation);
            stopLookAt.transform.SetPositionAndRotation(CharacterManager.Instance.playerLookAt.position, CharacterManager.Instance.playerLookAt.rotation);
            cmvc_FreeLook.Follow = stopFollow.transform;
            cmvc_FreeLook.LookAt = stopLookAt.transform;
            targetGroupCameraEnabled = false;
            SetActiveTargetGroupCamera(false);
        } else {
            cmvc_FreeLook.Follow = followPlayerForFL;
            SetLookAt(1);
            targetGroupCameraEnabled = true;
        }

        for (int i = 0; i < 3; i++) {
            cmFreeLook_Composer[i].m_HorizontalDamping = horizontalDamping[index];
        }
        cmTargetGroup_Composer.m_HorizontalDamping = horizontalDamping[index];
        cmBrain.m_DefaultBlend.m_Time = blendTime[index];
        cmBrain.m_CustomBlends.m_CustomBlends[2].m_Blend.m_Time = Mathf.Min(blendTime[index], 1f);
        horizontalDampingIndexSave = index;
    }

    public void SetBlendTime(float blendTime) {
        cmBrain.m_DefaultBlend.m_Time = blendTime;
        cmBrain.m_CustomBlends.m_CustomBlends[2].m_Blend.m_Time = Mathf.Min(blendTime, 1f);
    }

    public void ReloadHorizontalDamping() {
        SetHorizontalDamping(horizontalDampingIndexSave);
    }

    public void SetYAxisRate(float yAxisValue = -1f) {
        if (yAxisValue < 0f) {
            yAxisValue = GetFreeLookYAxisDefaultValue();
        }
        fixYAxisValue.targetYAxisValue = yAxisValue;
        fixYAxisValue.yAxisTargetingTimeRemain = 0.5f;
    }

    public void SetFreeLookSpeed(float rate = 5) {
        cmvc_FreeLook.m_YAxis.m_MaxSpeed = 0.4f * rate;
        cmvc_FreeLook.m_XAxis.m_MaxSpeed = 60f * rate;
    }

    public void SetControlStyle(int controlStyle) {
        if (controlStyle == 0) {
            camObj[emIndex].SetActive(false);
            camObj[flIndex].SetActive(true);
            ResetCamera();
        } else {
            camObj[emIndex].SetActive(true);
            //SetActiveTargetGroupCamera(false);
            camObj[tgIndex].SetActive(false);
            camObj[flIndex].SetActive(false);
        }
        // lastStyle = controlStyle;
        reserveCameraActivate = 0;
    }

    public void SetLookAt(int enabled) {
        if (enabled == 0) {
            cmvc_FreeLook.LookAt = null;
        } else {
            cmvc_FreeLook.LookAt = CharacterManager.Instance.playerLookAt;
        }
    }

    public void SetOverlook(float rate) {
        if (GameManager.Instance.save.config[GameManager.Save.configID_Overlook] != 0) {
            targetOverlookRate = rate;
        } else {
            targetOverlookRate = 0;
        }
    }

    void UpdateOverlook() {
        if (deltaTimeCache > 0f) {
            if (nowOverlookRate < targetOverlookRate) {
                nowOverlookRate += deltaTimeCache * 20f;
                if (nowOverlookRate > targetOverlookRate) {
                    nowOverlookRate = targetOverlookRate;
                }
            } else if (nowOverlookRate > targetOverlookRate) {
                nowOverlookRate -= deltaTimeCache * 20f;
                if (nowOverlookRate < targetOverlookRate) {
                    nowOverlookRate = targetOverlookRate;
                }
            }
            Vector3 offsetTemp = Vector3.Lerp(followOffset0, followOffset1, nowOverlookRate);
            cmTargetGroupTransposer.m_FollowOffset = offsetTemp;
            if (heightDistRate <= 0f && targetRadius < 1.8f) {
                if (cmTargetGroup_Composer.m_MinimumDistance != 3.2f) {
                    cmTargetGroup_Composer.m_MinimumDistance = 3.2f;
                }
            } else {
                cmTargetGroup_Composer.m_MinimumDistance = 3.2f + Mathf.Clamp(Mathf.Max(heightDistRate * 1.8f, (targetRadius - 1.8f) * 0.5f), 0f, 1.8f);
            }
        }
    }

    //ControlStyle - 0
    public void SetActiveTargetGroupCamera(bool flag) {
        flag = (flag && targetGroupCameraEnabled);
        if (camObj[tgIndex].activeSelf != flag) {
            /*
            if (flag) {
                camTrans[tgIndex].SetPositionAndRotation(camTrans[flIndex].position, camTrans[flIndex].rotation);
            } else {
                camTrans[flIndex].SetPositionAndRotation(camTrans[tgIndex].position, camTrans[tgIndex].rotation);
            }
            */
            if (!flag) {
                fixYAxisValue.yAxisFixReserved = 2;
            }
            camObj[tgIndex].SetActive(flag);
        }
    }

    void SetPlayerToFirst() {
        //targetGroup.m_Targets[0].target = CharacterManager.Instance.PlayerSearchTarget;
        targetGroup.m_Targets[0].weight = playerWeight;
        targetGroup.m_Targets[0].radius = playerRadius;
        cmvc_FreeLook.Follow = followPlayerForFL;
        SetLookAt(1);
    }

    public float MouseWheelBias() {
        if (mouse != null && mouse.axisCount > 2) {
            float mouseNum = mouse.GetAxis(2);
            if (mouseNum != 0f) {
                return Mathf.Clamp(Mathf.Abs(mouseNum * 10f), 1f, 100f);
            }
        }
        return 1f;
    }

    bool GetCameraControlEnabled() {
        switch (GameManager.Instance.save.config[GameManager.Save.configID_CameraControlButton]) {
            case 0:
                return true;
            case 1:
                return playerInput.GetButton(RewiredConsts.Action.Camera_Control);
            case 2:
                return playerInput.GetButton(RewiredConsts.Action.Camera_Control) || (CharacterManager.Instance && CharacterManager.Instance.pCon && CharacterManager.Instance.pCon.GetNowTarget() == null);
        }
        return true;
    }

    public float GetCameraHorizontal() {
        float horizontal = playerInput.GetAxis(RewiredConsts.Action.Camera_Horizontal);
        if (playerInput.IsCurrentInputSource(RewiredConsts.Action.Camera_Horizontal, ControllerType.Mouse)) {
            if (GetCameraControlEnabled()) {
                horizontal *= 1.5f;
                horizontal *= MouseWheelBias();
            } else {
                horizontal = 0f;
            }
        }
        if (GameManager.Instance.save.config[GameManager.Save.configID_CameraAxisInvert] % 2 > 0) {
            horizontal *= (-1);
        }
        return horizontal;
    }

    public float GetCameraVertical() {
        float vertical = playerInput.GetAxis(RewiredConsts.Action.Camera_Vertical);
        if (playerInput.IsCurrentInputSource(RewiredConsts.Action.Camera_Vertical, ControllerType.Mouse)) {
            if (GetCameraControlEnabled()) {
                vertical *= 0.5f;
                vertical *= MouseWheelBias();
            } else {
                vertical = 0f;
            }
            if (mouse != null && mouse.axisCount > 2) {
                float mouseNum = mouse.GetAxis(2);
                if (mouseNum != 0f) {
                    vertical *= (Mathf.Abs(mouseNum * 10f));
                }
            }
        }
        if (GameManager.Instance.save.config[GameManager.Save.configID_CameraAxisInvert] >= 2) {
            vertical *= (-1);
        }
        return vertical;
    }

    void RewiredToFreeLook() {
        float horizontalValue = GetCameraHorizontal();
        float verticalValue = GetCameraVertical();
        cmvc_FreeLook.m_XAxis.m_InputAxisValue = horizontalValue;
        cmvc_FreeLook.m_YAxis.m_InputAxisValue = verticalValue;
        if (GameManager.Instance.save.config[GameManager.Save.configID_FirstPerson] != 0 && CharacterManager.Instance.pCon) {
            CharacterManager.Instance.pCon.SetFirstPersonHV(horizontalValue, verticalValue);
        }
        if (horizontalValue != 0f || verticalValue != 0f || playerInput.GetButton(RewiredConsts.Action.Camera_Control)) {
            freelookAxisChangedTimeRemain = 1f;
        }
    }

    void CheckTargetMissing() {
        if (!CharacterManager.Instance.playerTarget) {
            targetGroup.m_Targets[1].weight = 0;
            targetGroup.m_Targets[1].radius = 0;
        }
    }

    public void SetTarget(Transform trans, float radius, float weight = 1f) {
        if (trans && GameManager.Instance.save.config[GameManager.Save.configID_CameraTurningSpeed] > 0) {
            targetRadius = radius;
            targetGroup.m_Targets[1].radius = radius;
            targetGroup.m_Targets[1].weight = targetWeight * weight;
        } else {
            if (targetGroup.m_Targets[1].weight != 0) {
                targetRadius = radius;
                targetGroup.m_Targets[1].weight = 0;
                targetGroup.m_Targets[1].radius = 0;
            }
        }
    }

    public void OverwriteTargetRadius(float radius) {
        if (GameManager.Instance.save.config[GameManager.Save.configID_CameraTurningSpeed] > 0) {
            targetRadius = radius;
            targetGroup.m_Targets[1].radius = radius;
        }
    }

    public void ResetCamera() {
        cmvc_FreeLook.m_YAxis.Value = GetFreeLookYAxisDefaultValue();
        SetActiveTargetGroupCamera(false);
        SetDefaultAmount();
        saveTargetAngle = 0f;
        if (eventTimer > 0f) {
            eventTimer = 0.002f;
        }
        if (CharacterManager.Instance.pCon) {
            CharacterManager.Instance.pCon.ResetFirstPersonHV();
        }
    }

    public void ResetCameraFixPos() {
        ResetCamera();
        if (CharacterManager.Instance.playerTrans && CharacterManager.Instance.playerLookAt) {
            Vector3 resetPosition = CharacterManager.Instance.playerTrans.position + CharacterManager.Instance.playerTrans.forward * -3.2f;
            resetPosition.y += 1.6f;
            for (int i = 0; i < camTrans.Length; i++) {
                if (camObj[i].activeSelf) {
                    camObj[i].SetActive(false);
                }
                camTrans[i].position = resetPosition;
            }
            eventCamera.transform.SetPositionAndRotation(resetPosition, Quaternion.LookRotation(CharacterManager.Instance.playerLookAt.position - resetPosition));
            photoCamera.transform.position = resetPosition;
        }
        reserveCameraActivate = 2;
    }

    public void FreeLook_OnTargetObjectWarped(Vector3 positionDelta) {
        cmvc_FreeLook.OnTargetObjectWarped(cmvc_FreeLook.LookAt, positionDelta);
    }

    //ControlStyle - 1
    void SetDefaultAmount() {
        direction = 180f;
        distance = 1.75f;
        height = 20f;
        yAxisClassic = 0.45f;
    }

    void GetTarget() {
        if (CharacterManager.Instance != null) {
            target = CharacterManager.Instance.playerTrans;
        }
    }

    void SmoothDampFreelookYAxis() {
        if (deltaTimeCache > 0f) {
            if (freelookAxisChangedTimeRemain <= 0f) {
                float returnSpeed = GetFreeLookYAxisReturnSpeed();
                if (returnSpeed > 0f) {
                    float defValue = GetFreeLookYAxisDefaultValue();
                    if (playerSpeedPivot && CharacterManager.Instance && CharacterManager.Instance.playerTrans && cmvc_FreeLook.m_YAxis.Value != defValue) {
                        Vector3 posA = CharacterManager.Instance.playerTrans.position;
                        Vector3 posB = playerSpeedPivot.position;
                        posA.y = posB.y;
                        float dist = Vector3.Distance(posA, posB);
                        if (dist > adjustAxisSpeedRange.x) {
                            freelookAxisRunningTime += deltaTimeCache;
                            if (freelookAxisRunningTime >= 0.5f / returnSpeed) {
                                cmvc_FreeLook.m_YAxis.Value = Mathf.SmoothDamp(cmvc_FreeLook.m_YAxis.Value, defValue, ref freelookAxisVelocity, adjustAxisSmoothTime / returnSpeed / Mathf.Clamp((dist - adjustAxisSpeedRange.x) / (adjustAxisSpeedRange.y - adjustAxisSpeedRange.x), 0.25f, 1f));
                            }
                        } else {
                            freelookAxisRunningTime = 0f;
                        }
                    }
                }
            } else {
                freelookAxisChangedTimeRemain -= deltaTimeCache;
            }
        }
    }

    void CalcTargetPosition() {
        targetPosition = target.position + offset;
    }

    void SetDistanceToTarget() {
        distanceToTarget = Vector3.Distance(mainCamTrans.position, targetPosition);
    }

    void SetFocusDistance() {
        if (ppb_profile && !eventCamera.activeSelf) {
            if (GameManager.Instance.save.config[GameManager.Save.configID_FirstPerson] == 0) {
                dofm_settings.focusDistance = distanceToTarget;
            } else {
                dofm_settings.focusDistance = 1.5f;
            }
            ppb_profile.depthOfField.settings = dofm_settings;
        }
    }

    void ForceFocusDistance(float focusDist) {
        if (ppb_profile) {
            dofm_settings.focusDistance = focusDist;
            ppb_profile.depthOfField.settings = dofm_settings;
        }
    }

    float GetFreeLookClippingNear() {
        float yValue = cmvc_FreeLook.m_YAxis.Value;
        if (yValue >= 0.25f) {
            return clippingNearMin;
        } else {
            return (clippingNearMin - 0.1f) * (yValue * 4f) + 0.1f;
        }
    }

    public float GetActualClippingNear() {
        if (cmvc_FreeLook) {
            return cmvc_FreeLook.m_Lens.NearClipPlane;
        } else {
            return clippingNearMin;
        }
    }

    float GetClippingToPlayerOffset() {
        if (CharacterManager.Instance && CharacterManager.Instance.pCon) {
            return Mathf.Clamp(0.3f + CharacterManager.Instance.pCon.GetNowSpeed() * 0.02f, 0.3f, 0.4f);
        } else {
            return 0.3f;
        }
    }

    void SetClippingNear() {
        if (Time.timeScale > 0) {
            float currentVelocity = 0;
            bool clipActive = false;
            if (((GameManager.Instance.save.config[GameManager.Save.configID_ClippingAutoAdjust] == 1 || GameManager.Instance.save.config[GameManager.Save.configID_ClippingAutoAdjust] == 3) && flexibleClippingNear) || (GameManager.Instance.save.config[GameManager.Save.configID_ClippingAutoAdjust] == 2)) {
                rayTemp.origin = targetPosition;
                rayTemp.direction = mainCamTrans.position - targetPosition;
                if (Physics.Raycast(rayTemp, out clippingLayerHit, distanceToTarget, clippingLayerMask, QueryTriggerInteraction.Ignore)) {
                    float clipParam = Mathf.Max(distanceToTarget - clippingLayerHit.distance + 0.05f, clippingNearMin);
                    float clippingNearMax = Mathf.Max(distanceToTarget - GetClippingToPlayerOffset(), clippingNearMin);
                    clipActive = true;
                    if (GameManager.Instance.save.config[GameManager.Save.configID_ClippingAutoAdjust] != 3) {
                        cmvc_Empty.m_Lens.NearClipPlane = Mathf.Clamp(Mathf.SmoothDamp(cmvc_Empty.m_Lens.NearClipPlane, clipParam, ref currentVelocity, 0.1f), clippingNearMin, clippingNearMax);
                        cmvc_FreeLook.m_Lens.NearClipPlane = Mathf.Clamp(Mathf.SmoothDamp(cmvc_FreeLook.m_Lens.NearClipPlane, clipParam, ref currentVelocity, 0.1f), clippingNearMin, clippingNearMax);
                        cmvc_TargetGroup.m_Lens.NearClipPlane = Mathf.Clamp(Mathf.SmoothDamp(cmvc_TargetGroup.m_Lens.NearClipPlane, clipParam, ref currentVelocity, 0.1f), clippingNearMin, clippingNearMax);
                    } else {
                        clipParamChangeInterval -= deltaTimeCache;
                        if (clipParamChangeInterval < 0f) {
                            bool changedFlag = false;
                            clipParam = Mathf.Clamp(clipParam, clippingNearMin, clippingNearMax);
                            if (Mathf.Abs(cmvc_Empty.m_Lens.NearClipPlane - clipParam) >= 0.3f) {
                                cmvc_Empty.m_Lens.NearClipPlane = clipParam;
                                changedFlag = true;
                            }
                            if (Mathf.Abs(cmvc_FreeLook.m_Lens.NearClipPlane - clipParam) >= 0.3f) {
                                cmvc_FreeLook.m_Lens.NearClipPlane = clipParam;
                                changedFlag = true;
                            }
                            if (Mathf.Abs(cmvc_TargetGroup.m_Lens.NearClipPlane - clipParam) >= 0.3f) {
                                cmvc_TargetGroup.m_Lens.NearClipPlane = clipParam;
                                changedFlag = true;
                            }
                            if (changedFlag) {
                                clipParamChangeInterval = 0.4f;
                            }
                        }
                    }
                }
            }
            if (!clipActive && cmvc_Empty.m_Lens.NearClipPlane != clippingNearMin) {
                if (GameManager.Instance.save.config[GameManager.Save.configID_ClippingAutoAdjust] != 3) {
                    float clippingNearMax = Mathf.Max(distanceToTarget - GetClippingToPlayerOffset(), clippingNearMin);
                    cmvc_Empty.m_Lens.NearClipPlane = Mathf.Clamp(Mathf.SmoothDamp(cmvc_Empty.m_Lens.NearClipPlane, clippingNearMin, ref currentVelocity, 0.1f), clippingNearMin, clippingNearMax);
                    cmvc_FreeLook.m_Lens.NearClipPlane = Mathf.Clamp(Mathf.SmoothDamp(cmvc_FreeLook.m_Lens.NearClipPlane, GetFreeLookClippingNear(), ref currentVelocity, 0.1f), clippingNearMin, clippingNearMax);
                    cmvc_TargetGroup.m_Lens.NearClipPlane = Mathf.Clamp(Mathf.SmoothDamp(cmvc_TargetGroup.m_Lens.NearClipPlane, clippingNearMin, ref currentVelocity, 0.1f), clippingNearMin, clippingNearMax);
                } else {
                    clipParamChangeInterval -= deltaTimeCache;
                    if (clipParamChangeInterval < 0f) {
                        bool changedFlag = false;
                        if (Mathf.Abs(cmvc_Empty.m_Lens.NearClipPlane - clippingNearMin) >= 0.3f) {
                            cmvc_Empty.m_Lens.NearClipPlane = clippingNearMin;
                            changedFlag = true;
                        }
                        if (Mathf.Abs(cmvc_FreeLook.m_Lens.NearClipPlane - GetFreeLookClippingNear()) >= 0.3f) {
                            cmvc_FreeLook.m_Lens.NearClipPlane = GetFreeLookClippingNear();
                            changedFlag = true;
                        }
                        if (Mathf.Abs(cmvc_TargetGroup.m_Lens.NearClipPlane - clippingNearMin) >= 0.3f) {
                            cmvc_TargetGroup.m_Lens.NearClipPlane = clippingNearMin;
                            changedFlag = true;
                        }
                        if (changedFlag) {
                            clipParamChangeInterval = 0.4f;
                        }
                    }
                }
            }
        }
    }

    public void SetEventCamera(Vector3 position, Vector3 eulerAngles, float timer = 5f, float blendTime = 1.5f, float focusDistance = 3f, float clippingDistance = 0f) {
        if (blendTime >= 0f) {
            SetBlendTime(blendTime);
        }
        if (eventCamera) {
            eventCamera.transform.position = position;
            eventCamera.transform.eulerAngles = eulerAngles;
            cmvc_Event.m_Lens.NearClipPlane = Mathf.Max(0.1f, clippingDistance);
            if (!eventCamera.activeSelf) {
                eventCamera.SetActive(true);
            }
            eventTimer = timer;
            ForceFocusDistance(focusDistance);
        }
        if (eventCameraFollowTarget) {
            eventCameraFollowTarget = null;
        }
    }

    public void SetEventCameraFollowTarget(Transform target, float timer = 5f, float blendTime = 1.5f, float focusDistance = 3f, float clippingDistance = 0f) {
        if (target) {
            SetEventCamera(target.position, target.eulerAngles, timer, blendTime, focusDistance, clippingDistance);
        }
        eventCameraFollowTarget = target;
    }

    public void SetEventCameraTweening(Vector3 position, Vector3 eulerAngles, float timer = 5f, float blendTime = 1.5f, float focusDistance = 3f, float clippingDistance = 0f) {
        if (blendTime >= 0f) {
            SetBlendTime(blendTime);
        }
        if (eventCamera) {
            eventCamera.transform.DOMove(position, blendTime);
            eventCamera.transform.DORotate(eulerAngles, blendTime);
            cmvc_Event.m_Lens.NearClipPlane = Mathf.Max(0.1f, clippingDistance);
            if (!eventCamera.activeSelf) {
                eventCamera.SetActive(true);
            }
            eventTimer = timer;
            ForceFocusDistance(focusDistance);
        }
        if (eventCameraFollowTarget) {
            eventCameraFollowTarget = null;
        }
    }

    public void SetEventTimer(float timer) {
        eventTimer = timer;
    }

    void CheckEventCamera() {
        if (eventCameraFollowTarget) {
            eventCamera.transform.position = eventCameraFollowTarget.position;
            eventCamera.transform.eulerAngles = eventCameraFollowTarget.eulerAngles;
        }
        if (eventTimer > 0f && eventTimer < 99999f) {
            eventTimer -= deltaTimeCache;
            if (eventTimer <= 0f && eventCamera && eventCamera.activeSelf) {
                if (eventCameraFollowTarget) {
                    eventCameraFollowTarget = null;
                }
                ReloadHorizontalDamping();
                fixYAxisValue.yAxisFixReserved = 2;
                eventCamera.SetActive(false);
            }
        }
    }

    public float GetFreeLookYAxisDefaultValue() {
        return Mathf.Clamp01(0.5f + GameManager.Instance.save.config[GameManager.Save.configID_CameraAxisDefault] * 0.025f);
    }

    public float GetFreeLookYAxisReturnSpeed() {
        return Mathf.Clamp(1f + GameManager.Instance.save.config[GameManager.Save.configID_CameraReturnSpeed] * 0.1f, 0f, 5f);
    }

    public void SetFreeLookYAxisToDefault() {
        fixYAxisValue.targetYAxisValue = cmvc_FreeLook.m_YAxis.Value = GetFreeLookYAxisDefaultValue();
        fixYAxisValue.yAxisFixReserved = 2;
    }

    void CheckPhotoCamera() {
        bool photoEnabled = (PauseController.Instance && PauseController.Instance.IsPhotoPausing);
        bool initFlag = false;
        if (photoCamera.activeSelf != photoEnabled) {
            if (photoEnabled) {
                photoSpeedIndex = 0;
                photoMoveY = false;
                initFlag = true;
                photoCamera.transform.SetPositionAndRotation(mainCamTrans.position, mainCamTrans.rotation);
            } else {
                cmvc_FreeLook.m_YAxis.Value = GetFreeLookYAxisDefaultValue();
                fixYAxisValue.yAxisFixReserved = 2;
            }
            photoCamera.SetActive(photoEnabled);
        }
        if (photoEnabled) {
            bool movedFlag = false;
            float horizontal = playerInput.GetAxis(RewiredConsts.Action.Camera_Horizontal);
            float vertical = playerInput.GetAxis(RewiredConsts.Action.Camera_Vertical);
            axisInput.x = playerInput.GetAxis(RewiredConsts.Action.Horizontal);
            axisInput.y = playerInput.GetAxis(RewiredConsts.Action.Vertical);
            Vector3 posTemp = photoCamera.transform.position;
            Vector3 rotTemp = photoCamera.transform.eulerAngles;
            rotTemp.z = 0f;
            if (playerInput.GetButtonDown(RewiredConsts.Action.Dodge) && UISE.Instance) {
                UISE.Instance.Play(UISE.SoundName.move);
                photoSpeedIndex = (photoSpeedIndex + 1) % photoSpeedArray.Length;
                initFlag = true;
            }
            if (playerInput.GetButtonDown(RewiredConsts.Action.Submit) && UISE.Instance) {
                UISE.Instance.Play(UISE.SoundName.submit);
                photoMoveY = !photoMoveY;
                initFlag = true;
            }
            if (axisInput.x != 0 || axisInput.y != 0) {
                photoMoveContinuousTime += Time.unscaledDeltaTime;
            } else {
                photoMoveContinuousTime = 0f;
            }
            if (photoSpeedArray[photoSpeedIndex] > 15f && photoMoveContinuousTime > 3f) {
                photoMoveMultiplier = (photoMoveContinuousTime - 3f) / 3f + 1f;
            } else {
                photoMoveMultiplier = 1f;
            }
            if (horizontal != 0f) {
                movedFlag = true;
                if (playerInput.IsCurrentInputSource(RewiredConsts.Action.Camera_Horizontal, ControllerType.Mouse)) {
                    if (GameManager.Instance.save.config[GameManager.Save.configID_CameraControlButton] == 0 || playerInput.GetButton(RewiredConsts.Action.Camera_Control)) {
                        horizontal *= 0.2f;
                        horizontal *= MouseWheelBias();
                    } else {
                        horizontal = 0f;
                    }
                }
                if (GameManager.Instance.save.config[GameManager.Save.configID_CameraAxisInvert] % 2 > 0) {
                    horizontal *= (-1);
                }
                rotTemp.y += horizontal * 60f * Time.unscaledDeltaTime;
                if (rotTemp.y > 180f) {
                    rotTemp.y -= 360f;
                } else if (rotTemp.y < -180f) {
                    rotTemp.y += 360f;
                }
            }
            if (vertical != 0f) {
                movedFlag = true;
                if (playerInput.IsCurrentInputSource(RewiredConsts.Action.Camera_Vertical, ControllerType.Mouse)) {
                    if (GameManager.Instance.save.config[GameManager.Save.configID_CameraControlButton] == 0 || playerInput.GetButton(RewiredConsts.Action.Camera_Control)) {
                        vertical *= 0.2f;
                        vertical *= MouseWheelBias();
                    } else {
                        vertical = 0f;
                    }
                }
                if (GameManager.Instance.save.config[GameManager.Save.configID_CameraAxisInvert] >= 2) {
                    vertical *= (-1);
                }
                rotTemp.x += vertical * 45f * Time.unscaledDeltaTime;
                if (rotTemp.x > 90f && rotTemp.x < 180f) {
                    rotTemp.x = 90f;
                } else if (rotTemp.x > 180f && rotTemp.x < 270f) {
                    rotTemp.x = 270f;
                }
            }
            if (axisInput.x != 0) {
                movedFlag = true;
                Vector3 vecTemp = photoCamera.transform.TransformDirection(vecRight);
                vecTemp.y = 0f;
                posTemp += vecTemp.normalized * axisInput.x * photoSpeedArray[photoSpeedIndex] * photoMoveMultiplier * Time.unscaledDeltaTime;
            }
            if (axisInput.y != 0) {
                movedFlag = true;
                if (photoMoveY) {
                    posTemp += vecUp * axisInput.y * photoSpeedArray[photoSpeedIndex] * photoMoveMultiplier * Time.unscaledDeltaTime;
                } else {
                    Vector3 vecTemp = photoCamera.transform.TransformDirection(vecForward);
                    vecTemp.y = 0f;
                    vecTemp.Normalize();
                    if (vecTemp == vecZero) {
                        float elevation = rotTemp.x;
                        if (elevation > 0f && elevation < 180f) {
                            vecTemp = photoCamera.transform.TransformDirection(vecUp);
                        } else if (elevation >= 180f) {
                            vecTemp = photoCamera.transform.TransformDirection(vecDown);
                        }
                        vecTemp.y = 0f;
                        vecTemp.Normalize();
                    }
                    posTemp += vecTemp * axisInput.y * photoSpeedArray[photoSpeedIndex] * photoMoveMultiplier * Time.unscaledDeltaTime;
                }
            }
            if (playerInput.GetButtonDown(RewiredConsts.Action.Aim)) {
                if (GameManager.Instance.save.config[GameManager.Save.configID_PhotoMode] == 1) {
                    GameManager.Instance.save.config[GameManager.Save.configID_PhotoMode] = 2;
                } else {
                    GameManager.Instance.save.config[GameManager.Save.configID_PhotoMode] = 1;
                }
                if (CanvasCulling.Instance) {
                    if (GameManager.Instance.save.config[GameManager.Save.configID_PhotoMode] == 1) {
                        CanvasCulling.Instance.CheckConfig(0, 0);
                        CanvasCulling.Instance.SetMapCameraEnabled(false);
                    } else {
                        CanvasCulling.Instance.CheckConfig();
                        CanvasCulling.Instance.SetMapCameraEnabled(true);
                    }
                }
                if (PauseController.Instance && PauseController.Instance.offPauseCanvas) {
                    if (GameManager.Instance.save.config[GameManager.Save.configID_PhotoMode] == 1) {
                        PauseController.Instance.offPauseCanvas.enabled = false;
                    } else {
                        PauseController.Instance.offPauseCanvas.enabled = true;
                    }
                }
                UISE.Instance.Play(UISE.SoundName.submit);
            }
            if (InstantiatePostProcessingProfile.Instance && playerInput.GetButtonDown(RewiredConsts.Action.Wild_Release)) {
                movedFlag = true;
                if (GameManager.Instance.save.config[GameManager.Save.configID_DepthOfField] == 0) {
                    GameManager.Instance.save.config[GameManager.Save.configID_DepthOfField] = 2;
                } else {
                    GameManager.Instance.save.config[GameManager.Save.configID_DepthOfField] = 0;
                }
                CheckDepthTextureMode();
                InstantiatePostProcessingProfile.Instance.QualitySettingsAdjustments();
                UISE.Instance.Play(UISE.SoundName.submit);
            }
            if (playerInput.GetButtonDown(RewiredConsts.Action.Pause) && PauseController.Instance && PauseController.Instance.photo.canvas) {
                movedFlag = true;
                PauseController.Instance.photo.canvas.enabled = !PauseController.Instance.photo.canvas.enabled;
                UISE.Instance.Play(UISE.SoundName.submit);
            }
            if (movedFlag) {
                if (!photoMoveInfinity && CharacterManager.Instance && CharacterManager.Instance.playerTrans) {
                    Vector3 playerPosition = CharacterManager.Instance.playerTrans.position;
                    Vector3 playerPosIgnoreY = playerPosition;
                    playerPosIgnoreY.y = posTemp.y;
                    if ((playerPosIgnoreY - posTemp).sqrMagnitude > 6f * 6f) {
                        posTemp = playerPosIgnoreY + (posTemp - playerPosIgnoreY).normalized * 6f;
                    }
                    if (posTemp.y > playerPosition.y + 5f) {
                        posTemp.y = playerPosition.y + 5f;
                    } else if (posTemp.y < playerPosition.y + 0.1f) {
                        posTemp.y = playerPosition.y + 0.1f;
                    }
                }
                photoCamera.transform.SetPositionAndRotation(posTemp, Quaternion.Euler(rotTemp));
            }
            if (movedFlag || initFlag) {
                if (PauseController.Instance) {
                    float azimuth = rotTemp.y + 180f;
                    float elevation = rotTemp.x;
                    if (azimuth < 0f) {
                        azimuth += 360f;
                    } else if (azimuth >= 360f) {
                        azimuth -= 360f;
                    }
                    if (elevation > 0f && elevation < 180f) {
                        elevation = elevation * -1f;
                    } else if (elevation >= 180f) {
                        elevation = elevation * -1f + 360f;
                    }
                    PauseController.Instance.UpdatePhotoControlText(photoMoveY, photoSpeedArray[photoSpeedIndex] * photoMoveMultiplier, posTemp, azimuth, elevation);
                }
            }
        }
    }

    public void UpdateDepthOfFieldSettings() {
        if (!ppb_profile) {
            ppb_profile = mainCamObj.GetComponent<PostProcessingBehaviour>().profile;
        }
        if (ppb_profile && instPPP && instPPP.isChanged) {
            dofm_settings = ppb_profile.depthOfField.settings;
            instPPP.isChanged = false;
        }
    }

    public void SetQuake(Vector3 epicenter, float amplitude = 10f, float frequency = 4f, float attackTime = 0f, float sustainTime = 0f, float decayTime = 1f, float impactRadius = 1f, float dissipationDistance = 50f, bool ignoreConfig = false) {
        if (ignoreConfig || GameManager.Instance.save.config[GameManager.Save.configID_CameraVibration] != 0) {
            impulseSource.m_ImpulseDefinition.m_AmplitudeGain = amplitude * (ignoreConfig || GameManager.Instance.save.config[GameManager.Save.configID_CameraVibration] >= 2 ? 1f : 0.5f);
            impulseSource.m_ImpulseDefinition.m_FrequencyGain = frequency;
            impulseSource.m_ImpulseDefinition.m_TimeEnvelope.m_AttackTime = attackTime;
            impulseSource.m_ImpulseDefinition.m_TimeEnvelope.m_SustainTime = sustainTime;
            impulseSource.m_ImpulseDefinition.m_TimeEnvelope.m_DecayTime = decayTime;
            impulseSource.m_ImpulseDefinition.m_ImpactRadius = impactRadius;
            impulseSource.m_ImpulseDefinition.m_DissipationDistance = dissipationDistance;
            impulseSource.GenerateImpulseAt(epicenter, vecOne);
        }
        float distance = Vector3.Distance(epicenter, target.position);
        if (distance < dissipationDistance) {
            float amount = amplitude * (1 / Mathf.Pow(2, Mathf.Log10(Mathf.Max(distance * 100 / Mathf.Max(10f, dissipationDistance), 1f)))) * 0.15f;
            if (amount >= 0.2f) {
                GameManager.Instance.SetVibration(Mathf.Clamp01(amount), attackTime + sustainTime + decayTime / 2, false);
            }
        }
    }

    public void CancelQuake() {
        if (CinemachineImpulseManager.Instance != null) {
            CinemachineImpulseManager.Instance.Clear();
        }
    }

    void CheckImpulseListener() {
        bool impulseEnabled = (Time.timeScale > 0);
        if (impulseListeners[0].enabled != impulseEnabled) {
            for (int i = 0; i < impulseListeners.Length; i++) {
                impulseListeners[i].enabled = impulseEnabled;
            }
        }
    }

    public void CheckDepthTextureMode() {
        if (mainCamera == null) {
            mainCamera = Camera.main;
        }
        if (mainCamera) {
            bool depth = (GameManager.Instance.save.config[GameManager.Save.configID_Antialiasing] >= 2 || GameManager.Instance.save.config[GameManager.Save.configID_AmbientOcclusion] != 0 || GameManager.Instance.save.config[GameManager.Save.configID_DepthOfField] != 0 || GameManager.Instance.save.config[GameManager.Save.configID_MotionBlur] != 0 || GameManager.Instance.save.config[GameManager.Save.configID_QualityLevel] >= 4);
            bool depthNormals = (GameManager.Instance.save.config[GameManager.Save.configID_AmbientOcclusion] != 0);
            bool motionVectors = (GameManager.Instance.save.config[GameManager.Save.configID_Antialiasing] >= 2 || GameManager.Instance.save.config[GameManager.Save.configID_MotionBlur] != 0);
            DepthTextureMode depthTemp = DepthTextureMode.None;
            if (depth) {
                depthTemp |= DepthTextureMode.Depth;
            }
            if (depthNormals) {
                depthTemp |= DepthTextureMode.DepthNormals;
            }
            if (motionVectors) {
                depthTemp |= DepthTextureMode.MotionVectors;
            }
            if (mainCamera.depthTextureMode != depthTemp) {
                mainCamera.depthTextureMode = depthTemp;
            }
        }
    }

    private void FollowBack() {
        wantedPosition = targetPosition;
        currentPosition = camTrans[emIndex].position;
        int cameraSpeed = GameManager.Instance.save.config[GameManager.Save.configID_CameraTurningSpeed];
        float tempAngle = target.eulerAngles.y;
        float distanceTemp;
        float heightTemp;
        float horizontal = playerInput.GetAxis(RewiredConsts.Action.Camera_Horizontal);
        float vertical = playerInput.GetAxis(RewiredConsts.Action.Camera_Vertical);
        if (horizontal != 0f) {
            if (playerInput.IsCurrentInputSource(RewiredConsts.Action.Camera_Horizontal, ControllerType.Mouse)) {
                if (GetCameraControlEnabled()) {
                    horizontal *= 0.2f;
                    horizontal *= MouseWheelBias();
                } else {
                    horizontal = 0f;
                }
            }
            if (GameManager.Instance.save.config[GameManager.Save.configID_CameraAxisInvert] % 2 > 0) {
                horizontal *= (-1);
            }
            direction += horizontal * 50f * GameManager.Instance.save.config[GameManager.Save.configID_CameraSensitivity] * deltaTimeCache;
        }
        if (vertical != 0f) {
            if (playerInput.IsCurrentInputSource(RewiredConsts.Action.Camera_Vertical, ControllerType.Mouse)) {
                if (GetCameraControlEnabled()) {
                    vertical *= 0.2f;
                    vertical *= MouseWheelBias();
                } else {
                    vertical = 0f;
                }
            }
            if (GameManager.Instance.save.config[GameManager.Save.configID_CameraAxisInvert] >= 2) {
                vertical *= (-1);
            }
            yAxisClassic += vertical * 0.3f * GameManager.Instance.save.config[GameManager.Save.configID_CameraSensitivity] * deltaTimeCache;
            yAxisClassic = Mathf.Clamp01(yAxisClassic);
            height = Mathf.Clamp(yAxisClassic - 0.25f, 0, 0.75f) * 100f;
            if (yAxisClassic < 0.25f) {
                distance = Mathf.Lerp(distanceMin, distanceDef, yAxisClassic * 4f);
            } else if (yAxisClassic > 0.75f) {
                distance = Mathf.Lerp(distanceDef, distanceMax, (yAxisClassic - 0.75f) * 4f);
            } else {
                distance = distanceDef;
            }
        }
        distanceTemp = Mathf.Max(Mathf.Min(distance + distanceBias + distanceBiassmall * smallRate, distanceMax), distanceMin);
        heightTemp = Mathf.Max(Mathf.Min(height + heightBias + heightBiassmall * smallRate, heightMax), heightMin);
        if (direction < 0.0f) {
            direction += 360.0f;
        } else if (direction >= 360.0f) {
            direction -= 360.0f;
        }
        if (cameraSpeed == 0) {
            tempAngle = saveTargetAngle;
        } else if (cameraSpeed <= 9) {
            if (tempAngle - saveTargetAngle > 180f) {
                saveTargetAngle += 360f;
            } else if (tempAngle - saveTargetAngle < -180f) {
                saveTargetAngle -= 360f;
            }
            tempAngle = Mathf.Lerp(saveTargetAngle, tempAngle, (cameraSpeed + 1) * (cameraSpeed + 1) * deltaTimeCache);
            saveTargetAngle = tempAngle;
        } else {
            saveTargetAngle = tempAngle;
        }
        wantedPosition.x += Mathf.Sin((tempAngle + direction) * Mathf.PI / 180.0f) * distanceTemp * distanceTemp * Mathf.Cos(heightTemp * Mathf.Deg2Rad);
        wantedPosition.z += Mathf.Cos((tempAngle + direction) * Mathf.PI / 180.0f) * distanceTemp * distanceTemp * Mathf.Cos(heightTemp * Mathf.Deg2Rad);
        wantedPosition.y += distanceTemp * distanceTemp * Mathf.Sin(heightTemp * Mathf.Deg2Rad);
        currentPosition.x = Mathf.Lerp(currentPosition.x, wantedPosition.x, xSpeed * deltaTimeCache);
        currentPosition.y = Mathf.Lerp(currentPosition.y, wantedPosition.y, ySpeed * deltaTimeCache);
        currentPosition.z = Mathf.Lerp(currentPosition.z, wantedPosition.z, zSpeed * deltaTimeCache);
        camTrans[emIndex].SetPositionAndRotation(currentPosition, Quaternion.Slerp(camTrans[emIndex].rotation, Quaternion.LookRotation(targetPosition - camTrans[emIndex].position), rotSpeed * deltaTimeCache));
    }

    private void Update() {
        deltaTimeCache = Time.deltaTime;
        CheckTargetMissing();
        UpdateOverlook();
    }

    private void LateUpdate() {
        /*
        if (lastStyle != GameManager.Instance.save.config[GameManager.Save.configID_moveStyle || reserveCameraActivate == 1) {
            SetControlStyle(GameManager.Instance.save.config[GameManager.Save.configID_moveStyle);
        } else if (reserveCameraActivate > 1) {
            reserveCameraActivate--;
        }
        */
        if (reserveCameraActivate == 1) {
            SetControlStyle(0);
        } else if (reserveCameraActivate > 1) {
            reserveCameraActivate--;
        }
        clippingNearMin = GameManager.Instance.save.config[GameManager.Save.configID_ClippingDistance] * 0.1f + 0.1f;
        if (!target) {
            GetTarget();
        }
        CheckPhotoCamera();
        CheckEventCamera();
        CheckTargetMissing();
        CalcTargetPosition();
        CheckImpulseListener();
        UpdateDepthOfFieldSettings();
        /*
        if (GameManager.Instance.save.config[GameManager.Save.configID_moveStyle == 1) {
            FollowBack();
        }
        */
        if (eventTimer <= 0f && !(GameManager.Instance.MouseEnabled && PauseController.Instance && (PauseController.Instance.pauseGame || PauseController.Instance.notPausingFrames <= 1)) && playerInput.GetButtonDown(RewiredConsts.Action.Camera_Reset)) {
            ResetCamera();
        }
        RewiredToFreeLook();
        SmoothDampFreelookYAxis();
        SetDistanceToTarget();
        SetFocusDistance();
        SetClippingNear();
        SetFirstPerson();
    }

    public void SetClippingFar(float param) {
        if (cmvc_Empty.m_Lens.FarClipPlane != param) {
            cmvc_Empty.m_Lens.FarClipPlane = param;
            cmvc_FreeLook.m_Lens.FarClipPlane = param;
            cmvc_TargetGroup.m_Lens.FarClipPlane = param;
            cmvc_Photo.m_Lens.FarClipPlane = param;
            cmvc_Event.m_Lens.FarClipPlane = param;
            cmvc_FirstPerson.m_Lens.FarClipPlane = param;
        }
    }

    public void UpdateFOV() {
        float fovTemp = 60f + GameManager.Instance.save.config[GameManager.Save.configID_FieldOfView];
        cmvc_Empty.m_Lens.FieldOfView = fovTemp;
        cmvc_FreeLook.m_Lens.FieldOfView = fovTemp;
        cmvc_TargetGroup.m_Lens.FieldOfView = fovTemp;
        cmvc_Photo.m_Lens.FieldOfView = fovTemp;
        cmvc_Event.m_Lens.FieldOfView = fovTemp;
        cmvc_FirstPerson.m_Lens.FieldOfView = fovTemp;
    }

    void SetFirstPerson() {
        bool fpActive = GameManager.Instance.save.config[GameManager.Save.configID_FirstPerson] != 0;
        if (cmvc_FirstPerson.gameObject.activeSelf != fpActive) {
            cmvc_FirstPerson.gameObject.SetActive(fpActive);
        }
        if (fpActive && CharacterManager.Instance.pCon) {
            cmvc_FirstPerson.transform.SetPositionAndRotation(CharacterManager.Instance.pCon.GetFirstPersonPosition(), CharacterManager.Instance.pCon.GetFirstPersonRotation());
        }
    }

}
