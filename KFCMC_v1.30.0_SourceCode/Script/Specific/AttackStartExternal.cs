using UnityEngine;

public class AttackStartExternal : MonoBehaviour {

    public AttackDetection attackDetection;

    public void AttackStartEx() {
        if (attackDetection) {
            attackDetection.DetectionStart();
        }
    }

    public void AttackEndEx() {
        if (attackDetection) {
            attackDetection.DetectionEnd();
        }
    }

}
