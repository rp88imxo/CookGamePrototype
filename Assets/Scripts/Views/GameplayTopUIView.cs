using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CookingPrototype.Kitchen.Views {
public class GameplayTopUIView : View {
	[SerializeField]
	private Image _ordersBar = null;

	[SerializeField]
	private TMP_Text _ordersCountText = null;

	[SerializeField]
	private TMP_Text _customersCountText = null;

	public void RepaintOrders(int totalOrdersServed, int ordersTarget) {
		_ordersCountText.text = $"{totalOrdersServed}/{ordersTarget}";
		_ordersBar.fillAmount = (float)totalOrdersServed / ordersTarget;
	}

	public void RepaintCustomers(int customersLeft) {
		_customersCountText.text = customersLeft.ToString();
	}
}
}