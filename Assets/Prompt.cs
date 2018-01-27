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
}

struct Bounds
{
    public Vector3[] points;
    public Vector3 leftMiddle;
    public Vector3 rightMiddle;
}

public class CycleList
{
    int firstIndex = 0;
    int poolSize;
    int words = 0;
    int poolEmpty = 0;

    const float SPACE_SIZE = 1;

    public GameObject[] obj;
    public BlockText[] text;

    Channel channel;
    Vector3 startPos;

    public CycleList(Vector3 startPos, int poolSize)
    {
        this.poolSize = poolSize;
        this.startPos = startPos;
        obj = new GameObject[poolSize];
        text = new BlockText[poolSize];
        poolEmpty = 0;
    }

    public int size { get { return poolSize; } }

    public int lastIndex {
        get
        {
            if (firstIndex == 0)
                return size - 1;
            else
                return firstIndex - 1;
        }
    }

    public bool firstOutOfSight()
    {
        return obj[firstIndex].transform.position.x + text[firstIndex].textWidth < startPos.x - 1;
    }

    public bool shouldChangeChannel()
    {
        return poolEmpty >= poolSize;
    }

    public void Update()
    {
        for (int i = 0; i < size; ++i)
            text[i].transform.position = text[i].transform.position + (Vector3.left * channel.speed) * Time.deltaTime; 

        if (firstOutOfSight())
        {
            Debug.Log("firstOutOfSight!");
            text[firstIndex].transform.position = text[lastIndex].transform.position + new Vector3(text[lastIndex].textWidth + SPACE_SIZE, 0, 0);

            if (channel.has(words))
            {
                text[firstIndex].text = channel[words];
                words++;
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

    public void initWithChannel(Channel channel)
    {
        this.channel = channel;
        firstIndex = 0;
        words = poolSize;
        poolEmpty = 0;

        for (int i = 0; i < size; ++i)
        {
            text[i].text = channel[i];
            obj[i].transform.position = startPos;
        }

        for (int i = 1; i < size; ++i)
        {
            //Debug.Log("textSize for: " + i + " = " + text[i - 1].textWidth);
            obj[i].transform.position = obj[i - 1].transform.position + new Vector3(text[i - 1].textWidth + SPACE_SIZE, 0, 0);
        }
    }
}


public class Prompt : MonoBehaviour
{
    public Frame rect;
    public GameObject textPrefab;
    public Tv tv;

    Bounds bounds;
    CycleList list;
    Channel channel;

    const int POOL_SIZE = 10;

    public void changeChannel(Channel channel)
    {
        list.initWithChannel(channel);
    }

    void Awake()
    {
        bounds = buildBounds();
        list = new CycleList(bounds.leftMiddle, 5);
        for (int i = 0; i < list.size; ++i)
        {
            list.obj[i] = Instantiate(textPrefab, bounds.leftMiddle, Quaternion.identity, transform);
            list.text[i] = list.obj[i].GetComponent<BlockText>();
        }
    }

    void Update()
    {
        list.Update();
        if (list.shouldChangeChannel())
        {
            tv.nextChannel();
        }
        //if (Input.inputString.Length > 0)
        //{
        //    char c = Input.inputString[0];
        //    if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
        //    {
        //        string chr = c.ToString().ToUpper();
        //        text[0].text = text[0].text + chr;
        //        for (int i = 1; i < 5; ++i)
        //        {
        //            text[i].transform.position = text[i].transform.position + new Vector3(0.3f, 0, 0);

        //        }


        //    }
        //}
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
    }
}
