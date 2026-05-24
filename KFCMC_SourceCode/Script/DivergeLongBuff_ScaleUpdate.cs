using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivergeLongBuff_ScaleUpdate : MonoBehaviour {

    public Vector3 longScale;

    Transform trans;
    Vector3 defaultScale;
    bool isLong = false;

    private void Awake() {
        trans = transform;
        defaultScale = trans.localScale;
    }

    private void Update() {
        bool longAnswer = (CharacterManager.Instance && CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Long));
        if (isLong != longAnswer) {
            isLong = longAnswer;
            if (isLong) {
                trans.localScale = longScale;
            } else {
                trans.localScale = defaultScale;
            }
        }
    }

}
