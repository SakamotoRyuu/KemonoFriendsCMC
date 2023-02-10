using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetItem : MonoBehaviour {

    [System.Serializable]
    public enum ItemType { Normal, Important, HealHP, HealSandstar};

    public int id;
    public ItemType itemType;
    public GameObject balloon;
    public Vector3 balloonOffset;
    public float getDelay = 0.3f;
    public float balloonDelay = 0.3f;

    [System.NonSerialized]
    public bool getEnable;

    private float duration = 0;
    bool balloonActivated = false;    

    private void Start() {
        SetMapChip();
        getEnable = false;
    }

    public virtual void GetProcess() {
        getEnable = false;
    }

    public virtual void TouchProcess() { }
    
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
}
