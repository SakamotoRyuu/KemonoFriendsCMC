using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateWithSecret : InstantiateWithFriends {

    public GameManager.SecretType secretType;

    protected override bool CheckCondition() {
        return GameManager.Instance.GetSecret(secretType);
    }

}
