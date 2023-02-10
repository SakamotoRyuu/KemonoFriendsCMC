using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_LastBattleAnother : SingletonMonoBehaviour<Event_LastBattleAnother>
{

    public Transform playerPivot;
    public Transform cervalBallPivot;
    public GameObject cervalBallPrefab;
    public RuntimeAnimatorController eventAnimator;
    public Transform cameraPivot1;
    public Transform cameraPivot2;
    public Transform cameraPivot3;
    public Transform cameraPivot4;
    public int innerCoreLightingNumber;
    public int innerCoreAmbientNumber;
    public GameObject innerCorePrefab;
    public GameObject sphericLavaPrefab;
    public GameObject gameoverTimer;

    int progress;
    float elapsedTime;
    GameObject cervalBallInstance;
    RandomQuake randomQuake;

    void NextProgress() {
        progress++;
        elapsedTime = 0f;
    }

    private void Update() {
        if (GameManager.Instance.IsPlayerAnother) {
            elapsedTime += Time.deltaTime;
            switch (progress) {
                case 0:
                    if (elapsedTime > 1f) {
                        CharacterManager.Instance.SetSpecialChat("EVENT_ANOTHERSERVAL_SP_14", -1, -1);
                        CharacterManager.Instance.pCon.SetFaceString("Attack", 6);
                        NextProgress();
                    }
                    break;
                case 1:
                    if (CharacterManager.Instance.isBossBattle) {
                        CharacterManager.Instance.SetSpecialChat("EVENT_ANOTHERSERVAL_SP_15", -1, -1);
                        CharacterManager.Instance.pCon.SetFaceString("Attack2", 6);
                        NextProgress();
                    }
                    break;
                case 2:
                    break;
                case 10:
                    if (elapsedTime >= 1f) {
                        PauseController.Instance.SetBlackCurtain(0f, false);
                        NextProgress();
                    }
                    break;
                case 11:
                    if (elapsedTime >= 3f) {
                        CameraManager.Instance.SetEventCamera(cameraPivot2.position, cameraPivot2.eulerAngles, 20f, 0f, Vector3.Distance(cameraPivot2.position, CharacterManager.Instance.playerAudioListener.position));
                        CharacterManager.Instance.SetSpecialChat("EVENT_ANOTHERSERVAL_SP_16", -1, 6);
                        NextProgress();
                    }
                    break;
                case 12:
                    if (elapsedTime >= 6f) {
                        Ambient.Instance.Play(innerCoreAmbientNumber, 1f);
                        if (sphericLavaPrefab) {
                            Instantiate(sphericLavaPrefab, transform);
                        }
                        randomQuake = StageManager.Instance.dungeonController.GetComponent<RandomQuake>();
                        if (randomQuake) {
                            randomQuake.enabled = false;
                            randomQuake.SetQuake(1f, 0.5f, 0f);
                        }
                        CameraManager.Instance.SetEventCameraTweening(cameraPivot3.position, cameraPivot3.eulerAngles, 20f, 0.75f, 50f);
                        NextProgress();
                    }
                    break;
                case 13:
                    if (elapsedTime >= 2f) {
                        PauseController.Instance.SetBlackCurtain(1f, false);
                        CharacterManager.Instance.pCon.SetAnotherAnimatorController(false, null);
                        CharacterManager.Instance.SetNewPlayer(CharacterManager.playerIndexAnotherEscape);
                        CharacterManager.Instance.ForceStopForEventAll(10f);
                        CameraManager.Instance.SetEventCamera(cameraPivot4.position, cameraPivot4.eulerAngles, 10f, 1.5f, Vector3.Distance(cameraPivot4.position, CharacterManager.Instance.playerAudioListener.position));
                        if (cervalBallInstance) {
                            Destroy(cervalBallInstance);
                        }
                        NextProgress();
                    }
                    break;
                case 14:
                    if (elapsedTime >= 0.5f) {
                        CharacterManager.Instance.pCon.SetFaceString("Determine", 7);
                        NextProgress();
                    }
                    break;
                case 15:
                    if (elapsedTime >= 0.5f) {
                        if (CanvasCulling.Instance) {
                            CanvasCulling.Instance.CheckConfig(CanvasCulling.indexGauge, 0);
                        }
                        if (randomQuake) {
                            randomQuake.enabled = false;
                            CameraManager.Instance.CancelQuake();
                        }
                        PauseController.Instance.SetBlackCurtain(0f, false);
                        NextProgress();
                    }
                    break;
                case 16:
                    if (elapsedTime >= 1f) {
                        CharacterManager.Instance.SetSpecialChat("EVENT_ANOTHERSERVAL_SP_17", -1, 5);
                        NextProgress();
                    }
                    break;
                case 17:
                    if (elapsedTime >= 4f) {
                        PauseController.Instance.pauseEnabled = true;
                        CharacterManager.Instance.ReleaseStopForEventAll();
                        CameraManager.Instance.SetEventTimer(0.002f);
                        GameObject goalInstance = StageManager.Instance.dungeonController.GetGoalInstance();
                        if (randomQuake) {
                            randomQuake.enabled = true;
                        }
                        if (goalInstance) {
                            goalInstance.SetActive(true);
                        }
                        if (gameoverTimer) {
                            Instantiate(gameoverTimer, StageManager.Instance.dungeonMother.transform);
                        }
                        if (CanvasCulling.Instance) {
                            CanvasCulling.Instance.CheckConfig();
                        }
                        NextProgress();
                    }
                    break;
                case 18:
                    break;
            }
        }
    }

    private void LateUpdate() {
        if (CanvasCulling.Instance) {
            if (progress >= 10 && progress <= 15) {
                CanvasCulling.Instance.CheckConfig(CanvasCulling.indexAll, 0);
            } else if (progress >= 16 && progress <= 17) {
                CanvasCulling.Instance.CheckConfig(CanvasCulling.indexGauge, 0);
            }
        }
    }

    public void BattleEnd() {
        if (GameManager.Instance.IsPlayerAnother) {
            PauseController.Instance.pauseEnabled = false;
            PauseController.Instance.SetBlackCurtain(1f, false);
            StageManager.Instance.DestroyTaggedObjects("Effect");
            StageManager.Instance.DestroyTaggedObjects("Projectile");
            StageManager.Instance.CleanObjectPool();
            CharacterManager.Instance.ClearBuff();
            CharacterManager.Instance.ClearSickAll();
            CharacterManager.Instance.ResetST();
            CharacterManager.Instance.ForceStopForEventAll(30f);
            CharacterManager.Instance.playerTrans.SetPositionAndRotation(playerPivot.position, playerPivot.rotation);
            cervalBallInstance = Instantiate(cervalBallPrefab, cervalBallPivot);
            CharacterManager.Instance.pCon.SetAnotherAnimatorController(true, eventAnimator);
            CharacterManager.Instance.pCon.SetFaceString("Sad2", 30);
            CameraManager.Instance.CancelQuake();
            CameraManager.Instance.SetEventCamera(cameraPivot1.position, cameraPivot1.eulerAngles, 20f, 0f, Vector3.Distance(cameraPivot1.position, CharacterManager.Instance.playerAudioListener.position));
            LightingDatabase.Instance.SetLighting(innerCoreLightingNumber);
            if (innerCorePrefab) {
                Instantiate(innerCorePrefab, transform);
            }
            if (StageManager.Instance.dungeonMother) {
                StageManager.Instance.dungeonMother.reaperSettings.enemyID = -1;
            }
            if (CanvasCulling.Instance) {
                CanvasCulling.Instance.CheckConfig(CanvasCulling.indexAll, 0);
            }
            progress = 10;
            elapsedTime = 0f;
        }
    }

}
