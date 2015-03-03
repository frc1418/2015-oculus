using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.IO;
using System;
using System.Threading;

public class RobotCamera : MonoBehaviour {
	private int fps = 10;
	private int compression = 30;
	private int size = 1;

	private int nextSize;

	private byte[] buffer = null;
	private MemoryStream ms;

	private Stream s;

	private StreamWriter sw;

	byte[] read(Stream s, int count){
		byte[] localBuffer = new byte[count];
		if (count > 0) {
			s.Read (buffer, 0, count);
		}
		if (localBuffer.Length != count) {
			Debug.LogError("Read count not eual to requested");
		}
		return localBuffer;
	}

	// Use this for initialization
	void Start () {
		Debug.Log ("Robo Vison active");
		TcpClient client = new TcpClient("localhost",1180);
		s = client.GetStream ();
		sw = new StreamWriter (s);

		sw.Write (BitConverter.GetBytes (fps));
		sw.Write (BitConverter.GetBytes (compression));
		sw.Write (BitConverter.GetBytes (size));
		sw.Flush ();

		Thread thread = new Thread(new ThreadStart(Read));
		thread.Start();
	}

	void Read(){
		buffer = read(s, 4);
		
		buffer = read(s, 4);
		nextSize = BitConverter.ToInt32 (buffer,0);
		Debug.Log ("Size: " + nextSize+" Bytes: "+buffer);
		
		buffer = read(s, nextSize);

		var tex = new Texture2D (4, 4);
		tex.LoadImage (buffer);
		renderer.material.mainTexture = tex;
	}

	// Update is called once per frame
	void Update () {

	}
}
