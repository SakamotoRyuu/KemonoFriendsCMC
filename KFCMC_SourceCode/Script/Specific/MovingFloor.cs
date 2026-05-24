using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingFloor : MonoBehaviour {

    public Rigidbody floorRigidbody;
    public bool rotationEnabled;
    public float heightOffset;
    public float centripetalOffset;
    public bool centripetalIgnoreY;
    public Transform centerPivot;
    public MovingFloor proxyMovingFloor;

    private void OnTriggerEnter(Collider other) {
        if (floorRigidbody && other.CompareTag("Player")) {
            FriendsBase fBase = other.GetComponent<FriendsBase>();
            if (fBase) {
                fBase.SetMovingFloor(this);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (floorRigidbody && other.CompareTag("Player")) {
            FriendsBase fBase = other.GetComponent<FriendsBase>();
            if (fBase) {
                if (proxyMovingFloor) {
                    fBase.SetMovingFloor(proxyMovingFloor);
                } else {
                    fBase.RemoveMovingFloor(this);
                }
            }
        }
    }

}
