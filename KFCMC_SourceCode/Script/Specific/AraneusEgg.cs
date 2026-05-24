using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AraneusEgg : MonoBehaviour {

    [System.Serializable]
    public class ChildSpider
    {
        public GameObject prefab;
        public int count;
        public float radius;
    }

    public CharacterBase parentCBase;
    public float destroyTimer;
    public GameObject effectPrefab;
    public ChildSpider[] childSpiders;

    private void Update() {
        if (parentCBase == null) {
            Destroy(gameObject);
        } else {
            destroyTimer -= Time.deltaTime;
            if (destroyTimer <= 0f) {
                Instantiate(effectPrefab, transform.position, Quaternion.identity);
                Vector3 randPos = Vector3.zero;
                for (int i = 0; i < childSpiders.Length; i++)
                {
                    for (int j = 0; j < childSpiders[i].count; j++)
                    {
                        Vector2 radiusTemp = Random.insideUnitCircle * childSpiders[i].radius;
                        randPos.x = radiusTemp.x;
                        randPos.z = radiusTemp.y;
                        GameObject spiderInstance = Instantiate(childSpiders[i].prefab, transform.position + randPos, Quaternion.Euler(0f, Random.Range(-180f, 180f), 0f));
                        AttackDetectionProjectile attackTemp = spiderInstance.GetComponentInChildren<AttackDetectionProjectile>();
                        if (attackTemp)
                        {
                            attackTemp.SetParentCharacterBase(parentCBase);
                        }
                    }
                }
                Destroy(gameObject);
            }
        }
    }

}
