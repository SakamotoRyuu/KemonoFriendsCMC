using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellienPiece : MonoBehaviour {
    
    public Rigidbody rb;
    public Renderer rend;

    Transform trans;
    bool effectEnable = true;
    int state = 0;
    float scale = 0;
    float duration = 0;
    float delayTime = 0.5f;
    float shrinkTime = 1;
    Vector3 scaleTemp = Vector3.zero;
    float startScale;
    float scaleRate;

    static readonly Quaternion quaternionIdentity = Quaternion.identity;
    static readonly Vector3 vecZero = Vector3.zero;
    const float stayTime = 0.5f;
    const float volume = 0.24f;

    public void SetParam(float scale, float mass, Vector3 force, Vector3 forcePosition, bool effectEnable = true) {
        scaleTemp.x = scaleTemp.y = scaleTemp.z = startScale = scale;
        trans.localScale = scaleTemp;
        rb.mass = mass;
        this.effectEnable = effectEnable;
        this.scale = scale;
        state = 0;
        duration = 0;
        delayTime = 0.2f + scale;
        shrinkTime = scale * 10;
        ObjectPool.Instance.activeInstanceCount++;
        gameObject.SetActive(true);
        rb.AddForceAtPosition(force, forcePosition);
    }

    private void Awake() {
        trans = transform;
    }

    void Update() {
        switch (state) {
            case 0:
                duration += Time.deltaTime;
                if ((duration >= stayTime && (rb.velocity).sqrMagnitude < 0.05f) || duration >= stayTime + shrinkTime + 2) {
                    state = 1;
                    duration = 0;
                }
                break;
            case 1:
                duration += Time.deltaTime;
                if (duration >= delayTime) {
                    state = 2;
                    if (effectEnable) {
                        /*
                        Instantiate(EffectDatabase.Instance.prefab[effectPrefabIndex], trans.position, quaternionIdentity).GetComponent<SandstarBlow>().SetParam(scale);
                        */
                        if (ParticleSystemPool.Instance && AudioSourcePool.Instance) {
                            Vector3 posTemp = trans.position;
                            ParticleSystemPool.Instance.Play(ParticleSystemPool.ParticleName.SandstarBlow, posTemp, scale);
                            AudioSourcePool.Instance.Play(AudioSourcePool.AudioName.SandstarBlow, posTemp, volume);
                        }
                    }
                    duration = 0f;
                }
                break;
            case 2:
                duration += Time.deltaTime;
                scaleRate = Easing.GetEasing(EasingType.SineInOut, duration, shrinkTime, startScale, 0f);
                if (scaleRate > 0f && duration < shrinkTime) {
                    scaleTemp.x = scaleTemp.y = scaleTemp.z = scaleRate;
                    trans.localScale = scaleTemp;
                } else {
                    rb.velocity = vecZero;
                    ObjectPool.Instance.activeInstanceCount--;
                    gameObject.SetActive(false);
                }
                break;
        }
    }
}
