using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using E7.Introloop;

public class Ambient : SingletonMonoBehaviour<Ambient> {

    public IntroloopPlayer introloopPlayer;
    public IntroloopAudio[] introloopAudio;
    private int playingIndex = -1;

    protected override void Awake() {
        if (CheckInstance()) {
            DontDestroyOnLoad(gameObject);
        }
    }

    public int GetPlayingIndex() {
        return playingIndex;
    }

    public void Play(int index, float fadeTime = 0.3f) {
        if (introloopPlayer != null) {
            if (index >= 0) {
                if (introloopAudio.Length > index && introloopAudio[index] != null && playingIndex != index) {
                    introloopPlayer.Play(introloopAudio[index], fadeTime);
                }
            } else {
                introloopPlayer.Stop(fadeTime);
            }
            playingIndex = index;
        }
    }

    public void Replay() {
        int playSave = playingIndex;
        playingIndex = -1;
        Play(playSave);
    }

    public void Stop() {
        introloopPlayer.Stop();
        playingIndex = -1;
    }
    
    public void StopFade(float fadeTime) {
        introloopPlayer.Stop(fadeTime);
        playingIndex = -1;
    }


}
