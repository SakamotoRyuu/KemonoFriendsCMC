using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotationOnMoveFloor : MonoBehaviour {

    public bool rotateX;
    public bool rotateY;
    public bool rotateZ;
    DungeonController dcSave;
    
    void Update() {
        if (StageManager.Instance && StageManager.Instance.dungeonController && StageManager.Instance.dungeonController != dcSave) {
            dcSave = StageManager.Instance.dungeonController;
            float angleX = rotateX ? Random.Range(-180f, 180f) : transform.localEulerAngles.x;
            float angleY = rotateY ? Random.Range(-180f, 180f) : transform.localEulerAngles.y;
            float angleZ = rotateZ ? Random.Range(-180f, 180f) : transform.localEulerAngles.z;
            transform.localEulerAngles = new Vector3(angleX, angleY, angleZ);
        }
    }
}
