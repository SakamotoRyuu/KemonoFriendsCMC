using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingTrigger : MonoBehaviour
{

    public GameObject imperatrixPrefab;
    public Transform imperatrixPivot;
    public GameObject effectPrefabSmoke;
    public Transform cameraPivotToImperatrix;
    public Transform playerPosPivot;
    public Transform cameraPivotToPlayer;
    public RuntimeAnimatorController playerRuntimeAnimatorController;
    public GameObject jumpEffectPrefab;
    public Transform japaribusTransform;
    public Vector3 japaribusLiftOffset;
    public GameObject collisionObj;
    public GameObject blackCurtainPrefab;

    private int progress;
    private float elapsedTime;
    private GameObject imperatrixInstance;
    private int depthSave;
    private int lockonSave;
    private Animator playerAnimator;

    private void NextProgress() {
        progress++;
        // elapsedTime = 0f;
    }

    void MoveImperatrix() {
        if (imperatrixInstance) {
            Vector3 posTemp = imperatrixInstance.transform.position;
            posTemp += imperatrixInstance.transform.forward * Time.deltaTime * 5.35f;
            if (posTemp.z > transform.position.z) {
                posTemp.z = transform.position.z;
            }
            imperatrixInstance.transform.position = posTemp;
        }
    }

    private void Update() {
        if (progress >= 1) {
            elapsedTime += Time.deltaTime;
        }
        switch (progress) {
            case 0:
                break;
            case 1:
                MoveImperatrix();
                if (elapsedTime >= 1f) {
                    CharacterManager.Instance.SetSpecialChat("EVENT_SERVAL_LAST_06", -1, -1);
                    NextProgress();
                }
                break;
            case 2:
                MoveImperatrix();
                if (elapsedTime >= 3f) {
                    CharacterManager.Instance.FriendsClearAll();
                    CharacterManager.Instance.playerTrans.SetPositionAndRotation(playerPosPivot.position, playerPosPivot.rotation);
                    CharacterManager.Instance.pCon.SetAnotherAnimatorController(true, playerRuntimeAnimatorController);
                    CharacterManager.Instance.AddSandstar(100, true);
                    CharacterManager.Instance.pCon.SupermanStart(false);
                    playerAnimator = CharacterManager.Instance.pCon.GetComponent<Animator>();
                    if (CanvasCulling.Instance) {
                        CanvasCulling.Instance.CheckConfig(CanvasCulling.indexAll, 0);
                    }
                    NextProgress();
                }
                break;
            case 3:
                MoveImperatrix();
                if (elapsedTime >= 5.25f) {
                    CameraManager.Instance.SetEventCameraFollowTarget(cameraPivotToPlayer, 100);
                    playerAnimator.SetTrigger("Ready");
                    NextProgress();
                }
                break;
            case 4:
                MoveImperatrix();
                if (elapsedTime >= 6.5f) {
                    playerAnimator.SetTrigger("Jump");
                    Instantiate(jumpEffectPrefab, CharacterManager.Instance.playerTrans.position, Quaternion.identity);
                    if (CharacterManager.Instance.pCon.hyperJumpEffect.audioSource) {
                        CharacterManager.Instance.pCon.hyperJumpEffect.audioSource.volume = 0;
                    }
                    CharacterManager.Instance.pCon.SetFixMoveVector(Vector3.up * 15f, 10f);
                    BGM.Instance.StopFade(3f);
                    Ambient.Instance.StopFade(3f);
                    if (collisionObj) {
                        collisionObj.SetActive(false);
                    }
                    NextProgress();
                }
                break;
            case 5:
                MoveImperatrix();
                japaribusTransform.position = CharacterManager.Instance.playerTrans.position + japaribusLiftOffset;
                if (CharacterManager.Instance.playerTrans.position.y > cameraPivotToPlayer.position.y) {
                    cameraPivotToPlayer.LookAt(CharacterManager.Instance.playerTrans);
                }
                if (elapsedTime >= 9.5f) {
                    // PauseController.Instance.SetBlackCurtain(1, false);
                    Instantiate(blackCurtainPrefab, PauseController.Instance.offPauseCanvas.transform).transform.SetAsFirstSibling();
                    CharacterManager.Instance.pCon.hyperJumpEffect.enabled = false;
                    CharacterManager.Instance.SetSpecialChat("EVENT_SERVAL_LAST_07", -1, 6);
                    NextProgress();
                }
                break;
            case 6:
                MoveImperatrix();
                japaribusTransform.position = CharacterManager.Instance.playerTrans.position + japaribusLiftOffset;
                if (CharacterManager.Instance.playerTrans.position.y > cameraPivotToPlayer.position.y) {
                    cameraPivotToPlayer.LookAt(CharacterManager.Instance.playerTrans);
                }
                if (elapsedTime >= 16f) {
                    RestoreDOF();
                    MessageUI.Instance.DestroyMessage();
                    if (StageManager.Instance) {
                        StageManager.Instance.DestroyTaggedObjects("Effect");
                        StageManager.Instance.CleanObjectPool();
                    }
                    GameManager.Instance.LoadScene("Ending");
                    GameManager.Instance.ChangeTimeScale(false, false);
                    NextProgress();
                }
                break;
            case 7:
                break;
        }
    }

    void DisableDOF() {
        depthSave = GameManager.Instance.save.config[GameManager.Save.configID_DepthOfField];
        if (depthSave != 0) {
            GameManager.Instance.save.config[GameManager.Save.configID_DepthOfField] = 0;
            CameraManager.Instance.CheckDepthTextureMode();
            InstantiatePostProcessingProfile.Instance.QualitySettingsAdjustments();
        }
        lockonSave = CharacterManager.Instance.autoAim;
        if (lockonSave != 0) {
            CharacterManager.Instance.autoAim = 0;
            CharacterManager.Instance.pCon.searchArea[0].SetUnlockTarget();
        }
        if (CanvasCulling.Instance) {
            CanvasCulling.Instance.CheckConfig(CanvasCulling.indexGauge, 0);
        }
    }

    void RestoreDOF() {
        if (depthSave != 0) {
            GameManager.Instance.save.config[GameManager.Save.configID_DepthOfField] = depthSave;
            CameraManager.Instance.CheckDepthTextureMode();
            InstantiatePostProcessingProfile.Instance.QualitySettingsAdjustments();
        }
        if (lockonSave != 0) {
            CharacterManager.Instance.autoAim = lockonSave;
        }
        if (CanvasCulling.Instance) {
            CanvasCulling.Instance.CheckConfig();
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (progress == 0 && other.CompareTag("ItemGetter")) {
            if (GameoverTimer.Instance) {
                GameoverTimer.Instance.StopTimer();
            }
            GameManager.Instance.isSecondLap = (GameManager.Instance.save.progress >= GameManager.gameClearedProgress);
            if ((GameManager.Instance.GetClearFlag() & GameManager.clearFlag_Another) != 0 && GameManager.Instance.save.GetRescueNow() >= GameManager.rescueMax) {
                GameManager.Instance.afterClearFlag = true;
            }
            GameManager.Instance.save.SetClearStage(GameManager.gameClearedProgress, true);
            GameManager.Instance.SetClearFlag(GameManager.clearFlag_Normal);
            PauseController.Instance.pauseEnabled = false;
            CharacterManager.Instance.StopFriends();
            CharacterManager.Instance.ForceStopForEventAll(100);
            CharacterManager.Instance.SetPlayerUpdateEnabled(false);
            CharacterManager.Instance.mapCamera.gameObject.SetActive(false);
            if (StageManager.Instance && StageManager.Instance.dungeonController) {
                RandomQuake randomQuake = StageManager.Instance.dungeonController.GetComponent<RandomQuake>();
                if (randomQuake) {
                    randomQuake.enabled = false;
                }
                /*
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                for (int i = 0; i < enemies.Length; i++) {
                    EnemyBase enemyBase = enemies[i].GetComponent<EnemyBase>();
                    if (enemyBase) {
                        enemyBase.ForceDeath();
                    }
                }
                */
                if (StageManager.Instance.dungeonController.treeSettings.container) {
                    Destroy(StageManager.Instance.dungeonController.treeSettings.container.gameObject);
                }
            }
            imperatrixInstance = Instantiate(imperatrixPrefab, imperatrixPivot.position, imperatrixPivot.rotation);
            Instantiate(effectPrefabSmoke, imperatrixPivot.position, imperatrixPivot.rotation);
            CameraManager.Instance.SetEventCameraFollowTarget(cameraPivotToImperatrix, 10f, 1.5f);
            DisableDOF();
            progress = 1;
        }
    }

}
