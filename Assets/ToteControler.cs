using UnityEngine;
using System.Collections;

public class ToteControler : MonoBehaviour {
	public int rotation;
	public bool surfaceRecognized;

	// Use this for initialization
	void Start () {
		rotation = 90;
	}
	
	// Update is called once per frame
	void Update () {
		/*
		 * While the block is not at the angle add one to the angle
		 * NOTE: THIS WILL BREAK IF THE INITAL ANGLE IS OFPUT
		 */
		while (transform.eulerAngles.z <= rotation-0.02 || transform.eulerAngles.z >= rotation+0.02) {
			transform.Rotate (Vector3.back * 1);
		}

		//Color indeication if a true surface has been detected.
		if (surfaceRecognized != true) {
			Color red = new Color (255, 0, 0, 255);
			renderer.material.color = red;
		} else {
			Color green = new Color (0, 255, 0, 255);
			renderer.material.color = green;
		}
	}
}
