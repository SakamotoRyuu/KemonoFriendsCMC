using System.Collections;
using UnityEngine;

public class AutoDestroy_SaveObject : AutoDestroy {

    public Transform[] saveTarget;

    protected override void Start() {
        StartCoroutine(DestroyAction());
    }

    IEnumerator DestroyAction() {
        yield return new WaitForSeconds(life);
        for (int i = 0; i < saveTarget.Length; i++) {
            if (saveTarget[i]) {
                saveTarget[i].SetParent(null);
            }
        }
        Destroy(gameObject);
    }

}
