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