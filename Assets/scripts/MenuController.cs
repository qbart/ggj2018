using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
	public void onPlayClicked()
	{
		Debug.Log ("aaaa");
		SceneManager.LoadScene("game_scene");
	}
}