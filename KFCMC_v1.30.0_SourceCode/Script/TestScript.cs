using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{

    public Transform trans0;
    public Transform trans1;
    public Transform trans2;
    Vector3 speed;
    Vector3 rotTemp;
    const int count = 10000;

    private void Start() {
        /*
        speed = new Vector3(0f, 90f, 0f);
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        float delta = 0.0166666f;

        sw.Start();
        for (int i = 0; i < count; i++) {
            trans0.Rotate(speed * delta);
        }
        Debug.Log("Rotate() = " + sw.Elapsed);
        sw.Reset();

        rotTemp = Vector3.zero;
        sw.Start();
        for (int i = 0; i < count; i++) {
            rotTemp.y += speed.y * delta;
            trans1.eulerAngles = rotTemp;
        }
        Debug.Log("eulerAngles = " + sw.Elapsed);
        sw.Reset();

        rotTemp = Vector3.zero;
        sw.Start();
        for (int i = 0; i < count; i++) {
            if (speed.x != 0f) {
                rotTemp.x += speed.x * delta;
                if (rotTemp.x > 180f) {
                    rotTemp.x -= 360f;
                }
            }
            if (speed.y != 0f) {
                rotTemp.y += speed.y * delta;
                if (rotTemp.y > 180f) {
                    rotTemp.y -= 360f;
                }
            }
            if (speed.z != 0f) {
                rotTemp.z += speed.z * delta;
                if (rotTemp.z > 180f) {
                    rotTemp.z -= 360f;
                }
            }
            trans2.localEulerAngles = rotTemp;
        }
        Debug.Log("localEulerAngles = " + sw.Elapsed);
        sw.Reset();
        */
    }

}
