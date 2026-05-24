using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ending_Credit : MonoBehaviour
{

    [System.Serializable]
    public struct CreditSet {
        public string captionKey;
        public string contentKey;
    }

    public CreditSet[] creditSet;
    public RectTransform parentPanel;
    public GameObject captionPrefab;
    public GameObject gridPanelPrefab;
    public GameObject textPrefab;
    public int gridColumns = 2;
    public float rowHeight = 30;

    readonly string[] separator = { "\n" };

    private void Start() {
        if (TextManager.IsInitialized) {
            float yPos = 0f;
            for (int i = 0; i < creditSet.Length; i++) {
                if (!string.IsNullOrEmpty(creditSet[i].captionKey)) {
                    GameObject captionInstance = Instantiate(captionPrefab, parentPanel);
                    captionInstance.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, yPos);
                    captionInstance.GetComponent<TMP_Text>().text = TextManager.Get(creditSet[i].captionKey);
                    yPos -= rowHeight;
                }
                if (!string.IsNullOrEmpty(creditSet[i].contentKey)) {
                    string[] contentArray = TextManager.Get(creditSet[i].contentKey).Split(separator, System.StringSplitOptions.None);
                    if (contentArray.Length == 1) {
                        GameObject contentInstance = Instantiate(textPrefab, parentPanel);
                        contentInstance.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, yPos);
                        contentInstance.GetComponent<TMP_Text>().text = contentArray[0];
                        yPos -= rowHeight;
                    } else {
                        GameObject gridPanelInstance = Instantiate(gridPanelPrefab, parentPanel);
                        gridPanelInstance.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, yPos);
                        Transform gridPanelTransform = gridPanelInstance.transform;
                        for (int j = 0; j < contentArray.Length; j++) {
                            GameObject contentInstance = Instantiate(textPrefab, gridPanelTransform);
                            contentInstance.GetComponent<TMP_Text>().text = contentArray[j];
                            if (gridColumns < 2 || j % gridColumns == gridColumns - 1 || j == contentArray.Length - 1) {
                                yPos -= rowHeight;
                            }
                        }
                    }
                }
                if (i < creditSet.Length - 1) {
                    yPos -= rowHeight;
                }
            }
            parentPanel.sizeDelta = new Vector2(parentPanel.sizeDelta.x, yPos * -1);
        }
    }

}
