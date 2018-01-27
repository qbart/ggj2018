using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopingSound : MonoBehaviour {

	AudioSource audioPlayer;
	public AudioClip sndStart;
	public AudioClip sndRepeat;
	public AudioClip sndEnd;
	bool startPlayed = false;

	// Use this for initialization
	void Start () {
		audioPlayer = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey ("space")) {
			if (!startPlayed) {
				audioPlayer.clip = sndStart;
				audioPlayer.Play();
				startPlayed = true;
			}
			if (!audioPlayer.isPlaying) {
				audioPlayer.clip = sndRepeat;
				audioPlayer.Play ();
			}
		} 

		if (Input.GetKeyUp("space")) {
			audioPlayer.PlayOneShot(sndEnd);
			startPlayed = false;
		}
	}
}
