using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour {

	public string tutorialText1;
	public string tutorialText2;

	public SpriteRenderer prompt;
	public bool requiresCandle;

	void OnTriggerStay2D( Collider2D other ){
		if(other.name == "player") {

			// if this requires the candle to trigger, check that first
			if(requiresCandle && !Player.instance.HasCandle()) {
				return;
			}
			

			UI.instance.tutorialText1.text = tutorialText1;
			UI.instance.tutorialText2.text = tutorialText2;

			if( prompt != null ) prompt.gameObject.SetActive(true);

			Destroy(this.gameObject);
		}
	}
}
