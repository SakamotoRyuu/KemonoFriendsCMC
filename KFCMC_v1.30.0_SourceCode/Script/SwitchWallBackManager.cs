using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchWallBackManager : SingletonMonoBehaviour<SwitchWallBackManager> {

    public class WallSet {
        public SwitchWallBack switchWallBack;
        public float interval;
        public int state;
        public Vector3 targetPos;
        public float targetPosDefaultY;
        public float preSqrDist;
        public WallSet(SwitchWallBack switchWallBack, float interval) {
            this.switchWallBack = switchWallBack;
            this.interval = interval;
            state = 0;
            targetPos = switchWallBack.switchTarget.transform.position;
            targetPosDefaultY = targetPos.y;
            preSqrDist = slowCalcSqrDist;
        }
    }

    List<WallSet> wallList = new List<WallSet>(2048);
    Transform playerTrans;
    Transform cameraTrans;
    Color colorWhite = Color.white;
    float intervalStart;
    bool initialized;

    const float gapDistMin = 6.25f;
    const float gapDistMax = 25f;
    const float slowCalcSqrDist = 225f;
    const float targetHeight = 3f;
    static readonly float[] intervalArray = new float[4] { 0.5f, 0.1f, 0.1f, 0.1f };

    public void ClearList() {
        wallList.Clear();
        intervalStart = 0;
        CameraManager.Instance.SetMainCameraTransform(ref cameraTrans);
        initialized = true;
    }

    public void SetWall(SwitchWallBack switchWallBack) {
        wallList.Add(new WallSet(switchWallBack, intervalStart));
        intervalStart += 0.01f;
        if (intervalStart >= 0.5f) {
            intervalStart -= 0.5f;
        }
    }

    void Update() {
        if (initialized && CharacterManager.Instance && CharacterManager.Instance.playerTrans && cameraTrans) {
            float deltaTimeCache = Time.deltaTime;
            Vector3 playerPos = CharacterManager.Instance.playerTrans.position;
            Vector3 cameraPos = cameraTrans.position;
            float playerCameraSqrDist = (playerPos - cameraPos).sqrMagnitude;
            int wallCount = wallList.Count;
            for (int i = wallCount - 1; i >= 0; i--) {
                if (wallList[i].switchWallBack) {
                    wallList[i].interval += deltaTimeCache;
                    if (wallList[i].interval >= intervalArray[wallList[i].state]) {
                        wallList[i].interval = 0f;
                        wallList[i].targetPos.y = Mathf.Clamp(playerPos.y, wallList[i].targetPosDefaultY, wallList[i].targetPosDefaultY + targetHeight);
                        float sqrDistTemp = (wallList[i].targetPos - cameraPos).sqrMagnitude - playerCameraSqrDist;
                        if (wallList[i].switchWallBack.sqrDistOffset != 0f) {
                            sqrDistTemp += wallList[i].switchWallBack.sqrDistOffset;
                        }
                        int stateTemp = 0;
                        if (sqrDistTemp >= slowCalcSqrDist) {
                            stateTemp = 0; // Visible_Slow
                        } else if (sqrDistTemp >= gapDistMax) {
                            stateTemp = 1; // Visible_Ready
                        } else if (sqrDistTemp > gapDistMin) {
                            stateTemp = 2; // Transparent
                        } else {
                            stateTemp = 3; // Invisible
                        }
                        if (wallList[i].state != stateTemp) {
                            wallList[i].state = stateTemp;
                            if (wallList[i].switchWallBack.nowFading != (stateTemp >= 2)) {
                                wallList[i].switchWallBack.SetMaterial(stateTemp >= 2);
                            }
                            if (wallList[i].switchWallBack.nowRendererEnabled != (stateTemp < 3)) {
                                wallList[i].switchWallBack.SetRendererEnabled(stateTemp < 3);
                            }
                        }
                        if (wallList[i].preSqrDist != sqrDistTemp) {
                            wallList[i].preSqrDist = sqrDistTemp;
                            if (wallList[i].state == 2) {
                                colorWhite.a = Mathf.Clamp01((sqrDistTemp - gapDistMin) / (gapDistMax - gapDistMin));
                                wallList[i].switchWallBack.SetColor(colorWhite);
                            }
                        }
                    }
                } else {
                    wallList.RemoveAt(i);
                }
            }
        }
    }
}
