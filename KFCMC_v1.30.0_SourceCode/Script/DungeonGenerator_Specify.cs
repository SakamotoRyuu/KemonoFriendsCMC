using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonGenerator_Specify : DungeonGenerator {

    [System.Serializable]
    public class Room {
        public int xStart;
        public int xRand = 2;
        public int yStart;
        public int yRand = 2;
        public int widthStart = 6;
        public int widthRand = 2;
        public int heightStart = 6;
        public int heightRand = 2;
        [System.NonSerialized]
        public int x, y, width, height;
    }

    [System.Serializable]
    public class Passage {
        public int roomA;
        public int roomB;
    }

    [SerializeField]
    public Room[] rooms;
    [SerializeField]
    public Passage[] passages;

    public override void Generate() {
        base.Generate();
        SpecifyGenerate();
        ModifyWalls();
    }

    public virtual void SpecifyGenerate() {
        RoomRandomize();

        passageCount = 0;
        for (int i = 0; i < passages.Length; i++) {
            map = PassageWriteToMap(map, i, passageBase + i * denomi);
            passageCount++;
        }

        roomCount = 0;
        for (int i = 0; i < rooms.Length; i++) {
            int writeNum;
            if (rooms[i].width > 1 && rooms[i].height > 1) {
                writeNum = roomBase + i * denomi;
                RoomInfo infoTemp = new RoomInfo();
                infoTemp.origin = new Vector2Int(rooms[i].x, rooms[i].y);
                infoTemp.size = new Vector2Int(rooms[i].width, rooms[i].height);
                roomInfo.Add(infoTemp);
                roomCount++;
            } else {
                writeNum = passageBase + i * denomi;
            }
            map = RoomWriteToMap(map, i, writeNum);
        }
    }

    public virtual void RoomRandomize() {
        for (int i = 0; i < rooms.Length; i++) {
            rooms[i].x = rooms[i].xStart + Random.Range(0, rooms[i].xRand + 1);
            rooms[i].y = rooms[i].yStart + Random.Range(0, rooms[i].yRand + 1);
            rooms[i].width = rooms[i].widthStart + Random.Range(0, rooms[i].widthRand + 1);
            rooms[i].height = rooms[i].heightStart + Random.Range(0, rooms[i].heightRand + 1);
            if (rooms[i].x < 1) {
                rooms[i].x = 1;
            } else if (rooms[i].x >= width - 2) {
                rooms[i].x = width - 3;
            }
            if (rooms[i].y < 1) {
                rooms[i].y = 1;
            } else if (rooms[i].y >= height - 2) {
                rooms[i].y = height - 3;
            }
            if (rooms[i].x + rooms[i].width >= width - 1) {
                rooms[i].width = width - 2 - rooms[i].x;
            }
            if (rooms[i].y + rooms[i].height >= height - 1) {
                rooms[i].height = height - 2 - rooms[i].y;
            }
        }
    }

    public virtual int[,] PassageWriteToMap(int[,] map, int index, int num = 0) {
        int turnCount = 0;
        int dirSave = -1;
        var fromX = Random.Range(rooms[passages[index].roomA].x, rooms[passages[index].roomA].x + rooms[passages[index].roomA].width);
        var fromY = Random.Range(rooms[passages[index].roomA].y, rooms[passages[index].roomA].y + rooms[passages[index].roomA].height);
        var toX = Random.Range(rooms[passages[index].roomB].x, rooms[passages[index].roomB].x + rooms[passages[index].roomB].width);
        var toY = Random.Range(rooms[passages[index].roomB].y, rooms[passages[index].roomB].y + rooms[passages[index].roomB].height);
        while (fromX != toX || fromY != toY) {
            map[fromX, fromY] = num;
            int direction = Random.Range(0, 2);
            turnCount--;
            if (turnCount > 0) {
                direction = dirSave;
            } else if (direction != dirSave) {
                dirSave = direction;
                turnCount = 2;
            }
            if (fromX != toX && fromY != toY && direction == 0 || fromY == toY) {
                fromX += (toX - fromX) > 0 ? 1 : -1;
            } else {
                fromY += (toY - fromY) > 0 ? 1 : -1;
            }
        }
        return map;
    }

    public virtual int[,] RoomWriteToMap(int[,] map, int index, int num = 0) {
        for (int dx = rooms[index].x; dx < rooms[index].x + rooms[index].width; dx++) {
            for (int dy = rooms[index].y; dy < rooms[index].y + rooms[index].height; dy++) {
                map[dx, dy] = num;
            }
        }
        return map;
    }
}