using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DGRandomizer : MonoBehaviour {

    public int mapWidthRandom = 6;
    public int roomMinWidthRandom = 2;
    public int roomMinHeightRandom = 2;
    public int maxRoomsRandom = 2;
    public int bigRoomRateRandom = 20;
    public int passageSurplusRateRandom = 20;

    private void Awake() {
        DungeonGenerator_Standard dg = GetComponent<DungeonGenerator_Standard>();
        if (dg != null) {
            int widthTemp = Random.Range(-mapWidthRandom, mapWidthRandom + 1);
            dg.width += widthTemp;
            dg.height -= widthTemp;
            dg.roomSettings.minWidth += Random.Range(-roomMinWidthRandom, roomMinWidthRandom + 1);
            if (dg.roomSettings.minWidth < 2) {
                dg.roomSettings.minWidth = 2;
            } else if (dg.roomSettings.minWidth > 10) {
                dg.roomSettings.minWidth = 10;
            }
            dg.roomSettings.minHeight += Random.Range(-roomMinHeightRandom, roomMinHeightRandom + 1);
            if (dg.roomSettings.minHeight < 2) {
                dg.roomSettings.minHeight = 2;
            } else if (dg.roomSettings.minHeight > 10) {
                dg.roomSettings.minHeight = 10;
            }
            dg.roomSettings.maxRooms += Random.Range(-maxRoomsRandom, maxRoomsRandom + 1);
            if (dg.roomSettings.maxRooms < dg.roomSettings.minRooms) {
                dg.roomSettings.maxRooms = dg.roomSettings.minRooms;
            } else if (dg.roomSettings.maxRooms < 1) {
                dg.roomSettings.maxRooms = 1;
            } else if (dg.roomSettings.maxRooms > 20) {
                dg.roomSettings.maxRooms = 20;
            }
            dg.roomSettings.bigRoomRate += Random.Range(-bigRoomRateRandom, bigRoomRateRandom + 1);
            if (dg.roomSettings.bigRoomRate < 0) {
                dg.roomSettings.bigRoomRate = 0;
            } else if (dg.roomSettings.bigRoomRate > 100) {
                dg.roomSettings.bigRoomRate = 100;
            }
            dg.roomSettings.passageSurplusRate += Random.Range(-passageSurplusRateRandom, passageSurplusRateRandom + 1);
            if (dg.roomSettings.passageSurplusRate < 0) {
                dg.roomSettings.passageSurplusRate = 0;
            } else if (dg.roomSettings.passageSurplusRate > 100) {
                dg.roomSettings.passageSurplusRate = 100;
            }
        }
    }
}
