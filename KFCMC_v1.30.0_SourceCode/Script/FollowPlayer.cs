using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    public bool forFreeLook;
    public float yOffset;
    Transform trans;
    float[] speedArray = new float[11] { 0f, 0.5f, 0.792238f, 1.196835f, 1.756989f, 2.53251f, 3.6062f, 5.092698f, 7.150719f, 10f, 1000f };

    private void Awake() {
        trans = transform;
    }
    
    void Start () {
        Vector3 posTemp = CharacterManager.Instance.playerTrans.position;
        posTemp.y += yOffset;
        trans.position = posTemp;

        /*
        if (forFreeLook) {
            trans.position = CharacterManager.Instance.playerTrans.position;
        } else {
            trans.position = CharacterManager.Instance.playerTrans.position + Vector3.up;
        }
        */
	}

    void Update() {
        Vector3 toPos = trans.position;
        int index = GameManager.Instance.save.config[GameManager.Save.configID_CameraFollowingSpeed];
        float deltaTimeCache = Time.deltaTime;
        if (forFreeLook) {
            Vector3 targetPos = CharacterManager.Instance.playerCameraPosition;
            targetPos.y += yOffset;
            if (index >= 0 && index <= 9) {
                float t = deltaTimeCache * speedArray[index];
                toPos.y = targetPos.y;
                toPos.x = Mathf.Lerp(toPos.x, targetPos.x, t);
                toPos.z = Mathf.Lerp(toPos.z, targetPos.z, t);
            } else {
                toPos = targetPos;
            }
            trans.position = toPos;
        } else {
            Vector3 targetPos = CharacterManager.Instance.playerCameraPosition;
            targetPos.y += yOffset;
            if (index >= 0 && index <= 9) {
                float t = deltaTimeCache * speedArray[index];
                toPos.x = Mathf.Lerp(toPos.x, targetPos.x, t);
                toPos.z = Mathf.Lerp(toPos.z, targetPos.z, t);
            } else {
                toPos.x = targetPos.x;
                toPos.z = targetPos.z;
            }
            toPos.y = Mathf.Lerp(toPos.y, targetPos.y, deltaTimeCache * (targetPos.y > toPos.y ? 8f : 5f));
            trans.position = toPos;
        }
    }
}
