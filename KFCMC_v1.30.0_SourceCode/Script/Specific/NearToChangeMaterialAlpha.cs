using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearToChangeMaterialAlpha : MonoBehaviour {

    public Renderer targetRenderer;
    public Transform distancePivot;
    public int materialIndex;
    public bool ignoreY;
    public bool switchRendererEnabled;
    public float nearDistance;
    public float nearAlpha;
    public float farDistance;
    public float farAlpha;
    public EasingType easingType;
    public float restDistance;

    Material[] matsTemp;
    Transform playerTrans;
    float alphaSave;
    Color colorTemp;
    float restTimeRemain;

    private void Awake() {
        if (targetRenderer) {
            matsTemp = targetRenderer.materials;
            playerTrans = CharacterManager.Instance.playerTrans;
            alphaSave = farAlpha;
            colorTemp = matsTemp[materialIndex].color;
            colorTemp.a = alphaSave;
            matsTemp[materialIndex].color = colorTemp;
            targetRenderer.materials = matsTemp;
            if (switchRendererEnabled && targetRenderer.enabled != (alphaSave > 0f)) {
                targetRenderer.enabled = alphaSave > 0f;
            }
        }
    }

    private void Update() {
        restTimeRemain -= Time.deltaTime;
        if (restTimeRemain <= 0f && targetRenderer && playerTrans && distancePivot) {
            Vector3 playerPos = playerTrans.position;
            Vector3 pivotPos = distancePivot.position;
            float alphaTemp = alphaSave;
            if (ignoreY) {
                playerPos.y = pivotPos.y;
            }
            float sqrDist = (playerPos - pivotPos).sqrMagnitude;
            if (sqrDist >= restDistance * restDistance) {
                restTimeRemain = 0.125f;
            } else {
                restTimeRemain = 0.01f;
            }
            if (sqrDist >= farDistance * farDistance) {
                alphaTemp = farAlpha;
            } else if (sqrDist <= nearDistance * nearDistance) {
                alphaTemp = nearAlpha;
            } else {
                alphaTemp = Easing.GetEasing(easingType, Vector3.Distance(playerPos, pivotPos) - nearDistance, farDistance - nearDistance, nearAlpha, farAlpha);
            }
            if (alphaTemp != alphaSave) {
                alphaSave = alphaTemp;
                colorTemp.a = alphaSave;
                matsTemp[materialIndex].color = colorTemp;
                targetRenderer.materials = matsTemp;
                if (switchRendererEnabled && targetRenderer.enabled != (alphaSave > 0f)) {
                    targetRenderer.enabled = alphaSave > 0f;
                }
            }
        }
    }

}
