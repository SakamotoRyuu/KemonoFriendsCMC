using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqueItemActivate : MonoBehaviour {

    private void Awake() {
        GetItem getItem = GetComponent<GetItem>();
        if (getItem) {
            int id = getItem.id;
            if (GameManager.Instance.save.NumOfSpecificItems(id) > 0 || GameManager.Instance.save.NumOfSpecificItemsInStorage(id) > 0) {
                gameObject.SetActive(false);
            }
        }
    }

}
