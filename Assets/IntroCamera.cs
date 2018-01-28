using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCamera : MonoBehaviour
{
    public static bool PLAY = true;

    //public float fov;
    public float size;

    Camera cam;

	void Start()
    {
        PLAY = false;
        cam = GetComponent<Camera>();
        //fov = cam.fieldOfView;
        size = cam.orthographicSize;
	}
	
	void Update()
    {
        //cam.fieldOfView = fov;
        cam.orthographicSize = size;

    }
}
