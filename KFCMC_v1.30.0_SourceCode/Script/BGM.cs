using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
using E7.Introloop;

public class BGM : SingletonMonoBehaviour<BGM> {    
        
    public IntroloopPlayer introloopPlayer;
    public IntroloopAudio[] introloopAudio;

    [System.NonSerialized]
    public string[] musicModTitle;
    [System.NonSerialized]
    public string[] musicModCaption;
    [System.NonSerialized]
    public bool musicModInfoChanged;
    [System.NonSerialized]
    public float masterVolumeRate = 1f;

    private int playingIndex = -1;
    private bool autoReplayEnabled;

    protected override void Awake() {
        if (CheckInstance()) {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start() {
        LoadMod();
    }

    private void Update() {
        if (autoReplayEnabled && playingIndex >= 0 && GetPlayingAudioSource() == null) {
            int playSave = playingIndex;
            playingIndex = -1;
            Play(playSave, 0f, true);
        }
    }

    public IEnumerator LoadFile(string fileName, AudioType audioType, int musicIndex, int linesCount) {
        bool modError = false;
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(fileName, audioType);
        yield return www.SendWebRequest();
        if (www.isHttpError || www.isNetworkError) {
            modError = true;
        } else {
            AudioClip audioClip = ((DownloadHandlerAudioClip)www.downloadHandler).audioClip;
            if (audioClip == null) {
                modError = true;
            } else {
                introloopAudio[musicIndex].SetModClip(audioClip);
            }
            if (modError && GameManager.Instance) {
                GameManager.Instance.SetModError(0, linesCount);
            }
        }
    }

    private void LoadMod() {
        string directoryPath = Application.dataPath + "/mods";
        if (Directory.Exists(directoryPath)) {
            bool foundFile = false;
            char[] charSeparators = new char[] { '\t' };
            string csvPath = directoryPath + "/music.csv";
            if (File.Exists(csvPath)) {
                foundFile = true;
            } else {
                csvPath = directoryPath + "/music.txt";
                if (File.Exists(csvPath)) {
                    foundFile = true;
                }
            }
            if (foundFile) {
                FileInfo fi = new FileInfo(csvPath);
                try {
                    using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8)) {
                        string refPath = directoryPath + "/";
                        int musicIndex = -1;
                        int linesCount = 0;
                        if (GameManager.Instance) {
                            GameManager.Instance.modFlag = true;
                            GameManager.Instance.musicModFlag = true;
                        }
                        while (sr.Peek() > -1) {
                            linesCount++;
                            bool modError = false;
                            string[] values = sr.ReadLine().Split(charSeparators, 3, System.StringSplitOptions.None);
                            if (values.Length > 0 && !string.IsNullOrEmpty(values[0])) {
                                if (values.Length < 2) {
                                    modError = true;
                                } else {
                                    switch (values[0]) {
                                        case "INDEX":
                                            if (int.TryParse(values[1], out int indexTemp)) {
                                                musicIndex = indexTemp - 1;
                                                if (musicIndex >= 0 && musicIndex < introloopAudio.Length) {
                                                    introloopAudio[musicIndex].Volume = 1f;
                                                    introloopAudio[musicIndex].Pitch = 1f;
                                                    introloopAudio[musicIndex].nonLooping = false;
                                                    introloopAudio[musicIndex].loopWholeAudio = true;
                                                } else {
                                                    modError = true;
                                                }
                                            } else {
                                                modError = true;
                                            }
                                            break;
                                        case "FILENAME":
                                            if (musicIndex >= 0 && musicIndex < introloopAudio.Length) {
                                                string fileName = refPath + values[1];
                                                if (!File.Exists(fileName)) {
                                                    modError = true;
                                                } else {
                                                    StartCoroutine(LoadFile(fileName, AudioType.OGGVORBIS, musicIndex, linesCount));
                                                }
                                            } else {
                                                modError = true;
                                            }
                                            break;
                                        case "TITLE":
                                            if (musicIndex >= 0 && musicIndex < introloopAudio.Length) {
                                                if (!musicModInfoChanged) {
                                                    musicModTitle = new string[introloopAudio.Length];
                                                    musicModCaption = new string[introloopAudio.Length];
                                                    musicModInfoChanged = true;
                                                }
                                                musicModTitle[musicIndex] = values[1];
                                            } else {
                                                modError = true;
                                            }
                                            break;
                                        case "CAPTION":
                                            if (musicIndex >= 0 && musicIndex < introloopAudio.Length) {
                                                if (!musicModInfoChanged) {
                                                    musicModTitle = new string[introloopAudio.Length];
                                                    musicModCaption = new string[introloopAudio.Length];
                                                    musicModInfoChanged = true;
                                                }
                                                musicModCaption[musicIndex] = values[1];
                                            } else {
                                                modError = true;
                                            }
                                            break;
                                        case "VOLUME":
                                            if (musicIndex >= 0 && musicIndex < introloopAudio.Length && float.TryParse(values[1], out float volumeTemp)) {
                                                introloopAudio[musicIndex].Volume = Mathf.Clamp01(volumeTemp);
                                            } else {
                                                modError = true;
                                            }
                                            break;
                                        case "PITCH":
                                            if (musicIndex >= 0 && musicIndex < introloopAudio.Length && float.TryParse(values[1], out float pitchTemp)) {
                                                introloopAudio[musicIndex].Pitch = Mathf.Clamp(pitchTemp, 0.1f, 3f);
                                            } else {
                                                modError = true;
                                            }
                                            break;
                                        case "LOOPTYPE":
                                            if (musicIndex >= 0 && musicIndex < introloopAudio.Length) {
                                                switch (values[1]) {
                                                    case "INTROLOOP":
                                                        introloopAudio[musicIndex].nonLooping = false;
                                                        introloopAudio[musicIndex].loopWholeAudio = false;
                                                        break;
                                                    case "LOOP":
                                                        introloopAudio[musicIndex].nonLooping = false;
                                                        introloopAudio[musicIndex].loopWholeAudio = true;
                                                        break;
                                                    case "NONLOOPING":
                                                        introloopAudio[musicIndex].nonLooping = true;
                                                        introloopAudio[musicIndex].loopWholeAudio = false;
                                                        break;
                                                    default:
                                                        modError = true;
                                                        break;
                                                }
                                            } else {
                                                modError = true;
                                            }
                                            break;
                                        case "INTROBOUNDARY":
                                            if (musicIndex >= 0 && musicIndex < introloopAudio.Length && float.TryParse(values[1], out float introBoundaryTemp)) {
                                                introloopAudio[musicIndex].introBoundary = introBoundaryTemp;
                                            } else {
                                                modError = true;
                                            }
                                            break;
                                        case "LOOPINGBOUNDARY":
                                            if (musicIndex >= 0 && musicIndex < introloopAudio.Length && float.TryParse(values[1], out float loopingBoundaryTemp)) {
                                                introloopAudio[musicIndex].loopingBoundary = loopingBoundaryTemp;
                                            } else {
                                                modError = true;
                                            }
                                            break;
                                        case "MASTERVOLUME":
                                            if (float.TryParse(values[1], out float masterVolumeTemp)) {
                                                masterVolumeRate = Mathf.Clamp(masterVolumeTemp, 0f, 3f);
                                            } else {
                                                modError = true;
                                            }
                                            break;
                                        default:
                                            modError = true;
                                            break;
                                    }
                                }
                            }
                            if (modError && GameManager.Instance) {
                                GameManager.Instance.SetModError(GameManager.ModType.Music, linesCount);
                            }
                        }
                    }
                } catch {
                    GameManager.Instance.SetModError(GameManager.ModType.Music, 0);
                }
            }
        }
    }

    public int GetPlayingIndex() {
        return playingIndex;
    }

    public void Play(int index, float fadeTime = 0.3f, bool autoReplay = false) {
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
        autoReplayEnabled = autoReplay;
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

    public AudioSource GetPlayingAudioSource() {
        return introloopPlayer.GetPlayingAudioSource();
    }

}
