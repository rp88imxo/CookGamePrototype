using System;
using System.Collections;
using System.Collections.Generic;
using CookingPrototype.Kitchen.Views;
using UnityEngine;

namespace CookingPrototype.Kitchen.Handlers {
public class BurgerData {
	public OrderAssemblyConfig OrderAssemblyConfig { get; set; }
	public BurgerPanConfig BurgerPanConfig { get; set; }
	public List<OrderModel> PossibleOrders { get; set; }
}

public class BurgersHandler : MonoBehaviour {
	[SerializeField]
	private OrderAssemblyHandler _orderAssemblyHandler;

	[SerializeField]
	private CookableFoodHandler _cookableFoodHandler;

	[SerializeField]
	private List<FoodPlacerHandler> _foodPlacerHandlers;
	
	public void Init(BurgerData burgerData,
		Action<List<string>> onServeClickedCallback) {
		_orderAssemblyHandler.Init(burgerData.OrderAssemblyConfig,
			burgerData.PossibleOrders,
			onServeClickedCallback);
		
		_cookableFoodHandler.Init(burgerData.BurgerPanConfig,
			ONTryAddFoodComponentClickedCallback);

		foreach ( var foodPlacer in _foodPlacerHandlers ) {
			foodPlacer.Init(ONTryAddFoodComponentClickedCallback);
		}
	}

	private bool ONTryAddFoodComponentClickedCallback(Food arg) {
		return _orderAssemblyHandler.TryAddFoodComponent(arg);
	}
}
}