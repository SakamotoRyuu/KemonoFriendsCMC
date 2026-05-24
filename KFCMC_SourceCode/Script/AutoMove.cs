using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AutoMove : MonoBehaviour {

    [System.Serializable]
    public class QuakeSettings {
        public bool enabled;
        public bool onComplete;
        public Vector3 offset;
        public float amplitude = 10f;
        public float frequency = 4f;
        public float attackTime = 0f;
        public float sustainTime = 0f;
        public float decayTime = 1f;
        public float impactRadius = 1f;
        public float dissipationDistance = 100f;
    }

    public bool playOnStart = true;
    public Vector3 move;
    public float delay;
    public float time;
    public Ease ease = Ease.Linear;
    public QuakeSettings quakeSettings;
    Transform trans;

    private void Awake() {
        trans = transform;
    }

    private void OnDestroy() {
        if (trans) {
            trans.DOKill();
        }
    }

    void Start () {
        if (playOnStart) {
            Action();
        }
    }

    public void Action() {
        if (trans) {
            if (quakeSettings.enabled) {
                if (quakeSettings.onComplete) {
                    if (delay > 0f) {
                        trans.DOLocalMove(move, time).SetRelative().SetEase(ease).SetDelay(delay).OnComplete(QuakeAction);
                    } else {
                        trans.DOLocalMove(move, time).SetRelative().SetEase(ease).OnComplete(QuakeAction);
                    }
                } else {
                    if (delay > 0f) {
                        trans.DOLocalMove(move, time).SetRelative().SetEase(ease).SetDelay(delay).OnStart(QuakeAction);
                    } else {
                        trans.DOLocalMove(move, time).SetRelative().SetEase(ease).OnStart(QuakeAction);
                    }
                }
            } else {
                if (delay > 0f) {
                    trans.DOLocalMove(move, time).SetRelative().SetEase(ease).SetDelay(delay);
                } else {
                    trans.DOLocalMove(move, time).SetRelative().SetEase(ease);
                }
            }
        }
    }
    
    void QuakeAction() {
        if (trans && CameraManager.Instance) {
            CameraManager.Instance.SetQuake(trans.position + quakeSettings.offset, quakeSettings.amplitude, quakeSettings.frequency, quakeSettings.attackTime, quakeSettings.sustainTime, quakeSettings.decayTime, quakeSettings.impactRadius, quakeSettings.dissipationDistance);
        }
    }

}
