using UnityEngine;

public class ImperatrixMundi_CelestialStar : MonoBehaviour {

    Enemy_ImperatrixMundi parentEnemy;
    public AttackDetection attackDetection;

    public void SetParentEnemy(Enemy_ImperatrixMundi parentEnemy) {
        this.parentEnemy = parentEnemy;
        if (attackDetection) {
            attackDetection.SetParentCharacterBase(parentEnemy);
        }
    }

    public void EmitReady() {
        if (parentEnemy) {
            parentEnemy.EmitCelestialReady();
        }
    }

    public void EmitStart() {
        if (parentEnemy) {
            parentEnemy.EmitCelestialStart();
        }
    }

}
