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
		//BetterConsole.RegisterCommand(echo);
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
		string[] splitCommand=SplitCommand(fullCommand);//fullCommand.Trim().Split(' ');
		if (splitCommand.Length>1){
			foreach(ConsoleCommand c in commands){
				if (c.commandName==splitCommand[0]){
					Type argType=c.arguments[c.arguments.Length-1].argType;
					if (argType==typeof(Transform)){
						//Autocomplete for a transform
						return "";
					}
					//We can't autocomplete numerics, vectors or bools
					break;
				}
			}
			return ""; //Nothing to add
		}
		//Look for commands starting with splitCommand[0]
		string commandName;
		for(int cIndex=0;cIndex<commands.Count;cIndex++){
			commandName=commands[cIndex].commandName;
			//Debug.Log(commandName+","+commandName.Substring(splitCommand[0].Length)+","+splitCommand[0]);
			if (commandName.Substring(0,Mathf.Min(splitCommand[0].Length,commandName.Length))==splitCommand[0]){
				//Up for consideration
				if (commandName.CompareTo(splitCommand[0]+previousAutocomplete)>0 || previousAutocomplete=="")
					return commandName.Substring(splitCommand[0].Length);
			}
		}
		return "";
	}

	static string[] SplitCommand(string fullCommand){
		fullCommand=fullCommand.Trim();
		List<string> splitCommand=new List<string>();
		bool enclosed=false;
		int start=0;
		for(int i=0;i<fullCommand.Length;i++){
			if (fullCommand[i]==' '&&!enclosed || (enclosed&&(fullCommand[i]==')' || fullCommand[i]=='\"'))){
				//if (i==fullCommand.Length-1 || fullCommand[i+1]!='\"')
				if (!enclosed){
					splitCommand.Add(fullCommand.Substring(start,i-start));
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

