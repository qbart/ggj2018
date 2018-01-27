using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;


[System.Serializable]
public struct Frame
{
    public float x;
    public float y;
    public float width;
    public float height;
    public float caretX;
}

struct Bounds
{
    public Vector3[] points;
    public Vector3 leftMiddle;
    public Vector3 rightMiddle;
    public Vector3 caret;
}

public class CycleList
{

}


public class Prompt : MonoBehaviour
{
    public Frame rect;
    public GameObject textPrefab;
    public Tv tv;
    public TextMesh ledText;

    Bounds bounds;

    int firstIndex = 0;
    int poolSize;
    int wordsNum = 0;
    int poolEmpty = 0;
    int currentInvalid = -1;

    const float SPACE_SIZE = 0.25f;

    public GameObject[] obj;
    public BlockText[] text;
    public Word[] words;

    Channel channel;
    Vector3 startPos;

    const int POOL_SIZE = 10;

    public void changeChannel(Channel channel)
    {
        initWithChannel(channel);
    }

    void Awake()
    {
        bounds = buildBounds();
        startPos = bounds.leftMiddle;
        poolSize = POOL_SIZE;
        obj = new GameObject[poolSize];
        text = new BlockText[poolSize];
        words = new Word[poolSize];

        for (int i = 0; i < size; ++i)
        {
            obj[i] = Instantiate(textPrefab, bounds.leftMiddle, Quaternion.identity, transform);
            text[i] = obj[i].GetComponent<BlockText>();
        }
    }

    void Update()
    {
        UpdateSubtitles();

        if (shouldChangeChannel())
        {
            tv.nextChannel();
        }
        else if (canType() && Input.inputString.Length > 0)
        {
            char c = Input.inputString[0];
            if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
            {
                string chr = c.ToString().ToUpper();

                //text[0].text = text[0].text + chr;
                //for (int i = 1; i < 5; ++i)
                //{
                //    text[i].transform.position = text[i].transform.position + new Vector3(0.3f, 0, 0);

                //}


            }
        }
    }


    int size { get { return poolSize; } }

    int lastIndex
    {
        get
        {
            if (firstIndex == 0)
                return size - 1;
            else
                return firstIndex - 1;
        }
    }

    bool canType()
    {
        return currentInvalid != -1;
    }

    bool firstOutOfSight()
    {
        return obj[firstIndex].transform.position.x + text[firstIndex].textWidth < startPos.x - 1;
    }

    bool shouldChangeChannel()
    {
        return poolEmpty >= poolSize;
    }

    int nextIndex(int offset)
    {
        int next = firstIndex + offset;
        if (next >= size)
            next -= size;

        return next;
    }

    void UpdateSubtitles()
    {
        int newInvalid = currentInvalid;
        for (int i = 0; i < size; ++i)
        {
            int next = nextIndex(i);
            if (words[next].invalid)
            {
                if (obj[next].transform.position.x < 0)
                {
                    newInvalid = next;
                    break;
                }
            }
        }
        if (newInvalid != currentInvalid)
        {
            Debug.Log("wordChanged!");
            Word word = words[newInvalid];
            ledText.text = channel.getMapping(word.text);
            currentInvalid = newInvalid;

        }

        for (int i = 0; i < size; ++i)
            text[i].transform.position = text[i].transform.position + (Vector3.left * channel.speed) * Time.deltaTime;

        if (firstOutOfSight())
        {
            Debug.Log("firstOutOfSight!");
            text[firstIndex].transform.position = text[lastIndex].transform.position + new Vector3(text[lastIndex].textWidth + SPACE_SIZE, 0, 0);

            if (channel.has(wordsNum))
            {
                Word word = channel[wordsNum];
                text[firstIndex].text = word.text;
                text[firstIndex].changeStyle(word.invalid);
                words[firstIndex] = word;
                wordsNum++;
            }
            else
            {
                text[firstIndex].text = "";
                if (poolEmpty < poolSize)
                    poolEmpty++;
            }

            ++firstIndex;
            if (firstIndex >= size)
                firstIndex = 0;
        }

        if (shouldChangeChannel())
        {
            Debug.Log("shouldChangeChannel!");
        }
    }

    void initWithChannel(Channel channel)
    {
        this.channel = channel;
        firstIndex = 0;
        wordsNum = poolSize;
        poolEmpty = 0;
        currentInvalid = -1;

        for (int i = 0; i < size; ++i)
        {
            Word word = channel[i];
            text[i].text = word.text;
            text[i].changeStyle(word.invalid);
            obj[i].transform.position = startPos;
            words[i] = word;
        }

        for (int i = 1; i < size; ++i)
        {
            obj[i].transform.position = obj[i - 1].transform.position + new Vector3(text[i - 1].textWidth + SPACE_SIZE, 0, 0);
        }
    }

    Bounds buildBounds()
    {
        float z = 0;
        Bounds b;
        b.points = new Vector3[] {
            new Vector3(rect.x, rect.y, z),
            new Vector3(rect.x + rect.width, rect.y, z),
            new Vector3(rect.x + rect.width, rect.y - rect.height, z),
            new Vector3(rect.x, rect.y - rect.height, z)
        };
        b.leftMiddle = (b.points[0] + b.points[3]) / 2;
        b.rightMiddle = (b.points[1] + b.points[2]) / 2;
        b.caret = Vector3.Lerp(b.points[0], b.points[1], rect.caretX);

        return b;
    }

    void OnDrawGizmos()
    {
        Vector3 POINT_SIZE = new Vector3(0.2f, 0.2f, 0.2f);
        Bounds b = buildBounds();
        Gizmos.DrawLine(b.points[0], b.points[1]);
        Gizmos.DrawLine(b.points[1], b.points[2]);
        Gizmos.DrawLine(b.points[2], b.points[3]);
        Gizmos.DrawLine(b.points[3], b.points[0]);
        Gizmos.DrawCube(b.leftMiddle, POINT_SIZE);
        Gizmos.DrawCube(b.rightMiddle, POINT_SIZE);

        Gizmos.DrawCube(b.caret, POINT_SIZE);
    }
}
