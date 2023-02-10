using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAnimatorParameterOnEnable : MonoBehaviour {

    public enum ParameterType { Float, Int, Bool, Trigger };

    public Animator animator;
    public ParameterType parameterType;
    public string parameterName;
    public float parameterNum;

    private void OnEnable() {
        if (animator) {
            switch (parameterType) {
                case ParameterType.Float:
                    animator.SetFloat(parameterName, parameterNum);
                    break;
                case ParameterType.Int:
                    animator.SetInteger(parameterName, (int)parameterNum);
                    break;
                case ParameterType.Bool:
                    animator.SetBool(parameterName, parameterNum != 0);
                    break;
                case ParameterType.Trigger:
                    animator.SetTrigger(parameterName);
                    break;
            }
        }
    }

}
