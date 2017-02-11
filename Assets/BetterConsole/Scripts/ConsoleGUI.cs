using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TurboTurnip.BetterConsole;

public class ConsoleGUI : MonoBehaviour {
	public bool activeConsole=true;
	public string nuCommand="",currentCommand="",autoComplete="";
	Rect defaultWindowPos=new Rect(0,0,500,150);
	GUIStyle logStyle,inputStyle;
	string[] autoCompleteFuncResult;

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

		//Debug.Log(typeof(ConsoleGUI).GetMember("activeConsole"));
	}


	void OnGUI(){
		if (!activeConsole) return;
		//inputStyle.normal.background = GUI.skin.textField.normal.background; //We can't access GUI.skin outside of GUI functions, so do it here
		GUI.Window(0,defaultWindowPos,DrawWindow,"Better Console");
	}

	void DrawWindow(int id){
		string totalLogs=BetterConsole.ConsolidateLines(); //Get the log output
		
		float requiredHeight=logStyle.CalcHeight(new GUIContent(totalLogs),defaultWindowPos.width-6);
		Rect logPos=new Rect(3,defaultWindowPos.height-20-requiredHeight,defaultWindowPos.width-6,requiredHeight);
		
		GUI.Label(logPos,totalLogs,logStyle);

		Event e=Event.current;
		int inputType=0;
		char inputChar=e.character;
		if (e.isKey&&e.type==EventType.KeyDown){
			if (e.keyCode==(KeyCode.Tab)) inputType=1;
			else if (e.keyCode==(KeyCode.Return)) inputType=2;
			else if (e.keyCode==KeyCode.Backspace) inputType=3;
			else inputType=4;//if (e.keyCode!=KeyCode.None) inputType=3;
			//inputKey=e.keyCode;
		}

		GUI.SetNextControlName("console_input");
		nuCommand=GUI.TextField(new Rect(3,defaultWindowPos.height-20,defaultWindowPos.width-6,20),currentCommand+autoComplete,inputStyle);
		//if () return;
		//GUI.GetNameOfFocusedControl()=="console_input"&&
		if (inputType==1){
			autoCompleteFuncResult=BetterConsole.AutoComplete(currentCommand,autoComplete);
			currentCommand=autoCompleteFuncResult[0];
			autoComplete=autoCompleteFuncResult[1];
			TextEditor txt = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
			txt.text=currentCommand+autoComplete;
			txt.cursorIndex = currentCommand.Length+autoComplete.Length;
			if (autoComplete!=""&&autoComplete[autoComplete.Length-1]=='\"')
				txt.cursorIndex-=1;
			txt.selectIndex=txt.cursorIndex;
		}else if (inputType==2){
			/*if (autoComplete!=""){
				currentCommand+=autoComplete;
				autoComplete="";
			}else{*/
				currentCommand+=autoComplete;
				autoComplete="";
				if (BetterConsole.ParseCommand(currentCommand))
					currentCommand="";
				
			//}
			GUI.FocusControl("console_input");
		}else if (inputType==3){
			if (autoComplete!="") autoComplete="";
			else currentCommand=nuCommand;
		}else if (GUI.GetNameOfFocusedControl()=="console_input"&&inputType==4){
			if (autoComplete==""){
				currentCommand=nuCommand;
			}else if (e.keyCode!=KeyCode.None){
				currentCommand+=autoComplete;
				autoComplete="";
			}
			GUI.FocusControl("console_input");

		}
	}
}
