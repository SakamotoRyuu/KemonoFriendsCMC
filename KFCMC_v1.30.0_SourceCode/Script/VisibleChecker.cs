using UnityEngine;

public class VisibleChecker : MonoBehaviour {

    [System.NonSerialized]
    public bool isVisible;

    private void OnBecameVisible() {
        isVisible = true;
    }

    private void OnBecameInvisible() {
        isVisible = false;
    }

}
