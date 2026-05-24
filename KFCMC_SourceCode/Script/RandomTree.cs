using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RandomTree : MonoBehaviour {

    [Range(0, 100)]
    public int incidence;
    public bool isBackGround = true;

    [System.Serializable]
    public class TreeSet {
        public GameObject prefab;
        public Vector3 posRange;
        public Vector2 scaleRange;
        public Vector3 offset;
        public int weighting = 1;
        public bool rotateY = true;
    }

    public TreeSet[] treeSet;
    public int clusterCount;

    protected virtual Quaternion GetRandomRotate() {
        return Quaternion.Euler(new Vector3(0, Random.Range(-180f, 180f), 0));
    }

    protected virtual bool GetInstantiateCondition() {
        return (incidence >= 100 ? true : Random.Range(0, 100) < incidence);
    }

    // Use this for initialization
    protected virtual void Start() {
        if (GetInstantiateCondition() && treeSet.Length > 0 && (!isBackGround || GameManager.Instance.save.config[GameManager.Save.configID_GenerateDungeonObjects] != 0)) {
            int[] accumulate = new int[treeSet.Length];
            accumulate[0] = treeSet[0].weighting;
            for (int i = 1; i < accumulate.Length; i++) {
                accumulate[i] = accumulate[i - 1] + treeSet[i].weighting;
            }
            int pointer = Random.Range(0, accumulate[accumulate.Length - 1]);
            for (int i = 0; i < accumulate.Length; i++) {
                if (pointer < accumulate[i]) {
                    if (treeSet[i].prefab) {
                        int clusterRepeat = 1;
                        if (clusterCount >= 2) {
                            clusterRepeat = Random.Range(clusterCount / 2, clusterCount + 1);
                        }
                        for (int count = 0; count < clusterRepeat; count++) {
                            Vector3 pos = Vector3.zero;
                            if (treeSet[i].posRange != Vector3.zero) {
                                pos = new Vector3(Random.Range(treeSet[i].posRange.x * -0.5f, treeSet[i].posRange.x * 0.5f), Random.Range(treeSet[i].posRange.y * -0.5f, treeSet[i].posRange.y * 0.5f), Random.Range(treeSet[i].posRange.z * -0.5f, treeSet[i].posRange.z * 0.5f));
                            }
                            GameObject instance = Instantiate(treeSet[i].prefab, transform.position + pos + treeSet[i].offset, treeSet[i].rotateY ? GetRandomRotate() : Quaternion.identity, transform);
                            if (treeSet[i].scaleRange.x != 1f || treeSet[i].scaleRange.y != 1f) {
                                float scale;
                                if (treeSet[i].scaleRange.x == treeSet[i].scaleRange.y) {
                                    scale = treeSet[i].scaleRange.x;
                                } else {
                                    scale = Random.Range(treeSet[i].scaleRange.x, treeSet[i].scaleRange.y);
                                }
                                Vector3 scaleVec = new Vector3(scale, scale, scale);
                                if (instance.transform.localScale != scaleVec) {
                                    instance.transform.localScale = scaleVec;
                                }
                            }
                            if (isBackGround) {
                                Collider[] col = instance.GetComponentsInChildren<Collider>();
                                if (col.Length > 0) {
                                    for (int j = 0; j < col.Length; j++) {
                                        col[j].enabled = false;
                                    }
                                }
                                NavMeshObstacle[] obs = instance.GetComponentsInChildren<NavMeshObstacle>();
                                if (obs.Length > 0) {
                                    for (int j = 0; j < obs.Length; j++) {
                                        obs[j].enabled = false;
                                    }
                                }
                                NavMeshLink[] navLink = instance.GetComponentsInChildren<NavMeshLink>();
                                if (navLink.Length > 0) {
                                    for (int j = 0; j < navLink.Length; j++) {
                                        navLink[j].enabled = false;
                                    }
                                }
                                WaitAndEnable[] waitAE = instance.GetComponentsInChildren<WaitAndEnable>();
                                if (waitAE.Length > 0) {
                                    for (int j = 0; j < waitAE.Length; j++) {
                                        waitAE[j].enabled = false;
                                    }
                                }
                            }
                        }
                    }
                    break;
                }
            }
        }
    }
}

