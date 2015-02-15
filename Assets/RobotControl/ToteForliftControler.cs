using UnityEngine;
using System.Collections;

public class ToteForliftControler : MonoBehaviour {
	//Top of the latter in  Latter Meters (cm for the 3d scale)
	private static float ladderTop = 200;

	public Shader defualt;
	public Shader translucent;
	public Color transBlack;

	private string smartDashTable = "";

	//Preset positions from the encoder
	private static float actualTop = 23973;
	private static float actualDisplacment = 4356;
	private static float actualBottom = 8293;
	private static float resetValue = 260;

	//State vars
	private bool calibrated = false;
	private bool connected = false;

	//Independent vars
	private float currentValue;
	private float currentLadderMeters;
	private float ladderDisplacment;

	//Rungs (0 is lowest rung)
	private GameObject[] rungs = new GameObject[3];
	private object[] rungColor = new object[3]; 
	private object[] rungShader = new object[3];
	private object[] rungPos = new object[3];

	//Rung inital pos
	private float initalXPos;
	private float initalYPos;
	private float initalZPos;

	private bool pendingUpdateRungs = false;
	private bool pendingUpdateCalibration = false;

	public float convertToLadderMeters(float value, bool raw){
		//Resets past reset value
		/*if (value / resetValue >= 0) {
			value = value % resetValue;
		}*/

		float conversionFactor = (ladderTop / (actualTop - actualBottom));
		float lm;
		if (raw) {
			lm = ((value -actualBottom) * conversionFactor);
		} else {
			lm = value * conversionFactor;
		}
		return lm/100;

	}

	// Use this for initialization
	void Start () {
		Debug.Log("Tote Forklift Active");

		rungs[0] = GameObject.Find("/Tote_ForkLift_Sim/RungBottom");
		rungs[1] = GameObject.Find("/Tote_ForkLift_Sim/RungMiddle");
		rungs[2] = GameObject.Find ("/Tote_ForkLift_Sim/RungTop");

		for(int i = 0; i<=2; i++){
			rungColor[i] = rungs[i].renderer.material.color;
			rungShader[i] = rungs[i].renderer.material.shader;
			rungPos[i] = rungs[i].transform.localPosition;
		}
		
		transBlack = new Color(0,0,0,0f);

		translucent = Shader.Find ("Transparent/Diffuse");
		defualt = Shader.Find ("Diffuse");

		initalXPos = rungs[0].transform.localPosition.x;
		initalYPos = rungs[0].transform.localPosition.y;
		initalZPos = rungs[0].transform.localPosition.z;

		NetworkTables.Instance.AddListener (smartDashTable+"Tote Forklift|Calibrated", setUpdate);
		NetworkTables.Instance.AddListener (smartDashTable+"Tote Forklift|Encoder", setUpdate);
	}

	void setUpdate(string key, object value){
		if (key.Equals ("Tote Forklift|Calibrated")) {
			pendingUpdateCalibration = true;
		} else if (key.Equals ("Tote Forklift|Encoder")) {
			pendingUpdateRungs = true;
		}
	}
	void updateCalibration(){
		//Debug.Log("Calibrate");
		NetworkTables.Instance.GetBool (smartDashTable + "Tote Forklift|Calibrated", out calibrated);
	}

	void updateRungs(){
		//Debug.Log("Rungs");
		updateColor ();
		
		double grabbedValue;
		NetworkTables.Instance.GetNumber(smartDashTable+"Tote Forklift|Encoder", out grabbedValue);
		currentValue = (float)grabbedValue;

		currentLadderMeters = convertToLadderMeters(currentValue, true);
		ladderDisplacment = convertToLadderMeters(actualDisplacment, false);
		
		for(int i = 0; i<=2; i++){
			float currentDisplacement = ladderDisplacment * i;
			float localCurrent = currentLadderMeters + currentDisplacement;
			float rawValue = initalYPos + localCurrent;
			
			if(rawValue >=-1 && rawValue <= (ladderTop/200)){
				//Debug.Log("On: "+i);
				float x = initalXPos;
				float y = initalYPos + localCurrent;
				float z = initalZPos;
				rungs[i].transform.localPosition = new Vector3(x, y, z);
				
			}else if(rawValue < -1){
				//Debug.Log("Under on: "+i);
				float x = (-1*initalXPos);
				float y = initalYPos + (-1*localCurrent);
				float z = initalZPos;
				rungs[i].transform.localPosition = new Vector3(x, y, z);
				
			}else{
				//Debug.Log("Over on: "+i);
				float x = (-1*initalXPos);
				float y = (ladderTop/200) - (localCurrent - (ladderTop/100));
				float z = initalZPos;
				rungs[i].transform.localPosition = new Vector3(x,y,z);
			}
		}
	}

	void updateColor(){
		//Debug.Log("Color");
		if (!connected) {
			for(int i = 0; i<=2; i++){
				rungColor[i] = rungs[i].renderer.material.color = transBlack;
				rungs[i].renderer.material.shader = translucent;
			}
		} else {
			if(calibrated){
				for(int i = 0; i<=2; i++){
					rungColor[i] = rungs[i].renderer.material.color = Color.green;
					rungs[i].renderer.material.shader = defualt;
					
				}
			}else{
				for(int i = 0; i<=2; i++){
					rungColor[i] = rungs[i].renderer.material.color = Color.yellow;
					rungs[i].renderer.material.shader = defualt;
				}
			}
		}
	}

	// Update is called once per frame
	void Update () {
		if (NetworkTables.Instance.connected) {
			connected = true;
		} else {
			connected = false;
		}

		if (connected) {
			if(pendingUpdateCalibration){
				updateCalibration();
				pendingUpdateCalibration = false;
			}
			if(calibrated){
				if(pendingUpdateRungs){
					updateRungs();
					pendingUpdateRungs = false;
				}
			}
		}
		//Debug.Log ("Value: " + currentValue + " LadderValue: " + currentLadderMeters +"Displacement: "+actualDisplacment+" LDiplace: "+ladderDisplacment+ " Connected: " + connected + " Calibrated: " + calibrated);
	}
}
