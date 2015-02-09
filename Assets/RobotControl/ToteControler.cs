using UnityEngine;
using System.Collections;

public class ToteControler : MonoBehaviour {
	//NetworkTables netTables = new NetworkTables ();

	private double rotation = 0;
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
	private double shortLeftY = 35;
	private double shortRightY = 35;

	//Long range sensore relative pos
	private double longLeftX = 0;
	private double longRightX = 22.8;
	private double longLeftY = 200;
	private double longRightY = 200;
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
		if (sensor == SENSORS.Short) {
			leftY -= 6;
			rightY -= 5;
		} else if (sensor == SENSORS.Long) {
			leftY -= 19.5;
			leftY -= 19.5;
		}
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

		if(shortLeftY < 35 && shortRightY < 35){
			sensor = SENSORS.Short;
			outOfRange = false;
		}else if(longLeftY < 200 && longRightY < 200 ){
			sensor = SENSORS.Long;
			outOfRange = false;
		}else{
			sensor = SENSORS.OutOfRange;
			outOfRange = true;
		}

		/*
		 * Solve for angle and displacement
		 */
	
		if (sensor == SENSORS.Short) {
			calculate(shortLeftY,shortRightY, shortRightX);
			//Debug.Log ("outOfRange: " + outOfRange + " sensor: " + sensor + " LY: " + shortLeftY + " RY: " + shortRightY + " slope: " + slope + " angle:" + (float)angle + " displacement: " + displacement);

		} else if (sensor == SENSORS.Long) {
			calculate(longLeftY,longRightY,longRightX);
			//Debug.Log ("outOfRange: " + outOfRange + " sensor: " + sensor + " LY: " + longLeftY + " RY: " + longRightY + " slope: " + slope + " angle:" + (float)angle + " displacement: " + displacement);
		} else if (sensor == SENSORS.OutOfRange) {
			reset();
			//Debug.Log ("outOfRange: " + outOfRange + " sensor: " + sensor + " slope: " + slope + " angle:" + (float)angle + " displacement: " + displacement);

		}

		/*
		 * Roation and Transformation
		 */
		transform.localEulerAngles = new Vector3(0,(float)rotation,90);
		float z = (float)(displacement + 2);
		float y = 0.24f;
		transform.localPosition = new Vector3(0,y,z);

		/*
		 * Color indication of state
		 */
		if (connected != true) {
				Color red = new Color (255, 0, 0, 255);
				renderer.material.color = red;
		} else {
			if(!outOfRange){
			Color green = new Color (0, 255, 0, 255);
			renderer.material.color = green;
			}else{
				Color yellow = new Color(255, 255, 0, 255);
				renderer.material.color = yellow;
			}
		}
		reset ();
	}
}
