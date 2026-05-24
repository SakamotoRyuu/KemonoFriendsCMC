using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateRandomLocalPosition : MonoBehaviour {

    public GameObject prefab;
    public int probability;
    public float scaleMin;
    public float scaleMax;
    public Vector3 localPosMin;
    public Vector3 localPosMax;
    public bool checkGenerateDungeonObject;

    private void Start() {
        if ((!checkGenerateDungeonObject || GameManager.Instance.save.config[GameManager.Save.configID_GenerateDungeonObjects] != 0) && Random.Range(0, 100) < probability) {
            GameObject instance = Instantiate(prefab, transform.position, transform.rotation, transform);
            instance.transform.localScale = Vector3.one * Random.Range(scaleMin, scaleMax);
            if (localPosMin != Vector3.zero || localPosMax != Vector3.zero) {
                float xTemp = localPosMin.x != localPosMax.x ? Random.Range(localPosMin.x, localPosMax.x) : localPosMin.x;
                float yTemp = localPosMin.y != localPosMax.y ? Random.Range(localPosMin.y, localPosMax.y) : localPosMin.y;
                float zTemp = localPosMin.z != localPosMax.z ? Random.Range(localPosMin.z, localPosMax.z) : localPosMin.z;
                instance.transform.localPosition = new Vector3(xTemp, yTemp, zTemp);
            }
        }
    }

}
