using UnityEngine;
using System.Collections;

public class LampControler : MonoBehaviour {
	private GameObject directionLamp;
	private bool connected;
	private bool reversedDrive;

	// Use this for initialization
	void Start () {
		Debug.Log("Lamp Controler Active");
		directionLamp = GameObject.Find ("/Screen_Pos/Lamps/DirectionLamp");

	}
	
	// Update is called once per frame
	void Update () {
		if (NetworkTables.Instance.connected) {
			connected = true;
			NetworkTables.Instance.GetBool ("toteCalibrated", out reversedDrive);
		} else {
			connected = false;
		}

		if (connected) {
			if(reversedDrive){
				Color red = new Color (255, 0, 0, 255);
				directionLamp.renderer.material.color = red;
			}else{
				Color green = new Color (0, 255, 0, 255);
				directionLamp.renderer.material.color = green;
			}
		} else {
			directionLamp.renderer.material.color = Color.gray;
		}
	}
}
