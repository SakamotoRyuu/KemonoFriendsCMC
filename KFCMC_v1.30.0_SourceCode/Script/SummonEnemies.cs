using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonEnemies : MonoBehaviour {

    public float radius = 20;
    public int count = 10;
    public bool byBlackCrystal;

	// Use this for initialization
	void Start () {
        if (StageManager.Instance != null && StageManager.Instance.dungeonController != null) {
            StageManager.Instance.dungeonController.SummonEnemies(count, -1, radius, byBlackCrystal);
        }
    }
	
}
