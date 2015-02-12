using UnityEngine;
using System.Collections;

public class KeyLisener : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Q)) {
			Application.Quit();
		}
	}
}
