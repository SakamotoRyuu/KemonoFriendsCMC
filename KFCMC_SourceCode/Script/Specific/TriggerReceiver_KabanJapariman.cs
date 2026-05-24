using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemDatabase;

public class TriggerReceiver_KabanJapariman: TriggerReceiver
{

    Rigidbody rb;
    bool isGrounded;
    bool isAte;
    bool isDestroyed;
    float elapsedTime;

    const int groundIndex = 0;
    const int playerIndex = 1;
    readonly int[] japarimanItemId = { (int)ItemID.Japariman };

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!isDestroyed && rb && !rb.isKinematic && Time.timeScale > 0)
        {
            elapsedTime += Time.deltaTime;
            if (!CharacterManager.Instance.GetFriendsExist(CharacterManager.friendsID_Serval, false) && !CharacterManager.Instance.GetFriendsExist(CharacterManager.friendsID_HyperServal, false))
            {
                if (isGrounded && elapsedTime >= 0.1f)
                {
                    Vector3 position = transform.position;
                    Ray ray = new Ray(transform.position, Vector3.down);
                    RaycastHit hitInfo;
                    if (Physics.Raycast(ray, out hitInfo, 1f, CharacterManager.friendsAppearPosLayerMask, QueryTriggerInteraction.Ignore))
                    {
                        position = hitInfo.point;
                    }
                    CharacterManager.Instance.CallServal(position);
                    Destroy(gameObject);
                    isDestroyed = true;
                }
            }
            else
            {
                if (isAte)
                {
                    CharacterManager.Instance.CallServal(transform.position);
                    Destroy(gameObject);
                    isDestroyed = true;
                }
                else if (isGrounded && elapsedTime >= 1f && (rb.IsSleeping() || rb.velocity.sqrMagnitude < 0.5f))
                {
                    ItemDatabase.Instance.GiveItem(japarimanItemId, transform);
                    Destroy(gameObject);
                    isDestroyed = true;
                }

            }
        }
    }

    public override void Receive(int index, Collider other)
    {
        if (!isDestroyed)
        {
            if (index == playerIndex && other.CompareTag("Player"))
            {
                FriendsBase fBase = other.GetComponent<FriendsBase>();
                if (fBase && CharacterManager.IsServalID(fBase.friendsId))
                {
                    isAte = true;
                }
            }
            if (index == groundIndex && !rb.isKinematic)
            {
                isGrounded = true;
            }
        }
    }

}
