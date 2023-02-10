using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemDatabase : SingletonMonoBehaviour<ItemDatabase> {

    public const int goldMax = 4;
    public const int containerMax = 5;
    public List<ItemData> itemList = new List<ItemData>();

    public const int replaceRows = 3;
    public const int replaceColumns = 4;
    public static readonly int[] replaceBeforeID = new int[] { 43, 86, 87, 88, 89 };
    public static readonly int[,] replaceAfterID = new int[replaceRows, replaceColumns] {
        { 53, 53, 54, 50 },
        { 53, 53, 54, 51 },
        { 53, 53, 54, 52 }
        };
    public static readonly int[] trophyID = new int[] { 370, 371, 372, 373 };

    private const int typeMax = 5;
    private int[] typeCount = new int[typeMax];
    private static readonly int[] goldUnits = new int[goldMax] { 1, 5, 20, 100 };
    private static readonly int[] goldId = new int[goldMax] { 300, 301, 302, 303 };
    private static readonly int[] containerId = new int[containerMax] { 310, 311, 312, 313, 314 };
    private static readonly Dictionary<string, GameObject> mList = new Dictionary<string, GameObject>();
    public const int friendsIndexBottom = 100;
    public const int facilityBottom = 500;

    protected override void Awake() {
        if (CheckInstance()) {
            DontDestroyOnLoad(gameObject);

            itemList.Add(new ItemData(0, 8, Resources.Load("Item/heal_00", typeof(Sprite)) as Sprite, "Prefab/Item/000_Heal0"));
            itemList.Add(new ItemData(1, 20, Resources.Load("Item/heal_01", typeof(Sprite)) as Sprite, "Prefab/Item/001_Heal1"));
            itemList.Add(new ItemData(2, 50, Resources.Load("Item/heal_02", typeof(Sprite)) as Sprite, "Prefab/Item/002_Heal2"));
            itemList.Add(new ItemData(9, 30, Resources.Load("Item/heal_09", typeof(Sprite)) as Sprite, "Prefab/Item/009_Sandstar"));

            itemList.Add(new ItemData(20, 10, Resources.Load("Item/buff_00", typeof(Sprite)) as Sprite, "Prefab/Item/020_Antidote"));
            itemList.Add(new ItemData(21, 30, Resources.Load("Item/buff_01", typeof(Sprite)) as Sprite, "Prefab/Item/021_BuffStamina"));
            itemList.Add(new ItemData(22, 30, Resources.Load("Item/buff_02", typeof(Sprite)) as Sprite, "Prefab/Item/022_BuffSpeed"));
            itemList.Add(new ItemData(23, 45, Resources.Load("Item/buff_03", typeof(Sprite)) as Sprite, "Prefab/Item/023_BuffAttack"));
            itemList.Add(new ItemData(24, 45, Resources.Load("Item/buff_04", typeof(Sprite)) as Sprite, "Prefab/Item/024_BuffDefense"));
            itemList.Add(new ItemData(25, 60, Resources.Load("Item/buff_05", typeof(Sprite)) as Sprite, "Prefab/Item/025_BuffKnock"));
            itemList.Add(new ItemData(26, 60, Resources.Load("Item/buff_06", typeof(Sprite)) as Sprite, "Prefab/Item/026_BuffAbsorb"));
            itemList.Add(new ItemData(27, 80, Resources.Load("Item/buff_07", typeof(Sprite)) as Sprite, "Prefab/Item/027_BuffStealth"));
            itemList.Add(new ItemData(28, 100, Resources.Load("Item/buff_08", typeof(Sprite)) as Sprite, "Prefab/Item/028_BuffLong"));
            itemList.Add(new ItemData(29, 150, Resources.Load("Item/buff_09", typeof(Sprite)) as Sprite, "Prefab/Item/029_BuffMulti"));

            itemList.Add(new ItemData(40, 10, Resources.Load("Item/graph_00", typeof(Sprite)) as Sprite, "Prefab/Item/040_GraphWarp"));
            itemList.Add(new ItemData(41, 30, Resources.Load("Item/graph_01", typeof(Sprite)) as Sprite, "Prefab/Item/041_GraphGuide"));
            itemList.Add(new ItemData(42, 30, Resources.Load("Item/graph_02", typeof(Sprite)) as Sprite, "Prefab/Item/042_GraphClearTraps"));
            itemList.Add(new ItemData(43, 50, Resources.Load("Item/graph_03", typeof(Sprite)) as Sprite, "Prefab/Item/043_GraphGold"));
            itemList.Add(new ItemData(44, 50, Resources.Load("Item/graph_04", typeof(Sprite)) as Sprite, "Prefab/Item/044_GraphBreak"));
            itemList.Add(new ItemData(45, 60, Resources.Load("Item/graph_05", typeof(Sprite)) as Sprite, "Prefab/Item/045_GraphBuild"));
            itemList.Add(new ItemData(46, 20, Resources.Load("Item/graph_06", typeof(Sprite)) as Sprite, "Prefab/Item/046_GraphConnect"));

            itemList.Add(new ItemData(50, 50, Resources.Load("Item/crystal_00", typeof(Sprite)) as Sprite, "Prefab/Item/050_CrystalGreen_S"));
            itemList.Add(new ItemData(51, 100, Resources.Load("Item/crystal_01", typeof(Sprite)) as Sprite, "Prefab/Item/051_CrystalGreen_M"));
            itemList.Add(new ItemData(52, 200, Resources.Load("Item/crystal_02", typeof(Sprite)) as Sprite, "Prefab/Item/052_CrystalGreen_L"));
            itemList.Add(new ItemData(53, 40, Resources.Load("Item/crystal_03", typeof(Sprite)) as Sprite, "Prefab/Item/053_CrystalBlack"));
            itemList.Add(new ItemData(54, 40, Resources.Load("Item/crystal_04", typeof(Sprite)) as Sprite, "Prefab/Item/054_CrystalRed"));
            itemList.Add(new ItemData(55, 80, Resources.Load("Item/crystal_05", typeof(Sprite)) as Sprite, "Prefab/Item/055_CrystalBlue"));
            itemList.Add(new ItemData(56, 80, Resources.Load("Item/crystal_06", typeof(Sprite)) as Sprite, "Prefab/Item/056_CrystalViolet"));
            itemList.Add(new ItemData(57, 200, Resources.Load("Item/crystal_07", typeof(Sprite)) as Sprite, "Prefab/Item/057_CrystalYellow"));
            itemList.Add(new ItemData(58, 200, Resources.Load("Item/crystal_08", typeof(Sprite)) as Sprite, "Prefab/Item/058_CrystalCrimson"));
            itemList.Add(new ItemData(59, 400, Resources.Load("Item/crystal_09", typeof(Sprite)) as Sprite, "Prefab/Item/059_CrystalWhite"));

            itemList.Add(new ItemData(60, 60, Resources.Load("Item/bomb_00", typeof(Sprite)) as Sprite, "Prefab/Item/060_BombNormal"));
            itemList.Add(new ItemData(61, 60, Resources.Load("Item/bomb_01", typeof(Sprite)) as Sprite, "Prefab/Item/061_BombPoison"));
            itemList.Add(new ItemData(62, 60, Resources.Load("Item/bomb_02", typeof(Sprite)) as Sprite, "Prefab/Item/062_BombAcid"));
            itemList.Add(new ItemData(63, 120, Resources.Load("Item/bomb_03", typeof(Sprite)) as Sprite, "Prefab/Item/063_BombNormal_L"));
            itemList.Add(new ItemData(64, 120, Resources.Load("Item/bomb_04", typeof(Sprite)) as Sprite, "Prefab/Item/064_BombPoison_L"));
            itemList.Add(new ItemData(65, 120, Resources.Load("Item/bomb_05", typeof(Sprite)) as Sprite, "Prefab/Item/065_BombAcid_L"));

            itemList.Add(new ItemData(70, 15, Resources.Load("Item/japariman", typeof(Sprite)) as Sprite, "Prefab/Item/070_Japariman"));
            itemList.Add(new ItemData(71, 45, Resources.Load("Item/japariman_3", typeof(Sprite)) as Sprite, "Prefab/Item/071_Japariman_3"));
            itemList.Add(new ItemData(72, 75, Resources.Load("Item/japariman_5", typeof(Sprite)) as Sprite, "Prefab/Item/072_Japariman_5"));

            itemList.Add(new ItemData(80, 666, Resources.Load("Item/sandstarRaw", typeof(Sprite)) as Sprite, "Prefab/Item/080_SandstarRaw"));
            itemList.Add(new ItemData(81, 0, Resources.Load("Item/celliumCure", typeof(Sprite)) as Sprite, "Prefab/Item/081_CelliumCure"));
            itemList.Add(new ItemData(82, 0, Resources.Load("Item/goldNugget", typeof(Sprite)) as Sprite, "Prefab/Item/082_GoldNugget"));
            itemList.Add(new ItemData(83, 0, Resources.Load("Item/fdKaban", typeof(Sprite)) as Sprite, "Prefab/Item/083_FDKaban"));

            itemList.Add(new ItemData(86, 400, Resources.Load("Item/coin_00", typeof(Sprite)) as Sprite, "Prefab/Item/086_Coin_S"));
            itemList.Add(new ItemData(87, 700, Resources.Load("Item/coin_01", typeof(Sprite)) as Sprite, "Prefab/Item/087_Coin_M"));
            itemList.Add(new ItemData(88, 1200, Resources.Load("Item/coin_02", typeof(Sprite)) as Sprite, "Prefab/Item/088_Coin_L"));
            itemList.Add(new ItemData(89, 2000, Resources.Load("Item/coin_03", typeof(Sprite)) as Sprite, "Prefab/Item/089_Coin_XL"));
            itemList.Add(new ItemData(90, 20000, Resources.Load("Item/coin_04", typeof(Sprite)) as Sprite, "Prefab/Item/090_MegatonCoin"));

            itemList.Add(new ItemData(94, 0, Resources.Load("Item/statue_blue", typeof(Sprite)) as Sprite, "Prefab/Item/094_Statue_Blue"));
            itemList.Add(new ItemData(95, 0, Resources.Load("Item/statue_red", typeof(Sprite)) as Sprite, "Prefab/Item/095_Statue_Red"));
            itemList.Add(new ItemData(96, 0, Resources.Load("Item/statue_violet", typeof(Sprite)) as Sprite, "Prefab/Item/096_Statue_Violet"));
            itemList.Add(new ItemData(97, 0, Resources.Load("Item/statue_black", typeof(Sprite)) as Sprite, "Prefab/Item/097_Statue_Black"));
            itemList.Add(new ItemData(98, 0, Resources.Load("Item/statue_silver", typeof(Sprite)) as Sprite, "Prefab/Item/098_Statue_Silver"));
            itemList.Add(new ItemData(99, 0, Resources.Load("Item/statue_golden", typeof(Sprite)) as Sprite, "Prefab/Item/099_Statue_Golden"));


            itemList.Add(new ItemData(100, -1, Resources.Load("Friends/face_all", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(101, -1, Resources.Load("Friends/face_kaban", typeof(Sprite)) as Sprite, "Prefab/IFR/101_IFR_Kaban"));
            itemList.Add(new ItemData(102, -1, Resources.Load("Friends/face_hippo", typeof(Sprite)) as Sprite, "Prefab/IFR/102_IFR_Hippo"));
            itemList.Add(new ItemData(103, -1, Resources.Load("Friends/face_otter", typeof(Sprite)) as Sprite, "Prefab/IFR/103_IFR_Otter"));
            itemList.Add(new ItemData(104, -1, Resources.Load("Friends/face_jaguar", typeof(Sprite)) as Sprite, "Prefab/IFR/104_IFR_Jaguar"));
            itemList.Add(new ItemData(105, -1, Resources.Load("Friends/face_ibis", typeof(Sprite)) as Sprite, "Prefab/IFR/105_IFR_Ibis"));
            itemList.Add(new ItemData(106, -1, Resources.Load("Friends/face_alpaca", typeof(Sprite)) as Sprite, "Prefab/IFR/106_IFR_Alpaca"));
            itemList.Add(new ItemData(107, -1, Resources.Load("Friends/face_sandCat", typeof(Sprite)) as Sprite, "Prefab/IFR/107_IFR_SandCat"));
            itemList.Add(new ItemData(108, -1, Resources.Load("Friends/face_tsuchinoko", typeof(Sprite)) as Sprite, "Prefab/IFR/108_IFR_Tsuchinoko"));
            itemList.Add(new ItemData(109, -1, Resources.Load("Friends/face_beaver", typeof(Sprite)) as Sprite, "Prefab/IFR/109_IFR_Beaver"));
            itemList.Add(new ItemData(110, -1, Resources.Load("Friends/face_prairieDog", typeof(Sprite)) as Sprite, "Prefab/IFR/110_IFR_PrairieDog"));
            itemList.Add(new ItemData(111, -1, Resources.Load("Friends/face_moose", typeof(Sprite)) as Sprite, "Prefab/IFR/111_IFR_Moose"));
            itemList.Add(new ItemData(112, -1, Resources.Load("Friends/face_lion", typeof(Sprite)) as Sprite, "Prefab/IFR/112_IFR_Lion"));
            itemList.Add(new ItemData(113, -1, Resources.Load("Friends/face_whiteOwl", typeof(Sprite)) as Sprite, "Prefab/IFR/113_IFR_WhiteOwl"));
            itemList.Add(new ItemData(114, -1, Resources.Load("Friends/face_eagleOwl", typeof(Sprite)) as Sprite, "Prefab/IFR/114_IFR_EagleOwl"));
            itemList.Add(new ItemData(115, -1, Resources.Load("Friends/face_margay", typeof(Sprite)) as Sprite, "Prefab/IFR/115_IFR_Margay"));
            itemList.Add(new ItemData(116, -1, Resources.Load("Friends/face_princess", typeof(Sprite)) as Sprite, "Prefab/IFR/116_IFR_Princess"));
            itemList.Add(new ItemData(117, -1, Resources.Load("Friends/face_rocker", typeof(Sprite)) as Sprite, "Prefab/IFR/117_IFR_Rocker"));
            itemList.Add(new ItemData(118, -1, Resources.Load("Friends/face_gean", typeof(Sprite)) as Sprite, "Prefab/IFR/118_IFR_Gean"));
            itemList.Add(new ItemData(119, -1, Resources.Load("Friends/face_hululu", typeof(Sprite)) as Sprite, "Prefab/IFR/119_IFR_Hululu"));
            itemList.Add(new ItemData(120, -1, Resources.Load("Friends/face_emperor", typeof(Sprite)) as Sprite, "Prefab/IFR/120_IFR_Emperor"));
            itemList.Add(new ItemData(121, -1, Resources.Load("Friends/face_redFox", typeof(Sprite)) as Sprite, "Prefab/IFR/121_IFR_RedFox"));
            itemList.Add(new ItemData(122, -1, Resources.Load("Friends/face_silverFox", typeof(Sprite)) as Sprite, "Prefab/IFR/122_IFR_SilverFox"));
            itemList.Add(new ItemData(123, -1, Resources.Load("Friends/face_campoFlicker", typeof(Sprite)) as Sprite, "Prefab/IFR/123_IFR_CampoFlicker"));
            itemList.Add(new ItemData(124, -1, Resources.Load("Friends/face_grayWolf", typeof(Sprite)) as Sprite, "Prefab/IFR/124_IFR_GrayWolf"));
            itemList.Add(new ItemData(125, -1, Resources.Load("Friends/face_giraffe", typeof(Sprite)) as Sprite, "Prefab/IFR/125_IFR_Giraffe"));
            itemList.Add(new ItemData(126, -1, Resources.Load("Friends/face_goldenMonkey", typeof(Sprite)) as Sprite, "Prefab/IFR/126_IFR_GoldenMonkey"));
            itemList.Add(new ItemData(127, -1, Resources.Load("Friends/face_brownBear", typeof(Sprite)) as Sprite, "Prefab/IFR/127_IFR_BrownBear"));
            itemList.Add(new ItemData(128, -1, Resources.Load("Friends/face_paintedWolf", typeof(Sprite)) as Sprite, "Prefab/IFR/128_IFR_PaintedWolf"));
            itemList.Add(new ItemData(129, -1, Resources.Load("Friends/face_raccoon", typeof(Sprite)) as Sprite, "Prefab/IFR/129_IFR_Raccoon"));
            itemList.Add(new ItemData(130, -1, Resources.Load("Friends/face_fennec", typeof(Sprite)) as Sprite, "Prefab/IFR/130_IFR_Fennec"));
            itemList.Add(new ItemData(131, -1, Resources.Load("Friends/face_anotherServal", typeof(Sprite)) as Sprite, "Prefab/IFR/131_IFR_AnotherServal"));

            itemList.Add(new ItemData(132, -1, Resources.Load("Friends/face_serval", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(133, -1, Resources.Load("Friends/face_hyperServal", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(140, -1, Resources.Load("Friends/face_anyone", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(141, -1, null, "Prefab/IFR/141_IFR_RF"));
            itemList.Add(new ItemData(142, -1, null, "Prefab/IFR/142_IFR_Owls"));
            itemList.Add(new ItemData(143, -1, null, "Prefab/IFR/143_IFR_PPP"));
            itemList.Add(new ItemData(144, -1, null, "Prefab/IFR/144_IFR_Foxs"));
            itemList.Add(new ItemData(145, -1, null, "Prefab/IFR/145_IFR_LuckyBeastShop"));

            Sprite lbSprite = Resources.Load("Friends/face_lb", typeof(Sprite)) as Sprite;
            itemList.Add(new ItemData(150, -1, lbSprite, "Prefab/IFR/150_IFR_LuckyBeast"));
            itemList.Add(new ItemData(151, -1, Resources.Load("Friends/face_fossa", typeof(Sprite)) as Sprite, "Prefab/IFR/151_IFR_Fossa"));
            itemList.Add(new ItemData(152, -1, Resources.Load("Friends/face_tamandua", typeof(Sprite)) as Sprite, "Prefab/IFR/152_IFR_Tamandua"));
            itemList.Add(new ItemData(153, -1, Resources.Load("Friends/face_scarletIbis", typeof(Sprite)) as Sprite, "Prefab/IFR/153_IFR_ScarletIbis"));
            itemList.Add(new ItemData(154, -1, Resources.Load("Friends/face_oryx", typeof(Sprite)) as Sprite, "Prefab/IFR/154_IFR_Oryx"));
            itemList.Add(new ItemData(155, -1, Resources.Load("Friends/face_elephant", typeof(Sprite)) as Sprite, "Prefab/IFR/155_IFR_Elephant"));
            itemList.Add(new ItemData(156, -1, Resources.Load("Friends/face_axisDeer", typeof(Sprite)) as Sprite, "Prefab/IFR/156_IFR_AxisDeer"));
            itemList.Add(new ItemData(157, -1, Resources.Load("Friends/face_porcupine", typeof(Sprite)) as Sprite, "Prefab/IFR/157_IFR_Porcupine"));
            itemList.Add(new ItemData(158, -1, Resources.Load("Friends/face_aurochs", typeof(Sprite)) as Sprite, "Prefab/IFR/158_IFR_Aurochs"));
            itemList.Add(new ItemData(159, -1, Resources.Load("Friends/face_armadillo", typeof(Sprite)) as Sprite, "Prefab/IFR/159_IFR_Armadillo"));
            itemList.Add(new ItemData(160, -1, Resources.Load("Friends/face_chameleon", typeof(Sprite)) as Sprite, "Prefab/IFR/160_IFR_Chameleon"));
            itemList.Add(new ItemData(161, -1, Resources.Load("Friends/face_shoebill", typeof(Sprite)) as Sprite, "Prefab/IFR/161_IFR_Shoebill"));
            itemList.Add(new ItemData(162, -1, Resources.Load("Friends/face_giantPenguin", typeof(Sprite)) as Sprite, "Prefab/IFR/162_IFR_GiantPenguin"));
            itemList.Add(new ItemData(163, -1, Resources.Load("Friends/face_capybara", typeof(Sprite)) as Sprite, "Prefab/IFR/163_IFR_Capybara"));
            itemList.Add(new ItemData(164, -1, Resources.Load("Friends/face_tapir", typeof(Sprite)) as Sprite, "Prefab/IFR/164_IFR_Tapir"));
            itemList.Add(new ItemData(165, -1, Resources.Load("Friends/face_aardwolf", typeof(Sprite)) as Sprite, "Prefab/IFR/165_IFR_Aardwolf"));
            itemList.Add(new ItemData(166, -1, Resources.Load("Friends/face_dolphin", typeof(Sprite)) as Sprite, "Prefab/IFR/166_IFR_Dolphin"));
            itemList.Add(new ItemData(167, -1, Resources.Load("Friends/face_lbGenbu", typeof(Sprite)) as Sprite, "Prefab/IFR/167_IFR_LuckyBeastGenbu"));
            itemList.Add(new ItemData(168, -1, Resources.Load("Friends/face_lbType3", typeof(Sprite)) as Sprite, "Prefab/IFR/167_IFR_LuckyBeastType3"));

            itemList.Add(new ItemData(169, -1, Resources.Load("Friends/face_nana", typeof(Sprite)) as Sprite, "Prefab/IFR/169_IFR_Nana"));
            itemList.Add(new ItemData(170, -1, Resources.Load("Friends/face_mirai", typeof(Sprite)) as Sprite, "Prefab/IFR/170_IFR_Mirai"));
            itemList.Add(new ItemData(171, -1, Resources.Load("Friends/face_kako", typeof(Sprite)) as Sprite, "Prefab/IFR/171_IFR_Kako"));
            itemList.Add(new ItemData(172, -1, Resources.Load("Friends/face_anotherServal", typeof(Sprite)) as Sprite, "Prefab/IFR/172_IFR_SecondServal"));
            itemList.Add(new ItemData(173, -1, Resources.Load("Friends/face_oinarisama", typeof(Sprite)) as Sprite, "Prefab/IFR/173_IFR_Oinarisama"));
            itemList.Add(new ItemData(174, -1, Resources.Load("Friends/face_kitsunezaki", typeof(Sprite)) as Sprite, "Prefab/IFR/174_IFR_Kitsunezaki"));
            


            itemList.Add(new ItemData(200, -1, Resources.Load("Weapon/weapon_dodge", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(201, -1, Resources.Load("Weapon/weapon_escape", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(202, -1, Resources.Load("Weapon/weapon_gather", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(203, 150, Resources.Load("Weapon/weapon_claw", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(204, 450, Resources.Load("Weapon/weapon_spark", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(205, 1500, Resources.Load("Weapon/weapon_plasma", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(206, 120, Resources.Load("Weapon/weapon_armor", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(207, 360, Resources.Load("Weapon/weapon_bArmor", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(208, 1200, Resources.Load("Weapon/weapon_gArmor", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(209, 500, Resources.Load("Weapon/weapon_booster", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(210, 2000, Resources.Load("Weapon/weapon_booster2", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(211, 300, Resources.Load("Weapon/weapon_spaceJump", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(212, 80, Resources.Load("Weapon/weapon_comboAttack", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(213, 100, Resources.Load("Weapon/weapon_pileAttack", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(214, 120, Resources.Load("Weapon/weapon_spinAttack", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(215, 320, Resources.Load("Weapon/weapon_waveAttack", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(216, 350, Resources.Load("Weapon/weapon_screwAttack", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(217, 460, Resources.Load("Weapon/weapon_boltAttack", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(218, 1800, Resources.Load("Weapon/weapon_judgement", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(219, 5000, Resources.Load("Weapon/weapon_antimatter", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(220, 3000, Resources.Load("Weapon/weapon_analyzer", typeof(Sprite)) as Sprite, null));

            itemList.Add(new ItemData(221, 50, Resources.Load("Weapon/strength_hpUp", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(222, 50, Resources.Load("Weapon/strength_stUp", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(223, -1, Resources.Load("Weapon/strength_invUp", typeof(Sprite)) as Sprite, null));

            itemList.Add(new ItemData(224, -1, Resources.Load("Weapon/weapon_amulet", typeof(Sprite)) as Sprite, "Prefab/Item/224_Amulet"));
            itemList.Add(new ItemData(225, -1, Resources.Load("Weapon/weapon_oinari", typeof(Sprite)) as Sprite, "Prefab/Item/225_Oinari"));
            itemList.Add(new ItemData(226, -1, Resources.Load("Weapon/weapon_suzaku", typeof(Sprite)) as Sprite, "Prefab/Item/226_Suzaku"));
            itemList.Add(new ItemData(227, -1, Resources.Load("Weapon/weapon_byakko", typeof(Sprite)) as Sprite, "Prefab/Item/227_Byakko"));
            itemList.Add(new ItemData(228, -1, Resources.Load("Weapon/weapon_genbu", typeof(Sprite)) as Sprite, "Prefab/Item/228_Genbu"));
            itemList.Add(new ItemData(229, -1, Resources.Load("Weapon/weapon_seiryu", typeof(Sprite)) as Sprite, "Prefab/Item/229_Seiryu"));
            itemList.Add(new ItemData(230, -1, Resources.Load("Weapon/weapon_gyobu", typeof(Sprite)) as Sprite, "Prefab/Item/230_Gyobu"));
            itemList.Add(new ItemData(231, -1, Resources.Load("Weapon/weapon_pfAmulet", typeof(Sprite)) as Sprite, null));

            itemList.Add(new ItemData(255, 1500, Resources.Load("Weapon/weapon_plasma_another", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(267, 460, Resources.Load("Weapon/weapon_flareAttack", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(268, 1800, Resources.Load("Weapon/weapon_inferno", typeof(Sprite)) as Sprite, null));

            itemList.Add(new ItemData(300, 1, null, "Prefab/Gold/300_Gold_1"));
            itemList.Add(new ItemData(301, 5, null, "Prefab/Gold/301_Gold_5"));
            itemList.Add(new ItemData(302, 20, null, "Prefab/Gold/302_Gold_20"));
            itemList.Add(new ItemData(303, 100, null, "Prefab/Gold/303_Gold_100"));
            itemList.Add(new ItemData(310, -1, null, "Prefab/Container/310_ContainerD"));
            itemList.Add(new ItemData(311, -1, null, "Prefab/Container/311_ContainerC"));
            itemList.Add(new ItemData(312, -1, null, "Prefab/Container/312_ContainerB"));
            itemList.Add(new ItemData(313, -1, null, "Prefab/Container/313_ContainerA"));
            itemList.Add(new ItemData(314, -1, null, "Prefab/Container/314_ContainerX"));
            itemList.Add(new ItemData(315, -1, null, "Prefab/Container/315_ContentD"));
            itemList.Add(new ItemData(316, -1, null, "Prefab/Container/316_ContentC"));
            itemList.Add(new ItemData(317, -1, null, "Prefab/Container/317_ContentB"));
            itemList.Add(new ItemData(318, -1, null, "Prefab/Container/318_ContentA"));
            itemList.Add(new ItemData(319, -1, null, "Prefab/Container/319_ContentX"));
            itemList.Add(new ItemData(320, -1, null, "Prefab/Document/320_Document_Nana"));
            itemList.Add(new ItemData(321, -1, null, "Prefab/Document/321_Document_Ad"));
            itemList.Add(new ItemData(322, -1, null, "Prefab/Document/322_Document_Close"));
            itemList.Add(new ItemData(323, -1, null, "Prefab/Document/323_Document_Report1"));
            itemList.Add(new ItemData(324, -1, null, "Prefab/Document/324_Document_Mirai1"));
            itemList.Add(new ItemData(325, -1, null, "Prefab/Document/325_Document_Mirai2"));
            itemList.Add(new ItemData(326, -1, null, "Prefab/Document/326_Document_LuckyBeast"));
            itemList.Add(new ItemData(327, -1, null, "Prefab/Document/327_Document_Report2"));
            itemList.Add(new ItemData(328, -1, null, "Prefab/Document/328_Document_Close2"));
            itemList.Add(new ItemData(329, -1, null, "Prefab/Document/329_Document_ExperimentLog"));
            itemList.Add(new ItemData(330, -1, null, "Prefab/Document/330_Document_Kako"));
            itemList.Add(new ItemData(331, -1, null, "Prefab/Document/331_Document_Cerval"));
            itemList.Add(new ItemData(332, -1, null, "Prefab/Document/332_Document_ServalOld"));
            itemList.Add(new ItemData(333, -1, null, "Prefab/Document/333_Document_ServalNew"));
            itemList.Add(new ItemData(334, -1, null, "Prefab/Document/334_Document_ZOO"));
            itemList.Add(new ItemData(335, -1, null, "Prefab/Document/335_Document_Weapon"));
            itemList.Add(new ItemData(336, -1, null, "Prefab/Document/336_Document_Faust"));
            itemList.Add(new ItemData(337, -1, null, "Prefab/Document/337_Document_PhotoFaust"));
            itemList.Add(new ItemData(338, -1, null, "Prefab/Document/338_Document_PhotoCerval"));
            itemList.Add(new ItemData(339, -1, null, "Prefab/Document/339_Document_PhotoThree"));


            itemList.Add(new ItemData(350, -1, null, "Prefab/Others/350_WarpConsole"));
            itemList.Add(new ItemData(351, -1, null, "Prefab/Others/351_TeleportPointPrefab"));
            itemList.Add(new ItemData(352, -1, Resources.Load("Item/healStar", typeof(Sprite)) as Sprite, "Prefab/Others/352_HealStar"));
            itemList.Add(new ItemData(353, -1, null, "Prefab/Others/353_UnlockConsole"));
            itemList.Add(new ItemData(354, -1, Resources.Load("Item/sandstarBlock", typeof(Sprite)) as Sprite, "Prefab/Others/354_SandstarBlock"));

            itemList.Add(new ItemData(370, -1, Resources.Load("Trophy/trophy_0", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(371, -1, Resources.Load("Trophy/trophy_1", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(372, -1, Resources.Load("Trophy/trophy_2", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(373, -1, Resources.Load("Trophy/trophy_3", typeof(Sprite)) as Sprite, null));


            itemList.Add(new ItemData(380, -1, null, "Prefab/Debug/380_Debug_LevelDown"));
            itemList.Add(new ItemData(381, -1, null, "Prefab/Debug/381_Debug_LevelUp"));
            itemList.Add(new ItemData(382, -1, null, "Prefab/Debug/382_Debug_Call10Enemies"));
            itemList.Add(new ItemData(383, -1, null, "Prefab/Debug/383_Debug_ChangeEnemiesLevel"));
            itemList.Add(new ItemData(384, -1, null, "Prefab/Debug/384_Debug_Gold+10000"));
            itemList.Add(new ItemData(385, -1, null, "Prefab/Debug/385_Debug_Progress+1"));
            itemList.Add(new ItemData(386, -1, null, "Prefab/Debug/386_Debug_Progress-1"));
            itemList.Add(new ItemData(387, -1, null, "Prefab/Debug/387_Debug_MaximizeInventory"));
            itemList.Add(new ItemData(388, -1, null, "Prefab/Debug/388_Debug_SaveAllLB"));
            itemList.Add(new ItemData(389, -1, null, "Prefab/Debug/389_Debug_Call1Enemy"));
            itemList.Add(new ItemData(390, -1, null, "Prefab/Debug/390_Debug_EnemyIdPlus"));
            itemList.Add(new ItemData(391, -1, null, "Prefab/Debug/391_Debug_EnemyIdMinus"));
            itemList.Add(new ItemData(392, -1, null, "Prefab/Debug/392_Debug_ShowID"));
            itemList.Add(new ItemData(393, -1, null, "Prefab/Debug/393_Debug_Difficulty"));
            itemList.Add(new ItemData(394, -1, null, "Prefab/Debug/394_Debug_InitializeAll"));
            itemList.Add(new ItemData(395, -1, null, "Prefab/Debug/395_Debug_SaveAllFriends"));
            itemList.Add(new ItemData(396, -1, null, "Prefab/Debug/396_Debug_CallVariousEnemies"));

            itemList.Add(new ItemData(400, -1, Resources.Load("Config/config_quit", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(401, -1, Resources.Load("Config/config_reset", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(402, -1, Resources.Load("Config/config_language", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(403, -1, Resources.Load("Config/config_keyBinds", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(404, -1, Resources.Load("Config/config_tutorial", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(405, -1, Resources.Load("Config/config_volume", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(406, -1, Resources.Load("Config/config_control", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(407, -1, Resources.Load("Config/config_camera", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(408, -1, Resources.Load("Config/config_show", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(409, -1, Resources.Load("Config/config_size", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(410, -1, Resources.Load("Config/config_highCost", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(411, -1, Resources.Load("Config/config_effect", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(412, -1, Resources.Load("Config/config_audio", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(413, -1, Resources.Load("Config/config_system", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(414, -1, Resources.Load("Config/config_assist", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(415, -1, Resources.Load("Config/config_expert", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(416, -1, Resources.Load("Config/config_trophy", typeof(Sprite)) as Sprite, null));

            itemList.Add(new ItemData(500, 0, Resources.Load("Facility/face_JapariLibrary", typeof(Sprite)) as Sprite, "Prefab/Facility/JapariLibrary"));
            itemList.Add(new ItemData(501, 1, Resources.Load("Facility/face_Pool", typeof(Sprite)) as Sprite, "Prefab/Facility/Pool"));
            itemList.Add(new ItemData(502, 2, Resources.Load("Facility/face_Suberidai", typeof(Sprite)) as Sprite, "Prefab/Facility/Suberidai"));
            itemList.Add(new ItemData(503, 3, Resources.Load("Facility/face_JunglePlant", typeof(Sprite)) as Sprite, "Prefab/Facility/JunglePlant_Library"));
            itemList.Add(new ItemData(504, 4, Resources.Load("Facility/face_JapariCafe", typeof(Sprite)) as Sprite, "Prefab/Facility/JapariCafe"));
            itemList.Add(new ItemData(505, 5, Resources.Load("Facility/face_DesertCave", typeof(Sprite)) as Sprite, "Prefab/Facility/DesertCave"));
            itemList.Add(new ItemData(506, 6, Resources.Load("Facility/face_LogHouse", typeof(Sprite)) as Sprite, "Prefab/Facility/LogHouse"));
            itemList.Add(new ItemData(507, 7, Resources.Load("Facility/face_ItemBox", typeof(Sprite)) as Sprite, "Prefab/Facility/ItemBox"));
            itemList.Add(new ItemData(508, 8, Resources.Load("Facility/face_MogisenButai", typeof(Sprite)) as Sprite, "Prefab/Facility/MogisenButai"));
            itemList.Add(new ItemData(509, 9, Resources.Load("Facility/face_LionCastle", typeof(Sprite)) as Sprite, "Prefab/Facility/LionCastle"));
            itemList.Add(new ItemData(510, 10, Resources.Load("Facility/face_PPP_Butai", typeof(Sprite)) as Sprite, "Prefab/Facility/PPP_Butai"));
            itemList.Add(new ItemData(511, 11, Resources.Load("Facility/face_MusicBox", typeof(Sprite)) as Sprite, "Prefab/Facility/MusicBox"));
            itemList.Add(new ItemData(512, 12, Resources.Load("Facility/face_Onsen", typeof(Sprite)) as Sprite, "Prefab/Facility/Onsen"));
            itemList.Add(new ItemData(513, 13, Resources.Load("Facility/face_VideoGame", typeof(Sprite)) as Sprite, "Prefab/Facility/VideoGameConsole"));
            itemList.Add(new ItemData(514, 13, null, "Prefab/Facility/VideoGameCase"));
            itemList.Add(new ItemData(515, 14, Resources.Load("Facility/face_LodgeRoom", typeof(Sprite)) as Sprite, "Prefab/Facility/LodgeRoom"));
            itemList.Add(new ItemData(516, 14, null, "Prefab/Facility/Illust_Giraffe"));
            itemList.Add(new ItemData(517, 14, null, "Prefab/Facility/Illust_CampoFlicker"));
            itemList.Add(new ItemData(518, 14, null, "Prefab/Facility/Pencil"));
            itemList.Add(new ItemData(519, 15, Resources.Load("Facility/face_Kitchen", typeof(Sprite)) as Sprite, "Prefab/Facility/Kitchen"));
            itemList.Add(new ItemData(520, 15, null, "Prefab/Facility/KitchenFire"));
            itemList.Add(new ItemData(521, 16, Resources.Load("Facility/face_MiniInductionCooker", typeof(Sprite)) as Sprite, "Prefab/Facility/MiniInductionCooker"));
            itemList.Add(new ItemData(522, 17, null, "Prefab/Facility/BusLikeSet"));
            itemList.Add(new ItemData(523, 17, Resources.Load("Facility/face_BusLike", typeof(Sprite)) as Sprite, "Prefab/Facility/BusLikeOnly"));
            itemList.Add(new ItemData(524, 18, Resources.Load("Facility/face_JapariBus", typeof(Sprite)) as Sprite, "Prefab/Facility/JapariBus"));
            itemList.Add(new ItemData(525, -1, null, "Prefab/Facility/AnotherServal_Sleeping"));
            itemList.Add(new ItemData(526, 19, null, "Prefab/Facility/Statue_Blue"));
            itemList.Add(new ItemData(527, 19, null, "Prefab/Facility/Statue_Red"));
            itemList.Add(new ItemData(528, 19, null, "Prefab/Facility/Statue_Violet"));
            itemList.Add(new ItemData(529, 19, null, "Prefab/Facility/Statue_Black"));
            itemList.Add(new ItemData(530, 19, null, "Prefab/Facility/Statue_Silver"));
            itemList.Add(new ItemData(531, 19, null, "Prefab/Facility/Statue_Golden"));
            itemList.Add(new ItemData(532, 19, Resources.Load("Facility/face_Statue", typeof(Sprite)) as Sprite, "Prefab/Facility/Statue_Complex"));
            itemList.Add(new ItemData(533, 20, Resources.Load("Facility/face_Sushizanmai", typeof(Sprite)) as Sprite, "Prefab/Facility/Sushizanmai"));
            itemList.Add(new ItemData(534, -1, Resources.Load("Facility/face_DictionaryBook", typeof(Sprite)) as Sprite, "Prefab/Facility/DictionaryBook"));
            itemList.Add(new ItemData(535, 21, Resources.Load("Enemy/face_054_Dummy", typeof(Sprite)) as Sprite, null));
            itemList.Add(new ItemData(536, -1, Resources.Load("Facility/face_DifficultyBook", typeof(Sprite)) as Sprite, "Prefab/Facility/DifficultyBook"));
            itemList.Add(new ItemData(537, -1, Resources.Load("Facility/face_TutorialBook", typeof(Sprite)) as Sprite, "Prefab/Facility/TutorialBook"));
            itemList.Add(new ItemData(538, -1, lbSprite, null));

            //itemList.Sort((a, b) => a.id - b.id);
            for (int i = 0; i < typeMax; i++) {
                typeCount[i] = itemList.Count(n => n.id / 100 == i);
            }
        }
    }

    public ItemData GetItemData(int id) {
        return itemList.Find(n => n.id == id);
    }

    public int GetRandomItemId() {
        return itemList[Random.Range(0, typeCount[0])].id;
    }

    public GameObject GetItemPrefab(int id) {
        string path = itemList.Find(n => n.id == id).path;
        if (path != null) {
            if (!mList.ContainsKey(path)) {
                mList[path] = Resources.Load(path, typeof(GameObject)) as GameObject;
            }
            return mList[path];
        } else {
            return null;
        }
    }

    public GameObject GetContainerPrefab(int rank) {
        if (rank < 0) {
            rank = 0;
        } else if (rank >= containerId.Length) {
            rank = containerId.Length - 1;
        }
        return GetItemPrefab(containerId[rank]);
    }

    public GameObject GetGoldPrefab(int rank) {
        if (rank < 0) {
            rank = 0;
        } else if (rank >= goldId.Length) {
            rank = goldId.Length - 1;
        }
        return GetItemPrefab(goldId[rank]);
    }
        
    public int GetItemPrice(int id) {
        return itemList.Find(n => n.id == id).price;
    }

    public string GetItemName(int id) {
        return TextManager.Get("ITEM_NAME_" + id.ToString("000"));
    }

    public string GetItemInfomation(int id) {
        return TextManager.Get("ITEM_INFO_" + id.ToString("000"));
    }

    public Sprite GetItemImage(int id) {
        return itemList.Find(n => n.id == id).image;
    }

    public void GiveGold(int price, Transform pivot) {
        int[] goldNum = new int[goldMax];
        int sum = 0;
        for (int i = goldMax - 1; i >= 0; i--) {
            goldNum[i] = price / goldUnits[i];
            price -= goldUnits[i] * goldNum[i];
            sum += goldNum[i];
        }        
        int[] itemId = new int[sum];
        for (int i = 0; i < sum; i++) {
            if (i < goldNum[0]) {
                itemId[i] = goldId[0];
            } else if (i < goldNum[0] + goldNum[1]) {
                itemId[i] = goldId[1];
            } else if (i < goldNum[0] + goldNum[1] + goldNum[2]) {
                itemId[i] = goldId[2];
            } else {
                itemId[i] = goldId[3];
            }
        }
        for (int i = itemId.Length - 1; i > 0; i--) {
            int j = Random.Range(0, i + 1);
            int temp = itemId[i];
            itemId[i] = itemId[j];
            itemId[j] = temp;
        }
        GiveItem(itemId, pivot);
    }

    void SetItemStatus(GameObject itemInstance, float force = 5, float balloonDelay = -1, float getDelay = -1) {
        if (balloonDelay >= 0 || getDelay >= 0) {
            GetItem getItem = itemInstance.GetComponent<GetItem>();
            if (getItem) {
                if (balloonDelay >= 0) {
                    getItem.balloonDelay = balloonDelay;
                }
                if (getDelay >= 0) {
                    getItem.getDelay = getDelay;
                }
            } else {
                GetDocument getDocument = itemInstance.GetComponent<GetDocument>();
                if (getDocument) {
                    if (balloonDelay >= 0) {
                        getDocument.balloonDelay = balloonDelay;
                    }
                    if (getDelay >= 0) {
                        getDocument.getDelay = getDelay;
                    }
                }
            }
        }
        if (force != 0) {
            Rigidbody rb = itemInstance.GetComponent<Rigidbody>();
            if (rb) {
                rb.AddForce(Vector3.up * force, ForceMode.VelocityChange);
            }
        }
    }

    public void GiveItem(int itemId, Vector3 pos, float force = 5, float balloonDelay = -1, float getDelay = -1, float followInertDelay = -1f, Transform parent = null, int replaceLevel = 0) {
        if (itemId >= 0) {
            if (replaceLevel > 0) {
                for (int i = 0; i < replaceBeforeID.Length; i++) {
                    if (itemId == replaceBeforeID[i]) {
                        itemId = replaceAfterID[Mathf.Clamp(replaceLevel - 1, 0, replaceRows - 1), Random.Range(0, replaceColumns)];
                        break;
                    }
                }
            }
            GameObject itemPrefab = GetItemPrefab(itemId);
            if (itemPrefab) {
                GameObject itemInstance = Instantiate(itemPrefab, pos, Quaternion.identity, parent);
                SetItemStatus(itemInstance, force, balloonDelay, getDelay);
                if (followInertDelay > 0f) {
                    FollowTarget_Item followTargetItem = itemInstance.GetComponentInChildren<FollowTarget_Item>();
                    if (followTargetItem) {
                        followTargetItem.inertDelay = followInertDelay;
                    }
                }
            }
        }
    }

    public void GiveItem(int[] itemId, Transform pivot, float force = 5, float balloonDelay = -1, float getDelay = -1, float followInertDelay = -1f, Transform parent = null, int replaceLevel = 0) {
        int num = 0;
        for (int i = 0; i < itemId.Length; i++) {
            if (itemId[i] >= 0) {
                num++;
            }
        }
        if (num > 0) {
            float angStart = (num <= 1 ? 0f : pivot.eulerAngles.y + 360f / num * 0.5f);
            float angDiff = (num <= 1 ? 0f : 360f / num);
            float radius = (num <= 1 ? 0f : num <= 3 ? 0.2f : 0.2f + (num - 3) * 0.1f);
            GameObject itemPrefab;
            Vector3 childPos;
            for (int i = 0; i < itemId.Length; i++) {
                if (itemId[i] >= 0) {
                    if (replaceLevel > 0) {
                        for (int j = 0; j < replaceBeforeID.Length; j++) {
                            if (itemId[i] == replaceBeforeID[j]) {
                                itemId[i] = replaceAfterID[Mathf.Clamp(replaceLevel - 1, 0, replaceRows - 1), Random.Range(0, replaceColumns)];
                                break;
                            }
                        }
                    }
                    itemPrefab = GetItemPrefab(itemId[i]);
                    if (itemPrefab) {
                        childPos = pivot.position;
                        if (num > 1) {
                            float angle = (angStart + angDiff * i) * Mathf.Deg2Rad;
                            childPos.x += Mathf.Sin(angle) * radius;
                            childPos.z += Mathf.Cos(angle) * radius;
                        }                        
                        GameObject itemInstance = Instantiate(itemPrefab, childPos, Quaternion.identity, parent);
                        SetItemStatus(itemInstance, force, balloonDelay, getDelay);
                        if (followInertDelay > 0f) {
                            FollowTarget_Item followTargetItem = itemInstance.GetComponentInChildren<FollowTarget_Item>();
                            if (followTargetItem) {
                                followTargetItem.inertDelay = followInertDelay;
                            }
                        }
                    }
                }
            }
        }
    }

}
