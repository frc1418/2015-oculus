﻿using UnityEngine;
using System.Collections;

public class ToteControler : MonoBehaviour {
	//NetworkTables netTables = new NetworkTables ();

	private float initalX;
	private float initalY;
	private float initalZ;

	private double rotation = 0;
	private bool connected;

	private string smartDashTable = "/SmartDashboard/";
	
	private enum SENSORS
	{
		Long,
		Short,
		SoloL,
		SoloR,
		Lim,
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
	private double longRightX = 12.5;
	private double longLeftY = 200;
	private double longLeftDist;
	private double longRightY = 200;
	private double longRightDist;

	private bool updateLim = true;
	private bool updateSensor = true;

	//Limit switches
	private bool lim1;
	private bool lim2;

	//Lim switchs
	//private bool limSwitch
	// Use this for initialization
	private void reset(){
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

		//Listeners
		NetworkTables.Instance.AddListener (smartDashTable+"toteLimitL", setUpdate);
		NetworkTables.Instance.AddListener (smartDashTable+"toteLimitR", setUpdate);

		NetworkTables.Instance.AddListener (smartDashTable+"shortSensorValueL", setUpdate);
		NetworkTables.Instance.AddListener (smartDashTable+"shortSensorValueR", setUpdate);

		NetworkTables.Instance.AddListener (smartDashTable+"longSensorValueL", setUpdate);
		NetworkTables.Instance.AddListener (smartDashTable+"longSensorValueR", setUpdate);
	}


	//Sets the updates
	void setUpdate(string key, object value){
		if (key.Equals (smartDashTable+"toteLimitL") || key.Equals (smartDashTable+"toteLimitL")) {
			updateLim = true;
		} else {
			updateSensor = true;
		}
	}


	void updateLimSwitches() {
		//Debug.Log ("Updated");
		NetworkTables.Instance.GetBool(smartDashTable+"toteLimitL", out lim1);
		NetworkTables.Instance.GetBool(smartDashTable+"toteLimitR", out lim2);

		//If both pressed
		if (!lim1 && !lim2) {
			sensor = SENSORS.Lim;
		}
		updateTote ();
	}

	void updateSensors(){
		if (lim1 || lim2) {
			NetworkTables.Instance.GetNumber (smartDashTable + "shortSensorValueL", out shortLeftY);
			NetworkTables.Instance.GetNumber (smartDashTable + "shortSensorValueR", out shortRightY);

			NetworkTables.Instance.GetNumber (smartDashTable + "longSensorValueL", out longLeftY);
			NetworkTables.Instance.GetNumber (smartDashTable + "longSensorValueR", out longRightY);


			shortLeftDist = shortLeftY - 7.5;
			shortRightDist = shortRightY - 6;
			longLeftDist = longLeftY - 19.5;
			longRightDist = longRightY - 19.5;
		
		
			if (shortLeftY < 0 && shortRightY < 0) {
				sensor = SENSORS.Short;
			
			} else if (longLeftY < 145 && longRightY < 145) {
				sensor = SENSORS.Long;
			
			} else if (shortLeftY < 35) {
				sensor = SENSORS.SoloL;
				displacement = shortLeftDist / 100;
			
			} else if (shortRightY < 35) {
				sensor = SENSORS.SoloR;
				displacement = shortRightDist / 100;
			
			} else if (longLeftY < 145) {
				sensor = SENSORS.SoloL;
				displacement = longLeftDist / 100;
			
			} else if (longRightY < 145) {
				sensor = SENSORS.SoloR;
				displacement = longRightDist / 100;
			
			} else {
				sensor = SENSORS.OutOfRange;
			}
			updateTote ();
		}
	}

	void updateTote(){
		updateLocalVars ();
		updateToteColor();
		updateTotePos ();
		updateToteOrientation ();
		updateToteScale ();
	}

	void updateLocalVars(){
		if (sensor == SENSORS.Short) {
			calculate(shortLeftDist,shortRightDist, shortRightX);
			//Debug.Log ("sensor: " + sensor + " LY: " + shortLeftY + " RY: " + shortRightY + " slope: " + slope + " angle:" + (float)angle + " displacement: " + displacement);
			
		} else if (sensor == SENSORS.Long) {
			calculate(longLeftDist,longRightDist,longRightX);
			//Debug.Log ("sensor: " + sensor + " LY: " + longLeftY + " RY: " + longRightY + " slope: " + slope + " angle:" + (float)angle + " displacement: " + displacement);
		}else if (sensor == SENSORS.OutOfRange || sensor == SENSORS.Lim) {
			reset();
			//Debug.Log ("sensor: " + sensor + " slope: " + slope + " angle:" + (float)angle + " displacement: " + displacement);
		}
	}

	void updateToteScale(){
		if (sensor == SENSORS.SoloL) {
			transform.localScale = new Vector3(0.53f, 0.25f, 0.3f);
		} else if (sensor == SENSORS.SoloR) {
			transform.localScale = new Vector3(0.53f, 0.25f, 0.3f);
		} else {
			transform.localScale = new Vector3(0.53f, 0.5f, 0.3f);
		}
	}

	void updateToteColor (){
		if (connected != true) {
			renderer.material.color = Color.red;
		} else {
			if(sensor == SENSORS.Lim){
				renderer.material.color = Color.green;

			}else if(sensor == SENSORS.Long || sensor == SENSORS.Short){
				renderer.material.color = Color.cyan;

			}else if(sensor == SENSORS.SoloL || sensor == SENSORS.SoloR){
				renderer.material.color = Color.blue;

			}else if(sensor == SENSORS.OutOfRange){
				renderer.material.color = Color.yellow;

			}
		}
	}

	void updateTotePos (){
		float x = initalX;
		float y = initalY;
		float z = (float)(initalZ+displacement);

		if (sensor == SENSORS.SoloL) {
			x = -0.25f;
		} else if (sensor == SENSORS.SoloR) {
			x = 0.25f;
		} else {
			x = 0f;
		}

		transform.localPosition = new Vector3(x,y,z);
	}

	void updateToteOrientation(){
		transform.localEulerAngles = new Vector3(0,(float)rotation,90);
	}

	// Update is called once per frame
	void Update () {
		if (NetworkTables.Instance.connected) {
			connected = true;
		} else {
			connected = false;
		}

		if (updateLim) {
			updateLimSwitches();
			updateLim = false;
		}
		if (updateSensor) {
			updateSensors ();
			updateSensor = false;
		} else {
			updateToteColor();
		}
		//Debug.Log ("Sensor: " + sensor);
	}
}
