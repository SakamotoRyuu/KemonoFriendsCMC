using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingTrigger_Another : MonoBehaviour {

    public Transform cameraPivotToCeiling;
    public Transform playerPosPivot;
    public Transform cameraPivotToPlayer;
    public RuntimeAnimatorController playerRuntimeAnimatorController;
    public GameObject jumpEffectPrefab;
    public Vector3 cervalBallLiftOffset1;
    public Vector3 cervalBallLiftOffset2;
    public GameObject blackCurtainPrefab;

    private int progress;
    private float elapsedTime;
    private int depthSave;
    private int lockonSave;
    private Animator playerAnimator;
    private PlayerController_Another pConAnother;
    private AudioSource cervalBallAudio;
    private float cervalBallStartVolume;

    private void NextProgress() {
        progress++;
        // elapsedTime = 0f;
    }

    private void Update() {
        if (progress >= 1) {
            elapsedTime += Time.deltaTime;
        }
        switch (progress) {
            case 0:
                break;
            case 1:
                if (elapsedTime >= 1f) {
                    CharacterManager.Instance.SetSpecialChat("EVENT_ANOTHERSERVAL_SP_18", -1, -1);
                    NextProgress();
                }
                break;
            case 2:
                if (elapsedTime >= 3f) {
                    CharacterManager.Instance.FriendsClearAll();
                    CharacterManager.Instance.playerTrans.SetPositionAndRotation(playerPosPivot.position, playerPosPivot.rotation);
                    CharacterManager.Instance.pCon.SetAnotherAnimatorController(true, playerRuntimeAnimatorController);
                    CharacterManager.Instance.AddSandstar(100, true);
                    CharacterManager.Instance.pCon.SupermanStart(false);
                    if (CanvasCulling.Instance) {
                        CanvasCulling.Instance.CheckConfig(CanvasCulling.indexAll, 0);
                    }
                    playerAnimator = CharacterManager.Instance.pCon.GetComponent<Animator>();
                    NextProgress();
                }
                break;
            case 3:
                if (elapsedTime >= 5f) {
                    CharacterManager.Instance.pCon.SetFaceSpecial(FriendsBase.FaceName.Attack, 15f);
                    CameraManager.Instance.SetEventCameraFollowTarget(cameraPivotToPlayer, 100);
                    playerAnimator.SetTrigger("Ready");
                    if (pConAnother && pConAnother.cervalBall) {
                        pConAnother.cervalBall.transform.SetParent(pConAnother.transform);
                        pConAnother.cervalBall.transform.localPosition = cervalBallLiftOffset1;
                    }
                    NextProgress();
                }
                break;
            case 4:
                if (elapsedTime >= 6.5f) {
                    playerAnimator.SetTrigger("Jump");
                    if (pConAnother && pConAnother.cervalBall) {
                        pConAnother.cervalBall.transform.SetParent(pConAnother.transform);
                        pConAnother.cervalBall.transform.localPosition = cervalBallLiftOffset2;
                        cervalBallAudio = pConAnother.cervalBall.GetComponentInChildren<AudioSource>();
                        if (cervalBallAudio) {
                            cervalBallStartVolume = cervalBallAudio.volume;
                        }
                    }
                    Instantiate(jumpEffectPrefab, CharacterManager.Instance.playerTrans.position, Quaternion.identity);
                    if (CharacterManager.Instance.pCon.hyperJumpEffect.audioSource) {
                        CharacterManager.Instance.pCon.hyperJumpEffect.audioSource.volume = 0;
                    }
                    CharacterManager.Instance.pCon.SetFixMoveVector(Vector3.up * 15f, 10f);
                    BGM.Instance.StopFade(3f);
                    Ambient.Instance.StopFade(3f);
                    NextProgress();
                }
                break;
            case 5:
                if (CharacterManager.Instance.playerTrans.position.y > cameraPivotToPlayer.position.y) {
                    cameraPivotToPlayer.LookAt(CharacterManager.Instance.playerTrans);
                }
                if (cervalBallAudio) {
                    cervalBallAudio.volume = Mathf.Clamp01((9.5f - elapsedTime) / 3f) * cervalBallStartVolume;
                }
                if (elapsedTime >= 9.5f) {
                    // PauseController.Instance.SetBlackCurtain(1, false);
                    Instantiate(blackCurtainPrefab, PauseController.Instance.offPauseCanvas.transform).transform.SetAsFirstSibling();
                    CharacterManager.Instance.SetSpecialChat("EVENT_ANOTHERSERVAL_SP_19", -1, 6);
                    NextProgress();
                }
                break;
            case 6:
                if (CharacterManager.Instance.playerTrans.position.y > cameraPivotToPlayer.position.y) {
                    cameraPivotToPlayer.LookAt(CharacterManager.Instance.playerTrans);
                }
                if (elapsedTime >= 16f) {
                    RestoreDOF();
                    if (StageManager.Instance) {
                        StageManager.Instance.DestroyTaggedObjects("Effect");
                        StageManager.Instance.CleanObjectPool();
                    }
                    GameManager.Instance.LoadScene("EndingAnother");
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
            if (CharacterManager.Instance.playerIndex != CharacterManager.playerIndexAnotherEscape) {
                CharacterManager.Instance.SetNewPlayer(CharacterManager.playerIndexAnotherEscape);
            }
            GameManager.Instance.save.SetClearStage(GameManager.gameClearedProgress, true);
            GameManager.Instance.SetClearFlag(GameManager.clearFlag_Another);
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
            }
            pConAnother = CharacterManager.Instance.playerObj.GetComponent<PlayerController_Another>();
            CameraManager.Instance.SetEventCameraFollowTarget(cameraPivotToCeiling, 10f, 1.5f);
            DisableDOF();
            progress = 1;
        }
    }
}
