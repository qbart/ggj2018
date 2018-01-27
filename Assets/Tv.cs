using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tv : MonoBehaviour
{
    public static string LOREM = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer id orci pharetra ex varius faucibus. Sed vulputate sollicitudin diam a fringilla. Duis dolor magna, gravida non eros sit amet, cursus eleifend ante. Donec a elit ac arcu finibus rhoncus eget id diam. Praesent lorem mauris, tempor sit amet venenatis at, mattis vel nisi. Duis vel pharetra nunc, vitae facilisis urna. Integer non aliquet erat. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris eget tellus ipsum. Aliquam id justo sed lacus gravida tristique. Integer blandit leo eget ipsum imperdiet varius. In tincidunt arcu mauris, nec lacinia augue faucibus ut.

Maecenas nec dolor in arcu dictum lacinia id in enim. Maecenas venenatis augue vel bibendum eleifend. Nulla tempor neque quis arcu convallis, id ornare urna euismod. Nullam aliquet interdum aliquet. Aenean et convallis leo. Vivamus interdum lobortis nunc, vel fermentum turpis cursus eu. Morbi blandit faucibus nunc, id facilisis nisi semper a. Nulla nisi sem, rutrum in ultrices at, venenatis at purus. Donec commodo felis arcu, et accumsan purus posuere et. Vestibulum non malesuada arcu. Aliquam convallis orci et odio lacinia, sit amet imperdiet orci lobortis. Nulla ac purus aliquet, dignissim diam eget, scelerisque sem. Vivamus ornare tellus sed arcu iaculis ornare. Aenean eu finibus augue. Pellentesque tincidunt sem vitae metus tincidunt, eget sodales odio posuere. Praesent eleifend vitae ante quis pharetra.

Donec at erat massa. Sed imperdiet, elit eget condimentum maximus, magna justo faucibus ante, dignissim elementum augue enim ac purus. Donec mattis et nisl ac finibus. Mauris vel hendrerit risus. In lobortis pulvinar nisi, sed elementum nisi molestie vel. Maecenas arcu odio, finibus pretium tempor quis, suscipit in eros. Pellentesque euismod mattis viverra. In maximus tincidunt velit. Sed sit amet tellus id quam tincidunt suscipit sit amet eu felis. In vel mi finibus, lacinia ligula vitae, placerat felis.

Pellentesque ut orci quam. Sed malesuada diam sed odio lacinia, sit amet tempor nulla tincidunt. Vivamus condimentum congue nisi in venenatis. Sed volutpat, quam vitae tempor vehicula, libero dui suscipit sapien, luctus molestie mauris lectus a magna. In molestie viverra justo, eget consectetur tellus sagittis id. Duis rutrum elit vitae nulla feugiat varius. Curabitur mollis elementum nibh et hendrerit. Donec in urna ex. Aliquam efficitur neque lobortis lorem scelerisque, ac condimentum libero molestie. Quisque eros enim, egestas et accumsan nec, sollicitudin nec urna. Pellentesque a augue nisl. Pellentesque eu est elit. Phasellus vitae lacus sem. Etiam laoreet tincidunt quam. Nulla facilisis vel justo eu sollicitudin. Nulla ac neque purus.";

    public Prompt prompt;

    Channel[] channels;
    int currentChannel = -1;

    void Start()
    {
        Dictionary<string, string>[] mapping = new Dictionary<string, string>[]
        {
            new Dictionary<string, string>()
            {
                { "sit", "dupa" },
                { "Sed", "secret" }
            },
            new Dictionary<string, string>()
            {
                { "one", "111" },
                { "two", "222" },
                { "ten", "000" },
            }
        };

        channels = new Channel[]
        {
            new Channel(5f, "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer id orci pharetra ex varius faucibus. Sed vulputate sollicitudin diam a fringilla. Duis dolor magna, gravida", mapping[0]),
            new Channel(9f, "one two three four five six seven eight nine ten eleven twelve one two three four five six seven eight nine ten eleven twelve one two three four five six seven eight nine ten eleven twelve ", mapping[1])
        };

        nextChannel();
    }

    public void nextChannel()
    {
        prompt.changeChannel(channels[1]);
        return;

        HashSet<int> channelSet = new HashSet<int>();
        for (int i = 0; i < channels.Length; ++i)
            channelSet.Add(i);

        channelSet.Remove(currentChannel);
        List<int> list = new List<int>();
        foreach (int element in channelSet)
            list.Add(element);

        currentChannel = list[Random.Range(0, list.Count)];
        prompt.changeChannel(channels[currentChannel]);
    }
}
