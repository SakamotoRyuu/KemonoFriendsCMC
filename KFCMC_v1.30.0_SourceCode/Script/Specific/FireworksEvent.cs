using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworksEvent : MonoBehaviour {

    public GameObject[] fireworksPrefab;
    public int lightingNumber = 27;
    public int ambientNumber = 12;

    Transform trans;
    float timer = 0f;
    int progress = 0;
    const int faceIndex = 132;
    const string talkName = "TALK_SERVAL_FIREWORKS";
    static readonly int[] fireCount = new int[] { 4, 4, 4, 7, 10 };

    void Awake() {
        trans = transform;
    }

    void SetFireworks() {
        int max = 1;
        if (GameManager.Instance.save.difficulty >= 0 && GameManager.Instance.save.difficulty < fireCount.Length) {
            max = fireCount[GameManager.Instance.save.difficulty];
        }
        for (int i = 0; i < max; i++) {
            float rand = 0f;
            if (i > 0) {
                rand = Random.Range(i * -1f, i * 1f);
            }
            Vector3 pos = new Vector3(trans.position.x + rand, trans.position.y + 0.5f, trans.position.z);
            int index = i % fireworksPrefab.Length;
            if (fireworksPrefab[index]) {
                Instantiate(fireworksPrefab[index], pos, Quaternion.identity);
            }
        }
        if (LightingDatabase.Instance) {
            LightingDatabase.Instance.SetLighting(lightingNumber);
            CharacterManager.Instance.SetPlayerLightActive();
        }
        if (Ambient.Instance) {
            Ambient.Instance.Play(ambientNumber);
        }
        if (CameraManager.Instance) {
            Vector3 cameraEventPos = trans.position + new Vector3(0f, 0.5f, -10f);
            Vector3 cameraFocusPos = trans.position;
            cameraFocusPos.y = 10f;
            CameraManager.Instance.SetEventCamera(cameraEventPos, new Vector3(-28f, 0f, 0f), 6f, 1.5f, Vector3.Distance(cameraFocusPos, cameraEventPos));
        }
        ItemCharacter[] ifrs = GameObject.FindObjectsOfType<ItemCharacter>();
        for (int i = 0; i < ifrs.Length; i++) {
            if (ifrs[i] && ifrs[i].isFriends && (ifrs[i].characterIndex == 26 || ifrs[i].characterIndex == 28)) {
                ifrs[i].ForceHomeTalkIndex(2);
            }
        }
    }
    
    void Update() {
        timer += Time.deltaTime;
        switch (progress) {
            case 0:
                if (timer >= 2f) {
                    progress = 1;
                    SetFireworks();
                }
                break;
            case 1:
                if (timer >= 4.5f) {
                    progress = 2;
                    if (CharacterManager.Instance) {
                        CharacterManager.Instance.SetSpecialChat(talkName, -1, -1);
                    }
                }
                break;
            case 2:
                if (timer >= 9f) {
                    progress = 3;
                    TrophyManager.Instance.CheckTrophy(TrophyManager.t_DefeatDummy);
                }
                break;
            case 3:
                if (timer >= 10f) {
                    progress = 4;
                    Destroy(gameObject);
                }
                break;
        }
    }
}
