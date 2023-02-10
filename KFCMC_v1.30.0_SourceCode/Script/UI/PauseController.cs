using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rewired;

public class PauseController : SingletonMonoBehaviour<PauseController> {

    [System.Serializable]
    public class Property {
        public Canvas canvas;
        public GraphicRaycaster gRaycaster;
        public GridLayoutGroup gridLayoutGroup;
        public Transform panelType;
        public Transform panelItem;
        public Transform panelStatus;
        public GridLayoutGroup statusGrid;
        public TMP_Text[] typeText;
        public TMP_Text name;
        public TMP_Text information;
        public TMP_Text title;
        public TMP_Text page;
        public TMP_Text count;
        public RectTransform cursorRect;
        public Image cursorImage;
        public int cursorPos;
        public Vector2 origin;
        public Vector2 interval;
        public GameObject slotPrefab;
        public Image[] arrow;
        public TMP_ColorGradient limitColorFull;
        public TMP_ColorGradient limitColorMiddle;
        public TMP_ColorGradient limitColorZero;
        public TMP_ColorGradient limitColorOver;
        public TMP_ColorGradient normalColor;
    }

    [System.Serializable]
    public class Shop {
        public Canvas canvas;
        public GraphicRaycaster gRaycaster;
        public GridLayoutGroup gridLayoutGroup;
        public Transform panel;
        public TMP_Text price;
        public TMP_Text fund;
        public TMP_Text name;
        public TMP_Text information;
    }

    [System.Serializable]
    public class Storage {
        public Canvas canvas;
        public GraphicRaycaster gRaycaster;
        public GridLayoutGroup gridLayoutGroup;
        public Transform panel;
        public TMP_Text title;
        public TMP_Text page;
        public TMP_Text count;
        public TMP_Text name;
        public TMP_Text information;
        public Image[] arrow;
    }

    [System.Serializable]
    public class Document {
        public Canvas canvas;
        public GraphicRaycaster gRaycaster;
        public Image background;
        public TMP_Text content;
        public Image arrowLeft;
        public Image arrowRight;
        public TMP_Text arrowEnd;
        public Image specialImage;
        [System.NonSerialized]
        public string textHeader;
        [System.NonSerialized]
        public int nowPage;
        [System.NonSerialized]
        public int numPages;
        [System.NonSerialized]
        public int fragmentID;
        [System.NonSerialized]
        public int fragmentPage;
        [System.NonSerialized]
        public int specialPage;
    }

    [System.Serializable]
    public class SoundTest {
        public Canvas canvas;
        public GraphicRaycaster gRaycaster;
        public GridLayoutGroup gridLayoutGroup;
        public RectTransform typeCursor;
        public RectTransform itemCursor;
        public TMP_Text title;
        public TMP_Text[] types;
        public TMP_Text[] contents;
        public TMP_Text information;
        public Vector2 typeCursorOrigin;
        public Vector2 typeCursorInterval;
        public Vector2 itemCursorOrigin;
        public Vector2 itemCursorInterval;
        public int nowType;
        public int nowPivot;
        public int nowPlus;
        [System.NonSerialized]
        public int depthSave;
    }

    [System.Serializable]
    public class Bus {
        public Canvas canvas;
        public GraphicRaycaster gRaycaster;
        public GridLayoutGroup gridLayoutGroup;
        public RectTransform itemCursor;
        public TMP_Text title;
        public TMP_Text[] contents;
        public Vector2 itemCursorOrigin;
        public Vector2 itemCursorInterval;
        public int nowPivot;
        public int nowPlus;
        public Vector2Int[] destination;
        public string[] dicKey;
        public BusConsole busConsole;
    }

    [System.Serializable]
    public class BlendChild {
        public int afterItemID;
        public int overrideCost;
        public int[] beforeItemID;
        public int[] beforeItemCount;
        public bool isHomeOnly;
        public bool isRandomStageOnly;
        public bool isGameClearedOnly;
    }

    [System.Serializable]
    public class Blend {
        public Canvas canvas;
        public GraphicRaycaster gRaycaster;
        public GridLayoutGroup gridLayoutGroup;
        public RectTransform itemCursor;
        public TMP_Text title;
        public TMP_Text[] contents;
        public Image[] enabledMarks;
        public TMP_Text information;
        public Vector2 itemCursorOrigin;
        public Vector2 itemCursorInterval;
        public int nowPivot;
        public int nowPlus;
        public bool requireGold;
        public TMP_ColorGradient okColor;
        public TMP_ColorGradient ngColor;
        public BlendConsole blendConsole;
        public BlendChild[] blendChild;
    }

    [System.Serializable]
    public class Choices {
        public Canvas canvas;
        public GraphicRaycaster gRaycaster;
        public GridLayoutGroup[] gridLayoutGroups;
        public RectTransform panel;
        public RectTransform cursorRect;
        public TMP_Text title;
        public TMP_Text[] choice;
        public int cursorPos;
        public int max = 3;
        public Vector2 origin;
        public Vector2 interval;
    }

    [System.Serializable]
    public class Volume : Choices {
        public TMP_Text[] amount;
        public Slider[] slider;
    }

    [System.Serializable]
    public class Config : Choices {
        public TMP_Text[] state;
        public TMP_Text information;
        public TMP_ColorGradient defaultColor;
        public TMP_ColorGradient changedColor;
        public TMP_ColorGradient enableColor;
        public TMP_ColorGradient disableColor;
        public TMP_Text pageText;
        public Image[] arrow;
        [System.NonSerialized]
        public bool arrowEnabled;
        [System.NonSerialized]
        public int count;
        [System.NonSerialized]
        public int pageNow;
        [System.NonSerialized]
        public int pageMax;

        public const int denomi = 9;

        public void ChangeArrow(bool flag) {
            if (arrowEnabled != flag) {
                arrowEnabled = flag;
                for (int i = 0; i < arrow.Length; i++) {
                    arrow[i].raycastTarget = flag;
                    arrow[i].enabled = flag;
                }
                pageText.enabled = flag;
            }
        }

        public void SetPageText() {
            pageText.text = string.Format("{0} {1} / {2}", TextManager.Get("CONFIG_PAGE"), pageNow + 1, pageMax);
        }
    }

    [System.Serializable]
    public class Caution {
        public Canvas canvas;
        public TMP_Text text;
        public Image markImage;
    }
    
    [System.Serializable]
    public class Photo {
        public Canvas canvas;
        public TMP_Text infoText;
        public TMP_Text controlText;
        public TMP_Text limitText;
        public GameObject[] exclude;
    }

    [System.Serializable]
    public class BlackCurtain {
        public Canvas canvas;
        public Image blackImage;
    }

    [System.Serializable]
    public class Tutorial {
        public Canvas canvas;
        public GraphicRaycaster gRaycaster;
        public RectTransform pivot;
        public GameObject[] slots;
        public Image arrowLeft;
        public Image arrowRight;
        public GameObject instance;
        public bool continuous;
        public int index;
    }

    [System.Serializable]
    public class Combination {
        public Canvas canvas;
        public GraphicRaycaster gRaycaster;
        public TMP_Text readWriteText;
        public Image readWriteImage;
        public Sprite readSprite;
        public Sprite writeSprite;
        public TMP_ColorGradient readColor;
        public TMP_ColorGradient writeColor;
        public RectTransform cursor;
        public int cursorPos;
        public Vector2 cursorOrigin;
        public Vector2 cursorInterval;
        public CombinationSlot[] slots;
        public GridLayoutGroup[] grids;
    }

    public Canvas baseCanvas;
    public Property property;
    public Shop shop;
    public Storage storage;
    public Choices choices;
    public Volume volume;
    public Config config;
    public Document document;
    public SoundTest soundTest;
    public Bus bus;
    public Blend blend;
    public Caution caution;
    public Photo photo;
    public BlackCurtain blackCurtain;
    public Tutorial tutorial;
    public Combination combination;
    public Canvas offPauseCanvas;
    public GameObject friendsActPrefab;
    public GameObject friendsDeadPrefab;
    public GameObject equipedPrefab;
    public GameObject lockedPrefab;
    public GameObject soldOutPrefab;
    public GameObject statusTextPrefab;
    public GameObject statusTextImagePrefab;
    public Sprite[] difficultySprites;
    public InGameProfiler inGameProfiler;
    public ControlMapperWrapper controlMapperWrapper;

    [System.NonSerialized]
    public bool pauseEnabled;
    [System.NonSerialized]
    public bool pauseGame;
    [System.NonSerialized]
    public bool returnToLibraryProcessing;
    [System.NonSerialized]
    public bool itemDisabled;
    [System.NonSerialized]
    public bool friendsDisabled;
    [System.NonSerialized]
    public bool equipChangeDisabled;
    [System.NonSerialized]
    public bool gameOverDisabled;
    [System.NonSerialized]
    public bool returnLibraryDisabled;
    [System.NonSerialized]
    public List<BlendChild> blendChildList = new List<BlendChild>(32);
    [System.NonSerialized]
    public int notPausingFrames;

    const int typeMax = 5;
    const int slotMax = 32;
    const int volumeMin = 0;
    const int volumeMax = 100;
    const int configItemMax = 17;
    const int anotherWeaponGap = 50;
    const string slash = " / ";
    const string slashTight = "/";
    const string colorTagWhite = "<color=#FFFFFF>";
    const string colorTagRed = "<color=#FF8080>";
    const string colorTagYellow = "<color=#FFFF80>";
    const string colorTagGray = "<color=#808080>";
    const string colorTagEnd = "</color>";
    static readonly int[] anotherIconID = new int[] { 205, 217, 218 };
    /*
    const int pauseTypeNormal = 0;
    const int pauseTypeBuy = 1;
    const int pauseTypeSell = 2;
    const int pauseTypeStore = 3;
    const int pauseTypeTakeout = 4;
    const int pauseTypeDocument = 5;
    const int pauseTypeSoundTest = 6;
    */

    enum Type { Item, Friends, Weapon, Status, Config, Buy, Sell, Store, TakeOut, SoundTest, Trophy, FDKaban };
    enum ChoicesType { Item, Friends, Buy, Sell, Store, TakeOut, StatusUp, Quit, SoundTest, Command, Reset, FDKaban, Language };
    enum PauseType { Normal, Buy, Sell, Store, Takeout, Document, SoundTest, Photo, Bus, Tutorial, Blend, Dictionary };
    PlayerController pCon;
    Player playerInput;
    StringBuilder sb = new StringBuilder();
    Sprite empty;
    GameObject[] slotObject = new GameObject[slotMax];
    GameObject[] markObject = new GameObject[slotMax];
    GameObject[] lockObject = new GameObject[slotMax];
    GameObject itemNameUIObject;
    int state;
    int typeCursor = 0;
    int itemCursor = 0;
    int[] cursorSave = new int[12];
    int numSlots = 0;
    // 0=pause  1=buy  2=sell  3=store  4=takeout  5=document  6=SoundTest
    PauseType pauseType = 0;
    int configType = 0;
    int[] slotItemId = new int[slotMax];
    bool[] buyItemSoldOut = new bool[slotMax];
    int[] buyItemPrice = new int[slotMax];
    float replayAppointment;
    bool firstUpdateApplySpeaker = false;
    int statusCursor = 0;
    int statusMax = 1;
    bool statusArrowEnabled;
    Vector2Int move;
    FriendsBase fBase;
    bool specialPageSFXEnabled;
    AudioSource specialPageSFXSource;
    int showCautionRemainFrames;
    int[] statusUpSave = new int[3];
    int[] volumeAmount = new int[3];
    bool frameAdvanceFlag;
    bool frameAdvanceEnabled;
    float frameAdvancePressTime;
    int depthOfFieldSave;
    Color darkColor = new Color(221f / 256f, 221f / 256f, 221f / 256f);
    Color blackColor = new Color(0f, 0f, 0f, 0f);
    Color whiteColor = new Color(1f, 1f, 1f, 0f);
    int blendChildMax;
    bool copyItemsReserved;
    int[] copyItemsArray;
    int[] skillTutorialProgress = new int[6];
    int keyBindsLanguageSave = -1;
    float pausingTime;
    Vector2 informationTextSizeDelta = new Vector2(840, 100);
    Vector2 informationTextPosition = new Vector2(0, -220);
    bool closeTutorialToOpenMenu;
    int eventEnterNum = -1;
    int eventClickNum = -1;
    float mouseScroll;
    bool shopBoughtFlag;
    int trophyPage;
    int storagePage;
    float blackCurtainAmountSave;
    bool isTypeTextMain;
    GameObject[] facilityObjs = new GameObject[fdKabanIDArray.Length];
    
    static readonly int[] configMax = new int[] { 15, 14, 9, 6, 7, 8, 4, 7, 8, 16 };
    static readonly int[] configStart = new int[] { 0, 20, 40, 50, 60, 70, 80, 90, 100, 110 };
    static readonly float[] informationTextWidth = new float[GameManager.languageMax] { 840f, 850f, 840f, 840f };
    static readonly float[] informationTextPosX = new float[GameManager.languageMax] { 0f, 5f, 0f, 0f };
    static readonly Vector2 vec2Zero = Vector2.zero;
    static readonly Vector2 vec2SpecialPos = new Vector2(0f, 40f);
    static readonly int[] skillID = { 13, 14, 15, 16, 17, 18 };
    static readonly int[] fdKabanIDArray = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 15, 19, 21, 35, 23, 24, 32, 33, 36, 37, 34, 38 };
    const int fdKabanDummyID = 35;
    const int dummyEnemyID = 54;
    const int fdKabanShopID = 38;
    const int shopIfrID = 145;
    static readonly Vector3 vecUp = Vector3.up;
    const int skillTutorialIndex = 10;
    const int eventChoiceBase = 900;
    public static readonly int[] itemHealAmounts = new int[] { 80, 200, 400 };
    public static readonly int[] itemHealPercents = new int[] { 20, 35, 50 };
    public static readonly int[] itemHealIndex = new int[] { 0, 1, 2 };
    public const int itemAntidoteID = 20;
    public const int itemSandstarID = 9;

    void OnPause(bool isOn, bool sound, PauseType pauseType) {
        this.pauseType = pauseType;
        Input.ResetInputAxes();
        switch (pauseType) {
            case PauseType.Normal:
                HideCaution();
                if (typeCursor < (int)Type.Item || typeCursor > (int)Type.Config) {
                    typeCursor = (int)Type.Item;
                }
                if (isTypeTextMain == false) {
                    SetTypeText(true);
                }
                SetStatusText(false);
                property.canvas.enabled = isOn;
                property.gridLayoutGroup.enabled = isOn;
                if (!isOn || GameManager.Instance.MouseEnabled) {
                    property.gRaycaster.enabled = isOn;
                }
                property.name.text = string.Empty;
                property.information.text = string.Empty;
                if (isOn) {
                    SelectItemTypeChild();
                } else {
                    UpdateLevelLimit();
                    UpdateEquipmentLimit();
                    UpdateFriendsEffectDisabled();
                    UpdateHellMode();
                }
                if (sound) {
                    UISE.Instance.Play(isOn ? UISE.SoundName.open : UISE.SoundName.close);
                }
                state = 0;
                break;
            case PauseType.Buy:
            case PauseType.Sell:
                shop.canvas.enabled = isOn;
                shop.gridLayoutGroup.enabled = isOn;
                if (!isOn || GameManager.Instance.MouseEnabled) {
                    shop.gRaycaster.enabled = isOn;
                }
                HideCaution();
                if (isOn) {
                    typeCursor = (pauseType == PauseType.Buy ? (int)Type.Buy : (int)Type.Sell);
                    itemCursor = cursorSave[typeCursor];
                    CreateSlot();
                    SetSlotObjectColor(true);
                    SetInformationText(shop.name, shop.information);
                } else {
                    shop.name.text = string.Empty;
                    shop.information.text = string.Empty;
                }
                state = 1;
                break;
            case PauseType.Store:
            case PauseType.Takeout:
                storage.canvas.enabled = isOn;
                storage.gridLayoutGroup.enabled = isOn;
                if (!isOn || GameManager.Instance.MouseEnabled) {
                    storage.gRaycaster.enabled = isOn;
                }
                HideCaution();
                if (isOn) {
                    typeCursor = (pauseType == PauseType.Store ? (int)Type.Store : (int)Type.TakeOut);
                    itemCursor = cursorSave[typeCursor];
                    CreateSlot();
                    SetSlotObjectColor(true);
                    SetInformationText(storage.name, storage.information);
                } else {
                    storage.name.text = string.Empty;
                    storage.information.text = string.Empty;
                }
                state = 1;
                break;
            case PauseType.Document:
                document.canvas.enabled = isOn;
                if (!isOn || GameManager.Instance.MouseEnabled) {
                    document.gRaycaster.enabled = isOn;
                }
                state = 2;
                break;
            case PauseType.SoundTest:
                PauseSoundTest(isOn);
                break;
            case PauseType.Photo:
                state = 50;
                break;
            case PauseType.Bus:
                PauseBus(isOn);
                break;
            case PauseType.Tutorial:
                tutorial.canvas.enabled = isOn;
                if (!isOn || GameManager.Instance.MouseEnabled) {
                    tutorial.gRaycaster.enabled = isOn;
                }
                if (!isOn && tutorial.instance != null) {
                    Destroy(tutorial.instance);
                }
                state = 25;
                break;
            case PauseType.Blend:
                PauseBlend(isOn);
                break;
            case PauseType.Dictionary:
                break;
        }
        if (offPauseCanvas) {
            if (pauseType == PauseType.Photo && GameManager.Instance.save.config[GameManager.Save.configID_PhotoMode] == 2) {
            }
            else {
                offPauseCanvas.enabled = !isOn;
            }
        }
        SetVolumeCanvasEnabled(false);
        SetConfigCanvasEnabled(false);
        SetChoicesCanvasEnabled(false);
        if (!isOn) {
            GameManager.Instance.CheckMinmi();
        }
        Time.timeScale = isOn ? 0 : GameManager.Instance.GetStandardTimeScale();
        GameManager.Instance.ChangeTimeScale(isOn);
        pauseGame = isOn;
        pausingTime = 0f;
        CharacterManager.Instance.SetPlayerUpdateEnabled(!isOn);
        // SpecialMove Tutorial
        if (pauseType == PauseType.Buy) {
            for (int i = 0; i < skillTutorialProgress.Length; i++) {
                if (isOn) {
                    skillTutorialProgress[i] = (GameManager.Instance.save.weapon[GameManager.skillTutorialIndex[i, 0]] != 0 ? 0 : 1);
                } else if (skillTutorialProgress[i] == 1 && GameManager.Instance.save.weapon[GameManager.skillTutorialIndex[i, 0]] != 0) {
                    skillTutorialProgress[i] = 2;
                }
            }
        }
        if (isOn && !baseCanvas.enabled) {
            baseCanvas.enabled = true;
        }
        if (!isOn && shopBoughtFlag) {
            shopBoughtFlag = false;
            if (TrophyManager.Instance) {
                TrophyManager.Instance.CheckTrophy(TrophyManager.t_BuyAllWeapons);
                TrophyManager.Instance.CheckTrophy(TrophyManager.t_GetAllHp);
                TrophyManager.Instance.CheckTrophy(TrophyManager.t_GetAllSt);
            }
        }
    }

    void SetChoicesCanvasEnabled(bool flag) {
        if (choices.canvas && choices.canvas.enabled != flag) {
            choices.canvas.enabled = flag;
            for (int i = 0; i < choices.gridLayoutGroups.Length; i++) {
                choices.gridLayoutGroups[i].enabled = flag;
            }
            bool grayFlag = flag && GameManager.Instance.MouseEnabled;
            choices.gRaycaster.enabled = grayFlag;
            if (GameManager.Instance.MouseEnabled) {
                property.gRaycaster.enabled = property.canvas.enabled && !grayFlag;
                shop.gRaycaster.enabled = shop.canvas.enabled && !grayFlag;
                storage.gRaycaster.enabled = storage.canvas.enabled && !grayFlag;
            }
        }
    }

    void SetVolumeCanvasEnabled(bool flag) {
        if (volume.canvas && volume.canvas.enabled != flag) {
            volume.canvas.enabled = flag;
            bool grayFlag = flag && GameManager.Instance.MouseEnabled;
            volume.gRaycaster.enabled = grayFlag;
            if (GameManager.Instance.MouseEnabled) {
                property.gRaycaster.enabled = property.canvas.enabled && !grayFlag;
            }
        }
    }

    void SetConfigCanvasEnabled(bool flag) {
        if (config.canvas && config.canvas.enabled != flag) {
            config.canvas.enabled = flag;
            for (int i = 0; i < config.gridLayoutGroups.Length; i++) {
                config.gridLayoutGroups[i].enabled = flag;
            }
            bool grayFlag = flag && GameManager.Instance.MouseEnabled;
            config.gRaycaster.enabled = grayFlag;
            if (GameManager.Instance.MouseEnabled) {
                property.gRaycaster.enabled = property.canvas.enabled && !grayFlag;
            }
        }
    }

    void InitSoundTexts() {
        if (soundTest.types.Length > 2) {
            soundTest.types[0].text = TextManager.Get("WORD_MUSIC");
            soundTest.types[1].text = TextManager.Get("WORD_AMBIENT");
            soundTest.types[2].text = TextManager.Get("CHOICE_CANCEL");
        }
        if (SoundDatabase.Instance) {
            SoundDatabase.Instance.SetEnabled();
        }
        soundTest.nowType = 0;
        ChangeSoundType();
    }

    void ChangeSoundType() {
        if (soundTest.nowType == 0) {
            if (BGM.Instance) {
                soundTest.nowPivot = BGM.Instance.GetPlayingIndex() + 1;
            }
        } else if (soundTest.nowType == 1) {
            if (Ambient.Instance) {
                soundTest.nowPivot = Ambient.Instance.GetPlayingIndex() + 1;
            }
        }
        soundTest.itemCursor.gameObject.SetActive(soundTest.nowType < 2);
        soundTest.nowPlus = 0;
        soundTest.typeCursor.anchoredPosition = soundTest.typeCursorOrigin + soundTest.typeCursorInterval * soundTest.nowType;
        soundTest.itemCursor.anchoredPosition = soundTest.itemCursorOrigin;
        SetSoundItemTexts();
    }

    void SetSoundItemTexts() {
        string titleTemp;
        if (soundTest.nowType < 2 && SoundDatabase.Instance) {
            int indexStart = soundTest.nowPivot;
            int indexMax = soundTest.nowType == 0 ? SoundDatabase.musicMax + 1 : SoundDatabase.ambientMax + 1;
            for (int i = 0; i < soundTest.contents.Length; i++) {
                int indexTemp = (indexStart + i) % indexMax - 1;
                if (indexTemp < 0) {
                    soundTest.contents[i].text = TextManager.Get("WORD_STOP");
                } else {
                    if (soundTest.nowType == 0 ? SoundDatabase.Instance.musicEnabled[indexTemp] : SoundDatabase.Instance.ambientEnabled[indexTemp]) {
                        if (soundTest.nowType == 0 && BGM.Instance.musicModInfoChanged && BGM.Instance.musicModTitle.Length > indexTemp && !string.IsNullOrEmpty(BGM.Instance.musicModTitle[indexTemp])) {
                            titleTemp = BGM.Instance.musicModTitle[indexTemp];
                        } else {
                            titleTemp = TextManager.Get(sb.Clear().Append(soundTest.nowType == 0 ? "MUSIC_NAME_" : "AMBIENT_NAME_").AppendFormat("{0:00}", indexTemp).ToString());
                        }
                    } else {
                        titleTemp = TextManager.Get("MUSIC_NAME_LOCK");
                    }
                    soundTest.contents[i].text = sb.Clear().AppendFormat("{0:00}", indexTemp + 1).Append(". ").Append(titleTemp).ToString();
                }
            }
            int selectTemp = (soundTest.nowPivot + soundTest.nowPlus) % indexMax - 1;
            if (selectTemp < 0) {
                soundTest.information.text = "";
            } else if (soundTest.nowType == 0 ? SoundDatabase.Instance.musicEnabled[selectTemp] : SoundDatabase.Instance.ambientEnabled[selectTemp]) {
                if (soundTest.nowType == 0 && BGM.Instance.musicModInfoChanged && BGM.Instance.musicModCaption.Length > selectTemp && !string.IsNullOrEmpty(BGM.Instance.musicModCaption[selectTemp])) {
                    soundTest.information.text = BGM.Instance.musicModCaption[selectTemp];
                } else {
                    soundTest.information.text = TextManager.Get(sb.Clear().Append(soundTest.nowType == 0 ? "MUSIC_INFO_" : "AMBIENT_INFO_").AppendFormat("{0:00}", selectTemp).ToString());
                }
            } else {
                soundTest.information.text = "";
            }
        } else {
            for (int i = 0; i < soundTest.contents.Length; i++) {
                soundTest.contents[i].text = "";
            }
            soundTest.information.text = "";
        }
    }

    void SetTypeText(bool main = true) {
        for (int i = 0; i < property.typeText.Length; i++) {
            if (property.typeText[i].enabled != main) {
                property.typeText[i].enabled = main;
            }
        }
        if (property.cursorImage.enabled != main) {
            property.cursorImage.enabled = main;
        }
        if (property.title.enabled == main) {
            property.title.enabled = !main;
        }
        if (property.page.enabled == main) {
            property.page.enabled = !main;
        }
        if (property.count.enabled == main) {
            property.count.enabled = !main;
        }
        if (main) {
            if (property.typeText.Length >= 5) {
                property.typeText[0].text = TextManager.Get("PAUSE_ITEM");
                property.typeText[1].text = TextManager.Get("PAUSE_FRIENDS");
                property.typeText[2].text = TextManager.Get("PAUSE_EQUIP");
                property.typeText[3].text = TextManager.Get("PAUSE_STATUS");
                property.typeText[4].text = TextManager.Get("PAUSE_CONFIG");
            }
        } else {
            int trophyCount = TrophyManager.Instance.GetTrophyCount();
            property.title.text = TextManager.Get("TROPHY_TITLE");
            property.page.text = TextManager.Get("TROPHY_PAGE_" + (trophyPage + 1));
            property.count.text = sb.Clear().Append(trophyCount).Append("/").Append(GameManager.trophyMax).ToString();
            property.count.colorGradientPreset = trophyCount >= GameManager.trophyMax ? property.limitColorOver : property.normalColor;
        }
        informationTextPosition.x = informationTextPosX[Mathf.Clamp(GameManager.Instance.save.language, 0, informationTextPosX.Length - 1)];
        property.information.rectTransform.anchoredPosition = informationTextPosition;
        informationTextSizeDelta.x = informationTextWidth[Mathf.Clamp(GameManager.Instance.save.language, 0, informationTextWidth.Length)];
        property.information.rectTransform.sizeDelta = informationTextSizeDelta;
        isTypeTextMain = main;
    }

    void SetShopItem() {
        int shopLevel = ShopDatabase.Instance.GetShopLevel();
        int shopListMax = ShopDatabase.Instance.shopList.Count;
        int slotCount = 0;
        for (int i = 0; i < slotMax; i++) {
            slotItemId[i] = -1;
        }
        for (int i = 0; i < shopListMax; i++) {
            if (shopLevel >= ShopDatabase.Instance.shopList[i].conditionShopLevel && (ShopDatabase.Instance.shopList[i].conditionId < 0 || GameManager.Instance.save.weapon[ShopDatabase.Instance.shopList[i].conditionId] != 0)) {
                slotItemId[slotCount] = ShopDatabase.Instance.shopList[i].id;
                buyItemPrice[slotCount] = ItemDatabase.Instance.GetItemPrice(slotItemId[slotCount]);
                if (slotItemId[slotCount] == GameManager.hpUpId) {
                    buyItemPrice[slotCount] *= Mathf.Min(GameManager.Instance.save.hpUpSale + 1, GameManager.hpUpSaleMax);
                } else if (slotItemId[slotCount] == GameManager.stUpId) {
                    buyItemPrice[slotCount] *= Mathf.Min(GameManager.Instance.save.stUpSale + 1, GameManager.stUpSaleMax);
                }
                slotCount++;
            }
        }
        numSlots = slotCount;
    }

    void SetVolumeText(int index, int param) {
        if (param > volumeMin) {
            sb.Clear();
            volume.amount[index].text = sb.Append(param).Append("%").ToString();
        } else {
            volume.amount[index].text = TextManager.Get("CONFIG_VOL_MUTE");
        }
    }

    void UpdateVolume(bool init = false) {
        volumeAmount[0] = GameManager.Instance.save.musicVolume;
        volumeAmount[1] = GameManager.Instance.save.seVolume;
        volumeAmount[2] = GameManager.Instance.save.ambientVolume;        
        for (int i = 0; i < volumeAmount.Length && i < volume.slider.Length; i++) {
            if (init || volume.slider[i].value != volumeAmount[i]) {
                if (volume.slider[i].maxValue != 100) {
                    volume.slider[i].maxValue = 100;
                }
                volume.slider[i].value = volumeAmount[i];
                SetVolumeText(i, volumeAmount[i]);
            }
        }
    }

    public void UpdateLevelLimit() {
        if (CharacterManager.Instance.levelLimit != GameManager.Instance.save.config[GameManager.Save.configID_LevelLimit]) {
            CharacterManager.Instance.levelLimit = GameManager.Instance.save.config[GameManager.Save.configID_LevelLimit];
            CharacterManager.Instance.CheckLimitSet(true, false);
        }
    }

    void UpdateEquipmentLimit() {
        if (CharacterManager.Instance && GameManager.Instance.equipmentLimitEnabled != (GameManager.Instance.save.config[GameManager.Save.configID_EquipmentLimit] != 0)) {
            GameManager.Instance.equipmentLimitEnabled = (GameManager.Instance.save.config[GameManager.Save.configID_EquipmentLimit] != 0);
            CharacterManager.Instance.CheckLimitSet(false, true);
        }
    }

    public void UpdateHellMode() {
        if (CharacterManager.Instance) {
            CharacterManager.Instance.UpdateHellMode();
        }
    }
    
    void UpdateFriendsEffectDisabled() {
        if (CharacterManager.Instance) {
            CharacterManager.Instance.UpdateFriendsEffectDisabled();
        }
    }

    void SetStatusUpText(int index, int param) {
        volume.amount[index].text = param.ToString();
    }

    void UpdateStatusUp(bool init = false) {
        volumeAmount[0] = statusUpSave[0];
        volumeAmount[1] = statusUpSave[1];
        volumeAmount[2] = statusUpSave[2];
        if (init) {
            volume.slider[0].maxValue = GameManager.Instance.save.GotHpUpOriginal;
            volume.slider[1].maxValue = GameManager.Instance.save.GotStUpOriginal;
            volume.slider[2].maxValue = GameManager.Instance.save.GotInvUpOriginal;
        }
        for (int i = 0; i < volumeAmount.Length && i < volume.slider.Length; i++) {
            if (init || volume.slider[i].value != volumeAmount[i]) {
                volume.slider[i].value = volumeAmount[i];
                SetStatusUpText(i, volumeAmount[i]);
            }
        }
    }

    void VolumeControl() {
        move = GameManager.Instance.MoveCursor(true, 0.0125f, 0.1f);
        if (move.x != 0) {
            if (volume.cursorPos == 0) {
                GameManager.Instance.save.musicVolume += move.x;
                if (GameManager.Instance.save.musicVolume < volumeMin) {
                    GameManager.Instance.save.musicVolume = volumeMin;
                } else if (GameManager.Instance.save.musicVolume > volumeMax) {
                    GameManager.Instance.save.musicVolume = volumeMax;
                }
            } else if (volume.cursorPos == 1) {
                GameManager.Instance.save.seVolume += move.x;
                if (GameManager.Instance.save.seVolume < volumeMin) {
                    GameManager.Instance.save.seVolume = volumeMin;
                } else if (GameManager.Instance.save.seVolume > volumeMax) {
                    GameManager.Instance.save.seVolume = volumeMax;
                }
            } else if (volume.cursorPos == 2) {
                GameManager.Instance.save.ambientVolume += move.x;
                if (GameManager.Instance.save.ambientVolume < volumeMin) {
                    GameManager.Instance.save.ambientVolume = volumeMin;
                } else if (GameManager.Instance.save.ambientVolume > volumeMax) {
                    GameManager.Instance.save.ambientVolume = volumeMax;
                }
            }
            UpdateVolume();
            GameManager.Instance.ApplyVolume();
        }
        if (move.y == 0 && mouseScroll != 0) {
            move.y = (mouseScroll > 0 ? -1 : 1);
        }
        if (move.y != 0) {
            volume.cursorPos = (volume.cursorPos + move.y + volume.max) % volume.max;
            volume.cursorRect.anchoredPosition = volume.origin + volume.interval * volume.cursorPos;
        }
        if (move != Vector2Int.zero) {
            UISE.Instance.Play(UISE.SoundName.move);
        }
        if (GameManager.Instance.GetCancelButtonDown) {
            SetVolumeCanvasEnabled(false);
            state = 1;
            UISE.Instance.Play(UISE.SoundName.cancel);
        }
    }

    void StatusUpControl() {
        move = GameManager.Instance.MoveCursor(true, 0.025f, 0.1f);
        if (move.x != 0) {
            if (volume.cursorPos == 0) {
                statusUpSave[0] += move.x;
                if (statusUpSave[0] < 0) {
                    statusUpSave[0] = 0;
                } else if (statusUpSave[0] > GameManager.Instance.save.GotHpUpOriginal) {
                    statusUpSave[0] = GameManager.Instance.save.GotHpUpOriginal;
                }
            } else if (volume.cursorPos == 1) {
                statusUpSave[1] += move.x;
                if (statusUpSave[1] < 0) {
                    statusUpSave[1] = 0;
                } else if (statusUpSave[1] > GameManager.Instance.save.GotStUpOriginal) {
                    statusUpSave[1] = GameManager.Instance.save.GotStUpOriginal;
                }
            } else if (volume.cursorPos == 2) {
                statusUpSave[2] += move.x;
                if (statusUpSave[2] < 0) {
                    statusUpSave[2] = 0;
                } else if (statusUpSave[2] > GameManager.Instance.save.GotInvUpOriginal) {
                    statusUpSave[2] = GameManager.Instance.save.GotInvUpOriginal;
                }
            }
            UpdateStatusUp();
        }
        if (move.y != 0) {
            volume.cursorPos = (volume.cursorPos + move.y + volume.max) % volume.max;
            volume.cursorRect.anchoredPosition = volume.origin + volume.interval * volume.cursorPos;
        }
        if (move != Vector2Int.zero) {
            UISE.Instance.Play(UISE.SoundName.move);
        }
        if (GameManager.Instance.GetCancelButtonDown) {
            GameManager.Instance.save.equipedHpUp = statusUpSave[0];
            GameManager.Instance.save.equipedStUp = statusUpSave[1];
            GameManager.Instance.save.equipedInvUp = statusUpSave[2];
            GameManager.Instance.save.equip[GameManager.hpUpId - GameManager.armsIDBase] = (GameManager.Instance.save.equipedHpUp > 0 ? 1 : 0);
            GameManager.Instance.save.equip[GameManager.stUpId - GameManager.armsIDBase] = (GameManager.Instance.save.equipedStUp > 0 ? 1 : 0);
            GameManager.Instance.save.equip[GameManager.invUpId - GameManager.armsIDBase] = (GameManager.Instance.save.equipedInvUp > 0 ? 1 : 0);
            if (CharacterManager.Instance) {
                CharacterManager.Instance.CheckItemOverflow();
                CharacterManager.Instance.CheckHPSTOverflow();
            }
            CreateSlot();
            SetSlotObjectColor(true);
            SetVolumeCanvasEnabled(false);
            state = 1;
            UISE.Instance.Play(UISE.SoundName.cancel);
        }
    }

    void SetConfigText_Name() {
        for (int i = 0; i < config.choice.Length; i++) {
            int pointer = config.pageNow * Config.denomi + i;
            if (pointer < config.count) {
                config.choice[i].text = TextManager.Get(sb.Clear().AppendFormat("CONFIG_NAME_{0}_{1}", configType, pointer).ToString());
            } else {
                config.choice[i].text = "";
            }
            //Difficulty NT
            int id = configStart[configType] + pointer;
            if ((id == GameManager.Save.configID_GameSpeed || id == GameManager.Save.configID_BattleAssist) && GameManager.Instance.save.difficulty > GameManager.difficultyNT) {
                config.choice[i].colorGradientPreset = config.disableColor;
            } else {
                config.choice[i].colorGradientPreset = config.enableColor;
            }
        }
    }

    void SetConfigText_State(int index) {
        if (config.pageNow * Config.denomi + index < configMax[configType]) {
            int pointer = configStart[configType] + config.pageNow * Config.denomi + index;
            int param = GameManager.Instance.save.config[pointer];
            if (pointer < GameManager.Instance.configDefaultValues.Length) {
                config.state[index].colorGradientPreset = (param == GameManager.Instance.configDefaultValues[pointer] ? config.defaultColor : config.changedColor);
            }
            switch (pointer) {
                case GameManager.Save.configID_UseRunButton:
                    if (param == 0) {
                        config.state[index].text = TextManager.Get("CONFIG_OFF");
                    } else if (param == 1) {
                        config.state[index].text = TextManager.Get("CONFIG_RUN");
                    } else if (param == 2) {
                        config.state[index].text = TextManager.Get("CONFIG_WALK");
                    }
                    break;
                case GameManager.Save.configID_SearchRadius:
                case GameManager.Save.configID_SearchRadiusBoss:
                    if (param >= 0) {
                        config.state[index].text = sb.Clear().Append(param * 2 + 10).Append(TextManager.Get("CONFIG_METER")).ToString();
                    }
                    break;
                case GameManager.Save.configID_LockonReset:
                    if (param == 0) {
                        config.state[index].text = TextManager.Get("CONFIG_OFF");
                    } else if (param == 1) {
                        config.state[index].text = TextManager.Get("CONFIG_ENABLE");
                    } else if (param == 2) {
                        config.state[index].text = TextManager.Get("CONFIG_DISABLE");
                    }
                    break;
                case GameManager.Save.configID_CameraSensitivity:
                case GameManager.Save.configID_CameraFollowingSpeed:
                case GameManager.Save.configID_SuppressCameraTurning:
                    config.state[index].text = param.ToString();
                    break;
                case GameManager.Save.configID_CameraAxisInvert:
                    config.state[index].text = TextManager.Get(sb.Clear().Append("CONFIG_AXIS_").Append(param).ToString());
                    break;
                case GameManager.Save.configID_CameraControlButton:
                    if (param == 0) {
                        config.state[index].text = TextManager.Get("CONFIG_OFF");
                    } else if (param == 1) {
                        config.state[index].text = TextManager.Get("CONFIG_ON");
                    } else if (param == 2) {
                        config.state[index].text = TextManager.Get("CONFIG_LOCKON");
                    }
                    break;
                case GameManager.Save.configID_CameraTurningSpeed:
                    if (param < 0) {
                        config.state[index].text = TextManager.Get("CONFIG_STOP");
                    } else {
                        config.state[index].text = param.ToString();
                    }
                    break;
                case GameManager.Save.configID_CameraTargeting:
                    config.state[index].text = TextManager.Get(sb.Clear().Append("CONFIG_CAMERATARGET_").Append(param).ToString());
                    break;
                case GameManager.Save.configID_Bloom:
                case GameManager.Save.configID_ObscureFriends:
                    if (param == 0) {
                        config.state[index].text = TextManager.Get("CONFIG_OFF");
                    } else if (param == 1) {
                        config.state[index].text = TextManager.Get("CONFIG_WEAK");
                    } else if (param == 2) {
                        config.state[index].text = TextManager.Get("CONFIG_MEDIUM");
                    } else if (param == 3) {
                        config.state[index].text = TextManager.Get("CONFIG_STRONG");
                    }
                    break;
                case GameManager.Save.configID_LockonInFrontOfCamera:
                    if (param == 0) {
                        config.state[index].text = TextManager.Get("CONFIG_OFF");
                    } else {
                        config.state[index].text = sb.Clear().AppendFormat("{0}°", 90 - (param - 1) * 5).ToString();
                    }
                    break;
                case GameManager.Save.configID_CameraVibration:
                    if (param == 0) {
                        config.state[index].text = TextManager.Get("CONFIG_OFF");
                    } else if (param == 1) {
                        config.state[index].text = TextManager.Get("CONFIG_WEAK");
                    } else if (param == 2) {
                        config.state[index].text = TextManager.Get("CONFIG_STRONG");
                    }
                    break;
                case GameManager.Save.configID_ShowCursor:
                    if (param == 0) {
                        config.state[index].text = TextManager.Get("CONFIG_OFF");
                    } else if (param == 1) {
                        config.state[index].text = TextManager.Get("CONFIG_ON");
                    } else if (param == 2) {
                        config.state[index].text = TextManager.Get("CONFIG_ONPAUSE");
                    } else if (param == 3) {
                        config.state[index].text = TextManager.Get("CONFIG_ONPLAY");
                    }
                    break;
                case GameManager.Save.configID_Target:
                case GameManager.Save.configID_DamageSize:
                case GameManager.Save.configID_HpGaugeSize:
                case GameManager.Save.configID_FriendsIconSize:
                    if (param == 0) {
                        config.state[index].text = TextManager.Get("CONFIG_OFF");
                    } else {
                        config.state[index].text = sb.Clear().AppendFormat("×{0:F1}", 0.4f + param * 0.1f).ToString();
                    }
                    break;
                case GameManager.Save.configID_HpAlarm:
                case GameManager.Save.configID_StAlarm:
                    if (param == 0) {
                        config.state[index].text = TextManager.Get("CONFIG_OFF");
                    } else if (param == 1) {
                        config.state[index].text = TextManager.Get("CONFIG_QUIET");
                    } else if (param == 2) {
                        config.state[index].text = TextManager.Get("CONFIG_MEDIUM");
                    } else if (param == 3) {
                        config.state[index].text = TextManager.Get("CONFIG_LOUD");
                    }
                    break;
                case GameManager.Save.configID_MapPos:
                case GameManager.Save.configID_GoldPos:
                    config.state[index].text = TextManager.Get(sb.Clear().Append("CONFIG_MAPPOS_").Append(param).ToString());
                    break;
                case GameManager.Save.configID_ShowGiraffeBeam:
                case GameManager.Save.configID_DynamicBone:
                case GameManager.Save.configID_ClothSimulation:
                case GameManager.Save.configID_FaceToEnemy:
                case GameManager.Save.configID_SittingAction:
                    config.state[index].text = TextManager.Get(sb.Clear().Append("CONFIG_PLAYERONLY_").Append(param).ToString());
                    break;
                case GameManager.Save.configID_TrophyNotification:
                    if (param == 0) {
                        config.state[index].text = TextManager.Get("CONFIG_OFF");
                    } else if (param == 1) {
                        config.state[index].text = TextManager.Get("CONFIG_ON");
                    } else if (param == 2) {
                        config.state[index].text = TextManager.Get("CONFIG_ON_SEOFF");
                    }
                    break;
                case GameManager.Save.configID_ShowArms:
                    if (param == 0) {
                        config.state[index].text = TextManager.Get("CONFIG_OFF");
                    } else if (param == 1) {
                        config.state[index].text = TextManager.Get("CONFIG_ON");
                    } else if (param == 2) {
                        config.state[index].text = TextManager.Get("CONFIG_ON_SEOFF");
                    } else if (param == -1) {
                        config.state[index].text = TextManager.Get("CONFIG_OFF_ALLFRIENDS");
                    }
                    break;
                case GameManager.Save.configID_ClippingAutoAdjust:
                    if (param == 0) {
                        config.state[index].text = TextManager.Get("CONFIG_OFF");
                    } else if (param == 1) {
                        config.state[index].text = TextManager.Get("CONFIG_INDOOR");
                    } else if (param == 2) {
                        config.state[index].text = TextManager.Get("CONFIG_ALL");
                    } else if (param == 3) {
                        config.state[index].text = TextManager.Get("CONFIG_LOWCOST");
                    }
                    break;
                case GameManager.Save.configID_ShowGrass:
                    if (param == 0) {
                        config.state[index].text = TextManager.Get("CONFIG_OFF");
                    } else if (param == 1) {
                        config.state[index].text = TextManager.Get("CONFIG_ON");
                    } else if (param == 2) {
                        config.state[index].text = TextManager.Get("CONFIG_LOWDENSITY");
                    }
                    break;
                case GameManager.Save.configID_Antialiasing:
                case GameManager.Save.configID_AmbientOcclusion:
                case GameManager.Save.configID_DepthOfField:
                case GameManager.Save.configID_MotionBlur:
                    if (param == 0) {
                        config.state[index].text = TextManager.Get("CONFIG_OFF");
                    } else if (param == 1) {
                        config.state[index].text = TextManager.Get("CONFIG_LQ");
                    } else if (param == 2) {
                        config.state[index].text = TextManager.Get("CONFIG_HQ");
                    }
                    break;
                case GameManager.Save.configID_FieldOfView:
                case GameManager.Save.configID_Brightness:
                    if (param > 0) {
                        config.state[index].text = "+" + param.ToString();
                    } else {
                        config.state[index].text = param.ToString();
                    }
                    break;
                case GameManager.Save.configID_ClippingDistance:
                    config.state[index].text = sb.Clear().AppendFormat("{0:0.0}", param * 0.1f + 0.1f).Append(TextManager.Get("CONFIG_METER")).ToString();
                    break;
                case GameManager.Save.configID_SpeakerMode:
                    config.state[index].text = TextManager.Get(sb.Clear().Append("CONFIG_SPEAKER_").Append(param).ToString());
                    break;
                case GameManager.Save.configID_QualityLevel:
                    config.state[index].text = TextManager.Get(sb.Clear().Append("CONFIG_QUALITY_").Append(param).ToString());
                    break;
                case GameManager.Save.configID_SystemInformation:
                    if (param == 0) {
                        config.state[index].text = TextManager.Get("CONFIG_OFF");
                    } else {
                        config.state[index].text = TextManager.Get(sb.Clear().Append("CONFIG_SYSTEMINFO_").Append(param).ToString());
                    }
                    break;
                case GameManager.Save.configID_ScreenShotFileFormat:
                    config.state[index].text = TextManager.Get(sb.Clear().Append("CONFIG_FORMAT_").Append(param).ToString());
                    break;
                case GameManager.Save.configID_CursorLock:
                    if (param == 0) {
                        config.state[index].text = TextManager.Get("CONFIG_OFF");
                    } else if (param == 1) {
                        config.state[index].text = TextManager.Get("CONFIG_CURSORLOCK_1");
                    } else if (param == 2) {
                        config.state[index].text = TextManager.Get("CONFIG_CURSORLOCK_2");
                    }
                    break;
                case GameManager.Save.configID_GamepadVibration:
                    if (param == 0) {
                        config.state[index].text = TextManager.Get("CONFIG_OFF");
                    } else if (param == 1) {
                        config.state[index].text = TextManager.Get("CONFIG_DAMAGE");
                    } else if (param == 2) {
                        config.state[index].text = TextManager.Get("CONFIG_ON");
                    }
                    break;
                case GameManager.Save.configID_LevelLimit:
                    if (param == 0) {
                        config.state[index].text = TextManager.Get("CONFIG_OFF");
                    } else if (param >= 1) {
                        config.state[index].text = sb.Clear().Append(TextManager.Get("WORD_LEVEL")).Append(param).ToString();
                    } else {
                        config.state[index].text = TextManager.Get("CONFIG_AUTO");
                    }
                    break;
                case GameManager.Save.configID_PhotoMode:
                    if (param == 0) {
                        config.state[index].text = TextManager.Get("CONFIG_OFF");
                    } else if (param == 1) {
                        config.state[index].text = TextManager.Get("CONFIG_PHOTO_HIDEUI");
                    } else if (param == 2) {
                        config.state[index].text = TextManager.Get("CONFIG_PHOTO_SHOWUI");
                    }
                    break;
                case GameManager.Save.configID_RestingMotion:
                    config.state[index].text = TextManager.Get(sb.Clear().Append("CONFIG_REST_").Append(param).ToString());
                    break;
                case GameManager.Save.configID_ItemAutomaticUse:
                    if (param == 0) {
                        config.state[index].text = TextManager.Get("CONFIG_OFF");
                    } else if (param == 1) {
                        config.state[index].text = TextManager.Get("CONFIG_ON");
                    } else if (param == 2) {
                        config.state[index].text = TextManager.Get("CONFIG_HEALONLY");
                    }
                    break;
                case GameManager.Save.configID_FriendsWalkingSpeed:
                case GameManager.Save.configID_FriendsRunningSpeed:
                case GameManager.Save.configID_GameSpeed:
                    config.state[index].text = sb.Clear().AppendFormat("×{0:F2}", 1.0f + param * 0.01f).ToString();
                    break;
                case GameManager.Save.configID_MessageTimeRate:
                    config.state[index].text = sb.Clear().AppendFormat("×{0:F1}", 1.0f + param * 0.1f).ToString();
                    break;
                case GameManager.Save.configID_CameraAxisDefault:
                    if (param > 0) {
                        config.state[index].text = sb.Clear().AppendFormat("+{0}%", param * 5).ToString();
                    } else {
                        config.state[index].text = sb.Clear().AppendFormat("{0}%", param * 5).ToString();
                    }
                    break;
                case GameManager.Save.configID_CameraReturnSpeed:
                    if (param <= -10) {
                        config.state[index].text = TextManager.Get("CONFIG_OFF");
                    } else {
                        config.state[index].text = sb.Clear().AppendFormat("×{0:F1}", 1.0f + param * 0.1f).ToString();
                    }
                    break;
                default:
                    if (param == 0) {
                        config.state[index].text = TextManager.Get("CONFIG_OFF");
                    } else {
                        config.state[index].text = TextManager.Get("CONFIG_ON");
                    }
                    break;
            }
        } else {
            config.state[index].text = "";
        }
    }

    void SetInformationTextConfig(TMP_Text informationText, int type, int index) {
        informationText.text = TextManager.Get(sb.Clear().AppendFormat("CONFIG_INFO_{0}_{1}", type, index).ToString());
    }

    void SetConfigText() {
        SetConfigText_Name();
        for (int i = 0; i < config.choice.Length; i++) {
            SetConfigText_State(i);
        }
        SetInformationTextConfig(config.information, configType, config.cursorPos);
    }

    void ConfigControl() {
        int index = configStart[configType] + config.cursorPos + config.pageNow * Config.denomi;
        bool soundFlag = false;
        float moveInterval = 0.1f;
        switch (index) {
            case GameManager.Save.configID_LevelLimit:
                if (!equipChangeDisabled) {
                    moveInterval = 0.025f;
                }
                break;
            case GameManager.Save.configID_FriendsWalkingSpeed:
            case GameManager.Save.configID_FriendsRunningSpeed:
            case GameManager.Save.configID_FieldOfView:
            case GameManager.Save.configID_ClippingDistance:
            case GameManager.Save.configID_GameSpeed:
            case GameManager.Save.configID_MessageTimeRate:
            case GameManager.Save.configID_CameraAxisDefault:
            case GameManager.Save.configID_CameraReturnSpeed:
                moveInterval = 0.06f;
                break;
        }
        move = GameManager.Instance.MoveCursor(true, moveInterval, 0.1f);
        if (eventClickNum >= 200 && eventClickNum < 300) {
            move.x = (eventClickNum == 200 ? -1 : 1);
        } else if (mouseScroll != 0) {
            move.x = (mouseScroll < 0 ? -1 : 1);
        }
        if (config.pageMax > 1 && eventClickNum >= 300 && eventClickNum <= 301) {
            move.y = (eventClickNum == 300 ? -Config.denomi : Config.denomi);
        }
        if (equipChangeDisabled && index == GameManager.Save.configID_LevelLimit && move.x != 0) {
            UISE.Instance.Play(UISE.SoundName.error);
            move.x = 0;
        }
        if (move.x != 0) {
            soundFlag = true;
            int stateMin = 0;
            int stateMax = 2;
            switch (index) {
                case GameManager.Save.configID_FriendsWalkingSpeed:
                case GameManager.Save.configID_FriendsRunningSpeed:
                    stateMin = -50;
                    stateMax = 1;
                    break;
                case GameManager.Save.configID_SearchRadius:
                case GameManager.Save.configID_SearchRadiusBoss:
                    stateMax = 16;
                    break;
                case GameManager.Save.configID_CameraSensitivity:
                case GameManager.Save.configID_CameraFollowingSpeed:
                case GameManager.Save.configID_SuppressCameraTurning:
                    stateMax = 11;
                    break;
                case GameManager.Save.configID_FieldOfView:
                    stateMin = -30;
                    stateMax = 31;
                    break;
                case GameManager.Save.configID_LockonInFrontOfCamera:
                    stateMax = 16;
                    break;
                case GameManager.Save.configID_CameraAxisInvert:
                case GameManager.Save.configID_ShowCursor:
                case GameManager.Save.configID_ClippingAutoAdjust:
                case GameManager.Save.configID_Bloom:
                case GameManager.Save.configID_HpAlarm:
                case GameManager.Save.configID_StAlarm:
                case GameManager.Save.configID_SystemInformation:
                case GameManager.Save.configID_ScreenShotFileFormat:
                case GameManager.Save.configID_RestingMotion:
                case GameManager.Save.configID_ObscureFriends:
                    stateMax = 4;
                    break;
                case GameManager.Save.configID_CameraTurningSpeed:
                    stateMin = -1;
                    stateMax = 21;
                    break;
                case GameManager.Save.configID_Target:
                case GameManager.Save.configID_DamageSize:
                case GameManager.Save.configID_HpGaugeSize:
                case GameManager.Save.configID_FriendsIconSize:
                    stateMax = 12;
                    break;
                case GameManager.Save.configID_UseRunButton:
                case GameManager.Save.configID_LockonReset:
                case GameManager.Save.configID_CameraControlButton:
                case GameManager.Save.configID_CameraTargeting:
                case GameManager.Save.configID_CameraVibration:
                case GameManager.Save.configID_ShowGiraffeBeam:
                case GameManager.Save.configID_TrophyNotification:
                case GameManager.Save.configID_ShowGrass:
                case GameManager.Save.configID_DynamicBone:
                case GameManager.Save.configID_ClothSimulation:
                case GameManager.Save.configID_FaceToEnemy:
                case GameManager.Save.configID_Antialiasing:
                case GameManager.Save.configID_AmbientOcclusion:
                case GameManager.Save.configID_DepthOfField:
                case GameManager.Save.configID_MotionBlur:
                case GameManager.Save.configID_CursorLock:
                case GameManager.Save.configID_GamepadVibration:
                case GameManager.Save.configID_PhotoMode:
                case GameManager.Save.configID_ItemAutomaticUse:
                case GameManager.Save.configID_SittingAction:
                    stateMax = 3;
                    break;
                case GameManager.Save.configID_ShowArms:
                    stateMax = 3;
                    stateMin = -1;
                    break;
                case GameManager.Save.configID_SpeakerMode:
                case GameManager.Save.configID_QualityLevel:
                    stateMax = 6;
                    break;
                case GameManager.Save.configID_MapPos:
                case GameManager.Save.configID_GoldPos:
                    stateMax = 8;
                    break;
                case GameManager.Save.configID_Brightness:
                    stateMin = -10;
                    stateMax = 11;
                    break;
                case GameManager.Save.configID_ClippingDistance:
                    stateMax = 30;
                    break;
                case GameManager.Save.configID_LevelLimit:
                    stateMin = -1;
                    stateMax = GameManager.Instance.save.Level + 1;
                    break;
                case GameManager.Save.configID_GameSpeed:
                    stateMin = -20;
                    stateMax = 1;
                    break;
                case GameManager.Save.configID_MessageTimeRate:
                    stateMin = -9;
                    stateMax = 21;
                    break;
                case GameManager.Save.configID_CameraAxisDefault:
                    stateMin = -20;
                    stateMax = 21;
                    break;
                case GameManager.Save.configID_CameraReturnSpeed:
                    stateMin = -10;
                    stateMax = 31;
                    break;
            }
            int stateTemp = GameManager.Instance.save.config[index] + move.x;
            if (stateTemp < stateMin) {
                stateTemp = stateMax - 1;
            } else if (stateTemp >= stateMax) {
                stateTemp = stateMin;
            }
            GameManager.Instance.save.config[index] = stateTemp;
            SetConfigText_State(config.cursorPos);
            UpdateConfig(index);
        }
        if (eventEnterNum >= 100 && eventEnterNum < 100 + config.max && config.cursorPos != eventEnterNum - 100) {
            config.cursorPos = eventEnterNum - 100;
            config.cursorRect.anchoredPosition = config.origin + config.interval * config.cursorPos;
            SetInformationTextConfig(config.information, configType, Mathf.Clamp(config.pageNow * Config.denomi + config.cursorPos, 0, config.count));
            soundFlag = true;
        } else if (move.y != 0) {
            int movedPos = config.cursorPos + move.y;
            if (config.pageMax > 1 && (movedPos < 0 || movedPos >= config.max)) {
                config.pageNow = (config.pageNow + config.pageMax + (movedPos < 0 ? -1 : 1)) % config.pageMax;
                config.max = (config.pageNow + 1 >= config.pageMax ? (config.count - 1) % Config.denomi + 1 : Config.denomi);
                config.cursorPos = (movedPos < 0 ? config.max - 1 : 0);
                config.SetPageText();
                SetConfigText();
            } else {
                config.cursorPos = (movedPos + config.max) % config.max;
            }
            config.cursorRect.anchoredPosition = config.origin + config.interval * config.cursorPos;
            SetInformationTextConfig(config.information, configType, Mathf.Clamp(config.pageNow * Config.denomi + config.cursorPos, 0, config.count));
            soundFlag = true;
        }
        if (soundFlag) {
            UISE.Instance.Play(UISE.SoundName.move);
        }
        if (GameManager.Instance.GetCancelButtonDown) {
            property.canvas.enabled = true;
            property.gridLayoutGroup.enabled = true;
            SetConfigCanvasEnabled(false);
            state = 1;
            UISE.Instance.Play(UISE.SoundName.cancel);
        }
    }

    void UpdateConfig(int index) {
        switch (index) {
            case GameManager.Save.configID_Blinking:
                if (CharacterManager.Instance) {
                    CharacterManager.Instance.ResetFaciemBlinkAll();
                }
                break;
            case GameManager.Save.configID_CameraSensitivity:
                if (CameraManager.Instance) {
                    CameraManager.Instance.SetFreeLookSpeed(GameManager.Instance.save.config[GameManager.Save.configID_CameraSensitivity]);
                }
                break;
            case GameManager.Save.configID_CameraControlButton:
                if (CameraManager.Instance) {
                    CameraManager.Instance.SetActiveTargetGroupCamera(false);
                }
                break;
            case GameManager.Save.configID_CameraTurningSpeed:
                if (CameraManager.Instance) {
                    CameraManager.Instance.SetHorizontalDamping(GameManager.Instance.save.config[GameManager.Save.configID_CameraTurningSpeed]);
                }
                break;
            case GameManager.Save.configID_FieldOfView:
                if (CameraManager.Instance) {
                    CameraManager.Instance.UpdateFOV();
                }
                break;
            case GameManager.Save.configID_Gauge:
                if (CanvasCulling.Instance) {
                    CanvasCulling.Instance.CheckConfig();
                }
                break;
            case GameManager.Save.configID_ShowArms:
                if (CanvasCulling.Instance) {
                    CanvasCulling.Instance.CheckConfig();
                }
                SetShowArms();
                break;
            case GameManager.Save.configID_ShowGiraffeBeam:
                if (CharacterManager.Instance) {
                    CharacterManager.Instance.UpdateSandstarReady();
                }
                break;
            case GameManager.Save.configID_DamageSize:
                if (CharacterManager.Instance) {
                    CharacterManager.Instance.ChangeDamageSize();
                }
                break;
            case GameManager.Save.configID_HpGaugeSize:
                if (CharacterManager.Instance) {
                    CharacterManager.Instance.UpdateHPSTCanvasSize();
                }
                break;
            case GameManager.Save.configID_FriendsIconSize:
                if (CharacterManager.Instance) {
                    CharacterManager.Instance.UpdateFriendsGridScale();
                }
                break;
            case GameManager.Save.configID_MapPos:
                if (CharacterManager.Instance) {
                    CharacterManager.Instance.UpdateMapCameraRect();
                }
                break;
            case GameManager.Save.configID_GoldPos:
                if (CharacterManager.Instance) {
                    CharacterManager.Instance.UpdateGoldPos();
                }
                break;
            case GameManager.Save.configID_ShowGrass:
                if (CanvasCulling.Instance) {
                    CanvasCulling.Instance.CheckConfig();
                }
                ResetGrassControl();
                break;
            case GameManager.Save.configID_DynamicBone:
                if (CharacterManager.Instance) {
                    CharacterManager.Instance.SetAllFriendsDynamicBone(GameManager.Instance.save.config[GameManager.Save.configID_DynamicBone]);
                }
                break;
            case GameManager.Save.configID_ClothSimulation:
                if (CharacterManager.Instance) {
                    CharacterManager.Instance.SetAllFriendsCloth(GameManager.Instance.save.config[GameManager.Save.configID_ClothSimulation]);
                }
                break;
            case GameManager.Save.configID_FaceToEnemy:
                if (CharacterManager.Instance) {
                    CharacterManager.Instance.SetAllFriendsHeadLook(GameManager.Instance.save.config[GameManager.Save.configID_FaceToEnemy]);
                }
                break;
            case GameManager.Save.configID_Antialiasing:
            case GameManager.Save.configID_AmbientOcclusion:
            case GameManager.Save.configID_DepthOfField:
            case GameManager.Save.configID_MotionBlur:
            case GameManager.Save.configID_Bloom:
            case GameManager.Save.configID_ColorGrading:
            case GameManager.Save.configID_Brightness:
                if (InstantiatePostProcessingProfile.Instance) {
                    InstantiatePostProcessingProfile.Instance.QualitySettingsAdjustments();
                }
                break;
            case GameManager.Save.configID_SpeakerMode:
                if (GameManager.Instance.ApplySpeakerMode()) {
                    GameManager.Instance.ApplyVolume();
                    replayAppointment = 0.2f;
                }
                break;
            case GameManager.Save.configID_AcousticEffects:
                if (StageManager.Instance && StageManager.Instance.dungeonController) {
                    StageManager.Instance.dungeonController.SetDefaultSnapshot();
                }
                break;
            case GameManager.Save.configID_RunInBackground:
                GameManager.Instance.save.SetRunInBackground();
                break;
            case GameManager.Save.configID_QualityLevel:
                QualitySettings.SetQualityLevel(GameManager.Instance.save.config[GameManager.Save.configID_QualityLevel], true);
                break;
            case GameManager.Save.configID_SystemInformation:
                if (inGameProfiler) {
                    inGameProfiler.SetProfilerEnabled(GameManager.Instance.save.config[GameManager.Save.configID_SystemInformation] != 0);
                }
                break;
            case GameManager.Save.configID_UseMouseForUI:
                property.gRaycaster.enabled = (GameManager.Instance.save.config[GameManager.Save.configID_UseMouseForUI] != 0);
                break;
            case GameManager.Save.configID_CameraAxisDefault:
                if (CameraManager.Instance) {
                    CameraManager.Instance.SetFreeLookYAxisToDefault();
                }
                break;
        }
        if (CameraManager.Instance) {
            switch (index) {
                case GameManager.Save.configID_Antialiasing:
                case GameManager.Save.configID_AmbientOcclusion:
                case GameManager.Save.configID_DepthOfField:
                case GameManager.Save.configID_MotionBlur:
                case GameManager.Save.configID_QualityLevel:
                    CameraManager.Instance.CheckDepthTextureMode();
                    break;
            }
        }
    }

    public void SetNumOfChoices(int num) {
        RectTransform canvasRect = choices.canvas.GetComponent<RectTransform>();
        if (canvasRect) {
            if (num <= 3) {
                canvasRect.anchoredPosition = new Vector2(0f, 0f);
            } else {
                canvasRect.anchoredPosition = new Vector2(0f, 15f * (num - 3));
            }
        }
        choices.panel.sizeDelta = new Vector2(choices.panel.sizeDelta.x, 140f + 60f * num);
        choices.panel.anchoredPosition = new Vector2(0f, 75f - 25f * num);
        for (int i = num; i < choices.choice.Length; i++) {
            choices.choice[i].text = string.Empty;
        }
        choices.max = num;
        choices.cursorPos = 0;
        choices.cursorRect.anchoredPosition = choices.origin;
    }

    void SetInformationText(TMP_Text nameText, TMP_Text informationText) {
        switch (typeCursor) {
            case (int)Type.Trophy:
                if (true) {
                    int trophyIndex = trophyPage * 32 + itemCursor;
                    if (itemCursor >= 0 && itemCursor < numSlots && slotItemId[itemCursor] >= 0) {
                        nameText.text = TextManager.Get("TROPHY_NAME_" + (trophyIndex + 1).ToString("000"));
                        informationText.text = TextManager.Get("TROPHY_INFO_" + (trophyIndex + 1).ToString("000"));
                    } else {
                        nameText.text = TextManager.Get("TROPHY_LOCK");
                        if (TrophyManager.Instance.GetTrophyInfoCondition(trophyIndex)) {
                            informationText.text = TextManager.Get("TROPHY_INFO_" + (trophyIndex + 1).ToString("000"));
                        } else {
                            informationText.text = TextManager.Get("TROPHY_LOCK");
                        }
                    }
                }
                break;
            case (int)Type.FDKaban:
                if (itemCursor >= 0 && itemCursor < numSlots && slotItemId[itemCursor] >= 0) {
                    nameText.text = TextManager.Get(string.Format("DIC_FACILITY_NAME_{0:D2}", itemCursor));
                } else {
                    nameText.text = "";
                }
                informationText.text = "";
                break;
            default:
                if (itemCursor >= 0 && itemCursor < numSlots && slotItemId[itemCursor] >= 0) {
                    int id = slotItemId[itemCursor];
                    nameText.text = ItemDatabase.Instance.GetItemName(id);
                    if (id == 402) {
                        informationText.text = sb.Clear().Append(ItemDatabase.Instance.GetItemInfomation(id)).AppendLine().Append(TextManager.Get("CONFIG_CURRENT")).Append(TextManager.Get("LANGUAGE_" + GameManager.Instance.save.language.ToString())).ToString();
                    } else {
                        informationText.text = ItemDatabase.Instance.GetItemInfomation(id);
                    }
                } else {
                    nameText.text = "";
                    informationText.text = "";
                }
                break;
        }
    }

    void SetSlotObjectColor(bool selected) {
        for (int i = 0; i < numSlots; i++) {
            if (slotObject[i] != null) {
                slotObject[i].GetComponent<Image>().color = selected && i == itemCursor ? new Color(1, 1, 0, 1) : new Color(1, 1, 1, 100.0f / 255.0f);
            }
        }
    }

    void UpdateFriendsMark() {
        for (int i = 0; i < slotMax; i++) {
            if (i < GameManager.Instance.save.friends.Length && GameManager.Instance.save.friends[i] != 0 && slotObject[i] != null && markObject[i] == null && CharacterManager.Instance.IsFriendsActive(i)) {
                markObject[i] = Instantiate(CharacterManager.Instance.GetFriendsExist(i, true) ? friendsActPrefab : friendsDeadPrefab, slotObject[i].transform);
            } else if (markObject[i] != null) {
                Destroy(markObject[i]);
                markObject[i] = null;
            }
        }
    }

    void UpdateEquipedMark() {
        bool[] isSpecial = new bool[slotMax];
        if (isSpecial.Length > GameManager.invUpIndex) {
            isSpecial[GameManager.hpUpIndex] = (GameManager.Instance.save.GotHpUp < GameManager.Instance.save.GotHpUpOriginal);
            isSpecial[GameManager.stUpIndex] = (GameManager.Instance.save.GotStUp < GameManager.Instance.save.GotStUpOriginal);
            isSpecial[GameManager.invUpIndex] = (GameManager.Instance.save.GotInvUp < GameManager.Instance.save.GotInvUpOriginal);
        }
        for (int i = 0; i < slotMax; i++) {
            if (i < GameManager.weaponMax && GameManager.Instance.save.weapon[i] != 0 && GameManager.Instance.save.equip[i] != 0) {
                if (slotObject[i] && !markObject[i]) {
                    markObject[i] = Instantiate(equipedPrefab, slotObject[i].transform);
                    if (lockObject[i]) {
                        lockObject[i].transform.SetAsLastSibling();
                    }
                }
                if (markObject[i]) {
                    markObject[i].GetComponent<RectTransform>().anchoredPosition = isSpecial[i] ? vec2SpecialPos : vec2Zero;
                }
            } else if (markObject[i]) {
                Destroy(markObject[i]);
                markObject[i] = null;
            }
            if (GameManager.Instance.save.IsWeaponLocked(i)) {
                if (slotObject[i] && !lockObject[i]) {
                    lockObject[i] = Instantiate(lockedPrefab, slotObject[i].transform);
                    lockObject[i].transform.SetAsLastSibling();
                }
            } else {
                if (lockObject[i]) {
                    Destroy(lockObject[i]);
                    lockObject[i] = null;
                }
            }
        }
    }

    void UpdateSoldOutMark() {
        buyItemSoldOut = new bool[slotMax];
        for (int i = 0; i < slotMax; i++) {
            bool soldOut = false;
            switch (slotItemId[i]) {
                case GameManager.hpUpId:
                    if (GameManager.Instance.save.hpUpSale >= GameManager.hpUpSaleMax) {
                        soldOut = true;
                    }
                    break;
                case GameManager.stUpId:
                    if (GameManager.Instance.save.stUpSale >= GameManager.stUpSaleMax) {
                        soldOut = true;
                    }
                    break;
                default:
                    if (slotItemId[i] >= GameManager.armsIDBase && slotItemId[i] < GameManager.armsIDBase + GameManager.weaponMax && GameManager.Instance.save.weapon[slotItemId[i] % 100] != 0) {
                        soldOut = true;
                    }
                    break;
            }
            if (soldOut) {
                if (slotObject[i] && !markObject[i]) {
                    markObject[i] = Instantiate(soldOutPrefab, slotObject[i].transform);
                }
            } else if (markObject[i]) {
                Destroy(markObject[i]);
                markObject[i] = null;
            }
            buyItemSoldOut[i] = soldOut;
        }
    }

    void CreateSlot() {
        DeleteSlot();
        int[] slotCount = new int[slotMax];
        int slotContentsCount = 0;
        bool isAnother = GameManager.Instance.IsPlayerAnother;
        Transform slotParent;
        if (typeCursor == (int)Type.Buy || typeCursor == (int)Type.Sell) {
            slotParent = shop.panel;
        } else if (typeCursor == (int)Type.Store || typeCursor == (int)Type.TakeOut) {
            slotParent = storage.panel;
        } else {
            slotParent = property.panelItem;
        }
        switch (typeCursor) {
            case (int)Type.Item:
                slotContentsCount = GameManager.Instance.save.items.Count;
                for (int i = 0; i < slotMax; i++) {
                    if (i < slotContentsCount) {
                        slotItemId[i] = GameManager.Instance.save.items[i];
                    } else {
                        slotItemId[i] = -1;
                    }
                }
                numSlots = GameManager.Instance.save.InventoryMax;
                break;
            case (int)Type.Friends:
                slotContentsCount = GameManager.Instance.save.friends.Length;
                for (int i = 0; i < slotMax; i++) {
                    if (i < slotContentsCount && GameManager.Instance.save.friends[i] != 0) {
                        slotItemId[i] = 100 + i;
                        if (i > 0) {
                            slotCount[i] = CharacterManager.Instance.GetFriendsCost(i);
                        }
                    } else {
                        slotItemId[i] = -1;
                    }
                }
                numSlots = GameManager.friendsMax;
                break;
            case (int)Type.Weapon:
                slotContentsCount = GameManager.Instance.save.weapon.Length;
                for (int i = 0; i < slotMax; i++) {
                    if (i < slotContentsCount && GameManager.Instance.save.weapon[i] != 0) {
                        slotItemId[i] = GameManager.armsIDBase + i;
                        if (isAnother) {
                            for (int j = 0; j < anotherIconID.Length; j++) {
                                if (slotItemId[i] == anotherIconID[j]) {
                                    slotItemId[i] += anotherWeaponGap;
                                    break;
                                }
                            }
                        }
                        switch (slotItemId[i]) {
                            case GameManager.hpUpId:
                                slotCount[i] = GameManager.Instance.save.GotHpUpOriginal;
                                break;
                            case GameManager.stUpId:
                                slotCount[i] = GameManager.Instance.save.GotStUpOriginal;
                                break;
                            case GameManager.invUpId:
                                slotCount[i] = GameManager.Instance.save.GotInvUpOriginal;
                                break;
                        }
                    } else {
                        slotItemId[i] = -1;
                    }
                }
                numSlots = GameManager.weaponMax;
                break;
            case (int)Type.Status:
                for (int i = 0; i < slotMax; i++) {
                    slotItemId[i] = -1;
                }
                numSlots = 0;
                break;
            case (int)Type.Config:
                for (int i = 0; i < slotMax; i++) {
                    if (i < configItemMax) {
                        slotItemId[i] = 400 + i;
                    } else {
                        slotItemId[i] = -1;
                    }
                }
                numSlots = configItemMax;
                break;
            case (int)Type.Buy:
                SetShopItem();
                SetPriceText();
                break;
            case (int)Type.Sell:
                slotContentsCount = GameManager.Instance.save.items.Count;
                for (int i = 0; i < slotMax; i++) {
                    if (i < slotContentsCount) {
                        slotItemId[i] = GameManager.Instance.save.items[i];
                        buyItemPrice[i] = ItemDatabase.Instance.GetItemPrice(slotItemId[i]) / 2;
                    } else {
                        slotItemId[i] = -1;
                        buyItemPrice[i] = -1;
                        buyItemSoldOut[i] = false;
                    }
                }
                numSlots = GameManager.Instance.save.InventoryMax;
                break;
            case (int)Type.Store:
                slotContentsCount = GameManager.Instance.save.items.Count;
                for (int i = 0; i < slotMax; i++) {
                    if (i < slotContentsCount) {
                        slotItemId[i] = GameManager.Instance.save.items[i];
                    } else {
                        slotItemId[i] = -1;
                    }
                }
                numSlots = GameManager.Instance.save.InventoryMax;
                break;
            case (int)Type.TakeOut:
                slotContentsCount = GameManager.Instance.save.storage.Count;
                for (int i = 0; i < slotMax; i++) {
                    int indexTemp = i + storagePage * slotMax;
                    if (indexTemp < slotContentsCount) {
                        slotItemId[i] = GameManager.Instance.save.storage[indexTemp];
                    } else {
                        slotItemId[i] = -1;
                    }
                }
                numSlots = Mathf.Min(slotMax, GameManager.storageMax - storagePage * slotMax);
                break;
            case (int)Type.Trophy:
                numSlots = slotMax;
                for (int i = 0; i < slotMax; i++) {
                    int index = trophyPage * 32 + i;
                    if (index < GameManager.trophyMax) {
                        if (TrophyManager.Instance.IsTrophyHad(index)) {
                            slotItemId[i] = ItemDatabase.trophyID[TrophyManager.Instance.trophyRanks[index]];
                            slotCount[i] = index + 1;
                        } else {
                            slotItemId[i] = -1;
                        }
                    } else {
                        numSlots = i;
                        break;
                    }
                }
                break;
            case (int)Type.FDKaban:
                numSlots = fdKabanIDArray.Length;
                for (int i = 0; i < slotMax; i++) {
                    if (i < fdKabanIDArray.Length) {
                        slotItemId[i] = ItemDatabase.facilityBottom + fdKabanIDArray[i];
                    } else {
                        slotItemId[i] = -1;
                    }
                }
                break;
        }
        for (int i = 0; i < numSlots; i++) {
            slotObject[i] = Instantiate(property.slotPrefab, slotParent);
            slotObject[i].GetComponent<EventTriggerSlot>().slotIndex = i;
            Image image = slotObject[i].transform.GetChild(0).GetComponent<Image>();
            if (slotItemId[i] >= 0) {
                image.sprite = ItemDatabase.Instance.itemList.Find(n => n.id == slotItemId[i]).image;
                if (slotCount[i] > 0) {
                    TMP_Text countText = slotObject[i].transform.GetChild(1).GetComponent<TMP_Text>();
                    if (countText != null) {
                        countText.enabled = true;
                        bool assigned = false;
                        switch (slotItemId[i]) {
                            case GameManager.hpUpId:
                                if (GameManager.Instance.save.GotHpUp < GameManager.Instance.save.equipedHpUp) {
                                    countText.text = sb.Clear().Append("<color=#FFBBBB>").Append(GameManager.Instance.save.GotHpUp).Append("</color>").Append(slashTight).Append(slotCount[i]).ToString();
                                    assigned = true;
                                } else if (GameManager.Instance.save.equipedHpUp < GameManager.Instance.save.GotHpUpOriginal) {
                                    countText.text = sb.Clear().Append(GameManager.Instance.save.equipedHpUp).Append(slashTight).Append(slotCount[i]).ToString();
                                    assigned = true;
                                }
                                break;
                            case GameManager.stUpId:
                                if (GameManager.Instance.save.GotStUp < GameManager.Instance.save.equipedStUp) {
                                    countText.text = sb.Clear().Append("<color=#FFBBBB>").Append(GameManager.Instance.save.GotStUp).Append("</color>").Append(slashTight).Append(slotCount[i]).ToString();
                                    assigned = true;
                                } else if (GameManager.Instance.save.equipedStUp < GameManager.Instance.save.GotStUpOriginal) {
                                    countText.text = sb.Clear().Append(GameManager.Instance.save.equipedStUp).Append(slashTight).Append(slotCount[i]).ToString();
                                    assigned = true;
                                }
                                break;
                            case GameManager.invUpId:
                                if (GameManager.Instance.save.GotInvUp < GameManager.Instance.save.equipedInvUp) {
                                    countText.text = sb.Clear().Append("<color=#FFBBBB>").Append(GameManager.Instance.save.GotInvUp).Append("</color>").Append(slashTight).Append(slotCount[i]).ToString();
                                    assigned = true;
                                } else if (GameManager.Instance.save.equipedInvUp < GameManager.Instance.save.GotInvUpOriginal) {
                                    countText.text = sb.Clear().Append(GameManager.Instance.save.equipedInvUp).Append(slashTight).Append(slotCount[i]).ToString();
                                    assigned = true;
                                }
                                break;
                        }
                        if (!assigned) {
                            countText.text = slotCount[i].ToString();
                        }
                    }
                }
                if (typeCursor == (int)Type.Friends && slotItemId[i] == 100) {
                    TMP_Text limitText = slotObject[i].transform.GetChild(2).GetComponent<TMP_Text>();
                    if (limitText != null) {
                        int limit = CharacterManager.Instance.GetFriendsLimit(true);
                        int costSum = CharacterManager.Instance.GetFriendsCostSum();
                        int summonRemain = limit - costSum;
                        if (limit >= CharacterManager.riskyCost && summonRemain < 0) {
                            limitText.colorGradientPreset = property.limitColorOver;
                            limitText.text = costSum.ToString();
                        } else if (summonRemain <= 0) {
                            limitText.colorGradientPreset = property.limitColorZero;
                            limitText.text = summonRemain.ToString();
                        } else if (summonRemain < limit) {
                            limitText.colorGradientPreset = property.limitColorMiddle;
                            limitText.text = summonRemain.ToString();
                        } else {
                            limitText.colorGradientPreset = property.limitColorFull;
                            limitText.text = summonRemain.ToString();
                        }
                        limitText.enabled = true;
                    }
                }
                if (typeCursor == (int)Type.Trophy) {
                    int index = trophyPage * 32 + i;
                    if (index < GameManager.trophyMax && TrophyManager.Instance.trophyIsNew[index]) {
                        TMP_Text limitText = slotObject[i].transform.GetChild(2).GetComponent<TMP_Text>();
                        if (limitText != null) {
                            limitText.fontSize = 28;
                            limitText.colorGradientPreset = property.limitColorOver;
                            limitText.text = TextManager.Get("TROPHY_NEW");
                        }
                    }
                }
            } else {
                image.sprite = empty;
            }
        }
        if (typeCursor == (int)Type.Friends) {
            UpdateFriendsMark();
        } else if (typeCursor == (int)Type.Weapon) {
            UpdateEquipedMark();
        } else if (typeCursor == (int)Type.Buy) { 
            UpdateSoldOutMark();
        } else if (typeCursor == (int)Type.Sell) {
            MakeItemCursor(true, 1);
            SetPriceText();
        } else if (typeCursor == (int)Type.Store) {
            MakeItemCursor(true, 2);
        } else if (typeCursor == (int)Type.TakeOut) {
            MakeItemCursor(true, 2);
        }
        MakeItemCursor(false);
    }

    void DeleteSlot() {
        for (int i = 0; i < slotMax; i++) {
            if (slotObject[i] != null) {
                Destroy(slotObject[i]);
                slotObject[i] = null;
            }
            if (markObject[i] != null) {
                Destroy(markObject[i]);
                markObject[i] = null;
            }
            if (lockObject[i] != null) {
                Destroy(lockObject[i]);
                lockObject[i] = null;
            }
        }
    }

    void SetStatusTextChild_Player() {
        TMP_Text slotText = null;
        Image slotImage = null;
        int now;
        int max;
        int maxNat;
        float nowF;
        float maxF;
        float maxNatF;
        for (int i = 0; i < 22; i++) {
            if (i == 21) {
                GameObject slotObj = Instantiate(statusTextImagePrefab, property.panelStatus);
                slotText = slotObj.GetComponent<TMP_Text>();
                slotImage = slotObj.GetComponentInChildren<Image>();
            } else {
                slotText = Instantiate(statusTextPrefab, property.panelStatus).GetComponent<TMP_Text>();
            }
            switch (i) {
                case 0:
                    slotText.text = TextManager.Get("STATUS_LEVEL");
                    break;
                case 1:
                    slotText.text = GameManager.Instance.save.Level.ToString();
                    break;
                case 2:
                    slotText.text = TextManager.Get("STATUS_NEXTLEVEL");
                    break;
                case 3:
                    slotText.text = sb.Clear().Append(GameManager.Instance.save.NowLevelExp).Append(slash).Append(GameManager.Instance.save.NeedExpToNextLevel).ToString();
                    break;
                case 4:
                    slotText.text = TextManager.Get("STATUS_HP");
                    break;
                case 5:
                    now = pCon.GetNowHP();
                    max = pCon.GetMaxHP();
                    maxNat = pCon.GetMaxHPNoEffected();
                    slotText.text = sb.Clear()
                        .Append(now <= 0 ? colorTagGray : now <= max / 5 ? colorTagRed : colorTagWhite)
                        .Append(now)
                        .Append(colorTagEnd)
                        .Append(slash)
                        .Append(max > maxNat ? colorTagYellow : colorTagWhite)
                        .Append(max)
                        .Append(colorTagEnd).ToString();
                    break;
                case 6:
                    slotText.text = TextManager.Get("STATUS_ST");
                    break;
                case 7:
                    nowF = pCon.GetNowST();
                    maxF = pCon.GetMaxST();
                    maxNatF = pCon.GetMaxSTNoEffected();
                    slotText.text = sb.Clear()
                        .Append(nowF.ToString("0"))
                        .Append(slash)
                        .Append(maxF > maxNatF ? colorTagYellow : colorTagWhite)
                        .Append(maxF.ToString("0"))
                        .Append(colorTagEnd).ToString();
                    break;
                case 8:
                    slotText.text = TextManager.Get("STATUS_SANDSTAR");
                    break;
                case 9:
                    slotText.text = sb.Clear().AppendFormat("{0:0.00} / {1}", CharacterManager.Instance.GetSandstar(), GameManager.Instance.save.SandstarMax).ToString();
                    break;
                case 10:
                    slotText.text = TextManager.Get("STATUS_ATTACK");
                    break;
                case 11:
                    maxF = pCon.GetAttack(true);
                    maxNatF = pCon.GetAttackNoEffected();
                    slotText.text = sb.Clear()
                        .Append(maxF > maxNatF ? colorTagYellow : maxF < maxNatF ? colorTagRed : colorTagWhite)
                        .Append(maxF.ToString("0.00"))
                        .Append(colorTagEnd).ToString();
                    break;
                case 12:
                    slotText.text = TextManager.Get("STATUS_DEFENSE");
                    break;
                case 13:
                    maxF = pCon.GetDefense(true);
                    maxNatF = pCon.GetDefenseNoEffected();
                    slotText.text = sb.Clear()
                        .Append(maxF > maxNatF ? colorTagYellow : maxF < maxNatF ? colorTagRed : colorTagWhite)
                        .Append(maxF.ToString("0.00"))
                        .Append(colorTagEnd).ToString();
                    break;
                case 14:
                    slotText.text = TextManager.Get("STATUS_GOLD");
                    break;
                case 15:
                    slotText.text = GameManager.Instance.save.money.ToString();
                    break;
                case 16:
                    slotText.text = TextManager.Get("STATUS_LOCATION");
                    break;
                case 17:
                    sb.Clear();
                    if (SaveController.Instance && SaveController.Instance.IsSpecialFloor(StageManager.Instance.stageNumber, StageManager.Instance.floorNumber)) {
                        sb.Append(TextManager.Get(string.Format("SAVEFLOOR_{0:00}_{1:00}", StageManager.Instance.stageNumber, StageManager.Instance.floorNumber)));
                    } else {
                        sb.Append(TextManager.Get(string.Format("STAGE_{0:00}", StageManager.Instance.stageNumber)));
                        if (StageManager.Instance.floorNumber + 1 > GameManager.Instance.GetDenotativeMaxFloor(StageManager.Instance.stageNumber)) {
                            sb.Append(" ?");
                        } else {
                            sb.Append(" ");
                            sb.Append((StageManager.Instance.floorNumber + 1).ToString());
                        }
                        sb.Append(TextManager.Get("WORD_FLOOR"));
                    }
                    slotText.text = sb.ToString();
                    break;
                case 18:
                    slotText.text = TextManager.Get("WORD_PLAYTIME");
                    break;
                case 19:
                    slotText.text = sb.Clear().Append((GameManager.Instance.save.totalPlayTime / 3600).ToString("0")).Append(":").Append((GameManager.Instance.save.totalPlayTime % 3600 / 60).ToString("00")).Append("\'").Append((GameManager.Instance.save.totalPlayTime % 60).ToString("00")).Append("\"").ToString();
                    break;
                case 20:
                    slotText.text = TextManager.Get("WORD_DIFFICULTY");
                    break;
                case 21:
                    if (GameManager.Instance.save.difficulty >= 1) {
                        slotImage.sprite = difficultySprites[GameManager.Instance.save.difficulty - 1];
                    }
                    slotText.text = sb.Clear().AppendFormat("   {0}", TextManager.Get("CONFIG_DIF_NAME_" + GameManager.Instance.save.difficulty.ToString())).ToString();
                    break;
            }
        }
    }

    void SetStatusTextChild_Friends(int index) {
        int now;
        int max;
        int maxNat;
        float nowF;
        float maxF;
        float maxNatF;
        if (index >= 0 && index < GameManager.friendsMax) {
            fBase = CharacterManager.Instance.friends[index].fBase;
            if (fBase) {
                TMP_Text slotText = null;
                for (int i = 0; i < 16; i++) {
                    slotText = Instantiate(statusTextPrefab, property.panelStatus).GetComponent<TMP_Text>();
                    switch (i) {
                        case 0:
                            slotText.text = TextManager.Get(sb.Clear().Append("ITEM_NAME_").Append((index + 100).ToString("000")).ToString());
                            break;
                        case 1:
                            slotText.text = string.Empty;
                            break;
                        case 2:
                            slotText.text = TextManager.Get("STATUS_HP");
                            break;
                        case 3:
                            now = fBase.GetNowHP();
                            max = fBase.GetMaxHP();
                            maxNat = fBase.GetMaxHPNoEffected();
                            slotText.text = sb.Clear()
                                .Append(now <= 0 ? colorTagGray : now <= max / 5 ? colorTagRed : colorTagWhite)
                                .Append(now)
                                .Append(colorTagEnd)
                                .Append(slash)
                                .Append(max > maxNat ? colorTagYellow : colorTagWhite)
                                .Append(max)
                                .Append(colorTagEnd).ToString();
                            break;
                        case 4:
                            slotText.text = TextManager.Get("STATUS_ST");
                            break;
                        case 5:
                            nowF = fBase.GetNowST();
                            maxF = fBase.GetMaxST();
                            maxNatF = fBase.GetMaxSTNoEffected();
                            slotText.text = sb.Clear()
                                .Append(nowF.ToString("0"))
                                .Append(slash)
                                .Append(maxF > maxNatF ? colorTagYellow : colorTagWhite)
                                .Append(maxF.ToString("0"))
                                .Append(colorTagEnd).ToString();
                            break;
                        case 6:
                            slotText.text = TextManager.Get("STATUS_ATTACK");
                            break;
                        case 7:
                            maxF = fBase.GetAttack(true);
                            maxNatF = fBase.GetAttackNoEffected();
                            slotText.text = sb.Clear()
                                .Append(maxF > maxNatF ? colorTagYellow : maxF < maxNatF ? colorTagRed : colorTagWhite)
                                .Append(maxF.ToString("0.00"))
                                .Append(colorTagEnd).ToString();
                            break;
                        case 8:
                            slotText.text = TextManager.Get("STATUS_DEFENSE");
                            break;
                        case 9:
                            maxF = fBase.GetDefense(true);
                            maxNatF = fBase.GetDefenseNoEffected();
                            slotText.text = sb.Clear()
                                .Append(maxF > maxNatF ? colorTagYellow : maxF < maxNatF ? colorTagRed : colorTagWhite)
                                .Append(maxF.ToString("0.00"))
                                .Append(colorTagEnd).ToString();
                            break;
                        case 10:
                            slotText.text = TextManager.Get("STATUS_DODGE");
                            break;
                        case 11:
                            nowF = fBase.GetDodgeRemain();
                            maxF = fBase.GetDodgePower();
                            /*
                            slotText.text = sb.Clear()
                                .Append((Mathf.Floor(nowF * 10f) * 0.1f).ToString("0.0"))
                                .Append(slash)
                                .Append(maxF.ToString("0"))
                                //.AppendFormat(" ({0}%)", (nowF <= 0 || maxF <= 0) ? 0 : nowF >= maxF ? 100 : Mathf.Clamp((int)(nowF * 100 / maxF), 1, 99))
                                .AppendFormat(" ({0}%)", (nowF <= 0 || maxF <= 0) ? 0 : nowF >= 10f ? 100 : Mathf.Clamp((int)(nowF * 100 / 10f), 1, 99))
                                .ToString();
                                */
                            slotText.text = sb.Clear()
                            .Append(nowF > maxF ? colorTagYellow : colorTagWhite)
                            .AppendFormat("{0}%", Mathf.RoundToInt(nowF * 10f * CharacterManager.Instance.riskyDecSqrt))
                            .Append(colorTagEnd)
                            .ToString();
                            break;
                        case 12:
                            slotText.text = TextManager.Get("STATUS_GUTS");
                            break;
                        case 13:
                            now = fBase.GetGutsPercent();
                            slotText.text = sb.Clear()
                                .Append(now <= 0 ? colorTagRed : colorTagWhite)
                                .Append(now)
                                .Append("%")
                                .Append(colorTagEnd).ToString();
                            break;
                            /*
                        case 14:
                            slotText.text = TextManager.Get("STATUS_GUTSCONDITION");
                            break;
                        case 15:
                            slotText.text = CharacterManager.Instance.GetGutsBorder().ToString();
                            break;
                            */
                    }
                }
            }
        }
    }

    void SetStatusText(bool show, int page = 0) {
        int count = property.panelStatus.childCount;
        if (count > 0) {
            for (int i = count - 1; i >= 0; i--) {
                Destroy(property.panelStatus.GetChild(i).gameObject);
            }
        }
        if (show && pCon) {
            if (page <= 0) {
                SetStatusTextChild_Player();
            } else {
                SetStatusTextChild_Friends(CharacterManager.Instance.GetFriendsOrder(page - 1));
            }
        }
        if (property.statusGrid.enabled != show) {
            property.statusGrid.enabled = show;
        }
    }

    void ChangeStatusArrowEnable(bool flag) {
        statusArrowEnabled = flag;
        for (int i = 0; i < property.arrow.Length; i++) {
            if (property.arrow[i].enabled != statusArrowEnabled) {
                property.arrow[i].enabled = statusArrowEnabled;
                property.arrow[i].raycastTarget = statusArrowEnabled;
            }
        }
    }

    void SelectItemTypeChild() {
        property.cursorRect.anchoredPosition = property.origin + property.interval * typeCursor;
        CreateSlot();
        bool isStatus = (typeCursor == (int)Type.Status);
        SetStatusText(isStatus, 0);
        bool arrowEnabledTemp = false;
        if (isStatus) {
            statusMax = CharacterManager.Instance.GetFriendsCount(false) + 1;
            arrowEnabledTemp = (statusMax > 1);
            statusCursor = 0;
        }
        if (arrowEnabledTemp != statusArrowEnabled) {
            ChangeStatusArrowEnable(arrowEnabledTemp);
        }
    }

    void SetPriceText() {
        if (itemCursor >= 0 && itemCursor < numSlots && buyItemPrice[itemCursor] >= 0) {
            shop.price.text = sb.Clear().Append(TextManager.Get("PAUSE_SHOP_PRICE")).Append(" ").Append(buyItemPrice[itemCursor]).ToString();
        } else {
            shop.price.text = string.Empty;
        }
        shop.fund.text = sb.Clear().Append(TextManager.Get("PAUSE_SHOP_FUND")).Append(" ").Append(GameManager.Instance.save.money).ToString();
    }
    
    void MakeItemCursor(bool coloring = true, int type = 0) {
        if (itemCursor >= numSlots) {
            itemCursor = numSlots - 1;
        } else if (itemCursor < 0) {
            itemCursor = 0;
        }
        if (coloring) {
            SetSlotObjectColor(true);
            switch (type) {
                case 0:
                    SetInformationText(property.name, property.information);
                    break;
                case 1:
                    SetInformationText(shop.name, shop.information);
                    break;
                case 2:
                    SetInformationText(storage.name, storage.information);
                    break;
            }
        }
    }

    void SelectItemType(int specifiedType = -1) {
        move = GameManager.Instance.MoveCursor(true);
        if (move.x != 0 || (eventEnterNum >= 0 && eventEnterNum < typeMax)) {
            typeCursor = (eventEnterNum >= 0 && eventEnterNum < typeMax ? eventEnterNum : (typeCursor + typeMax + move.x) % typeMax);
            SelectItemTypeChild();
            HideCaution();
            UISE.Instance.Play(UISE.SoundName.move);
        }
        bool considerSubmit = false;
        bool sounded = false;
        bool itemSelectReserved = false;
        if (specifiedType >= 0) {
            typeCursor = specifiedType;
            SelectItemTypeChild();
            considerSubmit = true;
        } else if (eventClickNum >= 0 && eventClickNum < typeMax) {
            typeCursor = eventClickNum;
            SelectItemTypeChild();
            considerSubmit = true;
        }
        if (eventClickNum >= 100 && eventClickNum < 100 + numSlots) {
            considerSubmit = true;
        }
        if (typeCursor == (int)Type.Status && statusMax > 1) {
            if (eventClickNum == 200 || mouseScroll > 0) {
                move.y = -1;
            } else if (eventClickNum == 201 || mouseScroll < 0) {
                move.y = 1;
            }
            if (move.y != 0) {
                statusCursor = (statusCursor + statusMax + move.y) % statusMax;
                UISE.Instance.Play(UISE.SoundName.move);
                SetStatusText(true, statusCursor);
            }
        }
        if (typeCursor != (int)Type.Status && (playerInput.GetButtonDown(RewiredConsts.Action.Submit) || considerSubmit)) {
            if ((friendsDisabled && typeCursor == (int)Type.Friends) || (itemDisabled && typeCursor == (int)Type.Item) || (equipChangeDisabled && typeCursor == (int)Type.Weapon)) {
                UISE.Instance.Play(UISE.SoundName.error);
                sounded = true;
            } else {
                if (numSlots > 0) {
                    state = 1;
                    if (eventClickNum >= 100 && eventClickNum < 100 + numSlots) {
                        itemCursor = eventClickNum - 100;
                        cursorSave[typeCursor] = itemCursor;
                        itemSelectReserved = true;
                    } else {
                        itemCursor = cursorSave[typeCursor];
                    }
                    MakeItemCursor();
                    UISE.Instance.Play(UISE.SoundName.submit);
                    sounded = true;
                }
            }
        }
        if (considerSubmit && !sounded) {
            UISE.Instance.Play(UISE.SoundName.submit);
        }
        if (itemSelectReserved) {
            SelectItem();
        }
    }

    void SelectItem() {
        move = GameManager.Instance.MoveCursor(true);
        if (numSlots > 0 && (move != Vector2Int.zero || (eventEnterNum >= 100 && eventEnterNum < 100 + numSlots) || (eventClickNum == 200 || eventClickNum == 201) || mouseScroll != 0)) {

            //TakeOut
            if (typeCursor == (int)Type.TakeOut) {
                int prePage = storagePage;
                if (move.y < 0) {
                    if (itemCursor < 8) {
                        storagePage = (storagePage - 1 + GameManager.storagePageMax) % GameManager.storagePageMax;
                        int newSlots = Mathf.Min(slotMax, GameManager.storageMax - storagePage * 32);
                        while (itemCursor < newSlots) {
                            itemCursor += 8;
                        }
                        itemCursor -= 8;
                        CreateSlot();
                    } else {
                        itemCursor -= 8;
                    }
                } else if (move.y > 0) {
                    if (itemCursor >= numSlots - 8) {
                        storagePage = (storagePage + 1 + GameManager.storagePageMax) % GameManager.storagePageMax;
                        while (itemCursor >= 0) {
                            itemCursor -= 8;
                        }
                        itemCursor += 8;
                        CreateSlot();
                    } else {
                        itemCursor += 8;
                    }
                }
                if (eventEnterNum >= 100 && eventEnterNum < 100 + numSlots) {
                    itemCursor = eventEnterNum - 100;
                } else if (move.x != 0) {
                    itemCursor += move.x;
                    if (itemCursor < 0) {
                        storagePage = (storagePage - 1 + GameManager.storagePageMax) % GameManager.storagePageMax;
                        CreateSlot();
                        itemCursor = numSlots - 1;
                    } else if (itemCursor >= numSlots) {
                        storagePage = (storagePage + 1 + GameManager.storagePageMax) % GameManager.storagePageMax;
                        CreateSlot();
                        itemCursor = 0;
                    }
                }
                if (eventClickNum == 200 || eventClickNum == 201 || mouseScroll != 0) {
                    int yTemp = (eventClickNum == 200 || mouseScroll > 0) ? -1 : (eventClickNum == 201 || mouseScroll < 0) ? 1 : 0;
                    storagePage = (storagePage + yTemp + GameManager.storagePageMax) % GameManager.storagePageMax;
                    CreateSlot();
                    if (itemCursor >= numSlots) {
                        itemCursor = numSlots - 1;
                    }
                }
                if (prePage != storagePage) {
                    storage.page.text = TextManager.Get("TROPHY_PAGE_" + (storagePage + 1));
                }

            } else {

                //Others
                if (move.y < 0) {
                    if (itemCursor < 8) {
                        while (itemCursor < numSlots) {
                            itemCursor += 8;
                        }
                    }
                    itemCursor -= 8;
                } else if (move.y > 0) {
                    if (itemCursor >= numSlots - 8) {
                        while (itemCursor >= 0) {
                            itemCursor -= 8;
                        }
                    }
                    itemCursor += 8;
                }
                itemCursor = (eventEnterNum >= 100 && eventEnterNum < 100 + numSlots ? eventEnterNum - 100 : (itemCursor + numSlots + move.x) % numSlots);
            }

            cursorSave[typeCursor] = itemCursor;
            SetSlotObjectColor(true);
            HideCaution();
            if (typeCursor == (int)Type.Buy || typeCursor == (int)Type.Sell) {
                SetPriceText();
                SetInformationText(shop.name, shop.information);
            } else if (typeCursor == (int)Type.Store || typeCursor == (int)Type.TakeOut) {
                SetInformationText(storage.name, storage.information);
            } else if (pauseType == PauseType.Normal) {
                SetInformationText(property.name, property.information);
            }
            UISE.Instance.Play(UISE.SoundName.move);
        }
        bool considerSubmit = false;
        if (eventClickNum >= 100 && eventClickNum < 100 + numSlots) {
            itemCursor = eventClickNum - 100;
            considerSubmit = true;
        }
        if (playerInput.GetButtonDown(RewiredConsts.Action.Submit) || considerSubmit) {
            if (slotItemId[itemCursor] >= 0) {
                UISE.Instance.Play(UISE.SoundName.submit);
                DecideItem();
            } else {
                UISE.Instance.Play(UISE.SoundName.error);
            }
        } else if (GameManager.Instance.GetCancelButtonDown) {
            if (pauseType == PauseType.Normal) {
                SetSlotObjectColor(false);
                state = 0;
                property.name.text = "";
                property.information.text = "";
                HideCaution();
            }
            UISE.Instance.Play(UISE.SoundName.cancel);
        } else if (eventClickNum >= 0 && eventClickNum < typeMax) {
            if (pauseType == PauseType.Normal) {
                property.name.text = "";
                property.information.text = "";
                HideCaution();
                SelectItemType(eventClickNum);
                if (typeCursor == (int)Type.Status) {
                    state = 0;
                }
            }
        }
    }

    void DecideItem() {
        int id = slotItemId[itemCursor];
        int type = id / 100;
        int mod = id % 100;
        if (pauseType == PauseType.Normal) {
            switch (type) {
                case 0:
                    if (id == GameManager.japarimanID || id == GameManager.japariman3SetID || id == GameManager.japariman5SetID) {
                        SetChoices(4, true, ItemDatabase.Instance.GetItemName(id), "CHOICE_USE", "CHOICE_PUT", "CHOICE_PACKING", "CHOICE_CANCEL");
                    } else {
                        SetChoices(3, true, ItemDatabase.Instance.GetItemName(id), "CHOICE_USE", "CHOICE_PUT", "CHOICE_CANCEL");
                    }
                    state = 10;
                    break;
                case 1:
                    if (mod == 0) {
                        if (CharacterManager.Instance.GetAnyFriendsExist()) {
                            SetChoices(6, true, ItemDatabase.Instance.GetItemName(id), "CHOICE_DRAW", "CHOICE_RELEASE", "CHOICE_COMMAND", "CHOICE_FRIENDS_SAVE", "CHOICE_FRIENDS_LOAD", "CHOICE_CANCEL");
                        } else {
                            SetChoices(6, true, ItemDatabase.Instance.GetItemName(id), "CHOICE_DRAW", "CHOICE_RANDOMCALLALL", "CHOICE_COMMAND", "CHOICE_FRIENDS_SAVE", "CHOICE_FRIENDS_LOAD", "CHOICE_CANCEL");
                        }
                        state = 11;
                    } else {
                        if (CharacterManager.Instance.GetFriendsExist(mod, false)) {
                            if (CharacterManager.Instance.GetFriendsExist(mod, true)) {
                                SetChoices(4, true, ItemDatabase.Instance.GetItemName(id), "CHOICE_DRAW", "CHOICE_RELEASE", "CHOICE_COMMAND", "CHOICE_CANCEL");
                            } else {
                                SetChoices(4, true, ItemDatabase.Instance.GetItemName(id), "CHOICE_REVIVE", "CHOICE_RELEASE", "CHOICE_COMMAND", "CHOICE_CANCEL");
                            }
                            state = 11;
                        } else {
                            SetChoices(2, true, ItemDatabase.Instance.GetItemName(id), "CHOICE_CALL", "CHOICE_CANCEL");
                            state = 11;
                        }
                    }
                    break;
                case 2:
                    UseWeapon(id);
                    break;
                case 4:
                    UseConfig(id);
                    SetInformationText(property.name, property.information);
                    break;
                case 5:
                    SetChoices(3, true, TextManager.Get(string.Format("DIC_FACILITY_NAME_{0:D2}", itemCursor)), "CHOICE_TAKEOUT", "CHOICE_STORE",  "CHOICE_CANCEL");
                    state = 17;
                    break;
            }
        } else if (typeCursor == (int)Type.Buy) {
            SetChoices(2, true, ItemDatabase.Instance.GetItemName(id), "CHOICE_BUY", "CHOICE_CANCEL");
            state = 12;
        } else if (typeCursor == (int)Type.Sell) {
            SetChoices(2, true, ItemDatabase.Instance.GetItemName(id), "CHOICE_SELL", "CHOICE_CANCEL");
            state = 13;
        } else if (typeCursor == (int)Type.Store) {
            SetChoices(2, true, ItemDatabase.Instance.GetItemName(id), "CHOICE_STORE", "CHOICE_CANCEL");
            state = 14;
        } else if (typeCursor == (int)Type.TakeOut) {
            SetChoices(2, true, ItemDatabase.Instance.GetItemName(id), "CHOICE_TAKEOUT", "CHOICE_CANCEL");
            state = 15;
        }
    }

    void SelectTrophy() {
        move = GameManager.Instance.MoveCursor(true);
        int prePage = trophyPage;
        if (numSlots > 0 && (move != Vector2Int.zero || (eventEnterNum >= 100 && eventEnterNum < 100 + numSlots))) {
            if (move.y < 0) {
                if (itemCursor < 8) {
                    trophyPage = (trophyPage - 1 + GameManager.trophyPageMax) % GameManager.trophyPageMax;
                    int newSlots = Mathf.Min(slotMax, GameManager.trophyMax - trophyPage * 32);
                    while (itemCursor < newSlots) {
                        itemCursor += 8;
                    }
                    itemCursor -= 8;
                    CreateSlot();
                } else {
                    itemCursor -= 8;
                }
            } else if (move.y > 0) {
                if (itemCursor >= numSlots - 8) {
                    trophyPage = (trophyPage + 1 + GameManager.trophyPageMax) % GameManager.trophyPageMax;
                    while (itemCursor >= 0) {
                        itemCursor -= 8;
                    }
                    itemCursor += 8;
                    CreateSlot();
                } else {
                    itemCursor += 8;
                }
            }
            if (eventEnterNum >= 100 && eventEnterNum < 100 + numSlots) {
                itemCursor = eventEnterNum - 100;
            } else if (move.x != 0) {
                itemCursor += move.x;
                if (itemCursor < 0) {
                    trophyPage = (trophyPage - 1 + GameManager.trophyPageMax) % GameManager.trophyPageMax;
                    CreateSlot();
                    itemCursor = numSlots - 1;
                } else if (itemCursor >= numSlots) {
                    trophyPage = (trophyPage + 1 + GameManager.trophyPageMax) % GameManager.trophyPageMax;
                    CreateSlot();
                    itemCursor = 0;
                }
            }
            cursorSave[typeCursor] = itemCursor;
            SetSlotObjectColor(true);
            SetInformationText(property.name, property.information);
            UISE.Instance.Play(UISE.SoundName.move);
        }
        if (eventClickNum == 200 || eventClickNum == 201 || mouseScroll != 0) {
            int yTemp = (eventClickNum == 200 || mouseScroll > 0) ? -1 : (eventClickNum == 201 || mouseScroll < 0) ? 1 : 0;
            trophyPage = (trophyPage + yTemp + GameManager.trophyPageMax) % GameManager.trophyPageMax;
            CreateSlot();
            if (itemCursor >= numSlots) {
                itemCursor = numSlots - 1;
                cursorSave[typeCursor] = itemCursor;
                SetSlotObjectColor(true);
            }
            SetInformationText(property.name, property.information);
            UISE.Instance.Play(UISE.SoundName.move);
        }
        if (prePage != trophyPage) {
            property.page.text = TextManager.Get("TROPHY_PAGE_" + (trophyPage + 1));
        }
        if (typeCursor == (int)Type.Trophy && slotObject[itemCursor]) {
            int index = trophyPage * 32 + itemCursor;
            if (index < GameManager.trophyMax && TrophyManager.Instance.trophyIsNew[index]) {
                TrophyManager.Instance.trophyIsNew[index] = false;
                TMP_Text limitText = slotObject[itemCursor].transform.GetChild(2).GetComponent<TMP_Text>();
                if (limitText != null) {
                    limitText.text = "";
                }
            }
        }
        if (GameManager.Instance.GetCancelButtonDown) {
            state = 1;
            typeCursor = (int)Type.Config;
            itemCursor = cursorSave[(int)Type.Config];
            SetTypeText(true);
            CreateSlot();
            MakeItemCursor(true, 0);
            ChangeStatusArrowEnable(false);
            UISE.Instance.Play(UISE.SoundName.cancel);
        }
    }

    void SelectFDKaban() {
        move = GameManager.Instance.MoveCursor(true);
        if (numSlots > 0 && (move != Vector2Int.zero || (eventEnterNum >= 100 && eventEnterNum < 100 + numSlots))) {
            if (move.y < 0) {
                if (itemCursor < 8) {
                    while (itemCursor < numSlots) {
                        itemCursor += 8;
                    }
                }
                itemCursor -= 8;
            } else if (move.y > 0) {
                if (itemCursor >= numSlots - 8) {
                    while (itemCursor >= 0) {
                        itemCursor -= 8;
                    }
                }
                itemCursor += 8;
            }
            itemCursor = (eventEnterNum >= 100 && eventEnterNum < 100 + numSlots ? eventEnterNum - 100 : (itemCursor + numSlots + move.x) % numSlots);            
            cursorSave[typeCursor] = itemCursor;
            SetSlotObjectColor(true);
            SetInformationText(property.name, property.information);
            UISE.Instance.Play(UISE.SoundName.move);
        }

        bool considerSubmit = false;
        if (eventClickNum >= 100 && eventClickNum < 100 + numSlots) {
            itemCursor = eventClickNum - 100;
            considerSubmit = true;
        }
        if (playerInput.GetButtonDown(RewiredConsts.Action.Submit) || considerSubmit) {
            if (slotItemId[itemCursor] >= 0) {
                UISE.Instance.Play(UISE.SoundName.submit);
                DecideItem();
            } else {
                UISE.Instance.Play(UISE.SoundName.error);
            }
        } else if (GameManager.Instance.GetCancelButtonDown) {
            state = 1;
            typeCursor = (int)Type.Item;
            itemCursor = cursorSave[(int)Type.Item];
            CreateSlot();
            MakeItemCursor(true, 0);
            ChangeStatusArrowEnable(false);
            UISE.Instance.Play(UISE.SoundName.cancel);
        }

    }

    void SelectSound() {
        move = GameManager.Instance.MoveCursor(true, 0.1f, 0.06f);
        if (eventClickNum >= 0 && eventClickNum < 3) {
            UISE.Instance.Play(UISE.SoundName.submit);
            soundTest.nowType = eventClickNum;
            ChangeSoundType();
        } else if (move.x != 0) {
            UISE.Instance.Play(UISE.SoundName.move);
            soundTest.nowType = (soundTest.nowType + 3 + move.x) % 3;
            ChangeSoundType();
        }
        int indexMax = soundTest.nowType == 0 ? SoundDatabase.musicMax + 1 : SoundDatabase.ambientMax + 1;
        int contentsMax = soundTest.contents.Length;
        if (eventClickNum == 200 || eventClickNum == 201 || mouseScroll != 0) {
            int yTemp = (eventClickNum == 200 || mouseScroll > 0) ? -1 : (eventClickNum == 201 || mouseScroll < 0) ? 1 : 0;
            soundTest.nowPivot = (soundTest.nowPivot + indexMax + yTemp) % indexMax;
            SetSoundItemTexts();
            UISE.Instance.Play(UISE.SoundName.move);
        }
        if ((move.y != 0 || (eventEnterNum >= 100 && eventEnterNum < 100 + contentsMax)) && soundTest.nowType < 2) {
            UISE.Instance.Play(UISE.SoundName.move);
            if (eventEnterNum >= 100 && eventEnterNum < 100 + contentsMax) {
                soundTest.nowPlus = eventEnterNum - 100;
            } else {
                soundTest.nowPlus += move.y;
                if (soundTest.nowPlus < 0) {
                    soundTest.nowPivot = (soundTest.nowPivot + indexMax - 1) % indexMax;
                    soundTest.nowPlus = 0;
                } else if (soundTest.nowPlus >= contentsMax) {
                    soundTest.nowPivot = (soundTest.nowPivot + indexMax + 1) % indexMax;
                    soundTest.nowPlus = contentsMax - 1;
                }
            }
            soundTest.itemCursor.anchoredPosition = soundTest.itemCursorOrigin + soundTest.itemCursorInterval * soundTest.nowPlus;
            SetSoundItemTexts();
        }
        if (playerInput.GetButtonDown(RewiredConsts.Action.Submit) || eventClickNum == 2 || (eventClickNum >= 100 && eventClickNum < 100 + contentsMax)) {
            if (eventClickNum >= 100 && eventClickNum < 100 + contentsMax) {
                soundTest.nowPlus = eventClickNum - 100;
                soundTest.itemCursor.anchoredPosition = soundTest.itemCursorOrigin + soundTest.itemCursorInterval * soundTest.nowPlus;
            }
            int indexTemp = (soundTest.nowPivot + soundTest.nowPlus) % indexMax - 1;
            if (soundTest.nowType == 0) {
                if (BGM.Instance) {
                    if (indexTemp >= 0) {
                        if (SoundDatabase.Instance.musicEnabled[indexTemp]) {
                            BGM.Instance.Play(indexTemp, 0f, true);
                        }
                    } else {
                        BGM.Instance.Stop();
                    }
                }
            } else if (soundTest.nowType == 1) {
                if (Ambient.Instance) {
                    if (indexTemp >= 0) {
                        if (SoundDatabase.Instance.ambientEnabled[indexTemp]) {
                            Ambient.Instance.Play(indexTemp, 0f);
                        }
                    } else {
                        Ambient.Instance.Stop();
                    }
                }
            } else {
                UISE.Instance.Play(UISE.SoundName.submit);
                PauseSoundTest(false);
            }
        }
        if (GameManager.Instance.GetCancelButtonDown) {
            UISE.Instance.Play(UISE.SoundName.cancel);
            PauseSoundTest(false);
        }
    }

    bool GetBusCondition(int index) {
        if (bus.destination[index].x == StageManager.specialStageId) {
            return GameManager.Instance.GetSecret(GameManager.SecretType.SkytreeCleared);
        } else {
            return GameManager.Instance.save.progress >= bus.destination[index].x;
        }
    }

    void InitBusTexts() {
        bus.title.text = TextManager.Get("WORD_DESTINATION");
        bus.nowPivot = 0;
        bus.nowPlus = 0;
        bus.itemCursor.anchoredPosition = bus.itemCursorOrigin + bus.itemCursorInterval * bus.nowPlus;
        SetBusItemTexts();
    }

    void SetBusItemTexts() {
        int indexStart = bus.nowPivot;
        int indexMax = bus.destination.Length;
        for (int i = 0; i < bus.contents.Length; i++) {
            int indexTemp = (indexStart + i) % indexMax;
            if (GetBusCondition(indexTemp)) {
                bus.contents[i].text = TextManager.Get(bus.dicKey[indexTemp]);
            } else {
                bus.contents[i].text = TextManager.Get("DUNGEON_LOCK");
            }
        }
    }

    void SelectBus() {
        move = GameManager.Instance.MoveCursor(true, 0.1f, 0.06f);
        int indexMax = bus.destination.Length;
        int contentsMax = bus.contents.Length;
        if (eventClickNum == 200 || eventClickNum == 201 || mouseScroll != 0) {
            int yTemp = (eventClickNum == 200 || mouseScroll > 0) ? -1 : (eventClickNum == 201 || mouseScroll < 0) ? 1 : 0;
            bus.nowPivot = (bus.nowPivot + indexMax + yTemp) % indexMax;
            SetBusItemTexts();
            UISE.Instance.Play(UISE.SoundName.move);
        }
        if (move.y != 0 || (eventEnterNum >= 100 && eventEnterNum < 100 + contentsMax)) {
            UISE.Instance.Play(UISE.SoundName.move);
            if (eventEnterNum >= 100 && eventEnterNum < 100 + contentsMax) {
                bus.nowPlus = eventEnterNum - 100;
            } else {
                bus.nowPlus += move.y;
                if (bus.nowPlus < 0) {
                    bus.nowPivot = (bus.nowPivot + indexMax - 1) % indexMax;
                    bus.nowPlus = 0;
                } else if (bus.nowPlus >= contentsMax) {
                    bus.nowPivot = (bus.nowPivot + indexMax + 1) % indexMax;
                    bus.nowPlus = contentsMax - 1;
                }
            }
            bus.itemCursor.anchoredPosition = bus.itemCursorOrigin + bus.itemCursorInterval * bus.nowPlus;
            SetBusItemTexts();
        }
        if (playerInput.GetButtonDown(RewiredConsts.Action.Submit) || (eventClickNum >= 100 && eventClickNum < 100 + contentsMax)) {
            if (eventClickNum >= 100 && eventClickNum < 100 + contentsMax) {
                bus.nowPlus = eventClickNum - 100;
                bus.itemCursor.anchoredPosition = bus.itemCursorOrigin + bus.itemCursorInterval * bus.nowPlus;
            }
            int indexTemp = (bus.nowPivot + bus.nowPlus) % indexMax;
            if (GetBusCondition(indexTemp)) {
                UISE.Instance.Play(UISE.SoundName.submit);
                PauseBus(false);
                if (bus.busConsole != null) {
                    bus.busConsole.SetMoveChoices(indexTemp);
                }
            } else {
                UISE.Instance.Play(UISE.SoundName.error);
            }
        }
        if (GameManager.Instance.GetCancelButtonDown) {
            UISE.Instance.Play(UISE.SoundName.cancel);
            PauseBus(false);
        }
    }

    void InitBlendTexts() {
        blend.title.text = TextManager.Get("WORD_BLEND");
        // blend.nowPivot = 0;
        // blend.nowPlus = 0;
        blend.itemCursor.anchoredPosition = blend.itemCursorOrigin + blend.itemCursorInterval * blend.nowPlus;
        SetBlendItemTexts();
    }

    // OK = 1
    // Lack of Gold = -1
    // Lack of Material = -2;
    int CheckBlendCondition(int selectIndex) {
        int answer = 0;
        if (selectIndex >= 0 && selectIndex < blendChildMax) {
            answer = 1;
            int costTemp = blendChildList[selectIndex].overrideCost;
            if (costTemp <= 0) {
                costTemp = ItemDatabase.Instance.GetItemPrice(blendChildList[selectIndex].afterItemID);
            }
            if (blend.blendConsole && blend.blendConsole.requireGold && GameManager.Instance.save.money < costTemp) {
                answer = -1;
            }
            for (int i = 0; answer > 0 && i < blendChildList[selectIndex].beforeItemID.Length; i++) {
                if (GameManager.Instance.save.NumOfSpecificItems(blendChildList[selectIndex].beforeItemID[i]) < blendChildList[selectIndex].beforeItemCount[i]) {
                    answer = -2;
                }
            }
        }
        return answer;
    }

    void SetBlendItemTexts() {
        int indexStart = blend.nowPivot;
        if (blendChildMax > 0) {
            for (int i = 0; i < blend.contents.Length; i++) {
                int indexTemp = (indexStart + i) % blendChildMax;
                blend.contents[i].text = TextManager.Get(System.String.Format("ITEM_NAME_{0:000}", blendChildList[indexTemp].afterItemID));
                if (CheckBlendCondition(indexTemp) > 0) {
                    blend.contents[i].colorGradientPreset = blend.okColor;
                    blend.enabledMarks[i].enabled = true;
                } else {
                    blend.contents[i].colorGradientPreset = blend.ngColor;
                    blend.enabledMarks[i].enabled = false;
                }
            }
            int selectTemp = (blend.nowPivot + blend.nowPlus) % blendChildMax;
            if (selectTemp >= 0 && selectTemp < blendChildMax) {
                int costTemp = blendChildList[selectTemp].overrideCost;
                if (costTemp <= 0) {
                    costTemp = blend.requireGold ? ItemDatabase.Instance.GetItemPrice(blendChildList[selectTemp].afterItemID) : 0;
                }

                sb.Clear().Append(TextManager.Get("WORD_BLENDCOST"));
                bool costLackFlag = GameManager.Instance.save.money < costTemp;
                if (costLackFlag) {
                    sb.Append("<color=#FFBBBB>");
                }
                sb.AppendFormat(" {0} ", costTemp).Append(TextManager.Get("STATUS_GOLD"));
                if (costLackFlag) {
                    sb.Append("</color>");
                }
                sb.AppendLine();
                sb.Append(TextManager.Get("WORD_BLENDMATERIAL")).Append(" ");

                if (blendChildList[selectTemp].beforeItemID.Length > 0) {
                    for (int i = 0; i < blendChildList[selectTemp].beforeItemID.Length; i++) {
                        bool itemLackFlag = GameManager.Instance.save.NumOfSpecificItems(blendChildList[selectTemp].beforeItemID[i]) < blendChildList[selectTemp].beforeItemCount[i];
                        if (itemLackFlag) {
                            sb.Append("<color=#FFBBBB>");
                        }
                        sb.Append(TextManager.Get(System.String.Format("ITEM_NAME_{0:000}", blendChildList[selectTemp].beforeItemID[i]))).AppendFormat(" x{0}", blendChildList[selectTemp].beforeItemCount[i]);
                        if (itemLackFlag) {
                            sb.Append("</color>");
                        }
                        if (i < blendChildList[selectTemp].beforeItemID.Length - 1) {
                            sb.Append(" , ");
                        }
                    }
                } else {
                    sb.Append(TextManager.Get("WORD_BLEND_NONE"));
                }
                blend.information.text = sb.ToString();
            } else {
                blend.information.text = "";
            }
        }
    }

    void SelectBlend() {
        if (blendChildMax > 0) {
            move = GameManager.Instance.MoveCursor(true, 0.1f, 0.06f);
            int contentsMax = blend.contents.Length;
            if (eventClickNum == 200 || eventClickNum == 201 || mouseScroll != 0) {
                int yTemp = (eventClickNum == 200 || mouseScroll > 0) ? -1 : (eventClickNum == 201 || mouseScroll < 0) ? 1 : 0;
                blend.nowPivot = (blend.nowPivot + blendChildMax + yTemp) % blendChildMax;
                SetBlendItemTexts();
                UISE.Instance.Play(UISE.SoundName.move);
            }
            if (move.y != 0 || (eventEnterNum >= 100 && eventEnterNum < 100 + contentsMax)) {
                HideCaution();
                UISE.Instance.Play(UISE.SoundName.move);
                if (eventEnterNum >= 100 && eventEnterNum < + 100 + contentsMax) {
                    blend.nowPlus = eventEnterNum - 100;
                } else {
                    blend.nowPlus += move.y;
                    if (blend.nowPlus < 0) {
                        blend.nowPivot = (blend.nowPivot + blendChildMax - 1) % blendChildMax;
                        blend.nowPlus = 0;
                    } else if (blend.nowPlus >= blend.contents.Length) {
                        blend.nowPivot = (blend.nowPivot + blendChildMax + 1) % blendChildMax;
                        blend.nowPlus = contentsMax - 1;
                    }
                }
                blend.itemCursor.anchoredPosition = blend.itemCursorOrigin + blend.itemCursorInterval * blend.nowPlus;
                SetBlendItemTexts();
            }
            if (playerInput.GetButtonDown(RewiredConsts.Action.Submit) || (eventClickNum >= 100 && eventClickNum < 100 + contentsMax)) {
                if (eventClickNum >= 100 && eventClickNum < 100 + contentsMax) {
                    blend.nowPlus = eventClickNum - 100;
                    blend.itemCursor.anchoredPosition = blend.itemCursorOrigin + blend.itemCursorInterval * blend.nowPlus;
                }
                int indexTemp = (blend.nowPivot + blend.nowPlus) % blendChildMax;
                int blendCondition = CheckBlendCondition(indexTemp);
                if (blendCondition > 0) {
                    UISE.Instance.Play(UISE.SoundName.submit);
                    PauseBlend(false);
                    if (blend.blendConsole != null) {
                        blend.blendConsole.SetChoices(indexTemp);
                    }
                } else {
                    UISE.Instance.Play(UISE.SoundName.error);
                    if (blendCondition == -1) {
                        ShowCaution(TextManager.Get("CAUTION_BLEND_GOLD"), false, false);
                    } else if (blendCondition == -2) {
                        ShowCaution(TextManager.Get("CAUTION_BLEND_MATERIAL"), false, false);
                    }
                }
            }
        }
        if (GameManager.Instance.GetCancelButtonDown) {
            UISE.Instance.Play(UISE.SoundName.cancel);
            PauseBlend(false);
        }
    }

    public int ChoicesControl() {
        move = GameManager.Instance.MoveCursor(true);
        if (choices.max > 0 && (move.y != 0 || (eventEnterNum >= eventChoiceBase && eventEnterNum < eventChoiceBase + choices.max))) {
            choices.cursorPos = (eventEnterNum >= eventChoiceBase && eventEnterNum < eventChoiceBase + choices.max ? eventEnterNum - eventChoiceBase : (choices.cursorPos + choices.max + move.y) % choices.max);
            choices.cursorRect.anchoredPosition = choices.origin + choices.cursorPos * choices.interval;
            UISE.Instance.Play(UISE.SoundName.move);
            HideCaution();
        }
        bool considerSubmit = false;
        if (eventClickNum >= eventChoiceBase && eventClickNum < eventChoiceBase + choices.max) {
            choices.cursorPos = eventClickNum - eventChoiceBase;
            choices.cursorRect.anchoredPosition = choices.origin + choices.cursorPos * choices.interval;
            considerSubmit = true;
        }
        if (playerInput.GetButtonDown(RewiredConsts.Action.Submit) || considerSubmit) {
            return choices.cursorPos;
        } else if (GameManager.Instance.GetCancelButtonDown) {
            return -2;
        } else {
            return -1;
        }
    }

    void DCChild_CancelCommon() {
        state = 1;
        UISE.Instance.Play(UISE.SoundName.submit);
    }

    public bool CheckJaparimanPackEnabled(int plus) {
        int invRemain = GameManager.Instance.save.InventoryMax - GameManager.Instance.save.NumOfAllItems();
        invRemain += GameManager.Instance.save.NumOfSpecificItems(GameManager.japarimanID) + GameManager.Instance.save.NumOfSpecificItems(GameManager.japariman3SetID) + GameManager.Instance.save.NumOfSpecificItems(GameManager.japariman5SetID);
        int japarimanCount = GameManager.Instance.GetJaparimanCount() + plus;
        int j5Count = japarimanCount / 5;
        int j3Count = (japarimanCount - j5Count * 5) / 3;
        int j1Count = japarimanCount - j5Count * 5 - j3Count * 3;
        return invRemain >= j5Count + j3Count + j1Count;
    }

    public bool IsItemGetable(int japaribunCount = 0) {
        if (japaribunCount > 0 && GameManager.Instance.save.config[GameManager.Save.configID_JaparibunsAutoPacking] != 0) {
            if (GameManager.Instance.save.items.Count < GameManager.Instance.save.InventoryMax) {
                return true;
            } else {
                return CheckJaparimanPackEnabled(japaribunCount);
            }
        } else {
            return GameManager.Instance.save.items.Count < GameManager.Instance.save.InventoryMax;
        }
    }

    public bool JaparimanPacking(int plus = 0) {
        int count = GameManager.Instance.GetJaparimanCount() + plus;
        bool answer = false;
        if (count >= 3) { 
            int j1Count = GameManager.Instance.save.NumOfSpecificItems(GameManager.japarimanID);
            int j3Count = GameManager.Instance.save.NumOfSpecificItems(GameManager.japariman3SetID);
            int j5Count = GameManager.Instance.save.NumOfSpecificItems(GameManager.japariman5SetID);
            if (j1Count > 0) {
                for (int i = 0; i < j1Count; i++) {
                    GameManager.Instance.save.RemoveItem(GameManager.japarimanID);
                }
            }
            if (j3Count > 0) {
                for (int i = 0; i < j3Count; i++) {
                    GameManager.Instance.save.RemoveItem(GameManager.japariman3SetID);
                }
            }
            if (j5Count > 0) {
                for (int i = 0; i < j5Count; i++) {
                    GameManager.Instance.save.RemoveItem(GameManager.japariman5SetID);
                }
            }
            while (count > 0) {
                if (count >= 5) {
                    GameManager.Instance.save.AddItem(GameManager.japariman5SetID);
                    count -= 5;
                } else if (count >= 3) {
                    GameManager.Instance.save.AddItem(GameManager.japariman3SetID);
                    count -= 3;
                } else {
                    GameManager.Instance.save.AddItem(GameManager.japarimanID);
                    count -= 1;
                }
            }
            int j1Check = GameManager.Instance.save.NumOfSpecificItems(GameManager.japarimanID);
            int j3Check = GameManager.Instance.save.NumOfSpecificItems(GameManager.japariman3SetID);
            int j5Check = GameManager.Instance.save.NumOfSpecificItems(GameManager.japariman5SetID);
            answer = (j1Count != j1Check || j3Count != j3Check || j5Count != j5Check);
        }
        return answer;
    }

    void OffPauseOrBack(bool isFriends = false) {
        if (GameManager.Instance.save.config[GameManager.Save.configID_ContinuousUseOfItems] == 0) {
            OnPause(false, false, PauseType.Normal);
        } else {
            CreateSlot();
            MakeItemCursor();
            if (isFriends) {
                UpdateFriendsMark();
            }
            state = 1;
        }
    }

    void Shuffle(ref int[] array) {
        int i = array.Length;
        while (i > 1) {
            int j = Random.Range(0, i);
            i--;
            int t = array[i];
            array[i] = array[j];
            array[j] = t;
        }
    }

    bool RandomCall(bool allFlag) {
        int[] id = new int[GameManager.friendsMax - 1];
        for (int i = 0; i < id.Length; i++) {
            id[i] = i + 1;
        }
        Shuffle(ref id);
        int limit = CharacterManager.Instance.GetFriendsLimit();
        int costSum = CharacterManager.Instance.GetFriendsCostSum();
        int minCost = 100;
        bool costFlag = !StageManager.Instance.IsHomeStage;
        int japaribun = costFlag ? GameManager.Instance.GetJaparimanCount() : 10000;
        bool success = false;
        int actualCost = 0;
        for (int i = 0; i < id.Length && costSum < limit && japaribun > 0 && (!success || allFlag); i++) {
            if (GameManager.Instance.save.friends[id[i]] != 0 && !CharacterManager.Instance.GetFriendsExist(id[i], false)) {
                int newCost = CharacterManager.Instance.GetFriendsCost(id[i]);
                if (newCost < minCost) {
                    minCost = newCost;
                }
                if (costSum + newCost <= limit && japaribun - newCost >= 0) {
                    if (CharacterManager.Instance.ChangeFriends(id[i], true, false) > 0) {
                        actualCost += newCost;
                        costSum += newCost;
                        japaribun -= newCost;
                        success = true;
                    }
                }
            }
        }
        if (actualCost > 0 && costFlag) {
            CharacterManager.Instance.UseJapariman(actualCost);
        }
        if (!success) {
            if (limit - costSum < minCost) {
                ShowCaution(TextManager.Get("CAUTION_COST"), false);
            } else if (!costFlag && japaribun <= minCost) {
                ShowCaution(TextManager.Get("CAUTION_JAPARIBUN"), false);
            }
        }
        return success;
    }

    void DecideChoices(ChoicesType type) {
        int id = slotItemId[itemCursor];
        int answer = ChoicesControl();
        int mod = id % 100;
        bool cancelChoicesFlag = true;
        if (answer >= 0) {
            switch (type) {
                case ChoicesType.Item:
                    switch (answer) {
                        case 0:
                            if (id == GameManager.fdKabanID) {
                                state = 4;
                                typeCursor = (int)Type.FDKaban;
                                itemCursor = cursorSave[(int)Type.FDKaban];
                                CreateSlot();
                                MakeItemCursor();
                                UISE.Instance.Play(UISE.SoundName.submit);
                            } else {
                                if (!UseItem(id)) {
                                    UISE.Instance.Play(UISE.SoundName.error);
                                    state = 1;
                                } else {
                                    GameManager.Instance.save.items.Remove(id);
                                    OffPauseOrBack();
                                }
                            }
                            break;
                        case 1:
                            ItemDatabase.Instance.GiveItem(id, pCon.transform.position, 1, -1, -1, 1f);
                            GameManager.Instance.save.items.Remove(id);
                            UISE.Instance.Play(UISE.SoundName.submit);
                            OffPauseOrBack();
                            break;
                        case 2:
                            if (id == GameManager.japarimanID || id == GameManager.japariman3SetID || id == GameManager.japariman5SetID) {
                                if (JaparimanPacking()) {
                                    UISE.Instance.Play(UISE.SoundName.use);
                                    CreateSlot();
                                    SetSlotObjectColor(true);
                                    state = 1;
                                } else {
                                    ShowCaution(TextManager.Get("CAUTION_PACKING"), false, false);
                                    UISE.Instance.Play(UISE.SoundName.error);
                                    state = 1;
                                }
                            } else {
                                DCChild_CancelCommon();
                            }
                            break;
                        case 3:
                            DCChild_CancelCommon();
                            break;
                    }
                    break;
                case ChoicesType.Friends:
                    if (mod == 0) {
                        switch (answer) {
                            case 0:
                                if (CharacterManager.Instance.GetAnyFriendsExist()) {
                                    CharacterManager.Instance.WarpToPlayerPosAll();
                                    OffPauseOrBack();
                                } else {
                                    UISE.Instance.Play(UISE.SoundName.error);
                                    DCChild_CancelCommon();
                                }
                                break;
                            case 1:
                                if (CharacterManager.Instance.GetAnyFriendsExist()) {
                                    for (int i = 0; i < GameManager.friendsMax; i++) {
                                        CharacterManager.Instance.Erase(i, true);
                                    }
                                    OffPauseOrBack();
                                } else {
                                    if (RandomCall(true)) {
                                        OffPauseOrBack();
                                    } else {
                                        UISE.Instance.Play(UISE.SoundName.error);
                                        DCChild_CancelCommon();
                                    }
                                }
                                break;
                            case 2:
                                cancelChoicesFlag = false;
                                UISE.Instance.Play(UISE.SoundName.submit);
                                SetChoices(5, true, ItemDatabase.Instance.GetItemName(id), "CHOICE_ATTACK", "CHOICE_EVADE", "CHOICE_IGNORE", "CHOICE_FREE", "CHOICE_CANCEL");
                                state = 41;
                                break;
                            case 3:
                                SetCombination(true, true);
                                UISE.Instance.Play(UISE.SoundName.submit);
                                break;
                            case 4:
                                SetCombination(true, false);
                                UISE.Instance.Play(UISE.SoundName.submit);
                                break;
                            case 5:
                                DCChild_CancelCommon();
                                break;
                        }
                    } else {
                        if (CharacterManager.Instance.GetFriendsExist(mod, false)) {
                            switch (answer) {
                                case 0:
                                    if (CharacterManager.Instance.GetFriendsExist(mod, true)) {
                                        CharacterManager.Instance.WarpToPlayerPos(mod);
                                        OffPauseOrBack();
                                    } else {
                                        if (!ReviveFriends(mod)) {
                                            UISE.Instance.Play(UISE.SoundName.error);
                                            state = 1;
                                        }
                                    }
                                    break;
                                case 1:
                                    if (!UseFriends(mod)) {
                                        UISE.Instance.Play(UISE.SoundName.error);
                                        state = 1;
                                    }
                                    break;
                                case 2:
                                    cancelChoicesFlag = false;
                                    UISE.Instance.Play(UISE.SoundName.submit);
                                    SetChoices(5, true, ItemDatabase.Instance.GetItemName(id), "CHOICE_ATTACK", "CHOICE_EVADE", "CHOICE_IGNORE", "CHOICE_FREE", "CHOICE_CANCEL");
                                    state = 41;
                                    break;
                                case 3:
                                    DCChild_CancelCommon();
                                    break;
                            }
                        } else {
                            switch (answer) {
                                case 0:
                                    if (!UseFriends(mod)) {
                                        UISE.Instance.Play(UISE.SoundName.error);
                                        state = 1;
                                    }
                                    break;
                                case 1:
                                    DCChild_CancelCommon();
                                    break;
                            }
                        }
                    }
                    break;
                case ChoicesType.Buy:
                    switch (answer) {
                        case 0:
                            int price = buyItemPrice[itemCursor];
                            if (price > 0 && GameManager.Instance.save.money >= price && !buyItemSoldOut[itemCursor]) {
                                UseBuy(id, price);
                            } else {
                                UISE.Instance.Play(UISE.SoundName.error);
                            }
                            state = 1;
                            break;
                        case 1:
                            DCChild_CancelCommon();
                            break;
                    }
                    break;
                case ChoicesType.Sell:
                    switch (answer) {
                        case 0:
                            int price = buyItemPrice[itemCursor];
                            UseSell(id, price);
                            state = 1;
                            break;
                        case 1:
                            DCChild_CancelCommon();
                            break;
                    }
                    break;
                case ChoicesType.Store:
                    switch (answer) {
                        case 0:
                            UseStore(id);
                            state = 1;
                            break;
                        case 1:
                            DCChild_CancelCommon();
                            break;
                    }
                    break;
                case ChoicesType.TakeOut:
                    switch (answer) {
                        case 0:
                            UseTakeOut(id);
                            state = 1;
                            break;
                        case 1:
                            DCChild_CancelCommon();
                            break;
                    }
                    break;
                case ChoicesType.Quit:
                    switch (answer) {
                        case 0:
                            if (returnLibraryDisabled || (StageManager.Instance && StageManager.Instance.stageNumber == StageManager.infernoStageId && GameManager.Instance.save.progress < GameManager.gameClearedProgress)) {
                                UISE.Instance.Play(UISE.SoundName.error);
                                state = 1;
                            } else {
                                SceneChange.Instance.StartEyeCatch();
                                state = 31;
                            }
                            break;
                        case 1:
                            UISE.Instance.Play(UISE.SoundName.use);
                            pauseEnabled = false;
                            OnPause(false, false, PauseType.Normal);
                            if (StageManager.Instance) {
                                StageManager.Instance.DestroyTaggedObjects("Effect");
                                StageManager.Instance.CleanObjectPool();
                            }
                            // GameManager.Instance.LoadLastSaveSlot();
                            GameManager.Instance.LoadScene("Title");
                            GameManager.Instance.ChangeTimeScale(false, false);
                            break;
                        case 2:
                            DCChild_CancelCommon();
                            break;
                    }
                    break;
                case ChoicesType.Command:
                    if (mod == 0) {
                        switch (answer) {
                            case 0:
                                CharacterManager.Instance.SetGeneralCommand(CharacterBase.Command.Default, true);
                                UISE.Instance.Play(UISE.SoundName.use);
                                OffPauseOrBack();
                                break;
                            case 1:
                                CharacterManager.Instance.SetGeneralCommand(CharacterBase.Command.Evade, true);
                                UISE.Instance.Play(UISE.SoundName.use);
                                OffPauseOrBack();
                                break;
                            case 2:
                                CharacterManager.Instance.SetGeneralCommand(CharacterBase.Command.Ignore, true);
                                UISE.Instance.Play(UISE.SoundName.use);
                                OffPauseOrBack();
                                break;
                            case 3:
                                CharacterManager.Instance.SetGeneralCommand(CharacterBase.Command.Free, true);
                                UISE.Instance.Play(UISE.SoundName.use);
                                OffPauseOrBack();
                                break;
                            case 4:
                                DCChild_CancelCommon();
                                break;
                        }
                    } else {
                        switch (answer) {
                            case 0:
                                CharacterManager.Instance.SetSpecificCommand(mod, CharacterBase.Command.Default, true);
                                UISE.Instance.Play(UISE.SoundName.use);
                                OffPauseOrBack();
                                break;
                            case 1:
                                CharacterManager.Instance.SetSpecificCommand(mod, CharacterBase.Command.Evade, true);
                                UISE.Instance.Play(UISE.SoundName.use);
                                OffPauseOrBack();
                                break;
                            case 2:
                                CharacterManager.Instance.SetSpecificCommand(mod, CharacterBase.Command.Ignore, true);
                                UISE.Instance.Play(UISE.SoundName.use);
                                OffPauseOrBack();
                                break;
                            case 3:
                                CharacterManager.Instance.SetSpecificCommand(mod, CharacterBase.Command.Free, true);
                                UISE.Instance.Play(UISE.SoundName.use);
                                OffPauseOrBack();
                                break;
                            case 4:
                                DCChild_CancelCommon();
                                break;
                        }
                    }
                    break;
                case ChoicesType.Reset:
                    switch (answer) {
                        case 0:
                        case 1:
                        case 2:
                            GameManager.Instance.save.config[GameManager.Save.configID_QualityLevel] = (answer == 0 ? 4 : answer == 1 ? 2 : 0);
                            QualitySettings.SetQualityLevel(GameManager.Instance.save.config[GameManager.Save.configID_QualityLevel], true);
                            GameManager.Instance.save.ResetQualityConfig(2 - answer);
                            UpdateConfig(GameManager.Save.configID_DynamicBone);
                            UpdateConfig(GameManager.Save.configID_ClothSimulation);
                            UpdateConfig(GameManager.Save.configID_FaceToEnemy);
                            UpdateConfig(GameManager.Save.configID_Antialiasing);
                            UISE.Instance.Play(UISE.SoundName.use);
                            DCChild_CancelCommon();
                            break;
                        case 3:
                            GameManager.Instance.save.ResetAntiScreensickConfig();
                            UpdateConfig(GameManager.Save.configID_CameraSensitivity);
                            UpdateConfig(GameManager.Save.configID_FieldOfView);
                            UpdateConfig(GameManager.Save.configID_MotionBlur);
                            UpdateConfig(GameManager.Save.configID_CameraAxisDefault);
                            UISE.Instance.Play(UISE.SoundName.use);
                            DCChild_CancelCommon();
                            break;
                        case 4:
                            GameManager.Instance.save.ResetVolumeConfig();
                            GameManager.Instance.ApplyVolume();
                            UISE.Instance.Play(UISE.SoundName.use);
                            DCChild_CancelCommon();
                            break;
                        case 5:
                            GameManager.Instance.save.ResetConfig();
                            UpdateConfig(GameManager.Save.configID_Blinking);
                            UpdateConfig(GameManager.Save.configID_CameraSensitivity);
                            UpdateConfig(GameManager.Save.configID_CameraControlButton);
                            UpdateConfig(GameManager.Save.configID_CameraTurningSpeed);
                            UpdateConfig(GameManager.Save.configID_FieldOfView);
                            UpdateConfig(GameManager.Save.configID_Gauge);
                            UpdateConfig(GameManager.Save.configID_ShowGiraffeBeam);
                            UpdateConfig(GameManager.Save.configID_ShowCursor);
                            UpdateConfig(GameManager.Save.configID_HpGaugeSize);
                            UpdateConfig(GameManager.Save.configID_FriendsIconSize);
                            UpdateConfig(GameManager.Save.configID_MapPos);
                            UpdateConfig(GameManager.Save.configID_GoldPos);
                            UpdateConfig(GameManager.Save.configID_DynamicBone);
                            UpdateConfig(GameManager.Save.configID_ClothSimulation);
                            UpdateConfig(GameManager.Save.configID_FaceToEnemy);
                            UpdateConfig(GameManager.Save.configID_Antialiasing);
                            UpdateConfig(GameManager.Save.configID_SpeakerMode);
                            UpdateConfig(GameManager.Save.configID_AcousticEffects);
                            UpdateConfig(GameManager.Save.configID_RunInBackground);
                            UpdateConfig(GameManager.Save.configID_QualityLevel);
                            UpdateConfig(GameManager.Save.configID_SystemInformation);
                            UpdateConfig(GameManager.Save.configID_UseMouseForUI);
                            UpdateConfig(GameManager.Save.configID_EquipmentLimit);
                            UISE.Instance.Play(UISE.SoundName.use);
                            DCChild_CancelCommon();
                            break;
                        case 6:
                            DCChild_CancelCommon();
                            break;
                    }
                    if (CameraManager.Instance) {
                        CameraManager.Instance.CheckDepthTextureMode();
                    }
                    break;
                case ChoicesType.FDKaban:
                    switch (answer) {
                        case 0:
                            TakeoutFacility(mod);
                            UISE.Instance.Play(UISE.SoundName.use);
                            OffPauseOrBack();
                            break;
                        case 1:
                            if (StoreFacility(mod)) {
                                UISE.Instance.Play(UISE.SoundName.use);
                            } else {
                                UISE.Instance.Play(UISE.SoundName.error);
                            }
                            OffPauseOrBack();
                            break;
                        case 2:
                            DCChild_CancelCommon();
                            break;
                    }
                    break;
                case ChoicesType.Language:
                    if (answer == GameManager.languageMax) {
                        DCChild_CancelCommon();
                        break;
                    } else if (answer >= 0 && answer < GameManager.languageMax) {
                        GameManager.Instance.save.language = answer;
                        InitializeLanguage();
                        UISE.Instance.Play(UISE.SoundName.use);
                        DCChild_CancelCommon();
                    }
                    break;
            }
            if (cancelChoicesFlag) {
                CancelChoices(false);
            }
        } else if (answer == -2) {
            SetChoicesCanvasEnabled(false);
            state = 1;
            UISE.Instance.Play(UISE.SoundName.cancel);
        }
    }

    public bool UseItem(int id) {
        bool used = true;
        bool caution = false;
        switch (id % 100) {
            case 0:
                CharacterManager.Instance.Heal(itemHealAmounts[0], itemHealPercents[0], (int)EffectDatabase.id.itemHeal01, true, true, true, true, true);
                break;
            case 1:
                CharacterManager.Instance.Heal(itemHealAmounts[1], itemHealPercents[1], (int)EffectDatabase.id.itemHeal02, true, true, true, true, true);
                break;
            case 2:
                CharacterManager.Instance.Heal(itemHealAmounts[2], itemHealPercents[2], (int)EffectDatabase.id.itemHeal03, true, true, false, true, true);
                break;
            case 9:
                CharacterManager.Instance.AddSandstar(5, true, (int)EffectDatabase.id.itemSandstar, true, true);
                break;
            case 20:
                CharacterManager.Instance.SetBuff(CharacterManager.BuffType.Antidote, 80);
                break;
            case 21:
                CharacterManager.Instance.SetBuff(CharacterManager.BuffType.Stamina, 40);
                break;
            case 22:
                CharacterManager.Instance.SetBuff(CharacterManager.BuffType.Speed, 40);
                break;
            case 23:
                CharacterManager.Instance.SetBuff(CharacterManager.BuffType.Attack, 40);
                break;
            case 24:
                CharacterManager.Instance.SetBuff(CharacterManager.BuffType.Defense, 40);
                break;
            case 25:
                CharacterManager.Instance.SetBuff(CharacterManager.BuffType.Knock, 40);
                break;
            case 26:
                CharacterManager.Instance.SetBuff(CharacterManager.BuffType.Absorb, 40);
                break;
            case 27:
                CharacterManager.Instance.SetBuff(CharacterManager.BuffType.Stealth, 40);
                break;
            case 28:
                CharacterManager.Instance.SetBuff(CharacterManager.BuffType.Long, 40);
                break;
            case 29:
                CharacterManager.Instance.SetBuff(CharacterManager.BuffType.Multi, 40);
                break;
            case 40:
                CharacterManager.Instance.SuperWarp();
                break;
            case 41:
                if (StageManager.Instance.mapActivateFlag < 2) {
                    StageManager.Instance.SetAllMapChipActivate(false);
                    CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.itemGuide);
                } else {
                    used = false;
                    caution = true;
                }
                break;
            case 42:
                if (StageManager.Instance.ClearTraps() && StageManager.Instance.dungeonController && StageManager.Instance.dungeonController.trapSettings.enabled) {
                    CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.itemClearTraps);
                } else {
                    used = false;
                    caution = true;
                }
                break;
            case 43:
                if (StageManager.Instance.dungeonController && StageManager.Instance.dungeonController.tileSettings.enabled) {
                    CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.itemGenerateGolds);
                    StageManager.Instance.dungeonController.usedGoldGraph = true;
                    StageManager.Instance.dungeonController.GenerateGolds(8, false, 5, 10);
                } else {
                    used = false;
                    caution = true;
                }
                break;
            case 44:
                if (StageManager.Instance.dungeonController) {
                    if (StageManager.Instance.dungeonController.BreakWalls(false)) {
                        CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.itemBreakWalls);
                        if (CameraManager.Instance && pCon) {
                            CameraManager.Instance.SetQuake(pCon.transform.position, 8, 0.2f, 2f);
                        }
                    } else {
                        used = false;
                        caution = true;
                    }
                } else {
                    used = false;
                    caution = true;
                }
                break;
            case 45:
                if (StageManager.Instance.dungeonController) {
                    if (StageManager.Instance.dungeonController.BreakWalls(true)) {
                        CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.itemReplaceWalls);
                        if (CameraManager.Instance && pCon) {
                            CameraManager.Instance.SetQuake(pCon.transform.position, 8, 0.2f, 2f);
                        }
                    } else {
                        used = false;
                        caution = true;
                    }
                } else {
                    used = false;
                    caution = true;
                }
                break;
            case 46:
                if (pCon && StageManager.Instance.dungeonController) {
                    GameObject warpObj1 = Instantiate(ItemDatabase.Instance.GetItemPrefab(350), pCon.transform.position, Quaternion.identity, StageManager.Instance.dungeonController.transform);
                    WarpConsole warpConsole1 = warpObj1.GetComponentInChildren<WarpConsole>();
                    CharacterManager.Instance.SuperWarp();
                    GameObject warpObj2 = Instantiate(ItemDatabase.Instance.GetItemPrefab(350), pCon.transform.position, Quaternion.identity, StageManager.Instance.dungeonController.transform);
                    WarpConsole warpConsole2 = warpObj2.GetComponentInChildren<WarpConsole>();
                    if (warpConsole1 && warpConsole2) {
                        warpConsole1.specifiedDestination = warpObj2.transform;
                        warpConsole2.specifiedDestination = warpObj1.transform;
                        Instantiate(MapDatabase.Instance.prefab[MapDatabase.other], warpObj1.transform);
                        Instantiate(MapDatabase.Instance.prefab[MapDatabase.other], warpObj2.transform);
                    }
                }
                break;
            case 50:
                CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.itemCrystal_GS, -1, false);
                break;
            case 51:
                CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.itemCrystal_GM, -1, false);
                break;
            case 52:
                CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.itemCrystal_GL, -1, false);
                break;
            case 53:
                if (StageManager.Instance.dungeonController && !StageManager.Instance.dungeonController.isBossFloor) {
                    CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.itemCrystal_K, -1, false);
                } else {
                    used = false;
                    caution = true;
                }
                break;
            case 54:
                if (StageManager.Instance.dungeonController && !StageManager.Instance.dungeonController.isBossFloor) {
                    CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.itemCrystal_R, -1, false);
                } else {
                    used = false;
                    caution = true;
                }
                break;
            case 55:
                if (StageManager.Instance.dungeonController && !StageManager.Instance.dungeonController.isBossFloor) {
                    CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.itemCrystal_B, -1, false);
                } else {
                    used = false;
                    caution = true;
                }
                break;
            case 56:
                CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.itemCrystal_V, -1, false);
                break;
            case 58:
                if (StageManager.Instance.dungeonController && !StageManager.Instance.dungeonController.isBossFloor) {
                    CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.itemCrystal_C, -1, false);
                } else {
                    used = false;
                    caution = true;
                }
                break;
            case 59:
                if (StageManager.Instance.dungeonController && !StageManager.Instance.dungeonController.isBossFloor) {
                    CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.itemCrystal_W, -1, false);
                } else {
                    used = false;
                    caution = true;
                }
                break;
            case 60:
                if (CharacterManager.Instance.GetSafetyBombEnabled()) {
                    CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.itemBomb01_Kaban, 1, false);
                } else {
                    CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.itemBomb01, -1, false);
                }
                break;
            case 61:
                if (CharacterManager.Instance.GetSafetyBombEnabled()) {
                    CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.itemBomb02_Kaban, 1, false);
                } else {
                    CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.itemBomb02, -1, false);
                }
                break;
            case 62:
                if (CharacterManager.Instance.GetSafetyBombEnabled()) {
                    CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.itemBomb03_Kaban, 1, false);
                } else {
                    CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.itemBomb03, -1, false);
                }
                break;
            case 63:
                if (CharacterManager.Instance.GetSafetyBombEnabled()) {
                    CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.itemBomb01_Kaban_L, 1, false);
                } else {
                    CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.itemBomb01_L, -1, false);
                }
                break;
            case 64:
                if (CharacterManager.Instance.GetSafetyBombEnabled()) {
                    CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.itemBomb02_Kaban_L, 1, false);
                } else {
                    CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.itemBomb02_L, -1, false);
                }
                break;
            case 65:
                if (CharacterManager.Instance.GetSafetyBombEnabled()) {
                    CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.itemBomb03_Kaban_L, 1, false);
                } else {
                    CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.itemBomb03_L, -1, false);
                }
                break;
            case 70:
                CharacterManager.Instance.Heal(40, 7, (int)EffectDatabase.id.itemJaparibun01, CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.JaparibunHealAll) != 0, true, true, true, true);
                break;
            case 71:
                CharacterManager.Instance.Heal(120, 21, (int)EffectDatabase.id.itemJaparibun02, CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.JaparibunHealAll) != 0, true, true, true, true);
                break;
            case 72:
                CharacterManager.Instance.Heal(200, 35, (int)EffectDatabase.id.itemJaparibun03, CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.JaparibunHealAll) != 0, true, true, true, true);
                break;
            case 80:
                if (StageManager.Instance && StageManager.Instance.GetSandstarRawEnabled()) {
                    StageManager.Instance.ActivateSandstarRaw();
                    CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.itemSandstarRaw, -1, false);
                } else {
                    used = false;
                    caution = true;
                }
                break;
            case 81:
                CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.itemCelliumCure, -1, true, false);
                break;
            default:
                used = false;
                break;
        }
        if (caution && pauseGame) {
            ShowCaution(TextManager.Get("CAUTION_ITEMBOSS"), false);
        }
        if (used) {
            CharacterManager.Instance.PlayerAttackStamp();
            if (id != 80 && !(id >= 60 && id <= 69)) {
                CharacterManager.Instance.UseItemCountIncrement();
            }
        }
        return used;
    }

    public bool ReviveFriends(int id) {
        bool answer = false;
        int check = CharacterManager.Instance.CheckFriendsReviveAbility(id, !StageManager.Instance.IsHomeStage);
        if (check == -1) {
            if (pauseGame) {
                ShowCaution(TextManager.Get("CAUTION_COST"), false);
            }
        } else if (check == -2) {
            if (pauseGame) {
                ShowCaution(TextManager.Get("CAUTION_JAPARIBUN"), false);
            }
        } else if (check == 0) {
            CharacterManager.Instance.Erase(id, false);
        }
        if (check >= 0) {
            CharacterManager.Instance.ChangeFriends(id, true, !StageManager.Instance.IsHomeStage);
            if (CharacterManager.Instance.friends[id].fBase) {
                CharacterManager.Instance.friends[id].fBase.SetForRevive();
            }
            if (pauseGame) {
                OffPauseOrBack();
            } else {
                MessageUI.Instance.SetMessage(sb.Clear()
                    .Append(TextManager.Get("QUOTE_START"))
                    .Append(ItemDatabase.Instance.GetItemName(100 + id))
                    .Append(TextManager.Get("QUOTE_END"))
                    .Append(TextManager.Get("MESSAGE_AUTOMATICREVIVE")).ToString());
            }
            answer = true;
        }
        return answer;
    }

    bool UseFriends(int id) {
        int friendsChanged = CharacterManager.Instance.ChangeFriends(id, true, !StageManager.Instance.IsHomeStage);
        bool answer = false;
        switch (friendsChanged) {
            case -2:
                ShowCaution(TextManager.Get("CAUTION_JAPARIBUN"), false);
                break;
            case -1:
                ShowCaution(TextManager.Get("CAUTION_COST"), false);
                break;
            case 0:
                OffPauseOrBack();
                answer = true;
                break;
            case 1:
                OffPauseOrBack();
                answer = true;
                break;
        }
        return answer;
    }

    void UseWeapon(int id) {
        if (id == GameManager.hpUpId || id == GameManager.stUpId || id == GameManager.invUpId) {
            state = 16;
            volume.cursorPos = (id == GameManager.hpUpId ? 0 : id == GameManager.stUpId ? 1 : 2);
            volume.cursorRect.anchoredPosition = volume.origin + volume.interval * volume.cursorPos;
            volume.choice[0].text = TextManager.Get("CONFIG_STATUSUP_0");
            volume.choice[1].text = TextManager.Get("CONFIG_STATUSUP_1");
            volume.choice[2].text = TextManager.Get("CONFIG_STATUSUP_2");
            statusUpSave[0] = GameManager.Instance.save.equipedHpUp;
            statusUpSave[1] = GameManager.Instance.save.equipedStUp;
            statusUpSave[2] = GameManager.Instance.save.equipedInvUp;
            UpdateStatusUp(true);
            SetVolumeCanvasEnabled(true);
        } else {
            if (CharacterManager.Instance) {
                if (GameManager.Instance.IsPlayerAnother && id >= GameManager.armsIDBase + anotherWeaponGap) { 
                    id -= anotherWeaponGap;
                }
                CharacterManager.Instance.SetWeapon(id % 100);
            }
            UpdateEquipedMark();
        }
    }

    void InitializeLanguage() {
        GameManager.Instance.InitLanguage();
        SetTypeText();
        SetInformationText(property.name, property.information);
        if (CharacterManager.Instance) {
            CharacterManager.Instance.UpdateLevelLimit();
            CharacterManager.Instance.ResetActionTypeTextLanguage();
            CharacterManager.Instance.ResetCommandTexts();
        }
    }

    void UseConfig(int id) {
        bool playSe = true;
        int cursor = id % 100;
        switch (cursor) {
            case 0:
                state = 30;
                if (StageManager.Instance && CharacterManager.Instance) {
                    bool isAnother = GameManager.Instance.IsPlayerAnother;
                    if (StageManager.Instance.IsHomeFloor) {
                        if (isAnother) {
                            SetChoices(3, true, ItemDatabase.Instance.GetItemName(id), "CHOICE_QUIT_LIBRARY_ANOTHER", "CHOICE_QUIT_GAME", "CHOICE_CANCEL");
                        } else {
                            SetChoices(3, true, ItemDatabase.Instance.GetItemName(id), "CHOICE_QUIT_LIBRARY", "CHOICE_QUIT_GAME", "CHOICE_CANCEL");
                        }
                    } else {
                        if (isAnother) {
                            SetChoices(3, true, ItemDatabase.Instance.GetItemName(id), "CHOICE_QUIT_STAGE_ANOTHER", "CHOICE_QUIT_GAME", "CHOICE_CANCEL");
                        } else {
                            SetChoices(3, true, ItemDatabase.Instance.GetItemName(id), "CHOICE_QUIT_STAGE", "CHOICE_QUIT_GAME", "CHOICE_CANCEL");
                        }
                    }
                }
                playSe = false;
                break;
            case 1:
                state = 23;
                SetChoices(7, true, ItemDatabase.Instance.GetItemName(id), "CHOICE_RESET_HQ", "CHOICE_RESET_MQ", "CHOICE_RESET_LQ", "CHOICE_RESET_ANTISCREENSICK", "CHOICE_RESET_VOLUME", "CHOICE_RESET_ALL", "CHOICE_CANCEL");
                playSe = false;
                break;
            case 2:
                state = 27;
                SetChoices(GameManager.languageMax + 1, true, ItemDatabase.Instance.GetItemName(id), "LANGUAGE_0", "LANGUAGE_1", "LANGUAGE_2", "LANGUAGE_3", "CHOICE_CANCEL");
                playSe = false;
                break;
            case 3:
                if (keyBindsLanguageSave != GameManager.Instance.save.language) {
                    keyBindsLanguageSave = GameManager.Instance.save.language;
                    controlMapperWrapper.InitializeTexts();
                }
                controlMapperWrapper.Open();
                baseCanvas.enabled = false;
                playSe = false;
                state = 60;
                break;
            case 4:
                SetTutorial(0);
                break;
            case 5:
                state = 20;
                volume.cursorPos = 0;
                volume.cursorRect.anchoredPosition = volume.origin + volume.interval * volume.cursorPos;
                volume.choice[0].text = TextManager.Get("CONFIG_VOL_MUSIC");
                volume.choice[1].text = TextManager.Get("CONFIG_VOL_SOUND");
                volume.choice[2].text = TextManager.Get("CONFIG_VOL_AMBIENT");
                UpdateVolume(true);
                SetVolumeCanvasEnabled(true);
                playSe = false;
                break;
            case 6:
            case 7:
            case 8:
            case 9:
            case 10:
            case 11:
            case 12:
            case 13:
            case 14:
            case 15:
                state = 21;
                GameManager.Instance.InitConfigDefaultArray();
                configType = cursor - 6;
                config.cursorPos = 0;
                config.count = configMax[configType];
                config.cursorRect.anchoredPosition = config.origin + config.interval * config.cursorPos;
                config.pageNow = 0;
                config.pageMax = (config.count - 1) / Config.denomi + 1;
                if (config.pageMax <= 1) {
                    config.max = config.count;
                } else if (config.pageNow < config.pageMax - 1) {
                    config.max = Config.denomi;
                } else {
                    config.max = (config.count - 1) % Config.denomi + 1;
                }
                SetConfigText();
                config.ChangeArrow(config.pageMax > 1);
                config.SetPageText();
                property.canvas.enabled = false;
                property.gridLayoutGroup.enabled = false;
                SetConfigCanvasEnabled(true);
                playSe = false;
                break;
            case 16:
                if (!GameManager.Instance.IsPlayerAnother) {
                    state = 3;
                    TrophyManager.Instance.FixTrophyArray();
                    typeCursor = (int)Type.Trophy;
                    itemCursor = cursorSave[(int)Type.Trophy];
                    SetTypeText(false);
                    CreateSlot();
                    MakeItemCursor();
                    ChangeStatusArrowEnable(true);
                    TrophyManager.Instance.CheckSteamAchievement();
                } else {
                    UISE.Instance.Play(UISE.SoundName.error);
                }
                playSe = false;
                break;
        }
        if (playSe) {
            UISE.Instance.Play(UISE.SoundName.use);
        }
    }

    void UseBuy(int id, int price) {
        bool complete = GameManager.Instance.save.AddItem(id);
        if (GameManager.Instance.save.config[GameManager.Save.configID_JaparibunsAutoPacking] != 0) {
            int japaribunType = GameManager.Instance.GetJaparibunType(id);
            if (japaribunType != 0) {
                if (complete) {
                    JaparimanPacking();
                } else if (CheckJaparimanPackEnabled(japaribunType)) {
                    JaparimanPacking(japaribunType);
                    complete = true;
                }
            }
        }
        if (complete) {
            GameManager.Instance.save.money -= price;
            CreateSlot();
            UpdateSoldOutMark();
            SetSlotObjectColor(true);
            UISE.Instance.Play(UISE.SoundName.pay);
        } else {
            UISE.Instance.Play(UISE.SoundName.error);
        }
        shopBoughtFlag = true;
    }

    void UseSell(int id, int price) {
        if (id < 100) {
            GameManager.Instance.save.RemoveItem(id);
            GameManager.Instance.save.money += price;
            CreateSlot();
            SetSlotObjectColor(true);
            UISE.Instance.Play(UISE.SoundName.pay);
        } else {
            UISE.Instance.Play(UISE.SoundName.error);
        }
    }

    void UseStore(int id) {
        if (id >= 0 && id < 100 && GameManager.Instance.save.AddStorage(id)) {
            GameManager.Instance.save.RemoveItem(id);
            CreateSlot();
            SetSlotObjectColor(true);
            UISE.Instance.Play(UISE.SoundName.submit);
            int count = GameManager.Instance.save.storage.Count;
            storage.count.text = sb.Clear().Append(count).Append("/").Append(GameManager.storageMax).ToString();
            storage.count.colorGradientPreset = (count >= GameManager.storageMax ? property.limitColorZero : property.normalColor);
        } else {
            ShowCaution(TextManager.Get("CAUTION_STORE"), false, false);
            UISE.Instance.Play(UISE.SoundName.error);
        }
    }

    void UseTakeOut(int id) {
        if (id >= 0 && id < 100 && GameManager.Instance.save.NumOfAllItems() < GameManager.Instance.save.InventoryMax) {
            GameManager.Instance.save.RemoveStorage(id);
            GameManager.Instance.save.AddItem(id);
            CreateSlot();
            SetSlotObjectColor(true);
            UISE.Instance.Play(UISE.SoundName.submit);
            int count = GameManager.Instance.save.storage.Count;
            storage.count.text = sb.Clear().Append(count).Append("/").Append(GameManager.storageMax).ToString();
            storage.count.colorGradientPreset = (count >= GameManager.storageMax ? property.limitColorZero : property.normalColor);
        } else {
            ShowCaution(TextManager.Get("CAUTION_TAKEOUT"), false, false);
            UISE.Instance.Play(UISE.SoundName.error);
        }
    }

    public void PauseBuy(bool isOn = true) {
        OnPause(isOn, false, PauseType.Buy);
    }

    public void PauseSell(bool isOn = true) {
        OnPause(isOn, false, PauseType.Sell);
    }

    public void PauseStore(bool isOn = true) {
        if (isOn) {
            int count = GameManager.Instance.save.storage.Count;
            storage.title.text = TextManager.Get("CHOICE_STORE");
            storage.page.text = "";
            storage.count.text = sb.Clear().Append(count).Append("/").Append(GameManager.storageMax).ToString();
            storage.count.colorGradientPreset = (count >= GameManager.storageMax ? property.limitColorZero : property.normalColor);
        }
        for (int i = 0; i < storage.arrow.Length; i++) {
            storage.arrow[i].enabled = false;
            storage.arrow[i].raycastTarget = false;
        }
        OnPause(isOn, false, PauseType.Store);
    }

    public void PauseTakeOut(bool isOn = true) {
        if (isOn) {
            int count = GameManager.Instance.save.storage.Count;
            if (storagePage != 0) {
                if (count <= slotMax) {
                    storagePage = 0;
                } else if (storagePage > (count - 1) / 32) {
                    storagePage = (count - 1) / 32;
                }
            }
            storage.title.text = TextManager.Get("CHOICE_TAKEOUT");
            storage.page.text = TextManager.Get("TROPHY_PAGE_" + (storagePage + 1));
            storage.count.text = sb.Clear().Append(count).Append("/").Append(GameManager.storageMax).ToString();
            storage.count.colorGradientPreset = (count >= GameManager.storageMax ? property.limitColorZero : property.normalColor);
        }
        for (int i = 0; i < storage.arrow.Length; i++) {
            storage.arrow[i].enabled = isOn;
            storage.arrow[i].raycastTarget = isOn;
        }
        OnPause(isOn, false, PauseType.Takeout);
    }

    public void PauseSoundTest(bool isOn = true) {
        Input.ResetInputAxes();
        pauseType = PauseType.SoundTest;
        soundTest.canvas.enabled = isOn;
        soundTest.gridLayoutGroup.enabled = isOn;
        if (!isOn || GameManager.Instance.MouseEnabled) {
            soundTest.gRaycaster.enabled = isOn;
        }
        HideCaution();
        if (isOn) {
            CharacterManager.Instance.StopFriends();
            InitSoundTexts();
            state = 22;
        } else {
            if (CameraManager.Instance) {
                CameraManager.Instance.SetEventTimer(0.005f);
            }
        }
        CharacterManager.Instance.SetPlayerUpdateEnabled(!isOn);
        pauseGame = isOn;
    }

    public void PauseBus(bool isOn = true) {
        Input.ResetInputAxes();
        pauseType = PauseType.Bus;
        bus.canvas.enabled = isOn;
        bus.gridLayoutGroup.enabled = isOn;
        if (!isOn || GameManager.Instance.MouseEnabled) {
            bus.gRaycaster.enabled = isOn;
        }
        HideCaution();
        if (isOn) {
            CharacterManager.Instance.StopFriends();
            InitBusTexts();
            state = 24;
        } else {
            if (CameraManager.Instance) {
                CameraManager.Instance.SetEventTimer(0.005f);
            }
        }
        CharacterManager.Instance.SetPlayerUpdateEnabled(!isOn);
        pauseGame = isOn;
    }

    public void PauseBlend(bool isOn = true, bool requireGold = true) {
        Input.ResetInputAxes();
        pauseType = PauseType.Blend;
        blend.canvas.enabled = isOn;
        blend.gridLayoutGroup.enabled = isOn;
        if (!isOn || GameManager.Instance.MouseEnabled) {
            blend.gRaycaster.enabled = isOn;
        }
        HideCaution();
        if (isOn) {
            blend.requireGold = requireGold;
            blendChildList.Clear();
            blendChildMax = 0;
            bool isHome = (StageManager.Instance && StageManager.Instance.IsHomeStage);
            bool isRandomStage = (StageManager.Instance && StageManager.Instance.IsRandomStage) || (GameManager.Instance && GameManager.Instance.IsPlayerAnother);
            bool isCleared = (GameManager.Instance.save.progress >= GameManager.gameClearedProgress);
            for (int i = 0; i < blend.blendChild.Length; i++) {
                if (blend.blendChild[i].isHomeOnly && !isHome) {
                    continue;
                }
                if (blend.blendChild[i].isRandomStageOnly && !isRandomStage) {
                    continue;
                }
                if (blend.blendChild[i].isGameClearedOnly && !isCleared) {
                    continue;
                }
                blendChildList.Add(blend.blendChild[i]);
                blendChildMax++;
            }
            CharacterManager.Instance.StopFriends();
            InitBlendTexts();
            state = 26;
        } else {
            if (CameraManager.Instance) {
                CameraManager.Instance.SetEventTimer(0.005f);
            }
        }
        CharacterManager.Instance.SetPlayerUpdateEnabled(!isOn);
        pauseGame = isOn;
        CharacterManager.Instance.showGoldContinuous = isOn && requireGold;
    }

    public void PausePhoto(bool isOn = true) {
        if (isOn) {
            bool photoMoveInfinity = (StageManager.Instance && StageManager.Instance.CheckPhotoStageCondition());
            photo.infoText.text = TextManager.Get("PHOTO_INFO");
            photo.limitText.text = photoMoveInfinity ? "" : TextManager.Get("PHOTO_LIMIT");
            if (UISE.Instance) {
                UISE.Instance.Play(UISE.SoundName.photoModeStart);
            }
            if (GameManager.Instance.save.config[GameManager.Save.configID_PhotoMode] == 1 && CanvasCulling.Instance) {
                CanvasCulling.Instance.CheckConfig(CanvasCulling.indexGauge, 0);
                CanvasCulling.Instance.SetMapCameraEnabled(false);
            }
            if (CameraManager.Instance) {
                CameraManager.Instance.photoMoveInfinity = photoMoveInfinity;
            }
            depthOfFieldSave = GameManager.Instance.save.config[GameManager.Save.configID_DepthOfField];
        } else {
            if (UISE.Instance) {
                UISE.Instance.Play(UISE.SoundName.photoModeEnd);
            }
            if (GameManager.Instance.save.config[GameManager.Save.configID_PhotoMode] == 1 && CanvasCulling.Instance) {
                CanvasCulling.Instance.CheckConfig();
                CanvasCulling.Instance.SetMapCameraEnabled(true);
            }
            if (InstantiatePostProcessingProfile.Instance && depthOfFieldSave != GameManager.Instance.save.config[GameManager.Save.configID_DepthOfField]) {
                GameManager.Instance.save.config[GameManager.Save.configID_DepthOfField] = depthOfFieldSave;
                if (CameraManager.Instance) {
                    CameraManager.Instance.CheckDepthTextureMode();
                }
                InstantiatePostProcessingProfile.Instance.QualitySettingsAdjustments();
            }
        }
        photo.canvas.enabled = isOn;
        frameAdvanceFlag = false;
        frameAdvanceEnabled = false;
        OnPause(isOn, false, PauseType.Photo);
    }

    public void PauseDictionary(bool isOn = true) {
        Input.ResetInputAxes();
        pauseType = PauseType.Dictionary;
        HideCaution();
        if (isOn) {
            CharacterManager.Instance.StopFriends();
            state = 70;
        } else {
            if (CameraManager.Instance) {
                CameraManager.Instance.SetEventTimer(0.005f);
            }
        }
        CharacterManager.Instance.SetPlayerUpdateEnabled(!isOn);
        pauseGame = isOn;
    }

    public void UpdatePhotoControlText(bool moveXY, float speed, Vector3 position, float azimuth, float elevation) {
        sb.Clear();
        sb.Append(TextManager.Get("PHOTO_CONTROL_NAME_0")).Append(moveXY ? "<color=#BBFFBB>" : "<color=#FFFFBB>").Append(TextManager.Get(moveXY ? "PHOTO_CONTROL_XY" : "PHOTO_CONTROL_XZ")).Append("</color>").AppendLine();
        sb.Append(TextManager.Get("PHOTO_CONTROL_NAME_1")).Append(speed <= 0.5f ? "<color=#CCCCFF>" : speed <= 2.0f ? "<color=#CCFFFF>" : speed <= 8.0f ? "<color=#FFCCCC>" : "<color=#FF9999>").AppendFormat("{0:0.0}", speed).Append(TextManager.Get("PHOTO_CONTROL_SPEED")).Append("</color>").AppendLine();
        sb.Append(TextManager.Get("PHOTO_CONTROL_NAME_2")).AppendFormat("{0:0.00}, {1:0.00}, {2:0.00}", position.x, position.y, position.z).AppendLine();
        sb.Append(TextManager.Get("PHOTO_CONTROL_NAME_3")).AppendFormat("{0:0.00}", azimuth).Append(TextManager.Get("PHOTO_CONTROL_DEGREE")).AppendLine();
        sb.Append(TextManager.Get("PHOTO_CONTROL_NAME_4")).AppendFormat("{0:0.00}", elevation).Append(TextManager.Get("PHOTO_CONTROL_DEGREE")).AppendLine();
        sb.Append(TextManager.Get("PHOTO_CONTROL_NAME_5")).Append(TextManager.Get(GameManager.Instance.save.config[GameManager.Save.configID_DepthOfField] == 0 ? "PHOTO_CONTROL_OFF" : "PHOTO_CONTROL_ON"));
        photo.controlText.text = sb.ToString();
    }

    public void ExcludeFromPhoto(bool flag) {
        for (int i = 0; i < photo.exclude.Length; i++) {
            if (photo.exclude[i]) {
                photo.exclude[i].SetActive(!flag);
            }
        }
    }

    public bool IsPhotoPausing {
        get { return pauseGame && pauseType == PauseType.Photo; }
    }

    public bool GetCanPhotoMode {
        get { return GameManager.Instance.save.config[GameManager.Save.configID_PhotoMode] != 0 && !pauseGame && Time.timeScale > 0f; }
    }

    public void CancelChoices(bool releasePause = true) {
        SetChoicesCanvasEnabled(false);
        if (releasePause) {
            if (CharacterManager.Instance != null) {
                CharacterManager.Instance.TimeStop(false);
            }
        }
        HideCaution();
    }
    public void SetChoices(int numOfChoices = 3, bool pause = true, string title = "", string choice1 = "", string choice2 = "", string choice3 = "", string choice4 = "", string choice5 = "", string choice6 = "", string choice7 = "") {
        SetNumOfChoices(numOfChoices);
        choices.title.text = sb.Clear().Append(TextManager.Get("QUOTE_START")).Append(title).Append(TextManager.Get("QUOTE_END")).ToString();
        if (numOfChoices > 0 && !string.IsNullOrEmpty(choice1)) {
            choices.choice[0].text = TextManager.Get(choice1);
        }
        if (numOfChoices > 1 && !string.IsNullOrEmpty(choice2)) {
            choices.choice[1].text = TextManager.Get(choice2);
        }
        if (numOfChoices > 2 && !string.IsNullOrEmpty(choice3)) {
            choices.choice[2].text = TextManager.Get(choice3);
        }
        if (numOfChoices > 3 && !string.IsNullOrEmpty(choice4)) {
            choices.choice[3].text = TextManager.Get(choice4);
        }
        if (numOfChoices > 4 && !string.IsNullOrEmpty(choice5)) {
            choices.choice[4].text = TextManager.Get(choice5);
        }
        if (numOfChoices > 5 && !string.IsNullOrEmpty(choice6)) {
            choices.choice[5].text = TextManager.Get(choice6);
        }
        if (numOfChoices > 6 && !string.IsNullOrEmpty(choice7)) {
            choices.choice[6].text = TextManager.Get(choice7);
        }
        SetChoicesCanvasEnabled(true);
        if (pause) {
            if (CharacterManager.Instance != null) {
                CharacterManager.Instance.TimeStop(true);
            }
        }
    }

    public void SetChoicesDirect(int numOfChoices = 3, bool pause = true, string title = "", string choice1 = "", string choice2 = "", string choice3 = "", string choice4 = "", string choice5 = "", string choice6 = "", string choice7 = "") {
        SetChoices(numOfChoices, pause, title);
        if (numOfChoices > 0) {
            choices.choice[0].text = choice1;
        }
        if (numOfChoices > 1) {
            choices.choice[1].text = choice2;
        }
        if (numOfChoices > 2) {
            choices.choice[2].text = choice3;
        }
        if (numOfChoices > 3) {
            choices.choice[3].text = choice4;
        }
        if (numOfChoices > 4) {
            choices.choice[4].text = choice5;
        }
        if (numOfChoices > 5) {
            choices.choice[5].text = choice6;
        }
        if (numOfChoices > 6) {
            choices.choice[6].text = choice7;
        }
    }

    public void CopyItems_Reserve(bool excludeMinmi = true) {
        if (!copyItemsReserved) {
            int itemsLength = GameManager.Instance.save.items.Count;
            if (itemsLength > 0) {
                copyItemsArray = GameManager.Instance.save.items.ToArray();
                copyItemsReserved = true;
                if (excludeMinmi) {
                    for (int i = 0; i < copyItemsArray.Length; i++) {
                        if (copyItemsArray[i] >= GameManager.minmiIDBase) {
                            copyItemsArray[i] = -1;
                        }
                    }
                }
            }
        }
    }

    public void CopyItems_Done() {
        if (copyItemsReserved && copyItemsArray.Length > 0) {
            ItemDatabase.Instance.GiveItem(copyItemsArray, CharacterManager.Instance.playerTrans);
            copyItemsReserved = false;
        }
    }

    public void OffPauseExternal(bool sound = false) {
        if (pauseGame) {
            OnPause(false, sound, pauseType);
        }
    }

    void ReturnLibrary() {
        returnToLibraryProcessing = true;
        if (SceneChange.Instance.GetEyeCatch()) {
            state = 0;
            returnToLibraryProcessing = false;
            OnPause(false, false, PauseType.Normal);
            CharacterManager.Instance.SetForParkman(false);
            int originallyPlayerIndex = CharacterManager.Instance.GetOriginallyPlayerIndex;
            if (CharacterManager.Instance.playerIndex != originallyPlayerIndex) {
                CharacterManager.Instance.SetNewPlayer(originallyPlayerIndex);
            }
            if (GameManager.Instance.save.isExpPreserved != 0) {
                CopyItems_Reserve(true);
                GameManager.Instance.save.RestoreExp();
                CharacterManager.Instance.UpdateSandstarMax();
            }
            int floorNumber = StageManager.Instance.IsHomeFloor ? StageManager.Instance.homeFloorNumberSecond : StageManager.Instance.homeFloorNumber;
            if (StageManager.Instance.IsHomeStage && StageManager.Instance.dungeonMother) {
                StageManager.Instance.dungeonMother.MoveFloor(floorNumber);
            } else {
                StageManager.Instance.MoveStage(StageManager.homeStageId, floorNumber, -1);
            }
            if (GameManager.Instance.IsPlayerAnother) {
                GameManager.Instance.InitializeForPlayerAnother();
            }
            CopyItems_Done();
            SetBlackCurtain(0f, false);
            SceneChange.Instance.EndEyeCatch();
        }
    }

    public void ReturnLibraryExternal() {
        ReturnLibrary();
    }

    public void SetBlackCurtain(float amount, bool isWhite = false) {
        amount = Mathf.Clamp01(amount);
        if (isWhite) {
            whiteColor.a = amount;
            blackCurtain.blackImage.color = whiteColor;
        } else {
            blackColor.a = amount;
            blackCurtain.blackImage.color = blackColor;
        }
        if (amount <= 0f && blackCurtain.canvas.enabled) {
            blackCurtain.canvas.enabled = false;
        } else if (amount > 0f && !blackCurtain.canvas.enabled) {
            blackCurtain.canvas.enabled = true;
        }
        blackCurtainAmountSave = amount;
    }

    public float GetBlackCurtainAmount {
        get {
            return blackCurtainAmountSave;
        }
    }

    public void SetDocument(Sprite background, Vector2 bgSize, string textHeader, int pages, bool dark = true) {
        specialPageSFXEnabled = false;
        specialPageSFXSource = null;
        document.background.color = (dark ? darkColor : Color.white);
        document.background.rectTransform.sizeDelta = bgSize;
        document.background.sprite = background;
        document.nowPage = 1;
        document.arrowLeft.enabled = false;
        document.arrowRight.enabled = true;
        document.arrowEnd.enabled = false;
        document.arrowEnd.color = Color.gray;
        document.textHeader = textHeader;
        document.numPages = pages;
        document.fragmentID = -1;
        document.fragmentPage = -1;
        document.content.text = TextManager.Get(sb.Clear().Append(document.textHeader).Append(document.nowPage.ToString("00")).ToString());
        document.specialPage = -1;
        if (document.specialImage.enabled) {
            document.specialImage.enabled = false;
        }
        OnPause(true, false, PauseType.Document);
    }

    public void SetDocumentFragment(Sprite background, Vector2 bgSize, string textHeader, int pages, bool dark = true, int fragmentID = -1, int fragmentPage = -1) {
        SetDocument(background, bgSize, textHeader, pages, dark);
        document.fragmentID = fragmentID;
        document.fragmentPage = fragmentPage;
    }

    public void SetDocumentSpecialImage(Sprite specialImage, int specialPageNum, Sprite background, Vector2 bgSize, string textHeader, int pages, bool dark = true) {
        SetDocument(background, bgSize, textHeader, pages, dark);
        document.specialPage = specialPageNum;
        document.specialImage.sprite = specialImage;
    }

    public void SetSpecialPageSFX(AudioSource sfxSource) {
        specialPageSFXEnabled = true;
        specialPageSFXSource = sfxSource;
    }

    void PlayPageSE() {
        if (specialPageSFXEnabled && specialPageSFXSource) {
            specialPageSFXSource.Play();
        } else {
            UISE.Instance.Play(UISE.SoundName.page);
        }
    }

    void UpdateDocument() {
        bool changed = false;
        bool considerSubmit = (document.nowPage > document.numPages && eventClickNum == 1);
        move = GameManager.Instance.MoveCursor(true);
        if (move.x == -1 || eventClickNum == 0 || mouseScroll > 0) {
            if (document.nowPage > 1) {
                document.nowPage--;
                changed = true;
            }
        } else if (move.x == 1 || eventClickNum == 1 || mouseScroll < 0) {
            if (document.nowPage <= document.numPages) {
                document.nowPage++;
                changed = true;
            }
        }
        if (GameManager.Instance.GetCancelButtonDown) {
            UISE.Instance.Play(UISE.SoundName.cancel);
            OnPause(false, false, PauseType.Document);
        } else if (document.nowPage > document.numPages && (playerInput.GetButtonDown(RewiredConsts.Action.Submit) || considerSubmit)) {
            PlayPageSE();
            OnPause(false, false, PauseType.Document);
        } else if (changed) {
            PlayPageSE();
            if (document.nowPage <= 1) {
                document.arrowLeft.enabled = false;
                document.arrowRight.enabled = true;
                document.arrowEnd.enabled = false;
            } else if (document.nowPage >= document.numPages) {
                document.arrowLeft.enabled = true;
                document.arrowRight.enabled = false;
                document.arrowEnd.enabled = true;
            } else {
                document.arrowLeft.enabled = true;
                document.arrowRight.enabled = true;
                document.arrowEnd.enabled = false;
            }
            if (document.nowPage > document.numPages) {
                if (document.arrowEnd.color != Color.white) {
                    document.arrowEnd.color = Color.white;
                }
            } else {
                if (document.arrowEnd.color != Color.gray) {
                    document.arrowEnd.color = Color.gray;
                }
                if (document.fragmentID >= 0 && document.nowPage >= 2) {
                    if (StageManager.Instance.IsHomeStage) {
                        int flagTemp = GameManager.Instance.save.document[document.fragmentID];
                        if ((flagTemp & (1 << (document.nowPage - 2))) != 0) {
                            document.content.text = TextManager.Get(sb.Clear().Append(document.textHeader).Append(document.nowPage.ToString("00")).ToString());
                        } else {
                            document.content.text = TextManager.Get("DOC_LOCK");
                        }
                    } else {
                        document.content.text = TextManager.Get(sb.Clear().Append(document.textHeader).Append((document.fragmentPage >= 1 ? document.fragmentPage + 1 : StageManager.Instance.stageNumber + 1).ToString("00")).ToString());
                    }
                } else {
                    document.content.text = TextManager.Get(sb.Clear().Append(document.textHeader).Append(document.nowPage.ToString("00")).ToString());
                }
            }
            if (document.specialPage > 0) {
                if (document.nowPage == document.specialPage && !document.specialImage.enabled) {
                    document.specialImage.enabled = true;
                } else if (document.nowPage != document.specialPage && document.specialImage.enabled) {
                    document.specialImage.enabled = false;
                }
            }
        }
    }

    void UpdateTutorial(bool initFlag) {
        bool changed = initFlag;
        if (tutorial.continuous) {
            move = GameManager.Instance.MoveCursor(true);
            if (move.x == -1 || eventClickNum == 0 || mouseScroll > 0) {
                if (tutorial.index > 1) {
                    tutorial.index--;
                    changed = true;
                }
            } else if (move.x == 1 || eventClickNum == 1 || mouseScroll < 0) {
                if (tutorial.index < GameManager.Instance.save.tutorial) {
                    tutorial.index++;
                    changed = true;
                }
            }
        }
        if (pausingTime >= 0.25f) {
            if (GameManager.Instance.GetCancelButtonDown) {
                UISE.Instance.Play(UISE.SoundName.cancel);
                OnPause(false, false, PauseType.Tutorial);
                if (closeTutorialToOpenMenu) {
                    OnPause(true, false, PauseType.Normal);
                }
            }
            if (playerInput.GetButtonDown(RewiredConsts.Action.Submit)) {
                if (tutorial.continuous && tutorial.index < GameManager.Instance.save.tutorial) {
                    tutorial.index++;
                    changed = true;
                } else {
                    UISE.Instance.Play(UISE.SoundName.page);
                    OnPause(false, false, PauseType.Tutorial);
                    if (closeTutorialToOpenMenu) {
                        OnPause(true, false, PauseType.Normal);
                    }
                }
            }
        }
        if (changed) {
            UISE.Instance.Play(UISE.SoundName.page);
            if (tutorial.continuous && GameManager.Instance.save.tutorial > 1) {
                if (tutorial.index <= 1) {
                    tutorial.arrowLeft.enabled = false;
                    tutorial.arrowRight.enabled = true;
                } else if (tutorial.index >= GameManager.Instance.save.tutorial) {
                    tutorial.arrowLeft.enabled = true;
                    tutorial.arrowRight.enabled = false;
                } else {
                    tutorial.arrowLeft.enabled = true;
                    tutorial.arrowRight.enabled = true;
                }
            } else {
                tutorial.arrowLeft.enabled = false;
                tutorial.arrowRight.enabled = false;
            }
            if (tutorial.index >= 1 && tutorial.index <= GameManager.Instance.save.tutorial) {
                if (tutorial.instance != null) {
                    Destroy(tutorial.instance);
                }
                tutorial.instance = Instantiate(tutorial.slots[tutorial.index - 1], tutorial.pivot);
            }
        }
    }

    void SetCombination(bool toActivate, bool isWrite) {
        HideCaution();
        if (toActivate) {
            int japaribunCount = StageManager.Instance.IsHomeStage ? 10000 : GameManager.Instance.GetJaparimanCount();
            int costLimit = CharacterManager.Instance.GetFriendsLimit(false);
            for (int i = 0; i < GameManager.friendsCombinationMax; i++) {
                combination.slots[i].SetSlot(i, costLimit, japaribunCount);
            }
            combination.readWriteText.text = isWrite ? TextManager.Get("WORD_COMBI_WRITE") : TextManager.Get("WORD_COMBI_READ");
            combination.readWriteText.colorGradientPreset = isWrite ? combination.writeColor : combination.readColor;
            combination.readWriteImage.sprite = isWrite ? combination.writeSprite : combination.readSprite;
            combination.cursor.anchoredPosition = combination.cursorOrigin + combination.cursorInterval * combination.cursorPos;
        }
        for (int i = 0; i < combination.grids.Length; i++) {
            combination.grids[i].enabled = toActivate;
        }
        if (toActivate) {
            state = isWrite ? 80 : 81;
        } else {
            state = 1;
        }
        property.canvas.enabled = !toActivate;
        property.gridLayoutGroup.enabled = !toActivate;
        property.gRaycaster.enabled = (GameManager.Instance.MouseEnabled && !toActivate);
        combination.canvas.enabled = toActivate;
        combination.gRaycaster.enabled = (GameManager.Instance.MouseEnabled && toActivate);
    }

    void UpdateCombination(bool isWrite) {
        move = GameManager.Instance.MoveCursor(true);
        if (move.y != 0 || (eventEnterNum >= 300 && eventEnterNum < 300 + combination.slots.Length)) {
            HideCaution();
            if (eventEnterNum >= 300 && eventEnterNum < 300 + combination.slots.Length) {
                combination.cursorPos = eventEnterNum - 300;
            } else {
                combination.cursorPos = (combination.cursorPos + combination.slots.Length + move.y) % combination.slots.Length;
            }
            combination.cursor.anchoredPosition = combination.cursorOrigin + combination.cursorInterval * combination.cursorPos;
            UISE.Instance.Play(UISE.SoundName.move);
        }
        if (playerInput.GetButtonDown(RewiredConsts.Action.Submit) || (eventClickNum >= 300 && eventClickNum < 300 + combination.slots.Length)) {
            if (eventClickNum >= 300 && eventClickNum < 300 + combination.slots.Length) {
                combination.cursorPos = eventClickNum - 300;
                combination.cursor.anchoredPosition = combination.cursorOrigin + combination.cursorInterval * combination.cursorPos;
            }
            if (isWrite) {
                RecordCurrentFriends(combination.cursorPos);
                SetCombination(false, isWrite);
            } else {
                if (CallRecordedFriends(combination.cursorPos)) {
                    SetCombination(false, isWrite);
                    OffPauseOrBack();
                } else {
                    UISE.Instance.Play(UISE.SoundName.error);
                }
            }
        } else if (GameManager.Instance.GetCancelButtonDown) {
            SetCombination(false, isWrite);
            UISE.Instance.Play(UISE.SoundName.cancel);
        }
    }

    void RecordCurrentFriends(int index) {
        int friendsMax = GameManager.friendsMax;
        int flag = 0;
        for (int i = 1; i < friendsMax; i++) {
            if (CharacterManager.Instance.GetFriendsExist(i, false)) {
                flag += 1 << (i - 1);
            }
        }
        GameManager.Instance.save.friendsCombination[Mathf.Clamp(index, 0, GameManager.friendsCombinationMax)] = flag;
        UISE.Instance.Play(UISE.SoundName.use);
    }

    bool CallRecordedFriends(int index) {
        int friendsMax = GameManager.friendsMax;
        bool costFlag = !StageManager.Instance.IsHomeStage;
        int japaribunCount = costFlag ? GameManager.Instance.GetJaparimanCount() : 10000;
        int summonRemain = CharacterManager.Instance.GetFriendsSummonRemain();
        int requiredCost = 0;
        int numerator = 0;
        int denominator = 0;
        int flag = GameManager.Instance.save.friendsCombination[Mathf.Clamp(index, 0, GameManager.friendsCombinationMax)];
        int[] callTargets = new int[friendsMax];
        for (int i = 1; i < friendsMax; i++) {
            if ((flag & (1 << (i - 1))) != 0) {
                if (!CharacterManager.Instance.GetFriendsExist(i, true)) {
                    requiredCost += CharacterDatabase.Instance.friends[i].cost;
                    denominator++;
                    if (!CharacterManager.Instance.GetFriendsExist(i, false)) {
                        callTargets[i] = 1;
                    } else {
                        callTargets[i] = 2;
                    }
                }
            }
        }
        if (flag == 0) {
            ShowCaution(TextManager.Get("CAUTION_FRIENDS_NORECORD"), false, false);
        } else if (requiredCost == 0) {
            ShowCaution(TextManager.Get("CAUTION_FRIENDS_CANTCALL"), false, false);
        } else if (requiredCost > summonRemain) {
            ShowCaution(TextManager.Get("CAUTION_COST"), false, false);
        } else if (requiredCost > japaribunCount) {
            ShowCaution(TextManager.Get("CAUTION_JAPARIBUN"), false, false);
        } else {
            for (int i = 1; i < friendsMax; i++) {
                if (callTargets[i] == 2) {
                    CharacterManager.Instance.Erase(i, true);
                }
            }
            int costActual = 0;
            for (int i = 1; i < friendsMax; i++) {
                if (callTargets[i] != 0) {
                    if (CharacterManager.Instance.ChangeFriends(i, true, false, false, numerator, denominator) > 0) {
                        costActual += CharacterManager.Instance.GetFriendsCost(i);
                        numerator++;
                    }
                }
            }
            if (costFlag && costActual > 0) {
                CharacterManager.Instance.UseJapariman(costActual);
            }
            return true;
        }
        return false;
    }

    public void ShowCaution(string text, bool sfx = true, bool mark = true) {
        if (sfx) {
            UISE.Instance.Play(UISE.SoundName.caution);
        }
        caution.markImage.enabled = mark;
        caution.text.text = text;
        caution.canvas.enabled = true;
        showCautionRemainFrames = 1;
    }

    public void HideCaution() {
        if (showCautionRemainFrames <= 0 && caution.canvas.enabled) {
            caution.canvas.enabled = false;
        }
    }

    public void SetTutorial(int index) {
        if (tutorial.instance != null) {
            Destroy(tutorial.instance);
        }
        if (index < 1) {
            tutorial.continuous = true;
            index = 1;
        } else {
            tutorial.continuous = false;
        }
        tutorial.index = index;
        if (pauseGame && pauseType == PauseType.Normal) {
            OnPause(false, false, PauseType.Normal);
            closeTutorialToOpenMenu = true;
        } else {
            closeTutorialToOpenMenu = false;
        }
        if (index >= 1 && index <= tutorial.slots.Length) {
            OnPause(true, false, PauseType.Tutorial);
            UpdateTutorial(true);
        }
    }

    void ResetGrassControl() {
        GameObject[] grassObjs = GameObject.FindGameObjectsWithTag("GrassControl");
        if (grassObjs.Length > 0) {
            for (int i = 0; i < grassObjs.Length; i++) {
                GrassControl grassControl = grassObjs[i].GetComponent<GrassControl>();
                if (grassControl) {
                    grassControl.SetGrassBody();
                }
            }
        }
    }

    protected override void Awake() {
        base.Awake();
        if (inGameProfiler) {
            inGameProfiler.SetProfilerEnabled(GameManager.Instance.save.config[GameManager.Save.configID_SystemInformation] != 0);
        }
        pauseEnabled = true;
        returnToLibraryProcessing = false;
        state = 0;
        typeCursor = 0;
        itemCursor = 0;
        empty = Resources.Load("empty", typeof(Sprite)) as Sprite;
    }

    void Start() {
        pCon = CharacterManager.Instance.pCon;
        playerInput = GameManager.Instance.playerInput;
        OnPause(false, false, PauseType.Buy);
        OnPause(false, false, PauseType.Normal);
        SetTypeText();
        GameManager.Instance.save.SetRunInBackground();
    }

    public void ResetPlayerController() {
        pCon = CharacterManager.Instance.pCon;
    }

    void StopBGM() {
        if (BGM.Instance) {
            BGM.Instance.Stop();
        }
        if (Ambient.Instance) {
            Ambient.Instance.Stop();
        }
    }

    void Update() {
        mouseScroll = Input.mouseScrollDelta.y;
        if (GameManager.Instance.MouseCancelling) {
            eventEnterNum = -1;
            eventClickNum = -1;
            mouseScroll = 0;
        }
        if (!firstUpdateApplySpeaker) {
            if (GameManager.Instance.ApplySpeakerMode()) {
                GameManager.Instance.ApplyVolume();
                replayAppointment = 0.2f;
            }
            firstUpdateApplySpeaker = true;
        }
        if (showCautionRemainFrames > 0) {
            showCautionRemainFrames--;
        }
        if (!pauseGame) {
            if (pauseEnabled && !choices.canvas.enabled && playerInput.GetButtonDown(RewiredConsts.Action.Pause)) {
                pauseType = PauseType.Normal;
                OnPause(true, true, PauseType.Normal);
                state = 0;
            }
        } else {
            pausingTime += Time.unscaledDeltaTime;
            if (pauseEnabled && !controlMapperWrapper.IsControlMapperActive) {
                if (pauseType == PauseType.Normal) {
                    if (playerInput.GetButtonDown(RewiredConsts.Action.Pause) || (state == 0 && GameManager.Instance.GetCancelButtonDown)) {
                        DeleteSlot();
                        OnPause(false, true, PauseType.Normal);
                    }
                } else if (pauseType == PauseType.Buy || pauseType == PauseType.Sell || pauseType == PauseType.Store || pauseType == PauseType.Takeout) {
                    if (state <= 1 && GameManager.Instance.GetCancelButtonDown) {
                        DeleteSlot();
                        OnPause(false, false, pauseType);
                        UISE.Instance.Play(UISE.SoundName.cancel);
                    }
                }
            }
            switch (state) {
                case 0:
                    SelectItemType();
                    break;
                case 1:
                    SelectItem();
                    break;
                case 2:
                    UpdateDocument();
                    break;
                case 3:
                    SelectTrophy();
                    break;
                case 4:
                    SelectFDKaban();
                    break;
                case 10:
                    DecideChoices(ChoicesType.Item);
                    break;
                case 11:
                    DecideChoices(ChoicesType.Friends);
                    break;
                case 12:
                    DecideChoices(ChoicesType.Buy);
                    break;
                case 13:
                    DecideChoices(ChoicesType.Sell);
                    break;
                case 14:
                    DecideChoices(ChoicesType.Store);
                    break;
                case 15:
                    DecideChoices(ChoicesType.TakeOut);
                    break;
                case 16:
                    StatusUpControl();
                    break;
                case 17:
                    DecideChoices(ChoicesType.FDKaban);
                    break;
                case 20:
                    VolumeControl();
                    break;
                case 21:
                    ConfigControl();
                    break;
                case 22:
                    SelectSound();
                    break;
                case 23:
                    DecideChoices(ChoicesType.Reset);
                    break;
                case 24:
                    SelectBus();
                    break;
                case 25:
                    UpdateTutorial(false);
                    break;
                case 26:
                    SelectBlend();
                    break;
                case 27:
                    DecideChoices(ChoicesType.Language);
                    break;
                case 30:
                    DecideChoices(ChoicesType.Quit);
                    break;
                case 31:
                    ReturnLibrary();
                    break;
                case 41:
                    DecideChoices(ChoicesType.Command);
                    break;
                case 50:
                    // Photo
                    break;
                case 60: // Key Bindings
                    if (!controlMapperWrapper.IsControlMapperActive) {
                        baseCanvas.enabled = true;
                        UISE.Instance.Play(UISE.SoundName.submit);
                        state = 1;
                    }
                    break;
                case 70: //Dictionary
                    break;
                case 80: //Combination
                    UpdateCombination(true);
                    break;
                case 81:
                    UpdateCombination(false);
                    break;

            }
        }
        // Gameover stuck buf countermeasures
        if (pauseGame && !pauseEnabled && pauseType == PauseType.Normal && pCon && pCon.GetNowHP() <= 0) {
            DeleteSlot();
            OnPause(false, true, PauseType.Normal);
        }
        if (IsPhotoPausing){
            if (frameAdvanceEnabled) {
                if (playerInput.GetButton(RewiredConsts.Action.Jump) || playerInput.GetButton(RewiredConsts.Action.Special)) {
                    frameAdvancePressTime += Time.unscaledDeltaTime;
                } else {
                    frameAdvancePressTime = 0f;
                }
                frameAdvanceFlag = (playerInput.GetButtonDown(RewiredConsts.Action.Jump) || playerInput.GetButtonDown(RewiredConsts.Action.Special) || frameAdvancePressTime >= 0.5f);
                float frameAdvanceScale = frameAdvanceFlag ? (playerInput.GetButton(RewiredConsts.Action.Dodge) ? 1f : 0.25f) : 0f;
                if (Time.timeScale != frameAdvanceScale) {
                    Time.timeScale = frameAdvanceScale;
                }
            } else {
                if (playerInput.GetButtonDown(RewiredConsts.Action.Jump) || playerInput.GetButtonDown(RewiredConsts.Action.Special)) {
                    frameAdvanceEnabled = true;
                }
            }
            if (playerInput.GetButtonDown(RewiredConsts.Action.Cancel)) {
                PausePhoto(false);
            }
        }
        if (replayAppointment > 0) {
            replayAppointment -= Time.unscaledDeltaTime;
            if (replayAppointment <= 0) {
                if (BGM.Instance) {
                    BGM.Instance.Replay();
                }
                if (Ambient.Instance) {
                    Ambient.Instance.Replay();
                }
                GameManager.Instance.ApplyVolume();
                GameManager.Instance.RestartSnapshot();
            }
        }
        // SpecialMove Tutorial
        if (pauseEnabled && !pauseGame) {
            for (int i = 0; i < skillTutorialProgress.Length; i++) {
                if (skillTutorialProgress[i] >= 2) {
                    GameManager.Instance.SetTutorial_SpecialMove(i);
                    skillTutorialProgress[i] = 0;
                    break;
                }
            }
        }
        if (pauseGame) {
            notPausingFrames = 0;
        } else {
            if (notPausingFrames < 60) {
                notPausingFrames++;
            }
        }
    }

    private void LateUpdate() {
        eventEnterNum = -1;
        eventClickNum = -1;
        if (GameManager.Instance){
            bool answer = (GameManager.Instance.save.config[GameManager.Save.configID_RunInBackground] != 0);
            if (Application.runInBackground != answer) {
                Application.runInBackground = answer;
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

    public void VolumeSliderChanged(int type) {
        int newNum = Mathf.RoundToInt(volume.slider[type].value);
        if (state == 20) {
            bool changed = false;
            switch (type) {
                case 0:
                    if (GameManager.Instance.save.musicVolume != newNum) {
                        GameManager.Instance.save.musicVolume = newNum;
                        changed = true;
                    }
                    break;
                case 1:
                    if (GameManager.Instance.save.seVolume != newNum) {
                        GameManager.Instance.save.seVolume = newNum;
                        changed = true;
                    }
                    break;
                case 2:
                    if (GameManager.Instance.save.ambientVolume != newNum) {
                        GameManager.Instance.save.ambientVolume = newNum;
                        changed = true;
                    }
                    break;
            }
            if (changed) {
                UISE.Instance.Play(UISE.SoundName.move);
                SetVolumeText(type, newNum);
                GameManager.Instance.ApplyVolume();
            }
        } else {
            if (statusUpSave[type] != newNum) {
                statusUpSave[type] = newNum;
                UISE.Instance.Play(UISE.SoundName.move);
                SetStatusUpText(type, newNum);
            }
        }
    }

    private void SetShowArms() {
        bool weaponVisible = GameManager.Instance.save.config[GameManager.Save.configID_ShowArms] >= 0;
        GameObject[] weaponObjs = GameObject.FindGameObjectsWithTag("FriendsWeapon");
        if (weaponObjs.Length > 0) {
            for (int i = 0; i < weaponObjs.Length; i++) {
                HoldWeaponObject holdWeapon = weaponObjs[i].GetComponent<HoldWeaponObject>();
                if (holdWeapon) {
                    holdWeapon.SetWeaponActive(weaponVisible);
                }
            }
        }
    }

    public void SetFacilityObj(int id, GameObject obj) {
        for (int i = 0; i < fdKabanIDArray.Length; i++) {
            if (id == fdKabanIDArray[i]) {
                if (facilityObjs[i]) {
                    Destroy(facilityObjs[i]);
                }
                facilityObjs[i] = obj;
                break;
            }
        }
    }

    void TakeoutFacility(int id) {
        Vector3 pos = CharacterManager.Instance.playerTrans.position;
        Quaternion rot = CharacterManager.Instance.playerTrans.rotation;
        bool found = false;
        int index = -1;
        for (int i = 0; i < fdKabanIDArray.Length; i++) {
            if (id == fdKabanIDArray[i]) {
                index = i;
                if (facilityObjs[i]) {
                    facilityObjs[i].SetActive(false);
                    FacilityInformation info = facilityObjs[i].GetComponent<FacilityInformation>();
                    if (info) {
                        if (info.floorHeight != 0f) {
                            pos += vecUp * info.floorHeight;
                        }
                        if (info.playerHeight != 0f) {
                            CharacterManager.Instance.playerTrans.position += vecUp * info.playerHeight;
                        }
                        if (info.forwardOffset != 0f) {
                            pos += CharacterManager.Instance.playerTrans.forward * info.forwardOffset;
                        }
                        if (info.rotationY != 0f) {
                            Vector3 eulerTemp = rot.eulerAngles;
                            eulerTemp.y += info.rotationY;
                            rot = Quaternion.Euler(eulerTemp);
                        }
                    }
                    ActivateExceptHome[] exceptHome = facilityObjs[i].GetComponents<ActivateExceptHome>();
                    if (exceptHome.Length > 0) {
                        for (int j = 0; j < exceptHome.Length; j++) {
                            exceptHome[j].ActivateExternal(false);
                        }
                    }
                    facilityObjs[i].transform.SetPositionAndRotation(pos, rot);
                    facilityObjs[i].SetActive(true);
                    found = true;
                    break;
                }
            }
        }
        if (!found && index >= 0) {
            GameObject prefab;
            switch (id) {
                case fdKabanDummyID:
                    prefab = CharacterDatabase.Instance.GetEnemy(dummyEnemyID);
                    break;
                case fdKabanShopID:
                    prefab = ItemDatabase.Instance.GetItemPrefab(shopIfrID);
                    break;
                default:
                    prefab = ItemDatabase.Instance.GetItemPrefab(ItemDatabase.facilityBottom + id);
                    break;
            }
            if (prefab) {
                FacilityInformation info = prefab.GetComponent<FacilityInformation>();
                if (info) {
                    if (info.floorHeight != 0f) {
                        pos += vecUp * info.floorHeight;
                    }
                    if (info.playerHeight != 0f) {
                        CharacterManager.Instance.playerTrans.position += vecUp * info.playerHeight;
                    }
                    if (info.forwardOffset != 0f) {
                        pos += CharacterManager.Instance.playerTrans.forward * info.forwardOffset;
                    }
                    if (info.rotationY != 0f) {
                        Vector3 eulerTemp = rot.eulerAngles;
                        eulerTemp.y += info.rotationY;
                        rot = Quaternion.Euler(eulerTemp);
                    }
                }
                facilityObjs[index] = Instantiate(prefab, pos, rot, StageManager.Instance && StageManager.Instance.dungeonController ? StageManager.Instance.dungeonController.transform : null);
                ActivateExceptHome[] exceptHome = facilityObjs[index].GetComponents<ActivateExceptHome>();
                if (exceptHome.Length > 0) {
                    for (int j = 0; j < exceptHome.Length; j++) {
                        exceptHome[j].ActivateExternal(false);
                    }
                }
            }
        }

    }

    bool StoreFacility(int id) {
        for (int i = 0; i < fdKabanIDArray.Length; i++) {
            if (id == fdKabanIDArray[i]) {
                if (facilityObjs[i]) {
                    Destroy(facilityObjs[i]);
                    facilityObjs[i] = null;
                    return true;
                }
            }
        }
        return false;
    }

    public bool GetNowPausing() {
        return pauseGame || choices.canvas.enabled || (SaveController.Instance && SaveController.Instance.save.canvas.enabled);
    }

    public void ItemAutomaticUse(int id) {
        if (!itemDisabled && UseItem(id)) {
            GameManager.Instance.save.items.Remove(id);
            if (MessageUI.Instance) {
                MessageUI.Instance.SetMessage(
                    sb.Clear()
                    .Append(TextManager.Get("QUOTE_START"))
                    .Append(ItemDatabase.Instance.GetItemName(id))
                    .Append(TextManager.Get("QUOTE_END"))
                    .Append(TextManager.Get("MESSAGE_AUTOMATICUSE"))
                    .ToString(), MessageUI.time_Default, MessageUI.panelType_Information, MessageUI.slotType_Auto);
            }
        }
    }

    public string GetConfigNameWithIndex(int index) {
        for (int i = configStart.Length - 1; i >= 0; i--) {
            if (index >= configStart[i]) {
                return TextManager.Get(StringUtils.Format("CONFIG_NAME_{0}_{1}", i, index - configStart[i]));
            }
        }
        return "";
    }
    
    public bool GetPauseEnabled() {
        return pauseEnabled && !choices.canvas.enabled;
    }

}
