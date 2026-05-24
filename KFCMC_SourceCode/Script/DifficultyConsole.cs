using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class DifficultyConsole : MonoBehaviour {

    public GameObject balloonPrefab;
    public Vector3 balloonOffset;
    public Sprite[] difficultySprites;

    StringBuilder sb = new StringBuilder();
    protected int state = 0;
    protected int pauseWait;
    protected ReferGameObjects referObjs;
    protected Image balloonImage;
    protected int nowSpriteIndex = -1;
    protected bool actionTextEnabled;
    const string targetTag = "ItemGetter";

    void CheckBalloon(int spriteIndex) {
        if (spriteIndex != nowSpriteIndex) {
            nowSpriteIndex = spriteIndex;
            if (balloonImage == null && referObjs && referObjs.gameObjects.Length > 0 && referObjs.gameObjects[0]) {
                balloonImage = referObjs.gameObjects[0].GetComponent<Image>();
            }
            if (balloonImage) {
                balloonImage.sprite = difficultySprites[Mathf.Clamp(spriteIndex, 0, difficultySprites.Length - 1)];
            }
        }
    }
    
    void Start () {
        if (balloonPrefab) {
            referObjs = Instantiate(balloonPrefab, transform.position + balloonOffset, transform.rotation, transform).GetComponent<ReferGameObjects>();
            CheckBalloon(GameManager.Instance.save.difficulty - 1);
        }
        Instantiate(MapDatabase.Instance.prefab[MapDatabase.other], transform);
    }

    private void CommonPause(bool flag) {
        if (CharacterManager.Instance) {
            CharacterManager.Instance.TimeStop(flag);
        }
        DifficultyController.Instance.Activate(flag);
    }

    protected virtual void Update() {
        if (state >= 1) {
            if (PauseController.Instance && PauseController.Instance.pauseGame) {
                pauseWait = 2;
            } else if (pauseWait > 0) {
                pauseWait--;
            }
        }
        if (CharacterManager.Instance && DifficultyController.Instance && !PauseController.Instance.pauseGame) {
            if (state == 1) {
                if (pauseWait <= 0 && PauseController.Instance.GetPauseEnabled() && GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit)) {
                    UISE.Instance.Play(UISE.SoundName.submit);
                    DifficultyController.Instance.SetTexts();
                    CommonPause(true);
                    state = 2;
                }
            } else if (state == 2) {
                int answer = DifficultyController.Instance.DifficultyControl(GameManager.Instance.MoveCursor(false));
                if (answer <= -2) {
                    UISE.Instance.Play(UISE.SoundName.cancel);
                    CommonPause(false);
                    state = 1;
                } else if (answer >= 1) {
                    UISE.Instance.Play(UISE.SoundName.use);
                    GameManager.Instance.save.difficulty = answer;
                    CheckBalloon(GameManager.Instance.save.difficulty - 1);
                    if (MessageUI.Instance) {
                        MessageUI.Instance.SetMessage(sb.Clear().Append(TextManager.Get("WORD_DIFFICULTY")).Append(TextManager.Get("QUOTE_START")).Append(TextManager.Get(StringUtils.Format("CONFIG_DIF_NAME_{0}", answer))).Append(TextManager.Get("QUOTE_END")).ToString());
                    }
                    CommonPause(false);
                    state = 1;
                }
            }
            bool actionTemp = (state == 1 && pauseWait <= 0 && PauseController.Instance.pauseEnabled);
            if (actionTextEnabled != actionTemp) {
                CharacterManager.Instance.SetActionType(actionTemp ? CharacterManager.ActionType.Search : CharacterManager.ActionType.None, gameObject);
                actionTextEnabled = actionTemp;
            }
        }
    }

    protected virtual void PlayerEnter() {
        state = state == 0 ? 1 : state;
    }

    protected virtual void PlayerExit() {
        state = state == 1 ? 0 : state;
    }

    private void OnTriggerEnter(Collider other) {
        if (enabled && other.CompareTag(targetTag)) {
            PlayerEnter();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (enabled && other.CompareTag(targetTag)) {
            PlayerExit();
        }
    }
    
}
