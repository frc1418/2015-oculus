using UnityEngine;
using System.Collections;

public class StreamScreen : MonoBehaviour {

	GameObject screen;
	WebCamTexture webcamTexture;

	// Use this for initialization
	void Start () {

		// TODO: Error message?
		if (WebCamTexture.devices.Length == 0)
			return;

		screen = GameObject.Find ("Screen");

		// setup a texture.. 
		var sz = screen.renderer.bounds.size;

		var deviceName = "";

		foreach (WebCamDevice device in WebCamTexture.devices) {
			// use the name of the device to pick the Lifecam instead of the webcam?
			// ... or, use a config file?
			//Debug.Log("Device:"+device.name+ "IS FRONT FACING:"+ device.isFrontFacing);
		}

		if (deviceName == "")
			deviceName = WebCamTexture.devices [0].name;

		//
		webcamTexture = new WebCamTexture (deviceName, 720, 1280, 30);
		webcamTexture.Play ();

		screen.renderer.material.mainTexture = webcamTexture;
	}
	
	// Update is called once per frame
	void Update () {
		// at the moment, this doesn't do anything
		// -> in the future, switch streams/textures/something on driver input
	}
}
