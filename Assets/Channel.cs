using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Channel
{
    private static string TAG = "[Channel] ";

    string text;
    string[] words;

    public Channel(string text)
    {
        string[] splitters = { @"\s+" };
        words = text.Split(splitters, System.StringSplitOptions.RemoveEmptyEntries);
        Debug.Log(TAG + "word count = " + words.Length);
    }
}
