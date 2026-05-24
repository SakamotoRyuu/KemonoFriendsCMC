using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationWingVariation : AnimationWing {

    public Vector3[] rotationArray;
    private int nowIndex = -1;

    public void ChangeRotation(int index) {
        if (nowIndex != index && index >= 0 && index < rotationArray.Length) {
            nowIndex = index;
            targetRotation = rotationArray[nowIndex];
        }
    }

}
