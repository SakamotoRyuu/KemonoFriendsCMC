using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
            temp[index++] = new ContainerData(0);
            temp[index++] = new ContainerData(0);
            temp[index++] = new ContainerData(0);
            temp[index++] = new ContainerData(0);
            temp[index++] = new ContainerData(0);
            temp[index++] = new ContainerData(0);
            temp[index++] = new ContainerData(70);//1
            temp[index++] = new ContainerData(70);//1
            temp[index++] = new ContainerData(70);//1
            temp[index++] = new ContainerData(70, 70);//2
            temp[index++] = new ContainerData(20);
            temp[index++] = new ContainerData(20);
            temp[index++] = new ContainerData(41);
            temp[index++] = new ContainerData(41);
            temp[index++] = new ContainerData(40);
            ContainerCopy(0, index, temp);

            index = 0;
            temp[index++] = new ContainerData(0, 0);
            temp[index++] = new ContainerData(0, 0);
            temp[index++] = new ContainerData(0, 0);
            temp[index++] = new ContainerData(1);
            temp[index++] = new ContainerData(1);
            temp[index++] = new ContainerData(1);
            temp[index++] = new ContainerData(70, 70);//2
            temp[index++] = new ContainerData(70, 70);//2
            temp[index++] = new ContainerData(71);//3
            temp[index++] = new ContainerData(71);//3
            temp[index++] = new ContainerData(9);
            temp[index++] = new ContainerData(21);
            temp[index++] = new ContainerData(22);
            temp[index++] = new ContainerData(23);
            temp[index++] = new ContainerData(24);
            temp[index++] = new ContainerData(41);
            temp[index++] = new ContainerData(41);
            temp[index++] = new ContainerData(42);
            temp[index++] = new ContainerData(42);
            temp[index++] = new ContainerData(53);
            temp[index++] = new ContainerData(54);
            temp[index++] = new ContainerData(86);
            ContainerCopy(1, index, temp);

            index = 0;
            temp[index++] = new ContainerData(1);
            temp[index++] = new ContainerData(1);
            temp[index++] = new ContainerData(1, 0);
            temp[index++] = new ContainerData(1, 0);
            temp[index++] = new ContainerData(1, 0);
            temp[index++] = new ContainerData(1, 1);
            temp[index++] = new ContainerData(1, 1);
            temp[index++] = new ContainerData(71, 70);//4
            temp[index++] = new ContainerData(71, 70);//4
            temp[index++] = new ContainerData(71, 70);//4
            temp[index++] = new ContainerData(71, 70);//4
            temp[index++] = new ContainerData(9);
            temp[index++] = new ContainerData(23);
            temp[index++] = new ContainerData(24);
            temp[index++] = new ContainerData(25);
            temp[index++] = new ContainerData(26);
            temp[index++] = new ContainerData(43);
            temp[index++] = new ContainerData(44);
            temp[index++] = new ContainerData(50);
            temp[index++] = new ContainerData(50);
            temp[index++] = new ContainerData(55);
            temp[index++] = new ContainerData(56);
            temp[index++] = new ContainerData(60);
            temp[index++] = new ContainerData(61);
            temp[index++] = new ContainerData(62);
            temp[index++] = new ContainerData(87);
            ContainerCopy(2, index, temp);

            index = 0;
            temp[index++] = new ContainerData(1, 1);
            temp[index++] = new ContainerData(1, 1);
            temp[index++] = new ContainerData(2);
            temp[index++] = new ContainerData(2);
            temp[index++] = new ContainerData(2);
            temp[index++] = new ContainerData(2, 1);
            temp[index++] = new ContainerData(2, 1);
            temp[index++] = new ContainerData(72);//5
            temp[index++] = new ContainerData(72);//5
            temp[index++] = new ContainerData(72,1);//6
            temp[index++] = new ContainerData(72,1);//6
            temp[index++] = new ContainerData(9, 9);
            temp[index++] = new ContainerData(25);
            temp[index++] = new ContainerData(26);
            temp[index++] = new ContainerData(27);
            temp[index++] = new ContainerData(28);
            temp[index++] = new ContainerData(43);
            temp[index++] = new ContainerData(44);
            temp[index++] = new ContainerData(51);
            temp[index++] = new ContainerData(51);
            temp[index++] = new ContainerData(55);
            temp[index++] = new ContainerData(56);
            temp[index++] = new ContainerData(60);
            temp[index++] = new ContainerData(61);
            temp[index++] = new ContainerData(62);
            temp[index++] = new ContainerData(88);
            ContainerCopy(3, index, temp);

            index = 0;
            temp[index++] = new ContainerData(2, 1);
            temp[index++] = new ContainerData(2, 1);
            temp[index++] = new ContainerData(2, 1);
            temp[index++] = new ContainerData(2, 2);
            temp[index++] = new ContainerData(2, 2);
            temp[index++] = new ContainerData(2, 2);
            temp[index++] = new ContainerData(72, 70);//6
            temp[index++] = new ContainerData(72, 70);//6
            temp[index++] = new ContainerData(72, 71);//8
            temp[index++] = new ContainerData(72, 71);//8
            temp[index++] = new ContainerData(27);
            temp[index++] = new ContainerData(28);
            temp[index++] = new ContainerData(29);
            temp[index++] = new ContainerData(29);
            temp[index++] = new ContainerData(41, 42);
            temp[index++] = new ContainerData(42, 41);
            temp[index++] = new ContainerData(46);
            temp[index++] = new ContainerData(52);
            temp[index++] = new ContainerData(52);
            temp[index++] = new ContainerData(57);
            temp[index++] = new ContainerData(58);
            temp[index++] = new ContainerData(63);
            temp[index++] = new ContainerData(64);
            temp[index++] = new ContainerData(65);
            temp[index++] = new ContainerData(89);
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
