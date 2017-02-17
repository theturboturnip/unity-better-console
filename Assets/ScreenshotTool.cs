#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ScreenshotTool : MonoBehaviour {
	//public KeyCode screenshotKey; 
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.M))
			TakeScreenshot();
	}

	void TakeScreenshot(){
		//Debug.Log("Screenshot");
		string path=Application.dataPath+"/"+System.DateTime.Now.ToString("hhmmss ddmmyyyy")+".png";
		//Debug.Log("Saved screenshot at "+path);
		Application.CaptureScreenshot(path);
	}
}
#endif