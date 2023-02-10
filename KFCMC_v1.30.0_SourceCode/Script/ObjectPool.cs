using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : SingletonMonoBehaviour<ObjectPool> {

    public GameObject[] poolPrefab;
    public int activeInstanceCount;

    Transform trans;
    Transform[] container;
    List<GameObject>[] poolInst;
    bool isInitialized = false;
    const int activeInstanceMax = 500;
    const int startDecreaseNum = 300;
    const int listCapacity = 512;

    protected void Initialize() {
        isInitialized = true;
        trans = transform;
        container = new Transform[poolPrefab.Length];
        poolInst = new List<GameObject>[poolPrefab.Length];
        for (int i = 0; i < poolPrefab.Length; i++) {
            container[i] = new GameObject("Container" + i.ToString()).transform;
            container[i].SetParent(trans);
            poolInst[i] = new List<GameObject>(listCapacity);
        }
        activeInstanceCount = 0;
    }

    protected override void Awake() {
        if (CheckInstance()) {
            DontDestroyOnLoad(gameObject);
            if (!isInitialized) {
                Initialize();
            }
        }
    }

    public void DeactivateAll() {
        for (int i = 0; i < poolInst.Length; i++) {
            int count = poolInst[i].Count;
            for (int j = 0; j < count; j++) {
                poolInst[i][j].SetActive(false);
            }
        }
        activeInstanceCount = 0;
    }

    public void ReserveObject(int index, int reserveCount) {
        if (!isInitialized) {
            Initialize();
        }
        int count = reserveCount - poolInst[index].Count;
        if (count > 0) {
            GameObject objTemp;
            for (int i = 0; i < count; i++) {
                objTemp = Instantiate(poolPrefab[index], container[index]);
                objTemp.SetActive(false);
                poolInst[index].Add(objTemp);
            }
        }
    }

    public GameObject GetObject(int index) {
        if (activeInstanceCount < activeInstanceMax && poolInst[index] != null) {
            int count = poolInst[index].Count;
            for (int i = 0; i < count; i++) {
                if (!poolInst[index][i].activeSelf) {
                    return poolInst[index][i];
                }
            }
            GameObject objTemp = Instantiate(poolPrefab[index], container[index]);
            objTemp.SetActive(false);
            poolInst[index].Add(objTemp);
            return objTemp;
        }
        return null;
    }

    public void CleanPool(int index, int leaveCount) {
        if (!isInitialized) {
            Initialize();
        }
        if (poolInst[index] != null) {
            int count = poolInst[index].Count - leaveCount;
            if (count > 0) {
                for (int i = 0; i < count; i++) {
                    Destroy(poolInst[index][i]);
                }
                poolInst[index].RemoveRange(0, count);
            }
            count = poolInst[index].Count;
            for (int i = 0; i < count; i++) {
                poolInst[index][i].SetActive(false);
            }
        }
    }

    public void CleanPoolAll(int leaveCount) {
        for (int i = 0; i < poolPrefab.Length; i++) {
            CleanPool(i, leaveCount);
        }
        activeInstanceCount = 0;
    }

    public int GetAppropriateNum(int originalNum) {
        if (activeInstanceCount >= activeInstanceMax) {
            return 0;
        } else if (activeInstanceCount <= startDecreaseNum) {
            return originalNum;
        } else {
            return (int)Mathf.Lerp(originalNum, 2, (float)(activeInstanceCount - startDecreaseNum) / (activeInstanceMax - startDecreaseNum));
        }
    }

}
