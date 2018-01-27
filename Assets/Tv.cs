using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tv : MonoBehaviour
{
    public static string LOREM = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer id orci pharetra ex varius faucibus. Sed vulputate sollicitudin diam a fringilla. Duis dolor magna, gravida non eros sit amet";

    public Prompt prompt;
	public TextAsset[] files;
	public TextAsset[] mappings;

    ChannelParams[] channels;
    int currentChannel = -1;

    void Start()
    {
        channels = new ChannelParams[2];
        for (int i = 0; i < channels.Length; ++i)
            channels[i] = new ChannelParams();

        channels[0].speed = 1f;
		channels [0].text = files [0].text;
		channels [0].mapping = readMapping (mappings [0]);

        channels[1].speed = 3f;
        channels[1].text = "one two three four five six seven eight nine ten eleven twelve one two three four five six seven eight nine ten eleven twelve one two three four five six seven eight nine ten eleven twelve";
        channels[1].mapping = new Dictionary<string, string>()
            {
                { "one", "xxx" },
                { "two", "yyy" },
                { "ten", "zzz" },
            };

        prompt.changeChannel(new Channel(channels[0]));
        //nextChannel();
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


	private Dictionary<string, string> readMapping(TextAsset mappingFile)
	{
		string[] records = mappingFile.text.Split ('\n');
		Dictionary<string, string> mapping = new Dictionary<string, string> ();
		foreach (string record in records)
		{	
			if (record.Contains(",")) {
				string[] fields = record.Split(',');
				mapping [fields [0]] = fields [1];
			}
		}

		return mapping;
	}
}
