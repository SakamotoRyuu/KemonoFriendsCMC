using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapBase : MonoBehaviour {

    public bool onlyOnce;
    public bool changeScale;
    public float yScale;
    public Vector2 scaleRange;
    public float destroyDelay;
    public bool canDodge;
    public bool destroyParent;

    protected bool worked;
    protected List<CharacterBase> cBaseList = new List<CharacterBase>();
    protected CharacterBase cBaseTemp;
    protected float radius = 1.5f;

    protected const string targetTag = "PlayerDamageDetection";

    protected virtual void Awake() {
        if (changeScale) {
            float rate = Random.Range(scaleRange.x, scaleRange.y);
            transform.localScale = new Vector3(rate, yScale, rate);
            radius *= rate;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(targetTag) && (!onlyOnce || !worked)) {
            if (canDodge) {
                DamageDetection ddTemp = other.gameObject.GetComponent<DamageDetection>();
                if (ddTemp) {
                    ddTemp.NonDamageDodge(transform.position);
                }
            }
            cBaseTemp = other.gameObject.GetComponentInParent<CharacterBase>();
            if (cBaseTemp && cBaseTemp.enabled) {
                bool check = true;
                int count = cBaseList.Count;
                for (int i = 0; i < count; i++) {
                    if (cBaseList[i] == cBaseTemp) {
                        check = false;
                        break;
                    }
                }
                if (check) {
                    worked = true;
                    cBaseList.Add(cBaseTemp);
                    if (!canDodge || !cBaseTemp.GetIsMuteki()) {
                        WorkEnter(cBaseList.Count - 1);
                    }
                    enabled = true;
                    if (onlyOnce) {
                        if (destroyParent && transform.parent != null) {
                            Destroy(transform.parent.gameObject, destroyDelay);
                        } else {
                            Destroy(gameObject, destroyDelay);
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag(targetTag)) {
            cBaseTemp = other.gameObject.GetComponentInParent<CharacterBase>();
            if (cBaseTemp && cBaseTemp.enabled) {
                for (int i = cBaseList.Count - 1; i >= 0; i--) {
                    if (cBaseList[i] == cBaseTemp) {
                        WorkExit(i);
                        cBaseList.RemoveAt(i);
                        break;
                    }
                }
                if (cBaseList.Count <= 0) {
                    enabled = false;
                }
            }
        }
    }

    protected virtual void WorkEnter(int index) {
    }    

    protected virtual void WorkExit(int index) {
    }

}
