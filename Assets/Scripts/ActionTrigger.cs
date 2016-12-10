using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// performs an action if the player is here and presses the action key
public class ActionTrigger : MonoBehaviour {

	bool isActive = false;
	public SpriteRenderer prompt;

	void Start(){
		
	}


	void OnTriggerEnter2D( Collider2D other ){

		if(other.name == "player") {
			isActive = true;
			prompt.enabled = true;
			//display the keyboard action button
		}

	}

	void Update(){

		// the player is here and pressed the action button
		if(isActive && Input.GetAxis("Action") > 0) {
			DoAction();
		}
	}

	void OnTriggerExit2D( Collider2D other ){
		if(other.name == "player") {
			isActive = false;
			prompt.enabled = false;
			//hide the keyboard action button
		}
	}

	protected virtual void DoAction(){
		// do whatever action you need to here
	}
}
