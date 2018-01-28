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
    public float markerX;
}

struct Bounds
{
    public Vector3[] points;
    public Vector3 leftMiddle;
    public Vector3 rightMiddle;
    public Vector3 marker;
}

struct Player
{
    public int score;
    public int channelScore;

    public void successWord()
    {
        score += 1;
    }

    public void failureWord()
    {
        score -= 1;
        channelScore -= 1;
    }

    public void successLevel()
    {
        score += 1;
    }

    public bool wantsToChangeChannel()
    {
        return channelScore <= 0;
    }

    public bool gameOver()
    {
        return score <= 0;
    }

    public void resetChannel()
    {
        channelScore = 2;
    }
}


struct AnimState
{
    public int prev;
    public int curr;
    public string trigger;
}

public class Prompt : MonoBehaviour
{
    public Frame rect;
    public GameObject textPrefab;
    public GameObject marker;
    public Tv tv;
    public TextMesh ledText;
    public Animator handAnim;
    public Animator tvAnim;
	public AudioClip clickSnd;
	public AudioClip keyDownSnd;
	public AudioClip tvOff;
	public AudioClip failSnd;
	public AudioClip overSnd;
	AudioSource audioPlayer;

    Bounds bounds;

    int firstIndex = 0;
    int poolSize;
    int wordsNum = 0;
    int poolEmpty = 0;
    int currentInvalid = -1;
    int channelChangeState = 0;
    bool requestChannelChange;
    int state = 0;

    const float SPACE_SIZE = 0.25f;

    GameObject[] obj;
    BlockText[] text;
    Word[] words;

    Channel channel;
    Vector3 startPos;

    Player player;

    AnimState animState;

    const int POOL_SIZE = 20;

    public void changeChannel(Channel channel)
    {
        initWithChannel(channel);
    }

    void onValidAnswer()
    {
        Debug.Log("correct answer!");
        player.successWord();
    }

    void onWrongAnswer()
    {
        Debug.Log("wrong answer!");
        player.failureWord();
        animState.trigger = "angry";
		if (!player.gameOver()) {
			audioPlayer.PlayOneShot (failSnd);
		}
    }

    void onWordMiss()
    {
        Debug.Log("missed answer");
        player.failureWord();
        animState.trigger = "angry";
		if (!player.gameOver()) {
			audioPlayer.PlayOneShot (failSnd);
		}
    }

    void onLevelFailed()
    {
        Debug.Log("level failed!");

        player.resetChannel();
		audioPlayer.PlayOneShot(clickSnd);
        animState.trigger = "click";

        requestChannelChange = true;
    }

    void onLevelSuccess()
    {
        Debug.Log("text finished!");

        player.successLevel();
        player.resetChannel();

		audioPlayer.PlayOneShot(clickSnd);
        animState.trigger = "click";

        requestChannelChange = true;
    }

    void onGameEnded()
    {
        Debug.Log("game ended!");
		audioPlayer.PlayOneShot(overSnd);
        audioPlayer.PlayOneShot(clickSnd);
        animState.trigger = "click";

        tvAnim.gameObject.SetActive(true);
        tvAnim.SetTrigger("turnoff");
		audioPlayer.PlayOneShot(tvOff);
        ledText.gameObject.SetActive(false);

        foreach (Transform bt in GetComponentsInChildren<Transform>())
        {
            bt.gameObject.SetActive(false);
        }
    }

    void onWordReachedMarker(Word word)
    {
        //Debug.Log("word reacher marker! " + word.text);

        if (word.skip)
        {
            if (word.success == 1)
                onValidAnswer();
            else if (word.success == 0)
                onWrongAnswer();
        }
        else if (word.success == 2)
            onWordMiss();
    }

	void Start()
    {
		audioPlayer = GetComponent<AudioSource>();
        tvAnim.gameObject.SetActive(false);
        Vector3 pos = marker.transform.position;
        pos.x = bounds.marker.x;
        marker.transform.position = pos;
    }

    void Awake()
    { 
        requestChannelChange = false;
        animState.curr = 0;
        animState.prev = 0;
        animState.trigger = "";

        player.score = 5;
        player.resetChannel();
        bounds = buildBounds();
        startPos = bounds.leftMiddle;
        poolSize = POOL_SIZE;
        obj = new GameObject[poolSize];
        text = new BlockText[poolSize];
        words = new Word[poolSize];
        channelChangeState = 0;

        for (int i = 0; i < size; ++i)
        {
            obj[i] = Instantiate(textPrefab, bounds.leftMiddle, Quaternion.identity, transform);
            text[i] = obj[i].GetComponent<BlockText>();
        }

        state = 0;
    }

    void Update()
    {
        if (animState.trigger != "")
        {
            handAnim.SetTrigger(animState.trigger);
            animState.trigger = "";
        }

        int oldAnim = animState.curr;
        animState.curr = handAnim.GetInteger("animIndex");
        animState.prev = oldAnim;

        bool clickEnded = animState.prev == 1 && animState.curr != 1;
        bool angryEnded = animState.prev == 2 && animState.curr != 2;

        // game over
        if (state == 1)
        {
            return;
        }

        if (player.gameOver())
        {
            state = 1;
            onGameEnded();
            return;
        }

        if (requestChannelChange)
        {
            requestChannelChange = false;
            tv.nextChannel();
            channelChangeState = 0;
        }

        UpdateSubtitles();

        bool switchToNextLevel = shouldChangeChannel();
        bool playerSwitchToNextLevel = player.wantsToChangeChannel();

        if (switchToNextLevel || playerSwitchToNextLevel)
        {
            if (channelChangeState == 0)
                channelChangeState = 1;

            if (channelChangeState == 1)
            {
                if (playerSwitchToNextLevel)
                    onLevelFailed();
                else
                    onLevelSuccess();

                channelChangeState = 2;
            }
        }
        else if (canType() && Input.inputString.Length > 0)
        {
            char c = Input.inputString[0];
            if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
            {
                //Debug.Log("char: " + c);
				audioPlayer.PlayOneShot(keyDownSnd);
                Word word = words[currentInvalid];
                word.addChar(c);
                text[currentInvalid].text = word.text;

                // check if valid
                if (channel.reachedLength(word))
                {
                    if (channel.isValid(word))
                    {
                        if (channel.markAsValid(word))
                            text[currentInvalid].text = word.text;

                        word.skip = true;
                        word.success = 1;
                        text[currentInvalid].changeStyle(1);
                        //onValidAnswer();
                    }
                    else
                    {
                        word.skip = true;
                        word.success = 0;
                        text[currentInvalid].changeStyle(2);
                        //onWrongAnswer();
                    }
                }

                for (int i = 1; i < size; ++i)
                {
                    int prev = nextIndex(i - 1);
                    int next = nextIndex(i);
                    if (obj[next].transform.position.x > obj[currentInvalid].transform.position.x)
                    {
                        obj[next].transform.position = obj[prev].transform.position + new Vector3(text[prev].textWidth + SPACE_SIZE, 0, 0);
                    }
                }
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
        foreach (int index in getVisibleIndices())
        {
            if (words[index].check && obj[index].transform.position.x < bounds.marker.x && words[index].invalid)
            {
                words[index].check = false;
                if (!words[index].skip)
                {
                    words[index].success = 2;
                    text[index].changeStyle(2);
                }
                onWordReachedMarker(words[index]);
            }
        }

        int newInvalid = currentInvalid;
        bool found = false;
        foreach (int index in getActiveIndices())
        {
            if (words[index].invalid)
            {
                found = true;
                newInvalid = index;
                break;
            }
        }
        if (!found)
            newInvalid = -1;

        if (newInvalid != currentInvalid)
        {
            //string s = "";
            //foreach (int i in getVisibleIndices())
            //    s += words[i].text + ", ";
            //Debug.Log("visible = " + s);

            //s = "";
            //foreach (int i in getActiveIndices())
            //    Debug.Log("active = " + i + " " + words[i].text);

            if (newInvalid >= 0)
            {
                Word word = words[newInvalid];
                ledText.text = channel.getMapping(word.text);
            }
            else
            {
                ledText.text = "";
                //onNoMoreWords();
            }
            currentInvalid = newInvalid;
        }

        for (int i = 0; i < size; ++i)
            text[i].transform.position = text[i].transform.position + (Vector3.left * channel.speed) * Time.deltaTime;

        if (firstOutOfSight())
        {
            //Debug.Log("firstOutOfSight!");
            text[firstIndex].transform.position = text[lastIndex].transform.position + new Vector3(text[lastIndex].textWidth + SPACE_SIZE, 0, 0);

            if (channel.has(wordsNum))
            {
                // take next
                Word word = channel[wordsNum];
                text[firstIndex].text = word.text;
                text[firstIndex].changeStyle(word.invalid ? 3 : 0);
                words[firstIndex] = word;
                wordsNum++;
            }
            else
            {
                text[firstIndex].text = "";
                words[firstIndex].invalid = false;
                if (poolEmpty < poolSize)
                    poolEmpty++;
            }

            ++firstIndex;
            if (firstIndex >= size)
                firstIndex = 0;
        }
    }

    void initWithChannel(Channel channel)
    {
        this.channel = channel;
        firstIndex = 0;
        wordsNum = poolSize;
        poolEmpty = 0;
        currentInvalid = -1;

        // init
        for (int i = 0; i < size; ++i)
        {
            Word word = channel[i];
            text[i].text = word.text;
            text[i].changeStyle(word.invalid ? 3 : 0);
            obj[i].transform.position = startPos;
            words[i] = word;
            words[i].width = text[i].textWidth;
        }

        for (int i = 1; i < size; ++i)
        {
            obj[i].transform.position = obj[i - 1].transform.position + new Vector3(text[i - 1].textWidth + SPACE_SIZE, 0, 0);
        }
    }

    List<int> getVisibleIndices()
    {
        List<int> indices = new List<int>();

        for (int i = 0; i < size; ++i)
        {
            int next = nextIndex(i);
            if (obj[next].transform.position.x >= bounds.leftMiddle.x && obj[next].transform.position.x <= bounds.rightMiddle.x)
                indices.Add(next);
        }

        return indices;
    }

    List<int> getActiveIndices()
    {
        List<int> indices = new List<int>();

        foreach (int index in getVisibleIndices())
        {
            if (obj[index].transform.position.x >= bounds.marker.x && obj[index].transform.position.x <= bounds.rightMiddle.x)
                if (!words[index].skip)
                    indices.Add(index);
        }

        return indices;
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
        b.marker = Vector3.Lerp(b.points[0], b.points[1], rect.markerX);

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

        Gizmos.DrawCube(b.marker, POINT_SIZE);
    }
}
