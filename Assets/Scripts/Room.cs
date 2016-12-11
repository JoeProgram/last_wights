using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// rooms help keep track of where objects are.
public class Room : MonoBehaviour {

	List<Ghost> ghosts;
	bool isOpen = false;

	public GameObject roomShadowPrefab;
	protected GameObject roomShadow;

	// Use this for initialization
	void Start () {

		ghosts = new List<Ghost>();


		// the room the player starts in is automatically revealed
		if(Player.instance.IsInRoom(this)) {
			OpenRoom();

		} else {

			roomShadow = Instantiate(roomShadowPrefab);
			roomShadow.transform.position = transform.position;
			roomShadow.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y * 3) ; //have to stretch the shadow up

			// disable all the contents of this room until the room is opened
			foreach(Transform t in transform) {
				t.gameObject.SetActive(false);

				if(t.gameObject.GetComponent<Ghost>() != null) {
					ghosts.Add(t.gameObject.GetComponent<Ghost>());
				}
			}

		}



	}
		
	void OnTriggerEnter2D( Collider2D other ){

		if(other.CompareTag("ghost_sprite")) {
			ghosts.Add(other.transform.parent.gameObject.GetComponent<Ghost>());
		}

	}

	void OnTriggerExit2D( Collider2D other ){

		if(other.CompareTag("ghost_sprite") && ghosts.Contains(other.transform.parent.GetComponent<Ghost>())) {
			ghosts.Remove(other.transform.parent.GetComponent<Ghost>());
		}

	}

	public List<Ghost> GetGhosts(){
		return ghosts;
	}

	// reveal everything in this room
	public void OpenRoom(){

		if(!isOpen) {
			isOpen = true;


			if( roomShadow != null ) Destroy(roomShadow);

			foreach(Transform t in transform) {
				t.gameObject.SetActive(true);
			}

		}
	}
}
