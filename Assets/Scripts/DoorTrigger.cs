using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : ActionTrigger {

	public Door door;



	protected override void DoAction(){

		door.OpenDoor();
		Destroy(gameObject);
	}
}
