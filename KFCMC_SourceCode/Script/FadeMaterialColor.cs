using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeMaterialColor : MonoBehaviour
{

    public enum LoopType { None, TriangleWave, SawtoothWave, SineWave };

    public Renderer targetRenderer;
    public int[] targetIndex;
    public float delayTime;
    public float fadeTime;
    public Color startColor;
    public Color endColor;
    public bool enablizeRendererOnStart;
    public bool disablizeRendererOnComplete;
    public Material specialMaterialOnFading;
    public bool emissionFade;
    [ColorUsage(false, true)]
    public Color emissionStartColor;
    [ColorUsage(false, true)]
    public Color emissionEndColor;
    public bool getMaterialsOnUpdate;
    public LoopType loopType;

    Material[] mats;
    Material[] originalMaterials;
    Color colorTemp;
    float elapsedTime;
    int emissionPropertyID;
    bool enablizedFlag;

    void SetNewColor(float t)
    {
        if (getMaterialsOnUpdate)
        {
            mats = targetRenderer.materials;
        }
        colorTemp.r = Mathf.Lerp(startColor.r, endColor.r, t);
        colorTemp.g = Mathf.Lerp(startColor.g, endColor.g, t);
        colorTemp.b = Mathf.Lerp(startColor.b, endColor.b, t);
        colorTemp.a = Mathf.Lerp(startColor.a, endColor.a, t);
        for (int i = 0; i < targetIndex.Length; i++)
        {
            mats[targetIndex[i]].color = colorTemp;
        }
        if (emissionFade)
        {
            colorTemp.r = Mathf.Lerp(emissionStartColor.r, emissionEndColor.r, t);
            colorTemp.g = Mathf.Lerp(emissionStartColor.g, emissionEndColor.g, t);
            colorTemp.b = Mathf.Lerp(emissionStartColor.b, emissionEndColor.b, t);
            colorTemp.a = Mathf.Lerp(emissionStartColor.a, emissionEndColor.a, t);
            for (int i = 0; i < targetIndex.Length; i++)
            {
                mats[targetIndex[i]].SetColor(emissionPropertyID, colorTemp);
            }
        }
        targetRenderer.materials = mats;
    }

    void Start()
    {
        if (targetRenderer)
        {
            mats = targetRenderer.materials;
            if (specialMaterialOnFading != null)
            {
                originalMaterials = targetRenderer.sharedMaterials;
                for (int i = 0; i < mats.Length; i++)
                {
                    int renderQueue = mats[i].renderQueue;
                    mats[i] = specialMaterialOnFading;
                    mats[i].renderQueue = renderQueue;
                }
            }
            if (emissionFade)
            {
                emissionPropertyID = Shader.PropertyToID("_EmissionColor");
            }
            if (delayTime > 0)
            {
                SetNewColor(0);
            }
        }
    }

    void Update()
    {
        if (targetRenderer)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > delayTime)
            {
                switch (loopType)
                {
                    case LoopType.None:
                        if (elapsedTime < delayTime + fadeTime)
                        {
                            SetNewColor((elapsedTime - delayTime) / fadeTime);
                        }
                        else
                        {
                            SetNewColor(1);
                            enabled = false;
                        }
                        break;
                    case LoopType.TriangleWave:
                        if (elapsedTime > delayTime + fadeTime * 2)
                        {
                            elapsedTime -= fadeTime * 2;
                        }
                        if (elapsedTime < delayTime + fadeTime)
                        {
                            SetNewColor((elapsedTime - delayTime) / fadeTime);
                        }
                        else
                        {
                            SetNewColor((fadeTime * 2 - (elapsedTime - delayTime)) / fadeTime);
                        }
                        break;
                    case LoopType.SawtoothWave:
                        if (elapsedTime > delayTime + fadeTime)
                        {
                            elapsedTime -= fadeTime;
                        }
                        SetNewColor((elapsedTime - delayTime) / fadeTime);
                        break;
                    case LoopType.SineWave:
                        if (elapsedTime > delayTime + fadeTime * 2)
                        {
                            elapsedTime -= fadeTime * 2;
                        }
                        if (elapsedTime < delayTime + fadeTime)
                        {
                            SetNewColor(Easing.SineInOut(elapsedTime - delayTime, fadeTime, 0, 1));
                        }
                        else
                        {
                            SetNewColor(Easing.SineInOut(fadeTime * 2 - (elapsedTime - delayTime), fadeTime, 0, 1));
                        }
                        break;
                }
                if (enabled)
                {
                    if (!enablizedFlag && enablizeRendererOnStart)
                    {
                        targetRenderer.enabled = true;
                        enablizedFlag = true;
                    }
                }
                else
                {
                    if (disablizeRendererOnComplete)
                    {
                        targetRenderer.enabled = false;
                    }
                    if (specialMaterialOnFading != null)
                    {
                        targetRenderer.materials = originalMaterials;
                    }
                }
            }
        }
    }
}
