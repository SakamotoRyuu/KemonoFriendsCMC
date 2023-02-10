using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFogOnEnable : MonoBehaviour {

    public int fogNumber;

    private void OnEnable() {
        if (FogDatabase.Instance) {
            FogDatabase.Instance.SetFog(fogNumber);
        }
    }

}
