using System.Collections;
using UnityEngine;

public class AutoDestroy_SaveChildParticles : AutoDestroy {

    protected override void Start() {
        StartCoroutine(DestroyAction());
    }

    IEnumerator DestroyAction() {
        yield return new WaitForSeconds(life);
        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        if (particles.Length > 0) {
            for (int i = 0; i < particles.Length; i++) {
                particles[i].transform.SetParent(null);
                particles[i].Stop();
            }
        }
        Destroy(gameObject);
    }

}
