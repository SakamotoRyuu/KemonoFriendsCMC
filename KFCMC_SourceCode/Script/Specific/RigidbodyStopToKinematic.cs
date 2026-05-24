using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyStopToKinematic : MonoBehaviour
{

    public Rigidbody rigid;
    public float delay = 0.8f;
    public float limitSpeed = 0.1f;
    public float duration = 0.1f;
    public GameObject prefabOnStop;
    public Transform prefabPivot;
    public Vector3 prefabOffset;
    public bool isPrefabInheritPivotRotation = true;

    float elapsedTime = 0f;
    int progress = 0;

    void Update()
    {
        if (progress == 0)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= delay)
            {
                elapsedTime = 0f;
                progress = 1;
            }
        }
        else if (progress == 1 && rigid)
        {
            if ((rigid.velocity).sqrMagnitude <= limitSpeed * limitSpeed)
            {
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= duration)
                {
                    rigid.collisionDetectionMode = CollisionDetectionMode.Discrete;
                    rigid.isKinematic = true;
                    rigid.useGravity = false;
                    progress = 2;
                    if (prefabOnStop)
                    {
                        GameObject instanceOnStop;
                        if (prefabPivot)
                        {
                            instanceOnStop = Instantiate(prefabOnStop, prefabPivot.position + prefabOffset, isPrefabInheritPivotRotation ? prefabPivot.rotation : Quaternion.identity);
                        }
                        else
                        {
                            instanceOnStop = Instantiate(prefabOnStop, transform.position + prefabOffset, isPrefabInheritPivotRotation ? transform.rotation : Quaternion.identity);
                        }
                        MissingObjectToDestroy missingChecker = instanceOnStop.GetComponent<MissingObjectToDestroy>();
                        if (missingChecker)
                        {
                            missingChecker.SetGameObject(gameObject);
                        }
                    }
                }
            }
            else
            {
                elapsedTime = 0f;
            }
        }
    }
}
