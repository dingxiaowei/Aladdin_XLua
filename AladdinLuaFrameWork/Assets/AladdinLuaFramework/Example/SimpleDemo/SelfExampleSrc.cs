using UnityEngine;
using System.Collections;
using XLua;
public class SelfExampleSrc : MonoBehaviour
{
	LuaEnv luaenv = new LuaEnv();
	void Start()
	{
		luaenv.DoString(@"

			print('Lua访问有命名空间的对象/静态方法')
			local luaModel = CS.Aladdin_XLua.CSModel
			local luaObj = luaModel()
			luaObj:SayHello()

			print('Lua访问无命名空间的对象方法')
			local luaM = CS.CSModelWidhoutNameSpace
			local luaO = luaM()
			luaO:SayHello()

			
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
			
			print('********************测试NGUI*****************')
			local label = CS.UILabel()
			label.text = '我是阿拉丁，我要赋值UILable'
			print(label.text)
			
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
