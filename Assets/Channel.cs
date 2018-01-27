using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Channel
{
    private static string TAG = "[Channel] ";

    public float speed;
    string[] words;

    public Channel(string text, float speed)
    {
        this.speed = speed;
        string[] splitters = { @" ", @"\n", @"\r", "\t" };
        words = text.Split(splitters, System.StringSplitOptions.RemoveEmptyEntries);
        Debug.Log(TAG + "word count: " + words.Length);
    }

    public string this[int index]
    {
        get
        {
            return words[index];
        }
    }

    public int wordCount
    {
        get { return words.Length; }
    }

    public bool has(int i)
    {
        return i < words.Length;
    }
}
