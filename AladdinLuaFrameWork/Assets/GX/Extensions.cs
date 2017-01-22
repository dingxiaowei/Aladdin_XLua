#if UNITY_EDITOR
using UnityEngine;
using XLua;
#endif
using System.Collections;

namespace GX
{
	[LuaCallCSharp]
	public static partial class Extensions
	{
		#region xLua
		#region NGUI
		public static UIButton GetButton(this GameObject go)
		{
			return go.GetComponent<UIButton>();
		}
		public static UILabel GetLabel(this GameObject go)
		{
			return go.GetComponent<UILabel>();
		}

		public static void AddLabel(this GameObject go)
		{
			go.AddComponent<UILabel>();
		}
		#endregion
		#endregion
	}
}
