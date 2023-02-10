using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISE : SingletonMonoBehaviour<UISE> {

    [System.Serializable]
    public class SoundEffect {
        public AudioClip open;
        public AudioClip close;
        public AudioClip move;
        public AudioClip cancel;
        public AudioClip submit;
        public AudioClip use;
        public AudioClip error;
        public AudioClip pay;
        public AudioClip page;
        public AudioClip bridge;
        public AudioClip caution;
        public AudioClip delete;
        public AudioClip aimon;
        public AudioClip aimoff;
        public AudioClip camera;
        public AudioClip photoModeStart;
        public AudioClip photoModeEnd;
        public AudioClip trophy;
    }

    public enum SoundName { open, close, move, cancel, submit, use, error, pay, page, bridge, caution, delete, aimon, aimoff, camera, photoModeStart, photoModeEnd, trophy };

    public SoundEffect se;
    public AudioSource audioSource;

    const int seMax = 18;
    private bool[] seFlags = new bool[seMax];

    protected override void Awake() {
        if (CheckInstance()) {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void Play(SoundName seName) {
        int index = (int)seName;
        if (seFlags[index] == false) {
            seFlags[index] = true;
            switch (seName) {
                case SoundName.open:
                    audioSource.PlayOneShot(se.open);
                    break;
                case SoundName.close:
                    audioSource.PlayOneShot(se.close);
                    break;
                case SoundName.move:
                    audioSource.PlayOneShot(se.move);
                    break;
                case SoundName.cancel:
                    audioSource.PlayOneShot(se.cancel);
                    break;
                case SoundName.submit:
                    audioSource.PlayOneShot(se.submit);
                    break;
                case SoundName.use:
                    audioSource.PlayOneShot(se.use);
                    break;
                case SoundName.error:
                    audioSource.PlayOneShot(se.error);
                    break;
                case SoundName.pay:
                    audioSource.PlayOneShot(se.pay);
                    break;
                case SoundName.page:
                    audioSource.PlayOneShot(se.page);
                    break;
                case SoundName.bridge:
                    audioSource.PlayOneShot(se.bridge);
                    break;
                case SoundName.caution:
                    audioSource.PlayOneShot(se.caution);
                    break;
                case SoundName.delete:
                    audioSource.PlayOneShot(se.delete);
                    break;
                case SoundName.aimon:
                    audioSource.PlayOneShot(se.aimon);
                    break;
                case SoundName.aimoff:
                    audioSource.PlayOneShot(se.aimoff);
                    break;
                case SoundName.camera:
                    audioSource.PlayOneShot(se.camera);
                    break;
                case SoundName.photoModeStart:
                    audioSource.PlayOneShot(se.photoModeStart);
                    break;
                case SoundName.photoModeEnd:
                    audioSource.PlayOneShot(se.photoModeEnd);
                    break;
                case SoundName.trophy:
                    audioSource.PlayOneShot(se.trophy);
                    break;
            }
        }
    }

    private void LateUpdate() {
        for (int i = 0; i < seMax; i++) {
            seFlags[i] = false;
        }
    }

}
