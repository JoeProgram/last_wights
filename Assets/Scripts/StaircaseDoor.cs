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

	protected override void DoAction(){
		StartCoroutine(MovePlayer());
	}

	protected IEnumerator MovePlayer(){

		connectedDoor.inputProcessed = true; //kludge - we need the other door to already have counted the keypress as processed, though it may not be active yet
		Player.instance.transform.position = connectedDoor.transform.position;


		foreach(Room room in connectedDoor.rooms) {
			room.OpenRoom();
		}

		StaircaseDoor.LastUsedDoor = this;

		yield return null;
	}
	

}
