using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopingSound : MonoBehaviour {

	AudioSource audio;
	public AudioClip sndStart;
	public AudioClip sndRepeat;
	public AudioClip sndEnd;
	bool startPlayed = false;

	// Use this for initialization
	void Start () {
		audio = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey ("space")) {
			if (!startPlayed) {
				audio.clip = sndStart;
				audio.Play();
				startPlayed = true;
			}
			if (!audio.isPlaying) {
				audio.clip = sndRepeat;
				audio.Play ();
			}
		} 

		if (Input.GetKeyUp("space")) {
			audio.PlayOneShot(sndEnd);
			startPlayed = false;
		}
	}
}
