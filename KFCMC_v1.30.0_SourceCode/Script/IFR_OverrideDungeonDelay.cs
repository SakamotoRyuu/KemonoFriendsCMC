using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IFR_OverrideDungeonDelay : MonoBehaviour {
    
    [System.Serializable]
    public class StageDelaySet {
        public int stageNumber = 0;
        public float dungeonDelay = 1f;
    }

    public StageDelaySet[] stageDelaySet;

	void Awake () {
        ItemCharacter itemCharacter = GetComponent<ItemCharacter>();
        if (StageManager.Instance && itemCharacter) {
            for (int i = 0; i < stageDelaySet.Length; i++) {
                if (stageDelaySet[i].stageNumber == StageManager.Instance.stageNumber) {
                    itemCharacter.dungeonDelay = stageDelaySet[i].dungeonDelay;
                    break;
                }
            }
        }
	}
}
