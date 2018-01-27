using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class Word
{
    public string text;
    public float width = 0;
    public bool invalid;

    int index = 0;

    public void addChar(char chr)
    {
        StringBuilder sb = new StringBuilder();
        string s = chr.ToString().ToUpper();

        if (index >= text.Length)
        {
            sb.Append(text);
            sb.Append(s);
        }
        else
        {
            for (int i = 0; i < text.Length; ++i)
            {
                if (i == index)
                    sb.Append(s);
                else
                    sb.Append(text[i]);
            }
        }
        text = sb.ToString();

        ++index;
    }
}


public class ChannelParams
{
    public float speed;
    public string text;
    public Dictionary<string, string> mapping;
}

public class Channel
{
    private static string TAG = "[Channel] ";

    public Dictionary<string, string> mapping;
    public float speed;

    Word[] words;

    public Channel(ChannelParams args)
    {
        this.mapping = new Dictionary<string, string>(args.mapping);
        this.speed = args.speed;
        string[] splitters = { @" ", @"\n", @"\r", "\t" };
        string[] w = args.text.Split(splitters, System.StringSplitOptions.RemoveEmptyEntries);
        words = new Word[w.Length];

        for (int i = 0; i < wordCount; ++i)
        {
            Word word = new Word();
            word.text = w[i];
            word.invalid = mapping.ContainsKey(word.text);
            words[i] = word;
        }

        //Debug.Log(TAG + "word count: " + words.Length);
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
