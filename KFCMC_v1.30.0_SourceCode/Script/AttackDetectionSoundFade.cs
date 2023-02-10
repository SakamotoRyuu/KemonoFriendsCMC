using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDetectionSoundFade : AttackDetection {

    public AudioSource audioSource;
    public float inTime = 1;
    public float outTime = 1;
    public float startVolume = 0;
    public float endVolume = 1;
    public float startPitch = 1;
    public float endPitch = 1;
    public bool reportHitAttackToParent;

    private double timeStamp;
    private bool attackEnabledSave;
    private int fading;

    protected override void Awake() {
        base.Awake();
        if (audioSource == null) {
            audioSource = GetComponent<AudioSource>();
        }
    }

    protected override void Update() {
        base.Update();
        if (audioSource) {
            if (attackEnabled && !attackEnabledSave) {
                fading = 1;
                timeStamp = GameManager.Instance.time;
                audioSource.volume = startVolume;
                audioSource.pitch = startPitch;
                if (audioSource.enabled) {
                    audioSource.Play();
                }
                attackEnabledSave = attackEnabled;
            } else if (!attackEnabled && attackEnabledSave) {
                fading = 2;
                timeStamp = GameManager.Instance.time;
                audioSource.volume = endVolume;
                audioSource.pitch = endPitch;
                attackEnabledSave = attackEnabled;
            }

            if (fading == 1) {
                if (GameManager.Instance.time - timeStamp < inTime) {
                    float timePoint = (float)(GameManager.Instance.time - timeStamp) / inTime;
                    audioSource.volume = Mathf.Lerp(startVolume, endVolume, timePoint);
                    audioSource.pitch = Mathf.Lerp(startPitch, endPitch, timePoint);
                } else {
                    audioSource.volume = endVolume;
                    audioSource.pitch = endPitch;
                    fading = 0;
                }
            } else if (fading == 2) {
                if (GameManager.Instance.time - timeStamp < outTime) {
                    float timePoint = (float)(GameManager.Instance.time - timeStamp) / outTime;
                    audioSource.volume = Mathf.Lerp(endVolume, startVolume, timePoint);
                    audioSource.pitch = Mathf.Lerp(endPitch, startPitch, timePoint);
                } else {
                    audioSource.volume = startVolume;
                    audioSource.pitch = startPitch;
                    fading = 0;
                    if (startVolume <= 0) {
                        audioSource.Stop();
                    }
                }
            }
        }
    }

    protected override void HitAttack(ref DamageDetection targetDD) {
        if (reportHitAttackToParent) {
            parentCBase.HitAttackAdditiveProcessDD(ref targetDD);
        }
        base.HitAttack(ref targetDD);
    }

}