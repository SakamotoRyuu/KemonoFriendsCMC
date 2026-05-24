using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContainerBalloonController : MonoBehaviour {

    public GameObject balloonObj;
    public GameObject enemyObj;
    public GameObject goldObj;
    public GameObject itemObj;
    public Image itemImage;

    public void Activate(bool flag) {
        if (balloonObj.activeSelf != flag) {
            balloonObj.SetActive(flag);
        }
    }

    public void SetBalloon(int itemID) {
        if (itemID == -2) {
            enemyObj.SetActive(true);
        } else if (itemID == -1) {
            goldObj.SetActive(true);
        } else if (itemID >= 0) {
            itemImage.sprite = ItemDatabase.Instance.GetItemImage(itemID);
            itemObj.gameObject.SetActive(true);
        }
    }
}
