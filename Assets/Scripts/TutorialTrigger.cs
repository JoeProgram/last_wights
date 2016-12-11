using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour {

	public string tutorialText1;
	public string tutorialText2;

	void OnTriggerEnter2D( Collider2D other ){
		if(other.name == "player") {

			UI.instance.tutorialText1.text = tutorialText1;
			UI.instance.tutorialText2.text = tutorialText2;

		}
	}
}
