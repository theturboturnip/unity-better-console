using System.Collections;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using UnityEngine;

namespace TurboTurnip.Utility{

public static class ConvertUtil {
	public static string TransformToString(Transform t){
		string name=t.gameObject.name;
		t=t.parent;
		while(t!=null){
			name=t.gameObject.name+"/"+name;
		}
		return name;
	}

	public static Transform StringToTransform(string s){
		GameObject foundObj=GameObject.Find(s);
		if (foundObj==null)
			return null;
		return foundObj.transform;
	}

	public static string Vector2ToString(Vector2 vec2){
		return "("+vec2.x+","+vec2.y+")";
	}
	public static Vector2 StringToVector2(string s){
		Vector2 toReturn = new Vector2();
		s=s.Trim();
		string[] splitComponents=s.Substring(1,s.Length-1).Split(',');
		toReturn.x=float.Parse(splitComponents[0]);
		toReturn.y=float.Parse(splitComponents[1]);
		return toReturn;
	}

	public static string Vector3ToString(Vector3 vec3){
		return "("+vec3.x+","+vec3.y+","+vec3.z+")";
	}
	public static Vector3 StringToVector3(string s){
		Vector3 toReturn = new Vector3();
		s=s.Trim();
		string[] splitComponents=s.Substring(1,s.Length-2).Split(',');
		toReturn.x=float.Parse(splitComponents[0]);
		toReturn.y=float.Parse(splitComponents[1]);
		toReturn.z=float.Parse(splitComponents[2]);
		return toReturn;
	}

	public static string Vector4ToString(Vector4 vec4){
		return "("+vec4.x+","+vec4.y+","+vec4.z+","+vec4.w+")";
	}
	public static Vector2 StringToVector4(string s){
		Vector4 toReturn = new Vector4();
		s=s.Trim();
		string[] splitComponents=s.Substring(1,s.Length-2).Split(',');
		toReturn.x=float.Parse(splitComponents[0]);
		toReturn.y=float.Parse(splitComponents[1]);
		toReturn.z=float.Parse(splitComponents[2]);
		toReturn.w=float.Parse(splitComponents[3]);
		return toReturn;
	}

	public static object ConvertString(string s,Type t){
		if (t==typeof(string)){
			return s; //If we want a string don't bother with conversion.
		}else if (t==typeof(Transform)){							
			return ConvertUtil.StringToTransform(s);				
		}else if (t==typeof(GameObject)){
			return ConvertUtil.StringToTransform(s).gameObject;		
		}else if (t==typeof(Vector2)){							
			return ConvertUtil.StringToVector2(s);					
		}else if (t==typeof(Vector3)){							
			return ConvertUtil.StringToVector3(s);					
		}else if (t==typeof(Vector4)){							
			return ConvertUtil.StringToVector4(s);					
		}else if (t==typeof(Quaternion)){						
			//Special case, Quaternions are represented with an euler Vector3 in the console so we should convert.
			return Quaternion.Euler(ConvertUtil.StringToVector3(s));					
		}
		//It's a generic type (we hope), so TypeConverter can handle it.
		return TypeDescriptor.GetConverter(t).ConvertFrom(s);
	}
}

}