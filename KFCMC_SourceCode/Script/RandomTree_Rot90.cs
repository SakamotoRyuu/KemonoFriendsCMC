using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTree_Rot90 : RandomTree {

    protected override Quaternion GetRandomRotate() {
        return Quaternion.Euler(new Vector3(0, Random.Range(-1, 3) * 90f, 0));
    }

}
