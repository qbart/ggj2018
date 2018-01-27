using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Word
{
    public string text;
    public bool invalid;
}


public class Channel
{
    private static string TAG = "[Channel] ";

    public Dictionary<string, string> mapping;
    public float speed;

    Word[] words;

    public Channel(float speed, string text, Dictionary<string, string> mapping)
    {
        this.mapping = mapping;
        this.speed = speed;
        string[] splitters = { @" ", @"\n", @"\r", "\t" };
        string[] w = text.Split(splitters, System.StringSplitOptions.RemoveEmptyEntries);
        words = new Word[w.Length];

        for (int i = 0; i < wordCount; ++i)
        {
            Word word;
            word.text = w[i];
            word.invalid = mapping.ContainsKey(word.text);
            words[i] = word;
        }

        Debug.Log(TAG + "word count: " + words.Length);
    }

    public Word this[int index]
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

    public string getMapping(string text)
    {
        return mapping[text];
    }
}
