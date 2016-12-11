using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour {

	public CanvasGroup winMessage;
	public CanvasGroup lostMessage;
	public Image candleActionPrompt;

	public Text tutorialText1;
	public Text tutorialText2;


	// singleton
	private static UI _instance;
	public static UI instance{
		get{
			if(_instance == null) {
				_instance = FindObjectOfType<UI>();
			}
			return _instance;
		}
	}
}
