using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateSearchEnemyRadius : MonoBehaviour {
    
    CapsuleCollider capCol;
    float radiusSave = -1;
    
	void Start () {
        capCol = GetComponent<CapsuleCollider>();
	}
	
	void Update () {
        if (CharacterManager.Instance) {
            float radiusTemp;
            if (CharacterManager.Instance.isBossBattle) {
                radiusTemp = (GameManager.Instance.save.config[GameManager.Save.configID_SearchRadiusBoss] * 2 + 10);
            } else {
                radiusTemp = (GameManager.Instance.save.config[GameManager.Save.configID_SearchRadius] * 2 + 10);
            }
            if (radiusTemp != radiusSave) {
                radiusSave = radiusTemp;
                capCol.radius = radiusTemp;
                capCol.height = radiusTemp * 2f;
            }
        }
	}
}
