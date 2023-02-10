using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator_Fill : DungeonGenerator {
    
    public int fillNumber = 100000;

    public override void Generate() {
        base.Generate();
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                map[x, y] = fillNumber;
            }
        }
    }

}
