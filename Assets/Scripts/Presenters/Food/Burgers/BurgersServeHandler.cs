using System;
using System.Collections;
using System.Collections.Generic;
using CookingPrototype.Kitchen.Views;
using UnityEngine;

namespace CookingPrototype.Kitchen.Handlers {
public class BurgerData : FoodServeAssemblyData {
}

public class BurgersServeHandler : FoodServeHandler<BurgerData> {
	[SerializeField]
	private CookableFoodHandler _cookableFoodHandler;

	public override void InitGameSession(BurgerData foodServeAssemblyData,
		Action<List<string>, Action,Action> onServeClickedCallback) {
		base.InitGameSession(foodServeAssemblyData, onServeClickedCallback);
		
		_cookableFoodHandler.Init(foodServeAssemblyData.CookableFoodConfig,
			ONTryAddFoodComponentClickedCallback);
	}

	public override void HandleSessionEnded() {
		base.HandleSessionEnded();
		_cookableFoodHandler.HandleSessionEnded();
	}
}
}