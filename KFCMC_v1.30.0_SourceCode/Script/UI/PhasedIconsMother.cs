using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhasedIconsMother : MonoBehaviour {

    public PhasedIcon[] pIcon;
    public Sprite[] fillSprite;
    public Sprite[] fillMaxSprite;
    
    private float amount;
    private int amountMax;
    private GridLayoutGroup grid;
    private const int spacingUnit = -2;
    private const int spacingStart = 6;

    private int nowSpriteType;

    public float Amount {
        get { return amount; }
        set {
            amount = value;
            for (int i = 0; i < pIcon.Length; i++) {
                pIcon[i].FillAmount = amount > i ? amount - i : 0;
            }
        }
    }
    public int AmountMax {
        get { return amountMax; }
        set {
            amountMax = value;
            for (int i = 0; i < pIcon.Length; i++) {
                pIcon[i].SetCanvas(i < amountMax);
            }
            grid.spacing = (amountMax >= spacingStart ? new Vector2(spacingUnit * (amountMax - spacingStart + 1), 0) : Vector2.zero);
        }
    }

    private void Awake() {
        grid = GetComponent<GridLayoutGroup>();
    }

    public void SetSpriteType(int newType) {
        if (nowSpriteType != newType && newType >= 0 && newType < fillSprite.Length) {
            nowSpriteType = newType;
            for (int i = 0; i < pIcon.Length; i++) {
                pIcon[i].fillImage.sprite = fillSprite[newType];
                pIcon[i].maxImage.sprite = fillMaxSprite[newType];
            }
        }
    }

}
