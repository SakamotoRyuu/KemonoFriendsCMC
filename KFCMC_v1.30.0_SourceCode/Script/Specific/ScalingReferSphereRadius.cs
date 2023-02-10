using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalingReferSphereRadius : MonoBehaviour
{

    public CapsuleCollider sphere;
    public Renderer rend;
    public float startAlphaParam = 0.5f;
    public bool alphaRandom;
    public float disablizeTime = 1f;

    private float elapsedTime;
    private bool endScaleFlag;
    private Color whiteColor = new Color(1f, 1f, 1f, 1f);
    private Vector3 vecOne = Vector3.one;
    private Transform trans;

    private void Awake() {
        rend.enabled = false;
        trans = transform;
    }

    // Update is called once per frame
    void Update() {
        if (rend && sphere) {
            if (!rend.enabled && sphere.enabled) {
                trans.localScale = 2f * vecOne * sphere.radius;
                rend.enabled = true;
                elapsedTime = 0f;
            }
            if (rend.enabled && sphere.enabled) {
                trans.localScale = 2f * vecOne * sphere.radius;
            }
            if (rend.enabled && !sphere.enabled) {
                if (!endScaleFlag) {
                    trans.localScale = 2f * vecOne * sphere.radius;
                    endScaleFlag = true;
                }
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= disablizeTime) {
                    Destroy(gameObject);
                } else if (disablizeTime > 0f) {
                    float paramMul = (disablizeTime - elapsedTime) / disablizeTime;
                    if (alphaRandom) {
                        whiteColor.a = startAlphaParam * Random.Range(paramMul * 0.4f, paramMul);
                    } else {
                        whiteColor.a = startAlphaParam * paramMul;
                    }
                    rend.material.color = whiteColor;
                }
            }
        } else {
            Destroy(gameObject);
        }
    }
}
