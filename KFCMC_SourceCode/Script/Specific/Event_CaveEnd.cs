using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_CaveEnd : MonoBehaviour
{

    public Transform startPoint;
    public Transform endPoint;
    public float fogStartDensity;
    public Color fogStartColor;
    public float skyboxStartExposure;
    [ColorUsage(false, true)]
    public Color ambientStartColor;
    public float fogEndDensity;
    public Color fogEndColor;
    public float skyboxEndExposure;
    [ColorUsage(false, true)]
    public Color ambientEndColor;
    public GameObject deactivateObj;
    public string deactivateTag;
    public string skyboxPropertyName;
    public string[] snapshotNames; // 0=Start 1=End

    int state;
    float playerPosSave;
    int skyboxPropertyID;
    float[] snapshotAmounts = new float[] { 1, 0 };

    private void Awake()
    {
        skyboxPropertyID = Shader.PropertyToID(skyboxPropertyName);
        playerPosSave = 1000;
    }

    private void Start()
    {
        if (deactivateObj)
        {
            deactivateObj.SetActive(false);
        }
        if (deactivateTag != "")
        {
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(deactivateTag);
            for (int i = 0; i < gameObjects.Length; i++)
            {
                gameObjects[i].SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (CharacterManager.Instance.playerTrans)
        {
            float playerPos = CharacterManager.Instance.playerTrans.position.x;
            if (playerPosSave > playerPos)
            {
                playerPosSave = playerPos;
                if (playerPos > endPoint.position.x)
                {

                    if (state != 1)
                    {
                        FogDatabase.Instance.SetDensity(fogEndDensity);
                        FogDatabase.Instance.SetColor(fogEndColor);
                        RenderSettings.skybox.SetFloat(skyboxPropertyID, skyboxEndExposure);
                        RenderSettings.ambientSkyColor = ambientEndColor;
                        LightingDatabase.Instance.SetDirty();
                        snapshotAmounts[0] = 0;
                        snapshotAmounts[1] = 1;
                        GameManager.Instance.MixSnapshot(snapshotNames, snapshotAmounts);
                        state = 1;
                    }
                }
                else if (playerPos > startPoint.position.x)
                {
                    float rate = Mathf.Clamp01((playerPos - startPoint.position.x) / (endPoint.position.x - startPoint.position.x));
                    FogDatabase.Instance.SetDensity(Mathf.Lerp(fogStartDensity, fogEndDensity, rate * rate));
                    FogDatabase.Instance.SetColor(Color.Lerp(fogStartColor, fogEndColor, 1f - (1f - rate) * (1f - rate)));
                    RenderSettings.skybox.SetFloat(skyboxPropertyID, Mathf.Lerp(skyboxStartExposure, skyboxEndExposure, rate));
                    RenderSettings.ambientSkyColor = Color.Lerp(ambientStartColor, ambientEndColor, rate);
                    LightingDatabase.Instance.SetDirty();
                    snapshotAmounts[0] = 1 - rate;
                    snapshotAmounts[1] = rate;
                    GameManager.Instance.MixSnapshot(snapshotNames, snapshotAmounts);
                    state = 2;
                }
                else
                {
                    if (state != 3)
                    {
                        FogDatabase.Instance.ResetFog();
                        RenderSettings.skybox.SetFloat(skyboxPropertyID, skyboxStartExposure);
                        RenderSettings.ambientSkyColor = ambientStartColor;
                        snapshotAmounts[0] = 1;
                        snapshotAmounts[1] = 0;
                        GameManager.Instance.MixSnapshot(snapshotNames, snapshotAmounts);
                        state = 3;
                        enabled = false;
                    }
                }
            }
        }
    }

}
