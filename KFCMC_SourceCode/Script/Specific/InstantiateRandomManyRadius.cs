using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateRandomManyRadius : MonoBehaviour {

    [System.Serializable]
    public struct RandomManyRadiusSet {
        public GameObject prefab;
        public int weight;
        public bool randomizeScale;
        public Vector2 scaleRange;
    }

    public float radius;
    public int count;
    public RandomManyRadiusSet[] randomManyRadiusSet;
    int[] accumulate;

    private void Start() {
        accumulate = new int[randomManyRadiusSet.Length];
        if (accumulate.Length > 0) {
            accumulate[0] = randomManyRadiusSet[0].weight;
            for (int i = 1; i < accumulate.Length; i++) {
                accumulate[i] = accumulate[i - 1] + randomManyRadiusSet[i].weight;
            }
        }
        InstantiateRandomBody();
    }

    private int GetRandomIndex(int defaultIndex = 0) {
        int pointer = Random.Range(0, accumulate[accumulate.Length - 1]);
        for (int i = 0; i < accumulate.Length; i++) {
            if (pointer < accumulate[i]) {
                return i;
            }
        }
        return defaultIndex;
    }

    private void InstantiateRandomBody() {
        Vector3 pivot = transform.position;
        for (int i = 0; i < count; i++) {
            int index = GetRandomIndex();
            if (randomManyRadiusSet[index].prefab) {
                Vector2 randCircle = Random.insideUnitCircle * radius;
                Vector3 randPos = pivot;
                randPos.x += randCircle.x;
                randPos.z += randCircle.y;
                if (randomManyRadiusSet[index].randomizeScale) {
                    Transform instanceTrans = Instantiate(randomManyRadiusSet[index].prefab, randPos, Quaternion.Euler(0f, Random.Range(-180f, 180f), 0f), transform).transform;
                    Vector3 scaleTemp = Vector3.one * Random.Range(randomManyRadiusSet[index].scaleRange.x, randomManyRadiusSet[index].scaleRange.y);
                    instanceTrans.localScale = scaleTemp;
                } else {
                    Instantiate(randomManyRadiusSet[index].prefab, randPos, Quaternion.Euler(0f, Random.Range(-180f, 180f), 0f), transform);
                }
            }
        }
    }

}
