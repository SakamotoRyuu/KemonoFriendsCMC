using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateWithGrid : MonoBehaviour {

    public Vector3 offset;
    public Vector3 interval;
    public Vector3Int maxCount = Vector3Int.one;
    public Vector3 rotationRange;
    public GameObject[] prefab;

    private void Awake() {
        if (GameManager.Instance.save.config[GameManager.Save.configID_GenerateDungeonObjects] != 0) {
            Transform trans = transform;
            Vector3 origin = trans.position + offset;
            Vector3 originRot = trans.rotation.eulerAngles;
            for (int i = 0; i < maxCount.x; i++) {
                for (int j = 0; j < maxCount.y; j++) {
                    for (int k = 0; k < maxCount.z; k++) {
                        Instantiate(prefab[Random.Range(0, prefab.Length)],
                            origin + Vector3.Scale(interval, new Vector3(i, j, k)),
                            Quaternion.Euler(originRot.x + Random.Range(rotationRange.x * -0.5f, rotationRange.x * 0.5f), originRot.y + Random.Range(rotationRange.y * -0.5f, rotationRange.y * 0.5f), originRot.z + Random.Range(rotationRange.z * -0.5f, rotationRange.z * 0.5f)),
                            trans);
                    }
                }
            }
        }
    }

}
