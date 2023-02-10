using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RideBase_Driving : RideBase
{
    [System.Serializable]
    public class AxleInfo {
        public WheelCollider wheel;
        public Transform model;
        public bool motor;
        public bool steering;
        public bool rotInverse;
        public Vector3 rotEuler;
    }

    public AxleInfo[] axleInfos;
    public float maxMotorTorque;
    public float maxBrakeTorque;
    public float maxSteeringAngle;
    public GameObject staticPrefab;
    public Transform centerOfMass;
    public bool fixFaceEnabled;
    public FriendsBase.FaceName fixFaceName;
    public RideBase rideTogetherBase;
    public Rigidbody parentRigidbody;
    public CarAttackDetection attackDetection;
    public GameObject wallBreaker;
    public ActivateExceptHome activateExceptHome;
    
    protected float nowMotor;
    protected float nowSteering;
    protected float nowBrake;
    protected float motorVelocity;
    protected float steeringVelocity;
    protected int pauseWait;
    protected float startInterval = 0.1f;
    protected static readonly Vector3 vecForward = Vector3.forward;
    protected static readonly Vector3 vecDown = Vector3.down;
    protected static readonly Vector3 vecZero = Vector3.zero;
    protected static readonly Vector3 vecLeftTorque = new Vector3(0f, 0f, 3f);
    protected static readonly Vector3 vecRightTorque = new Vector3(0f, 0f, -3f);
    protected static readonly Vector3 vecForwardTorque = new Vector3(3f, 0f, 0f);
    protected static readonly Vector3 vecBackTorque = new Vector3(-3f, 0f, 0f);
    const int facilityID = 24;

    private void Awake() {
        for (int i = 0; i < axleInfos.Length; i++) {
            axleInfos[i].rotEuler = axleInfos[i].model.localEulerAngles;
        }
        if (centerOfMass && parentRigidbody) {
            parentRigidbody.centerOfMass = centerOfMass.localPosition;
        }
    }

    public void SetFriendsRideTogether(int[] friendsID) {
        if (CharacterManager.Instance && rideTogetherBase && friendsID.Length > 0) {
            for (int i = 0; i < friendsID.Length && i < rideTogetherBase.ridePoints.Length; i++) {
                int idTemp = friendsID[i];
                if (CharacterManager.Instance.GetFriendsExist(idTemp, false) && rideTogetherBase.ridePoints[i].point && !CharacterManager.Instance.friends[idTemp].fBase.IsRiding) {
                    rideTogetherBase.ridePoints[i].point.position = CharacterManager.Instance.friends[idTemp].fBase.transform.position;
                    rideTogetherBase.WarpFriendSpecificRidePoint(CharacterManager.Instance.friends[idTemp].fBase, rideTogetherBase.ridePoints[i].point);
                }
            }
        }
    }

    private void Update() {
        if (PauseController.Instance) {
            if (PauseController.Instance.pauseGame) {
                pauseWait = 2;
            } else if (pauseWait > 0) {
                pauseWait--;
            }
        }
        if (Time.timeScale > 0f && PauseController.Instance && !PauseController.Instance.pauseGame && PauseController.Instance.pauseEnabled) {
            if (startInterval > 0f) {
                startInterval -= Time.deltaTime;
            }
            if (pauseWait <= 0 && startInterval <= 0f && GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit)) {
                ReleaseAllFriends();
            } else {
                float deltaTimeCache = Time.deltaTime;
                for (int i = 0; i < axleInfos.Length; i++) {
                    axleInfos[i].rotEuler.x += axleInfos[i].wheel.rpm * 6f * (axleInfos[i].rotInverse ? -1f : 1f) * deltaTimeCache;
                    if (axleInfos[i].rotEuler.x > 360f) {
                        axleInfos[i].rotEuler.x -= 360f;
                    } else if (axleInfos[i].rotEuler.x < -360f) {
                        axleInfos[i].rotEuler.x += 360f;
                    }
                    if (axleInfos[i].steering) {
                        axleInfos[i].rotEuler.y = axleInfos[i].wheel.steerAngle;
                    }
                    axleInfos[i].model.localEulerAngles = axleInfos[i].rotEuler;
                }
            }
        }
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
        if (anyoneRiding) {
            if (GameManager.Instance && Time.timeScale > 0f) {
                for (int i = 0; i < ridePoints.Length; i++) {
                    if (ridePoints[i].usingFriends) {
                        if (fixFaceEnabled) {
                            ridePoints[i].usingFriends.SetFaceSpecial(fixFaceName, 1f);
                        }
                    }
                }
            }
            if (Time.timeScale > 0f && PauseController.Instance && PauseController.Instance.pauseEnabled && !PauseController.Instance.pauseGame) {
                float axisV = GameManager.Instance.playerInput.GetAxis(RewiredConsts.Action.Vertical);
                float axisH = GameManager.Instance.playerInput.GetAxis(RewiredConsts.Action.Horizontal);
                float deltaTimeCache = Time.fixedDeltaTime;

                if (parentRigidbody && GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Dodge)) {
                    if (axisH < -0.7f) {
                        parentRigidbody.AddRelativeTorque(vecLeftTorque, ForceMode.VelocityChange);
                    } else if (axisH > 0.7f) {
                        parentRigidbody.AddRelativeTorque(vecRightTorque, ForceMode.VelocityChange);
                    } else if (axisV > 0.7f) {
                        parentRigidbody.AddRelativeTorque(vecForwardTorque * (nowMotor < 0f ? 1f + Mathf.Clamp(nowMotor * -0.01f, 0f, 0.5f) : 1f), ForceMode.VelocityChange);
                    } else {
                        parentRigidbody.AddRelativeTorque(vecBackTorque * (nowMotor > 0f ? 1f + Mathf.Clamp(nowMotor * 0.01f, 0f, 0.5f) : 1f), ForceMode.VelocityChange);
                    }
                }

                float maxMotorTemp = maxMotorTorque * (CharacterManager.Instance.pCon.isSuperman ? 2f : 1f) * (CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Speed) ? 2f : 1f);
                nowMotor = Mathf.SmoothDamp(nowMotor, maxMotorTemp * axisV, ref motorVelocity, 0.5f);
                if ((nowMotor > 0f && axisV <= 0f) || (nowMotor < 0f && axisV >= 0f) || nowMotor == 0f) {
                    nowBrake = maxBrakeTorque;
                } else { 
                    nowBrake = 0f;
                }
                if (axisH > 0.15f) {
                    axisH = Mathf.Clamp((axisH - 0.15f) * (1.25f + (Mathf.Abs(nowMotor) / maxMotorTemp) * 0.75f), -1f, 1f);
                } else if (axisH < -0.15f) {
                    axisH = Mathf.Clamp((axisH + 0.15f) * (1.25f + (Mathf.Abs(nowMotor) / maxMotorTemp) * 0.75f), -1f, 1f);
                } else {
                    axisH = 0f;
                }
                nowSteering = Mathf.SmoothDamp(nowSteering, maxSteeringAngle * axisH, ref steeringVelocity, 0.1f);
                foreach (AxleInfo axleInfo in axleInfos) {
                    if (axleInfo.steering) {
                        axleInfo.wheel.steerAngle = nowSteering;
                    }
                    if (axleInfo.motor) {
                        axleInfo.wheel.motorTorque = nowMotor;
                        axleInfo.wheel.brakeTorque = nowBrake;
                    }
                }
                if (parentRigidbody) {
                    float sqrMagnitude = parentRigidbody.velocity.sqrMagnitude;
                    if (attackDetection) {
                        attackDetection.attackPower = (int)(sqrMagnitude * 10);
                        attackDetection.knockAmount = sqrMagnitude * 10;
                        if (sqrMagnitude > 0.01f) {
                            attackDetection.transform.position = attackDetection.transform.parent.position + parentRigidbody.velocity * 0.02f;
                        } else {
                            attackDetection.transform.localPosition = vecZero;
                        }
                    }
                    bool wallBreakEnabled = (sqrMagnitude > 10f * 10f || nowMotor >= 100);
                    if (wallBreaker && wallBreaker.activeSelf != wallBreakEnabled) {
                        wallBreaker.SetActive(wallBreakEnabled);                        
                    }
                }

            }
        } else if (staticPrefab) {
            GameObject staticObj = Instantiate(staticPrefab, transform.position, transform.rotation, transform.parent);
            ActivateExceptHome[] toActs = staticObj.GetComponents<ActivateExceptHome>();
            if (activateExceptHome && toActs != null && toActs.Length > 0) {
                for (int i = 0; i < toActs.Length; i++) {
                    toActs[i].ActivateExternal(activateExceptHome.isExceptHome);
                }
            }
            if (PauseController.Instance) {
                PauseController.Instance.SetFacilityObj(facilityID, staticObj);
            }
            Destroy(gameObject);
        }
    }
}
