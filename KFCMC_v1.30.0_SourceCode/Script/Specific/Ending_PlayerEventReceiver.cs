using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ending_PlayerEventReceiver : MonoBehaviour {
    
    public PlayerController.JumpEffectSetting hyperJumpEffect;

    void PlayHyperEffect() {
        if (hyperJumpEffect.enabled) {
            if (hyperJumpEffect.audioSource) {
                if (hyperJumpEffect.audioSource.isPlaying) {
                    hyperJumpEffect.audioSource.Stop();
                }
                hyperJumpEffect.audioSource.Play();
            }
            for (int i = 0; i < hyperJumpEffect.particles.Length; i++) {
                if (hyperJumpEffect.particles[i]) {
                    hyperJumpEffect.particles[i].Play();
                }
            }
        }
    }

}
