using UnityEngine;
using System.Collections;

public class ToteControler : MonoBehaviour {
	//NetworkTables netTables = new NetworkTables ();

	private float initalX;
	private float initalY;
	private float initalZ;

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
	private double shortRightX = 31;
	private double shortLeftY = 35;
	private double shortLeftDist;
	private double shortRightY = 35;
	private double shortRightDist;
		
	//Long range sensore relative pos
	private double longRightX = 20.5;
	private double longLeftY = 200;
	private double longLeftDist;
	private double longRightY = 200;
	private double longRightDist;

	//Limit switches
	private bool lim1;
	private bool lim2;

	//Lim switchs
	//private bool limSwitch
	// Use this for initialization
	private void reset(){
		longRightX = 22.8;
		longLeftY = 200;
		longRightY = 200;

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

		initalX = transform.localPosition.x;
		initalY = transform.localPosition.y;
		initalZ = transform.localPosition.z;
	}
	
	// Update is called once per frame
	void Update () {
		if (NetworkTables.Instance.connected) {
			NetworkTables.Instance.GetNumber("shortSensorValueL", out shortLeftY );
			NetworkTables.Instance.GetNumber("shortSensorValueR", out shortRightY);

			NetworkTables.Instance.GetNumber("longSensorValueL", out longLeftY);
			NetworkTables.Instance.GetNumber("longSensorValueR", out longRightY);

			NetworkTables.Instance.GetBool("lim1", out lim1);
			NetworkTables.Instance.GetBool("lim2", out lim2);

			connected = true;
		} else {
			connected = false;
		}

		//Relative to front of robot
		shortLeftDist = shortLeftY - 7.5;
		shortRightDist = shortRightY - 6;
		longLeftDist = longLeftY - 19.5;
		longRightDist = longRightY - 19.5;


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
		float x = initalX;
		float z = (float)(displacement + initalZ);
		float y = initalY;

		if (sensor == SENSORS.SoloL) {
			transform.localScale = new Vector3(0.53f, 0.25f, 0.3f);
			x = -0.25f;
		} else if (sensor == SENSORS.SoloR) {
			transform.localScale = new Vector3(0.53f, 0.25f, 0.3f);
			x = 0.25f;
		} else {
			transform.localScale = new Vector3(0.53f, 0.5f, 0.3f);
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
				if(!lim1 && !lim2){
					Color green = new Color (0, 255, 0, 255);
					renderer.material.color = green;
				}else{
					Color light_blue = new Color (0, 255, 255, 255);
					renderer.material.color = light_blue;
				}
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
