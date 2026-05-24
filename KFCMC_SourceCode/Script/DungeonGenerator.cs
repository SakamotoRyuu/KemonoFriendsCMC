/// 作者 賀好 昭仁 様
/// https://mynavi-agent.jp/it/geekroid/2017/02/unity52d.html

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour {
    /*
    0~255:wall
    100000~:room
    200000~:passage
    */

    [System.Serializable]
    public class RoomInfo {
        public Vector2Int origin;
        public Vector2Int size;
        public RoomInfo() {
            origin = new Vector2Int(-1, -1);
            size = new Vector2Int(-1, -1);
        }
    }

    [Range(4, 100)]
    public int width;
    [Range(4, 100)]
    public int height;
    public int level;
    public bool isMonsterHouse;
    public bool isComplex;
    public int[] specifiedRooms;

    protected int[,] map;
    protected int roomCount = 0;
    protected int passageCount = 0;
    protected List<RoomInfo> roomInfo = new List<RoomInfo>();
    public const int wallBase = 0;
    public const int roomBase = 100000;
    public const int passageBase = 200000;
    public const int denomi = 1000;

    [System.NonSerialized]
    public bool passageToWall;
    [System.NonSerialized]
    public bool goalAvoidEnable;

    protected virtual void Awake() {
        goalAvoidEnable = true;
    }

    public virtual int[,] GetMap() {
        return map;
    }

    public virtual void SetMapPoint(int x, int y, int num) {
        if (x >= 0 && x < width && y >= 0 && y < height) {
            map[x, y] = num;
        }
    }

    public virtual int ResetWallModifyPoint(int x, int y) {
        if (x >= 0 && x < width && y >= 0 && y < height) {
            int wallFlag = GetAroundWall(x, y);
            map[x, y] = map[x, y] / denomi * denomi + wallFlag;
            return map[x, y];
        }
        return -1;
    }

    public virtual int GetRoomCount() {
        return roomCount;
    }

    public virtual int GetPassageCount() {
        return passageCount;
    }

    public virtual RoomInfo GetRoomInfo(int index) {
        if (index >= 0 && index < roomInfo.Count) {
            return roomInfo[index];
        } else {
            return new RoomInfo();
        }
    }

    public virtual void Generate() {
        map = new int[width, height];
    }

    public virtual int GetAroundWall(int x, int y) {
        int wallFlag = 0;
        if (y <= 0 || map[x, y - 1] < roomBase || (passageToWall && map[x, y - 1] >= passageBase)) {
            wallFlag += 1;
        }
        if (x <= 0 || map[x - 1, y] < roomBase ||(passageToWall && map[x - 1, y] >= passageBase)) {
            wallFlag += 2;
        }
        if (x >= width - 1 || map[x + 1, y] < roomBase || (passageToWall && map[x + 1, y] >= passageBase)) {
            wallFlag += 4;
        }
        if (y >= height - 1 || map[x, y + 1] < roomBase || (passageToWall && map[x, y + 1] >= passageBase)) {
            wallFlag += 8;
        }
        if (x <= 0 || y <= 0 || map[x - 1, y - 1] < roomBase || (passageToWall && map[x - 1, y - 1] >= passageBase)) {
            wallFlag += 16;
        }
        if (x >= width - 1 || y <= 0 || map[x + 1, y - 1] < roomBase || (passageToWall && map[x + 1, y - 1] >= passageBase)) {
            wallFlag += 32;
        }
        if (x <= 0 || y >= height - 1 || map[x - 1, y + 1] < roomBase || (passageToWall && map[x - 1, y + 1] >= passageBase)) {
            wallFlag += 64;
        }
        if (x >= width - 1 || y >= height - 1 || map[x + 1, y + 1] < roomBase || (passageToWall && map[x + 1, y + 1] >= passageBase)) {
            wallFlag += 128;
        }
        return wallFlag;
    }

    public virtual void ModifyWalls() {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                ResetWallModifyPoint(i, j);
            }
        }
    }

}

