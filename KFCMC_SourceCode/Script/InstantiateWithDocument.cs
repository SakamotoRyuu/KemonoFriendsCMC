using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateWithDocument : MonoBehaviour {

    public int documentId;
    public int itemId;

	void Start () {
		if (GameManager.Instance.save.document[documentId] != 0) {
            Instantiate(ItemDatabase.Instance.GetItemPrefab(itemId), transform.position, transform.rotation, transform);
        }
	}
}
