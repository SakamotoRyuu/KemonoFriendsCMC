using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateExceptHome : MonoBehaviour {

    public GameObject prefab;

    private void Awake() {
        if (prefab && StageManager.Instance && !StageManager.Instance.IsHomeStage) {
            Instantiate(prefab, transform);
        }
    }

}
