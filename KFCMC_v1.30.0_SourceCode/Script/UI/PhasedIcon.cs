using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhasedIcon : MonoBehaviour {

    public Canvas canvas;
    public Image frameImage;
    public Image fillImage;
    public Image maxImage;
    private float amount;
    public float FillAmount {
        get { return amount; }
        set {
            amount = value;
            fillImage.fillAmount = amount;
            if (amount < 1 && maxImage.enabled) {
                maxImage.enabled = false;
            }
            if (amount >= 1 && !maxImage.enabled) {
                maxImage.enabled = true;
            }
        }
    }    
    
    public void SetCanvas(bool flag) {
        canvas.enabled = flag;
    }

    private void Awake() {
        FillAmount = 0;
    }

}
