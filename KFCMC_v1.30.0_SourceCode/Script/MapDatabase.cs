using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDatabase : SingletonMonoBehaviour<MapDatabase> {    

    [System.Serializable]
    public class RouteChild {
        public Vector2Int[] step;
    }

    [System.Serializable]
    public class Route {
        public bool disabled;
        public RouteChild[] way;
    }

    public Route[] routes;

    [System.NonSerialized]
    public GameObject[] prefab;

    public const int mapMax = 12;
    public const int walkable = 0;
    public const int player = 1;
    public const int goal = 2;
    public const int friends = 3;
    public const int enemy = 4;
    public const int item = 5;
    public const int gold = 6;
    public const int itemCharacter = 7;
    public const int enemyL = 8;
    public const int enemyXL = 9;
    public const int enemyXXL = 10;
    public const int other = 11;
    static readonly Vector2Int vecZero = Vector2Int.zero;

    protected override void Awake() {
        if (CheckInstance()) {
            DontDestroyOnLoad(gameObject);
            prefab = new GameObject[mapMax];
            prefab[walkable] = Resources.Load("Prefab/Map/Map_Walkable") as GameObject;
            prefab[player] = Resources.Load("Prefab/Map/Map_Player") as GameObject;
            prefab[goal] = Resources.Load("Prefab/Map/Map_Goal") as GameObject;
            prefab[friends] = Resources.Load("Prefab/Map/Map_Friends") as GameObject;
            prefab[enemy] = Resources.Load("Prefab/Map/Map_Enemy") as GameObject;
            prefab[item] = Resources.Load("Prefab/Map/Map_Item") as GameObject;
            prefab[gold] = Resources.Load("Prefab/Map/Map_Gold") as GameObject;
            prefab[itemCharacter] = Resources.Load("Prefab/Map/Map_ItemCharacter") as GameObject;
            prefab[enemyL] = Resources.Load("Prefab/Map/Map_Enemy_L") as GameObject;
            prefab[enemyXL] = Resources.Load("Prefab/Map/Map_Enemy_XL") as GameObject;
            prefab[enemyXXL] = Resources.Load("Prefab/Map/Map_Enemy_XXL") as GameObject;
            prefab[other] = Resources.Load("Prefab/Map/Map_Other") as GameObject;
        }
    }
}
