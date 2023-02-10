using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Event_Summit : MonoBehaviour
{

    public Transform[] posPivot;
    public Transform[] sisinParent;
    public Transform[] sisinChild;
    public Transform[] sisinTargetPos;
    public MoveLoop_Floating[] floating;
    public GameObject completeEffect;
    public GameObject lavaObjects;
    public GameObject sunnyObjects;
    public Transform filterTrans;
    public Renderer filterRenderer;
    public Renderer summitRenderer;
    public Material summitNewMaterial;
    public int sunnyLightingNumber;
    public int sunnyAmbientNumber;
    public string[] talkName;

    int enemyCount;
    int activateCount;
    int state = 0;
    float elapsedTime;
    float interval;
    Vector3 vecTemp = Vector3.one;
    Vector2 tileTemp = Vector2.zero;
    bool[] talkedFlag = new bool[4];
    float[] characterExistTime = new float[4];

    [System.NonSerialized]
    public bool enemyZeroFlag;


    public void CallEnemy(int posIndex) {
        if (StageManager.Instance.dungeonController) {
            StageManager.Instance.SetActiveEnemies(true, posIndex);
            enemyZeroFlag = false;
            enemyCount++;
            if (enemyCount >= 4) {
                StageManager.Instance.dungeonController.bgmNumber = -1;
            }
        }
    }

    public void ActivateSisin() {
        activateCount++;
        if (activateCount >= 4 && state == 0) {
            state = 1;            
        }
    }

    void CheckTalkSub(int flagIndex, int friendsIndex) {
        if (flagIndex < talkedFlag.Length && !talkedFlag[flagIndex]) {
            if (CharacterManager.Instance.GetFriendsExist(friendsIndex, true)) {
                characterExistTime[flagIndex] += Time.deltaTime;
                if (characterExistTime[flagIndex] >= 1.5f) {
                    talkedFlag[flagIndex] = true;
                    if (flagIndex < talkName.Length) {
                        CharacterManager.Instance.SetSpecialChat(talkName[flagIndex], friendsIndex, -1);
                    }
                }
            } else {
                characterExistTime[flagIndex] = 0f;
            }
        }
    }

    void CheckTalk() {
        if (state == 0 && (enemyCount < 4 || !enemyZeroFlag)) {
            CheckTalkSub(0, 1);
            CheckTalkSub(1, 31);
        } else if (state >= 5) {
            CheckTalkSub(2, 1);
            CheckTalkSub(3, 31);
        }
    }

    private void Update() {
        switch (state) {
            case 0:
                interval += Time.deltaTime;
                if (interval >= 0.1f) {
                    interval = 0f;
                    enemyZeroFlag = (StageManager.Instance.dungeonController && StageManager.Instance.dungeonController.EnemyCount(true) <= 0);
                }
                break;
            case 1:
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= 2f) {
                    state = 2;
                    elapsedTime = 0f;
                    for (int i = 0; i < sisinChild.Length; i++) {
                        sisinChild[i].DOLocalRotate(Vector3.zero, 1f);
                        sisinParent[i].DOMove(sisinTargetPos[i].position, 2f);
                        if (floating[i]) {
                            floating[i].enabled = true;
                        }
                    }
                }
                break;
            case 2:
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= 3f) {
                    state = 3;
                    elapsedTime = 0f;
                    completeEffect.SetActive(true);
                }
                break;
            case 3:
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= 10f) {
                    ClearUp();
                }
                break;
            case 4:
                elapsedTime += Time.deltaTime;
                if (elapsedTime < 3f) {
                    vecTemp.z = vecTemp.x = 0.4f + elapsedTime * 0.416666f;
                } else {
                    vecTemp.z = vecTemp.x = 1.65f;
                    state = 5;
                    elapsedTime = 0f;
                    if (MessageUI.Instance) {
                        MessageUI.Instance.SetMessage(TextManager.Get("MESSAGE_FILTER"), MessageUI.time_Important, MessageUI.panelType_Information, MessageUI.slotType_Important);
                    }
                }
                vecTemp.y = 1f;
                tileTemp.x = tileTemp.y = vecTemp.x * 3f;
                filterTrans.localScale = vecTemp;
                filterRenderer.material.mainTextureScale = tileTemp;
                break;
            case 5:
                break;
        }
        CheckTalk();
    }

    public void ClearUp() {
        if (state < 4) {
            state = 4;
            elapsedTime = 0f;
            lavaObjects.SetActive(false);
            if (LightingDatabase.Instance) {
                LightingDatabase.Instance.SetLighting(sunnyLightingNumber);
                CharacterManager.Instance.SetPlayerLightActive();
            }
            if (Ambient.Instance) {
                Ambient.Instance.Stop();
                Ambient.Instance.Play(sunnyAmbientNumber, 3f);
            }
            if (CameraManager.Instance) {
                CameraManager.Instance.SetQuake(transform.position, 16, 4, 0f, 0.5f, 2.5f, 50f, 200f);
            }
            Material[] matsTemp = summitRenderer.materials;
            if (matsTemp.Length > 1) {
                matsTemp[1] = summitNewMaterial;
            }
            summitRenderer.materials = matsTemp;
            sunnyObjects.SetActive(true);
        }
    }

}
