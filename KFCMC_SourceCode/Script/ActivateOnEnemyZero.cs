using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOnEnemyZero : MonoBehaviour {

    public GameObject target;
    public bool invert;

    private bool activated;
    
	void Awake () {
		if (target) {
            target.SetActive(invert);
        }
        activated = false;
	}
	
	void Update () {
		if (!activated && target && StageManager.Instance && StageManager.Instance.dungeonController) {
            int enemyCount = StageManager.Instance.dungeonController.EnemyCount();
            if (enemyCount <= 0) {
                target.SetActive(!invert);
                activated = true;
            }
        }
	}
}
