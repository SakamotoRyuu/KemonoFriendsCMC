using UnityEngine;

public class HoldWeaponObject : MonoBehaviour {

    public Transform weaponTrans;
    public MeshRenderer[] weaponRenderer;

    static readonly Vector3 vecOne = Vector3.one;
    static readonly Vector3 vecZero = Vector3.zero;

    public void Awake() {
        if (GameManager.Instance && GameManager.Instance.save.config[GameManager.Save.configID_ShowArms] < 0) {
            SetWeaponActive(false);
        }
    }

    public void SetWeaponActive(bool toActive) {
        if (weaponRenderer.Length > 0) {
            for (int i = 0; i < weaponRenderer.Length; i++) {
                if (weaponRenderer[i]) {
                    weaponRenderer[i].enabled = toActive;
                }
            }
        } else if (weaponTrans) {
            weaponTrans.localScale = toActive ? vecOne : vecZero;
        }
    }

}
