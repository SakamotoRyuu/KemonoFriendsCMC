using UnityEngine;

public class AutoDestroy : MonoBehaviour {

    public float life = 5;

    protected virtual void Start() {
        Destroy(gameObject, life);
    }

}
