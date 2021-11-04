using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CookingPrototype.Kitchen.Handlers {

public class ColaAssemblyData {
	public OrderAssemblyConfig OrderAssemblyConfig { get; set; }
	public CookableFoodConfig CookableFoodConfig { get; set; }
	public List<OrderModel> PossibleOrders { get; set; }
}

public class ColaHandler : MonoBehaviour {
	[SerializeField]
	private OrderAssemblyHandler _orderAssemblyHandler;

	[SerializeField]
	private CookableFoodHandler _cookableFoodHandler;
	
	public void Init(ColaAssemblyData colaAssemblyData,
		Action<List<string>> onServeClickedCallback) {
		_orderAssemblyHandler.Init(colaAssemblyData.OrderAssemblyConfig,
			colaAssemblyData.PossibleOrders,
			onServeClickedCallback);
		
		_cookableFoodHandler.Init(colaAssemblyData.CookableFoodConfig,
			ONTryAddFoodComponentClickedCallback);
	}

	private bool ONTryAddFoodComponentClickedCallback(Food arg) {
		return _orderAssemblyHandler.TryAddFoodComponent(arg);
	}
}
}

