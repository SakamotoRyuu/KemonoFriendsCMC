using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDisableColliders : MonoBehaviour
{

    public float timer;
    public Collider[] colliders;

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] != null)
                {
                    colliders[i].enabled = false;
                }
            }
            enabled = false;
        }
    }

}
