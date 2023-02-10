using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameoverTimer : SingletonMonoBehaviour<GameoverTimer> {

    public float startTime = 600;
    public float[] compensationTime;
    public float collapseTime = 30;
    public GameObject effectPrefab;
    public GameObject gameoverPrefab;
    public GameObject timerTextSetPrefab;

    private float remainTime;
    private int state;
    private float elapsedTime;
    private GameObject timerTextSetInstance;
    private TMPTextArrayHolder tmpHolder;
    private int floorSave;
    private bool isStopped;
    private bool collapseEnabled;
    private float difficultyRate;

    void Start() {
        remainTime = startTime;
        state = 0;
        timerTextSetInstance = Instantiate(timerTextSetPrefab, PauseController.Instance.offPauseCanvas.transform);
        tmpHolder = timerTextSetInstance.GetComponent<TMPTextArrayHolder>();
        if (tmpHolder) {
            tmpHolder.tmpTexts[0].text = GetTimeText();
        }
        floorSave = StageManager.Instance.floorNumber;
        difficultyRate = (GameManager.Instance.save.difficulty <= 1 ? 2f : 1f);
        if (PauseController.Instance && GameManager.Instance.save.progress < GameManager.gameClearedProgress) {
            PauseController.Instance.returnLibraryDisabled = true;
        }
    }

    string GetTimeText() {
        int remainInteger = (int)remainTime;
        int remainDecimal = (int)(remainTime * 100) % 100;
        return string.Format("{0:00}\'{1:00}\"{2:00}", remainInteger / 60, remainInteger % 60, remainDecimal);
    }

    void Update() {
        if (!isStopped) {
            switch (state) {
                case 0:
                    if (Time.timeScale > 0f) {
                        remainTime -= Time.deltaTime;
                        if (remainTime < 0f) {
                            remainTime = 0f;
                        }
                        if (floorSave != StageManager.Instance.floorNumber && StageManager.Instance.dungeonController && compensationTime.Length > 0) {
                            floorSave = StageManager.Instance.floorNumber;
                            int generatorLevel = Mathf.Clamp(StageManager.Instance.dungeonController.generatorLevel, 0, compensationTime.Length - 1);
                            if (remainTime < compensationTime[generatorLevel] * difficultyRate) {
                                remainTime = compensationTime[generatorLevel] * difficultyRate;
                            }
                            collapseEnabled = (generatorLevel >= 1);
                        }
                        if (collapseEnabled && remainTime < collapseTime * difficultyRate) {
                            if (CameraManager.Instance && CharacterManager.Instance.playerTrans && StageManager.Instance.dungeonController && StageManager.Instance.dungeonController.BreakWalls(false)) {
                                CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.itemBreakWalls);
                                CameraManager.Instance.SetQuake(CharacterManager.Instance.playerTrans.position, 8, 0.2f, 2f);
                            }
                            collapseEnabled = false;
                        }
                        if (tmpHolder) {
                            tmpHolder.tmpTexts[0].text = GetTimeText();
                        }
                        if (remainTime <= 0f) {
                            CharacterManager.Instance.StopFriends();
                            CharacterManager.Instance.ForceStopForEventAll(3f);
                            CharacterManager.Instance.PlayerDied(false);
                            if (effectPrefab) {
                                Instantiate(effectPrefab, transform);
                            }
                            if (StageManager.Instance.dungeonController) {
                                RandomQuake quakeTemp = StageManager.Instance.dungeonController.GetComponent<RandomQuake>();
                                if (quakeTemp) {
                                    quakeTemp.enabled = false;
                                }
                            }
                            BGM.Instance.StopFade(1f);
                            Ambient.Instance.StopFade(1f);
                            GameManager.Instance.ChangeTimeScale(true);
                            elapsedTime = 0;
                            state++;
                        }
                    }
                    break;
                case 1:
                    elapsedTime += Time.unscaledDeltaTime;
                    PauseController.Instance.SetBlackCurtain(Mathf.Clamp01(elapsedTime), true);
                    if (elapsedTime >= 2.5f) {
                        GameManager.Instance.ChangeTimeScale(false);
                        if (timerTextSetInstance) {
                            Destroy(timerTextSetInstance);
                        }
                        if (gameoverPrefab) {
                            Instantiate(gameoverPrefab);
                        }
                        Destroy(gameObject);
                        state++;
                    }
                    break;
                case 2:
                    break;
            }
        }
    }

    public void StopTimer() {
        isStopped = true;
        PauseController.Instance.SetBlackCurtain(0f, true);
        Destroy(gameObject);
    }

    private void OnDestroy() {
        if (timerTextSetInstance) {
            Destroy(timerTextSetInstance);
        }
        if (PauseController.Instance) {
            PauseController.Instance.returnLibraryDisabled = false;
        }
    }

}
