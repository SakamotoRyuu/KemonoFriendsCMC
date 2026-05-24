using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitsunezakiIneko_Ring : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform attackCollidersParent;
    public float startRadius;
    public float radiusStretchSpeed;
    public float lifeTime;
    public float fadeOutStartTime;
    public Color startColor;
    public Color endColor;

    private Transform[] attackColliderPivots;
    private Transform[] attackColliderBodies;
    private CapsuleCollider[] attackColliderCapsules;
    private float elapsedTime;
    private bool isFadeOutStarted;
    
    private void Start()
    {
        int childCount = attackCollidersParent.childCount;
        float angleInterval = 360f / childCount;
        Vector3 pivotEulerAngles = Vector3.zero;
        Vector3 bodyEulerAngles = new Vector3(0f, angleInterval * 0.5f, 0f);
        attackColliderPivots = new Transform[childCount];
        attackColliderBodies = new Transform[childCount];
        attackColliderCapsules = new CapsuleCollider[childCount];
        for (int i = 0; i < childCount; i++)
        {
            attackColliderPivots[i] = attackCollidersParent.GetChild(i);
            attackColliderBodies[i] = attackColliderPivots[i].GetChild(0);
            attackColliderCapsules[i] = attackColliderBodies[i].GetComponent<CapsuleCollider>();
        }
        for (int i = 0; i < childCount; i++)
        {
            attackColliderPivots[i].localEulerAngles = pivotEulerAngles;
            attackColliderBodies[i].localEulerAngles = bodyEulerAngles;
            pivotEulerAngles.y += angleInterval;
        }
    }

    private void FixedUpdate()
    {
        elapsedTime += Time.fixedDeltaTime;
        Vector3 bodyPosition = Vector3.zero;
        float radius = startRadius + radiusStretchSpeed * elapsedTime;
        bodyPosition.z = radius;
        float lengthOfOneSide = radius * 2 * Mathf.PI / attackColliderBodies.Length;
        Vector3 centerShift = Vector3.zero;
        centerShift.x = lengthOfOneSide * 0.5f;
        float height = lengthOfOneSide + attackColliderCapsules[0].radius * 2f;
        for (int i = 0; i < attackColliderBodies.Length; i++)
        {
            attackColliderBodies[i].localPosition = bodyPosition;
            lineRenderer.SetPosition(i, attackColliderBodies[i].position);
        }
        lineRenderer.SetPosition(attackColliderBodies.Length, attackColliderBodies[0].position);
        for (int i = 0; i < attackColliderCapsules.Length; i++)
        {
            attackColliderCapsules[i].center = centerShift;
            attackColliderCapsules[i].height = height;
        }
        if (elapsedTime >= lifeTime)
        {
            Destroy(gameObject);
        }
        else if (elapsedTime >= fadeOutStartTime)
        {
            if (!isFadeOutStarted)
            {
                isFadeOutStarted = true;
                for (int i = 0; i < attackColliderCapsules.Length; i++)
                {
                    attackColliderCapsules[i].enabled = false;
                }
            }
            lineRenderer.material.color = Color.Lerp(startColor, endColor, (elapsedTime - fadeOutStartTime) / (lifeTime - fadeOutStartTime));
        }
    }

}
