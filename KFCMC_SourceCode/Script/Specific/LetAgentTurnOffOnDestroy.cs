using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LetAgentTurnOffOnDestroy : MonoBehaviour
{

    public float radius = 0.5f;
    public float heightLimit = 0f;
    public bool forOnDisable;

    void AgentTurnOff()
    {
        if (CharacterManager.Instance)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            Vector3 myPosition = transform.position;
            for (int i = 0; i < players.Length; i++)
            {
                if ((players[i].transform.position - myPosition).sqrMagnitude <= radius * radius && 
                    (heightLimit <= 0f || Mathf.Abs(players[i].transform.position.y - myPosition.y) <= heightLimit))
                {
                    CharacterBase cBase = players[i].GetComponent<CharacterBase>();
                    if (cBase)
                    {
                        cBase.AgentOffYieldOn();
                    }
                }
            }
        }
    }

    void OnDisable()
    {
        if (forOnDisable)
        {
            AgentTurnOff();
        }
    }

    void OnDestroy()
    {
        if (!forOnDisable)
        {
            AgentTurnOff();
        }
    }

}
