using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mebiustos.MMD4MecanimFaciem;
using Rewired;
using System.Text;

public class ItemCharacter : MonoBehaviour {

    public bool isFriends;
    public bool unacquiredToDestroy = true;
    public int characterIndex;
    public int dungeonAnimIndex = -1;
    public int homeApAnimIndex = -1;
    public bool characterBaseDisablize;
    public int[] itemId;
    public bool itemForEachStages;
    public int itemForEachStagesInterval = 1;

    public bool lookEnemy;
    public bool lookPlayer;
    public bool lookPlayerAtHome;
    public bool lookPlayerAtAp;
    public bool lookPlayerAlways;
    public bool homeSpecialAction;
    public bool homeSpecialActionCancelOnTalking;
    public bool homeApSpecialAction;
    public bool homeApSpecialActionCancelOnTalking;
    public string homeApSpecialFace;

    public GameObject balloonPrefab;
    public Vector3 balloonOffset = new Vector3(0, 1.6f, 0);
    public GameObject searchEnemyPrefab;
    public GameObject searchPlayerPrefab;
    public string talkName;
    public float dungeonDelay = 1;
    public int bondConditionStage = -1;
    public int homeTalkMax = 2;
    public int homeApTalkMax;
    public bool homeTalkForProgress;
    public int[] homeConditionId;
    public bool dungeonGetTalkForEachStages;
    public string bonusType;
    public int bonusIndex = -1;
    public bool bonusIndexPlusStageNumber;
    public GameObject climbBalloonPrefab;
    public bool existHoldWeapon;
    public bool forceHome;

    protected GameObject characterObj;
    protected GameObject balloonInstance;
    protected SearchArea searchEnemy;
    protected SearchArea searchPlayer;
    protected Transform trans;
    protected Transform chTrans;
    protected StringBuilder sb = new StringBuilder();
    protected int stageNumber;
    protected bool isHome;
    protected bool exist;
    protected bool enemy;
    protected float rotSpeed;
    protected bool smile;
    protected bool started;
    protected float deltaTimeCache;
    protected float deltaTimeSpeech;
    protected float elapsedTime;
    protected float destroyTimeSpeech;
    protected float destroyTimeConstant;
    protected float lookTimeRemain;
    protected float ignoreTimeRemain;
    protected Animator anim;
    protected FaciemController fCon;
    protected GameObject targetEnemy;
    protected GameObject targetPlayer;
    protected int faceIndex;
    protected int homeTalkIndex;
    protected string dungeonTalkOption;
    protected string getTalkOption;
    protected string homeTalkOption;
    protected float itemBalloonDelay = 5f;
    protected Vector3 searchAreaOffset = new Vector3(0, 1, 0);

    protected float rotSpeedSave;
    protected float eulerAngleYSave;
    protected bool enemySave;
    protected GameObject[] balloonChild;

    protected LineChecker lineChecker;
    protected Transform camT;
    protected bool onStay;
    protected Player playerInput;
    protected bool submitButtonTalkEnabled = true;
    protected Transform cullingPivotTrans;
    protected bool antiInertIFR;
    protected bool isHomeAp;

    protected int currentFaciemIndex = -1;
    protected enum FaceName { Idle, Blink, Fear, Smile };
    protected int[] faciemIndex = new int[4];
    protected DynamicBone[] dynamicBones;
    protected bool dynamicBoneEnabled = true;
    protected bool clothEnabled;
    protected bool clothExist;
    protected float smileDestroyTimer = 5f;
    protected bool presentEnabled;
    protected int[] presentItemID;
    protected HoldWeaponObject holdWeapon;
    protected bool actionEnabled;

    protected FriendsBase fBase;
    protected MessageBackColor mesBackColor;
    protected GameObject climbBalloonInstance;
    protected GameObject mapChip;
    protected bool bondMessageShowed;
    protected float smileCancelTimer;
    protected const string dungeonTalk = "_DUNGEON";
    protected const string getTalk = "_GET";
    protected const string homeTalk = "_HOME";
    protected const string homeApTalk = "_HOMEAP";

    protected const string talkHeader = "TALK_";
    protected const int ballIndexIdle = 0;
    protected const int ballIndexFear = 1;
    protected const int friendsIndexBottom = 100;
    protected const int npcIndexBottom = 150;
    protected const int dungeonFaceIndex = 140;
    protected const float lineCheckDistanceHQ = 53f;
    protected const float invisibleDistanceHQ = 50f;
    protected const float lineCheckDistanceLQ = 33f;
    protected const float invisibleDistanceLQ = 30f;
    protected const float visibleDistance = 15f;
    protected const string targetTag = "ItemGetter";
    protected static readonly Quaternion quaIden = Quaternion.identity;
    protected static readonly Vector3 vecForward = Vector3.forward;
    protected static readonly Vector3 climbBalloonOffset = new Vector3(0f, 4f, 0f);
    
    protected virtual void Awake() {
        trans = transform;
        lineChecker = GetComponent<LineChecker>();
        stageNumber = StageManager.Instance.stageNumber;
        isHome = GetIsHome();
        isHomeAp = !forceHome && homeApTalkMax > 0 && StageManager.Instance && StageManager.Instance.IsAmusementParkFloor;
        if (isHome) {
            gameObject.tag = "Untagged";
        }
        if (isHomeAp) {
            lookPlayerAtHome = lookPlayerAtAp;
        }
        bool conditionCheck = true;
        if (unacquiredToDestroy && (isHome || InertIFR)) {
            conditionCheck = CheckCondition();
        }
        if (conditionCheck) {
            if (isFriends) {
                characterObj = Instantiate(CharacterDatabase.Instance.GetFriends(characterIndex), trans);
            } else {
                characterObj = Instantiate(CharacterDatabase.Instance.GetNPC(characterIndex), trans);
            }
        } else {
            Destroy(gameObject);
        }
        if (characterObj) {
            chTrans = characterObj.transform;
            eulerAngleYSave = chTrans.localEulerAngles.y;
            characterObj.layer = LayerMask.NameToLayer("Item");
            anim = characterObj.GetComponent<Animator>();
            fCon = characterObj.GetComponentInChildren<FaciemController>();
            mesBackColor = characterObj.GetComponent<MessageBackColor>();
            if (anim) {
                if (isHomeAp && homeApAnimIndex >= 1) {
                    anim.runtimeAnimatorController = CharacterDatabase.Instance.GetAnimCon(homeApAnimIndex);
                } else if (!isHome && dungeonAnimIndex >= 0) {
                    anim.runtimeAnimatorController = CharacterDatabase.Instance.GetAnimCon(dungeonAnimIndex);
                }
                anim.keepAnimatorControllerStateOnDisable = true;
            }
            if (fCon) {
                faciemIndex[(int)FaceName.Idle] = fCon.GetFaceIndex("Idle");
                faciemIndex[(int)FaceName.Blink] = fCon.GetFaceIndex("Blink");
                faciemIndex[(int)FaceName.Fear] = fCon.GetFaceIndex("Fear");
                faciemIndex[(int)FaceName.Smile] = fCon.GetFaceIndex("Smile");
            }
            if (characterBaseDisablize) {
                fBase = characterObj.GetComponent<FriendsBase>();
                if (fBase) {
                    fBase.SetForItem();
                    clothExist = fBase.SetClothEnabled(GameManager.Instance.save.config[GameManager.Save.configID_ClothSimulation]);
                    if (clothExist) {
                        clothEnabled = (GameManager.Instance.save.config[GameManager.Save.configID_ClothSimulation] >= 2);
                    }
                }
            }
            dynamicBones = characterObj.GetComponents<DynamicBone>();
            faceIndex = (isFriends ? friendsIndexBottom : npcIndexBottom) + characterIndex;
            if (!isHome) {
                if (balloonPrefab) {
                    balloonInstance = Instantiate(balloonPrefab, chTrans.position + balloonOffset, chTrans.rotation, chTrans);
                    int childCount = balloonInstance.transform.childCount;
                    balloonChild = new GameObject[childCount];
                    for (int i = 0; i < childCount; i++) {
                        balloonChild[i] = balloonInstance.transform.GetChild(i).gameObject;
                    }
                }
                if (climbBalloonPrefab) {
                    climbBalloonInstance = Instantiate(climbBalloonPrefab, chTrans.position + climbBalloonOffset, chTrans.rotation, chTrans);
                    climbBalloonInstance.SetActive(false);
                }
            }
            searchEnemy = Instantiate(searchEnemyPrefab, chTrans.position + searchAreaOffset, quaIden, chTrans).GetComponent<SearchArea>();
            searchPlayer = Instantiate(searchPlayerPrefab, chTrans.position + searchAreaOffset, quaIden, chTrans).GetComponent<SearchArea>();
            if (existHoldWeapon) {
                holdWeapon = characterObj.GetComponent<HoldWeaponObject>();
            }
        }
    }

    protected bool InertIFR => CharacterManager.Instance.inertIFR && !antiInertIFR;

    protected virtual bool GetIsHome() {
        return forceHome || StageManager.Instance.IsHomeStage;
    }

    protected virtual bool CheckCondition() {
        bool answer = true;
        if (GameManager.Instance.save.config[GameManager.Save.configID_GenerateHomeObjects] == 0) {
            answer = false;
        } else if (isFriends && characterIndex >= 0 && characterIndex < GameManager.friendsMax && GameManager.Instance.save.friends[characterIndex] == 0) {
            answer = false;
        } else if (!isFriends && bonusType == "Inventory" && GameManager.Instance.save.inventoryNFS[bonusIndex] == 0) {
            answer = false;
        } else {
            for (int i = 0; i < homeConditionId.Length && answer; i++) {
                answer = (GameManager.Instance.save.friends[homeConditionId[i]] != 0);
            }
        }
        return answer;
    }

    protected void CheckDynamicBones() {
        if (dynamicBones.Length > 0) {
            bool flag = (GameManager.Instance.save.config[GameManager.Save.configID_DynamicBone] >= 2);
            if (dynamicBoneEnabled != flag) {
                dynamicBoneEnabled = flag;
                for (int i = 0; i < dynamicBones.Length; i++) {
                    dynamicBones[i].enabled = flag;
                }
            }
        }
    }

    protected void CheckCloth() {
        bool flag = (GameManager.Instance.save.config[GameManager.Save.configID_ClothSimulation] >= 2);
        if (clothEnabled != flag && fBase) {
            clothEnabled = flag;
            fBase.SetClothEnabled(GameManager.Instance.save.config[GameManager.Save.configID_ClothSimulation]);
        }
    }

    public void SetCullingPivotTrans(Transform targetTrans) {
        cullingPivotTrans = targetTrans;
    }

    protected virtual void Culling() {
        if (characterObj && camT) {
            bool answer = true;
            float sqrDist = (camT.position - (cullingPivotTrans ? cullingPivotTrans.position : trans.position)).sqrMagnitude;
            if (sqrDist > MyMath.Square(GameManager.Instance.save.config[GameManager.Save.configID_QualityLevel] <= 0 ? invisibleDistanceLQ : invisibleDistanceHQ)) {
                answer = false;
            }
            if (lineChecker) {
                bool lineCheckEnabled = sqrDist < MyMath.Square(GameManager.Instance.save.config[GameManager.Save.configID_QualityLevel] <= 0 ? lineCheckDistanceLQ : lineCheckDistanceHQ);
                if (lineChecker.enabled != lineCheckEnabled) {
                    lineChecker.enabled = lineCheckEnabled;
                }
                if (answer && sqrDist > visibleDistance * visibleDistance) {
                    answer = lineChecker.reach;
                }
            }
            if (exist) {
                answer = false;
            }
            if (characterObj.activeSelf != answer) {
                characterObj.SetActive(answer);
            }
        }
    }

    protected virtual void KeyItemPlus() {
        StageManager.Instance.dungeonController.keyItemRemain += 1;
    }

    protected virtual void KeyItemMinus() {
        StageManager.Instance.dungeonController.keyItemRemain -= 1;
    }

    public bool GetCharacterActive() {
        return characterObj && characterObj.activeSelf;
    }

    protected virtual void Start() {
        playerInput = GameManager.Instance.playerInput;
        if (CameraManager.Instance) {
            CameraManager.Instance.SetMainCameraTransform(ref camT);
        } else {
            Camera mainCamera = Camera.main;
            if (mainCamera) {
                camT = mainCamera.transform;
            }
        }
        if (characterObj) {
            mapChip = Instantiate(MapDatabase.Instance.prefab[MapDatabase.itemCharacter], trans);
            if (isHome) {
                started = true;
            } else {
                started = false;
                elapsedTime = 0;
                if (StageManager.Instance && StageManager.Instance.dungeonController) {
                    if (PayBonus(true)) {
                        KeyItemPlus();
                    }
                    if (isFriends) {
                        if (Bond(true)) {
                            KeyItemPlus();
                        }
                    }
                }
            }
            if (homeTalkForProgress && !isHomeAp) {
                homeTalkIndex = Mathf.Clamp(GameManager.Instance.save.progress, 0, GameManager.progressMax);
                homeTalkMax = homeTalkIndex + 1;
            } else {
                homeTalkIndex = 0;
            }
            if (dungeonGetTalkForEachStages) {
                getTalkOption = dungeonTalkOption = sb.Clear().AppendFormat("_{0:00}", stageNumber).ToString();
            }
            if (isHomeAp && !string.IsNullOrEmpty(homeApSpecialFace)) {
                fBase.SetFaceString(homeApSpecialFace, 9999999, true);
            }
        }
        if (CharacterManager.Instance && (isHome || InertIFR) && balloonInstance) {
            balloonInstance.SetActive(false);
        }        
    }

    protected virtual bool PayBonus(bool forCheck = false) {
        bool answer = false;
        if (!string.IsNullOrEmpty(bonusType)) {
            int pay = bonusIndex;
            if (bonusIndexPlusStageNumber) {
                pay += stageNumber;
            }
            switch (bonusType) {
                case "HP":
                    if (pay >= 0 && pay < GameManager.hpUpNFSMax) {
                        if (forCheck) {
                            if (GameManager.Instance.save.hpUpNFS[pay] == 0) {
                                answer = true;
                            }
                        } else {
                            GameManager.Instance.save.weapon[GameManager.hpUpId - 200] = 1;
                            GameManager.Instance.hpUpLimitAuto += 1;
                            if (GameManager.Instance.save.hpUpNFS[pay] == 0) {
                                GameManager.Instance.save.equip[GameManager.hpUpId - 200] = 1;
                                GameManager.Instance.save.hpUpNFS[pay] = 1;
                                GameManager.Instance.save.equipedHpUp = GameManager.Instance.save.GotHpUpOriginal;
                                MessageUI.Instance.SetMessage(TextManager.Get("MESSAGE_HPUP"), MessageUI.time_Important, MessageUI.panelType_Information, MessageUI.slotType_HP);
                                answer = true;
                            }
                        }
                    }
                    break;
                case "ST":
                    if (pay >= 0 && pay < GameManager.stUpNFSMax) {
                        if (forCheck) {
                            if (GameManager.Instance.save.stUpNFS[pay] == 0) {
                                answer = true;
                            }
                        } else {
                            GameManager.Instance.save.weapon[GameManager.stUpId - 200] = 1;
                            GameManager.Instance.stUpLimitAuto += 1;
                            if (GameManager.Instance.save.stUpNFS[pay] == 0) {
                                GameManager.Instance.save.equip[GameManager.stUpId - 200] = 1;
                                GameManager.Instance.save.stUpNFS[pay] = 1;
                                GameManager.Instance.save.equipedStUp = GameManager.Instance.save.GotStUpOriginal;
                                MessageUI.Instance.SetMessage(TextManager.Get("MESSAGE_STUP"), MessageUI.time_Important, MessageUI.panelType_Information, MessageUI.slotType_ST);
                                answer = true;
                            }
                        }
                    }
                    break;
                case "Inventory":
                    if (pay >= 0 && pay < GameManager.inventoryNFSMax) {
                        if (forCheck) {
                            if (GameManager.Instance.save.inventoryNFS[pay] == 0) {
                                answer = true;
                            }
                        } else {
                            GameManager.Instance.save.weapon[GameManager.invUpId - 200] = 1;
                            GameManager.Instance.inventoryLimitAuto += 1;
                            if (GameManager.Instance.save.inventoryNFS[pay] == 0) {
                                GameManager.Instance.save.equip[GameManager.invUpId - 200] = 1;
                                GameManager.Instance.save.inventoryNFS[pay] = 1;
                                GameManager.Instance.save.equipedInvUp = GameManager.Instance.save.GotInvUpOriginal;
                                MessageUI.Instance.SetMessage(TextManager.Get("MESSAGE_INVUP"), MessageUI.time_Important, MessageUI.panelType_Information, MessageUI.slotType_Inventory);
                                answer = true;
                            }
                        }
                    }
                    break;
                case "LuckyBeast":
                    if (pay >= 0 && pay < GameManager.luckyBeastMax) {
                        if (GameManager.Instance.save.luckyBeast[pay] == 0) {
                            answer = true;
                        }
                        if (!forCheck) {
                            GameManager.Instance.save.luckyBeast[pay] = 1;
                        }
                    }
                    break;
                case "SLB":
                    if (GameManager.Instance.GetSecret(GameManager.SecretType.SingularityLB) == false) {
                        answer = true;
                    }
                    if (!forCheck) {
                        GameManager.Instance.SetSecret(GameManager.SecretType.SingularityLB);
                    }
                    break;
            }
        }
        return answer;
    }

    protected virtual void CheckExist() {
        if (CharacterManager.Instance != null) {
            bool existTemp = CharacterManager.Instance.GetFriendsExist(characterIndex, false);
            if (exist != existTemp) {
                exist = existTemp;
                bool inertTemp = InertIFR;
                if (balloonInstance) {
                    balloonInstance.SetActive(!inertTemp && !exist);
                }
                if (climbBalloonInstance) {
                    climbBalloonInstance.SetActive(!inertTemp && !exist && CharacterManager.Instance.isClimbing);
                }
                characterObj.SetActive(!exist);
                mapChip.SetActive(!exist);
            }
        }
    }

    protected virtual void Update() {
        deltaTimeCache = Time.deltaTime;
        deltaTimeSpeech = deltaTimeCache * (MessageUI.Instance ? MessageUI.Instance.GetMessageTimeSpeed() : 1f);
        if (isFriends) {
            CheckExist();
        }
        if (!isFriends || !exist || !unacquiredToDestroy) {
            if (isHome) {
                HomeUpdate();
                Culling();
            } else {
                DungeonUpdate();
                if (InertIFR) {
                    Culling();
                }
            }
        }
        if (!InertIFR && !PauseController.Instance.pauseGame && submitButtonTalkEnabled && onStay && ignoreTimeRemain <= 0 && playerInput.GetButtonDown(RewiredConsts.Action.Submit)) {
            Action();
        }
        CheckDynamicBones();
        if (clothExist) {
            CheckCloth();
        }
        if (smileCancelTimer > 0f) {
            smileCancelTimer -= deltaTimeCache;
            if (smileCancelTimer <= 0f && fCon && fCon.CurrentFaceIndex == faciemIndex[(int)FaceName.Smile]) {
                fCon.SetFace(faciemIndex[(int)FaceName.Idle], true);
            }
        }
    }

    protected virtual void DungeonUpdate() {
        int balloonState = -1;
        int currentFaciemIndex = -1;
        enemy = false;
        if (ignoreTimeRemain > 0f) {
            ignoreTimeRemain -= deltaTimeCache;
        }
        if (!smile) {
            if (!started) {
                if (!SceneChange.Instance.GetIsProcessing && !PauseController.Instance.pauseGame) {
                    elapsedTime += deltaTimeSpeech;
                    if (elapsedTime >= dungeonDelay && dungeonDelay < 10000 && MessageUI.Instance.GetMessageCount(MessageUI.panelType_Speech) == 0) {
                        started = true;
                        if (!InertIFR) {
                            string speechContent = TextManager.Get(sb.Clear().Append(talkHeader).Append(talkName).Append(dungeonTalk).Append(dungeonTalkOption).ToString());
                            MessageUI.Instance.SetMessageOptional(speechContent, mesBackColor.color1, mesBackColor.color2, mesBackColor.twoToneType, dungeonFaceIndex, -1, 1, 1, true);
                            MessageUI.Instance.SetMessageLog(speechContent, dungeonFaceIndex);
                            Instantiate(EffectDatabase.Instance.prefab[(int)EffectDatabase.id.talk], chTrans.position, quaIden);
                        }
                    }
                }
            }
            targetEnemy = searchEnemy.GetNowTarget();
            targetPlayer = searchPlayer.GetNowTarget();
            if (fCon) {
                currentFaciemIndex = fCon.CurrentFaceIndex;
            }
            if (!targetEnemy) {
                if (fCon) {
                    if (currentFaciemIndex != faciemIndex[(int)FaceName.Idle] && currentFaciemIndex != faciemIndex[(int)FaceName.Blink]) {
                        fCon.SetFace(faciemIndex[(int)FaceName.Idle], true);
                    }
                }
                if (targetPlayer && lookPlayer) {
                    CommonLockon(targetPlayer);
                }
                balloonState = ballIndexIdle;
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
                balloonState = ballIndexFear;
            }
            if (CharacterManager.Instance && climbBalloonInstance && !exist) {
                if (climbBalloonInstance.activeSelf && !CharacterManager.Instance.isClimbing) {
                    climbBalloonInstance.SetActive(false);
                } else if (!climbBalloonInstance.activeSelf && CharacterManager.Instance.isClimbing) {
                    climbBalloonInstance.SetActive(true);
                }
            }
        } else {
            if (fCon) {
                if (currentFaciemIndex != faciemIndex[(int)FaceName.Smile]) {
                    fCon.SetFace(faciemIndex[(int)FaceName.Smile], true);
                }
            }
            destroyTimeSpeech += deltaTimeSpeech;
            destroyTimeConstant += deltaTimeCache;
            if (destroyTimeSpeech > smileDestroyTimer && destroyTimeConstant > 5f) {
                Destroy(gameObject);
            }
        }
        if (characterObj.activeSelf) {
            CalcRotSpeed();
            if (enemy != enemySave) {
                anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.Fear], enemy);
                enemySave = enemy;
            }
        }
        if (balloonInstance) {
            for (int i = 0; i < balloonChild.Length; i++) {
                if (i == balloonState ^ balloonChild[i].activeSelf) {
                    balloonChild[i].SetActive(i == balloonState);
                }
            }
        }
    }

    protected virtual void HomeUpdate() {
        if (ignoreTimeRemain > 0f) {
            ignoreTimeRemain -= deltaTimeCache;
        }
        if (lookPlayerAlways) {
            lookTimeRemain = 5f;
        }
        if (lookTimeRemain > 0f) {
            lookTimeRemain -= deltaTimeSpeech;
            targetPlayer = searchPlayer.GetNowTarget();
            if (targetPlayer && lookPlayerAtHome) {
                CommonLockon(targetPlayer);
            }
        } else {
            if (lookPlayerAtHome) {
                LerpRotationToIdentity();
            }
        }
        if (isHomeAp) {
            if (homeApSpecialAction && anim && anim.enabled && characterObj.activeSelf && !anim.GetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.HomeAction])) {
                if (!homeApSpecialActionCancelOnTalking || lookTimeRemain <= 0f) {
                    anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.HomeAction], true);
                }
            }
        } else if (isHome) {
            if (homeSpecialAction && anim && anim.enabled && characterObj.activeSelf && !anim.GetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.HomeAction])) {
                if (!homeSpecialActionCancelOnTalking || lookTimeRemain <= 0f) {
                    anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.HomeAction], true);
                }
            }
        }
        if (characterObj.activeSelf) {
            CalcRotSpeed();
            SetActionText();
        }        
    }

    protected virtual string GetTalkMargedName() {
        return sb.Clear().Append(talkHeader).Append(talkName).Append(getTalk).Append(getTalkOption).ToString();
    }

    protected virtual void DungeonAction() {
        if (!InertIFR && (!isFriends || !exist)) {
            ignoreTimeRemain = 10f;
            smile = true;
            started = true;
            anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Smile]);
            CharacterManager.Instance.EveryFriendsSmile();
            if (balloonInstance) {
                Destroy(balloonInstance);
            }
            if (GameManager.Instance.save.moveByBus == 0) {
                if (itemForEachStages) {
                    if (itemForEachStagesInterval > 0 && itemId.Length >= (stageNumber + 1) * itemForEachStagesInterval) {
                        int[] itemIdTemp = new int[itemForEachStagesInterval];
                        for (int i = 0; i < itemForEachStagesInterval; i++) {
                            itemIdTemp[i] = itemId[stageNumber * itemForEachStagesInterval + i];
                        }
                        ItemDatabase.Instance.GiveItem(itemIdTemp, chTrans, 5, itemBalloonDelay);
                    }
                } else {
                    if (itemId.Length > 0) {
                        ItemDatabase.Instance.GiveItem(itemId, chTrans, 5, itemBalloonDelay);
                    }
                }
            }
            if (!string.IsNullOrEmpty(getTalk)) {
                string speechContent = TextManager.Get(GetTalkMargedName());
                smileDestroyTimer = Mathf.Max(smileDestroyTimer, MessageUI.Instance.GetSpeechAppropriateTime(speechContent.Length));
                MessageUI.Instance.SetMessageOptional(speechContent, mesBackColor.color1, mesBackColor.color2, mesBackColor.twoToneType, faceIndex, -1, MessageUI.panelType_Speech, MessageUI.slotType_Speech, true);
                MessageUI.Instance.SetMessageLog(speechContent, faceIndex);
                Instantiate(EffectDatabase.Instance.prefab[(int)EffectDatabase.id.getFriends], chTrans.position, quaIden);
            }
            if (stageNumber >= bondConditionStage) {
                int itemID = (isFriends ? friendsIndexBottom : npcIndexBottom) + characterIndex;
                MessageUI.Instance.SetMessage(sb.Clear().Append(TextManager.Get("QUOTE_START")).Append(ItemDatabase.Instance.GetItemName(itemID)).Append(TextManager.Get("QUOTE_END")).Append(TextManager.Get("MESSAGE_RESCUE")).ToString(), MessageUI.time_Important, MessageUI.panelType_Information, MessageUI.slotType_Friends);
            }
            if (isFriends) {
                if (Bond(false)) {
                    if (StageManager.Instance && StageManager.Instance.dungeonController) {
                        KeyItemMinus();
                    }
                }
            }
            if (PayBonus(false)) {
                if (StageManager.Instance && StageManager.Instance.dungeonController) {
                    KeyItemMinus();
                }
            }
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_RescueAllFriends);
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_RescueAllLB);
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_GetAllHp);
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_GetAllSt);
        }
    }

    protected virtual void SetHomeTalkOption() {
        homeTalkOption = sb.Clear().AppendFormat("_{0:00}", homeTalkIndex).ToString();
    }

    protected virtual void SetHomeTalkIndex() {
        homeTalkIndex++;
        if (isHomeAp) {
            if (homeTalkIndex >= homeApTalkMax) {
                homeTalkIndex = 0;
            }
        } else {
            if (homeTalkIndex >= homeTalkMax) {
                homeTalkIndex = 0;
            }
        }
    }

    public virtual void ForceHomeTalkIndex(int index) {
        homeTalkIndex = index;
    }

    protected virtual string GetTalkMargedName_Home() {
        return sb.Clear().Append(talkHeader).Append(talkName).Append(isHomeAp ? homeApTalk : homeTalk).Append(homeTalkOption).ToString();
    }

    protected virtual void HomeAction() {
        if (!InertIFR && (!isFriends || !exist) && ignoreTimeRemain <= 0) {
            SetHomeTalkOption();
            ignoreTimeRemain = 1.5f;
            string speechContent = TextManager.Get(GetTalkMargedName_Home());
            lookTimeRemain = MessageUI.Instance.GetSpeechAppropriateTime(speechContent.Length);
            MessageUI.Instance.SetMessageOptional(speechContent, mesBackColor.color1, mesBackColor.color2, mesBackColor.twoToneType, faceIndex, lookTimeRemain, MessageUI.panelType_Speech, MessageUI.slotType_Speech, true);
            MessageUI.Instance.SetMessageLog(speechContent, faceIndex);
            Instantiate(EffectDatabase.Instance.prefab[(int)EffectDatabase.id.talk], chTrans.position, quaIden);
            SetHomeTalkIndex();
            if (isHomeAp) {
                if (homeApSpecialAction && homeApSpecialActionCancelOnTalking && anim) {
                    anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.HomeAction], false);
                }
            } else {
                if (homeSpecialAction && homeSpecialActionCancelOnTalking && anim) {
                    anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.HomeAction], false);
                }
            }
            if (presentEnabled && presentItemID.Length > 0) {
                presentEnabled = false;
                Instantiate(EffectDatabase.Instance.prefab[(int)EffectDatabase.id.getFriends], chTrans.position, quaIden);
                ItemDatabase.Instance.GiveItem(presentItemID, chTrans, 5, itemBalloonDelay);
            }
        }
    }

    protected virtual bool Bond(bool forCheck = false) {
        bool answer = false;
        if (stageNumber >= bondConditionStage && !InertIFR) {
            if (forCheck) {
                if (GameManager.Instance.save.friends[characterIndex] == 0) {
                    answer = true;
                }
            } else {
                GameManager.Instance.save.friends[0] = 1;
                GameManager.Instance.save.weapon[2] = 1;
                if (GameManager.Instance.save.friends[characterIndex] == 0) {
                    GameManager.Instance.save.friends[characterIndex] = 1;
                    if (MessageUI.Instance != null && ItemDatabase.Instance != null) {
                        int itemID = friendsIndexBottom + characterIndex;
                        MessageUI.Instance.SetMessage(sb.Clear().Append(TextManager.Get("QUOTE_START")).Append(ItemDatabase.Instance.GetItemName(itemID)).Append(TextManager.Get("QUOTE_END")).Append(TextManager.Get("MESSAGE_FRIENDSBOND")).ToString(), MessageUI.time_Important, MessageUI.panelType_Information, MessageUI.slotType_Friends);
                    }
                    answer = true;
                }
            }
        }
        return answer;
    }

    protected virtual void Action() {
        if (isHome) {
            HomeAction();
        } else {
            if (!smile && !enemy) {
                DungeonAction();
            }
        }
    }

    protected virtual void OnTriggerEnter(Collider other) {
        if (other.CompareTag(targetTag)) {
            onStay = true;
            if (CharacterManager.Instance.pCon && CharacterManager.Instance.pCon.GetTalkEnabled()) {
                Action();
            }
        }
    }

    protected virtual void OnTriggerExit(Collider other) {
        if (other.CompareTag(targetTag)) {
            onStay = false;
        }
    }

    void CalcRotSpeed() {
        if (deltaTimeCache > 0f) {
            float newEulerAngleY = chTrans.localEulerAngles.y;
            if (eulerAngleYSave > newEulerAngleY + 180f) {
                eulerAngleYSave -= 360f;
            } else if (eulerAngleYSave < newEulerAngleY - 180f) {
                eulerAngleYSave += 360f;
            }
            float diff = newEulerAngleY - eulerAngleYSave;
            if (diff > -0.001f && diff < 0.001f) {
                if (rotSpeed < -0.001f) {
                    rotSpeed = Mathf.Clamp(rotSpeed + deltaTimeCache * 2f, -1f, 0f);
                } else if (rotSpeed > 0.001f) {
                    rotSpeed = Mathf.Clamp(rotSpeed - deltaTimeCache * 2f, 0f, 1f);
                } else {
                    rotSpeed = 0f;
                }
            } else {
                rotSpeed = Mathf.Clamp(Mathf.Lerp(rotSpeed, Mathf.Clamp(diff, -2f, 2f), deltaTimeCache), -1f, 1f);
            }
            eulerAngleYSave = newEulerAngleY;
            if (rotSpeed != rotSpeedSave) {
                anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.RotSpeed], rotSpeed);
                rotSpeedSave = rotSpeed;
            }
        }
    }

    protected virtual void CommonLockon(GameObject target) {
        if (target != null && deltaTimeCache > 0f) {
            Vector3 toPos = target.transform.position;
            Vector3 fromPos = chTrans.position;
            toPos.y = fromPos.y;
            Vector3 diff = toPos - fromPos;
            if ((diff).sqrMagnitude > 0.01f) {
                float step = 8f * deltaTimeCache;
                Quaternion targetRot = Quaternion.LookRotation(diff);
                Quaternion nowRot = chTrans.rotation;
                nowRot = Quaternion.LookRotation(Vector3.RotateTowards(chTrans.TransformDirection(vecForward), diff, step * 0.15f, 0f));
                chTrans.rotation = Quaternion.Slerp(nowRot, targetRot, step * 0.45f);
            }
        }
    }

    protected virtual void LerpRotationToIdentity() {
        if (deltaTimeCache > 0f) {
            float yAngle = chTrans.localEulerAngles.y;
            if (yAngle != 0f) {
                if (yAngle > -0.05f && yAngle < 0.05f) {
                    chTrans.localRotation = quaIden;
                } else {
                    chTrans.localRotation = Quaternion.Slerp(chTrans.localRotation, quaIden, 5f * deltaTimeCache);
                }
            }
        }
    }

    public void SetPresentItem(int[] presentItemID) {
        presentEnabled = true;
        this.presentItemID = presentItemID;
    }

    public void SetSmileTimer(float timer = 5f) {
        if (fCon && fCon.CurrentFaceIndex != faciemIndex[(int)FaceName.Smile]) {
            fCon.SetFace(faciemIndex[(int)FaceName.Smile], true);
        }
        smileCancelTimer = timer;
    }

    protected virtual void SetActionText() {
        if (CharacterManager.Instance) {
            if (!InertIFR && !PauseController.Instance.pauseGame && onStay && ignoreTimeRemain <= 0) {
                if (!actionEnabled) {
                    CharacterManager.Instance.SetActionType(CharacterManager.ActionType.Talk, gameObject);
                    actionEnabled = true;
                }
            } else {
                if (actionEnabled) {
                    CharacterManager.Instance.SetActionType(CharacterManager.ActionType.None, gameObject);
                    actionEnabled = false;
                }
            }
        }
    }

}
