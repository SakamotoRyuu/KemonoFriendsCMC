using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCharacter_SLB : ItemCharacter {

    public GameObject extraObj;
    public GameManager.SecretType secretType;

    int eventState;
    float eventTimer;
    bool anotherServalTalkedFlag;
    bool restartedFlag;
    bool actionedFlag;
    bool extraObjActivated;
    float nextEventCondition = 5f;

    protected const int kabanID = 1;
    protected const string kabanOption = "_KABAN";
    protected const string noeventOption = "_NOEVENT";
    protected const int anotherServalID = 31;
    protected const int summitSceneNumber = 23;

    protected override bool GetIsHome() {
        if (stageNumber == 12 && StageManager.Instance.floorNumber == 14) {
            return true;
        } else {
            return base.GetIsHome();
        }
    }

    protected override bool CheckCondition() {
        return GameManager.Instance.GetSecret(secretType);
    }

    protected override void KeyItemPlus() {
        base.KeyItemPlus();
        StageManager.Instance.dungeonController.keyItemIsLB = true;
    }

    protected override void Awake() {
        base.Awake();
        started = true;
    }    

    protected override void Start() {
        base.Start();
        if (!isHome && !CharacterManager.Instance.inertIFR) {
            GameObject[] eventObj = GameObject.FindGameObjectsWithTag("GameController");
            if (eventObj != null && eventObj.Length > 0) {
                for (int i = 0; i < eventObj.Length; i++) {
                    Event_BigDog eventTemp = eventObj[i].GetComponent<Event_BigDog>();
                    if (eventTemp != null) {
                        restartedFlag = true;
                        eventState = 4;
                        antiInertIFR = true;
                        smile = eventTemp.slbActedFlag;
                        if (eventTemp.allFriendsActivateFlag) {
                            CharacterManager.Instance.inertIFR = true;
                            if (extraObj) {
                                extraObj.SetActive(true);
                                extraObjActivated = true;
                            }
                            break;
                        }
                    }
                }
            }
            if (!restartedFlag) {
                GameObject[] dungeonObj = GameObject.FindGameObjectsWithTag("Dungeon");
                if (dungeonObj != null && dungeonObj.Length > 0) {
                    for (int i = 0; i < dungeonObj.Length; i++) {
                        HoldPrefab holdPrefabTemp = dungeonObj[i].GetComponent<HoldPrefab>();
                        if (holdPrefabTemp != null) {
                            restartedFlag = true;
                            eventState = 4;
                            antiInertIFR = true;
                            break;
                        }
                    }
                }
            }
        }
    }

    protected override void Update() {
        base.Update();
        destroyTimeSpeech = -1000f;
        destroyTimeConstant = -1000f;
        if (!InertIFR) {
            if (eventState >= 1 && !restartedFlag) {
                eventTimer += deltaTimeCache;
                if (eventState == 1) {
                    if (CharacterManager.Instance.GetFriendsExist(kabanID, true)) {
                        string speechContent = TextManager.Get(GetTalkMargedName());
                        nextEventCondition = MessageUI.Instance.GetSpeechAppropriateTime(speechContent.Length);
                        MessageUI.Instance.SetMessageOptional(speechContent, mesBackColor.color1, mesBackColor.color2, mesBackColor.twoToneType, faceIndex, nextEventCondition, 1, 1, true);
                        MessageUI.Instance.SetMessageLog(speechContent, faceIndex);
                        eventTimer = 0f;
                        eventState = 2;
                    }
                } else if (eventState == 2) {
                    if (eventTimer > nextEventCondition && CharacterManager.Instance.GetFriendsExist(kabanID, true)) {
                        MessageBackColor friendsMBC = CharacterManager.Instance.GetMessageBackColor(kabanID);
                        if (friendsMBC != null) {
                            string speechContent = TextManager.Get("EVENT_KABAN_12_2");
                            nextEventCondition = MessageUI.Instance.GetSpeechAppropriateTime(speechContent.Length);
                            MessageUI.Instance.SetMessageOptional(speechContent, friendsMBC.color1, friendsMBC.color2, friendsMBC.twoToneType, friendsIndexBottom + kabanID, nextEventCondition, 1, 1, true);
                            MessageUI.Instance.SetMessageLog(speechContent, friendsIndexBottom + kabanID);
                            eventTimer = 0f;
                            eventState = 3;
                        }
                    }
                } else if (eventState == 3) {
                    if (eventTimer >= nextEventCondition) {
                        string speechContent = TextManager.Get(GameManager.Instance.save.GotLuckyBeastCount >= GameManager.luckyBeastMax ? "EVENT_SLB_12_0" : "EVENT_SLB_12_0_NOEVENT");
                        nextEventCondition = MessageUI.Instance.GetSpeechAppropriateTime(speechContent.Length);
                        MessageUI.Instance.SetMessageOptional(speechContent, mesBackColor.color1, mesBackColor.color2, mesBackColor.twoToneType, faceIndex, nextEventCondition, 1, 1, true);
                        MessageUI.Instance.SetMessageLog(speechContent, faceIndex);
                        if (characterObj) {
                            ChangeRendererMaterial changer = characterObj.GetComponent<ChangeRendererMaterial>();
                            if (changer) {
                                changer.ChangeMaterial(2);
                            }
                        }
                        eventTimer = 0f;
                        eventState = 4;
                    }
                }
            }
            if (actionedFlag && eventState >= 1 && !anotherServalTalkedFlag && CharacterManager.Instance.GetFriendsExist(anotherServalID, true)) {
                anotherServalTalkedFlag = true;
                MessageBackColor friendsMBC = CharacterManager.Instance.GetMessageBackColor(anotherServalID);
                if (friendsMBC != null) {
                    string speechContent = TextManager.Get("EVENT_ANOTHERSERVAL_12_2");
                    MessageUI.Instance.SetMessageOptional(speechContent, friendsMBC.color1, friendsMBC.color2, friendsMBC.twoToneType, friendsIndexBottom + anotherServalID, -1, 1, 1, true);
                    MessageUI.Instance.SetMessageLog(speechContent, friendsIndexBottom + anotherServalID);
                }
            }
        }
    }

    protected override string GetTalkMargedName() {
        string mergedName = base.GetTalkMargedName();
        if (CharacterManager.Instance && CharacterManager.Instance.GetFriendsExist(kabanID, true)) {
            mergedName += kabanOption;
            if (restartedFlag || GameManager.Instance.save.GotLuckyBeastCount < GameManager.luckyBeastMax) {
                mergedName += noeventOption;
            }
        }
        return mergedName;
    }

    protected override string GetTalkMargedName_Home() {
        if (StageManager.Instance && StageManager.Instance.dungeonController && StageManager.Instance.dungeonController.sceneNumber == summitSceneNumber) {
            return sb.Clear().Append(talkHeader).Append(talkName).Append("_SUM").Append(homeTalk).Append(homeTalkOption).ToString();
        } else {
            return base.GetTalkMargedName_Home();
        }
    }

    protected override void DungeonAction() {
        base.DungeonAction();
        if (!InertIFR) {
            // GameManager.Instance.SetSecret(secretType);
            if (eventState < 2) {
                if (CharacterManager.Instance.GetFriendsExist(kabanID, true)) {
                    eventState = 2;
                } else {
                    eventState = 1;
                }
            }
        }
        if (eventState >= 4 && ignoreTimeRemain > 3f) {
            ignoreTimeRemain = 3f;
        }
    }

    protected override void DungeonUpdate() {
        base.DungeonUpdate();
        if (!InertIFR) {
            if (smile) {
                targetEnemy = searchEnemy.GetNowTarget();
                targetPlayer = searchPlayer.GetNowTarget();
                if (!targetEnemy) {
                    if (fCon) {
                        if (currentFaciemIndex != faciemIndex[(int)FaceName.Idle] && currentFaciemIndex != faciemIndex[(int)FaceName.Blink]) {
                            fCon.SetFace(faciemIndex[(int)FaceName.Idle], true);
                        }
                    }
                    if (targetPlayer && lookPlayer) {
                        CommonLockon(targetPlayer);
                    }
                } else {
                    enemy = true;
                    if (fCon) {
                        if (currentFaciemIndex != faciemIndex[(int)FaceName.Fear] && currentFaciemIndex != faciemIndex[(int)FaceName.Blink]) {
                            fCon.SetFace(faciemIndex[(int)FaceName.Fear], true);
                        }
                    }
                    if (targetEnemy && lookEnemy) {
                        CommonLockon(targetEnemy);
                    }
                }
            }
        }
    }

    protected override void Action() {
        if (!InertIFR) {
            actionedFlag = true;
            if (smile && !enemy && eventState >= 4 && ignoreTimeRemain <= 0f) {
                sb.Clear();
                bool enemyRemain = (StageManager.Instance && StageManager.Instance.dungeonController && StageManager.Instance.dungeonController.EnemyCount() >= 1);
                if (enemyRemain) {
                    sb.Append("EVENT_SLB_12_1");
                } else {
                    sb.Append("EVENT_SLB_12_2");
                }
                if (CharacterManager.Instance.GetFriendsExist(kabanID, true)) {
                    sb.Append(kabanOption);
                    if (enemyRemain && !extraObjActivated && (restartedFlag || GameManager.Instance.save.GotLuckyBeastCount < GameManager.luckyBeastMax)){
                        sb.Append(noeventOption);
                    }
                }
                string speechContent = TextManager.Get(sb.ToString());
                lookTimeRemain = MessageUI.Instance.GetSpeechAppropriateTime(speechContent.Length);
                MessageUI.Instance.SetMessageOptional(speechContent, mesBackColor.color1, mesBackColor.color2, mesBackColor.twoToneType, faceIndex, lookTimeRemain, 1, 1, true);
                MessageUI.Instance.SetMessageLog(speechContent, faceIndex);
                Instantiate(EffectDatabase.Instance.prefab[(int)EffectDatabase.id.talk], chTrans.position, quaIden);
                ignoreTimeRemain = 1.5f;
            }
        }
        base.Action();
    }

    public bool GetActedFlag() {
        return smile;
    }

    public bool GetEventFlag() {
        return eventState >= 2 && GameManager.Instance.save.GotLuckyBeastCount >= GameManager.luckyBeastMax;
    }

    protected override void SetHomeTalkOption() {
        if (stageNumber == 12 && StageManager.Instance.floorNumber == 14) {
            homeTalkIndex = 1;
        }
        if (CharacterManager.Instance.GetFriendsExist(kabanID, true)) {
            homeTalkOption = sb.Clear().AppendFormat("_{0:00}", homeTalkIndex).Append("_KABAN").ToString();
        } else {
            base.SetHomeTalkOption();
        }
    }

}
