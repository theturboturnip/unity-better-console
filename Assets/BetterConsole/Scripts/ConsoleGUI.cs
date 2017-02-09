using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleGUI : MonoBehaviour {
	public bool activeConsole=true;
	public string nuCommand="",currentCommand="",autoComplete="";
	Rect defaultWindowPos=new Rect(0,0,500,150);
	void OnGUI(){
		if (!activeConsole) return;
		GUI.Window(0,defaultWindowPos,DrawWindow,"Better Console");
		//Debug.Log("I LOVE LOGS");
	}

	void DrawWindow(int id){
		string totalLogs=BetterConsole.ConsolidateLines();
		float height=defaultWindowPos.height-3-30;
		float requiredHeight=GUI.skin.label.CalcSize(new GUIContent(totalLogs)).y;
		Rect logPos=new Rect(3,defaultWindowPos.height-15-requiredHeight,defaultWindowPos.width-6,requiredHeight);
		GUIStyle style=new GUIStyle();
		style.richText=true;
		//style.font=new Font("Courier New");
		GUI.Label(logPos,totalLogs,style);

		Event e=Event.current;
		int inputType=0;
		if (e.isKey&&e.type==EventType.KeyDown){
			if (e.keyCode==(KeyCode.Tab)) inputType=1;
			else if (e.keyCode==(KeyCode.Return)) inputType=2;
			else inputType=3;
		}

		GUI.SetNextControlName("console_input");
		nuCommand=GUI.TextField(new Rect(3,defaultWindowPos.height-20,defaultWindowPos.width-6,20),currentCommand+autoComplete);
		//if () return;
		//GUI.GetNameOfFocusedControl()=="console_input"&&
		if (inputType==1){
			autoComplete=BetterConsole.AutoComplete(currentCommand,autoComplete);
			TextEditor txt = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
			txt.text=currentCommand+autoComplete;
			txt.cursorIndex = currentCommand.Length+autoComplete.Length;
			txt.selectIndex=txt.cursorIndex;
		}else if (inputType==2){
			if (autoComplete!=""){
				currentCommand+=autoComplete;
				autoComplete="";
			}else{
				BetterConsole.ParseCommand(currentCommand);
				currentCommand="";
				autoComplete="";
			}
			GUI.FocusControl("console_input");

		}else if (GUI.GetNameOfFocusedControl()=="console_input"&&inputType==3){
			if (autoComplete==""){
				currentCommand=nuCommand;
			}else{
				currentCommand+=autoComplete;
				autoComplete="";
			}
			GUI.FocusControl("console_input");

		}
	}
}
