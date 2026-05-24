using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmusementPark_RainbowEmission : MonoBehaviour {

    public Renderer targetRenderer;
    public int matIndex;
    [ColorUsage(false, true)]
    public Color[] colors;
    public float stoppingTime = 20f;
    public float transitionTime = 4f;
    public float timeMultiplierStopping = 1f;
    public float timeMultiplierTransition = 1f;

    Material[] matsTemp;
    float duration;
    int emissionID;
    bool isTransition;
    int colorIndex;
    Color colorSave = Color.black;

    private void Awake() {
        matsTemp = targetRenderer.materials;
        emissionID = Shader.PropertyToID("_EmissionColor");
    }

    void Update() {
        Color newColor;
        if (isTransition) {
            duration += Time.deltaTime * timeMultiplierTransition;
            if (duration >= transitionTime) {
                duration -= transitionTime;
                colorIndex = (colorIndex + 1) % colors.Length;
                isTransition = false;
            }
        } else {
            duration += Time.deltaTime * timeMultiplierStopping;
            if (duration >= stoppingTime) {
                duration -= stoppingTime;
                isTransition = true;
            }
        }
        if (isTransition) {
            newColor = Color.Lerp(colors[colorIndex], colors[(colorIndex + 1) % colors.Length], duration / transitionTime);
        } else {
            newColor = colors[colorIndex];
        }
        if (newColor != colorSave) {
            colorSave = newColor;
            matsTemp[matIndex].SetColor(emissionID, newColor);
        }
    }

}
