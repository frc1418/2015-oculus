using UnityEngine;
using System.Collections;
using System;
//using WebSocket;


public class NetworkTables : MonoBehaviour {
	static NetworkTables instance;

	public static NetworkTables getInstance() {
		return instance;
	}
	
	// Use this for initialization
	void Start () {
		instance = this;
		Debug.Log ("MOOO!");
		/*using (var ws = new WebSocket ("ws://localhost:8888/ws")) {
			JSONObject j = new JSONObject(JSONObject.Type.OBJECT);
			j.Add("key", "Apple");
			j.Add("value", "Pear");
			string encodedString = j.print();
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
			ws.Send(encodedString);
		}*/
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log("Math hacked");
	}
}