using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetElementalAttributeForAD : MonoBehaviour
{

    public AttackDetection targetAD;
    public AttackDetection.ElementalAttribute elementalAttribute;

    private void Awake() {
        targetAD.elementalAttribute = elementalAttribute;
    }

}
