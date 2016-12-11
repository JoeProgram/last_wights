using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaircaseDoor : ActionTrigger {

	public static StaircaseDoor LastUsedDoor; // globally keeps track of the last staircase door used

	public StaircaseDoor connectedDoor; // where this lets out

	public List<Room> rooms; // rooms (probably 1) this is associated with

	void Start(){
		Bounds triggerBounds = transform.GetComponent<BoxCollider2D>().bounds;

		foreach(Room room in GameObject.FindObjectsOfType<Room>()) {
			if(room.GetComponent<BoxCollider2D>().bounds.Intersects(triggerBounds)) {
				rooms.Add(room);
			}
		}
	}


	// also allow W or Up
	protected virtual void Update(){

		// the player is here and pressed the action button
		if(isActive && !inputProcessed && (Input.GetAxis("Action") > 0 || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) )) {
			DoAction();
		}

		if(Input.GetAxis("Action") > 0) {
			inputProcessed = true;
		} else {
			inputProcessed = false;
		}
	}

	protected override void DoAction(){
		StartCoroutine(MovePlayer());
	}

	protected IEnumerator MovePlayer(){

		connectedDoor.inputProcessed = true; //kludge - we need the other door to already have counted the keypress as processed, though it may not be active yet
		Player.instance.transform.position = connectedDoor.transform.position;

		if( sfxAction != null ) AudioSource.PlayClipAtPoint(sfxAction, Camera.main.transform.position);


		foreach(Room room in connectedDoor.rooms) {
			room.OpenRoom();
		}

		StaircaseDoor.LastUsedDoor = this;

		yield return null;
	}
	

}
