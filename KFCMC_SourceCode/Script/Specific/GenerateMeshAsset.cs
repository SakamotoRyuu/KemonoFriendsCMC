using System.Collections;
using UnityEngine;
using UnityEditor;
using System;

public class GenerateMeshAsset : MonoBehaviour {
    
    public void Generate() {
#if UNITY_EDITOR
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (meshCollider && meshCollider.sharedMesh) {
            string objName = gameObject.name;
            string objTrim = objName.Trim('.', '/');
            string path = "Assets/" + objTrim + ".mesh";
            AssetDatabase.CreateAsset(meshCollider.sharedMesh, path);
            AssetDatabase.SaveAssets();
            Debug.Log(path);
        } else {
            Debug.Log("Mesh not found.");
        }
#endif
    }
}
