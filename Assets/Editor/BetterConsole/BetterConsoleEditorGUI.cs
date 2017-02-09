using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*public class BetterConsoleEditorGUI : EditorWindow {
	[MenuItem ("Window/Better Console")]
	public static void ShowWindow () {
		EditorWindow.GetWindow(typeof(BetterConsoleEditorGUI));
	}

	string currentCommand="",autoComplete="",nuCommand="";

	void OnGUI(){
		//EditorGUI.LabelField();

		//string nuCommand=currentCommand+autoComplete;
		string totalLogs=BetterConsole.ConsolidateLines();
		float height=position.height-3-30;
		float requiredHeight=GUI.skin.label.CalcSize(new GUIContent(totalLogs)).y;
		Rect logPos=new Rect(3,position.height-30-requiredHeight,position.width-6,requiredHeight);
		GUIStyle style=new GUIStyle();
		style.richText=true;
		EditorGUI.LabelField(logPos,totalLogs,style);


		Event e = Event.current;
		KeyCode keyCodePressed=KeyCode.None;
		bool keyPressed=false;
		if (e.isKey &&e.type==EventType.KeyUp){
			GUI.FocusControl(null);

			keyCodePressed=e.keyCode;
			keyPressed=true;
		//Debug.Log(e.keyCode==KeyCode.Tab);
		
		this.Repaint();
		}
		//GUI.FocusControl("console_input");


		GUI.SetNextControlName("console_input");
		//EditorGUI.BeginChangeCheck();
		nuCommand=EditorGUI.TextField(new Rect(3,position.height-30,position.width-6,30),currentCommand+autoComplete);
		//Debug.Log(nuCommand);
		GUI.FocusControl("console_input");
		//Debug.Log(nuCommand);
		//if (keyPressed) GUI.FocusControl(null);
		if (keyCodePressed==KeyCode.Tab){
			//Debug.Log("Tabb");
			//if (autoComplete=="")
			//	currentCommand=nuCommand.Substring(0,nuCommand.Length-1);

			autoComplete=BetterConsole.AutoComplete(currentCommand,autoComplete);
			Debug.Log(currentCommand+" auto "+autoComplete);
		}else if (keyCodePressed==KeyCode.Return){
			if (autoComplete!=""){
				currentCommand+=autoComplete;
				autoComplete="";
			}else{
				BetterConsole.ParseCommand(currentCommand);
				currentCommand="";
				autoComplete="";
			}
			GUI.FocusControl("console_input");

		}else if (keyPressed){
			if (autoComplete==""){
				Debug.Log(e.character);
				currentCommand+=e.character;//nuCommand;
			}else{
				currentCommand+=autoComplete;
				autoComplete="";
			}
			GUI.FocusControl("console_input");

		}
		/*Event e = Event.current;
		if (e.isKey) {
		  Debug.Log("Key");
		  Debug.Log(e);
		  Debug.Log(nuCommand);
		}		

		if (e.isKey && e.type==EventType.KeyUp){
			if (e.keyCode== KeyCode.Tab){
				GUI.FocusControl(null);
				autoComplete=BetterConsole.AutoComplete(currentCommand,autoComplete);
				Debug.Log(currentCommand+autoComplete);
			}else if (e.keyCode==KeyCode.Backspace){
				GUI.FocusControl(null);
				if (autoComplete!="")
					nuCommand=currentCommand;
				else
					nuCommand=nuCommand.Substring(nuCommand.Length-1);
				autoComplete="";

			}else if (e.keyCode==KeyCode.Return){//} && GUI.GetFocusedControl()=="console_input"){
				GUI.FocusControl(null);
				Debug.Log(nuCommand);
				currentCommand=nuCommand;
				if (autoComplete==""&&BetterConsole.ParseCommand(currentCommand)){
					currentCommand="";
					autoComplete="";
				}
				autoComplete="";
				nuCommand="";
			}else if(e.character!='\0'){
				currentCommand+=e.character;
				autoComplete="";
			}
			this.Repaint();
		}
		//EditorGUI.BeginChangeCheck();
		//CHANGE ME
		//GUI.SetNextControlName("console_input");
		//EditorGUILayout.TextField(currentCommand+autoComplete);
		//EditorGUILayout.LabelField(currentCommand+autoComplete);
		//if (EditorGUI.EndChangeCheck()){
		//	currentCommand=nuCommand;
		//	if (autoComplete!="")
		//		autoComplete="";
		//}
		
		
		
	}

	void DrawLogs(){
		
	}
}
*/