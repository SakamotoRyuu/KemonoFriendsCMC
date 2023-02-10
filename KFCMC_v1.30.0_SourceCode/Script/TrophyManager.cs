using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrophyManager : SingletonMonoBehaviour<TrophyManager> {

    public const int t_CompleteTrophy = 0;
    public const int t_ClearNT = 1;
    public const int t_ClearVU = 2;
    public const int t_ClearEN = 3;
    public const int t_ClearCR = 4;
    public const int t_StageClear = 5;
    public const int t_SkytreeClear = 17;
    public const int t_SkytreeClearCR = 18;
    public const int t_DefeatMany1 = 19;
    public const int t_DefeatMany2 = 20;
    public const int t_DefeatMany3 = 21;
    public const int t_DefeatMany4 = 22;
    public const int t_DefeatAllType = 23;
    public const int t_CompleteEnemyBook = 24;
    public const int t_RescueAllFriends = 25;
    public const int t_RescueAllLB = 26;
    public const int t_BuyAllWeapons = 27;
    public const int t_GetAllHp = 28;
    public const int t_GetAllSt = 29;
    public const int t_GetAllDocuments = 30;
    public const int t_LevelMax = 31;
    public const int t_JustDodgeContinuous = 32;
    public const int t_JustDodgeCounter = 33;
    public const int t_Judgement = 34;
    public const int t_Bomb = 35;
    public const int t_BlackCrystal = 36;
    public const int t_RedCrystal = 37;
    public const int t_CrimsonCrystal = 38;
    public const int t_Chimera = 39;
    public const int t_ParkmanClear = 40;
    public const int t_ParkmanHighScore = 41;
    public const int t_TrainingClearLimited = 42;
    public const int t_TrainingClearRedMinmi = 47;
    public const int t_GraphCollapse9 = 48;
    public const int t_GraphCollapse12 = 49;
    public const int t_HugeDamage = 50;
    public const int t_BlackMinmiBoss = 51;
    public const int t_DefeatWR = 52;
    public const int t_DefeatSinWR = 53;
    public const int t_DefeatAllSinWR = 54;
    public const int t_SecretCoinWithTsuchinoko = 55;
    public const int t_PPPMonumentWithMargay = 56;
    public const int t_Fukkura = 57;
    public const int t_PressMachine = 58;
    public const int t_FossilWithKaban = 59;
    public const int t_AnotherServalCrystal = 60;
    public const int t_DefeatDummy = 61;
    public const int t_MegatonCoin = 62;
    public const int t_SecretDocument = 63;
    public const int t_MinmiStatue = 64;
    public const int t_GoldenMinmiED = 65;
    public const int t_Sushizanmai = 66;
    public const int t_WingBolt = 67;
    public const int t_AlligatorClipsEscape = 68;
    public const int t_AlligatorClipsKaban = 69;
    public const int t_AlligatorClipsHippo = 70;
    public const int t_Otter = 71;
    public const int t_CamponotusJaguar = 72;
    public const int t_DarkServalPile = 73;
    public const int t_Ibis = 74;
    public const int t_B2Needle = 75;
    public const int t_Alpaca = 76;
    public const int t_SandCat = 77;
    public const int t_PlugAdapterTsuchinoko = 78;
    public const int t_RedGrowlCounter = 79;
    public const int t_AureliaJumpDodge = 80;
    public const int t_Beaver = 81;
    public const int t_PrairieDog = 82;
    public const int t_AkulaKnock = 83;
    public const int t_Moose = 84;
    public const int t_Lion = 85;
    public const int t_DarkServalWave = 86;
    public const int t_Owls = 87;
    public const int t_BirdlienCounter = 88;
    public const int t_CeckyBeast = 89;
    public const int t_EnypniastesMargay = 90;
    public const int t_ClionePPP = 91;
    public const int t_Bathynomus = 92;
    public const int t_Glaucus = 93;
    public const int t_SmartBallRedFox = 94;
    public const int t_SilverFox = 95;
    public const int t_SnowTower = 96;
    public const int t_CampoFlicker = 97;
    public const int t_GrayWolf = 98;
    public const int t_Giraffe = 99;
    public const int t_Anomalocaris = 100;
    public const int t_Queen = 101;
    public const int t_BrownBear = 102;
    public const int t_PaintedWolf = 103;
    public const int t_DarkServalInferno = 104;
    public const int t_GoldenMonkey = 105;
    public const int t_Raccoon = 106;
    public const int t_Fennec = 107;
    public const int t_BigDogInside = 108;
    public const int t_BigDogAllFriends = 109;
    public const int t_AnotherAndDark = 110;
    public const int t_Australopithecus = 111;
    public const int t_DefeatDS1 = 112;
    public const int t_DefeatDS2 = 113;
    public const int t_DefeatBigDogEX = 114;
    public const int t_DefeatQueenRaw = 115;
    public const int t_DefeatHomoSapiens = 116;
    public const int t_DarkServalPurple = 117;
    public const int t_HeliozoaSilver = 118;
    public const int t_ImperatrixMundiSpeedrun = 119;


    [System.NonSerialized]
    public int[] trophyRanks = new int[GameManager.trophyMax];
    [System.NonSerialized]
    public bool[] trophyIsNew = new bool[GameManager.trophyMax];

    private int[] conditionFriends = new int[GameManager.trophyMax];
    private int[] conditionProgress = new int[GameManager.trophyMax];
    private int[] conditionMinmi = new int[GameManager.trophyMax];
    private int[] conditionTrophy = new int[GameManager.trophyMax];

    private int checkNum;
    private int defeatCount = -1;

    protected override void Awake() {
        base.Awake();
        trophyRanks[t_CompleteTrophy] = 3;
        trophyRanks[t_ClearEN] = 1;
        trophyRanks[t_ClearCR] = 2;
        trophyRanks[t_SkytreeClear] = 1;
        trophyRanks[t_SkytreeClearCR] = 2;
        trophyRanks[t_DefeatMany3] = 1;
        trophyRanks[t_DefeatMany4] = 2;
        trophyRanks[t_DefeatAllType] = 1;
        trophyRanks[t_CompleteEnemyBook] = 2;
        trophyRanks[t_RescueAllFriends] = 1;
        trophyRanks[t_RescueAllLB] = 1;
        trophyRanks[t_BuyAllWeapons] = 1;
        trophyRanks[t_GetAllHp] = 1;
        trophyRanks[t_GetAllSt] = 1;
        trophyRanks[t_GetAllDocuments] = 2;
        trophyRanks[t_ParkmanHighScore] = 1;
        trophyRanks[t_TrainingClearLimited + 4] = 1;
        trophyRanks[t_TrainingClearRedMinmi] = 1;
        trophyRanks[t_DefeatBigDogEX] = 1;
        trophyRanks[t_DefeatQueenRaw] = 1;
        trophyRanks[t_DefeatHomoSapiens] = 1;
        trophyRanks[t_DefeatSinWR] = 1;
        trophyRanks[t_DefeatAllSinWR] = 2;
        trophyRanks[t_MegatonCoin] = 1;
        trophyRanks[t_SecretDocument] = 2;
        trophyRanks[t_MinmiStatue] = 1;
        trophyRanks[t_GoldenMinmiED] = 1;
        trophyRanks[t_Sushizanmai] = 2;
        trophyRanks[t_DarkServalPurple] = 2;
        trophyRanks[t_HeliozoaSilver] = 2;
        trophyRanks[t_ImperatrixMundiSpeedrun] = 2;
        trophyRanks[t_GraphCollapse9] = 1;
        trophyRanks[t_GraphCollapse12] = 1;
        trophyRanks[t_HugeDamage] = 1;

        //1 - 22
        //2 - 11
        
        conditionFriends[t_AnotherServalCrystal] = 31;
        conditionFriends[t_AnotherAndDark] = 31;

        for (int i = 0; i < 12; i++) {
            conditionProgress[t_StageClear + i] = i;
        }
        conditionProgress[t_SkytreeClear] = 13;
        conditionProgress[t_SkytreeClearCR] = 13;
        conditionProgress[t_DefeatWR] = 13;
        conditionProgress[t_DefeatSinWR] = 13;
        conditionProgress[t_DefeatAllSinWR] = 13;
        conditionProgress[t_MinmiStatue] = 13;
        conditionProgress[t_GoldenMinmiED] = 13;
        conditionProgress[t_Sushizanmai] = 99;
        conditionProgress[t_Australopithecus] = 13;
        conditionProgress[t_DefeatDS1] = 13;
        conditionProgress[t_DefeatDS2] = 13;
        conditionProgress[t_DefeatBigDogEX] = 13;
        conditionProgress[t_DefeatQueenRaw] = 13;
        conditionProgress[t_DefeatHomoSapiens] = 13;
        conditionProgress[t_DarkServalPurple] = 13;
        conditionProgress[t_HeliozoaSilver] = 13;
        conditionProgress[t_ImperatrixMundiSpeedrun] = 13;

        conditionMinmi[t_TrainingClearRedMinmi] = 2;
        conditionMinmi[t_DarkServalPurple] = 3;
        conditionMinmi[t_BlackMinmiBoss] = 4;
        conditionMinmi[t_HeliozoaSilver] = 5;
        conditionMinmi[t_GoldenMinmiED] = 6;

        conditionTrophy[t_DefeatMany2] = t_DefeatMany1;
        conditionTrophy[t_DefeatMany3] = t_DefeatMany2;
        conditionTrophy[t_DefeatMany4] = t_DefeatMany3;
        conditionTrophy[t_DefeatAllSinWR] = t_DefeatSinWR;

    }

    public void FixTrophyArray() {
        if (GameManager.Instance.save.trophy.Length < GameManager.trophyArrayMax) {
            int[] newArray = new int[GameManager.trophyArrayMax];
            for (int i = 0; i < GameManager.Instance.save.trophy.Length; i++) {
                newArray[i] = GameManager.Instance.save.trophy[i];
            }
            GameManager.Instance.save.trophy = newArray;
        }
    }

    public bool IsTrophyHad(int index) {
        if (!GameManager.Instance.IsPlayerAnother) {
            int arrayNum = index / 32;
            int bitNum = 1 << (index % 32);
            return (GameManager.Instance.save.trophy[arrayNum] & bitNum) != 0;
        }
        return true;
    }

    public int GetTrophyCount() {
        int count = 0;
        for (int index = 0; index < GameManager.trophyMax; index++) {
            int arrayNum = index / 32;
            int bitNum = 1 << (index % 32);
            if ((GameManager.Instance.save.trophy[arrayNum] & bitNum) != 0) {
                count++;
            }
        }
        return count;
    }

    private void CheckTrophy_Complete() {
        if (GameManager.Instance.save.config[GameManager.Save.configID_DisableTrophy] == 0) {
            bool answer = true;
            for (int index = t_CompleteTrophy + 1; index < GameManager.trophyMax; index++) {
                int arrayNum = index / 32;
                int bitNum = 1 << (index % 32);
                if ((GameManager.Instance.save.trophy[arrayNum] & bitNum) == 0) {
                    answer = false;
                    break;
                }
            }
            if (answer) {
                GetTrophy(t_CompleteTrophy, t_CompleteTrophy / 32, 1 << (t_CompleteTrophy % 32));
                CheckSteamAchievement();
            }
        }
    }

    public void CheckSteamAchievement() {
        if (GameManager.Instance && GameManager.Instance.save.config[GameManager.Save.configID_DisableTrophy] == 0) {
            int arrayNum = t_CompleteTrophy / 32;
            int bitNum = 1 << (t_CompleteTrophy % 32);
            if ((GameManager.Instance.save.trophy[arrayNum] & bitNum) != 0) {
                GameManager.Instance.SetSteamAchievement("ALLTROPHIES");
            }
        }
    }

    private void GetTrophy(int index, int arrayNum, int bitNum, bool showMessage = true) {
        if (!GameManager.Instance.IsPlayerAnother && GameManager.Instance.save.config[GameManager.Save.configID_DisableTrophy] == 0) {
            GameManager.Instance.save.trophy[arrayNum] |= bitNum;
            if (showMessage) {
                if (GameManager.Instance.save.config[GameManager.Save.configID_TrophyNotification] != 0) {
                    if (MessageUI.Instance) {
                        MessageUI.Instance.SetMessageOptional(string.Format("{0}{1}{2}", TextManager.Get("QUOTE_START"), TextManager.Get("TROPHY_NAME_" + (index + 1).ToString("000")), TextManager.Get("QUOTE_END")), Color.black, Color.black, -1, ItemDatabase.trophyID[trophyRanks[index]], 5f, 0, MessageUI.slotType_Trophy);
                    }
                    if (GameManager.Instance.save.config[GameManager.Save.configID_TrophyNotification] == 1 && UISE.Instance) {
                        UISE.Instance.Play(UISE.SoundName.trophy);
                    }
                }
            }
            trophyIsNew[index] = true;
        }
    }

    public void CheckTrophy(int index, bool passCheck = false, bool showMessage = true) {
        if (!GameManager.Instance.IsPlayerAnother && index >= 0 && index < GameManager.trophyMax && GameManager.Instance.save.config[GameManager.Save.configID_DisableTrophy] == 0) {
            int arrayNum = index / 32;
            int bitNum = 1 << (index % 32);
            if (arrayNum < GameManager.Instance.save.trophy.Length && (GameManager.Instance.save.trophy[arrayNum] & bitNum) == 0) {
                bool answer = false;
                if (passCheck) {
                    answer = true;
                } else {
                    switch (index) {
                        case t_ClearNT:
                            answer = GameManager.Instance.save.progress >= GameManager.gameClearedProgress;
                            break;
                        case t_ClearVU:
                            answer = GameManager.Instance.GetGameClearDifficulty() >= GameManager.difficultyVU;
                            break;
                        case t_ClearEN:
                            answer = GameManager.Instance.GetGameClearDifficulty() >= GameManager.difficultyEN;
                            break;
                        case t_ClearCR:
                            answer = GameManager.Instance.GetGameClearDifficulty() >= GameManager.difficultyCR;
                            break;
                        case t_DefeatMany1:
                            answer = defeatCount >= 100;
                            break;
                        case t_DefeatMany2:
                            answer = defeatCount >= 500;
                            break;
                        case t_DefeatMany3:
                            answer = defeatCount >= 2000;
                            break;
                        case t_DefeatMany4:
                            answer = defeatCount >= 5000;
                            break;
                        case t_DefeatAllType:
                            if (GameManager.enemyMax * GameManager.enemyLevelMax <= GameManager.Instance.save.defeatEnemy.Length) {
                                answer = true;
                                for (int i = 0; i < GameManager.enemyMax; i++) {
                                    bool defeatAnyLevel = false;
                                    for (int j = 0; j < GameManager.enemyLevelMax; j++) {
                                        if (GameManager.Instance.save.defeatEnemy[i * GameManager.enemyLevelMax + j] > 0) {
                                            defeatAnyLevel = true;
                                            break;
                                        }
                                    }
                                    if (defeatAnyLevel == false) {
                                        answer = false;
                                        break;
                                    }
                                }
                            }
                            break;
                        case t_CompleteEnemyBook:
                            if (GameManager.enemyMax * GameManager.enemyLevelMax <= GameManager.Instance.save.defeatEnemy.Length) {
                                answer = true;
                                for (int i = 0; i < GameManager.enemyMax && answer; i++) {
                                    for (int j = 0; j < GameManager.enemyLevelMax; j++) {
                                        if (CharacterDatabase.Instance.enemy[i].appearInBook[j] && GameManager.Instance.save.defeatEnemy[i * GameManager.enemyLevelMax + j] <= 0) {
                                            answer = false;
                                            break;
                                        }
                                    }
                                }
                            }
                            break;
                        case t_RescueAllLB:
                            answer = ShopDatabase.Instance && ShopDatabase.Instance.GetShopLevel() >= 4 && GameManager.Instance.GetSecret(GameManager.SecretType.SingularityLB);
                            break;
                        case t_RescueAllFriends:
                            answer = GameManager.Instance.save.GotFriends >= 31 && GameManager.Instance.save.GotInvUpOriginal >= 16;
                            break;
                        case t_BuyAllWeapons:
                            answer = true;
                            for (int i = 3; i <= 20; i++) {
                                if (GameManager.Instance.save.weapon[i] == 0) {
                                    answer = false;
                                    break;
                                }
                            }
                            break;
                        case t_GetAllHp:
                            answer = GameManager.Instance.save.hpUpSale >= GameManager.hpUpSaleMax && GameManager.Instance.save.GotHpUpNFS >= GameManager.hpUpNFSMax;
                            break;
                        case t_GetAllSt:
                            answer = GameManager.Instance.save.stUpSale >= GameManager.stUpSaleMax && GameManager.Instance.save.GotStUpNFS >= GameManager.stUpNFSMax;
                            break;
                        case t_GetAllDocuments:
                            answer = GameManager.Instance.GetDocumentCompleted();
                            break;
                        case t_LevelMax:
                            answer = GameManager.Instance.save.Level >= GameManager.levelMax;
                            break;
                        case t_SecretDocument:
                            answer = GameManager.Instance.GetSecret(GameManager.SecretType.UndergroundLB) && GameManager.Instance.GetSecret(GameManager.SecretType.SafePack);
                            break;
                        case t_MinmiStatue:
                            answer = GameManager.Instance.GetMinmiCompleted();
                            break;
                        default:
                            answer = true;
                            break;
                    }
                }
                if (answer) {
                    GetTrophy(index, arrayNum, bitNum, showMessage);
                }
            }
            if (!IsTrophyHad(t_CompleteTrophy)) {
                CheckTrophy_Complete();
            }
        }
    }

    public void CheckTrophy_Clear(bool showMessage = true) {
        FixTrophyArray();
        if (!GameManager.Instance.IsPlayerAnother && GameManager.Instance.save.config[GameManager.Save.configID_DisableTrophy] == 0) {
            int minDifficulty = GameManager.Instance.GetGameClearDifficulty();
            int progress = GameManager.Instance.save.progress;
            bool skytreeCleared = GameManager.Instance.GetSecret(GameManager.SecretType.SkytreeCleared);
            for (int index = t_ClearNT; index <= t_SkytreeClearCR; index++) {
                int arrayNum = index / 32;
                int bitNum = (1 << (index % 32));
                if (arrayNum < GameManager.Instance.save.trophy.Length && (GameManager.Instance.save.trophy[arrayNum] & bitNum) == 0) {
                    bool answer = false;
                    switch (index) {
                        case t_ClearNT:
                            answer = progress >= GameManager.gameClearedProgress;
                            break;
                        case t_ClearVU:
                            answer = minDifficulty >= GameManager.difficultyVU;
                            break;
                        case t_ClearEN:
                            answer = minDifficulty >= GameManager.difficultyEN;
                            break;
                        case t_ClearCR:
                            answer = minDifficulty >= GameManager.difficultyCR;
                            break;
                        case t_SkytreeClear:
                            answer = skytreeCleared;
                            break;
                        case t_SkytreeClearCR:
                            answer = skytreeCleared && GameManager.Instance.save.clearDifficulty[StageManager.specialStageId] >= GameManager.difficultyCR;
                            break;
                        case t_MinmiStatue:
                            answer = GameManager.Instance.GetMinmiCompleted();
                            break;
                        default:
                            answer = progress > index - t_StageClear;
                            break;
                    }
                    if (answer) {
                        GetTrophy(index, arrayNum, bitNum, showMessage);
                    }
                }
            }
        }
    }

    public void CheckTrophy_DefeatMany() {
        if (defeatCount < 0) {
            defeatCount = GameManager.Instance.GetDefeatSum();
        } else {
            defeatCount++;
        }
        CheckTrophy(t_DefeatMany1);
        CheckTrophy(t_DefeatMany2);
        CheckTrophy(t_DefeatMany3);
        CheckTrophy(t_DefeatMany4);
    }

    public void CheckTrophy_DefeatGraphCollapse() {
        switch (StageManager.Instance.stageNumber) {
            case 9:
                CheckTrophy(t_GraphCollapse9, true);
                break;
            case 12:
                CheckTrophy(t_GraphCollapse12, true);
                break;
        }
    }

    public void CheckTrophy_SpecialChat(int friendsIndex) {
        switch (friendsIndex) {
            case 8:
                CheckTrophy(t_SecretCoinWithTsuchinoko, true);
                break;
            case 15:
                CheckTrophy(t_PPPMonumentWithMargay, true);
                break;
            case 1:
                CheckTrophy(t_FossilWithKaban, true);
                break;
            case 31:
                CheckTrophy(t_AnotherServalCrystal, true);
                break;
        }
    }

    public bool GetTrophyInfoCondition(int index) {
        if (index >= 0 && index < GameManager.trophyMax) {
            return (conditionProgress[index] <= 0 || GameManager.Instance.save.progress >= conditionProgress[index]) && (conditionFriends[index] <= 0 || GameManager.Instance.save.friends[conditionFriends[index]] != 0) && (conditionMinmi[index] <= 0 || (GameManager.Instance.save.minmi & (1 << (conditionMinmi[index] - 1))) != 0) && (conditionTrophy[index] <= 0 || IsTrophyHad(conditionTrophy[index]));
        }
        return false;
    }

}
