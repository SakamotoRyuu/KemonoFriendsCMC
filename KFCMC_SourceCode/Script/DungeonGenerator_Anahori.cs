///作者 2dgames_jp 様
///https://qiita.com/2dgames_jp/items/037dc62afb268759d16d

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator_Anahori : DungeonGenerator_Specify {
    
    public int holeMax = 32;
    const int passageNumber = 200000;

    public override void Generate() {
        map = new int[width, height];
        StartAnahori();
        SpecifyGenerate();
        ModifyWalls();
    }

    /// 2次元レイヤー
    class Layer2D {
        int _width; // 幅.
        int _height; // 高さ.
        int _outOfRange = -1; // 領域外を指定した時の値.
        int[] _values = null; // マップデータ.
                              /// 幅.
        public int Width {
            get { return _width; }
        }
        /// 高さ.
        public int Height {
            get { return _height; }
        }

        /// 作成.
        public void Create(int width, int height) {
            _width = width;
            _height = height;
            _values = new int[Width * Height];
        }

        /// 座標をインデックスに変換する.
        public int ToIdx(int x, int y) {
            return x + (y * Width);
        }

        /// 領域外かどうかチェックする.
        public bool IsOutOfRange(int x, int y) {
            if (x < 0 || x >= Width) {
                return true;
            }
            if (y < 0 || y >= Height) {
                return true;
            }

            // 領域内.
            return false;
        }
        /// 値の取得.
        // @param x X座標.
        // @param y Y座標.
        // @return 指定の座標の値（領域外を指定したら_outOfRangeを返す）.
        public int Get(int x, int y) {
            if (IsOutOfRange(x, y)) {
                return _outOfRange;
            }

            return _values[y * Width + x];
        }

        /// 値の設定.
        // @param x X座標.
        // @param y Y座標.
        // @param v 設定する値.
        public void Set(int x, int y, int v) {
            if (IsOutOfRange(x, y)) {
                // 領域外を指定した.
                return;
            }

            _values[y * Width + x] = v;
        }

        /// 特定の値をすべてのセルに設定する.
        public void Fill(int v) {
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    Set(x, y, v);
                }
            }
        }
        
        /// デバッグ出力.
        public void Dump() {
            Debug.Log("[Layer2D] (w,h)=(" + Width + "," + Height + ")");
            for (int y = 0; y < Height; y++) {
                string s = "";
                for (int x = 0; x < Width; x++) {
                    s += Get(x, y) + ",";
                }
                Debug.Log(s);
            }
        }
        
    }
    

    /// チップ定数
    const int CHIP_NONE = 0; // 通過可能
    const int CHIP_WALL = 1; // 通行不可

    /// 穴掘り開始
    void StartAnahori() {
        // ダンジョンを作る
        var layer = new Layer2D();
        // ダンジョンの幅と高さは奇数のみ
        layer.Create(width - 2, height - 2);
        // すべて壁を埋める
        layer.Fill(CHIP_WALL);

        // 開始地点を決める
        int xstart = 2; // 開始地点は偶数でないといけない
        int ystart = 4; // 開始地点は偶数でないといけない

        //改造
        xstart = 2 + Random.Range(0, (width - 1) / 2 - 2) * 2;
        ystart = 2 + Random.Range(0, (height - 1) / 2 - 2) * 2;

        // 穴掘り開始
        _Dig(layer, xstart, ystart);

        // 結果表示
        //layer.Dump();

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (x > 0 && y > 0 && x < width - 1 && y < height - 1) {
                    map[x, y] = (layer.Get(x - 1, y - 1) == CHIP_WALL ? 0 : passageNumber);
                } else {
                    map[x, y] = 0;
                }
            }
        }
        for (int i = 0; i < holeMax; i++) {
            for (int j = 0; j < 5; j++) {
                int x = 1 + Random.Range(0, (width - 1) / 2 - 1) * 2;
                int y = 1 + Random.Range(0, (height - 1) / 2 - 1) * 2;
                if (Random.Range(0, 2) == 0) {
                    x++;
                } else {
                    y++;
                }
                if (map[x, y] < roomBase) {
                    map[x, y] = roomBase;
                    break;
                }
            }
        }

    }

    /// 穴を掘る
    void _Dig(Layer2D layer, int x, int y) {
        // 開始地点を掘る
        layer.Set(x, y, CHIP_NONE);

        Vector2[] dirList = {
      new Vector2 (-1, 0),
      new Vector2 (0, -1),
      new Vector2 (1, 0),
      new Vector2 (0, 1)
    };

        // シャッフル
        for (int i = 0; i < dirList.Length; i++) {
            var tmp = dirList[i];
            var idx = Random.Range(0, dirList.Length - 1);
            dirList[i] = dirList[idx];
            dirList[idx] = tmp;
        }

        foreach (var dir in dirList) {
            int dx = (int)dir.x;
            int dy = (int)dir.y;
            if (layer.Get(x + dx * 2, y + dy * 2) == 1) {
                // 2マス先が壁なので掘れる
                layer.Set(x + dx, y + dy, CHIP_NONE);

                // 次の穴を掘る
                _Dig(layer, x + dx * 2, y + dy * 2);
            }
        }
    }
}
