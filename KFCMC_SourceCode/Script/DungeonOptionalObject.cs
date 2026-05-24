using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonOptionalObject : MonoBehaviour {

    public GameObject prefab;
    GameObject instance;
    
	void Start () {
		if (GameManager.Instance.save.config[GameManager.Save.configID_GenerateDungeonObjects] != 0) {
            instance = Instantiate(prefab, transform.position, transform.rotation, transform);
        }
	}

    private void Update() {
        if (GameManager.Instance.save.config[GameManager.Save.configID_GenerateDungeonObjects] != 0) {
            if (instance) {
                if (instance.activeSelf == false) {
                    instance.SetActive(true);
                }
            } else {
                instance = Instantiate(prefab, transform.position, transform.rotation, transform);
            }
        } else {
            if (instance && instance.activeSelf == true) {
                instance.SetActive(false);
            }
        }
    }
}
