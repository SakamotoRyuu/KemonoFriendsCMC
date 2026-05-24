using UnityEngine;
using System.IO;
using TMPro;

public class DumpTMPText : MonoBehaviour
{
    public string fileName;
    
    void Start()
    {
        StreamWriter sw = new StreamWriter("./Assets/" + fileName + ".txt",false);
        TMP_Text text = GetComponent<TMP_Text>();
        sw.WriteLine(text.text);
        sw.Flush();
        sw.Close();

    }
}