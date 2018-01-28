using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class IntroCamera : MonoBehaviour
{
    bool PLAY = true;

    public float size;

    Camera cam;
    float time;

	void Start()
    {
        cam = GetComponent<Camera>();
        size = cam.orthographicSize;

	}
	
	void Update()
    {
        time += Time.deltaTime;
        cam.orthographicSize = size;
        if (PLAY && time >= 6)
        {
            SceneManager.LoadScene("menu_scene");
            PLAY = false;
        }

    }
}
