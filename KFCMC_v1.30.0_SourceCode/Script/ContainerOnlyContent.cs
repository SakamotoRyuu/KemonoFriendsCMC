using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerOnlyContent : MonoBehaviour {

    public int rank;

	// Use this for initialization
	void Start () {
        bool isParenting = (StageManager.Instance && StageManager.Instance.dungeonController && StageManager.Instance.dungeonController.itemSettings.container);
        int replaceLevel = (StageManager.Instance && StageManager.Instance.dungeonController ? StageManager.Instance.dungeonController.itemReplaceLevel : 0);
        int itemID = ContainerDatabase.Instance.GetIDSingle(rank);
        ItemDatabase.Instance.GiveItem(itemID, transform.position, 5, -1, -1, -1, isParenting ? StageManager.Instance.dungeonController.itemSettings.container : null, replaceLevel);
        if (CharacterManager.Instance) {
            CharacterManager.Instance.CheckJaparimanShortage(itemID);
        }
        Destroy(gameObject);
    }
}