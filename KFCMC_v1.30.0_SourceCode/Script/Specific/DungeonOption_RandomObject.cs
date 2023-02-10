using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonOption_RandomObject : MonoBehaviour {

    [System.Serializable]
    public struct RandomObjectSet {
        public GameObject prefab;
        public float density;
        public float randomPosRadius;
        public int conditionQualityLevel;
    }

    public RandomObjectSet[] randomObjectSet;

    void Start() {
        if (GameManager.Instance.save.config[GameManager.Save.configID_GenerateDungeonObjects] != 0) {
            DungeonController dungeonController = GetComponent<DungeonController>();
            if (dungeonController) {
                for (int i = 0; i < randomObjectSet.Length; i++) {
                    if (randomObjectSet[i].prefab && (randomObjectSet[i].conditionQualityLevel <= 0 || QualitySettings.GetQualityLevel() >= randomObjectSet[i].conditionQualityLevel)) {
                        dungeonController.GenerateObjectOnWalkablePoint(randomObjectSet[i].prefab, randomObjectSet[i].density, randomObjectSet[i].randomPosRadius);
                    }
                }
            }
        }
    }
}
