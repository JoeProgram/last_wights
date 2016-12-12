using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayAlive : MonoBehaviour {



	// Use this for initialization
	void Start () {

		//only want one of these ever
		if(GameObject.FindGameObjectsWithTag("music").Length > 1) {
			Destroy(gameObject);
		} else {
			DontDestroyOnLoad(transform.gameObject);
		}

	}
	
}
