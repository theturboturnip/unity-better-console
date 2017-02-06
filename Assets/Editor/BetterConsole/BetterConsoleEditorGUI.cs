using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BetterConsoleEditorGUI : EditorWindow {
	[MenuItem ("Window/Better Console")]
    public static void ShowWindow () {
        EditorWindow.GetWindow(typeof(BetterConsoleEditorGUI));
    }

    string currentCommand="";

	void OnGUI(){
		//Draw a text input at the bottom
		currentCommand=EditorGUILayout.TextField(currentCommand);
		//if (currentCommand[currentCommand.Count-1]=="\t")
		//	currentCommand=BetterConsole.
		Event e = Event.current;
        if (e.keyCode == KeyCode.Return && e.isKey){
			if (BetterConsole.ParseCommand(currentCommand))
				currentCommand="";
		}
	}
}
