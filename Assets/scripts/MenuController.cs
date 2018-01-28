using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuController : MonoBehaviour
{
	public Animator animator;

	AudioSource audioPlayer;
	public AudioClip sndStart;

	void Start () {
		audioPlayer = GetComponent<AudioSource>();
	}

	public void onPlayClicked()
	{
		animator.SetTrigger ("pressed");
		audioPlayer.PlayOneShot (sndStart);
            SceneManager.LoadScene("game_scene");
	}

	public void OnPointerEnter()
	{	
		animator.SetTrigger ("hover");
		Debug.Log("The cursor entered the selectable UI element.");

	}

	public void OnPointerLeave()
	{	
		animator.SetTrigger ("idle");
		Debug.Log("The cursor entered the selectable UI element.");

	}
}