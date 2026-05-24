using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallMinmiCheckAllSave : MonoBehaviour {

    public int[] facilityID;
    public Transform[] minmiPivot;

    private void Start() {
        int minmiNum = 0;
        for (int i = 0; i < GameManager.saveSlotMax; i++) {
            GameManager.Save saveTemp = GameManager.Instance.TempLoad(i);
            minmiNum |= saveTemp.minmi;
        }
        if (minmiNum != 0) {
            for (int i = 0; i < minmiPivot.Length; i++) {
                if ((minmiNum & (1 << i)) != 0) {
                    // Instantiate(CharacterDatabase.Instance.GetFacility(facilityID[i]), minmiPivot[i].position, minmiPivot[i].rotation, minmiPivot[i]);
                    Instantiate(ItemDatabase.Instance.GetItemPrefab(ItemDatabase.facilityBottom + facilityID[i]), minmiPivot[i]);
                }
            }
        }
    }

}
