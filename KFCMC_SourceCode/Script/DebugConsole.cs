using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugConsole : EventOnEnterAndSubmit {

    public enum DebugType {
        LevelDown, LevelUp, Call10Enemies, ChangeEnemiesLevel, Gold, ProgressUp, ProgressDown, Inventory, SaveAllLB, Call1Enemy, EnemyIdPlus, EnemyIdMinus, InitializeAll, SaveAllFriends, CallVariousEnemies
    }

    public DebugType debugType;
    
    int enemyLevel = 1;

    void CallEnemy(int num, bool fixEnemyPointer) {
        if (StageManager.Instance && StageManager.Instance.dungeonController) {
            for (int i = 0; i < StageManager.Instance.dungeonController.enemySettings.enemy.Length; i++) {
                StageManager.Instance.dungeonController.enemySettings.enemy[i].maxNum = 20;
            }
            StageManager.Instance.dungeonController.enemySettings.respawnable = true;
            StageManager.Instance.dungeonController.fixEnemyPointer = fixEnemyPointer;
            StageManager.Instance.dungeonController.SummonEnemies(num, -1, 30);
        }
    }

    protected override void CallAction()
    {
        switch (debugType)
        {
            case DebugType.LevelDown:
                CharacterManager.Instance.AddExp((GameManager.Instance.save.NowLevelExp + 1) * (-1), true);
                MessageUI.Instance.SetMessageReplace(TextManager.Get("STATUS_LEVEL") + " " + GameManager.Instance.save.Level, "DBG_STATUS_LEVEL");
                break;
            case DebugType.LevelUp:
                CharacterManager.Instance.AddExp(GameManager.Instance.save.NeedExpToNextLevel, true);
                MessageUI.Instance.SetMessageReplace(TextManager.Get("STATUS_LEVEL") + " " + GameManager.Instance.save.Level, "DBG_STATUS_LEVEL");
                break;
            case DebugType.Call10Enemies:
                CallEnemy(10, true);
                break;
            case DebugType.ChangeEnemiesLevel:
                enemyLevel = (enemyLevel + 1) % GameManager.enemyLevelMax;
                if (StageManager.Instance && StageManager.Instance.dungeonController)
                {
                    for (int i = 0; i < StageManager.Instance.dungeonController.enemySettings.enemy.Length; i++)
                    {
                        StageManager.Instance.dungeonController.enemySettings.enemy[i].level = enemyLevel;
                    }
                }
                MessageEnemyNameAndLevel();
                break;
            case DebugType.Gold:
                GameManager.Instance.save.money += 10000;
                CharacterManager.Instance.ShowGold();
                break;
            case DebugType.ProgressUp:
                if (GameManager.Instance.save.progress < 13)
                {
                    GameManager.Instance.save.progress += 1;
                }
                GameManager.Instance.FixProgressWeapon();
                MessageUI.Instance.SetMessageReplace(TextManager.Get("MESSAGE_DBG_PROGRESS") + " " + GameManager.Instance.save.progress, "MESSAGE_DBG_PROGRESS");
                break;
            case DebugType.ProgressDown:
                if (GameManager.Instance.save.progress > 0)
                {
                    GameManager.Instance.save.progress -= 1;
                }
                GameManager.Instance.FixProgressWeapon();
                MessageUI.Instance.SetMessageReplace(TextManager.Get("MESSAGE_DBG_PROGRESS") + " " + GameManager.Instance.save.progress, "MESSAGE_DBG_PROGRESS");
                break;
            case DebugType.Inventory:
                for (int i = 0; i < GameManager.inventoryNFSMax; i++)
                {
                    GameManager.Instance.save.inventoryNFS[i] = 1;
                    GameManager.Instance.save.weapon[GameManager.invUpId - 200] = 1;
                    GameManager.Instance.save.equip[GameManager.invUpId - 200] = 1;
                }
                MessageUI.Instance.SetMessageReplace(TextManager.Get("MESSAGE_DBG_INVENTORY"), "MESSAGE_DBG_INVENTORY");
                break;
            case DebugType.SaveAllLB:
                for (int i = 0; i < GameManager.Instance.save.luckyBeast.Length; i++)
                {
                    GameManager.Instance.save.luckyBeast[i] = 1;
                }
                MessageUI.Instance.SetMessageReplace(TextManager.Get("MESSAGE_DBG_LUCKY_BEASTS"), "MESSAGE_DBG_LUCKY_BEASTS");
                break;
            case DebugType.Call1Enemy:
                CallEnemy(1, true);
                break;
            case DebugType.EnemyIdPlus:
                if (StageManager.Instance.dungeonController)
                {
                    StageManager.Instance.dungeonController.PlusEnemyPointer(1);
                }
                MessageEnemyNameAndLevel();
                break;
            case DebugType.EnemyIdMinus:
                if (StageManager.Instance.dungeonController)
                {
                    StageManager.Instance.dungeonController.PlusEnemyPointer(-1);
                }
                MessageEnemyNameAndLevel();
                break;
            case DebugType.InitializeAll:
                if (PauseController.Instance.GetPauseEnabled())
                {
                    PauseController.Instance.SetChoices(7, true, TextManager.Get("WORD_INITIALIZE_ALL"), "CHOICE_CANCEL", "CHOICE_CANCEL", "CHOICE_CANCEL", "CHOICE_EXECUTE", "CHOICE_CANCEL", "CHOICE_CANCEL", "CHOICE_CANCEL");
                    state = 1;
                }
                break;
            case DebugType.SaveAllFriends:
                if (PauseController.Instance.GetPauseEnabled())
                {
                    PauseController.Instance.SetChoices(3, true, TextManager.Get("WORD_SAVE_ALL_FRIENDS"), "CHOICE_CALLABLE_FRIENDS", "CHOICE_OTHER_FRIENDS", "CHOICE_CANCEL");
                    state = 1;
                }
                break;
            case DebugType.CallVariousEnemies:
                CallEnemy(10, false);
                break;
        }
        if (UISE.Instance)
        {
            UISE.Instance.Play(UISE.SoundName.submit);
        }
    }

    void MessageEnemyNameAndLevel()
    {
        int id = StageManager.Instance.dungeonController.GetNextEnemyID();
        if (id >= 0)
        {
            int enemyLevelTemp = enemyLevel;
            if (StageManager.Instance.dungeonController.enemySettings.enemy.Length > 1)
            {
                enemyLevelTemp = StageManager.Instance.dungeonController.enemySettings.enemy[1].level;
            }
            MessageUI.Instance.SetMessageReplace(
                TextManager.Get("CELLIEN_NAME_" + id.ToString("d2"))
                + " "
                + TextManager.Get("ANALYZE_LEVEL")
                + " "
                + enemyLevelTemp,
                "ENEMY_NAME_AND_LEVEL");
        }
    }

    protected override void Update() {
        base.Update();
        if (!isCalledFrame && CharacterManager.Instance && state == 1)
        {
            switch (debugType)
            {
                case DebugType.InitializeAll:
                    UpdateChoices_InitializeAll();
                    break;
                case DebugType.SaveAllFriends:
                    UpdateChoices_SaveAllFriends();
                    break;
            }
        }
    }

    protected void UpdateChoices_InitializeAll()
    {
        switch (PauseController.Instance.ChoicesControl())
        {
            case -2:
                UISE.Instance.Play(UISE.SoundName.cancel);
                PauseController.Instance.CancelChoices();
                state = 0;
                break;
            case 3:
                UISE.Instance.Play(UISE.SoundName.delete);
                for (int i = 0; i < GameManager.Instance.save.friends.Length; i++)
                {
                    GameManager.Instance.save.friends[i] = 0;
                }
                for (int i = 3; i < GameManager.Instance.save.equip.Length; i++)
                {
                    GameManager.Instance.save.equip[i] = 0;
                }
                for (int i = 3; i < GameManager.Instance.save.weapon.Length; i++)
                {
                    GameManager.Instance.save.weapon[i] = 0;
                }
                for (int i = 0; i < GameManager.Instance.save.document.Length; i++)
                {
                    GameManager.Instance.save.document[i] = 0;
                }
                for (int i = 0; i < GameManager.Instance.save.hpUpNFS.Length; i++)
                {
                    GameManager.Instance.save.hpUpNFS[i] = 0;
                }
                for (int i = 0; i < GameManager.Instance.save.stUpNFS.Length; i++)
                {
                    GameManager.Instance.save.stUpNFS[i] = 0;
                }
                for (int i = 0; i < GameManager.Instance.save.inventoryNFS.Length; i++)
                {
                    GameManager.Instance.save.inventoryNFS[i] = 0;
                }
                for (int i = 0; i < GameManager.Instance.save.clearDifficulty.Length; i++)
                {
                    GameManager.Instance.save.clearDifficulty[i] = 0;
                }
                for (int i = 0; i < GameManager.Instance.save.defeatEnemy.Length; i++)
                {
                    GameManager.Instance.save.defeatEnemy[i] = 0;
                }
                for (int i = 0; i < GameManager.Instance.save.luckyBeast.Length; i++)
                {
                    GameManager.Instance.save.luckyBeast[i] = 0;
                }
                for (int i = 0; i < GameManager.Instance.save.reachedFloor.Length; i++)
                {
                    GameManager.Instance.save.reachedFloor[i] = 0;
                }
                for (int i = 0; i < GameManager.Instance.save.zakoRushClearTime.Length; i++)
                {
                    GameManager.Instance.save.zakoRushClearTime[i] = 0;
                }
                GameManager.Instance.save.progress = 0;
                GameManager.Instance.save.equipedHpUp = 0;
                GameManager.Instance.save.equipedStUp = 0;
                GameManager.Instance.save.equipedInvUp = 0;
                GameManager.Instance.save.hpUpSale = 0;
                GameManager.Instance.save.stUpSale = 0;
                GameManager.Instance.save.money = 0;
                GameManager.Instance.save.minmi = 0;
                GameManager.Instance.save.secret = 0;
                GameManager.Instance.save.sandstar = 0;
                GameManager.Instance.save.parkmanScore = 0;
                GameManager.Instance.save.parkmanScoreHard = 0;
                GameManager.Instance.save.tutorial = 0;
                GameManager.Instance.save.totalPlayTime = 0;
                GameManager.Instance.save.items.Clear();
                GameManager.Instance.save.storage.Clear();
                CharacterManager.Instance.AddExp(GameManager.Instance.save.exp * (-1), true);
                CharacterManager.Instance.UpdateSandstarMax();
                PauseController.Instance.CancelChoices();
                MessageUI.Instance.SetMessageReplace(TextManager.Get("MESSAGE_DBG_INITIALIZE"), "MESSAGE_DBG_INITIALIZE");
                state = 0;
                break;
            case 0:
            case 1:
            case 2:
            case 4:
            case 5:
            case 6:
                UISE.Instance.Play(UISE.SoundName.submit);
                PauseController.Instance.CancelChoices();
                state = 0;
                break;
        }
    }

    protected void UpdateChoices_SaveAllFriends()
    {
        switch (PauseController.Instance.ChoicesControl())
        {
            case -2:
                UISE.Instance.Play(UISE.SoundName.cancel);
                PauseController.Instance.CancelChoices();
                state = 0;
                break;
            case 0:
                for (int i = 0; i < GameManager.friendsMax; i++)
                {
                    GameManager.Instance.save.friends[i] = 1;
                }
                UISE.Instance.Play(UISE.SoundName.submit);
                PauseController.Instance.CancelChoices();
                MessageUI.Instance.SetMessageReplace(TextManager.Get("MESSAGE_DBG_CALLABLE_FRIENDS"), "MESSAGE_DBG_CALLABLE_FRIENDS");
                state = 0;
                break;
            case 1:
                for (int i = 0; i < GameManager.hpUpNFSMax; i++)
                {
                    GameManager.Instance.save.hpUpNFS[i] = 1;
                }
                for (int i = 0; i < GameManager.stUpNFSMax; i++)
                {
                    GameManager.Instance.save.stUpNFS[i] = 1;
                }
                for (int i = 0; i < GameManager.inventoryNFSMax; i++)
                {
                    GameManager.Instance.save.inventoryNFS[i] = 1;
                }
                UISE.Instance.Play(UISE.SoundName.submit);
                PauseController.Instance.CancelChoices();
                MessageUI.Instance.SetMessageReplace(TextManager.Get("MESSAGE_DBG_OTHER_FRIENDS"), "MESSAGE_DBG_OTHER_FRIENDS");
                state = 0;
                break;
            case 2:
                UISE.Instance.Play(UISE.SoundName.submit);
                PauseController.Instance.CancelChoices();
                state = 0;
                break;
        }
    }

}
