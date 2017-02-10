using System.Collections;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using UnityEngine;

public struct ConsoleArgument{
	public Type argType;
	public string argName; 
	public ConsoleArgument(Type t,string n){
		argType=t;
		argName=n;
	}
}

public delegate bool ConsoleCommandCallback(object[] arguments);

public class ConsoleCommand : IComparable{
	public string commandName,helpString;
	public ConsoleArgument[] arguments;
	public ConsoleCommandCallback callback;

	public ConsoleCommand(string name, ConsoleCommandCallback c,string explanation,params ConsoleArgument[] args){
		arguments=args;
		commandName=name;
		callback=c;
		helpString=commandName+" ";
		foreach(ConsoleArgument a in args){
			helpString+=a.argName+" <"+a.argType+"> ";
		}
		helpString+="\n\t"+explanation;
	}

	public bool ParseCommand(string[] givenArgs){
		if (givenArgs.Length==1){
			Debug.Log(helpString);
			return true;
		}
		if (givenArgs.Length<arguments.Length+1){
			Debug.LogError(commandName+" requires "+arguments.Length+" commands, given "+(givenArgs.Length-1)+".");
			return false;
		}else if (givenArgs.Length>arguments.Length+1){
			Debug.LogError("Too many arguments supplied!");
			return false;
		}
		object[] toSend=new object[arguments.Length];
		TypeConverter tc;
		for (int i=0;i<arguments.Length;i++){
			if (arguments[i].argType==typeof(string)){
				toSend[i]=givenArgs[i+1];
			}else if (arguments[i].argType==typeof(Transform)){
				toSend[i]=ConvertUtil.StringToTransform(givenArgs[i+1]);
			}else if (arguments[i].argType==typeof(GameObject)){
				toSend[i]=ConvertUtil.StringToTransform(givenArgs[i+1]).gameObject;
			}else if (arguments[i].argType==typeof(Vector2)){
				toSend[i]=ConvertUtil.StringToVector2(givenArgs[i+1]);
			}else if (arguments[i].argType==typeof(Vector3)){
				toSend[i]=ConvertUtil.StringToVector3(givenArgs[i+1]);
			}else if (arguments[i].argType==typeof(Vector3)){
				toSend[i]=ConvertUtil.StringToVector4(givenArgs[i+1]);
			}else{
				tc=TypeDescriptor.GetConverter(arguments[i].argType);
				toSend[i]=tc.ConvertFrom(givenArgs[i+1]);
			}
		}
		return callback(toSend);
	}

	public int CompareTo(object cmp){
		if (cmp==null) return 1;
		return this.commandName.CompareTo(((ConsoleCommand)cmp).commandName);
	}
}

static class BetterConsoleDefaultCommands{
	public static void RegisterCommands(){
		BetterConsole.RegisterCommand("translate",TranslateCommand,"Translates to_move by move_delta",new ConsoleArgument(typeof(Transform),"to_move"),new ConsoleArgument(typeof(Vector3),"move_delta"));
		//translate);
		BetterConsole.RegisterCommand("echo",Echo,"Logs to_echo into the console",new ConsoleArgument(typeof(string),"to_echo"));
		BetterConsole.RegisterCommand("egg",Egg,"Eggs to_egg",new ConsoleArgument(typeof(GameObject),"to_egg"));
		//BetterConsole.RegisterCommand(echo);
	} 

	public static bool Egg(object[] args){
		Debug.Log(((GameObject)args[0]).name+" WAS EGGED");
		return true;
	}

	public static bool Echo(object[] args){
		Debug.Log((string)args[0]);
		return true;
	}

	public static bool TranslateCommand(object[] args){
		Transform toMove=args[0] as Transform;
		Vector3 delta=(Vector3)args[1];
		toMove.position+=delta;
		return true;
	}
}

public static class BetterConsole {
	public struct Line{
		public int type;
		public string text,traceback;
		public Line(string t,string tr){
			type=0;
			text=t;
			traceback=tr;
		}
		public Line(int ty,string t,string tr){
			type=ty;
			text=t;
			traceback=tr;
		}
	}

	/*
		BACKEND
				 */

	static List<BetterConsole.Line> logQueue;
	static List<ConsoleCommand> commands;
	static int maxLines;
	static bool inited=false,allowCollapse;

	public static void LoadParams(){
		logQueue=new List<BetterConsole.Line>();
		Dictionary<string,Dictionary<string,string>> parsedINI=IniParse.ParseINI((Resources.Load("BetterConsoleConfig") as TextAsset).text);
		maxLines=int.Parse(parsedINI["Config"]["MaxLines"]);
		commands=new List<ConsoleCommand>();
		Application.logMessageReceived+=HandleUnityLog;
		inited=true;
		BetterConsoleDefaultCommands.RegisterCommands();
	}

	static void PushLine(BetterConsole.Line toLog){
		if (!inited) LoadParams();
		logQueue.Add(toLog);
		if (logQueue.Count>maxLines && maxLines>0)
			logQueue.RemoveAt(0);
	}

	public static string ConsolidateLines(){
		if (!inited) LoadParams();
		string lines="";
		foreach(Line l in logQueue){
			if (l.type==1)
				lines+="<color=yellow>";
			else if (l.type==2)
				lines+="<color=red>";
			lines+=l.text;
			if (l.type!=0)
				lines+="</color>";
			lines+="\n";
		}
		lines=lines.Trim();
		return lines;
	}

	/*
		COMMANDS
				  */

	public static bool RegisterCommand(ConsoleCommand toAdd){
		if (!inited) LoadParams();
		foreach(ConsoleCommand c in commands){
			if (c.commandName==toAdd.commandName)
				return false;
		}
		commands.Add(toAdd);
		commands.Sort();
		Debug.Log("Successfully added command "+toAdd.commandName);
		return true;
	}

	public static bool RegisterCommand(string commandName,ConsoleCommandCallback callback,string explanation,params ConsoleArgument[] args){
		if (!inited) LoadParams();
		foreach(ConsoleCommand c in commands){
			if (c.commandName==commandName)
				return false;
		}
		ConsoleCommand toAdd=new ConsoleCommand(commandName,callback,explanation,args);
		commands.Add(toAdd);
		commands.Sort();
		Debug.Log("Successfully added command "+toAdd.commandName);
		return true;
	}

	public static bool ParseCommand(string fullCommand){
		if (!inited) LoadParams();
		Debug.Log(":"+fullCommand);
		string[] splitCommand=SplitCommand(fullCommand);
		if (splitCommand.Length==0) return false;
		foreach(ConsoleCommand c in commands){
			if (c.commandName==splitCommand[0]){
				return c.ParseCommand(splitCommand);
			}
		}
		Debug.LogError("That command doesn't exist!");
		return false;
	}

	public static string AutoComplete(string fullCommand,string previousAutocomplete){
		if (!inited) LoadParams();
		string[] splitCommand=SplitCommand(fullCommand,false);//fullCommand.Trim().Split(' ');
		//if (splitCommand.Length==0) return "";
		string finalArg=(splitCommand.Length>=1)?splitCommand[splitCommand.Length-1]:"";
		if (splitCommand.Length>1) splitCommand[0]=splitCommand[0].Trim(); //This is for command recognition ("echo "!="echo")
		List<string> autoCompleteChoices=new List<string>();
		//if (splitCommand.Length>1){
			foreach(ConsoleCommand c in commands){
				if (splitCommand.Length<=1){
					//Debug.Log("Autocompleting Command");
					autoCompleteChoices.Add(c.commandName);
					continue;
				}
				if (c.commandName==splitCommand[0] && splitCommand.Length<=c.arguments.Length+1){
					Type argType=c.arguments[splitCommand.Length-2].argType;
					if (argType==typeof(Transform) || argType==typeof(GameObject)){
						//Autocomplete for a transform
						//return "";
						Transform t = ConvertUtil.StringToTransform(finalArg);
						string parentName=finalArg;
						//Debug.Log(t);
						if (t==null){
							parentName=FindParentTransformNameFromString(finalArg);
							t=ConvertUtil.StringToTransform(parentName);
							//Debug.Log(t+" found from string "+finalArg);
						}
						if (t==null){
							//Search through top level
							GameObject[] roots=UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
							foreach(GameObject root in roots){
								autoCompleteChoices.Add(root.name);
							}
							//return "";
						}else{
							//Search through children
							if (parentName[parentName.Length-1]!='/')
								parentName+='/';
							foreach(Transform child in t){
								autoCompleteChoices.Add(parentName+child.gameObject.name);
								//Debug.Log(parentName+child.gameObject.name);
							}
						}
					}
					//We can't autocomplete numerics, vectors or bools
					break;
				}
			}
			//return ""; //Nothing to add
		//}else{
		//}
		//Look for commands starting with splitCommand[0]
		string name;
		for(int i=0;i<autoCompleteChoices.Count;i++){
			name=autoCompleteChoices[i];
			if (finalArg.Length<=name.Length && name.Substring(0,finalArg.Length)==finalArg){
				//Up for consideration
				if (name==finalArg+previousAutocomplete) continue;
				//Debug.Log(name+" up for consideration");
				if ((i==0 && finalArg+previousAutocomplete==autoCompleteChoices[autoCompleteChoices.Count-1]) || (i>0 && finalArg+previousAutocomplete==autoCompleteChoices[i-1]) || previousAutocomplete=="")
					return name.Substring(finalArg.Length);
			}
		}
		/*for(int cIndex=0;cIndex<commands.Count;cIndex++){
			name=commands[cIndex].commandName;
			//Debug.Log(commandName+","+commandName.Substring(splitCommand[0].Length)+","+splitCommand[0]);
			if (splitCommand[0].Length<commandName.Length && commandName.Substring(0,splitCommand[0].Length)==splitCommand[0]){
				//Up for consideration
				if (commandName.CompareTo(splitCommand[0]+previousAutocomplete)>0 || previousAutocomplete=="")
					return commandName.Substring(splitCommand[0].Length);
			}
		}*/
		return "";
	}

	static string FindParentTransformNameFromString(string s){
		for(int i=s.Length-1;i>=0;i--)
			if (s[i]=='/') return s.Substring(0,i+1);
		return s;
	}

	static string[] SplitCommand(string fullCommand,bool trimSpaces=true){
		//fullCommand=fullCommand.Trim();
		List<string> splitCommand=new List<string>();
		bool enclosed=false;
		int start=0;
		for(int i=0;i<fullCommand.Length;i++){
			if (fullCommand[i]==' '&&!enclosed || (enclosed&&(fullCommand[i]==')' || fullCommand[i]=='\"'))){
				//if (i==fullCommand.Length-1 || fullCommand[i+1]!='\"')
				if (!enclosed){
					splitCommand.Add(fullCommand.Substring(start,i-start+1));
					start=i+1;
				}else if (fullCommand[start]=='\"'){
					splitCommand.Add(fullCommand.Substring(start+1,i-start-1));
					start=i+2;
				}else{
					splitCommand.Add(fullCommand.Substring(start-1,i-start+2));
					start=i+1;
				}
				enclosed=false;
			}else if (fullCommand[i]=='(' || fullCommand[i]=='\"')
				enclosed=true;
		}
		//if (enclosed){
		if (start<fullCommand.Length) {
			if (fullCommand[start]=='\"') splitCommand.Add(fullCommand.Substring(start+1,fullCommand.Length-1-start));
			else if (start+1<fullCommand.Length && fullCommand[start+1]=='(') splitCommand.Add(fullCommand.Substring(start-1,fullCommand.Length-start));
			else splitCommand.Add(fullCommand.Substring(start,fullCommand.Length-start));
		}

		if (trimSpaces){
			for(int i=0;i<splitCommand.Count;i++)
				splitCommand[i]=splitCommand[i].Trim();
		}
		return splitCommand.ToArray();
	}

	/*				
		LOGGING  
				 */

	static string Log(string strToLog,string traceback=""){
		//string strToLog=string.Join(" ",objs);
		Line toLog=new Line(strToLog,traceback);
		PushLine(toLog);
		return strToLog;
	}

	static string LogWarning(string strToLog,string traceback=""){
		//string strToLog=string.Join(" ",objs);
		Line toLog=new Line(1,strToLog,traceback);
		PushLine(toLog);
		return strToLog;
	}

	static string LogError(string strToLog,string traceback=""){
		//string strToLog=string.Join(" ",objs);
		Line toLog=new Line(2,strToLog,traceback);
		PushLine(toLog);
		return strToLog;
	}

	static void HandleUnityLog(string logString, string stackTrace, LogType type){
		if (type==LogType.Error)
			LogError(logString,stackTrace);
		else if (type==LogType.Warning)
			LogWarning(logString,stackTrace);
		else
			Log(logString,stackTrace);
	}

}

