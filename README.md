#Aladdin_XLua
## [Unity XLua]热更新XLua入门(一)-基础篇
### 前言
前段时间腾讯开源了一个内部热更框架XLua在Unity开发群里引起一阵热议，也受到广大开发者的热捧，然后我当然也抱着好奇的心去学习学习。后面也会将扩展之后的工程放在git上，大家一起学习交流！在此感谢XLua作者创造出这么好用的框架！

### 相关链接

 1. [XLua源码](Unity3D%E7%83%AD%E6%9B%B4%E6%96%B0LuaFramework%E5%85%A5%E9%97%A8%E5%AE%9E%E6%88%98)

 2. [C#->Lua开源工具](https://github.com/yanghuan/bridge.lua)
 可以将C#转化成lua并且具有
 
 3. [相关介绍文章](http://gad.qq.com/article/detail/7182056)
https://www.oschina.net/news/80638/c-net-lua-unity3d


### 个人对XLua看法
 1. 简洁易用，容易上手
 2.  可扩展性高，添加自定义的CS模块或者第三方插件非常方便
 3. 大厂维护，可靠
 4. 特色：HotFix
 关于这个HotFix是其他热更lua框架所不具备的，也是他最大的优势和特色之一，原理就是通过特性标记然后在IL逻辑层判断修改逻辑，使程序支持热更的lua逻辑代码而不是走之前的C#逻辑

### 自己扩展XLua支持NGUI开发
现在开源热更Lua框架都很少支持NGUI了，可能现在趋势都是用原生的UGUI，但估计还有一些NGUI粉喜欢用NGUI开发，毕竟NGUI用了很长时间，XLua例子里面已经支持了lua使用UGUI，这里我就自己补充让它支持NGUI开发。后续我也会多添加一些UGUI的例子。先看看扩展的NGUI做的界面效果图，然后下面再讲解怎么让XLua支持第三方插件。
#### 效果图
![这里写图片描述](http://img.blog.csdn.net/20170114153718102?watermark/2/text/aHR0cDovL2Jsb2cuY3Nkbi5uZXQvZGluZ3hpYW93ZWkyMDEz/font/5a6L5L2T/fontsize/400/fill/I0JBQkFCMA==/dissolve/70/gravity/SouthEast)

### 快速上手
在学习一个东西之前最好先学习一下XLua作者辛辛苦苦写的那些多教程文档，包括案例以及wiki和issu，如果还有什么不明白的地方可以在加入文章最后的群我们一起交流学习。

#### 1.自定义C#类供Lua访问
这里可以学习一下作者写的Test中的例子，还是比较详细，但我还是想记录一下我自己尝试的过程。

##### （1）特性方式
XLua支持使用特性标签让自定义的类可供Lua访问
C#：

```
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
```
添加上特性标签之后，程序在启动的会自动查找具有特性标记的类然后搜集进入lua栈，使得我们可以用Lua访问自定义的C#类和方法。

Lua访问：

```
using UnityEngine;
using System.Collections;
using XLua;
public class SelfExampleSrc : MonoBehaviour
{
	LuaEnv luaenv = new LuaEnv();
	void Start()
	{
		luaenv.DoString(@"

			print('Lua访问特性标记的对象方法')
			local luaM2 = CS.CSModelWithAttribute
			local luaO2 = luaM2()
			luaM2:SayHello1()
			luaO2:SayHello2()
			luaO2:SayHello3('我是阿拉丁')  --每次添加一个CS方法时候都要重新Generate一下啊

			--测试字符串返回
			local value = luaO2:SayHello4('你好，我是lua')
			print(value)                              
			
			--测试ref
			local inputValue = '你好，我是lua'
			local outputValue = luaO2:SayHelloWithRefParam(inputValue)
			print(outputValue)                          --lua是通过字符串返回

			local outValue1,outValue2 = luaO2:SayHelloWithRefParamAndReturnString(inputValue)
			print(outValue1)
			print(outValue2)
			
			--测试out
			inputValue = '我是测试lua'
			outputValue = luaO2:SayHelloWithOutParam(inputValue)
			print(outputValue)
			
			local luaM3 = CS.CSModelTest
			local luaO3 = luaM3()
			luaO3.TestDelegate('lua中测试委托')


			luaO3.onClick = function(obj)
				print('hello 我是lua')
				print(obj)
			end
			luaO3.onClick('我是lua')
		");
	}
}

```


##### （2）wrap方式
如果是我们自己写的C#代码，我们可以通过第一种方式添加特性来让Lua支持也是比较方便，如果是用的开源第三方插件，我们如何快速的让XLua支持，就可以用过Generate Wrap的方式，这一点也是其他lua框架所采取的策略。
###### a）有命名空间的类
C#：

```
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
```
Lua：

```
print('Lua访问有命名空间的对象/静态方法')
local luaModel = CS.Aladdin_XLua.CSModel
local luaObj = luaModel()
luaObj:SayHello()
```
luaModel是C#类，下面luaObj是类生成的对象，最后是访问对象方法，关于Lua冒号调用和点调用的区别：冒号调用默认传入了self参数，不清楚的可以百度相关文章。


###### b）无命名空间的类
C#：

```
public class CSModelWidhoutNameSpace
{
	public void SayHello()
	{
		Debug.Log("Hello Aladdin_XLua without Namespace");
	}
}
```
Lua：

```
print('Lua访问无命名空间的对象方法')
local luaM = CS.CSModelWidhoutNameSpace
local luaO = luaM()
luaO:SayHello()
```
如果没有命名空间的话直接CS后面就是类名，其实CS也是一个更外面一层的命名空间，只不过是作者帮我们分装的。

###### 3）委托类型
C#：

```
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
```
委托其实也是跟Class平级的，委托也是一种类型，所以我们也需要对它像对待类那样处理，通过添加特性标记或者通过Wrap方式处理，这里委托是放在类里面，其实也可以直接放在命名空间下面，.NET库是这样操作的，但我们看NGUI源码会发现，NGUI源码都是这样操作的，比如按钮的onClick事件，看它的委托类型VoidDelegate就会发现也是这样操作的，所以我这里例子也放在类的里面。

C#

```
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
```
Lua:

```
local luaM3 = CS.CSModelTest
local luaO3 = luaM3()
luaO3.TestDelegate('lua中测试委托')

luaO3.onClick = function(obj)
	print('hello 我是lua')
	print(obj)
end
luaO3.onClick('我是lua')
```

###### 4）带有ref out 参数的函数如何处理
因为Lua是弱类型没有C#那么多类型，有时候一些参数可能就不太好处理，比如C#的不同类型参数的重载，lua就不太好处理，这里可以查看XLua中的issues，作者有一个问题的相关解答。下面我举例ref和out参数类型的函数Lua如何访问。
C#:

```
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

```


Lua:

```
--测试ref
local inputValue = '你好，我是lua'
local outputValue = luaO2:SayHelloWithRefParam(inputValue)
print(outputValue)                          --lua是通过字符串返回

local outValue1,outValue2 = luaO2:SayHelloWithRefParamAndReturnString(inputValue)
print(outValue1)
print(outValue2)
			
--测试out
inputValue = '我是测试lua'
outputValue = luaO2:SayHelloWithOutParam(inputValue)
print(outputValue)
```
一开始我测试的时候是本以为lua调用ref传入的参数，也会返回出修改的结果，但出乎我的意料，并没能修改，经过作者提示，lua是通过返回值返回的ref参数，如果函数本身就有返回值，那么最后一个参数是返回的ref或者out参数，这个读者可以尝试一下。

###### 运行结果
![这里写图片描述](http://img.blog.csdn.net/20170114170348580?watermark/2/text/aHR0cDovL2Jsb2cuY3Nkbi5uZXQvZGluZ3hpYW93ZWkyMDEz/font/5a6L5L2T/fontsize/400/fill/I0JBQkFCMA==/dissolve/70/gravity/SouthEast)

###### 关于Wrap
Wrap是C#跟Lua之间的一个桥梁，Lua想要访问C#必须要用过Wrap访问，相信看过其他Lua框架的这一点应该不陌生，XLua对生成Wrap也是非常方便。

我们只要新建一个类然后继承一个GenConfig的接口，下面是接口内容，关于这几个类型XLua文档中也有介绍，我们只需要把自定义的类添加到LuaCallCSharp集合中即可，然后点击Generate就会自动帮我们生成对应的Wrap文件

```
//注意：用户自己代码不建议在这里配置，建议通过标签来声明!!
    public interface GenConfig 
    {
        //lua中要使用到C#库的配置，比如C#标准库，或者Unity API，第三方库等。
        List<Type> LuaCallCSharp { get; }

        //C#静态调用Lua的配置（包括事件的原型），仅可以配delegate，interface
        List<Type> CSharpCallLua { get; }

        //黑名单
        List<List<string>> BlackList { get; }
    }
```
当然作者也说了，我们自定的C#代码最好不要通过这种方式，我这里只是演示如何添加，下面会说第三方插件通过这话总方式支持。
C#:

```
public static class AladdinGenConfig
{
	//lua扩展第三方或者自定义类库
	public class LuaCallCSharpExtern : GenConfig
	{

		public List<Type> LuaCallCSharp
		{
			get
			{
				return new List<Type>()
				{
					typeof(CSModelWidhoutNameSpace),
					typeof(CSModel),
					typeof(CSModelTest),
				}
			}
		}
	}
}
```


#### 2.NGUI扩展
正如上图所示的效果，下面讲述一下我是如何支持NGUI扩展的，也参考了作者UGUI的一个例子修改的。

##### a）生成Wrap接口
这一步上上面说的一样，只要把NGUI的组件类全部都添加到LuaCallCSharp列表中然后Generate一下即可，这里要注意的是组件中委托类型也需要添加进去。

##### b）搭建两个UI界面，UI逻辑接口用C#，Lua是调用逻辑调用界面中C#的方法。

![这里写图片描述](http://img.blog.csdn.net/20170114170632174?watermark/2/text/aHR0cDovL2Jsb2cuY3Nkbi5uZXQvZGluZ3hpYW93ZWkyMDEz/font/5a6L5L2T/fontsize/400/fill/I0JBQkFCMA==/dissolve/70/gravity/SouthEast)
![这里写图片描述](http://img.blog.csdn.net/20170114170640737?watermark/2/text/aHR0cDovL2Jsb2cuY3Nkbi5uZXQvZGluZ3hpYW93ZWkyMDEz/font/5a6L5L2T/fontsize/400/fill/I0JBQkFCMA==/dissolve/70/gravity/SouthEast)

C#:
购买

```
using UnityEngine;
using System.Collections;
using XLua;
public class AsyncBuy : MonoBehaviour
{
	LuaEnv luaenv = null;

	void Start()
	{
		luaenv = new LuaEnv();
		luaenv.DoString("require 'async_buy'");
	}

	// Update is called once per frame
	void Update()
	{
		if (luaenv != null)
		{
			luaenv.Tick();
		}
	}
}

```
Panel逻辑：

```
using UnityEngine;
using UnityEngine.UI;
using XLua;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

public class MessagePanel : MonoBehaviour
{
	/// <summary>
	/// 显示对话弹框
	/// </summary>
	/// <param name="message"></param>
	/// <param name="title"></param>
	/// <param name="onFinished"></param>
	public static void ShowAlertPanel(string message, string title, Action onFinished = null)
	{
		Debug.Log("显示提示弹框");
		var rootPanel = GameObject.Find("Panel").transform;
		var alertPanel = rootPanel.Find("AlertPanel");
		if (alertPanel == null)
		{
			alertPanel = (Instantiate(Resources.Load("AlertPanel")) as GameObject).transform;
			alertPanel.gameObject.name = "AlertPanel";
			alertPanel.SetParent(rootPanel);
			alertPanel.localPosition = Vector3.zero;
			alertPanel.localScale = Vector3.one;
		}
		alertPanel.Find("Title").GetComponent<UILabel>().text = title;
		alertPanel.Find("Content").GetComponent<UILabel>().text = message;
		alertPanel.gameObject.SetActive(true);
		if (onFinished != null)
		{
			var buyBtn = alertPanel.Find("BtnBuy").gameObject;
			buyBtn.SetActive(true);
			var button = buyBtn.GetComponent<UIButton>();
			UIEventListener.Get(buyBtn).onClick = go =>
			{
				onFinished();
				alertPanel.gameObject.SetActive(false);
			};
		}
	}

	/// <summary>
	/// 显示确认弹框
	/// </summary>
	/// <param name="message"></param>
	/// <param name="title"></param>
	/// <param name="onFinished"></param>
	public static void ShowConfirmPanel(string message, string title, Action<bool> onFinished = null)
	{
		var rootPanel = GameObject.Find("Panel").transform;
		var confirmPanel = rootPanel.Find("ConfirmPanel");
		if (confirmPanel == null)
		{
			confirmPanel = (Instantiate(Resources.Load("ConfirmPanel")) as GameObject).transform;
			confirmPanel.gameObject.name = "ConfirmPanel";
			confirmPanel.SetParent(rootPanel);
			confirmPanel.localPosition = Vector3.zero;
			confirmPanel.localScale = Vector3.one;
		}
		confirmPanel.Find("Title").GetComponent<UILabel>().text = title;
		confirmPanel.Find("Content").GetComponent<UILabel>().text = message;
		confirmPanel.gameObject.SetActive(true);
		if (onFinished != null)
		{
			var confirmBtn = confirmPanel.Find("BtnBuy").GetComponent<UIButton>();
			var cancelBtn = confirmPanel.Find("CancelBuy").GetComponent<UIButton>();

			UIEventListener.Get(confirmBtn.gameObject).onClick = go =>
			{
				onFinished(true);
				confirmPanel.gameObject.SetActive(false);
			};

			UIEventListener.Get(cancelBtn.gameObject).onClick = go =>
			{
				confirmPanel.gameObject.SetActive(false);
			};
		}
	}
}
```
Lua：
lua文件放在对应的Resources下即可
async_buy.lua

```

local util = require 'xlua.util'
local message_panel = require 'message_panel'

-------------------------async_recharge-----------------------------
local function async_recharge(num, cb) --模拟的异步充值
    print('requst server...')
    cb(true, num)
end

local recharge = util.async_to_sync(async_recharge)
-------------------------async_recharge end----------------------------
local buy = function()
    message_panel.alert("余额提醒","您余额不足，请充值！")
	if message_panel.confirm("确认充值10元吗？","确认框" ) then
		local r1, r2 = recharge(10)
		print('recharge result', r1, r2)
		message_panel.alert("提示","充值成功！")
	else
	    print('cancel')
	    message_panel.alert("提示","取消充值！")
	end
end

CS.UIEventListener.Get(CS.UnityEngine.GameObject.Find("BtnBuy").gameObject).onClick = util.coroutine_call(buy)

```
message_panel.lua

```

local util = require 'xlua.util'

local sync_alert = util.async_to_sync(CS.MessagePanel.ShowAlertPanel)
local sync_confirm = util.async_to_sync(CS.MessagePanel.ShowConfirmPanel) 

--构造alert和confirm函数
return {
    alert = function(title, message)
		if not message then
			title, message = message, title
		end
		 sync_alert(message,title)
    end;
	
	confirm = function(title, message)
		local ret = sync_confirm(title,message)
		return ret == true
    end;
 }
```
运行的结果就如第一张图所示

## [Unity XLua]热更新XLua入门(二)-俄罗斯方块实例篇
### 前言
在xLua没出来之前，开源的lua框架基本都是以界面用Lua开发为主，核心战斗用C#开发，但xLua出来之后主推C#开发，Lua用作HotFix，这里我展示的第一个例子就是基于界面的经典2D小游戏——俄罗斯方块，界面逻辑是用C#写，启动加载逻辑是用lua，后面我会继续第二个同样的Demo，但是以纯Lua为主，这个案例明天更新。

### 效果图
![这里写图片描述](http://img.blog.csdn.net/20170122011246613?watermark/2/text/aHR0cDovL2Jsb2cuY3Nkbi5uZXQvZGluZ3hpYW93ZWkyMDEz/font/5a6L5L2T/fontsize/400/fill/I0JBQkFCMA==/dissolve/70/gravity/SouthEast)
![这里写图片描述](http://img.blog.csdn.net/20170122011546224?watermark/2/text/aHR0cDovL2Jsb2cuY3Nkbi5uZXQvZGluZ3hpYW93ZWkyMDEz/font/5a6L5L2T/fontsize/400/fill/I0JBQkFCMA==/dissolve/70/gravity/SouthEast)
![这里写图片描述](http://img.blog.csdn.net/20170122011557755?watermark/2/text/aHR0cDovL2Jsb2cuY3Nkbi5uZXQvZGluZ3hpYW93ZWkyMDEz/font/5a6L5L2T/fontsize/400/fill/I0JBQkFCMA==/dissolve/70/gravity/SouthEast)
由于我不会美术，所以这里使用的开源的游戏资源，感谢此作者。


### 后续计划

 - 添加资源热更
 - 添加一个小游戏Demo
 - 添加UGUI的案例

---
### 欢迎加群交流
1.unity游戏开发群
![QQ群](http://img.blog.csdn.net/20161128123546291)
<a target="_blank" href="//shang.qq.com/wpa/qunwpa?idkey=e3e3e79643dedbe3ad8b25c448338f0be9fa23526a28a6f8a9f2389081e1eda0"><img border="0" src="//pub.idqqimg.com/wpa/images/group.png" alt="unity3d unity 游戏开发" title="unity3d unity 游戏开发"></a>

2.专门探讨XLua的程序群：437645698


----------

### Git下载
GitOS第一时间更新：http://git.oschina.net/dingxiaowei/Aladdin_XLua
Github同步：https://github.com/dingxiaowei/Aladdin_XLua

