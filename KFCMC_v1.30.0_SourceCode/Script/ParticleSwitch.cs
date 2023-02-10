using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSwitch : MonoBehaviour {

    [System.Serializable]
    public class ParticleSet {
        public ParticleSystem particle;
        public int number;
    }

    public ParticleSet[] particleSet;

	// Use this for initialization
	void Start () {
		for (int i = 0; i < particleSet.Length; i++) {
            particleSet[i].particle.Stop();
            particleSet[i].particle.Clear();
        }
	}

    public void SetParticle(int number) {
        for (int i = 0; i < particleSet.Length; i++) {
            if (particleSet[i].number == number && !particleSet[i].particle.isPlaying) {
                particleSet[i].particle.Play();
            } else if (particleSet[i].number != number && !particleSet[i].particle.isStopped) {
                particleSet[i].particle.Stop();
            }
        }
    }
}
