using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchArea : MonoBehaviour {

    class SearchMemory {
        public GameObject obj;
        public Transform trans;
        public float forgetTimeRemain;
        public bool triggerStaying;
        public bool rayReaching;
    }

    [System.Serializable]
    public class ColliderParam {
        public Vector3 center;
        public float radius;
        public float height;
    }
    
    public string searchTag = "PlayerSearchTarget";
    public float forgetTime = 3f;
    public float dontForgetDistance = 5f;
    public float seeThroughDistance = 3f;
    public float linecastInterval = 0.08f;
    public float priorityEffectRate;
    public bool multiCheck;
    public bool changeOnWatchout;
    public CapsuleCollider changeParamTargetCollider;
    public ColliderParam defaultColliderParam;
    public ColliderParam watchoutColliderParam;    
    public TargetUI targetUI;

    List<SearchMemory> target = new List<SearchMemory>();
    GameObject nowTarget;
    Transform nowTargetTrans;
    CapsuleCollider nowTargetCapsuleCollider;
    LayerMask fieldLayerMask;
    LayerMask playerFindableLayerMask;
    Transform trans;
    float unlockTimer;
    float watchoutTimer;
    float linecastIntervalRemain;
    bool watchoutNow;
    Camera mainCamera;
    Transform[] children;
    Vector3[] rayPivot;
    CharacterBase parentCBase;
    float targetUpdatingMargin;
    float targetChangeInterval;
    const float marginMax = 1.6f;
    const float marginMin = 0.6f;
    static readonly Vector3 vecZero = Vector3.zero;
    static readonly Vector3 multiCheckOffset = new Vector3(0f, 2f, 0f);
    static readonly Vector3 vecForward = Vector3.forward;
    static readonly float[] suppressArray = new float[11] { 0f, 1f, 1.333333f, 1.666666f, 2f, 2.333333f, 2.666666f, 3f, 3.333333f, 3.666666f, 4f };

    [System.NonSerialized]
    public bool isLocking;
    [System.NonSerialized]
    public bool isPlayer;

    void Awake() {
        trans = transform;
        SetNowTarget(null);
        isLocking = false;
        unlockTimer = 0f;
        if (multiCheck) {
            fieldLayerMask = LayerMask.GetMask("Field");
        } else {
            fieldLayerMask = LayerMask.GetMask("Field", "SecondField");
        }
        playerFindableLayerMask = LayerMask.GetMask("Field");
        if (!changeParamTargetCollider) {
            changeParamTargetCollider = GetComponent<CapsuleCollider>();
        }
        if (changeParamTargetCollider) {
            watchoutNow = true;
            ChangeColliderParam(false);
        }
    }

    private void Start() {
        parentCBase = GetComponentInParent<CharacterBase>();
        if (isPlayer) {
            targetUpdatingMargin = 0.5f;
        }
    }

    bool GetMainCamera() {
        if (mainCamera == null) {
            if (CameraManager.Instance) {
                CameraManager.Instance.SetMainCamera(ref mainCamera);
            } else {
                mainCamera = Camera.main;
            }
        }
        return mainCamera != null;
    }

    void ChangeColliderParam(bool toWatchout) {
        if (watchoutNow != toWatchout && changeParamTargetCollider && changeOnWatchout) {
            if (toWatchout) {
                changeParamTargetCollider.center = watchoutColliderParam.center;
                changeParamTargetCollider.radius = watchoutColliderParam.radius;
                changeParamTargetCollider.height = watchoutColliderParam.height;
            } else {
                changeParamTargetCollider.center = defaultColliderParam.center;
                changeParamTargetCollider.radius = defaultColliderParam.radius;
                changeParamTargetCollider.height = defaultColliderParam.height;
            }
        }
        watchoutNow = toWatchout;
    }

    void Update() {
        float deltaTimeCache = Time.deltaTime;
        float suppress = suppressArray[Mathf.Clamp(GameManager.Instance.save.config[GameManager.Save.configID_SuppressCameraTurning], 0, suppressArray.Length - 1)];
        unlockTimer -= deltaTimeCache;
        if (isPlayer) {
            if (targetUpdatingMargin > marginMin * suppress) {
                targetUpdatingMargin = Mathf.Clamp(targetUpdatingMargin - deltaTimeCache, marginMin * suppress, marginMax * suppress);
            }
            if (targetChangeInterval > 0f) {
                if (!nowTarget || !nowTarget.activeInHierarchy) {
                    targetChangeInterval = 0f;
                } else {
                    targetChangeInterval -= deltaTimeCache;
                }
            }
        }
        if (isLocking) {
            if (unlockTimer < 0f || !nowTarget){
                isLocking = false;
            } else if (!nowTarget.activeInHierarchy) {
                bool otherTargetFound = false;
                CharacterBase targetCBase = nowTarget.GetComponentInParent<CharacterBase>();
                if (targetCBase && targetCBase.searchTarget.Length > 1) {
                    float minDist = float.MaxValue;
                    int minIndex = -1;
                    for (int i = 0; i < targetCBase.searchTarget.Length; i++) {
                        if (targetCBase.searchTarget[i] != null && targetCBase.searchTarget[i].activeInHierarchy) {
                            float distTemp = (targetCBase.searchTarget[i].transform.position - trans.position).sqrMagnitude;
                            if (priorityEffectRate > 0f) {
                                SearchTargetPriority stp = targetCBase.searchTarget[i].GetComponent<SearchTargetPriority>();
                                if (stp) {
                                    if (stp.onlyVisiblePlayer && !isPlayer) {
                                        continue;
                                    }
                                    distTemp -= stp.priority * stp.priority * (stp.ignoreMultiplier ? 1f : priorityEffectRate) * (stp.priority < 0 ? -1 : 1);
                                }
                            }
                            if (distTemp < minDist) {
                                minDist = distTemp;
                                minIndex = i;
                            }
                        }
                    }
                    if (minIndex >= 0) {
                        SetNowTarget(targetCBase.searchTarget[minIndex]);
                        otherTargetFound = true;
                    }
                }
                if (!otherTargetFound) {
                    isLocking = false;
                }
            }
        }
        if (!isLocking) {
            if (watchoutNow) {
                watchoutTimer -= deltaTimeCache;
                if (watchoutTimer <= 0) {
                    ChangeColliderParam(false);
                }
            }
            int count = target.Count;
            if (count > 0) {
                for (int i = count - 1; i >= 0; i--) {
                    if (!target[i].obj || !target[i].obj.activeInHierarchy) {
                        target.RemoveAt(i);
                    } else {
                        float sqrDist = (trans.position - target[i].trans.position).sqrMagnitude;
                        if ((target[i].rayReaching && sqrDist <= dontForgetDistance * dontForgetDistance) || (!target[i].rayReaching && sqrDist <= seeThroughDistance * seeThroughDistance)) { 
                            target[i].rayReaching = true;
                        } else {
                            linecastIntervalRemain -= deltaTimeCache;
                            if (linecastIntervalRemain < 0f) {
                                target[i].rayReaching = !Physics.Linecast(trans.position, target[i].trans.position, fieldLayerMask, QueryTriggerInteraction.Ignore);
                                if (multiCheck && !target[i].rayReaching) {
                                    target[i].rayReaching = !Physics.Linecast(trans.position + multiCheckOffset, target[i].trans.position, fieldLayerMask, QueryTriggerInteraction.Ignore);
                                }
                                linecastIntervalRemain = linecastInterval;
                            }
                        }
                        if (target[i].triggerStaying && target[i].rayReaching) {
                            target[i].forgetTimeRemain = forgetTime;
                        } else {
                            target[i].forgetTimeRemain -= deltaTimeCache;
                            if (target[i].forgetTimeRemain < 0) {
                                if (nowTarget == target[i].obj) {
                                    nowTarget = null;
                                    nowTargetTrans = null;
                                }
                                if (!target[i].triggerStaying) {
                                    target.RemoveAt(i);
                                }
                            }
                        }
                    }
                }
            }
            count = target.Count;
            if (count > 0) {
                float distSave = float.MaxValue;
                float distNow;
                int index = -1;
                for (int i = 0; i < count; i++) {
                    if (target[i].forgetTimeRemain >= 0) {
                        if (isPlayer && target[i].obj && GameManager.Instance.save.config[GameManager.Save.configID_LockonInFrontOfCamera] != 0 && IsTargetInFrontOfCamera(target[i].obj) == false) {
                            continue;
                        }
                        distNow = (trans.position - target[i].trans.position).sqrMagnitude;
                        if (priorityEffectRate > 0f && target[i].obj) {
                            SearchTargetPriority stp = target[i].obj.GetComponent<SearchTargetPriority>();
                            if (stp) {
                                if (stp.onlyVisiblePlayer && !isPlayer) {
                                    continue;
                                }
                                distNow -= stp.priority * stp.priority * (stp.ignoreMultiplier ? 1f : priorityEffectRate) * (stp.priority < 0 ? -1 : 1);
                            }
                        }
                        if (distNow < distSave) {
                            index = i;
                            distSave = distNow;
                        }
                    }
                }
                if (index >= 0 && nowTarget != target[index].obj) {
                    if (isPlayer && nowTarget && nowTarget.activeInHierarchy && targetUpdatingMargin > 0f) {
                        if (targetChangeInterval <= 0f) {
                            float distOld = (trans.position - nowTarget.transform.position).sqrMagnitude;
                            if (priorityEffectRate > 0f) {
                                SearchTargetPriority stp = nowTarget.GetComponent<SearchTargetPriority>();
                                if (stp) {
                                    distOld -= stp.priority * stp.priority * (stp.ignoreMultiplier ? 1f : priorityEffectRate) * (stp.priority < 0 ? -1 : 1);
                                }
                            }
                            if (distSave < distOld - targetUpdatingMargin) {
                                SetNowTarget(target[index].obj);
                                targetUpdatingMargin = marginMax * suppress;
                            }
                        }
                    } else {
                        SetNowTarget(target[index].obj);
                    }
                }
            } else {
                if (nowTarget) {
                    SetNowTarget(null);
                }
            }
        }
        if (isPlayer && nowTarget && CharacterManager.Instance && CharacterManager.Instance.autoAim == 0 && GameManager.Instance.save.config[GameManager.Save.configID_LockonInFrontOfCamera] != 0 && IsTargetInFrontOfCamera(nowTarget) == false) {
            SetNowTarget(null);
        }
    }
    
    void OnTriggerEnter(Collider other) {    
        if (other.CompareTag(searchTag)) {
            int count = target.Count;
            for (int i = 0; i < count; i++) {
                if (target[i].obj == other.gameObject) {
                    target[i].triggerStaying = true;
                    return;
                }
            }
            target.Add(new SearchMemory() { obj = other.gameObject, trans = other.gameObject.transform, forgetTimeRemain = 0, triggerStaying = true, rayReaching = false });
            // PaperPlane
            if (parentCBase && parentCBase.isEnemy && other.gameObject.GetComponent<Decoy>() != null) {
                parentCBase.Attraction(other.gameObject, CharacterBase.AttractionType.PaperPlane, true, 0.6f);
            }
        }
    }
    
    void OnTriggerExit(Collider other) {
        if (other.CompareTag(searchTag)) {
            int count = target.Count;
            for (int i = 0; i < count; i++) {
                if (target[i].obj == other.gameObject) {
                    target[i].triggerStaying = false;
                    break;
                }
            }
        }
    }

    public void SetNowTarget(GameObject obj) {
        if (obj) {
            nowTarget = obj;
            nowTargetTrans = nowTarget.transform;
            nowTargetCapsuleCollider = nowTarget.GetComponent<CapsuleCollider>();
        } else {
            nowTarget = null;
            nowTargetTrans = null;
            nowTargetCapsuleCollider = null;            
        }
        if (isPlayer) {
            targetChangeInterval = (nowTarget ? suppressArray[Mathf.Clamp(GameManager.Instance.save.config[GameManager.Save.configID_SuppressCameraTurning], 0, suppressArray.Length - 1)] * 0.2f : 0f);
        }
    }
    
    public GameObject GetNowTarget() { 
        return nowTarget;
    }
    public Transform GetNowTargetTransform() {
        return nowTargetTrans;
    }
    public float GetNowTargetRadius() {
        if (nowTargetCapsuleCollider) {
            return nowTargetCapsuleCollider.radius * nowTargetTrans.lossyScale.x;
        } else {
            return 0f;
        }
    }

    public Vector3 GetTargetsAveragePosition() {
        Vector3 answer = vecZero;
        int weightSum = 0;
        int count = target.Count;
        if (count > 0) {
            for (int i = 0; i < count; i++) {
                if (target[i].obj) {
                    int weighting = 1;
                    float sqrDist = (target[i].trans.position - trans.position).sqrMagnitude;
                    if (sqrDist < 36f) {
                        weighting++;
                        if (sqrDist < 16f) {
                            weighting++;
                            if (sqrDist < 4f) {
                                weighting++;
                            }
                        }
                    }
                    answer += target[i].trans.position * weighting;
                    weightSum += weighting;
                }
            }
            if (weightSum > 0) {
                answer /= weightSum;
            }
        }
        return answer;
    }

    public bool SetLockTarget(GameObject lockTarget, float time = 5f) {
        bool answer = false;
        if (lockTarget && lockTarget.activeInHierarchy && lockTarget.CompareTag(searchTag)) {
            SetNowTarget(lockTarget);
            answer = true;
        } else {
            children = lockTarget.GetComponentsInChildren<Transform>();
            for (int i = 0; i < children.Length; i++) {
                if (children[i].CompareTag(searchTag) && children[i].gameObject.activeInHierarchy) {
                    SetNowTarget(children[i].gameObject);
                    answer = true;
                    break;
                }
            }
        }
        if (answer) {
            isLocking = true;
            unlockTimer = time;
        }
        return answer;
    }
    
    public bool IsBesieged(float condDist) { 
        if (nowTarget) {
            int count = target.Count;
            if (count > 1) {
                CharacterBase nowTargetCBase = nowTarget.GetComponentInParent<CharacterBase>();
                if (nowTargetCBase) {
                    float condSqrDist = condDist * condDist;
                    for (int i = 0; i < count; i++) {
                        if (target[i].obj && target[i].obj != nowTarget) {
                            CharacterBase otherTargetCBase = target[i].obj.GetComponentInParent<CharacterBase>();
                            if (otherTargetCBase != nowTargetCBase) {
                                CapsuleCollider otherTargetCapsule = target[i].obj.GetComponent<CapsuleCollider>();
                                if (otherTargetCapsule) {
                                    float radius = otherTargetCapsule.radius;
                                    if (MyMath.SqrMagnitudeIgnoreY(trans.position, target[i].trans.position) - radius * radius <= condSqrDist) {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    public bool SetLockTargetFromCharacter(CharacterBase lockTarget, float time = 5f) {
        if (lockTarget && lockTarget.gameObject.activeInHierarchy) {
            float minDist = float.MaxValue;
            int minIndex = -1;
            for (int i = 0; i < lockTarget.searchTarget.Length; i++) {
                if (lockTarget.searchTarget[i] != null && lockTarget.searchTarget[i].activeInHierarchy) {
                    float distTemp = (lockTarget.searchTarget[i].transform.position - trans.position).sqrMagnitude;
                    if (priorityEffectRate > 0f) {
                        SearchTargetPriority stp = lockTarget.searchTarget[i].GetComponent<SearchTargetPriority>();
                        if (stp) {
                            if (stp.onlyVisiblePlayer && !isPlayer) {
                                continue;
                            }
                            distTemp -= stp.priority * stp.priority * (stp.ignoreMultiplier ? 1f : priorityEffectRate) * (stp.priority < 0 ? -1 : 1);
                        }
                    }
                    if (distTemp < minDist) {
                        minDist = distTemp;
                        minIndex = i;
                    }
                }
            }
            if (minIndex >= 0) {
                SetNowTarget(lockTarget.searchTarget[minIndex].gameObject);
                isLocking = true;
                unlockTimer = time;
                return true;
            }
        }
        return false;
    }

    public void SetUnlockTimer(float time) {
        unlockTimer = time;
        isLocking = (nowTarget && unlockTimer > 0f);
    }

    public void SetUnlockTarget() {
        isLocking = false;
    }

    public int GetTargetNum {
        get {
            return target.Count;
        }
    }
    public bool GetAnyFound {
        get {
            int count = target.Count;
            bool answer = false;
            if (count > 0) {
                for (int i = 0; i < count; i++) {
                    if (target[i].trans) {
                        Vector3 selfPos = trans.position;
                        Vector3 targetPos = target[i].trans.position;
                        if ((selfPos - targetPos).sqrMagnitude <= seeThroughDistance * seeThroughDistance) {
                            answer = true;
                            break;
                        } else if (!Physics.Linecast(selfPos, targetPos, fieldLayerMask, QueryTriggerInteraction.Ignore)) {
                            answer = true;
                            break;
                        }
                    }
                }
            }
            return answer;
        }
    }

    public bool GetPlayerFindable {
        get {
            bool answer = false;
            if (CharacterManager.Instance && CharacterManager.Instance.playerSearchTarget) {
                Vector3 selfPos = trans.position;
                Vector3 targetPos = CharacterManager.Instance.playerSearchTarget.position;
                float sqrDist = (selfPos - targetPos).sqrMagnitude;
                if (sqrDist <= 10f * 10f) {
                    answer = true;
                } else if (sqrDist <= 30f * 30f && !Physics.Linecast(selfPos, targetPos, playerFindableLayerMask, QueryTriggerInteraction.Ignore)) {
                    answer = true;
                }
            }
            return answer;
        }
    }

    /*
    public void ChangeTarget(int dir, float time = 5.0f) {
        if (nowTarget && GetTargetNum() > 1 && (cameraTrans || GetMainCamera())) {
            Vector3 cameraForward = cameraTrans.TransformDirection(Vector3.forward);
            float nowTargetAngle = Quaternion.FromToRotation(cameraForward, nowTargetTrans.position).eulerAngles.y;
            float angleDiffMin = 360f;
            GameObject nextTarget = nowTarget;
            int count = target.Count;
            for (int i = 0; i < count; i++) {
                if (target[i].obj && nowTarget != target[i].obj) {
                    float targetAngle = Quaternion.FromToRotation(cameraForward, target[i].trans.position).eulerAngles.y;
                    if (dir < 0) {
                        if (targetAngle > nowTargetAngle) {
                            targetAngle -= 360f;
                        }
                        if (nowTargetAngle - targetAngle < angleDiffMin) {
                            angleDiffMin = nowTargetAngle - targetAngle;
                            nextTarget = target[i].obj;
                        }
                    } else {
                        if (targetAngle < nowTargetAngle) {
                            targetAngle += 360f;
                        }
                        if (targetAngle - nowTargetAngle < angleDiffMin) {
                            angleDiffMin = targetAngle - nowTargetAngle;
                            nextTarget = target[i].obj;
                        }
                    }
                }
            }
            if (nowTarget != nextTarget) {
                SetLockTarget(nextTarget);
            }
        }
    }
    */
    public void ChangeTarget(float time = 5.0f) {
        if (nowTarget && GetTargetNum > 1) {
            GameObject nextTarget = nowTarget;
            float baseDist = (nowTarget.transform.position - trans.position).sqrMagnitude;
            float minDistBased = float.MaxValue;
            int minIndexBased = -1;
            float minDistUnlimited = float.MaxValue;
            int minIndexUnlimited = -1;
            int count = target.Count;
            for (int i = 0; i < count; i++) {
                if (target[i].obj && nowTarget != target[i].obj) {
                    float sqrDist = (trans.position - target[i].trans.position).sqrMagnitude;
                    if ((target[i].rayReaching && sqrDist <= dontForgetDistance * dontForgetDistance) || (!target[i].rayReaching && sqrDist <= seeThroughDistance * seeThroughDistance)) {
                        target[i].rayReaching = true;
                    } else {
                        target[i].rayReaching = !Physics.Linecast(trans.position, target[i].trans.position, fieldLayerMask, QueryTriggerInteraction.Ignore);
                        if (multiCheck && !target[i].rayReaching) {
                            target[i].rayReaching = !Physics.Linecast(trans.position + multiCheckOffset, target[i].trans.position, fieldLayerMask, QueryTriggerInteraction.Ignore);
                        }
                    }
                    if (target[i].rayReaching) {
                        float distTemp = (target[i].trans.position - trans.position).sqrMagnitude;
                        if (distTemp >= baseDist && distTemp < minDistBased) {
                            minIndexBased = i;
                            minDistBased = distTemp;
                        }
                        if (distTemp < minDistUnlimited) {
                            minIndexUnlimited = i;
                            minDistUnlimited = distTemp;
                        }
                    }
                }
            }
            if (minIndexBased >= 0) {
                nextTarget = target[minIndexBased].obj;
            } else if (minIndexUnlimited >= 0) {
                nextTarget = target[minIndexUnlimited].obj;
            }
            if (nowTarget != nextTarget) {
                SetLockTarget(nextTarget);
            }
        }
    }

    public void ShowTarget(bool visible = true, int colorNum = 0) {
        if (targetUI != null) {
            if (visible && nowTargetTrans && (mainCamera != null || GetMainCamera())) {
                targetUI.SetCursor(RectTransformUtility.WorldToScreenPoint(mainCamera, nowTargetTrans.position), GameManager.Instance.save.config[GameManager.Save.configID_Target]);
                targetUI.SetColor(colorNum);
                targetUI.gameObject.SetActive(true);
            } else {
                targetUI.gameObject.SetActive(false);
            }
        }
    }

    bool IsTargetInFrontOfCamera(GameObject targetObj) {
        if (targetObj && (mainCamera != null || GetMainCamera())) {
            float radius = 0.5f;
            Vector3 targetPos = targetObj.transform.position;
            SearchTargetReference reference = targetObj.GetComponent<SearchTargetReference>();
            if (reference != null && reference.referObj != null) {
                targetPos = reference.referObj.transform.position;
                radius = reference.referRadius;
            } else {
                CapsuleCollider capCol = targetObj.GetComponent<CapsuleCollider>();
                if (capCol) {
                    radius = capCol.radius * targetObj.transform.lossyScale.x;
                }
            }
            Vector3 camForward = mainCamera.transform.TransformDirection(vecForward);
            targetPos += camForward * radius;
            float angleRange = 90f - (GameManager.Instance.save.config[GameManager.Save.configID_LockonInFrontOfCamera] - 1) * 5f;
            return Vector3.Angle(camForward, targetPos - mainCamera.transform.position) < angleRange;
        } else {
            return true;
        }

    }

    public void SetWatchout(float time) {
        ChangeColliderParam(true);
        watchoutTimer = time;
    }

    public float GetWatchoutTimer => watchoutTimer;

    public void SetNotWatchout() {
        ChangeColliderParam(false);
        watchoutTimer = 0f;
    }

    public bool GetThisIsFound() {
        int length = target.Count;
        if (length >= 10) {
            return true;
        } else {
            for (int i = 0; i < length; i++) {
                if (target[i].obj) {
                    CharacterBase targetCBase = target[i].obj.GetComponentInParent<CharacterBase>();
                    if (targetCBase && targetCBase.searchArea.Length > 0 && targetCBase.searchArea[0] && targetCBase.searchArea[0].GetAnyFound) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

}
