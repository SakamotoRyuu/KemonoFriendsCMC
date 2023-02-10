using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AraneusEgg : MonoBehaviour {

    public CharacterBase parentCBase;
    public float destroyTimer;
    public GameObject effectPrefab;
    public GameObject spiderPrefab;
    public int spiderNum = 4;
    public float spiderRadius = 0.5f;

    private void Update() {
        if (parentCBase == null) {
            Destroy(gameObject);
        } else {
            destroyTimer -= Time.deltaTime;
            if (destroyTimer <= 0f) {
                Instantiate(effectPrefab, transform.position, Quaternion.identity);
                Vector3 randPos = Vector3.zero;
                for (int i = 0; i < spiderNum; i++) {
                    Vector2 radiusTemp = Random.insideUnitCircle * spiderRadius;
                    randPos.x = radiusTemp.x;
                    randPos.z = radiusTemp.y;
                    GameObject spiderInstance = Instantiate(spiderPrefab, transform.position + randPos, Quaternion.Euler(0f, Random.Range(-180f, 180f), 0f));
                    AttackDetectionProjectile attackTemp = spiderInstance.GetComponentInChildren<AttackDetectionProjectile>();
                    if (attackTemp) {
                        attackTemp.SetParentCharacterBase(parentCBase);
                    }
                }
                Destroy(gameObject);
            }
        }
    }

}
