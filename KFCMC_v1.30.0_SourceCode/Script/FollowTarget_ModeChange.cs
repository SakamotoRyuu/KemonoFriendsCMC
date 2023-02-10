using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget_ModeChange : FollowTarget {

    [System.Serializable]
    public class RandomizeSettings {
        public bool randomizeDelay = false;
        public Vector2 delayRange;
        public bool randomizeSpeed = false;
        public Vector2 speedRange;
        public bool randomizeChangeModeTime = false;
        public Vector2 changeModeTimeRange;
    }
    
    public FollowMode changedFollowMode;
    public float changeModeTime = 1;
    public float changeLookSpeed = 5f;
    public RandomizeSettings randomizeSettings;


    bool modeChangeComplete = false;

    protected override void Awake() {
        base.Awake();
        if (randomizeSettings.randomizeDelay) {
            delayTime = Random.Range(randomizeSettings.delayRange.x, randomizeSettings.delayRange.y);
        }
        if (randomizeSettings.randomizeSpeed) {
            speed = Random.Range(randomizeSettings.speedRange.x, randomizeSettings.speedRange.y);
        }
        if (randomizeSettings.randomizeChangeModeTime) {
            changeModeTime = Random.Range(randomizeSettings.changeModeTimeRange.x, randomizeSettings.changeModeTimeRange.y);
        }
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
        if (!modeChangeComplete && elapsedTime >= changeModeTime) {
            followMode = changedFollowMode;
            modeChangeComplete = true;
            lookSpeed = changeLookSpeed;
        }
    }

}
