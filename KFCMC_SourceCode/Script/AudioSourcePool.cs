using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourcePool : SingletonMonoBehaviour<AudioSourcePool>
{

    public enum AudioName
    {
        None,
        PlayerAttack,
        FriendsAttack,
        PlayerDamagedS,
        PlayerDamagedM,
        PlayerDamagedL,
        FriendsDamagedS,
        FriendsDamagedM,
        FriendsDamagedL,
        EnemyDamaged,
        EnemyDamagedL1,
        EnemyDamagedL2,
        EnemyDamagedCritical,
        EnemyDamagedCriticalL1,
        EnemyDamagedCriticalL2,
        EnemyDamagedGuard,
        DigGround,
        BallBounce,
        ContainerBreak,
        GetItem,
        GetItemImportant,
        GetMoney,
        SandstarBlow,
        EnemyDead,
        WallBreak,
        GetMemoryFragment
    };

    [System.Serializable]
    public class AudioSet
    {
        public AudioName name;
        public GameObject prefab;
        public int reserveNum;
        public int maxNum;
        public int capacity;
        public CharacterManager.LoudSoundType loudSoundType;
        public float loudSoundDecreaseAmount;
        [System.NonSerialized]
        public List<AudioSource> sourceList;
    }

    public AudioSet[] audioSets;
    public Dictionary<AudioName, int> audioDictionary;
    static readonly Quaternion quaIden = Quaternion.identity;

    protected override void Awake() {
        if (CheckInstance()) {
            audioDictionary = new Dictionary<AudioName, int>(audioSets.Length);
            for (int i = 0; i < audioSets.Length; i++) {
                audioSets[i].sourceList = new List<AudioSource>(audioSets[i].capacity);
                audioDictionary.Add(audioSets[i].name, i);
            }
        }
    }

    private void Update() {
        for (int s = 0; s < audioSets.Length; s++) { 
            int count = audioSets[s].sourceList.Count;
            for (int i = count - 1; i >= 0; i--) {
                if (audioSets[s].sourceList[i] && audioSets[s].sourceList[i].gameObject.activeSelf && !audioSets[s].sourceList[i].isPlaying) {
                    if (audioSets[s].maxNum > 0 && count > audioSets[s].maxNum) {
                        Destroy(audioSets[s].sourceList[i].gameObject);
                        audioSets[s].sourceList.RemoveAt(i);
                        count--;
                    } else {
                        audioSets[s].sourceList[i].gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    public void Play(AudioName audioName, Vector3 position, float volume = 1f) {
        if (audioName != AudioName.None) {
            int index = audioDictionary[audioName];
            int count = audioSets[index].sourceList.Count;
            bool found = false;
            if (audioSets[index].loudSoundDecreaseAmount > 0f) {
                volume *= CharacterManager.Instance.GetLoudSoundVolumeRate(audioSets[index].loudSoundType, audioSets[index].loudSoundDecreaseAmount);
            }
            if (volume > 0f) {
                for (int i = 0; i < count; i++) {
                    if (audioSets[index].sourceList[i] && !audioSets[index].sourceList[i].gameObject.activeSelf) {
                        audioSets[index].sourceList[i].transform.position = position;
                        audioSets[index].sourceList[i].volume = volume;
                        audioSets[index].sourceList[i].gameObject.SetActive(true);
                        audioSets[index].sourceList[i].Play();
                        found = true;
                        break;
                    }
                }
                if (!found && count < audioSets[index].capacity) {
                    AudioSource audioSource = Instantiate(audioSets[index].prefab, position, quaIden).GetComponent<AudioSource>();
                    audioSource.volume = volume;
                    audioSource.Play();
                    audioSets[index].sourceList.Add(audioSource);
                }
            }
        }
    }

    public void Reduce() {
        for (int s = 0; s < audioSets.Length; s++) { 
            int count = audioSets[s].sourceList.Count;
            if (count > audioSets[s].reserveNum) {
                for (int i = count - 1; i >= 0; i--) {
                    if (audioSets[s].sourceList[i] == null) {
                        audioSets[s].sourceList.RemoveAt(i);
                        count--;
                    }
                }
            }
            if (count > audioSets[s].reserveNum) {
                for (int i = count - 1; i >= 0 && count > audioSets[s].reserveNum; i--) {
                    if (!audioSets[s].sourceList[i].isPlaying) {
                        Destroy(audioSets[s].sourceList[i].gameObject);
                        audioSets[s].sourceList.RemoveAt(i);
                        count--;
                    }
                }
            }
        }
    }

    public void StopAll() {
        for (int s = 0; s < audioSets.Length; s++) { 
            int count = audioSets[s].sourceList.Count;
            for (int i = 0; i < count; i++) {
                if (audioSets[s].sourceList[i]) {
                    audioSets[s].sourceList[i].Stop();
                } 
            }
        }
    }

}


