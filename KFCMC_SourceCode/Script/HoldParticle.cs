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
    public ParticleSystem parentParticle;

    public virtual void SetParam(float multiplier) {
        for (int i = 0; i < pSets.Length; i++) {
            if (pSets[i].particle) {
                var emissionModule = pSets[i].particle.emission;
                var particleBurst = emissionModule.GetBurst(0);
                particleBurst.count = Mathf.Clamp((int)(multiplier * pSets[i].multiplier) + 1, pSets[i].minCount, pSets[i].maxCount);
                emissionModule.SetBurst(0, particleBurst);
            }
        }
    }

}
