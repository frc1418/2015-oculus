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
			double value = slope / Mathf.Abs ((float)slope);

			if (value > 0) {
				angle = Mathf.Atan ((float)(shortRightY / shortRightX));
			} else if (value < 0) {
				angle = 360 - (Mathf.Atan ((float)(shortLeftY / shortRightX)));
			}

			displacement = (Mathf.Tan((int)(angle*(Mathf.PI/180)*(shortRightX/2))));

		} else if (sensor == SENSORS.Long) {
			slope = ((longRightY - longLeftY) / longRightX);
			double value = slope / Mathf.Abs ((float)slope);

			if (value > 0) {
				angle = ((Mathf.Atan ((float)(longRightY / longRightX)))*(180/Mathf.PI));
			} else if (value < 0) {
				angle = 360 - ((Mathf.Atan ((float)(longLeftY / longRightX)))*(180/Mathf.PI));
			}

			displacement = (Mathf.Tan((int)(angle*(Mathf.PI/180)*(longRightX/2))));
		}
		if (!outOfRange) {
			while (transform.eulerAngles.z <= rotation-1 || transform.eulerAngles.z >= rotation+1) {
				transform.Rotate (Vector3.right * 1);
			}
			float z = (float)(displacement + 2);
			transform.Translate (0, 1, z);
		}

		//Color indeication if a true surface has been detected.
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
	}
}
