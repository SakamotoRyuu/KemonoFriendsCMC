using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineCurveMove : MonoBehaviour {
    
    [System.Serializable]
    public class SineParamSet {
        public Vector3 amplitude;
        public Vector3 period;
        public Vector3 center;
        public Vector3 timeOffset;
        public bool timeOffsetRandomize = true;
        public float time;
    }

    public SineParamSet position;
    public SineParamSet rotation;

    private Vector3 vectorTemp;
    private static readonly Vector3 vecZero = Vector3.zero;

    private void Start() {
        if (position.timeOffsetRandomize) {
            position.timeOffset = new Vector3(Random.Range(0f, position.period.x), Random.Range(0f, position.period.y), Random.Range(0f, position.period.z));
        }
        if (rotation.timeOffsetRandomize) {
            rotation.timeOffset = new Vector3(Random.Range(0f, rotation.period.x), Random.Range(0f, rotation.period.y), Random.Range(0f, rotation.period.z));
        }
    }

    private void Update() {
        if (position.amplitude != vecZero && position.period != vecZero) {
            vectorTemp.x = (position.period.x != 0 ? Mathf.Sin(Mathf.PI * 2 * (position.time + position.timeOffset.x) / position.period.x) : 0) * position.amplitude.x + position.center.x;
            vectorTemp.y = (position.period.y != 0 ? Mathf.Sin(Mathf.PI * 2 * (position.time + position.timeOffset.y) / position.period.y) : 0) * position.amplitude.y + position.center.y;
            vectorTemp.z = (position.period.z != 0 ? Mathf.Sin(Mathf.PI * 2 * (position.time + position.timeOffset.z) / position.period.z) : 0) * position.amplitude.z + position.center.z;
            transform.localPosition = vectorTemp;
            position.time += Time.deltaTime;
        }
        if (rotation.amplitude != vecZero && rotation.period != vecZero) {
            vectorTemp.x = (rotation.period.x != 0 ? Mathf.Sin(Mathf.PI * 2 * (rotation.time + rotation.timeOffset.x) / rotation.period.x) : 0) * rotation.amplitude.x + rotation.center.x;
            vectorTemp.y = (rotation.period.y != 0 ? Mathf.Sin(Mathf.PI * 2 * (rotation.time + rotation.timeOffset.y) / rotation.period.y) : 0) * rotation.amplitude.y + rotation.center.y;
            vectorTemp.z = (rotation.period.z != 0 ? Mathf.Sin(Mathf.PI * 2 * (rotation.time + rotation.timeOffset.z) / rotation.period.z) : 0) * rotation.amplitude.z + rotation.center.z;
            transform.localEulerAngles = vectorTemp;
            rotation.time += Time.deltaTime;
        }
    }

}
