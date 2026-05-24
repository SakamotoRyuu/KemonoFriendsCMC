using UnityEngine;

public class AutoDeactivate : MonoBehaviour {

    public float remainTime = 0;
    
	void Update () {
        remainTime -= Time.deltaTime;
        if (remainTime <= 0) {
            enabled = false;
        }
	}
}
