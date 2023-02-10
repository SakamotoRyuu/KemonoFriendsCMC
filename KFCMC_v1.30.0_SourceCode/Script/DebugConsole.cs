using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugConsole : MonoBehaviour {

    public enum DebugType {
        LevelDown, LevelUp, Call10Enemies, ChangeEnemiesLevel, Gold, ProgressUp, ProgressDown, Inventory, SaveAllLB, Call1Enemy, EnemyIdPlus, EnemyIdMinus, InitializeAll, SaveAllFriends, CallVariousEnemies
    }

    public DebugType debugType;
    
    bool entered = false;
    int enemyLevel = 1;
    int pauseWait;
    int state;
    bool actionTextEnabled;
    const string targetTag = "ItemGetter";    

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

    private void Update() {
        if (PauseController.Instance && PauseController.Instance.pauseGame) {
            pauseWait = 2;
        } else if (pauseWait > 0) {
            pauseWait--;
        }
        if (CharacterManager.Instance) {
            if (state == 0) {
                if (entered && Time.timeScale > 0f && pauseWait <= 0) {
                    if (GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit)) {
                        switch (debugType) {
                            case DebugType.LevelDown:
                                CharacterManager.Instance.AddExp((GameManager.Instance.save.NowLevelExp + 1) * (-1), true);
                                break;
                            case DebugType.LevelUp:
                                CharacterManager.Instance.AddExp(GameManager.Instance.save.NeedExpToNextLevel, true);
                                break;
                            case DebugType.Call10Enemies:
                                CallEnemy(10, true);
                                break;
                            case DebugType.ChangeEnemiesLevel:
                                enemyLevel = (enemyLevel + 1) % 5;
                                if (StageManager.Instance && StageManager.Instance.dungeonController) {
                                    for (int i = 0; i < StageManager.Instance.dungeonController.enemySettings.enemy.Length; i++) {
                                        StageManager.Instance.dungeonController.enemySettings.enemy[i].level = enemyLevel;
                                    }
                                }
                                break;
                            case DebugType.Gold:
                                GameManager.Instance.save.money += 10000;
                                break;
                            case DebugType.ProgressUp:
                                if (GameManager.Instance.save.progress < 12) {
                                    GameManager.Instance.save.progress += 1;
                                }
                                break;
                            case DebugType.ProgressDown:
                                if (GameManager.Instance.save.progress > 0) {
                                    GameManager.Instance.save.progress -= 1;
                                }
                                break;
                            case DebugType.Inventory:
                                for (int i = 0; i < GameManager.inventoryNFSMax; i++) {
                                    GameManager.Instance.save.inventoryNFS[i] = 1;
                                    GameManager.Instance.save.weapon[GameManager.invUpId - 200] = 1;
                                    GameManager.Instance.save.equip[GameManager.invUpId - 200] = 1;
                                }
                                break;
                            case DebugType.SaveAllLB:
                                for (int i = 0; i < GameManager.Instance.save.luckyBeast.Length; i++) {
                                    GameManager.Instance.save.luckyBeast[i] = 1;
                                }
                                break;
                            case DebugType.Call1Enemy:
                                CallEnemy(1, true);
                                break;
                            case DebugType.EnemyIdPlus:
                                if (StageManager.Instance.dungeonController) {
                                    StageManager.Instance.dungeonController.PlusEnemyPointer(1);
                                }
                                break;
                            case DebugType.EnemyIdMinus:
                                if (StageManager.Instance.dungeonController) {
                                    StageManager.Instance.dungeonController.PlusEnemyPointer(-1);
                                }
                                break;
                            case DebugType.InitializeAll:
                                if (PauseController.Instance.GetPauseEnabled()) {
                                    PauseController.Instance.SetChoices(7, true, TextManager.Get("WORD_INITIALIZEALL"), "CHOICE_CANCEL", "CHOICE_CANCEL", "CHOICE_CANCEL", "CHOICE_EXECUTE", "CHOICE_CANCEL", "CHOICE_CANCEL", "CHOICE_CANCEL");
                                    state = 1;
                                }
                                break;
                            case DebugType.SaveAllFriends:
                                for (int i = 0; i < GameManager.friendsMax; i++) {
                                    GameManager.Instance.save.friends[i] = 1;
                                }
                                for (int i = 0; i < GameManager.hpUpNFSMax; i++) {
                                    GameManager.Instance.save.hpUpNFS[i] = 1;
                                }
                                for (int i = 0; i < GameManager.stUpNFSMax; i++) {
                                    GameManager.Instance.save.stUpNFS[i] = 1;
                                }
                                for (int i = 0; i < GameManager.inventoryNFSMax; i++) {
                                    GameManager.Instance.save.inventoryNFS[i] = 1;
                                }
                                break;
                            case DebugType.CallVariousEnemies:
                                CallEnemy(10, false);
                                break;
                        }
                        if (UISE.Instance) {
                            UISE.Instance.Play(UISE.SoundName.submit);
                        }
                    }
                }
            } else {
                switch (PauseController.Instance.ChoicesControl()) {
                    case -2:
                        UISE.Instance.Play(UISE.SoundName.cancel);
                        PauseController.Instance.CancelChoices();
                        state = 0;
                        break;
                    case 3:
                        UISE.Instance.Play(UISE.SoundName.delete);
                        for (int i = 0; i < GameManager.Instance.save.friends.Length; i++) {
                            GameManager.Instance.save.friends[i] = 0;
                        }
                        for (int i = 3; i < GameManager.Instance.save.equip.Length; i++) {
                            GameManager.Instance.save.equip[i] = 0;
                        }
                        for (int i = 3; i < GameManager.Instance.save.weapon.Length; i++) {
                            GameManager.Instance.save.weapon[i] = 0;
                        }
                        for (int i = 0; i < GameManager.Instance.save.document.Length; i++) {
                            GameManager.Instance.save.document[i] = 0;
                        }
                        for (int i = 0; i < GameManager.Instance.save.hpUpNFS.Length; i++) {
                            GameManager.Instance.save.hpUpNFS[i] = 0;
                        }
                        for (int i = 0; i < GameManager.Instance.save.stUpNFS.Length; i++) {
                            GameManager.Instance.save.stUpNFS[i] = 0;
                        }
                        for (int i = 0; i < GameManager.Instance.save.inventoryNFS.Length; i++) {
                            GameManager.Instance.save.inventoryNFS[i] = 0;
                        }
                        for (int i = 0; i < GameManager.Instance.save.clearDifficulty.Length; i++) {
                            GameManager.Instance.save.clearDifficulty[i] = 0;
                        }
                        for (int i = 0; i < GameManager.Instance.save.defeatEnemy.Length; i++) {
                            GameManager.Instance.save.defeatEnemy[i] = 0;
                        }
                        for (int i = 0; i < GameManager.Instance.save.luckyBeast.Length; i++) {
                            GameManager.Instance.save.luckyBeast[i] = 0;
                        }
                        for (int i = 0; i < GameManager.Instance.save.reachedFloor.Length; i++) {
                            GameManager.Instance.save.reachedFloor[i] = 0;
                        }
                        for (int i = 0; i < GameManager.Instance.save.zakoRushClearTime.Length; i++) {
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
            bool actionTemp = (state == 0 && entered && pauseWait <= 0 && PauseController.Instance.pauseEnabled);
            if (actionTextEnabled != actionTemp) {
                CharacterManager.Instance.SetActionType(actionTemp ? CharacterManager.ActionType.Search : CharacterManager.ActionType.None, gameObject);
                actionTextEnabled = actionTemp;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(targetTag)) {
            entered = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag(targetTag)) {
            entered = false;
        }
    }

}
