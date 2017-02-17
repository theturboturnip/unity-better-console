using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TurboTurnip.BetterConsole;

public class BetterConsoleGUI : MonoBehaviour {
	public bool showConsole=true;
	public KeyCode consoleKey=KeyCode.LeftBracket;
	string nuCommand="",currentCommand="",autoComplete="";
	Rect defaultWindowPos=new Rect(0,0,500,150);
	GUIStyle logStyle,inputStyle;
	string[] autoCompleteFuncResult;
	Vector2 scrollPos=Vector2.zero;
	List<string> commandMemory;
	int memoryIndex=1;

	void Start(){
		//Initialize Styles so we don't constantly create new ones in OnGUI()
		logStyle=new GUIStyle();
		logStyle.richText=true;
		logStyle.wordWrap=true;
		logStyle.fixedWidth=defaultWindowPos.width-6;
		logStyle.font=(Font)Resources.Load("BetterConsoleCourier");
		logStyle.fontSize=14;

		inputStyle=new GUIStyle();
		inputStyle.font=logStyle.font;
		inputStyle.fontSize=logStyle.fontSize;
		inputStyle.border=new RectOffset(4,4,4,4);

		Application.logMessageReceived+=OnLog;
		commandMemory=new List<string>();

		GameObject[] roots=UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
		foreach (GameObject root in roots){
			root.BroadcastMessage("RegisterCommands",SendMessageOptions.DontRequireReceiver);
		}
	}

	public void Update(){
		if (ShouldToggle())
			showConsole=!showConsole;
	}

	public virtual bool ShouldToggle(){
		return Input.GetKeyDown(consoleKey);
	}


	void OnGUI(){
		if (!showConsole) return;
		GUI.Window(0,defaultWindowPos,DrawWindow,"Better Console");
	}

	void DrawWindow(int id){
		string totalLogs=BetterConsole.ConsolidateLines(); //Get the log output
		
		//Figure positioning, draw scroll box and console output
		float requiredHeight=logStyle.CalcHeight(new GUIContent(totalLogs),defaultWindowPos.width-6);
		Rect logPos=new Rect(3,defaultWindowPos.height-25-requiredHeight,defaultWindowPos.width-25,requiredHeight);
		scrollPos=GUI.BeginScrollView(new Rect(3,20,defaultWindowPos.width-6,defaultWindowPos.height-20-20),scrollPos,logPos);
		GUI.Label(logPos,totalLogs,logStyle);
		GUI.EndScrollView();

		//Intercept input before the TextField swallows it
		Event e=Event.current;
		KeyCode inputKey=KeyCode.None;
		bool keyDown=false;
		if (e.isKey&&e.type==EventType.KeyDown){
			inputKey=e.keyCode;
			keyDown=true;
		}

		//Draw the input
		GUI.SetNextControlName("console_input");
		nuCommand=GUI.TextField(new Rect(3,defaultWindowPos.height-20,defaultWindowPos.width-6,20),currentCommand+autoComplete,inputStyle);

		if (inputKey==KeyCode.Tab){
			autoCompleteFuncResult=BetterConsole.AutoComplete(currentCommand,autoComplete);
			currentCommand=autoCompleteFuncResult[0];
			autoComplete=autoCompleteFuncResult[1];
			TextEditor txt = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
			txt.text=currentCommand+autoComplete;
			txt.cursorIndex = currentCommand.Length+autoComplete.Length;
			if (autoComplete!=""&&autoComplete[autoComplete.Length-1]=='\"')
				txt.cursorIndex-=1;
			txt.selectIndex=txt.cursorIndex;
		}else if (inputKey==KeyCode.Return){
			currentCommand+=autoComplete;
			autoComplete="";
			if (BetterConsole.ParseCommand(currentCommand)){
				memoryIndex=1;
				commandMemory.Add(currentCommand);
				if (commandMemory.Count>BetterConsole.maxRememberedCommands)
					commandMemory.RemoveAt(0);
				currentCommand="";
			}
			scrollPos=Vector2.up*(requiredHeight+10);
			GUI.FocusControl("console_input");
		}else if (inputKey==KeyCode.Backspace){
			if (autoComplete!="") autoComplete="";
			else currentCommand=nuCommand;
		}else if (inputKey==KeyCode.UpArrow){
			autoComplete="";
			memoryIndex=Mathf.Clamp(memoryIndex-1,-commandMemory.Count+1,1);
			if (memoryIndex+commandMemory.Count-1<commandMemory.Count && memoryIndex+commandMemory.Count-1>=0)
				currentCommand=commandMemory[memoryIndex+commandMemory.Count-1];
		}else if (inputKey==KeyCode.DownArrow){
			autoComplete="";
			memoryIndex=Mathf.Clamp(memoryIndex+1,-commandMemory.Count+1,1);
			if (memoryIndex+commandMemory.Count-1<commandMemory.Count && memoryIndex+commandMemory.Count-1>=0)
				currentCommand=commandMemory[memoryIndex+commandMemory.Count-1];
			else if (memoryIndex==1)
				currentCommand="";
		}else if (GUI.GetNameOfFocusedControl()=="console_input"&&keyDown){
			if (autoComplete==""){
				currentCommand=nuCommand;
			}else if (e.keyCode!=KeyCode.None){
				currentCommand+=autoComplete;
				autoComplete="";
			}
			GUI.FocusControl("console_input");

		}
	}

	void OnLog(string msg,string trace,LogType type){
		scrollPos.y+=10000000000000; //Make sure we scroll to the bottom
	}
}

