using UnityEngine;
using System.Collections;

public class CanForklifControler : MonoBehaviour {
	//Top of the latter in  Latter Meters (cm for the 3d scale)
	private static float ladderTop = 200;

	//Shaders
	public Shader defualt;
	public Shader translucent;
	public Color transBlack;

	private string smartDashTable = "";
	
	//Preset positions from the encoder
	private static float actualTop = 10180;
	private static float actualBottom = 0;

	//State vars
	private bool calibrated = false;
	private bool connected = false;
	
	//Independent vars
	private float currentValue;
	private float currentLadderMeters;

	//Rungs (0 is lowest rung)
	private GameObject lift;
	
	//Rung inital pos
	private float initalXPos;
	private float initalYPos;
	private float initalZPos;

	private bool pendingUpdateCalibration = false;
	private bool pendingUpdateLift = false;
	
	public float convertToLadderMeters(float value){
		float conversionFactor = (ladderTop / (actualTop - actualBottom));
		float lm;
		lm = value * conversionFactor;
		return lm/100;
		
	}
	
	// Use this for initialization
	void Start () {
		Debug.Log("Can Forklift Active");
		
		lift = GameObject.Find("/Can_ForkLift_Sim/Lift");

		transBlack = new Color(0,0,0,0f);

		translucent = Shader.Find ("Transparent/Diffuse");
		defualt = Shader.Find ("Diffuse");
		
		initalXPos = lift.transform.localPosition.x;
		initalYPos = lift.transform.localPosition.y;
		initalZPos = lift.transform.localPosition.z;

		NetworkTables.Instance.AddListener (smartDashTable+"Can Forklift|Calibrated", setUpdate);
		NetworkTables.Instance.AddListener (smartDashTable+"Can Forklift|Encoder", setUpdate);
	}

	void setUpdate(string key, object value){
		if (key.Equals ("Can Forklift|Calibrated")) {
			pendingUpdateCalibration = true;
		} else if (key.Equals ("Can Forklift|Encoder")) {
			pendingUpdateLift = true;
		}
	}

	void updateCalibration(){
		NetworkTables.Instance.GetBool (smartDashTable + "Can Forklift|Calibrated", out calibrated);
	}

	void updateLift(){
		double grabbedValue;
		NetworkTables.Instance.GetNumber(smartDashTable+"Can Forklift|Encoder", out grabbedValue );
		currentValue = (float)grabbedValue;

		currentLadderMeters = convertToLadderMeters(currentValue);
		float rawValue = initalYPos + currentLadderMeters;
		
		
		if(rawValue >=-1 && rawValue <= (ladderTop/200)){
			lift.transform.localPosition = new Vector3(initalXPos, rawValue, initalZPos);
		}
		updateColor ();
	}

	void updateColor(){
		if (!connected) {
			//Debug.Log("I AM CHANGING SHIT BRO");
			lift.renderer.material.shader = translucent;
			lift.renderer.material.color = transBlack;
		}else{
			lift.renderer.material.shader = defualt;
			if(calibrated){
				lift.renderer.material.color = Color.green;
			}else{
				lift.renderer.material.color = Color.yellow;
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
			if(calibrated && pendingUpdateLift){
				updateLift();
				pendingUpdateLift = false;
			}
		}
		

		//Debug.Log ("Connect: " + connected + " Calibrated: " + calibrated +" CurrentValue: " + currentLadderMeters); //+"Displacement: "+actualDisplacment+" LDiplace: "+ladderDisplacment+ " Connected: " + connected + " Calibrated: " + calibrated);
	}
}
