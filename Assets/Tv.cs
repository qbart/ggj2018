using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tv : MonoBehaviour
{
    public Prompt prompt;
    public GameObject[] visualChannels;
	public TextAsset[] files;
	public TextAsset[] mappings;
    public float[] speeds;

    ChannelParams[] channels;
    int currentChannel = -1;
    public AudioSource media;


    void Start()
    {
        media = GetComponent<AudioSource>();
        channels = new ChannelParams[files.Length];
        for (int i = 0; i < channels.Length; ++i)
        {
            ChannelParams pars = new ChannelParams();
            pars.speed = speeds[i];
            pars.text = files[i].text;
            pars.mapping = readMapping(mappings[i]);
            channels[i] = pars;
        }
        changeChannel(1);
    }

    public void nextChannel()
    {
        if (channels.Length == 1)
        {
            currentChannel = 0;
        }
        else
        {
            HashSet<int> channelSet = new HashSet<int>();
            for (int i = 0; i < channels.Length; ++i)
                channelSet.Add(i);

            channelSet.Remove(currentChannel);
            channelSet.Remove(0);
            List<int> list = new List<int>();
            foreach (int element in channelSet)
                list.Add(element);

            currentChannel = list[Random.Range(0, list.Count)];
        }
        changeChannel(currentChannel);
    }

    void changeChannel(int index)
    {
        media.Stop();

        for (int i = 0; i < visualChannels.Length; ++i)
        {
            if (i == index)
            {
                visualChannels[i].SetActive(true);
				media.clip = visualChannels [i].GetComponent<VisualChannel> ().clip;
                media.Play();
            }
            else
                visualChannels[i].SetActive(false);
        }
        prompt.changeChannel(new Channel(channels[index]));
    }


	private Dictionary<string, string> readMapping(TextAsset mappingFile)
	{
		string[] records = mappingFile.text.Split('\n');
		Dictionary<string, string> mapping = new Dictionary<string, string> ();
		foreach (string record in records)
		{	
			if (record.Contains(",")) {
				string[] fields = record.Split(',');
				mapping[fields[0].Trim()] = fields[1].Trim();
			}
		}

		return mapping;
	}
}
