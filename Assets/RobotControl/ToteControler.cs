using UnityEngine;
using System.Collections;

public class ToteControler : MonoBehaviour {
	NetworkTables netTables = new NetworkTables ();

	public int rotation;
	private bool outOfRange;
	private bool connected;

	private enum SENSORS
	{
		Long,
		Short,
		OutOfRange
	};
	private SENSORS sensor; 

	private double slope;
	private double angle;

	//Short range sensor relative pos
	private double shortLeftX = 0;
	private double shortRightX = 34.8;
	private double shortLeftY;
	private double shortRightY;

	//Long range sensore relative pos
	private double longLeftX = 0;
	private double longRightX = 22.8;
	private double longLeftY;
	private double longRightY;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (netTables.connected) {
			shortLeftY = netTables.table["shortSensorValueL"];
			shortRightY = netTables.table["shortSensorValueR"];

			longLeftY = netTables.table["longSensorValueL"];
			longRightY = netTables.table["longSensorValueR"];

			connected = true;
		} else {
			connected = false;
		}

		if(shortLeftY >= 35 || shortLeftY >= 35){
			sensor = SENSORS.Long;
		}else if(longLeftY >= 200 || longRightY >= 200 ){
			sensor = SENSORS.Short;
		}else{
			sensor = SENSORS.OutOfRange;
		}

		if(sensor == SENSORS.Short){
			slope = ((shortRightY - shortLeftY)/shortRightX);
			//angle
		}
		/*
		 * While the block is not at the angle add one to the angle
		 * NOTE: THIS WILL BREAK IF THE INITAL ANGLE IS OFPUT
		 */
		while (transform.eulerAngles.z <= rotation-0.02 || transform.eulerAngles.z >= rotation+0.02) {
			transform.Rotate (Vector3.back * 1);
		}

		//Color indeication if a true surface has been detected.
		if (connected != true) {
			Color red = new Color (255, 0, 0, 255);
			renderer.material.color = red;
		} else {
			Color green = new Color (0, 255, 0, 255);
			renderer.material.color = green;
		}
	}
}
