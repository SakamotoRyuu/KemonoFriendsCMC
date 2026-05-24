using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemPool : SingletonMonoBehaviour<ParticleSystemPool>
{

    public enum ParticleName
    {
        None,
        FriendsDamagedS,
        FriendsDamagedM,
        FriendsDamagedL,
        EnemyDamaged,
        EnemyDamagedCritical,
        EnemyDamagedGuard,
        EnemyDamagedCriticalL,
        EnemyDamagedBigDog,
        EnemyDamagedIceS,
        EnemyDamagedIceL,
        EnemyDamagedCriticalHeat,
        ContainerBreak,
        ContainerBreakJewel,
        GetMoney,
        SandstarBlow,
        GetMemoryFragment
    };

    [System.Serializable]
    public class ParticleSet
    {
        public ParticleName name;
        public GameObject prefab;
        public int reserveNum;
        public int maxNum;
        public int capacity;
        public Quaternion rotation = Quaternion.identity;
        [System.NonSerialized]
        public List<HoldParticle> sourceList;
    }

    public ParticleSet[] particleSets;
    public Dictionary<ParticleName, int> particleDictionary;

    protected override void Awake() {
        if (CheckInstance()) {
            particleDictionary = new Dictionary<ParticleName, int>(particleSets.Length);
            for (int i = 0; i < particleSets.Length; i++) {
                particleSets[i].sourceList = new List<HoldParticle>(particleSets[i].capacity);
                particleDictionary.Add(particleSets[i].name, i);
            }
        }
    }


    private void Update() {
        for (int s = 0; s < particleSets.Length; s++) { 
            int count = particleSets[s].sourceList.Count;
            for (int i = count - 1; i >= 0; i--) {
                if (particleSets[s].sourceList[i] && particleSets[s].sourceList[i].gameObject.activeSelf && !particleSets[s].sourceList[i].parentParticle.isPlaying) {
                    if (particleSets[s].maxNum > 0 && count > particleSets[s].maxNum) {
                        Destroy(particleSets[s].sourceList[i].gameObject);
                        particleSets[s].sourceList.RemoveAt(i);
                        count--;
                    } else {
                        particleSets[s].sourceList[i].gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    public void Play(ParticleName particleName, Vector3 position, float multiplier = 1f) {
        if (particleName != ParticleName.None) {
            int index = particleDictionary[particleName];
            int count = particleSets[index].sourceList.Count;
            bool found = false;
            for (int i = 0; i < count; i++) {
                if (particleSets[index].sourceList[i] && !particleSets[index].sourceList[i].gameObject.activeSelf) {
                    particleSets[index].sourceList[i].transform.position = position;
                    particleSets[index].sourceList[i].SetParam(multiplier);
                    particleSets[index].sourceList[i].gameObject.SetActive(true);
                    particleSets[index].sourceList[i].parentParticle.Play();
                    found = true;
                    break;
                }
            }
            if (!found && count < particleSets[index].capacity) {
                HoldParticle holdParticle = Instantiate(particleSets[index].prefab, position, particleSets[index].rotation).GetComponent<HoldParticle>();
                holdParticle.SetParam(multiplier);
                holdParticle.parentParticle.Play();
                particleSets[index].sourceList.Add(holdParticle);
            }
        }
    }

    public void Reduce() {
        for (int s = 0; s < particleSets.Length; s++) { 
            int count = particleSets[s].sourceList.Count;
            if (count > particleSets[s].reserveNum) {
                for (int i = count - 1; i >= 0; i--) {
                    if (particleSets[s].sourceList[i] == null) {
                        particleSets[s].sourceList.RemoveAt(i);
                        count--;
                    }
                }
            }
            if (count > particleSets[s].reserveNum) {
                for (int i = count - 1; i >= 0 && count > particleSets[s].reserveNum; i--) {
                    if (!particleSets[s].sourceList[i].parentParticle.isPlaying) {
                        Destroy(particleSets[s].sourceList[i].gameObject);
                        particleSets[s].sourceList.RemoveAt(i);
                        count--;
                    }
                }
            }
        }
    }

    public void StopAll() {
        for (int s = 0; s < particleSets.Length; s++) { 
            int count = particleSets[s].sourceList.Count;
            for (int i = 0; i < count; i++) {
                if (particleSets[s].sourceList[i].parentParticle) {
                    particleSets[s].sourceList[i].parentParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                }
            }
        }
    }

}


