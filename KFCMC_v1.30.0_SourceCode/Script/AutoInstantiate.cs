using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoInstantiate : MonoBehaviour {

    public GameObject prefab;
    public float timer;
    public bool andDestroy;
    public Vector3 offset;
    public Vector3 angle;
    public bool parenting;

    protected float remainTime;
    protected bool completed = false;
    protected GameObject instance;

	// Use this for initialization
	void Start () {
        remainTime = timer;
	}

    protected virtual void ExecuteInstantiate() {
        instance = Instantiate(prefab, transform.position + offset, angle == Vector3.zero ? Quaternion.identity : Quaternion.Euler(angle.x, angle.y, angle.z), parenting ? transform : null);
    }
	
	// Update is called once per frame
	void Update () {
        if (!completed) {
            remainTime -= Time.deltaTime;
            if (remainTime <= 0) {
                ExecuteInstantiate();
                completed = true;
                if (andDestroy) {
                    Destroy(gameObject);
                }
            }
        }
	}
}
