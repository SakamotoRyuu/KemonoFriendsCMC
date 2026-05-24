using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealStar : MonoBehaviour {

    public enum HealType { HP, Sandstar };

    public GameObject balloon;
    public Vector3 balloonOffset;
    public float getDelay = 0.3f;
    public float balloonDelay = 0.3f;
    public HealType healType;

    [System.NonSerialized]
    public bool getEnable;

    private float duration = 0;
    bool balloonActivated = false;

    protected virtual void SetMapChip() {
        Instantiate(MapDatabase.Instance.prefab[MapDatabase.item], transform);
    }

    private void Update() {
        if (duration < getDelay || duration < balloonDelay) {
            duration += Time.deltaTime;
            if (!getEnable && duration >= getDelay) {
                getEnable = true;
            }
            if (!balloonActivated && duration >= balloonDelay) {
                if (balloon) {
                    Instantiate(balloon, transform.position + balloonOffset, Quaternion.identity, transform);
                }
                balloonActivated = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (getEnable && other.CompareTag("ItemGetter") && CharacterManager.Instance) {
            switch (healType) {
                case HealType.HP:
                    CharacterManager.Instance.Heal(80, 20, (int)EffectDatabase.id.itemHeal01, true, true, true, true, true);
                    break;
                case HealType.Sandstar:
                    CharacterManager.Instance.AddSandstar(1.25f, true, (int)EffectDatabase.id.itemSandstar, true, true);
                    break;
            }
            Destroy(gameObject);
        }
    }

}
