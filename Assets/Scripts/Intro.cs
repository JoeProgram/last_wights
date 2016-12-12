using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour {

	public Image blackout;
	public float displayTime = 3;

	// Use this for initialization
	void Start () {
		StartCoroutine(StartHelper());
	}

	IEnumerator StartHelper(){

		yield return new WaitForSeconds(0.25f);

		DOTween.To(
			() => blackout.color,
			x => blackout.color = x,
			new Color(0,0,0,0),
			1f);
		
		yield return new WaitForSeconds(displayTime);

		DOTween.To(
			() => blackout.color,
			x => blackout.color = x,
			Color.black,
			1f);

		yield return new WaitForSeconds(1.25f);

		//load next level
		SceneManager.LoadScene(  (SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings);


	}

}
