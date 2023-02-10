using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandbyAndActivate : MonoBehaviour {

    public GameObject blackCurtain;
    public GameObject[] standbyPrefab;
    public int[] standbyLightingNumber;
    public int[] standbyNPC;
    public GameObject completeActivateObject;
    private bool completed;

    private void Start() {
        if (blackCurtain) {
            blackCurtain.SetActive(true);
        }
    }

    void Update() {
        if (completed) {
            if (blackCurtain) {
                blackCurtain.SetActive(false);
            }
            if (completeActivateObject) {
                completeActivateObject.SetActive(true);
            }
            gameObject.SetActive(false);
        } else {
            for (int i = 0; i < standbyPrefab.Length; i++) {
                if (standbyPrefab[i]) {
                    GameObject objTemp = Instantiate(standbyPrefab[i], transform);
                    if (objTemp) {
                        Destroy(objTemp);
                    }
                }
            }
            if (standbyLightingNumber.Length > 0 && LightingDatabase.Instance) {
                for (int i = 0; i < standbyLightingNumber.Length; i++) {
                    LightingDatabase.Instance.LoadSkybox(standbyLightingNumber[i]);
                }
            }
            if (standbyNPC.Length > 0 && CharacterDatabase.Instance) {
                for (int i = 0; i < standbyNPC.Length; i++) {
                    CharacterDatabase.Instance.GetNPC(standbyNPC[i]);
                }
            }
            completed = true;
        }
    }
}
