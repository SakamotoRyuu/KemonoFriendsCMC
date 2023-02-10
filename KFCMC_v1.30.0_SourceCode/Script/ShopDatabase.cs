using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopDatabase : SingletonMonoBehaviour<ShopDatabase> {

    public const int p1 = 1;
    public const int p2 = 5;
    public const int p3 = 10;
    public const int p4 = 11;

    public class ShopData {
        public int id;
        public int conditionId;
        public int conditionShopLevel;
        public ShopData(int id, int conditionId, int conditionShopLevel) {
            this.id = id;
            this.conditionId = conditionId;
            this.conditionShopLevel = conditionShopLevel;
        }
    }

    public List<ShopData> shopList = new List<ShopData>();

    protected override void Awake() {
        if (CheckInstance()) {
            DontDestroyOnLoad(gameObject);
            shopList.Add(new ShopData(0, -1, 1));
            shopList.Add(new ShopData(1, -1, 2));
            shopList.Add(new ShopData(2, -1, 3));
            shopList.Add(new ShopData(9, -1, 2));
            shopList.Add(new ShopData(20, -1, 1));
            shopList.Add(new ShopData(41, -1, 1));
            shopList.Add(new ShopData(42, -1, 1));
            shopList.Add(new ShopData(70, -1, 1));
            shopList.Add(new ShopData(71, -1, 1));
            shopList.Add(new ShopData(72, -1, 1));
            shopList.Add(new ShopData(80, -1, 4));
            shopList.Add(new ShopData(203, -1, 1));
            shopList.Add(new ShopData(204, 3, 2));
            shopList.Add(new ShopData(205, 4, 3));
            shopList.Add(new ShopData(206, -1, 1));
            shopList.Add(new ShopData(207, 6, 2));
            shopList.Add(new ShopData(208, 7, 3));
            shopList.Add(new ShopData(209, -1, 2));
            shopList.Add(new ShopData(210, 9, 3));
            shopList.Add(new ShopData(211, -1, 2));
            shopList.Add(new ShopData(212, -1, 1));
            shopList.Add(new ShopData(213, -1, 1));
            shopList.Add(new ShopData(214, -1, 1));
            shopList.Add(new ShopData(215, -1, 2));
            shopList.Add(new ShopData(216, -1, 2));
            shopList.Add(new ShopData(217, -1, 2));
            shopList.Add(new ShopData(218, -1, 3));
            shopList.Add(new ShopData(219, -1, 4));
            shopList.Add(new ShopData(220, -1, 4));
            shopList.Add(new ShopData(221, -1, 1));
            shopList.Add(new ShopData(222, -1, 1));
        }
    }

    public int GetShopLevel() {
        int progress = GameManager.Instance.save.progress;
        int answer = 0;
        if (progress >= p4) {
            bool luckyComplete = true;
            for (int i = 0; i < GameManager.luckyBeastMax && luckyComplete; i++) {
                luckyComplete = (GameManager.Instance.save.luckyBeast[i] != 0);
            }
            if (luckyComplete) {
                answer = 4;
            }
        }
        if (answer < 4) {
            if (progress >= p3) {
                answer = 3;
            } else if (progress >= p2) {
                answer = 2;
            } else if (progress >= p1) {
                answer = 1;
            }
        }
        return answer;
    }
}
