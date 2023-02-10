using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GetDocument : MonoBehaviour {

    public int id;
    public Sprite background;
    public Vector2 backgroundSize = new Vector2(600, 600);
    public int numPages;
    public bool darkFilter = true;
    public bool activateEnemiesOnDestroy;
    public bool activateOnEnemyZero;
    public GameObject instantiateOnDestroy;
    public GameObject balloonPrefab;
    public bool commentAtHomeIsParticular;
    public float balloonDelay = 0.3f;
    public float getDelay = 0.3f;
    public bool anotherPlayerComment;
    public GetItem enableAfterRead;
    public bool checkSecretTrophy;
    public CharacterManager.ActionType actionType = CharacterManager.ActionType.Read;

    protected int state = 0;
    protected int stageId = 0;
    protected GameObject balloonObj;
    protected float duration = 0f;
    protected int pauseWait;
    protected StringBuilder sb = new StringBuilder();
    
    protected const int idBias = 320;
    protected const int homeStageID = 0;
    protected const string textHeader = "DOC_";
    protected const int talkConditionID = 1;
    protected const string talkHeader = "DOC_COMMENT_";
    protected const int faceIndex = 101;
    protected const string targetTag = "ItemGetter";

    protected const int talkConditionIDEX = 31;
    protected const string talkHeaderEX = "DOC_COMMENT_EX_";
    protected const int faceIndexEX = 131;

    protected const string talkHeaderAnother = "DOC_COMMENT_ANOTHER_";


    protected virtual void Start() {
        if (StageManager.Instance) {
            stageId = StageManager.Instance.stageNumber;
            SetMapChip();
        }
        if (balloonPrefab) {
            balloonObj = Instantiate(balloonPrefab, transform.position, Quaternion.identity, transform);
        }
    }

    protected virtual void SetMapChip() {
        Instantiate(MapDatabase.Instance.prefab[MapDatabase.other], transform.position, Quaternion.identity, transform);
    }

    protected virtual bool ActivateCondition_Get() {
        if (duration >= getDelay) {
            if (activateOnEnemyZero && StageManager.Instance && StageManager.Instance.dungeonController) {
                return (stageId == homeStageID || StageManager.Instance.dungeonController.EnemyCount() <= 0);
            } else {
                return true;
            }
        } else {
            return false;
        }
    }

    protected virtual bool ActivateCondition_Balloon() {
        if (duration >= balloonDelay) {
            if (activateOnEnemyZero && StageManager.Instance && StageManager.Instance.dungeonController) {
                return (stageId == homeStageID || StageManager.Instance.dungeonController.EnemyCount() <= 0);
            } else {
                return true;
            }
        } else {
            return false;
        }
    }

    protected virtual void FriendsComment() {
        if (MessageUI.Instance && CharacterManager.Instance) {
            if (CharacterManager.Instance.GetFriendsExist(talkConditionID, true)) {
                sb.Clear().Append(talkHeader).Append(id.ToString("00"));
                if (commentAtHomeIsParticular && stageId == homeStageID) {
                    sb.Append("_HOME");
                }
                CharacterManager.Instance.SetSpecialChat(sb.ToString(), talkConditionID, -1);
            }
            if (CharacterManager.Instance.GetFriendsExist(talkConditionIDEX, true)) {
                sb.Clear().Append(talkHeaderEX).Append(id.ToString("00"));
                if (commentAtHomeIsParticular && stageId == homeStageID) {
                    sb.Append("_HOME");
                }
                CharacterManager.Instance.SetSpecialChat(sb.ToString(), talkConditionIDEX, -1);
            }
            if (anotherPlayerComment && GameManager.Instance.IsPlayerAnother) {
                sb.Clear().Append(talkHeaderAnother).Append(id.ToString("00"));
                if (stageId == homeStageID) {
                    sb.Append("_HOME");
                }
                CharacterManager.Instance.SetSpecialChat(sb.ToString(), -1, -1);
            }
        }
    }

    protected virtual void SaveDocument() {
        if (id < GameManager.documentMax) {
            GameManager.Instance.save.document[id] = 1;
        }
    }

    protected virtual void SetDocument() {
        PauseController.Instance.SetDocument(background, backgroundSize, sb.Clear().Append(textHeader).Append(id.ToString("00")).Append("_").ToString(), numPages, darkFilter);
    }

    protected virtual void PlaySE() {
        UISE.Instance.Play(UISE.SoundName.page);
    }

    protected virtual void Update() {
        if (CharacterManager.Instance && !CharacterManager.Instance.GetPlayerLive()) {
            PlayerExit();
        }
        bool balloonActiveFlag = ActivateCondition_Balloon();
        bool getActiveFlag = ActivateCondition_Get();
        if (balloonObj && balloonObj.activeSelf != balloonActiveFlag) {
            balloonObj.SetActive(balloonActiveFlag);
        }
        if (PauseController.Instance) {
            if (PauseController.Instance.pauseGame) {
                pauseWait = 2;
            } else if (pauseWait > 0) {
                pauseWait--;
            }
            if (pauseWait <= 0 && getActiveFlag) {
                switch (state) {
                    case 1:
                        if (pauseWait <= 0 && PauseController.Instance.pauseEnabled && GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit) && getActiveFlag) {
                            if (stageId != homeStageID) {
                                SaveDocument();
                            }
                            PlaySE();
                            SetDocument();
                            state = 2;
                        }
                        break;
                    case 2:
                        if (stageId != homeStageID) {
                            if (MessageUI.Instance) {
                                MessageUI.Instance.SetMessage(sb.Clear().Append(TextManager.Get("QUOTE_START")).Append(ItemDatabase.Instance.GetItemName(id + idBias)).Append(TextManager.Get("QUOTE_END")).Append(TextManager.Get("MESSAGE_GOTITEM")).ToString());
                            }
                        }
                        TrophyManager.Instance.CheckTrophy(TrophyManager.t_GetAllDocuments);
                        if (checkSecretTrophy) {
                            TrophyManager.Instance.CheckTrophy(TrophyManager.t_SecretDocument);
                        }
                        FriendsComment();
                        if (stageId != homeStageID) {
                            if (activateEnemiesOnDestroy && StageManager.Instance) {
                                StageManager.Instance.SetActiveEnemies(true);
                            }
                            if (instantiateOnDestroy) {
                                if (GameManager.Instance.playerInput.GetButton(RewiredConsts.Action.Special) && GameManager.Instance.playerInput.GetButton(RewiredConsts.Action.Jump)) {
                                    Instantiate(instantiateOnDestroy, transform.position, Quaternion.identity);
                                    UISE.Instance.Play(UISE.SoundName.bridge);
                                } else if (StageManager.Instance && StageManager.Instance.dungeonController) {
                                    Instantiate(instantiateOnDestroy, transform.position, Quaternion.identity, StageManager.Instance.dungeonController.transform);
                                }
                            }
                            Destroy(gameObject);
                        }
                        if (enableAfterRead) {
                            enableAfterRead.enabled = true;
                            Collider enableAfterReadCollider = enableAfterRead.gameObject.GetComponent<Collider>();
                            if (enableAfterReadCollider) {
                                enableAfterReadCollider.enabled = true;
                            }
                        }
                        state = 0;
                        if (CharacterManager.Instance) {
                            CharacterManager.Instance.SetActionType(CharacterManager.ActionType.None, gameObject);
                        }
                        break;
                }
            }
        }
        duration += Time.deltaTime;
    }

    protected virtual void PlayerEnter() {
        if (CharacterManager.Instance && ActivateCondition_Get()) {
            CharacterManager.Instance.SetActionType(actionType, gameObject);
        }
        state = state == 0 ? 1 : state;
    }

    protected virtual void PlayerExit() {
        if (CharacterManager.Instance) {
            CharacterManager.Instance.SetActionType(CharacterManager.ActionType.None, gameObject);
        }
        state = state == 1 ? 0 : state;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(targetTag)) {
            PlayerEnter();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag(targetTag)) {
            PlayerExit();
        }
    }
}
