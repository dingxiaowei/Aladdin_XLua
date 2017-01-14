using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;
using System;
using Aladdin_XLua;


//namespace Aladdin_XLua
//{
//	public delegate void VoidDelegate(GameObject go);
//	public delegate void BoolDelegate(GameObject go, bool state);
//	public delegate void FloatDelegate(GameObject go, float delta);
//	public delegate void VectorDelegate(GameObject go, Vector2 delta);
//	public delegate void ObjectDelegate(GameObject go, GameObject obj);
//	public delegate void KeyCodeDelegate(GameObject go, KeyCode key);
//}


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
					typeof(MessagePanel),
					//******************NGUI************************************
					typeof(UILabel),
					typeof(UIButton),
					typeof(UISprite),
					typeof(UIEventListener),
					typeof(UITexture),
					typeof(UICamera),
					typeof(UIAnchor),
					typeof(UIAtlas),
					typeof(UIBasicSprite),
					typeof(UIButtonActivate),
					typeof(UIButtonColor),
					typeof(UIButtonKeys),
					typeof(UIButtonMessage),
					typeof(UIButtonOffset),
					typeof(UIButtonRotation),
					typeof(UIButtonScale),
					typeof(UICamera),
					typeof(UICenterOnChild),
					typeof(UICenterOnClick),
					typeof(UICursor),
					typeof(UIDragCamera),
					typeof(UIDragDropContainer),
					typeof(UIDragDropItem),
					typeof(UIDragDropRoot),
					typeof(UIDraggableCamera),
					typeof(UIDragObject),
					typeof(UIDragResize),
					typeof(UIDragScrollView),
					typeof(UIDrawCall),
					typeof(UIEquipmentSlot),
					typeof(UIEventTrigger),
					typeof(UIFont),
					typeof(UIForwardEvents),
					typeof(UIGeometry),
					typeof(UIGrid),
					typeof(UIImageButton),
					typeof(UIInput),
					typeof(UIInputOnGUI),
					typeof(UIItemSlot),
					typeof(UIItemStorage),
					typeof(UIKeyBinding),
					typeof(UIKeyNavigation),
					typeof(UILocalize),
					typeof(UIOrthoCamera),
					typeof(UIPanel),
					typeof(UIPlayAnimation),
					typeof(UIPlaySound),
					typeof(UIPlayTween),
					typeof(UIPopupList),
					typeof(UIProgressBar),
					typeof(UIRect),
					typeof(UIRoot),
					typeof(UISavedOption),
					typeof(UIScrollBar),
					typeof(UIScrollView),
					typeof(UISlider),
					typeof(UISliderColors),
					typeof(UISnapshotPoint),
					typeof(UISoundVolume),
					typeof(UISpriteAnimation),
					typeof(UISpriteData),
					typeof(UIStorageSlot),
					typeof(UIStretch),
					typeof(UITable),
					typeof(UITextList),
					typeof(UIToggle),
					typeof(UIToggledComponents),
					typeof(UIToggledObjects),
					typeof(UITooltip),
					typeof(UITweener),
					typeof(UIViewport),
					typeof(UIWidget),
					typeof(UIWidgetContainer),
					typeof(UIWrapContent),
					typeof(UnityAPICompatibilityVersionAttribute),
				};
			}
		}

		public List<Type> CSharpCallLua
		{
			get
			{
				return new List<Type>()
				{
					typeof(CSModelTest.SelfVoidDelegate),  //自定义的委托也要generate
					typeof(CSModelTest),

					typeof(UIEventListener),
					typeof(UIEventListener.VoidDelegate),
					typeof(UIEventListener.BoolDelegate),
					typeof(UIEventListener.FloatDelegate),
					typeof(UIEventListener.VectorDelegate),
					typeof(UIEventListener.ObjectDelegate),
					typeof(UIEventListener.KeyCodeDelegate),
				};
			}
		}

		public List<List<string>> BlackList
		{
			get { return null; }
		}
	}
}
