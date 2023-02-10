using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchWallFadeManager : SingletonMonoBehaviour<SwitchWallFadeManager> {

    public class WallSet {
        public SwitchWallFade switchWallFade;
        public float interval;
        public int state; // 0=Visible_Slow 1=Visible_Ready 2=Fade
        public Vector3 targetPos;
        public float targetPosDefaultY;
        public float preSqrDist;
        public WallSet(SwitchWallFade switchWallFade, float interval) {
            this.switchWallFade = switchWallFade;
            this.interval = interval;
            state = 0;
            targetPos = switchWallFade.transform.position;
            targetPosDefaultY = targetPos.y;
            preSqrDist = slowCalcMax;
        }
    }

    List<WallSet> wallList = new List<WallSet>(2048);
    float intervalStart;
    Transform cameraTrans;
    bool initialized;
    LayerMask layerMask;
    Color colorWhite = Color.white;

    const float gapDistMin = -4f;
    const float gapDistMax = 0f;
    const float slowCalcMax = 100f;
    static readonly float[] intervalArray = new float[3] { 0.5f, 0.1f, 0.01f };
    static readonly Vector3 playerOffset = new Vector3(0f, 0.5f, 0f);

    public void ClearList() {
        wallList.Clear();
        intervalStart = 0;
        CameraManager.Instance.SetMainCameraTransform(ref cameraTrans);
        initialized = true;
    }

    public void SetWall(SwitchWallFade switchWallFade) {
        wallList.Add(new WallSet(switchWallFade, intervalStart));
        intervalStart += 0.01f;
        if (intervalStart >= 0.5f) {
            intervalStart -= 0.5f;
        }
    }

    private void Start() {
        layerMask = LayerMask.GetMask("InvisibleWall");
    }

    void Update() {
        if (initialized && CharacterManager.Instance && CharacterManager.Instance.playerTrans && cameraTrans) {
            float deltaTimeCache = Time.deltaTime;
            Vector3 playerPos = CharacterManager.Instance.playerTrans.position + playerOffset;
            Vector3 cameraPos = cameraTrans.position;
            float playerCameraSqrDist = (playerPos - cameraPos).sqrMagnitude;
            int wallCount = wallList.Count;
            for (int i = wallCount - 1; i >= 0; i--) {
                if (wallList[i].switchWallFade) {
                    wallList[i].interval += deltaTimeCache;
                    if (wallList[i].interval >= intervalArray[wallList[i].state]) {
                        wallList[i].interval = 0f;
                        wallList[i].targetPos.y = Mathf.Clamp(playerPos.y, wallList[i].targetPosDefaultY, wallList[i].targetPosDefaultY + wallList[i].switchWallFade.targetHeight);
                        float sqrDistTemp = (wallList[i].targetPos - cameraPos).sqrMagnitude - playerCameraSqrDist;
                        int stateTemp = 0;
                        if (sqrDistTemp >= slowCalcMax) {
                            stateTemp = 0; // Visible_Slow
                        } else if (sqrDistTemp >= gapDistMax) {
                            stateTemp = 1; // Visible_Ready
                        } else {
                            if (Physics.Linecast(playerPos, cameraPos, layerMask, QueryTriggerInteraction.Ignore)) {
                                stateTemp = 2; // Fade
                            } else {
                                stateTemp = 1;
                            }
                        }
                        if (wallList[i].state != stateTemp) {
                            wallList[i].state = stateTemp;
                            if (wallList[i].switchWallFade.nowFading != (stateTemp == 2)) {
                                wallList[i].switchWallFade.SetMaterial(stateTemp == 2);
                            }
                        }

                        if (wallList[i].preSqrDist != sqrDistTemp) {
                            wallList[i].preSqrDist = sqrDistTemp;
                            if (wallList[i].state == 2) {
                                if (sqrDistTemp > gapDistMin) {
                                    colorWhite.a = (sqrDistTemp - gapDistMin) / (gapDistMax - gapDistMin) * 0.5f + 0.5f;
                                } else {
                                    colorWhite.a = 0.5f;
                                }
                                wallList[i].switchWallFade.SetColor(colorWhite);
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
