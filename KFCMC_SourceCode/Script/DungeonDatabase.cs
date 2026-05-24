using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DungeonDatabase : SingletonMonoBehaviour<DungeonDatabase> {

    [System.Serializable]
    public class DG_Set {
        public GameObject dungeonGenerator;
        public int level;
    }

    public DG_Set[] dgSet;
    private GameObject dgInstance;
    private int dgIndex = 0;
    private int[] recent = new int[10];    

    public void ResetRecent() {
        for (int i = 0; i < recent.Length; i++) {
            recent[i] = -1;
        }
    }

    public int GetDGIndex() {
        return dgIndex;
    }

    public DungeonGenerator GetGenerator(int level) {
        if (dgInstance != null) {
            Destroy(dgInstance);
            dgInstance = null;
        }
        int index = 0;
        if (level >= 0) {
            int[] dgShuffle = new int[dgSet.Length];
            for (int i = 0; i < dgShuffle.Length; i++) {
                dgShuffle[i] = i;
            }
            int n = dgShuffle.Length;
            while (n > 1) {
                n--;
                int k = Random.Range(0, n + 1);
                int temp = dgShuffle[k];
                dgShuffle[k] = dgShuffle[n];
                dgShuffle[n] = temp;
            }
            int start = Random.Range(0, dgShuffle.Length);
            bool find = false;
            for (int i = 0; i < dgShuffle.Length && !find; i++) {
                index = dgShuffle[(start + i) % dgShuffle.Length];
                if (dgSet[index].level == level) {
                    find = true;
                    for (int j = 0; j < recent.Length && find; j++) {
                        if (index == recent[j]) {
                            find = false;
                        }
                    }
                }
            }
            if (!find) {
                index = Random.Range(0, dgSet.Length);
            }
            for (int i = recent.Length - 1; i > 0; i--) {
                recent[i] = recent[i - 1];
            }
            recent[0] = index;
        }
        dgInstance = Instantiate(dgSet[index].dungeonGenerator);
        dgIndex = index;
        return dgInstance.GetComponent<DungeonGenerator>();
    }

}
