using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetLightingOnEnable : MonoBehaviour {

    public int lightingNumber;
    public bool checkPlayerLight;

    private void OnEnable() {
        if (LightingDatabase.Instance) {
            LightingDatabase.Instance.SetLighting(lightingNumber);
            if (checkPlayerLight && CharacterManager.Instance) {
                CharacterManager.Instance.SetPlayerLightActive();
            }
        }
    }

}
