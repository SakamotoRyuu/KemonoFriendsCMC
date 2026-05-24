using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DungeonGenerator_ReadFile : DungeonGenerator {

    public string filePath;
    public int[,] mapMod;

    public override void Generate() {
        base.Generate();
        mapMod = new int[width, height];
        TextAsset csv = Resources.Load<TextAsset>(filePath);
        StringReader reader = new StringReader(csv.text);
        int y = 0;
        while (reader.Peek() > -1 && y < height) {
            string[] values = reader.ReadLine().Split('\t');
            for (int x = 0; x < values.Length && x < width; x++) {
                int tileType = int.Parse(values[x]);
                int tileMod = 0;
                if (tileType >= 10) {
                    tileMod = tileType % 10;
                    tileType /= 10;
                }
                switch (tileType) {
                    case 0:
                        map[x, y] = wallBase;
                        break;
                    case 1:
                        map[x, y] = roomBase;
                        break;
                    case 2:
                        map[x, y] = passageBase;
                        break;
                    default:
                        break;
                }
                mapMod[x, y] = tileMod;
            }
            y++;
        }
        ModifyWalls();
    }

}
