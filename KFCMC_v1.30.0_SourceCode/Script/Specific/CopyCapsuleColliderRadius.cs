using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyCapsuleColliderRadius : MonoBehaviour {

    public CapsuleCollider copyFrom;
    public CapsuleCollider copyTo;
    public bool checkCenter;
    public bool checkHeight;
    public bool checkDirection;
    public bool checkEnabled;

    private void Update() {
        if (copyFrom && copyTo) {
            if (copyTo.radius != copyFrom.radius) {
                copyTo.radius = copyFrom.radius;
            }
            if (checkCenter) {
                if (copyTo.center != copyFrom.center) {
                    copyTo.center = copyFrom.center;
                }
            }
            if (checkHeight) {
                if (copyTo.height != copyFrom.height) {
                    copyTo.height = copyFrom.height;
                }
            }
            if (checkDirection) {
                if (copyTo.direction != copyFrom.direction) {
                    copyTo.direction = copyFrom.direction;
                }
            }
            if (checkEnabled) {
                if (copyTo.enabled != copyFrom.enabled) {
                    copyTo.enabled = copyFrom.enabled;
                }
            }
        }
    }

}
