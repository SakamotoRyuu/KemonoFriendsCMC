using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePitchWithDistance : MonoBehaviour {

    public AudioSource sfxAudio;
    public float sfxStartDistance;
    public float sfxEndDistance;
    public float minPitch;
    public float maxPitch;
    int state;

    private void Update() {
        switch (state) {
            case 0:
                if (sfxAudio && CharacterManager.Instance.playerTrans && (CharacterManager.Instance.playerTrans.position - transform.position).sqrMagnitude <= sfxStartDistance * sfxStartDistance) {
                    sfxAudio.pitch = minPitch;
                    sfxAudio.Play();
                    state = 1;
                }
                break;
            case 1:
                if (sfxAudio && CharacterManager.Instance.playerTrans) {
                    float sqrDist = (CharacterManager.Instance.playerTrans.position - transform.position).sqrMagnitude;
                    if (sqrDist > sfxStartDistance * sfxStartDistance) {
                        sfxAudio.Stop();
                        sfxAudio.pitch = minPitch;
                        state = 0;
                    } else if (sqrDist <= sfxEndDistance * sfxEndDistance) {
                        if (sfxAudio.pitch != maxPitch) {
                            sfxAudio.pitch = maxPitch;
                        }
                    } else if (sfxStartDistance > sfxEndDistance) {
                        float dist = Vector3.Distance(CharacterManager.Instance.playerTrans.position, transform.position);
                        float pitchTemp = Mathf.Lerp(minPitch, maxPitch, Mathf.Clamp01(1f - (dist - sfxEndDistance) / (sfxStartDistance - sfxEndDistance)));
                        if (sfxAudio.pitch != pitchTemp) {
                            sfxAudio.pitch = pitchTemp;
                        }
                    }
                }
                break;
        }
    }
}
