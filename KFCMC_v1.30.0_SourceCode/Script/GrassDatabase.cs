using UnityEngine;

public class GrassDatabase : SingletonMonoBehaviour<GrassDatabase> {

    [System.Serializable]
    public class GrassData {
        public GameObject prefab;
        public int quads;
    }

    public GrassData[] grass;

    protected override void Awake() {
        if (CheckInstance()) {
            DontDestroyOnLoad(gameObject);
        }
    }

    public GameObject GetGrassPrefab(int area) {
        for (int i = 0; i < grass.Length - 1; i++) {
            if (area < grass[i + 1].quads) {
                return grass[i].prefab;
            }
        }        
        return null;
    }
}
