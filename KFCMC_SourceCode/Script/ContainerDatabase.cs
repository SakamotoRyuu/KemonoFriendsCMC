using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static ItemDatabase;

public class ContainerDatabase : SingletonMonoBehaviour<ContainerDatabase> {
    
    const int rankMax = 5;
    
    public class ContainerData {
        public int id1;
        public int id2;
        public ContainerData(int id1, int id2 = -1) {
            this.id1 = id1;
            this.id2 = id2;
        }
    }

    public ContainerData[][] containerArray = new ContainerData[rankMax][];

    void ContainerCopy(int rank, int max, ContainerData[] tempArray) {
        containerArray[rank] = new ContainerData[max];
        for (int i = 0; i < max; i++) {
            containerArray[rank][i] = tempArray[i];
        }
    }

    protected override void Awake() {
        if (CheckInstance()) {
            DontDestroyOnLoad(gameObject);
            int index;
            ContainerData[] temp = new ContainerData[50];

            index = 0;
            temp[index++] = new ContainerData((int)ItemID.Heal0);
            temp[index++] = new ContainerData((int)ItemID.Heal0);
            temp[index++] = new ContainerData((int)ItemID.Heal0);
            temp[index++] = new ContainerData((int)ItemID.Heal0);
            temp[index++] = new ContainerData((int)ItemID.Heal0);
            temp[index++] = new ContainerData((int)ItemID.Heal0);
            temp[index++] = new ContainerData((int)ItemID.Japariman);//1
            temp[index++] = new ContainerData((int)ItemID.Japariman);//1
            temp[index++] = new ContainerData((int)ItemID.Japariman);//1
            temp[index++] = new ContainerData((int)ItemID.Japariman_2);//2
            temp[index++] = new ContainerData((int)ItemID.Antidote);
            temp[index++] = new ContainerData((int)ItemID.Antidote);
            temp[index++] = new ContainerData((int)ItemID.GraphGuide);
            temp[index++] = new ContainerData((int)ItemID.GraphGuide);
            temp[index++] = new ContainerData((int)ItemID.GraphWarp);
            ContainerCopy(0, index, temp);

            index = 0;
            temp[index++] = new ContainerData((int)ItemID.Heal0, (int)ItemID.Heal0);
            temp[index++] = new ContainerData((int)ItemID.Heal0, (int)ItemID.Heal0);
            temp[index++] = new ContainerData((int)ItemID.Heal0, (int)ItemID.Heal0);
            temp[index++] = new ContainerData((int)ItemID.Heal1);
            temp[index++] = new ContainerData((int)ItemID.Heal1);
            temp[index++] = new ContainerData((int)ItemID.Heal1);
            temp[index++] = new ContainerData((int)ItemID.Japariman_2);//2
            temp[index++] = new ContainerData((int)ItemID.Japariman_2);//2
            temp[index++] = new ContainerData((int)ItemID.Japariman_3);//3
            temp[index++] = new ContainerData((int)ItemID.Japariman_3);//3
            temp[index++] = new ContainerData((int)ItemID.Sandstar);
            temp[index++] = new ContainerData((int)ItemID.BuffStamina);
            temp[index++] = new ContainerData((int)ItemID.BuffSpeed);
            temp[index++] = new ContainerData((int)ItemID.BuffAttack);
            temp[index++] = new ContainerData((int)ItemID.BuffDefense);
            temp[index++] = new ContainerData((int)ItemID.GraphGuide);
            temp[index++] = new ContainerData((int)ItemID.GraphGuide);
            temp[index++] = new ContainerData((int)ItemID.GraphClearTraps);
            temp[index++] = new ContainerData((int)ItemID.GraphClearTraps);
            temp[index++] = new ContainerData((int)ItemID.CrystalBlack);
            temp[index++] = new ContainerData((int)ItemID.CrystalRed);
            temp[index++] = new ContainerData((int)ItemID.Coin_S);
            ContainerCopy(1, index, temp);

            index = 0;
            temp[index++] = new ContainerData((int)ItemID.Heal1);
            temp[index++] = new ContainerData((int)ItemID.Heal1);
            temp[index++] = new ContainerData((int)ItemID.Heal1, (int)ItemID.Heal0);
            temp[index++] = new ContainerData((int)ItemID.Heal1, (int)ItemID.Heal0);
            temp[index++] = new ContainerData((int)ItemID.Heal1, (int)ItemID.Heal0);
            temp[index++] = new ContainerData((int)ItemID.Heal1, (int)ItemID.Heal1);
            temp[index++] = new ContainerData((int)ItemID.Heal1, (int)ItemID.Heal1);
            temp[index++] = new ContainerData((int)ItemID.Japariman_4);//4
            temp[index++] = new ContainerData((int)ItemID.Japariman_4);//4
            temp[index++] = new ContainerData((int)ItemID.Japariman_4);//4
            temp[index++] = new ContainerData((int)ItemID.Japariman_4);//4
            temp[index++] = new ContainerData((int)ItemID.Sandstar);
            temp[index++] = new ContainerData((int)ItemID.BuffAttack);
            temp[index++] = new ContainerData((int)ItemID.BuffDefense);
            temp[index++] = new ContainerData((int)ItemID.BuffKnock);
            temp[index++] = new ContainerData((int)ItemID.BuffAbsorb);
            temp[index++] = new ContainerData((int)ItemID.GraphGold);
            temp[index++] = new ContainerData((int)ItemID.GraphBreak);
            temp[index++] = new ContainerData((int)ItemID.CrystalGreen_S);
            temp[index++] = new ContainerData((int)ItemID.CrystalGreen_S);
            temp[index++] = new ContainerData((int)ItemID.CrystalBlue);
            temp[index++] = new ContainerData((int)ItemID.CrystalViolet);
            temp[index++] = new ContainerData((int)ItemID.BombNormal);
            temp[index++] = new ContainerData((int)ItemID.BombPoison);
            temp[index++] = new ContainerData((int)ItemID.BombAcid);
            temp[index++] = new ContainerData((int)ItemID.Coin_M);
            ContainerCopy(2, index, temp);

            index = 0;
            temp[index++] = new ContainerData((int)ItemID.Heal1, (int)ItemID.Heal1);
            temp[index++] = new ContainerData((int)ItemID.Heal1, (int)ItemID.Heal1);
            temp[index++] = new ContainerData((int)ItemID.Heal2);
            temp[index++] = new ContainerData((int)ItemID.Heal2);
            temp[index++] = new ContainerData((int)ItemID.Heal2);
            temp[index++] = new ContainerData((int)ItemID.Heal2, (int)ItemID.Heal1);
            temp[index++] = new ContainerData((int)ItemID.Heal2, (int)ItemID.Heal1);
            temp[index++] = new ContainerData((int)ItemID.Japariman_5);//5
            temp[index++] = new ContainerData((int)ItemID.Japariman_5);//5
            temp[index++] = new ContainerData((int)ItemID.Japariman_5, (int)ItemID.Japariman);//6
            temp[index++] = new ContainerData((int)ItemID.Japariman_5, (int)ItemID.Japariman);//6
            temp[index++] = new ContainerData((int)ItemID.Sandstar, (int)ItemID.Sandstar);
            temp[index++] = new ContainerData((int)ItemID.BuffKnock);
            temp[index++] = new ContainerData((int)ItemID.BuffAbsorb);
            temp[index++] = new ContainerData((int)ItemID.BuffStealth);
            temp[index++] = new ContainerData((int)ItemID.BuffLong);
            temp[index++] = new ContainerData((int)ItemID.GraphGold);
            temp[index++] = new ContainerData((int)ItemID.GraphBreak);
            temp[index++] = new ContainerData((int)ItemID.CrystalGreen_M);
            temp[index++] = new ContainerData((int)ItemID.CrystalGreen_M);
            temp[index++] = new ContainerData((int)ItemID.CrystalBlue);
            temp[index++] = new ContainerData((int)ItemID.CrystalViolet);
            temp[index++] = new ContainerData((int)ItemID.BombNormal);
            temp[index++] = new ContainerData((int)ItemID.BombPoison);
            temp[index++] = new ContainerData((int)ItemID.BombAcid);
            temp[index++] = new ContainerData((int)ItemID.Coin_L);
            ContainerCopy(3, index, temp);

            index = 0;
            temp[index++] = new ContainerData((int)ItemID.Heal2, (int)ItemID.Heal1);
            temp[index++] = new ContainerData((int)ItemID.Heal2, (int)ItemID.Heal1);
            temp[index++] = new ContainerData((int)ItemID.Heal2, (int)ItemID.Heal1);
            temp[index++] = new ContainerData((int)ItemID.Heal2, (int)ItemID.Heal2);
            temp[index++] = new ContainerData((int)ItemID.Heal2, (int)ItemID.Heal2);
            temp[index++] = new ContainerData((int)ItemID.Heal2, (int)ItemID.Heal2);
            temp[index++] = new ContainerData((int)ItemID.Japariman_5, (int)ItemID.Japariman);//6
            temp[index++] = new ContainerData((int)ItemID.Japariman_5, (int)ItemID.Japariman);//6
            temp[index++] = new ContainerData((int)ItemID.Japariman_5, (int)ItemID.Japariman_3);//8
            temp[index++] = new ContainerData((int)ItemID.Japariman_5, (int)ItemID.Japariman_3);//8
            temp[index++] = new ContainerData((int)ItemID.BuffStealth);
            temp[index++] = new ContainerData((int)ItemID.BuffLong);
            temp[index++] = new ContainerData((int)ItemID.BuffMulti);
            temp[index++] = new ContainerData((int)ItemID.BuffMulti);
            temp[index++] = new ContainerData((int)ItemID.GraphGuide, (int)ItemID.GraphClearTraps);
            temp[index++] = new ContainerData((int)ItemID.GraphClearTraps, (int)ItemID.GraphGuide);
            temp[index++] = new ContainerData((int)ItemID.GraphConnect);
            temp[index++] = new ContainerData((int)ItemID.GraphBuild);
            temp[index++] = new ContainerData((int)ItemID.CrystalGreen_L);
            temp[index++] = new ContainerData((int)ItemID.CrystalGreen_L);
            temp[index++] = new ContainerData((int)ItemID.CrystalYellow);
            temp[index++] = new ContainerData((int)ItemID.CrystalCrimson);
            temp[index++] = new ContainerData((int)ItemID.BombNormal_L);
            temp[index++] = new ContainerData((int)ItemID.BombPoison_L);
            temp[index++] = new ContainerData((int)ItemID.BombAcid_L);
            temp[index++] = new ContainerData((int)ItemID.Coin_XL);
            ContainerCopy(4, index, temp);
        }
    }
    
    public int GetIDSingle(int rank) {
        int answer = -1;
        if (rank >= 0 && rank < rankMax) {
            int count = containerArray[rank].Length;
            int index = Random.Range(0, count);
            ContainerData temp = containerArray[rank][index];
            if (temp.id2 >= 0 && Random.Range(0, 2) == 1) {
                answer = temp.id2;
            } else {
                answer = temp.id1;
            }
        }
        return answer;
    }

    public int[] GetIDArray(int rank) {
        int[] answer = new int[2] { -1, -1 };
        if (rank >= 0 && rank < rankMax) {
            int count = containerArray[rank].Length;
            int index = Random.Range(0, count);
            ContainerData temp = containerArray[rank][index];
            answer[0] = temp.id1;
            answer[1] = temp.id2;
        }
        return answer;
    }
}
