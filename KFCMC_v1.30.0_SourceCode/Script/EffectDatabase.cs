using UnityEngine;

public class EffectDatabase : SingletonMonoBehaviour<EffectDatabase> {

    [System.NonSerialized]
    public GameObject[] prefab;

    public enum id {
        empty, friendsAppear, friendsDisappear, friendsWarp, friendsWarpSound, friendsLevelUp, friendsYKReady, friendsYKStart, friendsYKDuring, friendsYKEnd,
        enemyLevelUp, enemyLevelDown, enemyYK, enemyYK_Boss, enemyYK_LastBoss, enemyYK_Man, enemyYK_Aura, enemyYK_AuraBig, enemyYK_AuraBigSuper,
        talk, getFriends, fieldBuff, alarmHP, alarmST,
        sickFire, sickPoison, sickIce, sickAcid, sickSlow, sickStop, sickFrightened, sickFireEnemy,
        itemHeal01, itemHeal02, itemHeal03, itemSandstar, itemAntidote, itemAntidoteSE,
        itemBuffSE, itemBuffStamina, itemBuffSpeed, itemBuffAttack, itemBuffDefense, itemBuffKnock, itemBuffAbsorb, itemBuffStealth, itemBuffLong, itemBuffMulti,
        itemWarp, itemGuide, itemClearTraps, clearTraps, itemGenerateGolds, itemBreakWalls, itemReplaceWalls,
        itemCrystal_GS, itemCrystal_GM, itemCrystal_GL, itemCrystal_K, itemCrystal_R, itemCrystal_B, itemCrystal_V, itemCrystal_Y, itemCrystal_C, itemCrystal_W,
        itemBomb01, itemBomb01_Kaban, itemBomb02, itemBomb02_Kaban, itemBomb03, itemBomb03_Kaban,
        itemBomb01_L, itemBomb01_Kaban_L, itemBomb02_L, itemBomb02_Kaban_L, itemBomb03_L, itemBomb03_Kaban_L,
        itemJaparibun01, itemJaparibun02, itemJaparibun03,
        itemSandstarRaw, itemCelliumCure,
        giraffeBeam, sandstarRawCloud, sandstarBlow, sacrifice, breathBubble,
        defeatMissionStartSFX, defeatMissionCompleteSFX, defeatMissionCompleteVFX,
        blackMinmiGlitch, levelFive, passiveSneeze, protect, protectNoSfx, notificationShortcut
    };

    protected override void Awake() {
        if (CheckInstance()) {
            DontDestroyOnLoad(gameObject);
            prefab = new GameObject[System.Enum.GetValues(typeof(id)).Length];
            prefab[(int)id.empty] = Resources.Load("Prefab/Effect/Eff_Empty") as GameObject;
            prefab[(int)id.friendsAppear] = Resources.Load("Prefab/Effect/Eff_FriendsAppear") as GameObject;
            prefab[(int)id.friendsDisappear] = Resources.Load("Prefab/Effect/Eff_FriendsDisappear") as GameObject;
            prefab[(int)id.friendsWarp] = Resources.Load("Prefab/Effect/Eff_FriendsWarp") as GameObject;
            prefab[(int)id.friendsWarpSound] = Resources.Load("Prefab/Effect/Eff_FriendsWarpSound") as GameObject;
            prefab[(int)id.friendsLevelUp] = Resources.Load("Prefab/Effect/Eff_FriendsLevelUp") as GameObject;
            prefab[(int)id.friendsYKReady] = Resources.Load("Prefab/Effect/Eff_FriendsYKReady") as GameObject;
            prefab[(int)id.friendsYKStart] = Resources.Load("Prefab/Effect/Eff_FriendsYKStart") as GameObject;
            prefab[(int)id.friendsYKDuring] = Resources.Load("Prefab/Effect/Eff_FriendsYKDuring") as GameObject;
            prefab[(int)id.friendsYKEnd] = Resources.Load("Prefab/Effect/Eff_FriendsYKEnd") as GameObject;
            prefab[(int)id.enemyLevelUp] = Resources.Load("Prefab/Effect/Eff_EnemyLevelUp") as GameObject;
            prefab[(int)id.enemyLevelDown] = Resources.Load("Prefab/Effect/Eff_EnemyLevelDown") as GameObject;
            prefab[(int)id.enemyYK] = Resources.Load("Prefab/Effect/Eff_EnemyYK") as GameObject;
            prefab[(int)id.enemyYK_Boss] = Resources.Load("Prefab/Effect/Eff_EnemyYK_Boss") as GameObject;
            prefab[(int)id.enemyYK_LastBoss] = Resources.Load("Prefab/Effect/Eff_EnemyYK_LastBoss") as GameObject;
            prefab[(int)id.enemyYK_Man] = Resources.Load("Prefab/Effect/Eff_EnemyYK_Man") as GameObject;
            prefab[(int)id.enemyYK_Aura] = Resources.Load("Prefab/Effect/Eff_EnemyYK_Aura") as GameObject;
            prefab[(int)id.enemyYK_AuraBig] = Resources.Load("Prefab/Effect/Eff_EnemyYK_AuraBig") as GameObject;
            prefab[(int)id.enemyYK_AuraBigSuper] = Resources.Load("Prefab/Effect/Eff_EnemyYK_AuraBigSuper") as GameObject;
            prefab[(int)id.talk] = Resources.Load("Prefab/Effect/Eff_Talk") as GameObject;
            prefab[(int)id.getFriends] = Resources.Load("Prefab/Effect/Eff_GetFriends") as GameObject;
            prefab[(int)id.fieldBuff] = Resources.Load("Prefab/Effect/Eff_FieldBuff") as GameObject;
            prefab[(int)id.alarmHP] = Resources.Load("Prefab/Effect/Eff_Alarm_HP") as GameObject;
            prefab[(int)id.alarmST] = Resources.Load("Prefab/Effect/Eff_Alarm_ST") as GameObject;
            prefab[(int)id.sickFire] = Resources.Load("Prefab/Effect/Eff_SickFire") as GameObject;
            prefab[(int)id.sickPoison] = Resources.Load("Prefab/Effect/Eff_SickPoison") as GameObject;
            prefab[(int)id.sickIce] = Resources.Load("Prefab/Effect/Eff_SickIce") as GameObject;
            prefab[(int)id.sickAcid] = Resources.Load("Prefab/Effect/Eff_SickAcid") as GameObject;
            prefab[(int)id.sickSlow] = Resources.Load("Prefab/Effect/Eff_SickSlow") as GameObject;
            prefab[(int)id.sickStop] = Resources.Load("Prefab/Effect/Eff_SickStop") as GameObject;
            prefab[(int)id.sickFrightened] = Resources.Load("Prefab/Effect/Eff_SickFrightened") as GameObject;
            prefab[(int)id.sickFireEnemy] = Resources.Load("Prefab/Effect/Eff_SickFireEnemy") as GameObject;
            prefab[(int)id.itemHeal01] = Resources.Load("Prefab/Effect/Eff_ItemHeal01") as GameObject;
            prefab[(int)id.itemHeal02] = Resources.Load("Prefab/Effect/Eff_ItemHeal02") as GameObject;
            prefab[(int)id.itemHeal03] = Resources.Load("Prefab/Effect/Eff_ItemHeal03") as GameObject;
            prefab[(int)id.itemSandstar] = Resources.Load("Prefab/Effect/Eff_ItemSandstar") as GameObject;
            prefab[(int)id.itemAntidote] = Resources.Load("Prefab/Effect/Eff_ItemAntidote") as GameObject;
            prefab[(int)id.itemAntidoteSE] = Resources.Load("Prefab/Effect/Eff_ItemAntidoteSE") as GameObject;
            prefab[(int)id.itemWarp] = Resources.Load("Prefab/Effect/Eff_ItemWarp") as GameObject;
            prefab[(int)id.itemGuide] = Resources.Load("Prefab/Effect/Eff_ItemGuide") as GameObject;
            prefab[(int)id.itemClearTraps] = Resources.Load("Prefab/Effect/Eff_ItemClearTraps") as GameObject;
            prefab[(int)id.clearTraps] = Resources.Load("Prefab/Effect/Eff_ClearTraps") as GameObject;
            prefab[(int)id.itemGenerateGolds] = Resources.Load("Prefab/Effect/Eff_ItemGenerateGolds") as GameObject;
            prefab[(int)id.itemBreakWalls] = Resources.Load("Prefab/Effect/Eff_ItemBreakWalls") as GameObject;
            prefab[(int)id.itemReplaceWalls] = Resources.Load("Prefab/Effect/Eff_ItemReplaceWalls") as GameObject;
            prefab[(int)id.itemBuffSE] = Resources.Load("Prefab/Effect/Eff_ItemBuffSE") as GameObject;
            prefab[(int)id.itemBuffStamina] = Resources.Load("Prefab/Effect/Eff_ItemBuffStamina") as GameObject;
            prefab[(int)id.itemBuffSpeed] = Resources.Load("Prefab/Effect/Eff_ItemBuffSpeed") as GameObject;
            prefab[(int)id.itemBuffAttack] = Resources.Load("Prefab/Effect/Eff_ItemBuffAttack") as GameObject;
            prefab[(int)id.itemBuffDefense] = Resources.Load("Prefab/Effect/Eff_ItemBuffDefense") as GameObject;
            prefab[(int)id.itemBuffKnock] = Resources.Load("Prefab/Effect/Eff_ItemBuffKnock") as GameObject;
            prefab[(int)id.itemBuffAbsorb] = Resources.Load("Prefab/Effect/Eff_ItemBuffAbsorb") as GameObject;
            prefab[(int)id.itemBuffStealth] = Resources.Load("Prefab/Effect/Eff_ItemBuffStealth") as GameObject;
            prefab[(int)id.itemBuffLong] = Resources.Load("Prefab/Effect/Eff_ItemBuffLong") as GameObject;
            prefab[(int)id.itemBuffMulti] = Resources.Load("Prefab/Effect/Eff_ItemBuffMulti") as GameObject;
            prefab[(int)id.itemCrystal_GS] = Resources.Load("Prefab/Effect/Eff_ItemCrystal_GS") as GameObject;
            prefab[(int)id.itemCrystal_GM] = Resources.Load("Prefab/Effect/Eff_ItemCrystal_GM") as GameObject;
            prefab[(int)id.itemCrystal_GL] = Resources.Load("Prefab/Effect/Eff_ItemCrystal_GL") as GameObject;
            prefab[(int)id.itemCrystal_K] = Resources.Load("Prefab/Effect/Eff_ItemCrystal_K") as GameObject;
            prefab[(int)id.itemCrystal_R] = Resources.Load("Prefab/Effect/Eff_ItemCrystal_R") as GameObject;
            prefab[(int)id.itemCrystal_B] = Resources.Load("Prefab/Effect/Eff_ItemCrystal_B") as GameObject;
            prefab[(int)id.itemCrystal_V] = Resources.Load("Prefab/Effect/Eff_ItemCrystal_V") as GameObject;
            prefab[(int)id.itemCrystal_Y] = Resources.Load("Prefab/Effect/Eff_ItemCrystal_Y") as GameObject;
            prefab[(int)id.itemCrystal_C] = Resources.Load("Prefab/Effect/Eff_ItemCrystal_C") as GameObject;
            prefab[(int)id.itemCrystal_W] = Resources.Load("Prefab/Effect/Eff_ItemCrystal_W") as GameObject;
            prefab[(int)id.itemBomb01] = Resources.Load("Prefab/Effect/Eff_ItemBomb01") as GameObject;
            prefab[(int)id.itemBomb01_Kaban] = Resources.Load("Prefab/Effect/Eff_ItemBomb01_Kaban") as GameObject;
            prefab[(int)id.itemBomb02] = Resources.Load("Prefab/Effect/Eff_ItemBomb02") as GameObject;
            prefab[(int)id.itemBomb02_Kaban] = Resources.Load("Prefab/Effect/Eff_ItemBomb02_Kaban") as GameObject;
            prefab[(int)id.itemBomb03] = Resources.Load("Prefab/Effect/Eff_ItemBomb03") as GameObject;
            prefab[(int)id.itemBomb03_Kaban] = Resources.Load("Prefab/Effect/Eff_ItemBomb03_Kaban") as GameObject;
            prefab[(int)id.itemBomb01_L] = Resources.Load("Prefab/Effect/Eff_ItemBomb01_L") as GameObject;
            prefab[(int)id.itemBomb01_Kaban_L] = Resources.Load("Prefab/Effect/Eff_ItemBomb01_Kaban_L") as GameObject;
            prefab[(int)id.itemBomb02_L] = Resources.Load("Prefab/Effect/Eff_ItemBomb02_L") as GameObject;
            prefab[(int)id.itemBomb02_Kaban_L] = Resources.Load("Prefab/Effect/Eff_ItemBomb02_Kaban_L") as GameObject;
            prefab[(int)id.itemBomb03_L] = Resources.Load("Prefab/Effect/Eff_ItemBomb03_L") as GameObject;
            prefab[(int)id.itemBomb03_Kaban_L] = Resources.Load("Prefab/Effect/Eff_ItemBomb03_Kaban_L") as GameObject;
            prefab[(int)id.itemJaparibun01] = Resources.Load("Prefab/Effect/Eff_ItemJaparibun01") as GameObject;
            prefab[(int)id.itemJaparibun02] = Resources.Load("Prefab/Effect/Eff_ItemJaparibun02") as GameObject;
            prefab[(int)id.itemJaparibun03] = Resources.Load("Prefab/Effect/Eff_ItemJaparibun03") as GameObject;
            prefab[(int)id.itemSandstarRaw] = Resources.Load("Prefab/Effect/Eff_ItemSandstarRaw") as GameObject;
            prefab[(int)id.itemCelliumCure] = Resources.Load("Prefab/Effect/Eff_ItemCelliumCure") as GameObject;
            prefab[(int)id.giraffeBeam] = Resources.Load("Prefab/Effect/Eff_GiraffeBeam") as GameObject;
            prefab[(int)id.sandstarRawCloud] = Resources.Load("Prefab/Effect/Eff_SandstarRawCloud") as GameObject;
            prefab[(int)id.sandstarBlow] = Resources.Load("Prefab/Effect/Eff_SandstarBlow") as GameObject;
            prefab[(int)id.sacrifice] = Resources.Load("Prefab/Effect/Eff_Sacrifice") as GameObject;
            prefab[(int)id.breathBubble] = Resources.Load("Prefab/Effect/Eff_BreathBubble") as GameObject;
            prefab[(int)id.defeatMissionStartSFX] = Resources.Load("Prefab/Effect/Eff_DefeatMissionStartSFX") as GameObject;
            prefab[(int)id.defeatMissionCompleteSFX] = Resources.Load("Prefab/Effect/Eff_DefeatMissionCompleteSFX") as GameObject;
            prefab[(int)id.defeatMissionCompleteVFX] = Resources.Load("Prefab/Effect/Eff_DefeatMissionCompleteVFX") as GameObject;
            prefab[(int)id.blackMinmiGlitch] = Resources.Load("Prefab/Effect/Eff_BlackMinmiGlitch") as GameObject;
            prefab[(int)id.levelFive] = Resources.Load("Prefab/Effect/Eff_LevelFive") as GameObject;
            prefab[(int)id.passiveSneeze] = Resources.Load("Prefab/Effect/Eff_PassiveSneeze") as GameObject;
            prefab[(int)id.protect] = Resources.Load("Prefab/Effect/Eff_Protect") as GameObject;
            prefab[(int)id.protectNoSfx] = Resources.Load("Prefab/Effect/Eff_ProtectNoSfx") as GameObject;
            prefab[(int)id.notificationShortcut] = Resources.Load("Prefab/Effect/Eff_NotificationShortcut") as GameObject;
        }
    }
    
}
