using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTextureSharedManager : SingletonMonoBehaviour<ScrollTextureSharedManager> {

    public ScrollTextureShared[] scrollTextureShared;
    private bool[] called;

    protected override void Awake() {
        called = new bool[scrollTextureShared.Length];
    }

    private void Update() {
        for (int i = 0; i < called.Length; i++) {
            if (called[i]) {
                called[i] = false;
            }
        }
    }

    public void CallScroll(int index) {
        if (index >= 0 && index < scrollTextureShared.Length && scrollTextureShared[index] != null && !called[index]) {
            scrollTextureShared[index].Scroll();
            called[index] = true;
        }
    }

}
