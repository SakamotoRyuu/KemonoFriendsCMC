using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.PostProcessing;
using Rewired;
using Mebiustos.MMD4MecanimFaciem;
using System.Text;

public class DicFriendsConsole : ConsoleBase {

    [System.Serializable]
    public class MotionException {
        public int motionIndex;
        public float attackSpeed;
        public string faceName;
    }

    [System.Serializable]
    public class FriendsData {
        public int characterIndex;
        public bool isNPC;
        public bool isLuckyBeast;
        public bool isPlayer;
        public bool afterEnd;
        public bool afterComplete;
        public int nameID;
        public long motionFlags;
        public int expressionFlags;
        public float heightOffset;
        public MotionException[] motionExceptions;
    }

    public enum EnemyType { ZakoBlue, ZakoGreen, Boss };

    [System.Serializable]
    public class EnemyData {
        public int id;
        public EnemyType type;
        public long motionFlags;
        public float height;
        public float radius;
        public float cameraScalePlus;
        public bool knockHeavyHalfSpeed;
        public bool friendsAnimator;
        public bool secret;
        public bool defaultSuperman;
    }
    
    public enum FacilityType { Friends, Enemy, Lucky, Minmi, Complete };

    [System.Serializable]
    public class FacilityData {
        public int id;
        public FacilityType type;
        public int[] requireFriends;
        public float height;
        public float radius;
        public float cameraScalePlus;
    }

    [System.Serializable]
    public class DropEnemy {
        public int id;
        public int levelMin;
        public int levelMax;
    }

    [System.Serializable]
    public class ItemData {
        public int id;
        public DropEnemy[] dropEnemy;
        public float height;
        public float radius;
    }

    public GameObject cameraObj;
    public Camera cameraFirst;
    public Camera cameraSecond;
    public PostProcessingBehaviour[] ppBehaviourFriends;
    public PostProcessingProfile ppProfileMainBlur;
    public Transform friendsCenter;
    public Transform friendsPivot;
    public GameObject friendsLayerObj;
    public Canvas parentCanvas;
    public Canvas[] childCanvas;
    public Canvas[] eachTypeCanvas;
    public Canvas[] additiveCanvas;
    public GraphicRaycaster[] childRaycaster;
    public TMP_Text friendsHeader;
    public TMP_Text percentText;
    public Image[] indexIcons;
    public TMP_Text[] indexNames;
    public GridLayoutGroup[] gridLayoutGroups;
    public Material iconShowMaterial;
    public Material iconHideMaterial;
    public Image choosingIconFace;
    public RectTransform cursor;
    public Vector2 cursorOrigin;
    public Vector2 cursorInterval;
    public Image talkBackLeft;
    public Image talkBackRight;
    public TMP_Text textName;
    public TMP_Text textType;
    public TMP_Text textAlias;
    public TMP_Text textInfo;
    public FriendsData[] friendsData;
    public RectTransform detailCursor;
    public Vector2 detailCursorOrigin;
    public Vector2 detailCursorInterval;
    public TMP_Text detailTextName;
    public TMP_Text detailTextCameraCaption;
    public TMP_Text detailTextCameraType;
    public TMP_Text detailTextMotionCaption;
    public TMP_Text detailTextMotionType;
    public TMP_Text detailTextExpressionCaption;
    public TMP_Text detailTextExpressionType;
    public TMP_Text detailTextSpeedCaption;
    public TMP_Text detailTextSpeedType;
    public TMP_Text additiveTextCameraCaption;
    public TMP_Text additiveTextCameraType;
    public GameObject wildReleaseStartPrefab;
    public GameObject wildReleaseEndPrefab;
    public TMP_Text friendsOperationTextSubmit;
    public TMP_Text friendsOperationTextWildRelease;
    public EnemyData[] enemyData;
    public MessageBackColor[] enemyMessageColor;
    public Image[] enemyLevelFrame;
    public Image[] enemyLevelFill;
    public Color[] enemyLevelColor;
    public FacilityData[] facilityData;
    public MessageBackColor facilityColor;
    public Image facilityCheckImage;
    public Sprite[] facilityCheckSprite;
    public TMP_Text[] facilityTextChange;
    public Image[] quitArrows;
    public Image nextArrow;
    public Image facilityEventImage;
    public ItemData[] itemData;
    public TMP_Text itemDropText;
    public MessageBackColor itemColor;
    public TMP_ColorGradient whiteGradient;
    public TMP_ColorGradient rainbowGradient;
    public RectTransform[] percentagePosition;
    
    private int pointer;
    private int cursorPos;
    private int listOrigin;
    private int dicState;
    private GameObject characterInstance;
    private FriendsBase fBase;
    private Vector2Int move;
    private int[] configSave = new int[2];
    private float additiveCanvasTimeRemain;
    private int detailCursorPos;
    private int detailMotionType;
    private int detailMotionMem;
    private int detailExpressionType;
    private int detailExpressionMem;
    private int detailCameraType;
    private int detailSpeedType;
    private FaciemController faceCon;
    private Animator anim;
    private float idlingTime;
    private float jumpingTime;
    private bool wildReleaseFlag;
    private float wildReleaseTime;
    private bool operationShowed;
    private static readonly Vector3 vecZero = Vector3.zero;
    private const int gridLayoutCanvasType = 0;
    private const int detailMotionMax = 46;
    private const int detailExpressionMax = 17;
    private const int detailSpeedMax = 21;
    private static readonly Vector3 friendsPivotPositionBase = new Vector3(0f, -0.72f, 0f);
    private static readonly Vector3 friendsCameraPosition = new Vector3(0f, 0f, 1.5f);
    private const int dummyEnemyID = 54;
    private float cameraMoveSpeed = 0.6f;
    private Vector3 cameraMoveRange = new Vector3(0.6f, 1.2f, 1.2f);
    private int enemyLevelMem;
    private int enemyLevelNow;
    private EnemyBase eBase;
    private StringBuilder sb = new StringBuilder();
    private int choiceAnswerPre = -1;
    private int eventEnterNum = -1;
    private int eventClickNum = -1;
    private int enemyCompNow;
    private int enemyCompMax;
    private float mouseScroll;


    private static readonly string[] enemyColorTags = new string[] {
        "<color=#FFFFBF>", "<color=#BFDFFF>", "<color=#FFC0C0>", "<color=#DFBFFF>", "<color=#C0C0C0>",
        "<color=#BFFFFF>", "<color=#BFFFBF>", "<color=#FFCFBF>", "<color=#BFBFFF>", "<color=#C0C0C0>"
    };
    private const string colorTagEnd = "</color>";
    private const int choiceType_Friends = 0;
    private const int choiceType_Enemy = 1;
    private const int choiceType_Facility = 2;
    private const int choiceType_Item = 3;

    private void InitCharacter() {
        fBase = null;
        faceCon = null;
        eBase = null;
        if (characterInstance) {
            Destroy(characterInstance);
        }
    }

    private void SetFriends(int pointer) {
        if (characterInstance) {
            fBase = null;
            faceCon = null;
            Destroy(characterInstance);
        }
        if (pointer >= 0 && pointer < friendsData.Length) {
            GameObject characterPrefab = GetCharacterObj(pointer);
            if (characterPrefab) {
                if (CheckFriendsCondition(pointer)) {
                    textName.text = TextManager.Get(string.Format("ITEM_NAME_{0:D3}", friendsData[pointer].nameID));
                    textType.text = TextManager.Get(string.Format("DIC_FRIENDS_{0:D2}_TYPE", pointer));
                    textAlias.text = TextManager.Get(string.Format("DIC_FRIENDS_{0:D2}_ALIAS", pointer));
                    textInfo.text = TextManager.Get(string.Format("DIC_FRIENDS_{0:D2}_INFO", pointer));
                    friendsPivot.localPosition = friendsPivotPositionBase + Vector3.up * friendsData[pointer].heightOffset;
                    characterInstance = Instantiate(characterPrefab, friendsPivot);
                    if (friendsData[pointer].isNPC) {
                        fBase = null;
                        GameObject ifrPrefab = ItemDatabase.Instance.GetItemPrefab(friendsData[pointer].nameID);
                        if (ifrPrefab) {
                            characterInstance.GetComponent<Animator>().runtimeAnimatorController = CharacterDatabase.Instance.GetAnimCon(ifrPrefab.GetComponent<ItemCharacter>().dungeonAnimIndex);
                        }
                    } else {
                        fBase = characterInstance.GetComponent<FriendsBase>();
                        if (fBase) {
                            fBase.SetForDictionary();
                        }
                    }
                    faceCon = characterInstance.GetComponent<FaciemController>();
                    anim = characterInstance.GetComponent<Animator>();
                    MessageBackColor messageBackColor = characterInstance.GetComponent<MessageBackColor>();
                    if (messageBackColor) {
                        SetTalkBack(messageBackColor);
                    }
                    ReferGameObjects refObjs = characterInstance.GetComponent<ReferGameObjects>();
                    if (refObjs) {
                        for (int i = 0; i < refObjs.gameObjects.Length; i++) {
                            if (refObjs.gameObjects[i]) {
                                refObjs.gameObjects[i].layer = friendsLayerObj.layer;
                            }
                        }
                    }
                    detailMotionType = detailMotionMem;
                    if (detailMotionType != 0) {
                        for (int i = 0; i < detailMotionMax; i++) {
                            if ((friendsData[pointer].motionFlags & (1L << detailMotionType)) != 0L) {
                                SetMotion_Start();
                                break;
                            } else {
                                detailMotionType--;
                                if (detailMotionType < 0) {
                                    detailMotionType = detailMotionMax - 1;
                                }
                            }
                        }
                    } else if (anim) {
                        anim.speed = detailSpeedType * 0.1f;
                    }
                    detailExpressionType = detailExpressionMem;
                    if (detailExpressionType != 0) {
                        for (int i = 0; i < detailExpressionMax; i++) {
                            if ((friendsData[pointer].expressionFlags & (1 << detailExpressionType)) != 0) {
                                SetExpression();
                                break;
                            } else {
                                detailExpressionType--;
                                if (detailExpressionType < 0) {
                                    detailExpressionType = detailExpressionMax - 1;
                                }
                            }
                        }
                    }
                    if (wildReleaseFlag && fBase && GetEnableWildRelease()) {
                        fBase.SupermanStart(false);
                    }
                    if (cameraObj) {
                        cameraObj.transform.localPosition = friendsCameraPosition;
                    }
                } else {
                    textName.text = TextManager.Get("DIC_LOCK");
                    textType.text = "";
                    textAlias.text = "";
                    textInfo.text = "";
                    MessageBackColor messageBackColor = characterPrefab.GetComponent<MessageBackColor>();
                    if (messageBackColor) {
                        SetTalkBack(messageBackColor);
                    }
                }
            }
        }
    }

    private void SetEnemyLevel(int pointer) {
        if (eBase) {
            eBase.SetLevel(enemyLevelNow, false, false);
            if (enemyData[pointer].type == EnemyType.Boss) {
                eBase.SetForDictionary(enemyData[pointer].defaultSuperman || enemyLevelNow >= GameManager.enemyLevelMax - 1, friendsLayerObj.layer);
            }
        }
        SetTalkBack(enemyMessageColor[(enemyData[pointer].type == EnemyType.Boss ? 5 : 0) + enemyLevelNow]);
    }

    private void SetEnemy(int pointer) {
        if (characterInstance) {
            eBase = null;
            Destroy(characterInstance);
        }
        if (pointer >= 0 && pointer < enemyData.Length) {
            int idTemp = enemyData[pointer].id;
            GameObject characterPrefab = CharacterDatabase.Instance.GetEnemy(idTemp);
            if (characterPrefab) {
                if (CheckEnemyCondition(pointer)) {
                    textName.text = TextManager.Get(string.Format("CELLIEN_NAME_{0:D2}", idTemp));
                    textType.text = "";
                    textAlias.text = TextManager.Get(string.Format("DIC_ENEMY_ALIAS_{0:D2}", idTemp));
                    string[] infoArray = TextManager.Get(string.Format("DIC_ENEMY_INFO_{0:D2}", idTemp)).Split('@');
                    sb.Clear();
                    for (int i = 0; i < infoArray.Length; i++) {
                        if (!string.IsNullOrEmpty(infoArray[i])) {
                            if (i == 0) {
                                sb.Append(infoArray[i]);
                            } else if (GameManager.Instance.save.defeatEnemy[idTemp * GameManager.enemyLevelMax + i - 1] > 0) {
                                sb.Append(enemyColorTags[(enemyData[pointer].type == EnemyType.ZakoGreen ? 5 : 0) + i - 1]).Append(infoArray[i]).Append(colorTagEnd);
                            } else {
                                sb.Append(enemyColorTags[(enemyData[pointer].type == EnemyType.ZakoGreen ? 5 : 0) + i - 1]).AppendLine().Append(TextManager.Get("DIC_ENEMY_INFO_LOCK")).Append(colorTagEnd);
                            }
                        }
                    }
                    int lvIndex = GameManager.enemyLevelMax;
                    for (int i = 0; i < GameManager.enemyLevelMax; i++) {
                        if (CharacterDatabase.Instance.enemy[idTemp].statusExist[i]) {
                            lvIndex--;
                        }
                    }
                    for (int i = 0; i < lvIndex; i++) {
                        enemyLevelFrame[i].enabled = false;
                        enemyLevelFill[i].enabled = false;
                    }
                    for (int i = 0; i < GameManager.enemyLevelMax && lvIndex < enemyLevelFill.Length; i++) {                        
                        if (CharacterDatabase.Instance.enemy[idTemp].statusExist[i] && lvIndex < enemyLevelFill.Length) {
                            enemyLevelFrame[lvIndex].enabled = CharacterDatabase.Instance.enemy[idTemp].appearInBook[i];
                            if (enemyData[pointer].type == EnemyType.ZakoBlue) {
                                enemyLevelFill[lvIndex].color = enemyLevelColor[i];
                            } else if (enemyData[pointer].type == EnemyType.ZakoGreen) {
                                enemyLevelFill[lvIndex].color = enemyLevelColor[5 + i];
                            } else {
                                enemyLevelFill[lvIndex].color = enemyLevelColor[i < 4 ? 10 : 11];
                            }
                            enemyLevelFill[lvIndex].enabled = GameManager.Instance.save.defeatEnemy[idTemp * GameManager.enemyLevelMax + i] > 0;
                            lvIndex++;
                        }
                    }
                    textInfo.text = sb.ToString();
                    friendsPivot.localPosition = enemyData[pointer].height * 0.5f * Vector3.down;
                    characterInstance = Instantiate(characterPrefab, friendsPivot);
                    eBase = characterInstance.GetComponent<EnemyBase>();
                    eBase.SetForItem();
                    anim = characterInstance.GetComponent<Animator>();

                    enemyLevelNow = enemyLevelMem;
                    if (GameManager.Instance.save.defeatEnemy[idTemp * GameManager.enemyLevelMax + enemyLevelNow] <= 0) {
                        if (enemyLevelNow > 0) {
                            for (int i = enemyLevelNow - 1; i >= 0; i--) {
                                if (GameManager.Instance.save.defeatEnemy[idTemp * GameManager.enemyLevelMax + i] > 0) {
                                    enemyLevelNow = i;
                                    break;
                                }
                            }
                        }
                        if (enemyLevelNow < GameManager.enemyLevelMax - 1 && enemyLevelNow == enemyLevelMem) {
                            for (int i = enemyLevelNow + 1; i < GameManager.enemyLevelMax; i++) {
                                if (GameManager.Instance.save.defeatEnemy[idTemp * GameManager.enemyLevelMax + i] > 0) {
                                    enemyLevelNow = i;
                                    break;
                                }
                            }
                        }
                    }
                    SetEnemyLevel(pointer);
                    ReferGameObjects refObjs = characterInstance.GetComponent<ReferGameObjects>();
                    if (refObjs) {
                        for (int i = 0; i < refObjs.gameObjects.Length; i++) {
                            if (refObjs.gameObjects[i]) {
                                refObjs.gameObjects[i].layer = friendsLayerObj.layer;
                            }
                        }
                    } else {
                        MeshRenderer[] meshes = characterInstance.GetComponentsInChildren<MeshRenderer>();
                        LineRenderer[] lines = characterInstance.GetComponentsInChildren<LineRenderer>();
                        if (meshes.Length > 0) {
                            for (int i = 0; i < meshes.Length; i++) {
                                meshes[i].gameObject.layer = friendsLayerObj.layer;
                            }
                        }
                        if (lines.Length > 0) {
                            for (int i = 0; i < lines.Length; i++) {
                                lines[i].gameObject.layer = friendsLayerObj.layer;
                            }
                        }
                    }
                    detailMotionType = detailMotionMem;
                    for (int i = 0; i < detailMotionMax; i++) {
                        if ((enemyData[pointer].motionFlags & (1L << detailMotionType)) != 0L) {
                            SetMotion_Start();
                            break;
                        } else {
                            detailMotionType--;
                            if (detailMotionType < 0) {
                                detailMotionType = detailMotionMax - 1;
                            }
                        }
                    }
                    if (cameraObj) {
                        cameraObj.transform.localPosition = enemyData[pointer].radius * 4.444444f * Vector3.forward;
                    }
                } else {
                    textName.text = TextManager.Get("DIC_LOCK");
                    textType.text = "";
                    textAlias.text = "";
                    textInfo.text = "";
                    SetTalkBack(enemyMessageColor[1]);
                    for (int i = 0; i < enemyLevelFill.Length; i++) {
                        enemyLevelFrame[i].enabled = false;
                        enemyLevelFill[i].enabled = false;
                    }
                }
                Vector3 posTemp = friendsCenter.localPosition;
                posTemp.x /= cameraMoveRange.x;
                posTemp.y /= cameraMoveRange.y;
                posTemp.z /= cameraMoveRange.z;
                cameraMoveSpeed = cameraMoveRange.x = enemyData[pointer].radius;
                cameraMoveRange.y = enemyData[pointer].height;
                cameraMoveRange.z = enemyData[pointer].radius * (2f + enemyData[pointer].cameraScalePlus);
                friendsCenter.localPosition = Vector3.Scale(posTemp, cameraMoveRange);
            }
        }
    }

    private void SetFacility(int pointer) {
        if (characterInstance) {
            Destroy(characterInstance);
        }
        if (pointer >= 0 && pointer < facilityData.Length) {
            GameObject characterPrefab = GetFacilityObj(pointer);
            if (characterPrefab) {
                if (CheckFacilityCondition(pointer)) {
                    textName.text = TextManager.Get(string.Format("DIC_FACILITY_NAME_{0:D2}", pointer));
                    textType.text = "";
                    textAlias.text = "";
                    textInfo.text = TextManager.Get(string.Format("DIC_FACILITY_INFO_{0:D2}", pointer));
                    friendsPivot.localPosition = facilityData[pointer].height * 0.5f * Vector3.down;
                    characterInstance = Instantiate(characterPrefab, friendsPivot);
                    if (facilityData[pointer].type == FacilityType.Enemy) {
                        characterInstance.GetComponent<CharacterBase>().SetForItem();
                    }
                    ReferGameObjects refObjs = characterInstance.GetComponent<ReferGameObjects>();
                    if (refObjs) {
                        for (int i = 0; i < refObjs.gameObjects.Length; i++) {
                            if (refObjs.gameObjects[i]) {
                                refObjs.gameObjects[i].layer = friendsLayerObj.layer;
                            }
                        }
                    }
                    NavMeshPrefabInstance navMesh = characterInstance.GetComponent<NavMeshPrefabInstance>();
                    if (navMesh) {
                        navMesh.enabled = false;
                    }
                    cameraObj.transform.localPosition = facilityData[pointer].radius * 2.833333f * Vector3.forward;
                    facilityCheckImage.sprite = facilityCheckSprite[CharacterDatabase.Instance.CheckFacilityEnabled(facilityData[pointer].id) ? 0 : 1];
                    facilityCheckImage.enabled = true;
                    facilityTextChange[0].text = TextManager.Get("DIC_FACILITY_CHANGE_0");
                    if (GameManager.Instance.save.config[GameManager.Save.configID_GenerateHomeObjects] != 0) {
                        facilityTextChange[1].text = TextManager.Get("DIC_FACILITY_CHANGE_1");
                    } else {
                        facilityTextChange[1].text = sb.Clear().Append(TextManager.Get("DIC_FACILITY_CHANGE_1")).AppendLine().Append(TextManager.Get("DIC_FACILITY_CHANGE_2")).ToString();
                    }
                    facilityTextChange[0].enabled = true;
                    facilityTextChange[1].enabled = false;
                } else {
                    textName.text = TextManager.Get("DIC_LOCK");
                    textType.text = "";
                    textAlias.text = "";
                    textInfo.text = "";
                    facilityCheckImage.enabled = false;
                    facilityTextChange[0].enabled = false;
                    facilityTextChange[1].enabled = false;
                }
                Vector3 posTemp = friendsCenter.localPosition;
                posTemp.x /= cameraMoveRange.x;
                posTemp.y /= cameraMoveRange.y;
                posTemp.z /= cameraMoveRange.z;
                cameraMoveSpeed = cameraMoveRange.x = facilityData[pointer].radius;
                cameraMoveRange.y = facilityData[pointer].height;
                cameraMoveRange.z = facilityData[pointer].radius * (2f + facilityData[pointer].cameraScalePlus);
                friendsCenter.localPosition = Vector3.Scale(posTemp, cameraMoveRange);
            }
        }
    }

    private void SetItem(int pointer) {
        if (characterInstance) {
            Destroy(characterInstance);
        }
        if (pointer >= 0 && pointer < itemData.Length) {
            GameObject characterPrefab = ItemDatabase.Instance.GetItemPrefab(itemData[pointer].id);
            if (characterPrefab) {
                if (CheckItemCondition(pointer)) {
                    textName.text = TextManager.Get(string.Format("ITEM_NAME_{0:D3}", itemData[pointer].id));
                    textType.text = "";
                    textAlias.text = "";
                    textInfo.text = TextManager.Get(string.Format("DIC_ITEM_INFO_{0:D3}", itemData[pointer].id));
                    if (itemData[pointer].dropEnemy.Length > 0) {
                        sb.Clear().Append(TextManager.Get("DIC_ITEM_DROP"));
                        for (int i = 0; i < itemData[pointer].dropEnemy.Length; i++) {
                            int idTemp = itemData[pointer].dropEnemy[i].id;
                            if (idTemp >= 0 && idTemp < GameManager.enemyMax) {
                                sb.AppendLine();
                                EnemyType typeTemp = EnemyType.Boss;
                                int levelMin = itemData[pointer].dropEnemy[i].levelMin;
                                int levelMax = itemData[pointer].dropEnemy[i].levelMax;
                                bool defeatedFlag = false;
                                if (levelMin >= 0) {
                                    for (int j = 0; j < enemyData.Length; j++) {
                                        if (enemyData[j].id == idTemp) {
                                            typeTemp = enemyData[j].type;
                                            break;
                                        }
                                    }
                                    for (int j = levelMin; j <= levelMax; j++) {
                                        if (GameManager.Instance.save.defeatEnemy[idTemp * GameManager.enemyLevelMax + j] > 0) {
                                            defeatedFlag = true;
                                            break;
                                        }
                                    }
                                } else {
                                    defeatedFlag = (GameManager.Instance.save.defeatEnemy[idTemp * GameManager.enemyLevelMax] > 0);
                                }

                                if (defeatedFlag) {
                                    if (typeTemp != EnemyType.Boss) {
                                        sb.Append(enemyColorTags[(typeTemp == EnemyType.ZakoGreen ? 5 : 0) + Mathf.Clamp(levelMin, 0, 4)]);
                                    }
                                    sb.Append(TextManager.Get(string.Format("CELLIEN_NAME_{0:D2}", idTemp)));
                                    if (typeTemp != EnemyType.Boss) {
                                        sb.Append("(").Append(TextManager.Get("ANALYZE_LEVEL"));
                                        if (GameManager.Instance.save.language == GameManager.languageEnglish) {
                                            sb.Append(" ");
                                        }
                                        sb.Append(levelMin);
                                        if (levelMin < levelMax) {
                                            sb.Append("-");
                                            sb.Append(levelMax);
                                        }
                                        sb.Append(")");
                                        sb.Append(colorTagEnd);
                                    }
                                } else {
                                    sb.Append(TextManager.Get("DIC_LOCK"));
                                }
                            }
                        }
                        itemDropText.text = sb.ToString();
                    } else {
                        itemDropText.text = "";
                    }

                    friendsPivot.localPosition = itemData[pointer].height * 0.5f * Vector3.down;
                    characterInstance = Instantiate(characterPrefab, friendsPivot);

                    GetItem getItemTemp = characterInstance.GetComponent<GetItem>();
                    if (getItemTemp) {
                        getItemTemp.enabled = false;
                    }
                    Rigidbody rigidbodyTemp = characterInstance.GetComponent<Rigidbody>();
                    if (rigidbodyTemp) {
                        rigidbodyTemp.useGravity = false;
                        rigidbodyTemp.isKinematic = true;
                    }
                    ReferGameObjects refObjs = characterInstance.GetComponent<ReferGameObjects>();
                    if (refObjs) {
                        for (int i = 0; i < refObjs.gameObjects.Length; i++) {
                            if (refObjs.gameObjects[i]) {
                                refObjs.gameObjects[i].layer = friendsLayerObj.layer;
                                AutoRotation autoRotation = refObjs.gameObjects[i].GetComponent<AutoRotation>();
                                if (autoRotation) {
                                    autoRotation.always = true;
                                    autoRotation.enabled = false;
                                }
                            }
                        }
                    }                    
                    cameraObj.transform.localPosition = itemData[pointer].radius * 2.833333f * Vector3.forward;
                } else {
                    textName.text = TextManager.Get("DIC_LOCK");
                    textType.text = "";
                    textAlias.text = "";
                    textInfo.text = "";
                    itemDropText.text = "";
                }
                Vector3 posTemp = friendsCenter.localPosition;
                posTemp.x /= cameraMoveRange.x;
                posTemp.y /= cameraMoveRange.y;
                posTemp.z /= cameraMoveRange.z;
                cameraMoveSpeed = cameraMoveRange.x = itemData[pointer].radius;
                cameraMoveRange.y = itemData[pointer].height;
                cameraMoveRange.z = itemData[pointer].radius * 2f;
                friendsCenter.localPosition = Vector3.Scale(posTemp, cameraMoveRange);
            }
        }
    }

    private void SetTalkBack(MessageBackColor messageBackColor) {
        if (MessageUI.Instance && messageBackColor) {
            talkBackLeft.sprite = MessageUI.Instance.twoToneLeft[Mathf.Clamp(messageBackColor.twoToneType, 0, MessageUI.Instance.twoToneLeft.Length - 1)];
            talkBackRight.sprite = MessageUI.Instance.twoToneRight[Mathf.Clamp(messageBackColor.twoToneType, 0, MessageUI.Instance.twoToneRight.Length - 1)];
            Color colorTemp1 = messageBackColor.color1;
            Color colorTemp2 = messageBackColor.color2;
            colorTemp1.a = 0.5f;
            colorTemp2.a = 0.5f;
            talkBackLeft.color = colorTemp1;
            talkBackRight.color = colorTemp2;
        }
    }

    private GameObject GetCharacterObj(int pointer) {
        if (pointer >= 0 && pointer < friendsData.Length) {
            if (friendsData[pointer].isNPC) {
                return CharacterDatabase.Instance.GetNPC(friendsData[pointer].characterIndex);
            } else {
                return CharacterDatabase.Instance.GetFriends(friendsData[pointer].characterIndex);
            }
        }
        return null;
    }

    private GameObject GetFacilityObj(int pointer) {
        if (pointer >= 0 && pointer < facilityData.Length) {
            if (facilityData[pointer].type == FacilityType.Enemy) {
                return CharacterDatabase.Instance.GetEnemy(dummyEnemyID);
            } else {
                return ItemDatabase.Instance.GetItemPrefab(ItemDatabase.facilityBottom + facilityData[pointer].id);
                // return CharacterDatabase.Instance.GetFacility(facilityData[pointer].id);
            }
        }
        return null;
    }

    private bool CheckFriendsCondition(int pointer) {
        if (pointer >= 0 && pointer < friendsData.Length) {
            if (friendsData[pointer].afterComplete) {
                return GameManager.Instance.GetPerfectCompleted();
            } else if (friendsData[pointer].afterEnd) {
                return GameManager.Instance.save.progress >= GameManager.gameClearedProgress;
            } else if (friendsData[pointer].isPlayer) {
                return true;
            } else if (friendsData[pointer].isLuckyBeast) {
                return GameManager.Instance.save.GotLuckyBeastCount >= GameManager.luckyBeastMax && GameManager.Instance.GetSecret(GameManager.SecretType.SingularityLB);
            } else if (friendsData[pointer].isNPC) {
                if (friendsData[pointer].characterIndex - 1 < GameManager.inventoryNFSMax) {
                    return GameManager.Instance.save.inventoryNFS[Mathf.Clamp(friendsData[pointer].characterIndex - 1, 0, GameManager.inventoryNFSMax - 1)] != 0;
                } else {
                    return true;
                }
            } else {
                if (friendsData[pointer].characterIndex < GameManager.friendsMax) {
                    return GameManager.Instance.save.friends[friendsData[pointer].characterIndex] != 0;
                } else {
                    return true;
                }
            }
        }
        return false;
    }

    private bool CheckEnemyCondition(int pointer) {
        if (pointer >= 0 && pointer < enemyData.Length) {
            for (int i = 0; i < GameManager.enemyLevelMax; i++) {
                if (CharacterDatabase.Instance.enemy[enemyData[pointer].id].statusExist[i] && GameManager.Instance.save.defeatEnemy[enemyData[pointer].id * GameManager.enemyLevelMax + i] > 0) {
                    return true;
                }
            }
        }
        return false;
    }

    private bool CheckFacilityCondition(int pointer) {
        if (pointer >= 0 && pointer < facilityData.Length) {
            bool checker = true;
            switch (facilityData[pointer].type) {
                case FacilityType.Friends:
                case FacilityType.Enemy:
                    for (int i = 0; i < facilityData[pointer].requireFriends.Length; i++) {
                        if (GameManager.Instance.save.friends[facilityData[pointer].requireFriends[i]] == 0) {
                            checker = false;
                            break;
                        }
                    }
                    break;
                case FacilityType.Lucky:
                    if (!GameManager.Instance.GetSecret(GameManager.SecretType.SingularityLB)) {
                        checker = false;
                    }
                    break;
                case FacilityType.Minmi:
                    if (!GameManager.Instance.GetMinmiCompleted()) {
                        checker = false;
                    }
                    break;
                case FacilityType.Complete:
                    if (!GameManager.Instance.GetPerfectCompleted()) {
                        checker = false;
                    }
                    break;
            }
            return checker;
        }
        return false;
    }

    private bool CheckItemCondition(int pointer) {
        if (pointer >= 0 && pointer < itemData.Length) {
            int arrayNum = itemData[pointer].id / 32;
            int bitNum = 1 << (itemData[pointer].id % 32);
            if (GameManager.Instance.save.itemExperience.Length > arrayNum && (GameManager.Instance.save.itemExperience[arrayNum] & bitNum) != 0) {
                return true;
            }
        }
        return false;
    }

    void SetFriendsIndex() {
        int pointerTemp = listOrigin;
        for (int i = 0; i < indexNames.Length; i++) {
            bool condition = CheckFriendsCondition(pointerTemp);
            bool iconFlag = condition || !friendsData[pointerTemp].afterEnd;
            if (condition) {
                indexNames[i].text = TextManager.Get(string.Format("ITEM_NAME_{0:D3}", friendsData[pointerTemp].nameID));
                indexIcons[i].material = iconShowMaterial;
            } else {
                indexNames[i].text = TextManager.Get("DIC_LOCK");
                indexIcons[i].material = iconHideMaterial;
            }
            indexIcons[i].sprite = ItemDatabase.Instance.itemList.Find(n => n.id == friendsData[pointerTemp].nameID).image;
            if (indexIcons[i].enabled != iconFlag) {
                indexIcons[i].enabled = iconFlag;
            }
            pointerTemp++;
            if (pointerTemp >= friendsData.Length) {
                pointerTemp = 0;
            }
        }
        SetFriendsChoosingIcon();
    }

    void SetFriendsChoosingIcon() {
        if (CheckFriendsCondition(pointer)) {
            choosingIconFace.sprite = indexIcons[cursorPos].sprite;
            choosingIconFace.enabled = true;
        } else {
            choosingIconFace.enabled = false;
        };
    }

    void SetEnemyIndex() {
        int pointerTemp = listOrigin;
        for (int i = 0; i < indexNames.Length; i++) {
            bool condition = CheckEnemyCondition(pointerTemp);
            bool iconFlag = condition || !enemyData[pointerTemp].secret;
            if (condition) {
                indexNames[i].text = TextManager.Get(string.Format("CELLIEN_NAME_{0:D2}", enemyData[pointerTemp].id));
                indexIcons[i].material = iconShowMaterial;
            } else {
                indexNames[i].text = TextManager.Get("DIC_LOCK");
                indexIcons[i].material = iconHideMaterial;
            }
            indexIcons[i].sprite = CharacterDatabase.Instance.GetEnemySprite(enemyData[pointerTemp].id);
            if (indexIcons[i].enabled != iconFlag) {
                indexIcons[i].enabled = iconFlag;
            }
            pointerTemp++;
            if (pointerTemp >= enemyData.Length) {
                pointerTemp = 0;
            }
        }
        SetEnemyChoosingIcon();
    }

    void SetEnemyChoosingIcon() {
        if (CheckEnemyCondition(pointer)) {
            choosingIconFace.sprite = indexIcons[cursorPos].sprite;
            choosingIconFace.enabled = true;
        } else {
            choosingIconFace.enabled = false;
        }
    }

    void SetFacilityIndex() {
        int pointerTemp = listOrigin;
        for (int i = 0; i < indexNames.Length; i++) {
            bool condition = CheckFacilityCondition(pointerTemp);
            bool iconFlag = condition || (facilityData[pointerTemp].type != FacilityType.Complete);
            if (condition) {
                indexNames[i].text = TextManager.Get(string.Format("DIC_FACILITY_NAME_{0:D2}", pointerTemp));
                indexIcons[i].material = iconShowMaterial;
            } else {
                indexNames[i].text = TextManager.Get("DIC_LOCK");
                indexIcons[i].material = iconHideMaterial;
            }
            // indexIcons[i].sprite = CharacterDatabase.Instance.GetFacilitySprite(facilityData[pointerTemp].id);
            indexIcons[i].sprite = ItemDatabase.Instance.GetItemImage(ItemDatabase.facilityBottom + facilityData[pointerTemp].id);
            if (indexIcons[i].enabled != iconFlag) {
                indexIcons[i].enabled = iconFlag;
            }
            pointerTemp++;
            if (pointerTemp >= facilityData.Length) {
                pointerTemp = 0;
            }
        }
        SetFacilityChoosingIcon();
    }

    void SetFacilityChoosingIcon() {
        if (CheckFacilityCondition(pointer)) {
            choosingIconFace.sprite = indexIcons[cursorPos].sprite;
            choosingIconFace.enabled = true;
        } else {
            choosingIconFace.enabled = false;
        };
    }

    void SetItemIndex() {
        int pointerTemp = listOrigin;
        for (int i = 0; i < indexNames.Length; i++) {
            bool condition = CheckItemCondition(pointerTemp);
            bool iconFlag = condition;
            if (condition) {
                indexNames[i].text = TextManager.Get(string.Format("ITEM_NAME_{0:D3}", itemData[pointerTemp].id));
                indexIcons[i].material = iconShowMaterial;
            } else {
                indexNames[i].text = TextManager.Get("DIC_LOCK");
                indexIcons[i].material = iconHideMaterial;
            }
            indexIcons[i].sprite = ItemDatabase.Instance.GetItemImage(itemData[pointerTemp].id);
            if (indexIcons[i].enabled != iconFlag) {
                indexIcons[i].enabled = iconFlag;
            }
            pointerTemp++;
            if (pointerTemp >= itemData.Length) {
                pointerTemp = 0;
            }
        }
        SetItemChoosingIcon();
    }

    void SetItemChoosingIcon() {
        if (CheckItemCondition(pointer)) {
            choosingIconFace.sprite = indexIcons[cursorPos].sprite;
            choosingIconFace.enabled = true;
        } else {
            choosingIconFace.enabled = false;
        };
    }

    private void ChangeCanvas(int canvasType) {
        for (int i = 0; i < childCanvas.Length; i++) {
            childCanvas[i].enabled = (i == canvasType);
        }
        for (int i = 0; i < gridLayoutGroups.Length; i++) {
            gridLayoutGroups[i].enabled = (i == gridLayoutCanvasType);
        }
        for (int i = 0;i < additiveCanvas.Length; i++) {
            additiveCanvas[i].enabled = false;
        }
        for (int i = 0; i < childRaycaster.Length; i++) {
            if (childRaycaster[i]) {
                childRaycaster[i].enabled = (i == canvasType && GameManager.Instance.MouseEnabled);
            }
        }
        additiveCanvasTimeRemain = 0;
    }

    protected override void ConsoleStart() {
        if (GameManager.Instance) {
            switch (choiceAnswer) {
                case choiceType_Friends:
                    dicState = 1;
                    break;
                case choiceType_Enemy:
                    dicState = 11;
                    break;
                case choiceType_Facility:
                    dicState = 21;
                    break;
                case choiceType_Item:
                    dicState = 31;
                    break;
            }
            for (int i = 0; i < eachTypeCanvas.Length; i++) {
                if (eachTypeCanvas[i]) {
                    eachTypeCanvas[i].enabled = (i == choiceAnswer);
                }
            }
            if (choiceAnswer != choiceAnswerPre) {
                pointer = 0;
                listOrigin = 0;
                cursorPos = 0;
                cursor.anchoredPosition = cursorOrigin;
                choiceAnswerPre = choiceAnswer;
            }
            detailCursorPos = 0;
            detailCursor.anchoredPosition = detailCursorOrigin;
            detailMotionMem = detailMotionType = 0;
            detailExpressionMem = detailExpressionType = 0;
            detailCameraType = 0;
            detailSpeedType = 10;
            wildReleaseFlag = false;
            additiveCanvasTimeRemain = 0;
            operationShowed = false;
            cameraMoveSpeed = 0.6f;
            cameraMoveRange.x = 0.6f;
            cameraMoveRange.y = cameraMoveRange.z = 1.2f;
            enemyLevelMem = enemyLevelNow = 1;
            eventEnterNum = -1;
            eventClickNum = -1;
            SetQuitArrows();
            switch (choiceAnswer) {
                case choiceType_Friends:                    
                    SetFriendsIndex();
                    friendsHeader.text = TextManager.Get("CHOICE_DIC_FRIENDS");
                    break;
                case choiceType_Enemy:
                    SetEnemyIndex();
                    friendsHeader.text = TextManager.Get("CHOICE_DIC_ENEMY");
                    break;
                case choiceType_Facility:
                    SetFacilityIndex();
                    SetTalkBack(facilityColor);
                    friendsHeader.text = TextManager.Get("CHOICE_DIC_FACILITY");
                    break;
                case 3:
                    SetItemIndex();
                    SetTalkBack(itemColor);
                    friendsHeader.text = TextManager.Get("CHOICE_DIC_ITEM");
                    break;
            }
            InitCharacter();
            if (parentCanvas) {
                parentCanvas.gameObject.SetActive(true);
                ChangeCanvas(0);
            }
            if (CanvasCulling.Instance) {
                CanvasCulling.Instance.CheckConfig(CanvasCulling.indexGauge, 0);
            }
            if (CameraManager.Instance) {
                CameraManager.Instance.SetEventCamera(CameraManager.Instance.mainCamObj.transform.position, CameraManager.Instance.mainCamObj.transform.eulerAngles, 5, 0f, 1.5f);
            }
            if (balloonInstance) {
                balloonInstance.SetActive(false);
            }
            for (int i = 0; i < additiveCanvas.Length; i++) {
                if (additiveCanvas[i] && additiveCanvas[i].enabled) {
                    additiveCanvas[i].enabled = false;
                }
            }
            ResetCamera();
            SetCameraRect();
            if (choiceAnswer == choiceType_Enemy) {
                cameraFirst.fieldOfView = cameraSecond.fieldOfView = 40f;
            } else {
                cameraFirst.fieldOfView = cameraSecond.fieldOfView = 60f;
            }
            if (facilityEventImage) {
                facilityEventImage.enabled = facilityEventImage.raycastTarget = (choiceAnswer == choiceType_Facility);
            }
            SetPercentageText();
            SwitchPostProcessing(true);
        }
    }

    protected override void ConsoleEnd() {
        if (GameManager.Instance) {
            if (parentCanvas) {
                parentCanvas.gameObject.SetActive(false);
            }
            InitCharacter();
            dicState = 0;
            if (CanvasCulling.Instance) {
                CanvasCulling.Instance.CheckConfig();
            }
            if (balloonInstance) {
                balloonInstance.SetActive(true);
            }
            SwitchPostProcessing(false);
        }
    }

    private void OnDestroy() {
        if (dicState != 0) {
            ConsoleEnd();
        }
    }

    void SetQuitArrows() {
        bool quitArrowFlag = (GameManager.Instance.MouseEnabled && GameManager.Instance.mouseStoppingTime < 2f);
        for (int i = 0; i < quitArrows.Length; i++) {
            if (quitArrows[i].enabled != quitArrowFlag) {
                quitArrows[i].enabled = quitArrowFlag;
                quitArrows[i].raycastTarget = quitArrowFlag;
            }
        }
        bool nextArrowFlag = (quitArrowFlag && dicState < 20);
        if (nextArrow.enabled != nextArrowFlag) {
            nextArrow.enabled = nextArrowFlag;
            nextArrow.raycastTarget = nextArrowFlag;
        }
    }

    protected override void ConsoleUpdate() {
        if (dicState > 0 && GameManager.Instance) {
            mouseScroll = Input.mouseScrollDelta.y;
            if (GameManager.Instance.MouseCancelling) {
                eventEnterNum = -1;
                eventClickNum = -1;
                mouseScroll = 0;
            }
            if (CameraManager.Instance) {
                CameraManager.Instance.SetEventTimer(5f);
            }
            SetQuitArrows();

            move = GameManager.Instance.MoveCursor(true);

            bool considerSubmit = false;
            bool considerCancel = false;
            if ((eventEnterNum >= 100 && eventEnterNum < 100 + indexNames.Length) || (eventClickNum >= 100 && eventClickNum < 100 + indexNames.Length)) {
                int dicType = dicState / 10;
                if (eventClickNum >= 100 && eventClickNum < 100 + indexNames.Length) {
                    UISE.Instance.Play(UISE.SoundName.submit);
                    cursorPos = eventClickNum - 100;
                    considerSubmit = true;
                } else {
                    UISE.Instance.Play(UISE.SoundName.move);
                    cursorPos = eventEnterNum - 100;
                }
                cursor.anchoredPosition = cursorOrigin + cursorInterval * cursorPos;
                switch (dicType) {
                    case 0:
                        pointer = (listOrigin + cursorPos) % friendsData.Length;
                        SetFriendsChoosingIcon();
                        break;
                    case 1:
                        pointer = (listOrigin + cursorPos) % enemyData.Length;
                        SetEnemyChoosingIcon();
                        break;
                    case 2:
                        pointer = (listOrigin + cursorPos) % facilityData.Length;
                        SetFacilityChoosingIcon();
                        break;
                    case 3:
                        pointer = (listOrigin + cursorPos) % itemData.Length;
                        SetItemChoosingIcon();
                        break;
                }
            }
            if (eventClickNum == 200 || eventClickNum == 201 || mouseScroll != 0) {
                UISE.Instance.Play(UISE.SoundName.move);
                int moveTemp = (eventClickNum == 200 || mouseScroll > 0) ? -1 : (eventClickNum == 201 || mouseScroll < 0) ? 1 : 0;
                int dicType = dicState / 10;
                switch (dicType) {
                    case 0:
                        listOrigin = (listOrigin + moveTemp + friendsData.Length) % friendsData.Length;
                        pointer = (listOrigin + cursorPos) % friendsData.Length;
                        SetFriendsIndex();
                        if (dicState == 2) {
                            SetFriends(pointer);
                        }
                        break;
                    case 1:
                        listOrigin = (listOrigin + moveTemp + enemyData.Length) % enemyData.Length;
                        pointer = (listOrigin + cursorPos) % enemyData.Length;
                        SetEnemyIndex();
                        if (dicState == 12) {
                            SetEnemy(pointer);
                        }
                        break;
                    case 2:
                        listOrigin = (listOrigin + moveTemp + facilityData.Length) % facilityData.Length;
                        pointer = (listOrigin + cursorPos) % facilityData.Length;
                        SetFacilityIndex();
                        if (dicState == 22) {
                            SetFacility(pointer);
                        }
                        break;
                    case 3:
                        listOrigin = (listOrigin + moveTemp + itemData.Length) % itemData.Length;
                        pointer = (listOrigin + cursorPos) % itemData.Length;
                        SetItemIndex();
                        if (dicState == 32) {
                            SetItem(pointer);
                        }
                        break;
                }
            } else if (eventClickNum == 202) {
                move.x = -1;
            } else if (eventClickNum == 203) {
                move.x = 1;
            } else if (eventClickNum == 204) {
                considerSubmit = true;
            } else if (eventClickNum == 205) {
                considerCancel = true;
            }

            if (eventClickNum >= 300 && eventClickNum < 399) {
                UISE.Instance.Play(UISE.SoundName.move);
                detailCursorPos = eventClickNum / 10 % 10;
                detailCursor.anchoredPosition = detailCursorOrigin + detailCursorInterval * detailCursorPos;
                move.x = (eventClickNum % 10 == 0 ? -1 : 1);
            }

            if (move.y != 0) {
                switch (dicState) {
                    case 1:
                    case 2:
                        UISE.Instance.Play(UISE.SoundName.move);
                        pointer = (pointer + friendsData.Length + move.y) % friendsData.Length;
                        if (move.y < 0 && cursorPos <= 0) {
                            listOrigin = pointer;
                            SetFriendsIndex();
                        } else if (move.y > 0 && cursorPos >= indexNames.Length - 1) {
                            listOrigin = (pointer - (indexNames.Length - 1) + friendsData.Length) % friendsData.Length;
                            SetFriendsIndex();
                        } else {
                            cursorPos = (pointer - listOrigin + friendsData.Length) % friendsData.Length;
                            SetFriendsChoosingIcon();
                        }
                        cursor.anchoredPosition = cursorOrigin + cursorInterval * cursorPos;
                        break;
                    case 3:
                    case 13:
                        UISE.Instance.Play(UISE.SoundName.move);
                        detailCursorPos = (detailCursorPos + move.y + 4) % 4;
                        detailCursor.anchoredPosition = detailCursorOrigin + detailCursorInterval * detailCursorPos;
                        break;
                    case 11:
                    case 12:
                        UISE.Instance.Play(UISE.SoundName.move);
                        pointer = (pointer + enemyData.Length + move.y) % enemyData.Length;
                        if (move.y < 0 && cursorPos <= 0) {
                            listOrigin = pointer;
                            SetEnemyIndex();
                        } else if (move.y > 0 && cursorPos >= indexNames.Length - 1) {
                            listOrigin = (pointer - (indexNames.Length - 1) + enemyData.Length) % enemyData.Length;
                            SetEnemyIndex();
                        } else {
                            cursorPos = (pointer - listOrigin + enemyData.Length) % enemyData.Length;
                            SetEnemyChoosingIcon();
                        }
                        cursor.anchoredPosition = cursorOrigin + cursorInterval * cursorPos;
                        break;
                    case 21:
                    case 22:
                        UISE.Instance.Play(UISE.SoundName.move);
                        pointer = (pointer + facilityData.Length + move.y) % facilityData.Length;
                        if (move.y < 0 && cursorPos <= 0) {
                            listOrigin = pointer;
                            SetFacilityIndex();
                        } else if (move.y > 0 && cursorPos >= indexNames.Length - 1) {
                            listOrigin = (pointer - (indexNames.Length - 1) + facilityData.Length) % facilityData.Length;
                            SetFacilityIndex();
                        } else {
                            cursorPos = (pointer - listOrigin + facilityData.Length) % facilityData.Length;
                            SetFacilityChoosingIcon();
                        }
                        cursor.anchoredPosition = cursorOrigin + cursorInterval * cursorPos;
                        break;
                    case 31:
                    case 32:
                        UISE.Instance.Play(UISE.SoundName.move);
                        pointer = (pointer + itemData.Length + move.y) % itemData.Length;
                        if (move.y < 0 && cursorPos <= 0) {
                            listOrigin = pointer;
                            SetItemIndex();
                        } else if (move.y > 0 && cursorPos >= indexNames.Length - 1) {
                            listOrigin = (pointer - (indexNames.Length - 1) + itemData.Length) % itemData.Length;
                            SetItemIndex();
                        } else {
                            cursorPos = (pointer - listOrigin + itemData.Length) % itemData.Length;
                            SetItemChoosingIcon();
                        }
                        cursor.anchoredPosition = cursorOrigin + cursorInterval * cursorPos;
                        break;
                }
            } else if (move.x != 0) {
                switch (dicState) {
                    case 2:
                    case 12:
                    case 22:
                    case 32:
                        UISE.Instance.Play(UISE.SoundName.move);
                        detailCameraType = (detailCameraType + move.x + 4) % 4;
                        additiveTextCameraCaption.text = TextManager.Get("DIC_CAMERA_CAPTION");
                        additiveTextCameraType.text = TextManager.Get(string.Format("DIC_CAMERA_{0}", detailCameraType));
                        additiveCanvas[0].enabled = true;
                        additiveCanvasTimeRemain = 3f;
                        if (additiveCanvas[1].enabled) {
                            additiveCanvas[1].enabled = false;
                        }
                        break;
                    case 3:
                    case 13:
                        UISE.Instance.Play(UISE.SoundName.move);
                        if (detailCursorPos == 0) {
                            detailCameraType = (detailCameraType + move.x + 4) % 4;
                            detailTextCameraType.text = TextManager.Get(string.Format("DIC_CAMERA_{0}", detailCameraType));
                        } else if (detailCursorPos == 1) {
                            int motionTemp = detailMotionType;
                            long motionFlags = (choiceAnswer == choiceType_Enemy ? enemyData[pointer].motionFlags : friendsData[pointer].motionFlags);
                            for (int i = 0; i < detailMotionMax; i++) {
                                motionTemp += move.x;
                                if (motionTemp < 0) {
                                    motionTemp = detailMotionMax - 1;
                                } else if (motionTemp >= detailMotionMax) {
                                    motionTemp = 0;
                                }
                                if ((motionFlags & (1L << motionTemp)) != 0L) {
                                    detailMotionType = detailMotionMem = motionTemp;
                                    detailTextMotionType.text = TextManager.Get(string.Format("DIC_MOTION_{0:D2}", detailMotionType));
                                    break;
                                }
                            }
                            SetMotion_Start();
                        } else if (detailCursorPos == 2) {
                            if (choiceAnswer == choiceType_Enemy) {
                                for (int i = 1; i < GameManager.enemyLevelMax; i++) {
                                    int levelTemp = (enemyLevelNow + i * move.x + GameManager.enemyLevelMax) % GameManager.enemyLevelMax;
                                    if (GameManager.Instance.save.defeatEnemy[enemyData[pointer].id * GameManager.enemyLevelMax + levelTemp] > 0) {
                                        enemyLevelMem = enemyLevelNow = levelTemp;
                                        SetEnemyLevel(pointer);
                                        break;
                                    }
                                }
                                if (enemyData[pointer].type == EnemyType.Boss) {
                                    detailTextExpressionType.text = TextManager.Get(string.Format("ANALYZE_BOSSLEVEL_{0}", enemyLevelNow));
                                } else {
                                    detailTextExpressionType.text = enemyLevelNow.ToString();
                                }
                            } else {
                                int expressionTemp = detailExpressionType;
                                for (int i = 0; i < detailExpressionMax; i++) {
                                    expressionTemp += move.x;
                                    if (expressionTemp < 0) {
                                        expressionTemp = detailExpressionMax - 1;
                                    } else if (expressionTemp >= detailExpressionMax) {
                                        expressionTemp = 0;
                                    }
                                    if ((friendsData[pointer].expressionFlags & (1 << expressionTemp)) != 0) {
                                        detailExpressionType = detailExpressionMem = expressionTemp;
                                        detailTextExpressionType.text = TextManager.Get(string.Format("DIC_EXPRESSION_{0:D2}", detailExpressionType));
                                        break;
                                    }
                                }
                                SetExpression();
                            }
                        } else if (detailCursorPos == 3) {
                            detailSpeedType = (detailSpeedType + move.x + detailSpeedMax) % detailSpeedMax;
                            detailTextSpeedType.text = string.Format("{0:F1}", detailSpeedType * 0.1f);
                            if (anim) {
                                anim.speed = detailSpeedType * 0.1f;
                            }
                        }
                        break;
                }
            }
            switch (dicState) {
                case 1:
                    if (GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit) || considerSubmit) {
                        UISE.Instance.Play(UISE.SoundName.submit);
                        ChangeCanvas(1);
                        SetFriends(pointer);
                        SetCameraRect();
                        if (!operationShowed) {
                            operationShowed = true;
                            additiveCanvasTimeRemain = 5f;
                            friendsOperationTextSubmit.text = TextManager.Get("DIC_OPERATION_SUBMIT");
                            friendsOperationTextWildRelease.text = GetEnableWildRelease() ? TextManager.Get("DIC_OPERATION_WILDRELEASE") : "";
                            additiveCanvas[1].enabled = true;
                        }
                        dicState = 2;
                    } else if (GameManager.Instance.GetCancelButtonDown || considerCancel) {
                        UISE.Instance.Play(UISE.SoundName.cancel);
                        PauseController.Instance.PauseDictionary(false);
                        dicState = 0;
                    }
                    break;
                case 2:
                    if (move.y != 0) {
                        SetFriends(pointer);
                    } else if (GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit) || considerSubmit) {
                        UISE.Instance.Play(UISE.SoundName.submit);
                        SetDetailCanvas();
                        ChangeCanvas(2);
                        SetCameraRect(1.5f);
                        if (additiveCanvas[1].enabled) {
                            additiveCanvas[1].enabled = false;
                            additiveCanvasTimeRemain = 0f;
                        }
                        dicState = 3;
                    } else if (GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Cancel) || considerCancel) {
                        UISE.Instance.Play(UISE.SoundName.cancel);
                        ChangeCanvas(0);
                        SetFriends(-1);
                        dicState = 1;
                    } else {
                        MoveCamera();
                    }
                    SetMotion_Update();
                    break;
                case 3:
                    if (GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit) || considerSubmit) {
                        UISE.Instance.Play(UISE.SoundName.submit);
                        ChangeCanvas(1);
                        SetCameraRect();
                        dicState = 2;
                    } else if (GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Cancel) || considerCancel) {
                        UISE.Instance.Play(UISE.SoundName.cancel);
                        ChangeCanvas(1);
                        SetCameraRect();
                        dicState = 2;
                    } else {
                        MoveCamera();
                    }
                    SetMotion_Update();
                    break;
                case 11:
                    if (GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit) || considerSubmit) {
                        UISE.Instance.Play(UISE.SoundName.submit);
                        ChangeCanvas(1);
                        SetEnemy(pointer);
                        SetCameraRect();
                        if (!operationShowed) {
                            operationShowed = true;
                            additiveCanvasTimeRemain = 5f;
                            friendsOperationTextSubmit.text = TextManager.Get("DIC_OPERATION_SUBMIT");
                            friendsOperationTextWildRelease.text = "";
                            additiveCanvas[1].enabled = true;
                        }
                        dicState = 12;
                    } else if (GameManager.Instance.GetCancelButtonDown || considerCancel) {
                        UISE.Instance.Play(UISE.SoundName.cancel);
                        PauseController.Instance.PauseDictionary(false);
                        dicState = 0;
                    }
                    break;
                case 12:
                    if (move.y != 0) {
                        SetEnemy(pointer);
                    } else if (GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit) || considerSubmit) {
                        UISE.Instance.Play(UISE.SoundName.submit);
                        SetDetailCanvas();
                        ChangeCanvas(2);
                        SetCameraRect(1.5f);
                        if (additiveCanvas[1].enabled) {
                            additiveCanvas[1].enabled = false;
                            additiveCanvasTimeRemain = 0f;
                        }
                        dicState = 13;
                    } else if (GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Cancel) || considerCancel) {
                        UISE.Instance.Play(UISE.SoundName.cancel);
                        ChangeCanvas(0);
                        SetEnemy(-1);
                        dicState = 11;
                    } else {
                        MoveCamera();
                    }
                    SetMotion_Update();
                    break;
                case 13:
                    if (GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit) || considerSubmit) {
                        UISE.Instance.Play(UISE.SoundName.submit);
                        ChangeCanvas(1);
                        SetCameraRect();
                        dicState = 12;
                    } else if (GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Cancel) || considerCancel) {
                        UISE.Instance.Play(UISE.SoundName.cancel);
                        ChangeCanvas(1);
                        SetCameraRect();
                        dicState = 12;
                    } else {
                        MoveCamera();
                    }
                    SetMotion_Update();
                    break;
                case 21:
                    if (GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit) || considerSubmit) {
                        UISE.Instance.Play(UISE.SoundName.submit);
                        ChangeCanvas(1);
                        SetFacility(pointer);
                        SetCameraRect();
                        dicState = 22;
                    } else if (GameManager.Instance.GetCancelButtonDown || considerCancel) {
                        UISE.Instance.Play(UISE.SoundName.cancel);
                        PauseController.Instance.PauseDictionary(false);
                        dicState = 0;
                    }
                    break;
                case 22:
                    if (move.y != 0) {
                        SetFacility(pointer);
                    } else if (GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit) || considerSubmit) {
                        if (characterInstance) {
                            UISE.Instance.Play(UISE.SoundName.use);
                            // int referBit = CharacterDatabase.Instance.facility[pointer].referBit;
                            int referBit = ItemDatabase.Instance.GetItemPrice(ItemDatabase.facilityBottom + facilityData[pointer].id);
                            CharacterDatabase.Instance.SwitchFacilityEnabled(facilityData[pointer].id);
                            facilityCheckImage.sprite = facilityCheckSprite[CharacterDatabase.Instance.CheckFacilityEnabled(facilityData[pointer].id) ? 0 : 1];
                            facilityTextChange[1].enabled = true;
                        }
                    } else if (GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Cancel) || considerCancel) {
                        UISE.Instance.Play(UISE.SoundName.cancel);
                        ChangeCanvas(0);
                        SetFacility(-1);
                        dicState = 21;
                    } else {
                        MoveCamera();
                    }
                    break;
                case 31:
                    if (GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit) || considerSubmit) {
                        UISE.Instance.Play(UISE.SoundName.submit);
                        ChangeCanvas(1);
                        SetItem(pointer);
                        SetCameraRect();
                        dicState = 32;
                    } else if (GameManager.Instance.GetCancelButtonDown || considerCancel) {
                        UISE.Instance.Play(UISE.SoundName.cancel);
                        PauseController.Instance.PauseDictionary(false);
                        dicState = 0;
                    }
                    break;
                case 32:
                    if (move.y != 0) {
                        SetItem(pointer);
                    } else if (GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Cancel) || considerCancel) {
                        UISE.Instance.Play(UISE.SoundName.cancel);
                        ChangeCanvas(0);
                        SetItem(-1);
                        dicState = 31;
                    } else {
                        MoveCamera();
                    }
                    break;
            }
            if (additiveCanvasTimeRemain > 0f) {
                additiveCanvasTimeRemain -= Time.unscaledDeltaTime;
                if (additiveCanvasTimeRemain <= 0f) {
                    for (int i = 0; i < additiveCanvas.Length; i++) {
                        additiveCanvas[i].enabled = false;
                    }
                }
            }
            if (wildReleaseTime > 0f) {
                wildReleaseTime -= Time.unscaledDeltaTime;
            }
            if (wildReleaseTime <= 0f && (dicState == 2 || dicState == 3) && GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Wild_Release)) {
                wildReleaseTime = 0.3f;
                wildReleaseFlag = !wildReleaseFlag;
                if (GetEnableWildRelease()) {
                    if (wildReleaseFlag) {
                        Instantiate(wildReleaseStartPrefab, transform.position, Quaternion.identity);
                        fBase.SupermanStart(false);
                    } else {
                        fBase.SupermanEnd(false);
                        Instantiate(wildReleaseEndPrefab, transform.position, Quaternion.identity);
                    }
                }
            }
            eventEnterNum = -1;
            eventClickNum = -1;
            mouseScroll = 0;
        }
    }

    private bool GetEnableWildRelease() {
        return friendsData[pointer].isNPC == false && fBase && (GameManager.Instance.save.weapon[GameManager.friendsCostInfinityIndex] != 0 || (GameManager.Instance.save.weapon[GameManager.friendsCostMaxUpIndex] != 0 && friendsData[pointer].isPlayer));
    }

    private void SwitchPostProcessing(bool toFriendsCamera) {
        if (GameManager.Instance && InstantiatePostProcessingProfile.Instance && CameraManager.Instance && CameraManager.Instance.mainCamObj) {
            if (toFriendsCamera) {
                configSave[0] = GameManager.Instance.save.config[GameManager.Save.configID_DepthOfField];
                configSave[1] = GameManager.Instance.save.config[GameManager.Save.configID_Bloom];
                GameManager.Instance.save.config[GameManager.Save.configID_DepthOfField] = 0;
                GameManager.Instance.save.config[GameManager.Save.configID_Bloom] = 0;
                InstantiatePostProcessingProfile.Instance.QualitySettingsAdjustments();
                CameraManager.Instance.mainCamObj.GetComponent<PostProcessingBehaviour>().profile = ppProfileMainBlur;
                for (int i = 0; i < ppBehaviourFriends.Length; i++) {
                    ppBehaviourFriends[i].profile = InstantiatePostProcessingProfile.Instance.GetProfile();
                    ppBehaviourFriends[i].enabled = true;
                }
                cameraObj.SetActive(true);
            } else {
                cameraObj.SetActive(false);
                for (int i = 0; i < ppBehaviourFriends.Length; i++) {
                    ppBehaviourFriends[i].profile = null;
                    ppBehaviourFriends[i].enabled = false;
                }
                CameraManager.Instance.mainCamObj.GetComponent<PostProcessingBehaviour>().profile = InstantiatePostProcessingProfile.Instance.GetProfile();
                GameManager.Instance.save.config[GameManager.Save.configID_DepthOfField] = configSave[0];
                GameManager.Instance.save.config[GameManager.Save.configID_Bloom] = configSave[1];
                InstantiatePostProcessingProfile.Instance.QualitySettingsAdjustments();
            }
        }
    }

    private void SetCameraRect(float multiplier = 1f) {
        float screenRate = (float)Screen.width / Screen.height;
        float rectWidth = 0.5f;
        if (screenRate >= 16f / 9f) {
            rectWidth = 0.5f;
        } else if (screenRate <= 4f / 3f) {
            rectWidth = 0.4f;
        } else {
            rectWidth = Mathf.Clamp(0.5f - (16f / 9f - screenRate) * (9f / 40f), 0.4f, 0.5f);
        }
        if (multiplier != 1f) {
            rectWidth *= multiplier;
        }
        cameraFirst.rect = new Rect(0, 0, rectWidth, 1);
        cameraSecond.rect = new Rect(rectWidth, 0, 1f - rectWidth, 1);
    }

    private void SetDetailCanvas() {
        if (choiceAnswer == choiceType_Enemy) {
            if (CheckEnemyCondition(pointer)) {
                detailTextName.text = TextManager.Get(string.Format("CELLIEN_NAME_{0:D2}", enemyData[pointer].id));
            } else {
                detailTextName.text = TextManager.Get("DIC_LOCK");
            }
            detailTextExpressionCaption.text = TextManager.Get("WORD_LEVEL");
            if (enemyData[pointer].type == EnemyType.Boss) {
                detailTextExpressionType.text = TextManager.Get(string.Format("ANALYZE_BOSSLEVEL_{0}", enemyLevelNow));
            } else {
                detailTextExpressionType.text = enemyLevelNow.ToString();
            }
        } else {
            if (CheckFriendsCondition(pointer)) {
                detailTextName.text = TextManager.Get(string.Format("ITEM_NAME_{0:D3}", friendsData[pointer].nameID));
            } else {
                detailTextName.text = TextManager.Get("DIC_LOCK");
            }
            detailTextExpressionCaption.text = TextManager.Get("DIC_EXPRESSION_CAPTION");
            detailTextExpressionType.text = TextManager.Get(string.Format("DIC_EXPRESSION_{0:D2}", detailExpressionType));
        }
        detailTextCameraCaption.text = TextManager.Get("DIC_CAMERA_CAPTION");
        detailTextCameraType.text = TextManager.Get(string.Format("DIC_CAMERA_{0}", detailCameraType));
        detailTextMotionCaption.text = TextManager.Get("DIC_MOTION_CAPTION");
        detailTextMotionType.text = TextManager.Get(string.Format("DIC_MOTION_{0:D2}", detailMotionType));
        detailTextSpeedCaption.text = TextManager.Get("DIC_SPEED_CAPTION");
        detailTextSpeedType.text = string.Format("{0:F1}", detailSpeedType * 0.1f);
    }

    private void ResetCamera() {
        friendsCenter.localEulerAngles = vecZero;
        friendsCenter.localPosition = vecZero;
    }

    private void MoveCamera() {
        if (playerInput.GetButtonDown(RewiredConsts.Action.Camera_Reset)) {
            ResetCamera();
        } else if (CameraManager.Instance) {
            float deltaTimeCache = Time.deltaTime;
            float horizontal = playerInput.GetAxis(RewiredConsts.Action.Camera_Horizontal);
            float vertical = playerInput.GetAxis(RewiredConsts.Action.Camera_Vertical);
            if (horizontal != 0f) {
                if (playerInput.IsCurrentInputSource(RewiredConsts.Action.Camera_Horizontal, ControllerType.Mouse)) {
                    if (GameManager.Instance.save.config[GameManager.Save.configID_CameraControlButton] != 0 && !playerInput.GetButton(RewiredConsts.Action.Camera_Control)) {
                        horizontal = 0f;
                    } else {
                        horizontal *= 0.2f;
                        horizontal *= CameraManager.Instance.MouseWheelBias();
                    }
                }
            }
            if (vertical != 0f) {
                if (playerInput.IsCurrentInputSource(RewiredConsts.Action.Camera_Vertical, ControllerType.Mouse)) {
                    if (GameManager.Instance.save.config[GameManager.Save.configID_CameraControlButton] != 0 && !playerInput.GetButton(RewiredConsts.Action.Camera_Control)) {
                        vertical = 0f;
                    } else {
                        vertical *= 0.2f;
                        vertical *= CameraManager.Instance.MouseWheelBias();
                    }
                }
            }
            if (horizontal != 0f || vertical != 0f) {
                Vector3 friendsVector = vecZero;
                switch (detailCameraType) {
                    case 0:
                    case 3:
                        friendsVector = friendsCenter.localEulerAngles;
                        if (horizontal != 0f) {
                            friendsVector.y -= horizontal * 180f * deltaTimeCache;
                        }
                        if (vertical != 0f) {
                            friendsVector.x -= vertical * 180f * deltaTimeCache;
                            if (friendsVector.x > 90f && friendsVector.x < 180f) {
                                friendsVector.x = 90f;
                            } else if (friendsVector.x >= 180f && friendsVector.x < 270f) {
                                friendsVector.x = 270f;
                            }
                        }
                        friendsCenter.localEulerAngles = friendsVector;
                        break;
                    case 1:
                        friendsVector = friendsCenter.localPosition;
                        if (horizontal != 0f) {
                            friendsVector.x = Mathf.Clamp(friendsVector.x - horizontal * cameraMoveSpeed * deltaTimeCache, -cameraMoveRange.x, cameraMoveRange.x);
                        }
                        if (vertical != 0f) {
                            friendsVector.y = Mathf.Clamp(friendsVector.y + vertical * cameraMoveSpeed * deltaTimeCache, -cameraMoveRange.y, cameraMoveRange.y);
                        }
                        friendsCenter.localPosition = friendsVector;
                        break;
                    case 2:
                        if (vertical != 0f) {
                            friendsVector = friendsCenter.localPosition;
                            friendsVector.z = Mathf.Clamp(friendsVector.z - vertical * cameraMoveSpeed * deltaTimeCache, -cameraMoveRange.z, cameraMoveRange.z);
                            friendsCenter.localPosition = friendsVector;
                        }
                        break;
                }
            } else if (detailCameraType == 3) {
                Vector3 friendsVector = friendsCenter.localEulerAngles;
                friendsVector.y -= 45f * deltaTimeCache;
                friendsCenter.localEulerAngles = friendsVector;
            }
        }
    }

    private void SetMotion_Start() {
        if (anim) {
            anim.Rebind();
            anim.speed = detailSpeedType * 0.1f;
            if (choiceAnswer != choiceType_Friends || !friendsData[pointer].isNPC) {
                anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.AnimSpeed], 1f);
            }
            if (choiceAnswer == choiceType_Enemy && !enemyData[pointer].friendsAnimator) {
                anim.PlayInFixedTime(AnimHash.Instance.ID[(int)AnimHash.ParamName.StateEnemyIdle], -1, 0f);
            }
            float attackSpeed = 1f;
            switch (detailMotionType) {
                case 0:
                    break;
                case 1:
                    anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.IdleType], 0);
                    anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.IdleMotion]);
                    break;
                case 2:
                    anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.IdleType], 1);
                    anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.IdleMotion]);
                    break;
                case 3:
                    if (choiceAnswer == choiceType_Enemy && !enemyData[pointer].friendsAnimator) {
                        anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.Move], true);
                    } else {
                        anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.Run], true);
                    }
                    anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.Speed], 3f);
                    break;
                case 4:
                    if (choiceAnswer == choiceType_Enemy && enemyData[pointer].friendsAnimator) {
                        anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.Move], true);
                    }
                    anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.Run], true);
                    anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.Speed], 6f);
                    break;
                case 5:
                    if (choiceAnswer == choiceType_Enemy && enemyData[pointer].friendsAnimator) {
                        anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.Move], true);
                    }
                    anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.Run], true);
                    anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.Speed], 9f);
                    break;
                case 6:
                    if (choiceAnswer == choiceType_Enemy && enemyData[pointer].friendsAnimator) {
                        anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.Move], true);
                    }
                    anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.Run], true);
                    anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.Speed], 13f);
                    break;
                case 7:
                    anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.JumpType], 0);
                    anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Jump]);
                    break;
                case 8:
                    anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.JumpType], 1);
                    anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Jump]);
                    break;
                case 9:
                    anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.JumpType], 3);
                    anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Jump]);
                    break;
                case 10:
                    anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.JumpType], 2);
                    anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Jump]);
                    break;
                case 11:
                    anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.StepDirection], -1);
                    anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.SideStep]);
                    break;
                case 12:
                    anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.StepDirection], 1);
                    anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.SideStep]);
                    break;
                case 13:
                    anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.StepDirection], 0);
                    anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.SideStep]);
                    break;
                case 14:
                    anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.StepDirection], 2);
                    anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.SideStep]);
                    break;
                case 15:
                    if (choiceAnswer == choiceType_Enemy && enemyData[pointer].knockHeavyHalfSpeed) {
                        anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], 1f);
                    }
                    anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockLight]);
                    break;
                case 16:
                    if (choiceAnswer == choiceType_Enemy && enemyData[pointer].knockHeavyHalfSpeed) {
                        anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], 0.5f);
                    }
                    anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockHeavy]);
                    break;
                case 17:
                    anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.Dead], true);
                    break;
                case 18:
                    anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.Refresh], true);
                    break;
                case 19:
                    anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.DrownTolerance], false);
                    anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.Drown], true);
                    break;
                case 20:
                    anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.DrownTolerance], true);
                    anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.Drown], true);
                    break;
                case 21:
                    anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.Climb], true);
                    break;
                case 22:
                    anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.Fear], true);
                    break;
                case 23:
                    anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Smile]);
                    break;
                case 24:
                case 25:
                case 26:
                case 27:
                case 28:
                case 29:
                case 30:
                case 31:
                case 32:
                case 33:
                case 34:
                case 35:
                case 36:
                case 37:
                case 38:
                case 39:
                case 40:
                case 41:
                case 42:
                case 43:
                case 44:
                    if (choiceAnswer == choiceType_Friends) {
                        for (int i = 0; i < friendsData[pointer].motionExceptions.Length; i++) {
                            if (friendsData[pointer].motionExceptions[i].motionIndex == detailMotionType) {
                                attackSpeed = friendsData[pointer].motionExceptions[i].attackSpeed;
                                i = friendsData[pointer].motionExceptions.Length;
                            }
                        }
                    }
                    anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.AttackSpeed], attackSpeed);
                    anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.AttackType], detailMotionType - 24);
                    anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Attack]);
                    break;
                case 45:
                    anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Spawn]);
                    break;
            }
        }
        if (choiceAnswer == choiceType_Friends && detailExpressionType == 0) {
            SetExpressionAccordingMotion();
        }
    }

    private void SetMotion_Update() {
        if (anim && detailMotionType != 0 && detailSpeedType >= 1) {
            AnimatorStateInfo animSI = anim.GetCurrentAnimatorStateInfo(0);
            if (detailMotionType >= 7 && detailMotionType <= 10 && animSI.IsTag("Jump")) {
                jumpingTime += Time.deltaTime;
                if (jumpingTime >= 2f / (detailSpeedType * 0.1f)) {
                    jumpingTime = 0f;
                    anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Landing]);
                }
            } else {
                jumpingTime = 0f;
            }
            if (choiceAnswer == choiceType_Enemy && !enemyData[pointer].friendsAnimator) {
                if (animSI.fullPathHash == AnimHash.Instance.ID[(int)AnimHash.ParamName.StateEnemyIdle]) {
                    idlingTime += Time.deltaTime;
                } else {
                    idlingTime = 0f;
                }
            } else {
                if (choiceAnswer == choiceType_Friends && friendsData[pointer].isNPC) {
                    if (animSI.fullPathHash == AnimHash.Instance.ID[(int)AnimHash.ParamName.StateIFRIdle]) {
                        idlingTime += Time.deltaTime;
                    } else {
                        idlingTime = 0f;
                    }
                } else {
                    if (animSI.fullPathHash == AnimHash.Instance.ID[(int)AnimHash.ParamName.StateFriendsIdle]) {
                        idlingTime += Time.deltaTime;
                    } else {
                        idlingTime = 0f;
                    }
                }                
            }
            if (idlingTime >= 0.35f / (detailSpeedType * 0.1f)) {
                idlingTime = 0f;
                switch (detailMotionType) {
                    case 1:
                    case 2:
                        anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.IdleMotion]);
                        break;
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                        anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Jump]);
                        break;
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                        anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.SideStep]);
                        break;
                    case 15:
                        anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockLight]);
                        break;
                    case 16:
                        anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockHeavy]);
                        break;
                    case 23:
                        anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Smile]);
                        break;
                    case 24:
                    case 25:
                    case 26:
                    case 27:
                    case 28:
                    case 29:
                    case 30:
                    case 31:
                    case 32:
                    case 33:
                    case 34:
                    case 35:
                    case 36:
                    case 37:
                    case 38:
                    case 39:
                    case 40:
                    case 41:
                    case 42:
                    case 43:
                    case 44:
                        anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Attack]);
                        break;
                    case 45:
                        anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Spawn]);
                        break;
                }
            }
        }
    }

    private void SetExpression() {
        if (faceCon) {
            switch (detailExpressionType) {
                case 0:
                    SetExpressionAccordingMotion();
                    break;
                case 1:
                    faceCon.SetFace("Idle");
                    break;
                case 2:
                    faceCon.SetFace("Blink");
                    break;
                case 3:
                    faceCon.SetFace("Idle1");
                    break;
                case 4:
                    faceCon.SetFace("Idle2");
                    break;
                case 5:
                    faceCon.SetFace("Run");
                    break;
                case 6:
                    faceCon.SetFace("Jump");
                    break;
                case 7:
                    faceCon.SetFace("Damage");
                    break;
                case 8:
                    faceCon.SetFace("Dead");
                    break;
                case 9:
                    faceCon.SetFace("Refresh");
                    break;
                case 10:
                    faceCon.SetFace("Fear");
                    break;
                case 11:
                    faceCon.SetFace("Smile");
                    break;
                case 12:
                    faceCon.SetFace("Attack");
                    break;
                case 13:
                    faceCon.SetFace("Dead2");
                    break;
                case 14:
                    faceCon.SetFace("Fear2");
                    break;
                case 15:
                    faceCon.SetFace("Smile2");
                    break;
                case 16:
                    faceCon.SetFace("Attack2");
                    break;
            }
        }
    }

    private void SetExpressionAccordingMotion() {
        if (faceCon) {
            bool exceptionFound = false;
            for (int i = 0; i < friendsData[pointer].motionExceptions.Length; i++) {
                if (detailMotionType == friendsData[pointer].motionExceptions[i].motionIndex && !string.IsNullOrEmpty(friendsData[pointer].motionExceptions[i].faceName)) {
                    faceCon.SetFace(friendsData[pointer].motionExceptions[i].faceName);
                    exceptionFound = true;
                    break;
                }
            }
            if (!exceptionFound) {
                switch (detailMotionType) {
                    case 0:
                    case 3:
                        faceCon.SetFace("Idle");
                        break;
                    case 1:
                        faceCon.SetFace("Idle1");
                        break;
                    case 2:
                        faceCon.SetFace("Idle2");
                        break;
                    case 4:
                    case 5:
                    case 6:
                        faceCon.SetFace("Run");
                        break;
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 20:
                    case 21:
                        faceCon.SetFace("Jump");
                        break;
                    case 18:
                        faceCon.SetFace("Refresh");
                        break;
                    case 15:
                    case 16:
                    case 19:
                        faceCon.SetFace("Damage");
                        break;
                    case 17:
                        faceCon.SetFace("Dead");
                        break;
                    case 22:
                        faceCon.SetFace("Fear");
                        break;
                    case 23:
                        faceCon.SetFace("Smile");
                        break;
                    case 24:
                    case 25:
                    case 26:
                    case 27:
                    case 28:
                    case 29:
                    case 30:
                    case 31:
                    case 32:
                    case 33:
                        faceCon.SetFace("Attack");
                        break;
                }
            }
        }
    }

    public void EventEnter(int param) {
        eventEnterNum = param;
    }

    public void EventClick(int param) {
        if (!Input.GetMouseButtonUp(1)) {
            eventClickNum = param;
        }
    }

    private void SetPercentageText() {
        int nowNum = 0;
        int maxNum = 0;
        switch (choiceAnswer) {
            case choiceType_Friends:
                maxNum = friendsData.Length;
                for (int i = 0; i < friendsData.Length; i++) {
                    if (CheckFriendsCondition(i)) {
                        nowNum++;
                    }
                }
                break;
            case choiceType_Enemy:
                maxNum = enemyData.Length;
                for (int i = 0; i < enemyData.Length; i++) {
                    if (CheckEnemyCondition(i)) {
                        nowNum++;
                    }
                }
                break;
            case choiceType_Facility:
                maxNum = facilityData.Length;
                for (int i = 0; i < facilityData.Length; i++) {
                    if (CheckFacilityCondition(i)) {
                        nowNum++;
                    }
                }
                break;
            case choiceType_Item:
                maxNum = itemData.Length;
                for (int i = 0; i < itemData.Length; i++) {
                    if (CheckItemCondition(i)) {
                        nowNum++;
                    }
                }
                break;
        }
        if (choiceAnswer == choiceType_Enemy && enemyCompMax <= 0) {
            if (GameManager.enemyMax * GameManager.enemyLevelMax <= GameManager.Instance.save.defeatEnemy.Length) {
                for (int i = 0; i < GameManager.enemyMax; i++) {
                    for (int j = 0; j < GameManager.enemyLevelMax; j++) {
                        if (CharacterDatabase.Instance.enemy[i].appearInBook[j]) {
                            enemyCompMax++;
                            if (GameManager.Instance.save.defeatEnemy[i * GameManager.enemyLevelMax + j] > 0) {
                                enemyCompNow++;
                            }
                        }
                    }
                }
            }
        }
        if (choiceAnswer == choiceType_Enemy) {
            percentText.rectTransform.anchoredPosition = percentagePosition[1].anchoredPosition;
            percentText.text = StringUtils.Format("{0} / {1}<size=70%> ({2} / {3})</size>", nowNum, maxNum, enemyCompNow, enemyCompMax);
        } else {
            percentText.rectTransform.anchoredPosition = percentagePosition[0].anchoredPosition;
            percentText.text = StringUtils.Format("{0} / {1}", nowNum, maxNum);
        }
        percentText.colorGradientPreset = (nowNum >= maxNum ? rainbowGradient : whiteGradient);
    }

}
