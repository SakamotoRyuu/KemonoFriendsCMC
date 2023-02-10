using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CharacterDatabase : SingletonMonoBehaviour<CharacterDatabase> {

    const int enemyLevelMax = 5;    

    [System.Serializable]
    public class Friends {
        public string path;
        public int cost;
        public GameObject cache;
    }
    
    public class EnemyStatus {
        public int hp;
        public int attack;
        public int defense;
        public int exp;
        public int item0;
        public int item1;
        public int assumeLevel;
        public EnemyStatus() {
            hp = 50;
            attack = 10;
            defense = 0;
            exp = 1;
            item0 = -1;
            item1 = -1;
            assumeLevel = 1;
        }
        public EnemyStatus(int hp, int attack, int defense, int exp, int item0 = -1, int item1 = -1, int assumeLevel = 1) {
            this.hp = hp;
            this.attack = attack;
            this.defense = defense;
            this.exp = exp;
            this.item0 = item0;
            this.item1 = item1;
            this.assumeLevel = assumeLevel;
        }
    }

    [System.Serializable]
    public class VariableStatus {
        public int noCoreHP;
        public int haveCoreHP;
        public int attack;
        public int exp;
        public VariableStatus() {
            noCoreHP = 50;
            haveCoreHP = 50;
            attack = 10;
            exp = 1;
        }
        public VariableStatus(int noCoreHP, int haveCoreHP, int attack, int exp) {
            this.noCoreHP = noCoreHP;
            this.haveCoreHP = haveCoreHP;
            this.attack = attack;
            this.exp = exp;
        }
    }

    [System.Serializable]
    public class Enemy {
        public string path;
        public string spritePath;
        public int maxLevel;
        public bool[] statusExist = new bool[enemyLevelMax];
        public bool[] appearInBook = new bool[enemyLevelMax];
        public EnemyStatus[] status = new EnemyStatus[enemyLevelMax];
        public GameObject cache;
        public Sprite spriteCache;
    }

    [System.Serializable]
    public class NPC {
        public string path;
        public GameObject cache;
    }

    /*
    [System.Serializable]
    public class Facility {
        public int referBit;
        public string path;
        public string spritePath;
        public GameObject cache;
        public Sprite spriteCache;
    }
    */

    [System.Serializable]
    public class AnimCon {
        public string path;
        public RuntimeAnimatorController cache;
    }

    [System.Serializable]
    public class UICache {
        public GameObject balloonSerif;
        public GameObject damagePlayer;
        public GameObject damageHeal;
        public GameObject damageEnemy;
        public GameObject damageCritical;
        public GameObject damageBack;
        public GameObject damageEffective;
        public GameObject damageHyper;
        public GameObject damageHard;
        public GameObject damageHardBack;
        public GameObject enemyCanvas;
        public GameObject justDodgeHigh;
        public GameObject justDodgeMiddle;
        public GameObject justDodgeLow;
        public GameObject getExp;
        public GameObject gutsNormal;
        public GameObject gutsLast;
        public GameObject defeatRemain;
        public GameObject fieldBuffHP;
        public GameObject fieldBuffST;
        public GameObject fieldBuffAttack;
    }
    
    public Friends[] friends;
    public Enemy[] enemy;
    public VariableStatus[] variableStatus;
    public NPC[] npc;
    public UICache ui;
    public AnimCon[] animCon;
    // public Facility[] facility;

    public static readonly int[] sinWREnemyIDArray = { 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 52, 55, 56, 57, 58, 59, 60, 64, 65, 66 };
    public const int sandstarRawLevel = 4;
    public const int amusementBossLevel = 1;

    protected override void Awake() {
        if (CheckInstance()) {
            DontDestroyOnLoad(gameObject);
            SetCharacterData();
            LoadEnemyTable();
            LoadStatusTable();
            SetUICache();
        }
    }

    void SetCharacterData() {
        SetFriends(1, "Prefab/Friends/001_Kaban", 1);
        SetFriends(2, "Prefab/Friends/002_Hippo", 2);
        SetFriends(3, "Prefab/Friends/003_Otter", 1);
        SetFriends(4, "Prefab/Friends/004_Jaguar", 3);
        SetFriends(5, "Prefab/Friends/005_Ibis", 2);
        SetFriends(6, "Prefab/Friends/006_Alpaca", 1);
        SetFriends(7, "Prefab/Friends/007_SandCat", 2);
        SetFriends(8, "Prefab/Friends/008_Tsuchinoko", 3);
        SetFriends(9, "Prefab/Friends/009_Beaver", 2);
        SetFriends(10, "Prefab/Friends/010_PrairieDog", 2);
        SetFriends(11, "Prefab/Friends/011_Moose", 5);
        SetFriends(12, "Prefab/Friends/012_Lion", 5);
        SetFriends(13, "Prefab/Friends/013_WhiteOwl", 4);
        SetFriends(14, "Prefab/Friends/014_EagleOwl", 4);
        SetFriends(15, "Prefab/Friends/015_Margay", 1);
        SetFriends(16, "Prefab/Friends/016_Princess", 1);
        SetFriends(17, "Prefab/Friends/017_Rocker", 1);
        SetFriends(18, "Prefab/Friends/018_Gean", 1);
        SetFriends(19, "Prefab/Friends/019_Hululu", 3);
        SetFriends(20, "Prefab/Friends/020_Emperor", 2);
        SetFriends(21, "Prefab/Friends/021_RedFox", 2);
        SetFriends(22, "Prefab/Friends/022_SilverFox", 3);
        SetFriends(23, "Prefab/Friends/023_CampoFlicker", 2);
        SetFriends(24, "Prefab/Friends/024_GrayWolf", 3);
        SetFriends(25, "Prefab/Friends/025_Giraffe", 3);
        SetFriends(26, "Prefab/Friends/026_GoldenMonkey", 4);
        SetFriends(27, "Prefab/Friends/027_BrownBear", 6);
        SetFriends(28, "Prefab/Friends/028_PaintedWolf", 4);
        SetFriends(29, "Prefab/Friends/029_Raccoon", 2);
        SetFriends(30, "Prefab/Friends/030_Fennec", 2);
        SetFriends(31, "Prefab/Friends/031_AnotherServal", 10);
        SetFriends(32, "Prefab/Friends/032_Serval", 8);
        SetFriends(33, "Prefab/Friends/033_HyperServal", 33);
        
        enemy[0].path = "Prefab/Enemy/000_WingBolt";
        enemy[1].path = "Prefab/Enemy/001_PlugAdapter";
        enemy[2].path = "Prefab/Enemy/002_SmartBall";
        enemy[3].path = "Prefab/Enemy/003_Debris";
        enemy[4].path = "Prefab/Enemy/004_Amoeba";
        enemy[5].path = "Prefab/Enemy/005_Dissodinium";
        enemy[6].path = "Prefab/Enemy/006_Ceratium";
        enemy[7].path = "Prefab/Enemy/007_Ornithocercus";
        enemy[8].path = "Prefab/Enemy/008_Desmodesmus";
        enemy[9].path = "Prefab/Enemy/009_Chaetoceros";
        enemy[10].path = "Prefab/Enemy/010_Euglena";
        enemy[11].path = "Prefab/Enemy/011_Volvox";
        enemy[12].path = "Prefab/Enemy/012_Alexandrium";
        enemy[13].path = "Prefab/Enemy/013_Ambigolimax";
        enemy[14].path = "Prefab/Enemy/014_Camponotus";
        enemy[15].path = "Prefab/Enemy/015_Anotogaster";
        enemy[16].path = "Prefab/Enemy/016_Scorpiones";
        enemy[17].path = "Prefab/Enemy/017_ScorpionesMagnus";
        enemy[18].path = "Prefab/Enemy/018_Araneus";
        enemy[19].path = "Prefab/Enemy/019_Aurelia";
        enemy[20].path = "Prefab/Enemy/020_Daphnia";
        enemy[21].path = "Prefab/Enemy/021_Aedes";
        enemy[22].path = "Prefab/Enemy/022_Tenodera";
        enemy[23].path = "Prefab/Enemy/023_Polistes";
        enemy[24].path = "Prefab/Enemy/024_Parantica";
        enemy[25].path = "Prefab/Enemy/025_Birdlien";
        enemy[26].path = "Prefab/Enemy/026_Enypniastes";
        enemy[27].path = "Prefab/Enemy/027_Clione";
        enemy[28].path = "Prefab/Enemy/028_Bathynomus";
        enemy[29].path = "Prefab/Enemy/029_Oligochaeta";
        enemy[30].path = "Prefab/Enemy/030_Ramazzottius";
        enemy[31].path = "Prefab/Enemy/031_Ammonitida";
        enemy[32].path = "Prefab/Enemy/032_Amanita";
        enemy[33].path = "Prefab/Enemy/033_Periplaneta";
        enemy[34].path = "Prefab/Enemy/034_Anomalocaris";
        enemy[35].path = "Prefab/Enemy/035_Halocynthia";
        enemy[36].path = "Prefab/Enemy/036_Dunkleosteus";
        enemy[37].path = "Prefab/Enemy/037_TRex";
        enemy[38].path = "Prefab/Enemy/038_Eomaia";
        enemy[39].path = "Prefab/Enemy/039_Australopithecus";
        enemy[40].path = "Prefab/Enemy/040_AlligatorClips";
        enemy[41].path = "Prefab/Enemy/041_DarkServal1";
        enemy[42].path = "Prefab/Enemy/042_B-2";
        enemy[43].path = "Prefab/Enemy/043_RedGrowl";
        enemy[44].path = "Prefab/Enemy/044_Akula";
        enemy[45].path = "Prefab/Enemy/045_DarkServal2";
        enemy[46].path = "Prefab/Enemy/046_CeckyBeast";
        enemy[47].path = "Prefab/Enemy/047_Glaucus";
        enemy[48].path = "Prefab/Enemy/048_SnowTower";
        enemy[49].path = "Prefab/Enemy/049_Queen";
        enemy[50].path = "Prefab/Enemy/050_DarkServal_SP1";
        enemy[51].path = "Prefab/Enemy/051_DarkServal_SP2";
        enemy[52].path = "Prefab/Enemy/052_DarkServal3";
        enemy[53].path = "Prefab/Enemy/053_HomoSapiens";
        enemy[54].path = "Prefab/Enemy/054_Dummy";
        enemy[55].path = "Prefab/Enemy/055_AlligatorClipsRaw";
        enemy[56].path = "Prefab/Enemy/056_B-2Raw";
        enemy[57].path = "Prefab/Enemy/057_RedGrowlRaw";
        enemy[58].path = "Prefab/Enemy/058_CeckyBeastRaw";
        enemy[59].path = "Prefab/Enemy/059_SnowTowerRaw";
        enemy[60].path = "Prefab/Enemy/060_BigDog";
        enemy[61].path = "Prefab/Enemy/061_BigDogEX";
        enemy[62].path = "Prefab/Enemy/062_BigDogInside";
        enemy[63].path = "Prefab/Enemy/063_QueenRaw";
        enemy[64].path = "Prefab/Enemy/064_Heliozoa";
        enemy[65].path = "Prefab/Enemy/065_Empress";
        enemy[66].path = "Prefab/Enemy/066_ImperatrixMundi";
        enemy[0].spritePath = "Enemy/face_000_WingBolt";
        enemy[1].spritePath = "Enemy/face_001_PlugAdapter";
        enemy[2].spritePath = "Enemy/face_002_SmartBall";
        enemy[3].spritePath = "Enemy/face_003_Debris";
        enemy[4].spritePath = "Enemy/face_004_Amoeba";
        enemy[5].spritePath = "Enemy/face_005_Dissodinium";
        enemy[6].spritePath = "Enemy/face_006_Ceratium";
        enemy[7].spritePath = "Enemy/face_007_Ornithocercus";
        enemy[8].spritePath = "Enemy/face_008_Desmodesmus";
        enemy[9].spritePath = "Enemy/face_009_Chaetoceros";
        enemy[10].spritePath = "Enemy/face_010_Euglena";
        enemy[11].spritePath = "Enemy/face_011_Volvox";
        enemy[12].spritePath = "Enemy/face_012_Alexandrium";
        enemy[13].spritePath = "Enemy/face_013_Ambigolimax";
        enemy[14].spritePath = "Enemy/face_014_Camponotus";
        enemy[15].spritePath = "Enemy/face_015_Anotogaster";
        enemy[16].spritePath = "Enemy/face_016_Scorpiones";
        enemy[17].spritePath = "Enemy/face_017_ScorpionesMagnus";
        enemy[18].spritePath = "Enemy/face_018_Araneus";
        enemy[19].spritePath = "Enemy/face_019_Aurelia";
        enemy[20].spritePath = "Enemy/face_020_Daphnia";
        enemy[21].spritePath = "Enemy/face_021_Aedes";
        enemy[22].spritePath = "Enemy/face_022_Tenodera";
        enemy[23].spritePath = "Enemy/face_023_Polistes";
        enemy[24].spritePath = "Enemy/face_024_Parantica";
        enemy[25].spritePath = "Enemy/face_025_Birdlien";
        enemy[26].spritePath = "Enemy/face_026_Enypniastes";
        enemy[27].spritePath = "Enemy/face_027_Clione";
        enemy[28].spritePath = "Enemy/face_028_Bathynomus";
        enemy[29].spritePath = "Enemy/face_029_Oligochaeta";
        enemy[30].spritePath = "Enemy/face_030_Ramazzottius";
        enemy[31].spritePath = "Enemy/face_031_Ammonitida";
        enemy[32].spritePath = "Enemy/face_032_Amanita";
        enemy[33].spritePath = "Enemy/face_033_Periplaneta";
        enemy[34].spritePath = "Enemy/face_034_Anomalocaris";
        enemy[35].spritePath = "Enemy/face_035_Halocynthia";
        enemy[36].spritePath = "Enemy/face_036_Dunkleosteus";
        enemy[37].spritePath = "Enemy/face_037_TRex";
        enemy[38].spritePath = "Enemy/face_038_Eomaia";
        enemy[39].spritePath = "Enemy/face_039_Australopithecus";
        enemy[40].spritePath = "Enemy/face_040_AlligatorClips";
        enemy[41].spritePath = "Enemy/face_041_DarkServal1";
        enemy[42].spritePath = "Enemy/face_042_B-2";
        enemy[43].spritePath = "Enemy/face_043_RedGrowl";
        enemy[44].spritePath = "Enemy/face_044_Akula";
        enemy[45].spritePath = "Enemy/face_045_DarkServal2";
        enemy[46].spritePath = "Enemy/face_046_CeckyBeast";
        enemy[47].spritePath = "Enemy/face_047_Glaucus";
        enemy[48].spritePath = "Enemy/face_048_SnowTower";
        enemy[49].spritePath = "Enemy/face_049_Queen";
        enemy[50].spritePath = "Enemy/face_050_DarkServal_SP1";
        enemy[51].spritePath = "Enemy/face_051_DarkServal_SP2";
        enemy[52].spritePath = "Enemy/face_052_DarkServal3";
        enemy[53].spritePath = "Enemy/face_053_HomoSapiens";
        enemy[54].spritePath = "Enemy/face_054_Dummy";
        enemy[55].spritePath = "Enemy/face_055_AlligatorClipsRaw";
        enemy[56].spritePath = "Enemy/face_056_B-2Raw";
        enemy[57].spritePath = "Enemy/face_057_RedGrowlRaw";
        enemy[58].spritePath = "Enemy/face_058_CeckyBeastRaw";
        enemy[59].spritePath = "Enemy/face_059_SnowTowerRaw";
        enemy[60].spritePath = "Enemy/face_060_BigDog";
        enemy[61].spritePath = "Enemy/face_061_BigDogEX";
        enemy[62].spritePath = "Enemy/face_062_BigDogInside";
        enemy[63].spritePath = "Enemy/face_063_QueenRaw";
        enemy[64].spritePath = "Enemy/face_064_Heliozoa";
        enemy[65].spritePath = "Enemy/face_065_Empress";
        enemy[66].spritePath = "Enemy/face_066_ImperatrixMundi";
        for (int i = 0; i < 67; i++) {
            enemy[i].maxLevel = 4;
        }
        enemy[50].maxLevel = 0;
        enemy[51].maxLevel = 0;
        enemy[53].maxLevel = 0;
        enemy[61].maxLevel = 0;
        enemy[63].maxLevel = 0;
        for (int i = 0; i < 40; i++) {
            for (int j = 1; j < enemy[i].appearInBook.Length; j++) {
                enemy[i].appearInBook[j] = true;
            }
        }
        for (int i = 40; i < 67; i++) {
            enemy[i].appearInBook[0] = true;
        }
        enemy[39].appearInBook[0] = true;
        enemy[66].appearInBook[4] = true;

        npc[0].path = "Prefab/NPC/000_LuckyBeast";
        npc[1].path = "Prefab/NPC/001_Fossa";
        npc[2].path = "Prefab/NPC/002_Tamandua";
        npc[3].path = "Prefab/NPC/003_ScarletIbis";
        npc[4].path = "Prefab/NPC/004_Oryx";
        npc[5].path = "Prefab/NPC/005_Elephant";
        npc[6].path = "Prefab/NPC/006_AxisDeer";
        npc[7].path = "Prefab/NPC/007_Porcupine";
        npc[8].path = "Prefab/NPC/008_Aurochs";
        npc[9].path = "Prefab/NPC/009_Armadillo";
        npc[10].path = "Prefab/NPC/010_Chameleon";
        npc[11].path = "Prefab/NPC/011_Shoebill";
        npc[12].path = "Prefab/NPC/012_GiantPenguin";
        npc[13].path = "Prefab/NPC/013_Capybara";
        npc[14].path = "Prefab/NPC/014_Tapir";
        npc[15].path = "Prefab/NPC/015_Aardwolf";
        npc[16].path = "Prefab/NPC/016_Dolphin";
        npc[17].path = "Prefab/NPC/017_LuckyBeastGenbu";
        npc[18].path = "Prefab/NPC/018_LuckyBeastType3";
        npc[19].path = "Prefab/NPC/019_Nana";
        npc[20].path = "Prefab/NPC/020_Mirai";
        npc[21].path = "Prefab/NPC/021_Kako";
        npc[22].path = "Prefab/NPC/022_SecondServal";
        npc[23].path = "Prefab/NPC/023_Oinarisama";
        npc[24].path = "Prefab/NPC/024_Kitsunezaki";


        animCon[1].path = "AnimCon/Kaban";
        animCon[2].path = "AnimCon/Hippo";
        animCon[3].path = "AnimCon/Otter";
        animCon[4].path = "AnimCon/Jaguar";
        animCon[5].path = "AnimCon/Ibis";
        animCon[6].path = "AnimCon/Alpaca";
        animCon[7].path = "AnimCon/SandCat";
        animCon[8].path = "AnimCon/Tsuchinoko";
        animCon[9].path = "AnimCon/Beaver";
        animCon[10].path = "AnimCon/PrairieDog";
        animCon[11].path = "AnimCon/Moose";
        animCon[12].path = "AnimCon/Lion";
        animCon[13].path = "AnimCon/WhiteOwl";
        animCon[14].path = "AnimCon/EagleOwl";
        animCon[15].path = "AnimCon/Margay";
        animCon[16].path = "AnimCon/PPP_Female";
        animCon[17].path = "AnimCon/PPP_Male";
        animCon[21].path = "AnimCon/RedFox";
        animCon[22].path = "AnimCon/SilverFox";
        animCon[23].path = "AnimCon/CampoFlicker";
        animCon[24].path = "AnimCon/GrayWolf";
        animCon[25].path = "AnimCon/Giraffe";
        animCon[26].path = "AnimCon/GoldenMonkey";
        animCon[27].path = "AnimCon/BrownBear";
        animCon[28].path = "AnimCon/PaintedWolf";
        animCon[29].path = "AnimCon/Raccoon";
        animCon[30].path = "AnimCon/Fennec";
        animCon[31].path = "AnimCon/AnotherServal";        
        animCon[32].path = "AnimCon/IFR_Kaban";
        animCon[33].path = "AnimCon/IFR_Hippo";
        animCon[34].path = "AnimCon/IFR_Otter";
        animCon[35].path = "AnimCon/IFR_Jaguar";
        animCon[36].path = "AnimCon/IFR_Ibis";
        animCon[37].path = "AnimCon/IFR_Alpaca";
        animCon[38].path = "AnimCon/IFR_SandCat";
        animCon[39].path = "AnimCon/IFR_Tsuchinoko";
        animCon[40].path = "AnimCon/IFR_PrairieDog";
        animCon[41].path = "AnimCon/IFR_Moose";
        animCon[42].path = "AnimCon/IFR_Lion";
        animCon[43].path = "AnimCon/IFR_EagleOwl";
        animCon[44].path = "AnimCon/IFR_CampoFlicker";
        animCon[45].path = "AnimCon/IFR_GrayWolf";
        animCon[46].path = "AnimCon/IFR_Giraffe";
        animCon[47].path = "AnimCon/IFR_GoldenMonkey";
        animCon[48].path = "AnimCon/IFR_BrownBear";
        animCon[49].path = "AnimCon/IFR_PaintedWolf";
        animCon[50].path = "AnimCon/IFR_AnotherServal";
        animCon[51].path = "AnimCon/IFR_LuckyBeast";
        animCon[52].path = "AnimCon/IFR_Fossa";
        animCon[53].path = "AnimCon/IFR_Tamandua";
        animCon[54].path = "AnimCon/IFR_Oryx";
        animCon[55].path = "AnimCon/IFR_AxisDeer";
        animCon[56].path = "AnimCon/IFR_Aurochs";
        animCon[57].path = "AnimCon/IFR_Shoebill";
        animCon[58].path = "AnimCon/IFR_GiantPenguin";
        animCon[59].path = "AnimCon/IFR_Dolphin";
        animCon[60].path = "AnimCon/IFR_LuckyBeastGenbu";
        animCon[61].path = "AnimCon/IFR_LuckyBeastType3";
        animCon[62].path = "AnimCon/Serval";
        animCon[63].path = "AnimCon/IFR_SitChair";
        animCon[64].path = "AnimCon/IFR_IbisAp";
        animCon[65].path = "AnimCon/IFR_MargayAp";
        animCon[66].path = "AnimCon/IFR_Kitsunezaki";

        /*
        facility[0].path = "Prefab/Facility/JapariLibrary";
        facility[1].path = "Prefab/Facility/Pool";
        facility[2].path = "Prefab/Facility/Suberidai";
        facility[3].path = "Prefab/Facility/JunglePlant_Library";
        facility[4].path = "Prefab/Facility/JapariCafe";
        facility[5].path = "Prefab/Facility/DesertCave";
        facility[6].path = "Prefab/Facility/LogHouse";
        facility[7].path = "Prefab/Facility/ItemBox";
        facility[8].path = "Prefab/Facility/MogisenButai";
        facility[9].path = "Prefab/Facility/LionCastle";
        facility[10].path = "Prefab/Facility/PPP_Butai";
        facility[11].path = "Prefab/Facility/MusicBox";
        facility[12].path = "Prefab/Facility/Onsen";
        facility[13].path = "Prefab/Facility/VideoGameConsole";
        facility[14].path = "Prefab/Facility/VideoGameCase";
        facility[15].path = "Prefab/Facility/LodgeRoom";
        facility[16].path = "Prefab/Facility/Illust_Giraffe";
        facility[17].path = "Prefab/Facility/Illust_CampoFlicker";
        facility[18].path = "Prefab/Facility/Pencil";
        facility[19].path = "Prefab/Facility/Kitchen";
        facility[20].path = "Prefab/Facility/KitchenFire";
        facility[21].path = "Prefab/Facility/MiniInductionCooker";
        facility[22].path = "Prefab/Facility/BusLikeSet";
        facility[23].path = "Prefab/Facility/BusLikeOnly";
        facility[24].path = "Prefab/Facility/JapariBus";
        facility[25].path = "Prefab/Facility/AnotherServal_Sleeping";
        facility[26].path = "Prefab/Facility/Statue_Blue";
        facility[27].path = "Prefab/Facility/Statue_Red";
        facility[28].path = "Prefab/Facility/Statue_Violet";
        facility[29].path = "Prefab/Facility/Statue_Black";
        facility[30].path = "Prefab/Facility/Statue_Silver";
        facility[31].path = "Prefab/Facility/Statue_Golden";
        facility[32].path = "Prefab/Facility/Statue_Complex";
        facility[33].path = "Prefab/Facility/Sushizanmai";
        facility[34].path = "Prefab/Facility/DictionaryBook";
        facility[35].path = ""; //Dummy
        facility[36].path = "Prefab/Facility/DifficultyBook";
        facility[37].path = "Prefab/Facility/TutorialBook";
        facility[0].referBit = 0;
        facility[1].referBit = 1;
        facility[2].referBit = 2;
        facility[3].referBit = 3;
        facility[4].referBit = 4;
        facility[5].referBit = 5;
        facility[6].referBit = 6;
        facility[7].referBit = 7;
        facility[8].referBit = 8;
        facility[9].referBit = 9;
        facility[10].referBit = 10;
        facility[11].referBit = 11;
        facility[12].referBit = 12;
        facility[13].referBit = 13;
        facility[14].referBit = 13;
        facility[15].referBit = 14;
        facility[16].referBit = 14;
        facility[17].referBit = 14;
        facility[18].referBit = 14;
        facility[19].referBit = 15;
        facility[20].referBit = 15;
        facility[21].referBit = 16;
        facility[22].referBit = 17;
        facility[23].referBit = 17;
        facility[24].referBit = 18;
        facility[25].referBit = -1;
        facility[26].referBit = 19;
        facility[27].referBit = 19;
        facility[28].referBit = 19;
        facility[29].referBit = 19;
        facility[30].referBit = 19;
        facility[31].referBit = 19;
        facility[32].referBit = 19;
        facility[33].referBit = 20;
        facility[34].referBit = -1;
        facility[35].referBit = 21;
        facility[36].referBit = -1;
        facility[0].spritePath = "Facility/face_JapariLibrary";
        facility[1].spritePath = "Facility/face_Pool";
        facility[2].spritePath = "Facility/face_Suberidai";
        facility[3].spritePath = "Facility/face_JunglePlant";
        facility[4].spritePath = "Facility/face_JapariCafe";
        facility[5].spritePath = "Facility/face_DesertCave";
        facility[6].spritePath = "Facility/face_LogHouse";
        facility[7].spritePath = "Facility/face_ItemBox";
        facility[8].spritePath = "Facility/face_MogisenButai";
        facility[9].spritePath = "Facility/face_LionCastle";
        facility[10].spritePath = "Facility/face_PPP_Butai";
        facility[11].spritePath = "Facility/face_MusicBox";
        facility[12].spritePath = "Facility/face_Onsen";
        facility[14].spritePath = "Facility/face_VideoGame";
        facility[15].spritePath = "Facility/face_LodgeRoom";
        facility[19].spritePath = "Facility/face_Kitchen";
        facility[21].spritePath = "Facility/face_MiniInductionCooker";
        facility[23].spritePath = "Facility/face_BusLike";
        facility[24].spritePath = "Facility/face_JapariBus";
        facility[32].spritePath = "Facility/face_Statue";
        facility[33].spritePath = "Facility/face_Sushizanmai";
        facility[34].spritePath = "Facility/face_DictionaryBook";
        facility[35].spritePath = "Enemy/face_054_Dummy";
        facility[36].spritePath = "Facility/face_DifficultyBook";
        facility[37].spritePath = "Facility/face_TutorialBook";
        */
    }

    void LoadEnemyTable() {
        string filePath = "Text/enemy";
        TextAsset csv = Resources.Load<TextAsset>(filePath);
        StringReader reader = new StringReader(csv.text);
        while (reader.Peek() > -1) {
            string[] values = reader.ReadLine().Split('\t');
            int id = int.Parse(values[0]);
            if (id >= 0 && id < GameManager.enemyMax) {
                int level = int.Parse(values[1]);
                if (level >= 0 && level <= enemy[id].maxLevel) {
                    enemy[id].statusExist[level] = true;
                    enemy[id].status[level] = new EnemyStatus(int.Parse(values[2]), int.Parse(values[3]), int.Parse(values[4]), int.Parse(values[5]), int.Parse(values[6]), int.Parse(values[7]), int.Parse(values[8]));
                }
            }
        }
    }

    void LoadStatusTable() {
        string filePath = "Text/status";
        TextAsset csv = Resources.Load<TextAsset>(filePath);
        StringReader reader = new StringReader(csv.text);
        int index = 0;
        int max = variableStatus.Length;
        while (reader.Peek() > -1 && index < max) {
            string[] values = reader.ReadLine().Split('\t');
            variableStatus[index] = new VariableStatus(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3]));
            index++;
        }
    }

    /*
    void LoadFriendsTable() {
        string filePath = "Text/friends";
        TextAsset csv = Resources.Load<TextAsset>(filePath);
        StringReader reader = new StringReader(csv.text);
        while (reader.Peek() > -1) {
            string[] values = reader.ReadLine().Split('\t');
            int id = int.Parse(values[0]);
            if (id >= 0 && id < GameManager.friendsMax) {
                friends[id].status = new FriendsStatus(int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3]), int.Parse(values[4]), int.Parse(values[5]));
            }
        }
    }
    */

    void SetFriends(int id, string objName, int cost) {
        friends[id].path = objName;
        friends[id].cost = cost;
    }

    void SetEnemy(int id, string objName, int maxLevel = 4) {
        enemy[id].path = objName;
        enemy[id].maxLevel = maxLevel;
    }

    void SetUICache() {
        ui.balloonSerif = Resources.Load("Prefab/UI/BalloonSerif", typeof(GameObject)) as GameObject;
        ui.damagePlayer = Resources.Load("Prefab/UI/DamagePlayer", typeof(GameObject)) as GameObject;
        ui.damageHeal = Resources.Load("Prefab/UI/DamageHeal", typeof(GameObject)) as GameObject;
        ui.damageEnemy = Resources.Load("Prefab/UI/DamageEnemy", typeof(GameObject)) as GameObject;
        ui.damageCritical = Resources.Load("Prefab/UI/DamageCritical", typeof(GameObject)) as GameObject;
        ui.damageBack = Resources.Load("Prefab/UI/DamageBack", typeof(GameObject)) as GameObject;
        ui.damageEffective = Resources.Load("Prefab/UI/DamageEffective", typeof(GameObject)) as GameObject;
        ui.damageHyper = Resources.Load("Prefab/UI/DamageHyper", typeof(GameObject)) as GameObject;
        ui.damageHard = Resources.Load("Prefab/UI/DamageHard", typeof(GameObject)) as GameObject;
        ui.damageHardBack = Resources.Load("Prefab/UI/DamageHardBack", typeof(GameObject)) as GameObject;
        ui.enemyCanvas = Resources.Load("Prefab/UI/EnemyCanvas", typeof(GameObject)) as GameObject;
        ui.justDodgeHigh = Resources.Load("Prefab/UI/JustDodgeHigh", typeof(GameObject)) as GameObject;
        ui.justDodgeMiddle = Resources.Load("Prefab/UI/JustDodgeMiddle", typeof(GameObject)) as GameObject;
        ui.justDodgeLow = Resources.Load("Prefab/UI/JustDodgeLow", typeof(GameObject)) as GameObject;
        ui.getExp = Resources.Load("Prefab/UI/GetExp", typeof(GameObject)) as GameObject;
        ui.gutsNormal = Resources.Load("Prefab/UI/GutsNormal", typeof(GameObject)) as GameObject;
        ui.gutsLast = Resources.Load("Prefab/UI/GutsLast", typeof(GameObject)) as GameObject;
        ui.defeatRemain = Resources.Load("Prefab/UI/DefeatRemain", typeof(GameObject)) as GameObject;
        ui.fieldBuffHP = Resources.Load("Prefab/UI/FieldBuffHP", typeof(GameObject)) as GameObject;
        ui.fieldBuffST = Resources.Load("Prefab/UI/FieldBuffST", typeof(GameObject)) as GameObject;
        ui.fieldBuffAttack = Resources.Load("Prefab/UI/FieldBuffAttack", typeof(GameObject)) as GameObject;
    }

    public GameObject GetDamagePrefab(int type) {
        switch (type) {
            case 0:
                return ui.damagePlayer;
            case 1:
                return ui.damageHeal;
            case 2:
                return ui.damageEnemy;
            case 3:
                return ui.damageCritical;
            case 4:
                return ui.damageBack;
            case 5:
                return ui.damageEffective;
            case 6:
                return ui.damageHyper;
            case 7:
                return ui.damageHard;
            case 8:
                return ui.damageHardBack;
        }
        return null;
    }

    public GameObject GetFriends(int id) {
        if (friends[id].cache == null) {
            friends[id].cache = Resources.Load(friends[id].path, typeof(GameObject)) as GameObject;
        }
        return friends[id].cache;
    }

    public GameObject GetEnemy(int id) {
        if (enemy[id].cache == null) {
            enemy[id].cache = Resources.Load(enemy[id].path, typeof(GameObject)) as GameObject;
        }
        return enemy[id].cache;
    }

    public GameObject GetNPC(int id) {
        if (npc[id].cache == null) {
            npc[id].cache = Resources.Load(npc[id].path, typeof(GameObject)) as GameObject;
        }
        return npc[id].cache;
    }

    /*
    public GameObject GetFacility(int id) {
        if (facility[id].cache == null) {
            facility[id].cache = Resources.Load(facility[id].path, typeof(GameObject)) as GameObject;
        }
        return facility[id].cache;
    }

    public Sprite GetFacilitySprite(int id) {
        if (string.IsNullOrEmpty(facility[id].spritePath)) {
            return null;
        } else if (facility[id].spriteCache == null) {
            facility[id].spriteCache = Resources.Load(facility[id].spritePath, typeof(Sprite)) as Sprite;
        }
        return facility[id].spriteCache;
    }
    */
    
    public Sprite GetEnemySprite(int id) {
        if (string.IsNullOrEmpty(enemy[id].spritePath)) {
            return null;
        } else if (enemy[id].spriteCache == null) {
            enemy[id].spriteCache = Resources.Load(enemy[id].spritePath, typeof(Sprite)) as Sprite;
        }
        return enemy[id].spriteCache;
    }

    public bool CheckFacilityEnabled(int id) {
        if (id >= 0) {
            int referBit = ItemDatabase.Instance.GetItemPrice(ItemDatabase.facilityBottom + id);
            if (referBit < 0 || GameManager.Instance.save.facilityDisabled == 0) {
                return true;
            } else {
                return (GameManager.Instance.save.facilityDisabled & (1 << referBit)) == 0;
            }
        }
        return false;
    }

    public void SwitchFacilityEnabled(int id) {
        if (id >= 0) {
            int referBit = ItemDatabase.Instance.GetItemPrice(ItemDatabase.facilityBottom + id);
            if (referBit >= 0) {
                GameManager.Instance.save.facilityDisabled ^= (1 << referBit);
            }
        }
    }

    public RuntimeAnimatorController GetAnimCon(int id) {
        if (animCon[id].cache == null) {
            animCon[id].cache = Resources.Load(animCon[id].path, typeof(RuntimeAnimatorController)) as RuntimeAnimatorController;
        }
        return animCon[id].cache;
    }

    /*
    public void UnloadEnemyCache() {
        for (int i = 0; i < enemy.Length; i++) {
            if (enemy[i].cache != null) {
                Destroy(enemy[i].cache);
                enemy[i].cache = null;
            }
        }
        Resources.UnloadUnusedAssets();
    }
    */
}
