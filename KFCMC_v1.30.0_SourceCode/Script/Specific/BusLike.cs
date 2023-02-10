using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BusLike : MonoBehaviour {

    public Cloth[] cloth;
    public GameObject[] characterObj;
    public GameObject[] characterRenderer;
    public int[] characterID;

    NavMeshAgent agent;
    Animator anim;
    float animSpeedSave = 1f;
    float animRotationSave = 0f;
    int animSpeedHash;
    int animRotationHash;
    GameObject[] respawnPoints;
    int respawnIndex;
    Vector3 posSave;
    Transform trans;
    float blockedTime;
    bool clothSave;
    float lastRotateY;
    const float speedRate = 0.35f;

    private void Awake() {
        trans = transform;
    }

    void Start() {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        animSpeedHash = Animator.StringToHash("Speed");
        animRotationHash = Animator.StringToHash("Rotation");
        animSpeedSave = 1f;
        anim.SetFloat(animSpeedHash, animSpeedSave);
        animRotationSave = 0f;
        anim.SetFloat(animRotationHash, animRotationSave);
        respawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
        respawnIndex = Random.Range(0, respawnPoints.Length);
        posSave = trans.position;
        clothSave = (GameManager.Instance.save.config[GameManager.Save.configID_ClothSimulation] != 0);
        SetCloth(clothSave);
        lastRotateY = trans.localEulerAngles.y;
        CheckCharacterActive(true);
        for (int i = 0; i < characterObj.Length; i++) {
            Instantiate(MapDatabase.Instance.prefab[MapDatabase.itemCharacter], characterObj[i].transform);
        }
    }

    void GotoNextPoint() {
        respawnIndex = (Random.Range(0, respawnPoints.Length - 1) + 1) % respawnPoints.Length;
        if (agent.pathStatus != NavMeshPathStatus.PathInvalid && respawnPoints[respawnIndex]) {
            agent.SetDestination(respawnPoints[respawnIndex].transform.position);
        }
    }

    void SetCloth(bool flag) {
        for (int i = 0; i < cloth.Length; i++) {
            if (cloth[i] && cloth[i].enabled != flag) {
                cloth[i].enabled = flag;
            }
        }
    }

    void CheckCharacterActive(bool forceSet = false) {
        bool allCharacterActiveTemp = true;
        for (int i = 0; i < characterObj.Length && i < characterID.Length; i++) {
            bool notExist = !CharacterManager.Instance.GetFriendsExist(characterID[i], false);
            if (allCharacterActiveTemp && !notExist) {
                allCharacterActiveTemp = false;
            }
            if (characterObj[i] && characterObj[i].activeSelf != notExist) {
                characterObj[i].SetActive(notExist);
            }
        }
        if (forceSet || agent.isStopped == allCharacterActiveTemp) {
            agent.isStopped = !allCharacterActiveTemp;
        }
        if (CameraManager.Instance && CameraManager.Instance.mainCamObj) {
            bool rendererFlag = (CameraManager.Instance.mainCamObj.transform.position - transform.position).sqrMagnitude <= (GameManager.Instance.save.config[GameManager.Save.configID_QualityLevel] <= 0 ? 30f * 30f : 50f * 50f);
            for (int i = 0; i < characterRenderer.Length; i++) {
                if (characterRenderer[i] && characterRenderer[i].activeSelf != rendererFlag) {
                    characterRenderer[i].SetActive(rendererFlag);
                }
            }
        }
    }

    void Update() {
        float deltaTimeCache = Time.deltaTime;
        CheckCharacterActive(false);
        if (deltaTimeCache > 0f) {
            float speedTemp = Mathf.Clamp01(agent.velocity.magnitude * speedRate);
            if (animSpeedSave != speedTemp) {
                animSpeedSave = speedTemp;
                anim.SetFloat(animSpeedHash, animSpeedSave);
            }
            float eulerAnglesYTemp = trans.localEulerAngles.y;
            if (lastRotateY > eulerAnglesYTemp + 180f) {
                lastRotateY -= 360f;
            } else if (lastRotateY < eulerAnglesYTemp - 180f) {
                lastRotateY += 360f;
            }
            float nowRot = Mathf.Clamp((eulerAnglesYTemp - lastRotateY) / (deltaTimeCache * 30f), -1.1f, 1.1f);
            lastRotateY = eulerAnglesYTemp;
            float rotationTemp = Mathf.Clamp(Mathf.Lerp(animRotationSave, nowRot, deltaTimeCache * 10f), -1f, 1f);
            if (rotationTemp > -0.001f && rotationTemp < 0.001f) {
                rotationTemp = 0f;
            }
            if (animRotationSave != rotationTemp) {
                animRotationSave = rotationTemp;
                anim.SetFloat(animRotationHash, animRotationSave);
            }
            if ((trans.position - posSave).sqrMagnitude / deltaTimeCache < 0.5f) {
                blockedTime += deltaTimeCache;
                if (blockedTime >= 5f) {
                    GotoNextPoint();
                    blockedTime = 0f;
                }
            } else {
                blockedTime = 0f;
            }
            posSave = trans.position;
            if (clothSave != (GameManager.Instance.save.config[GameManager.Save.configID_ClothSimulation] != 0)) {
                clothSave = (GameManager.Instance.save.config[GameManager.Save.configID_ClothSimulation] != 0);
                SetCloth(clothSave);
            }
        }
    }
}
