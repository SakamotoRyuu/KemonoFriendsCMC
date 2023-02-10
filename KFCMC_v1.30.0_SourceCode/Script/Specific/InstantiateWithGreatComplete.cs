using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateWithGreatComplete : InstantiateWithFriends {

    protected override bool CheckCondition() {
        bool answer = GameManager.Instance.GetPerfectCompleted();
        if (answer && TrophyManager.Instance) {
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_Sushizanmai, true);
            GameManager.Instance.SetSteamAchievement("MINMIGREATBUDDHA");
        }
        return answer;
    }

}
