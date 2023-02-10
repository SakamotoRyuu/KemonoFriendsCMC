using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldParticle : MonoBehaviour {

    [System.Serializable]
    public struct ParticleSet {
        public ParticleSystem particle;
        public float multiplier;
        public int minCount;
        public int maxCount;
    }

    public ParticleSet[] pSets;
    public bool playManual;
    public ParticleSystem parentParticle;

}
