using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

// keeps track of state of level
public class Game : MonoBehaviour {

	public CanvasGroup lostMessage;

	public void Start(){
		lostMessage = GameObject.Find("message_banner").GetComponent<CanvasGroup>();
	}

	public void LoseGame(){
		StartCoroutine(LoseGameHelper());
	}

	protected IEnumerator LoseGameHelper(){

		// reveal the lost message
		DOTween.To(
			() => lostMessage.alpha,
			x => lostMessage.alpha = x,
			1,
			0.5f
		);
		yield return new WaitForSeconds(0.5f);

		yield return new WaitForSeconds(2f);
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);


	}












	// singleton
	private static Game _instance;
	public static Game instance{
		get{
			if(_instance == null) {
				_instance = FindObjectOfType<Game>();
			}
			return _instance;
		}
	}






}
