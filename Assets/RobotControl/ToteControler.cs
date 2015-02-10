using UnityEngine;
using System.Collections;

public class ToteControler : MonoBehaviour {
	//NetworkTables netTables = new NetworkTables ();

	private double rotation = 0;
	private bool connected;

	private enum SENSORS
	{
		Long,
		Short,
		SoloL,
		SoloR,
		OutOfRange
	};
	private SENSORS sensor; 

	private double displacement;
	private double slope;
	private double angle;

	//Short range sensor relative pos
	private double shortLeftX = 0;
	private double shortRightX = 34.8;
	private double shortLeftY = 35;
	private double shortLeftDist;
	private double shortRightY = 35;
	private double shortRightDist;
		
	//Long range sensore relative pos
	private double longLeftX = 0;
	private double longRightX = 22.8;
	private double longLeftY = 200;
	private double longLeftDist;
	private double longRightY = 200;
	private double longRightDist;

	//Distance from front of robot
	//private double 

	//Lim switchs
	//private bool limSwitch
	// Use this for initialization
	private void reset(){
		longLeftX = 0;
		longRightX = 22.8;
		longLeftY = 200;
		longRightY = 200;

		shortLeftX = 0;
		shortRightX = 34.8;
		shortLeftY = 35;
		shortRightY = 35;

		angle = 0;
		rotation = 0;
		displacement = 0;
	}

	public void calculate(double leftY, double rightY, double x){

		slope = ((rightY - leftY) / x);
		double value = (float)slope / Mathf.Abs ((float)slope);
		
		if (value > 0) {
			double opposite = rightY - leftY;
			double tan = opposite / x;
			angle = (Mathf.Atan ((float)tan))* (180 / Mathf.PI);

			rotation = 360 - angle;

			double angleRad = angle * (Mathf.PI / 180);
			double x2 = x / 2;
			double o2 = (Mathf.Tan ((float)angleRad)) * x2;
			displacement = (o2 + leftY) / 100;
		} else if (value < 0) {
			double opposite = leftY - rightY;
			double tan = opposite / x;
			angle = (Mathf.Atan ((float)tan))* (180 / Mathf.PI);

			rotation = 360 - angle;
			
			double angleRad = angle * (Mathf.PI / 180);
			double x2 = x / 2;
			double o2 = (Mathf.Tan ((float)angleRad)) * x2;
			displacement = (o2 + rightY) / 100;
		}else if(slope == 0){
			angle = 0;
			rotation = 0;
			displacement = shortLeftY/100;
		}
	}

	void Start () {
		Debug.Log("Tote Controler Active");
	}
	
	// Update is called once per frame
	void Update () {
		if (NetworkTables.Instance.connected) {
			NetworkTables.Instance.GetNumber("shortSensorValueL", out shortLeftY );
			NetworkTables.Instance.GetNumber("shortSensorValueR", out shortRightY);

			NetworkTables.Instance.GetNumber("longSensorValueL", out longLeftY);
			NetworkTables.Instance.GetNumber("longSensorValueR", out longRightY);

			connected = true;
		} else {
			connected = false;
		}

		//Relative to front of robot
		shortLeftDist = shortLeftY - 5.5;
		shortRightDist = shortRightY - 5.5;
		longLeftDist = longLeftY - 19.5;
		longRightDist = longRightY - 19.5;

		Debug.Log ("sL: " + shortLeftY + " sR: " + shortRightY + " lL: " + longLeftY + " lR: " + longRightY);

		if (shortLeftY < 35 && shortRightY < 35) {
			sensor = SENSORS.Short;

		} else if (longLeftY < 200 && longRightY < 200) {
			sensor = SENSORS.Long;

		} else if (shortLeftY < 35){
			sensor = SENSORS.SoloL;
			displacement = shortLeftDist/100;

		} else if (shortRightY < 35){
			sensor = SENSORS.SoloR;
			displacement = shortRightDist/100;

		}else if (longLeftY < 200){
			sensor = SENSORS.SoloL;
			displacement = longLeftDist/100;

		}else if (longRightY < 200){
			sensor = SENSORS.SoloR;
			displacement = longRightDist/100;

		}else{
			sensor = SENSORS.OutOfRange;
		}
		Debug.Log ("Sensor: " + sensor);

		/*
		 * Solve for angle and displacement
		 */
	
		if (sensor == SENSORS.Short) {
			calculate(shortLeftDist,shortRightDist, shortRightX);
			//Debug.Log ("outOfRange: " + outOfRange + " sensor: " + sensor + " LY: " + shortLeftY + " RY: " + shortRightY + " slope: " + slope + " angle:" + (float)angle + " displacement: " + displacement);

		} else if (sensor == SENSORS.Long) {
			calculate(longLeftDist,longRightDist,longRightX);
			//Debug.Log ("outOfRange: " + outOfRange + " sensor: " + sensor + " LY: " + longLeftY + " RY: " + longRightY + " slope: " + slope + " angle:" + (float)angle + " displacement: " + displacement);
		}else if (sensor == SENSORS.OutOfRange) {
			reset();
			//Debug.Log ("outOfRange: " + outOfRange + " sensor: " + sensor + " slope: " + slope + " angle:" + (float)angle + " displacement: " + displacement);

		}

		/*
		 * Roation and Transformation
		 */
		float x = 0;
		float z = (float)(displacement + 2.5);
		float y = 0.24f;

		if (sensor == SENSORS.SoloL) {
			Debug.Log("SOLOL");
			transform.localScale = new Vector3(0.53f, 0.5f, 0.3f);
			x = -0.25f;
		} else if (sensor == SENSORS.SoloR) {
			Debug.Log("SOLOR");
			transform.localScale = new Vector3(0.53f, 0.5f, 0.3f);
			x = 0.25f;
		} else {
			transform.localScale = new Vector3(0.53f, 1f, 0.3f);
			x = 0f;
		}
		transform.localEulerAngles = new Vector3(0,(float)rotation,90);

		transform.localPosition = new Vector3(x,y,z);

		/*
		 * Color indication of state
		 */
		if (connected != true) {
				Color red = new Color (255, 0, 0, 255);
				renderer.material.color = red;
		} else {
			if(sensor == SENSORS.Long || sensor == SENSORS.Short){
			Color green = new Color (0, 255, 0, 255);
			renderer.material.color = green;
			}else if(sensor == SENSORS.SoloL || sensor == SENSORS.SoloR){
				Color blue = new Color(0, 0, 255, 255);
				renderer.material.color = blue;
			}else if(sensor == SENSORS.OutOfRange){
				Color yellow = new Color(255, 255, 0, 255);
				renderer.material.color = yellow;
			}
		}
		reset ();
	}
}
