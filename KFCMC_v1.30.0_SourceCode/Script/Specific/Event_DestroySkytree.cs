using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_DestroySkytree : MonoBehaviour {

    public AudioSource audioSource;
    int progress;
    float elapsedTime;

    void Update() {
        elapsedTime += Time.deltaTime;
        switch (progress) {
            case 0:
                if (elapsedTime >= 5f) {
                    CameraManager.Instance.SetQuake(Vector3.zero, 2, 8, 2f, 5f, 2f, 100f, 200f);
                    if (audioSource) {
                        audioSource.Play();
                    }
                    elapsedTime = 0f;
                    progress++;
                }
                break;
            case 1:
                if (elapsedTime >= 2) {
                    elapsedTime = 0f;
                    progress++;
                }
                break;
            case 2:
                PauseController.Instance.SetBlackCurtain(Mathf.Clamp01(elapsedTime * 0.5f), true);
                if (audioSource) {
                    audioSource.volume = Mathf.Clamp01(1f - elapsedTime * 0.5f);
                }
                if (elapsedTime >= 2f) {
                    CameraManager.Instance.CancelQuake();
                    if (audioSource) {
                        audioSource.Stop();
                    }
                    elapsedTime = 0f;
                    progress++;
                }
                break;
            case 3:
                if (elapsedTime >= 0.25f) {
                    StageManager.Instance.dungeonMother.MoveFloor(1, -1, true);
                    transform.parent = StageManager.Instance.dungeonController.transform;
                    elapsedTime = 0f;
                    progress++;
                }
                break;
            case 4:
                if (CharacterManager.Instance.pCon) {
                    CharacterManager.Instance.pCon.Event_PlayerJumping(0f, 0);
                }
                TrophyManager.Instance.CheckTrophy_Clear();
                GameManager.Instance.SetSteamAchievement("CLEARSKYTREE");
                elapsedTime = 0f;
                progress++;
                break;
            case 5:
                PauseController.Instance.SetBlackCurtain(Mathf.Clamp01(1f - elapsedTime * 0.5f), true);
                if (elapsedTime >= 2f) {
                    elapsedTime = 0f;
                    progress++;
                    Destroy(gameObject);
                }
                break;
        }
    }

    private void OnDestroy() {
        if (PauseController.Instance) {
            PauseController.Instance.SetBlackCurtain(0f, true);
        }
    }
}
