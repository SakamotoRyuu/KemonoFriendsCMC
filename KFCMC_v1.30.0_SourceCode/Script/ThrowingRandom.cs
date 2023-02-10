using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingRandom : Throwing {

    public int[] weighting;
    int[] accumulate;
    int objIndex = -1;

    protected override void Start() {
        base.Start();
        accumulate = new int[weighting.Length];
        accumulate[0] = weighting[0];
        for (int i = 1; i < accumulate.Length; i++) {
            accumulate[i] = accumulate[i - 1] + weighting[i];
        }
    }

    private int GetRandomIndex(int defaultIndex = 0) {
        int pointer = Random.Range(0, accumulate[accumulate.Length - 1]);
        for (int i = 0; i < accumulate.Length; i++) {
            if (pointer < accumulate[i]) {
                return i;
            }
        }
        return defaultIndex;
    }    

    public override void ThrowReady(int index) {
        objIndex = GetRandomIndex(index);
        base.ThrowReady(objIndex);
    }

    public override void ThrowStart(int index) {
        if (objIndex < 0) {
            objIndex = GetRandomIndex(index);
        }
        base.ThrowStart(objIndex);
        objIndex = -1;
    }
}
