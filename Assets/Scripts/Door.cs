using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

	public SpriteRenderer openDoor;
	public List<Room> rooms;

	void Start(){
		Bounds triggerBounds = transform.GetChild(0).GetComponent<BoxCollider2D>().bounds;

		foreach(Room room in GameObject.FindObjectsOfType<Room>()) {
			if(room.GetComponent<BoxCollider2D>().bounds.Intersects(triggerBounds)) {
				rooms.Add(room);
			}
		}
	}

	public void OpenDoor(){
		GetComponent<BoxCollider2D>().enabled = false;
		GetComponent<SpriteRenderer>().enabled = false;
		openDoor.enabled = true;
		openDoor.flipX = Player.instance.transform.position.x > transform.position.x;

		// reveal everything in connected rooms
		foreach( Room room in rooms) room.OpenRoom(); 

	}
	

}
