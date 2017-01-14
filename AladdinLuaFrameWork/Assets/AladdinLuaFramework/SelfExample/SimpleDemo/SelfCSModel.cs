using UnityEngine;
using System.Collections;
using XLua;
using System;
using Aladdin_XLua;

namespace Aladdin_XLua
{
	public class CSModel
	{
		public void SayHello()
		{
			Debug.Log("Hello Aladdin＿XLua");
		}
	}
}

public class CSModelWidhoutNameSpace
{
	public void SayHello()
	{
		Debug.Log("Hello Aladdin_XLua without Namespace");
	}
}

//namespace Aladdin_XLua
//{
//	public delegate void SelfVoidDelegate(GameObject go);
//}

public class CSModelTest
{
	public SelfVoidDelegate onClick;
	public delegate void SelfVoidDelegate(GameObject go);
	void OnClick() { Debug.Log("测试"); }

	public Action<string> TestDelegate = (param) =>
	{
		Debug.Log("TestDelegate in c#:" + param);
	};
}


[LuaCallCSharp]
public class CSModelWithAttribute
{
	public static void SayHello1()
	{
		Debug.Log("Hello Aladdin_XLua, I am static model function");
	}
	public void SayHello2()
	{
		Debug.Log("Hello Aladdin_XLua, I am model function");
	}
	public void SayHello3(string s)
	{
		Debug.Log("Hello Aladdin_XLua, I am model function whih param:" + s);
	}

	public string SayHello4(string s)
	{
		Debug.Log("Hello Aladdin_XLua, 我是具有返回值的CS方法:" + s);
		return "你好，我获得了lua，我是C#";
	}

	public void SayHelloWithRefParam(ref string s)
	{
		Debug.Log("传入的参数是:" + s);
		s = "Hello 我是C#";
	}

	public string SayHelloWithRefParamAndReturnString(ref string s)
	{
		Debug.Log("传入的参数是:" + s);
		s = "Hello 我是C#";
		return "我是返回的字符串";
	}

	public void SayHelloWithOutParam(out string s)
	{
		s = "Hello，我是C#";
		Debug.Log("Hello Aladdin_XLua, I am model function whih out param:" + s);
	}
}
