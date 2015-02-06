using UnityEngine;
using System.Collections;
using System;
using System.Json;

public class Send{
	public Send(){
		string key = "Apple";
		string value = "Pear";
	}
}

public class NetworkTables : MonoBehaviour {
	static NetworkTables instance;

	public static NetworkTables getInstance() {
		return instance;
	}
	
	// Use this for initialization
	void Start () {
		instance = this;
		Debug.Log ("MOOO!");
		using (var ws = new WebSocket ("ws://localhost:8888/ws")) {
			Send send = new Send();
			ws.OnOpen += (sender, e) => {
				Debug.Log("Connected.");
			};

			ws.OnMessage += (sender, e) => {
				Debug.Log("Localhost says: " + e.Data);
			};

			ws.OnError += (sender, e) => {
				Debug.Log("EEROOR: "+ e.Message);
			};

			ws.Connect ();
			ws.Send(send);
		}
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log("Math hacked");
	}
}