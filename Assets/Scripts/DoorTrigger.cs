using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : ActionTrigger {

	public Door door;



	protected override void DoAction(){

		door.OpenDoor();
		if( sfxAction != null ) AudioSource.PlayClipAtPoint(sfxAction, Camera.main.transform.position);
		Destroy(gameObject);
	}
}
