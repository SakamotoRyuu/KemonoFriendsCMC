using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonPlayerHeightWall : MonoBehaviour {

    public WallControl wallControl;
    public Collider wallCollider;

    void Start() {
        if (wallControl && wallCollider && StageManager.Instance && StageManager.Instance.dungeonController) {
            StageManager.Instance.dungeonController.SetPlayerHeightWall(wallControl.mapPosition.x, wallControl.mapPosition.y, wallCollider);
        }
    }
}
