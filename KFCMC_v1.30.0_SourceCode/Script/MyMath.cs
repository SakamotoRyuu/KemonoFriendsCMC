using UnityEngine;

public static class MyMath {

    // 正規化処理
    public static float Normalize(ref Vector3 v) {
        float mag = (float)System.Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
        v.x /= mag; v.y /= mag; v.z /= mag;
        // 正規化と同時にスカラー値を返して同じ処理を繰り返さない
        return mag;
    }

    // 正規化したベクトルを返す、インスタンスの値渡しは行わない
    public static float Normalize(ref Vector3 v, out Vector3 vv) {
        float mag = (float)System.Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
        vv.x = v.x / mag; vv.y = v.y / mag; vv.z = v.z / mag;
        return mag;
    }

    // Y座標省略
    public static float NormalizeXZ(ref Vector3 v) {
        float mag = (float)System.Math.Sqrt(v.x * v.x + v.z * v.z);
        v.x /= mag; v.z /= mag; v.y = 0f;
        return mag;
    }

    public static float Square(float f) {
        return f * f;
    }
    public static float PowInt(float x, int y) {
        float answer = 1f;
        if (y > 0) {
            for (int i = 0; i < y; i++) {
                answer *= x;
            }
        }
        return answer;
    }
    public static float SqrMagnitudeIgnoreY(Vector3 pos1, Vector3 pos2) {
        pos1.y = pos2.y = 0f;
        return (pos1 - pos2).sqrMagnitude;
    }

    public static int RandomRangeExclude(int min, int max, int exclude) {
        int answer = Random.Range(min, max - 1);
        if (answer >= exclude) {
            answer++;
        }
        return answer;
    }

}