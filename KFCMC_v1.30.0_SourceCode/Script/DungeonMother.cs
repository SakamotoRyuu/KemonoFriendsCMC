using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMother : MonoBehaviour {
    
    [System.Serializable]
    public class ReaperSettings {
        public int enemyID = 50;
        public int appearMusicNumber = 10;
        public int appearMusicNumberAnother;
    }

    [System.Serializable]
    public class ShortcutSettings {
        public GameObject shortcutStartPrefab;
        public int startFloor = 0;
        public int shortcutFloor = 10;
        public int endFloor = 9;
    }

    public int bonusFloorNum = -1;
    public int wrSpeedLevel;
    public ReaperSettings reaperSettings;
    public ShortcutSettings[] shortcutSettings;
    public GameObject[] dungeonPrefab;

    [System.NonSerialized]
    public int floorNum;
    [System.NonSerialized]
    public bool[] disadvantageProtectMessageShowed = new bool[disadvantageTypeMax];
    
    const int gameoverTutorialIndex = 2;
    const int disadvantageTypeMax = 5;
    bool movedOnceFlag;

    public void MoveFloor(int nextFloor = 1, int saveSlot = -1, bool additive = false) {
        if (StageManager.Instance != null) {
            StageManager.Instance.floorMax = dungeonPrefab.Length;
            StageManager.Instance.bonusFloorNum = bonusFloorNum;
            StageManager.Instance.mapActivateFlag = 0;
            CharacterManager.Instance.ResetForMoveFloor();
            if (CameraManager.Instance) {
                CameraManager.Instance.smallRate = 0f;
                CameraManager.Instance.SetYAxisRate();
            }
            if (additive) {
                floorNum += nextFloor;
            } else { 
                floorNum = nextFloor;
            }
            StageManager.Instance.DestroyDungeonController();
            if (floorNum < dungeonPrefab.Length && dungeonPrefab[floorNum] != null) {
                StageManager.Instance.floorNumber = floorNum;
                StageManager.Instance.SetReachedFloor();
                StageManager.Instance.GenerateDungeon(dungeonPrefab[floorNum], floorNum);
                GameManager.Instance.save.nowStage = StageManager.Instance.stageNumber;
                GameManager.Instance.save.nowFloor = floorNum;
                if (saveSlot >= 0 && saveSlot < GameManager.saveSlotMax) {
                    CharacterManager.Instance.WriteFriendsLiving();
                    GameManager.Instance.DataSave(saveSlot);
                    if (MessageUI.Instance != null) {
                        MessageUI.Instance.SetMessage(TextManager.Get("MESSAGE_SAVED"));
                    }
                }
                if (CameraManager.Instance) {
                    CameraManager.Instance.ResetCameraFixPos();
                }
            } else {
                StageManager.Instance.MoveStage(0);
            }
            if (GameManager.Instance.gameOverFlag) {
                GameManager.Instance.gameOverCount++;
            } else {
                GameManager.Instance.gameOverCount = 0;
            }
            if (!movedOnceFlag) {
                movedOnceFlag = true;
                if (GameManager.Instance.save.moveByBus != 0 && StageManager.Instance.dungeonController && StageManager.Instance.dungeonController.disadvantageType != DungeonController.DisadvantageType.None) {
                    disadvantageProtectMessageShowed[(int)StageManager.Instance.dungeonController.disadvantageType] = true;
                }
            }
            GameManager.Instance.gameOverFlag = false;
            GameManager.Instance.save.SetRunInBackground();
        } 
    }
}
