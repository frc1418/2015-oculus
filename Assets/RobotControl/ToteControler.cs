using UnityEngine;
using System.Collections;

public class ToteControler : MonoBehaviour {
	//NetworkTables netTables = new NetworkTables ();

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

	private double displacement;
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
		if (NetworkTables.Instance.connected) {
			shortLeftY =  NetworkTables.Instance.GetNumber("shortSensorValueL");
			shortRightY = NetworkTables.Instance.GetNumber("shortSensorValueR");

			longLeftY = NetworkTables.Instance.GetNumber("longSensorValueL");
			longRightY = NetworkTables.Instance.GetNumber("longSensorValueR");

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
			outOfRange = true;
		}

		/*
		 * Solve for angle
		 */
		if (sensor == SENSORS.Short) {
			slope = ((shortRightY - shortLeftY) / shortRightX);
			double value = slope / Mathf.Abs (slope);
			if (value > 0) {
				angle = Mathf.Tan (shortRightY / shortRightX);
			} else if (value < 0) {
				angle = 360 - (Mathf.Tan (shortLeftY / shortRightX));
			}
			displacement = (Mathf.Tan(angle*(Mathf.PI/180)*(shortRightX/2)));

		} else if (sensor == SENSORS.Long) {
			slope = ((longRightY - longLeftY) / longRightX);
			double value = slope / Mathf.Abs (slope);
			if (value > 0) {
				angle = ((Mathf.Tanh (longRightY / longRightX))*(180/Mathf.PI));
			} else if (value < 0) {
				angle = 360 - ((Mathf.Tanh (longLeftY / longRightX))*(180/Mathf.PI));
			}
			displacement = (Mathf.Tan(angle*(Mathf.PI/180)*(longRightX/2)));
		}

		/*
		 * While the block is not at the angle add one to the angle
		 * NOTE: THIS WILL BREAK IF THE INITAL ANGLE IS OFPUT
		 */
		while (transform.eulerAngles.z <= rotation-0.02 || transform.eulerAngles.z >= rotation+0.02) {
			transform.Rotate (Vector3.back * 1);
		}
		
		transform.position(Vector3(0, 1, 2+displacement));
	

		//Color indeication if a true surface has been detected.
		if (connected != true) {
			if(!outOfRange){
				Color red = new Color (255, 0, 0, 255);
				renderer.material.color = red;
			}else{
				Color yellow = new Color(255, 255, 0, 255);
				renderer.material.color = yellow;
			}
		} else {
			Color green = new Color (0, 255, 0, 255);
			renderer.material.color = green;
		}
	}
}
