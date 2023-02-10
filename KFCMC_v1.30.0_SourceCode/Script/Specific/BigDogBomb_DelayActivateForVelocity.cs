using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigDogBomb_DelayActivateForVelocity : MonoBehaviour
{

    public Rigidbody rigid;
    public GameObject activateTarget;
    public float conditionSpeed;
    public float delay;
    int state;
    float elapsedTime;

    private void FixedUpdate() {
        switch (state) {
            case 0:
                if (rigid && rigid.velocity.sqrMagnitude >= conditionSpeed * conditionSpeed) {
                    state = 1;
                }
                break;
            case 1:
                elapsedTime += Time.fixedDeltaTime;
                if (elapsedTime >= delay) {
                    if (activateTarget) {
                        activateTarget.SetActive(true);
                    }
                    state = 2;
                }
                break;
        }
    }

}
