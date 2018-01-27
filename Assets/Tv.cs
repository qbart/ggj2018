using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tv : MonoBehaviour
{
    public static string LOREM = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer id orci pharetra ex varius faucibus. Sed vulputate sollicitudin diam a fringilla. Duis dolor magna, gravida non eros sit amet";

    public Prompt prompt;

    ChannelParams[] channels;
    int currentChannel = -1;

    void Start()
    {
        channels = new ChannelParams[2];
        for (int i = 0; i < channels.Length; ++i)
            channels[i] = new ChannelParams();

        channels[0].speed = 1f;
        channels[0].text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer id orci pharetra ex varius faucibus. Sed vulputate sollicitudin diam a fringilla. Duis dolor magna, gravida";
        channels[0].mapping = new Dictionary<string, string>()
            {
                { "sit", "sat" },
                { "Sed", "SED" }
            };

        channels[1].speed = 1f;
        channels[1].text = "one two three four five six seven eight nine ten eleven twelve one two three four five six seven eight nine ten eleven twelve one two three four five six seven eight nine ten eleven twelve";
        channels[1].mapping = new Dictionary<string, string>()
            {
                { "one", "xxx" },
                { "two", "yyy" },
                { "ten", "zzz" },
            };

        nextChannel();
    }

    public void nextChannel()
    {
        HashSet<int> channelSet = new HashSet<int>();
        for (int i = 0; i < channels.Length; ++i)
            channelSet.Add(i);

        channelSet.Remove(currentChannel);
        List<int> list = new List<int>();
        foreach (int element in channelSet)
            list.Add(element);

        currentChannel = list[Random.Range(0, list.Count)];
        prompt.changeChannel(new Channel(channels[currentChannel]));
    }
}
