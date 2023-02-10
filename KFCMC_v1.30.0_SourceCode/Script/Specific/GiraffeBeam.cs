using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XftWeapon;

public class GiraffeBeam : MonoBehaviour {

    public XWeaponTrail xWeaponTrail;
    public LineRenderer lineRenderer;
    public bool forHyper;

    bool beamEnabled = false;
    bool isPlayer;
    AttackDetection attackDetection;

    void SetBeam(bool flag) {
        if (GameManager.Instance.save.config[GameManager.Save.configID_ShowGiraffeBeam] >= (isPlayer ? 1 : 2)) {
            if (flag) {
                xWeaponTrail.Activate();
            } else {
                xWeaponTrail.StopSmoothly(0.2f);
            }
            lineRenderer.enabled = flag;
        } else {
            xWeaponTrail.Deactivate();
            lineRenderer.enabled = false;
        }
        beamEnabled = flag;
    }

    bool CheckCondition() {
        return (attackDetection && ((CharacterManager.Instance && CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Long)) || (forHyper && attackDetection.parentCBase && attackDetection.parentCBase.isSuperman)));
    }

	// Use this for initialization
	void Start () {
        xWeaponTrail.Init();
        SetBeam(false);
        attackDetection = GetComponentInParent<AttackDetection>();
        if (attackDetection && attackDetection.parentCBase) {
            isPlayer = attackDetection.parentCBase.isPlayer;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (CheckCondition()) {
            if (attackDetection.attackEnabled != beamEnabled) {
                SetBeam(attackDetection.attackEnabled);
            }
        } else {
            Destroy(gameObject);
        }
	}
}
