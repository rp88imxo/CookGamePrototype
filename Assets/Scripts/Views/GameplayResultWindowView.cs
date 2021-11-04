using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CookingPrototype.Kitchen.Views {
public class GameplayResultWindowView : View {
	[SerializeField]
	private Button _actionButton;

	[SerializeField]
	private TMP_Text _ordersCountText;
	
	[SerializeField]
	private Image _ordersBar;

	private Action _actionButtonClicked;
	
	public void Init(Action actionButtonClickedCallback) {
		_actionButtonClicked = actionButtonClickedCallback;
		_actionButton.onClick.AddListener(ActionButtonClickedCallback); 
	}

	private void ActionButtonClickedCallback() {
		Hide();
		_actionButtonClicked?.Invoke();
	}

	public void Repaint(int totalOrdersServed, int ordersTarget) {
		Show();
		_ordersCountText.text = $"{totalOrdersServed}/{ordersTarget}";
		_ordersBar.fillAmount = (float)totalOrdersServed / ordersTarget;
	}
}
}

