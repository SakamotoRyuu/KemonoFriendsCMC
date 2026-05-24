using UnityEngine;

public class GrassControl : MonoBehaviour {

    public Material[] material;
    public float[] densityMultiplier;

    int matIndex;
    float xSize;
    float zSize;
    float density;
    GameObject grassObj;
    bool initialized;
    int grassIndexSave = -1;

    public void SetGrassBody() {
        if (initialized) {
            if (GameManager.Instance.save.config[GameManager.Save.configID_ShowGrass] == 0) {
                grassIndexSave = -1;
                if (grassObj != null) {
                    Destroy(grassObj);
                    grassObj = null;
                }
            } else {
                int grassIndex = (int)(xSize * zSize * density * (GameManager.Instance.save.config[GameManager.Save.configID_ShowGrass] >= 2 ? 0.5f : 1f));
                if (grassIndex != grassIndexSave) {
                    if (grassObj != null) {
                        Destroy(grassObj);
                        grassObj = null;
                    }
                    grassIndexSave = grassIndex;
                    GameObject prefab = GrassDatabase.Instance.GetGrassPrefab(grassIndex);
                    if (prefab) {
                        grassObj = Instantiate(prefab, transform);
                        Renderer rend = grassObj.GetComponent<Renderer>();
                        if (rend) {
                            grassObj.transform.localScale = new Vector3(xSize, 1, zSize);
                            rend.material = material[matIndex];
                            grassObj.SetActive(true);
                        }
                    }
                }
            }
        }
    }

    public void SetGrass(int matIndex, float xSize, float zSize, float density = 1) {
        if (material.Length > 0) {
            this.matIndex = matIndex % material.Length;
            this.xSize = xSize;
            this.zSize = zSize;
            this.density = density;
            if (this.matIndex < densityMultiplier.Length) {
                this.density *= densityMultiplier[this.matIndex];
            }
            initialized = true;
            SetGrassBody();
        }
    }
}
