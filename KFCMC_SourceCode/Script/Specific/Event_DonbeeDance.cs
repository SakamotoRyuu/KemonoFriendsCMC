using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_DonbeeDance : MonoBehaviour
{
    public Transform characterPivot;
    public Transform cameraPivot;
    public GameObject mapChip;

    float stayTime;
    int progress;
    Friends_SilverFox silverFox;
    const string targetTag = "Player";
    const int silverFoxIndex = 22;
    const int redFoxIndex = 21;

    private void OnTriggerEnter(Collider other) {
        if (progress <= 0 && other.CompareTag(targetTag) && CharacterManager.Instance.GetFriendsExist(silverFoxIndex, true)) {
            silverFox = other.GetComponent<Friends_SilverFox>();
            if (silverFox) {
                silverFox.ForceStopForEvent(7f);
                silverFox.Warp(characterPivot.position, 7f, 0f);
                silverFox.transform.rotation = characterPivot.rotation;
                Vector3 characterFacePos = characterPivot.position;
                characterFacePos.y += 1.1f;
                CameraManager.Instance.SetEventCamera(cameraPivot.position, cameraPivot.eulerAngles, 7f, 1.5f, Vector3.Distance(characterFacePos, cameraPivot.position));
                progress = 1;
            }
        }
    }

    private void Update() {
        if (progress >= 1 && progress < 4) {
            if (CharacterManager.Instance.GetFriendsExist(silverFoxIndex, true)) {
                stayTime += Time.deltaTime;
                if (stayTime < 6.5f) {
                    HoldFox();
                }
                if (progress == 1 && stayTime >= 1.5f) {
                    silverFox.SetDance();
                    progress = 2;
                }
                if (progress == 2 && stayTime >= 6.5f) {
                    CharacterManager.Instance.SetSpecialChat("EVENT_SILVERFOX_09", silverFoxIndex, -1);
                    progress = 3;
                }
                if (progress == 3 && stayTime >= 9.5f) {
                    if (CharacterManager.Instance.GetFriendsExist(redFoxIndex, true)) {
                        CharacterManager.Instance.SetSpecialChat("EVENT_REDFOX_09", redFoxIndex, -1);
                    }
                    TrophyManager.Instance.CheckTrophy(TrophyManager.t_Fukkura, true);
                    if (mapChip) {
                        mapChip.SetActive(false);
                    }
                    progress = 4;
                }
            } else {
                progress = 4;
            }
        }
    }

    void HoldFox() {
        if (silverFox) {
            Vector3 vecTemp = characterPivot.position;
            vecTemp.y = silverFox.transform.position.y;
            if (silverFox.transform.position != vecTemp) {
                silverFox.transform.position = vecTemp;
            }
        }
    }

}
