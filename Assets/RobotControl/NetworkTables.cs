﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;

using ServiceStack.Text;
using WebSocketSharp;

/// <summary>
/// An interface to NetworkTables via a websocket. Does not need to be added
/// to a game element, as this is taken care of by the Singleton class.
/// 
/// This is setup as a singleton, to access the underlying object use this:
/// 
/// 	NetworkTables.Instance
/// </summary>
public class NetworkTables : Singleton<NetworkTables> {

	// properties

	public string websocketURL = "ws://127.0.0.1:8888/ws";

	// variables

	protected Dictionary<string, object> table = new Dictionary<string, object> ();

	// websocket instance
	protected WebSocket ws = null;
	public bool connected = false;
	public bool destroyed = false;

	private string pastMessage;

	// do not allow creation of this instance
	protected NetworkTables() {
	}

	class NumberMessage {
		public string key;
		public double value;
		public string action = "write";
	}

	class StringMessage {
		public string key;
		public string value;
		public string action = "write";
	}

	// Use this for initialization
	void Start () {
		Debug.Log("NetworkTables Active");
		ws = new WebSocket (websocketURL);

		// setup event handlers
		ws.OnOpen += (object sender, EventArgs e) => {

			connected = true;
			Debug.Log ("Connected to " + websocketURL);

			// access the queue of waiting writes, and write them out
		};

		ws.OnMessage += (object sender, WebSocketSharp.MessageEventArgs e) => {

			// decode the json
			JsonObject o = JsonObject.Parse(e.Data);

			// store it in the dictionary
			if(!table.ContainsKey(o.Get("key"))){
				table.Add(o.Get("key"), o.Get("value"));
			}else{
				table[o.Get("key")] = o.Get("value");
			}
			//Debug.Log("Key: "+o.Get("key")+"Value: "+o.Get("value"));
		};

		ws.OnError += (object sender, ErrorEventArgs e) => {
			if (e.Exception != null && e.Message != pastMessage){
				Debug.LogException(e.Exception);
				pastMessage = e.Message;
			}
		};

		ws.OnClose += (object sender, WebSocketSharp.CloseEventArgs e) => {

			// if the websocket disconnects, automatically try to reconnect to it!

			if (connected)
				Debug.Log ("Websocket disconnected!");

			connected = false;

			// not running on the main thread, so it's ok to sleep here
			if (!destroyed) {
				Thread.Sleep(1000);
				ws.ConnectAsync();
			}
		};
		// do the connect
		ws.ConnectAsync ();
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnDestroy() {
		destroyed = true;
	}

	#region API

	// returns true if a value was retrieved, false otherwise
	public bool GetString(string key, out string value) {
		object tmpValue;
		if (table.TryGetValue (key, out tmpValue)) {
			value = (string)tmpValue;
			return true;
		}

		value = null;
		return false;
	}

	// returns true if a value was retrieved, false otherwise
	public bool GetNumber(string key, out double value) {
		object tmpValue;
		if (table.TryGetValue (key, out tmpValue)) {
			value = Convert.ToDouble(tmpValue);
			return true;
		}
		
		value = 0;
		return false;
	}

	public void PutNumber(string key, double value) {

		if (ws == null || ws.ReadyState != WebSocketState.Open) {
			// TODO: queue up any writes

			return;
		}

		// create the json blob here
		var msg = new NumberMessage ();
		msg.key = key;
		msg.value = value;
		
		//ws.SendAsync(msg.ToJson(), null);
	}

	public void PutString(string key, string value) {

		if (ws == null || ws.ReadyState != WebSocketState.Open) {
			// TODO: queue up any writes
		}
		Debug.Log (key+" "+value);
		var msg = new StringMessage();
		
		Debug.Log(msg);
		msg.key = key;
		msg.value = value;
		//string json = msg.ToJson ();
		//Debug.Log ("Sending: "+ json);
		//ws.SendAsync(json, null);
	}

	#endregion

}