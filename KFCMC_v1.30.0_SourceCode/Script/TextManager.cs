/// 作者 大場洋平 様
/// http://taiyoproject.com/post-430

using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
 
/// <summary>
/// 文字列管理クラス(多言語対応)
/// </summary>
public static class TextManager {
    // 文字列格納、検索用ディクショナリー
    private static Dictionary<string, string> sDictionary = new Dictionary<string, string>();
    
    /// <summary>
    /// 使用言語
    /// </summary>
    public enum LANGUAGE {
        JAPANESE,
        ENGLISH,
        CHINESE,
        KOREAN
    }
    public static bool IsInitialized { get; private set; }

    /// <summary>
    /// 文字列初期化
    /// </summary>
    /// <param name="lang">使用言語</param>
    public static void Init(LANGUAGE lang) {
        // リソースファイルパス決定
        string filePath;
        switch (lang) {
            case LANGUAGE.JAPANESE:
                filePath = "Text/japanese";
                break;
            case LANGUAGE.ENGLISH:
                filePath = "Text/english";
                break;
            case LANGUAGE.CHINESE:
                filePath = "Text/chinese";
                break;
            case LANGUAGE.KOREAN:
                filePath = "text/korean";
                break;
            default:
                throw new Exception("TextManager Init failed.");
        }

        // ディクショナリー初期化
        sDictionary.Clear();
        TextAsset csv = Resources.Load<TextAsset>(filePath);
        StringReader reader = new StringReader(csv.text);
        while (reader.Peek() > -1) {
            string[] values = reader.ReadLine().Split('\t');
            sDictionary.Add(values[0], values[1].Replace("\\n", "\n"));
        }
        IsInitialized = true;
    }    

    /// <summary>
    /// 文字列取得
    /// </summary>
    /// <param name="key">文字列取得キー</param>
    /// <returns>キーに該当する文字列</returns>
    public static string Get(string key) {
        return sDictionary[key];
    }
}