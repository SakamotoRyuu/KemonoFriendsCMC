using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class FootstepManager : SingletonMonoBehaviour<FootstepManager> {

    int actionType;
    int lastPlayNum;
    bool loaded;
    int landRemain;

    AudioClip[,,] footStepSounds = new AudioClip[10, 3, 5];
    string[] materialName = new string[10] { "concrete", "dirt", "grass", "metal", "snow", "wood", "mud", "carpet", "ice", "water" };
    string[] actionName = new string[3] { "land", "run", "walk" };
    int[,] randomMax = new int[10, 3] { { 2, 5, 5 }, { 2, 5, 5 }, { 2, 5, 5 }, { 2, 5, 5 }, { 2, 5, 5 }, { 2, 5, 5 }, { 0, 0, 5 }, { 0, 0, 5 }, { 0, 0, 5 }, { 0, 0, 5 } };
    float[] pitch = new float[10] { 1.3f, 1.3f, 1.3f, 1.3f, 1.3f, 1.3f, 1, 1, 1, 1 };
    
    public void Init() {
        actionType = 2;
        lastPlayNum = -1;
    }

    public void LoadFiles() {
        if (!loaded) {
            loaded = true;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < materialName.Length; i++) {
                for (int j = 0; j < actionName.Length; j++) {
                    for (int k = 0; k < randomMax[i, j]; k++) {
                        footStepSounds[i, j, k] = Resources.Load(sb.Clear().Append("Footstep/").Append(materialName[i]).Append("_").Append(actionName[j]).Append("_0").Append(k + 1).ToString()) as AudioClip;
                    }
                }
            }
        }
    }

    public void SetActionType(int num) {
        actionType = num;
        lastPlayNum = -1;
        landRemain = (actionType == 0 ? 2 : 0);
    }

    public float GetMaterialPitch(int materialType) {
        if (materialType >= 0 && materialType < materialName.Length) {
            return pitch[materialType];
        } else {
            return 1f;
        }
    }

    public int JudgeType(GameObject judgeObj) {
        for (int i = 0; i < materialName.Length; i++) {
            if (judgeObj.CompareTag(materialName[i])) {
                return i;
            }
        }
        return -1;
    }

    public AudioClip GetPlayClip(int materialType) {
        if (materialType >= 0 && materialType < materialName.Length && actionType >= 0 && actionType < actionName.Length) {
            if (randomMax[materialType, actionType] <= 0) {
                actionType = 2;
            }
            if (randomMax[materialType, actionType] > 0) {
                int randomNum = Random.Range(0, randomMax[materialType, actionType]);
                if (randomNum == lastPlayNum) {
                    randomNum = (randomNum + Random.Range(1, randomMax[materialType, actionType])) % randomMax[materialType, actionType];
                }
                lastPlayNum = randomNum;
                int actionTemp = actionType;
                if (actionType == 0) {
                    landRemain--;
                    if (landRemain <= 0) {
                        actionType = 2;
                    }
                }
                return footStepSounds[materialType, actionTemp, randomNum];
            } else { return null; }
        } else { return null; }
    }
}