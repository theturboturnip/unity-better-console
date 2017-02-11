using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurboTurnip.Utility{

public static class IniParse {
	public static Dictionary<string,Dictionary<string,string>> ParseINI(string rawText){
		Dictionary<string,Dictionary<string,string>> data=new Dictionary<string,Dictionary<string,string>>();
		string[] rawLines=rawText.Split('\n');
		string currentHeader="",trimmedLine;
		string[] splitLine;
		foreach(string line in rawLines){
			trimmedLine=line.Trim();
			if (trimmedLine[0]=='[' && trimmedLine[trimmedLine.Length-1]==']'){
				currentHeader=trimmedLine.Substring(1,trimmedLine.Length-2);
				data[currentHeader]=new Dictionary<string,string>();
				Debug.Log("New Header "+currentHeader);
			}else if (currentHeader!=""){
				splitLine=trimmedLine.Split('=');
				data[currentHeader][splitLine[0]]=splitLine[1];
				Debug.Log(splitLine[0]+"="+splitLine[1]);
			}
		}
		return data;
	}
}

}