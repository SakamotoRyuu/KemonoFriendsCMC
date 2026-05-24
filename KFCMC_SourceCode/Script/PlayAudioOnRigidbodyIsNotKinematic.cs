using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioOnRigidbodyIsNotKinematic : MonoBehaviour
{

    public Rigidbody rb;
    public AudioSource audioSource;
    public float playTime;

    void Update()
    {
        if (rb && audioSource && !rb.isKinematic && !audioSource.isPlaying)
        {
            audioSource.time = playTime;
            audioSource.Play();
            enabled = false;
        }
    }
}
