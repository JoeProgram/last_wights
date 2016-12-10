using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// rooms help keep track of where objects are.
public class Room : MonoBehaviour {

	List<Ghost> ghosts;

	// Use this for initialization
	void Start () {
		ghosts = new List<Ghost>();
	}
	
	void OnTriggerEnter2D( Collider2D other ){

		if(other.CompareTag("ghost")) {
			ghosts.Add(other.gameObject.GetComponent<Ghost>());
		}

	}

	void OnTriggerExit2D( Collider2D other ){

		if(other.CompareTag("ghost") && ghosts.Contains(other.GetComponent<Ghost>())) {
			ghosts.Remove(other.GetComponent<Ghost>());
		}

	}

	public List<Ghost> GetGhosts(){
		return ghosts;
	}
}
