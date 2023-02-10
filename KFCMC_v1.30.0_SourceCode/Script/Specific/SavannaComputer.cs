using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavannaComputer : MonoBehaviour {

    public int progress;
    public GameObject destroyReference;
    public GameObject balloon;
    public GameObject displayObj;
    public int targetMatIndex;
    public Material[] changeMat;
    public float[] changeMatTime;
    public string messageKey;
    
    public AudioSource aSrcStart;
    public AudioSource aSrcLoop;
    public float loopStartTime;

    public bool missionCompleted;

    GameObject balloonIns;
    GameObject mapChip;
    float elapsedTime;
    int pauseWait;
    int state = 0;
    int nextMat = 0;
    Renderer displayRend;
    Material[] rendMaterials;
    const string targetTag = "ItemGetter";

    // Use this for initialization
    void Start() {
        elapsedTime = 0;
        state = 0;
        missionCompleted = false;
        displayRend = displayObj.GetComponent<Renderer>();
        rendMaterials = displayRend.materials;
    }
	
	// Update is called once per frame
	void Update () {
        switch (state) {
            case 0:
                if (!destroyReference) {
                    state = 1;
                    if (balloon) {
                        balloonIns = Instantiate(balloon, transform);
                    }
                    mapChip = Instantiate(MapDatabase.Instance.prefab[MapDatabase.other], transform);
                }
                break;
            case 1:
                break;
            case 2:
                if (PauseController.Instance) {
                    if (PauseController.Instance.pauseGame) {
                        pauseWait = 2;
                    } else if (pauseWait > 0) {
                        pauseWait--;
                    }
                    if (pauseWait <= 0 && PauseController.Instance.pauseEnabled && GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit)) {
                        state = 3;
                        missionCompleted = true;
                        aSrcStart.Play();
                        GameManager.Instance.save.SetClearStage(progress);
                        if (balloonIns) {
                            Destroy(balloonIns);
                        }
                        if (mapChip) {
                            mapChip.SetActive(false);
                        }
                        if (CharacterManager.Instance) {
                            CharacterManager.Instance.SetActionType(CharacterManager.ActionType.None, gameObject);
                        }
                        if (!string.IsNullOrEmpty(messageKey)) {
                            MessageUI.Instance.SetMessage(TextManager.Get(messageKey), MessageUI.time_Important, MessageUI.panelType_Information, MessageUI.slotType_Lucky);
                        }
                    }
                }
                break;
            case 3:
                elapsedTime += Time.deltaTime;
                if (nextMat < changeMatTime.Length && elapsedTime >= changeMatTime[nextMat]) {
                    rendMaterials[targetMatIndex] = changeMat[nextMat];
                    displayRend.materials = rendMaterials;
                    nextMat++;
                }
                if (elapsedTime >= loopStartTime) {
                    aSrcStart.Stop();
                    aSrcLoop.Play();
                }
                break;
        }
	}

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(targetTag) && state == 1) {
            state = 2;
            if (CharacterManager.Instance) {
                CharacterManager.Instance.SetActionType(CharacterManager.ActionType.Search, gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag(targetTag) && state == 2) {
            state = 1;
            if (CharacterManager.Instance) {
                CharacterManager.Instance.SetActionType(CharacterManager.ActionType.None, gameObject);
            }
        }
    }
}
