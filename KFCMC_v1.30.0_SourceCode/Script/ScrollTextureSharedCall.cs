using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTextureSharedCall : MonoBehaviour {

    public int index;
	
	// Update is called once per frame
	void Update () {
		if (ScrollTextureSharedManager.Instance != null) {
            ScrollTextureSharedManager.Instance.CallScroll(index);
        }
	}
}
