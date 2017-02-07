using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BetterConsoleEditorGUI : EditorWindow {
	[MenuItem ("Window/Better Console")]
    public static void ShowWindow () {
        EditorWindow.GetWindow(typeof(BetterConsoleEditorGUI));
    }

    string currentCommand="",autoComplete="";

	void OnGUI(){
		//Draw a text input at the bottom
		//currentCommand=EditorGUILayout.TextField(currentCommand);
		//if (currentCommand[currentCommand.Count-1]=="\t")
		//	currentCommand=BetterConsole.
		Event e = Event.current;
		if (e.isKey && e.type==EventType.KeyDown){
			if (e.keyCode == KeyCode.Return){
				if (BetterConsole.ParseCommand(currentCommand)){
					currentCommand="";
					autoComplete="";
				}
			}else if (e.keyCode== KeyCode.Tab){
				autoComplete=BetterConsole.AutoComplete(currentCommand,autoComplete);
				Debug.Log(currentCommand+autoComplete);
			}else if (e.keyCode==KeyCode.Backspace){
				autoComplete="";
			}
		}
		EditorGUI.BeginChangeCheck();
		//CHANGE ME
        string nuCommand=EditorGUILayout.TextField(currentCommand+autoComplete);
        if (EditorGUI.EndChangeCheck()){
        	currentCommand=nuCommand;
        	if (autoComplete!="")
        		autoComplete="";
        }
	}
}
