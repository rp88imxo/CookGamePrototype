using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CookingPrototype.Kitchen.Handlers {
public class ColaAssemblyData : FoodServeAssemblyData { }

public class ColaServeHandler : FoodServeHandler<ColaAssemblyData> {
	[SerializeField]
	private ColaFoodHandler _colaFoodHandler;

	public override void InitGameSession(ColaAssemblyData foodServeAssemblyData,
		Action<List<string>, Action, Action> onServeClickedCallback) {
		base.InitGameSession(foodServeAssemblyData, onServeClickedCallback);

		_colaFoodHandler.Init(
			foodServeAssemblyData.CookableFoodConfig,
			ONTryAddFoodComponentClickedCallback,_orderAssemblyHandler);
	}

	public override void HandleSessionEnded() {
		base.HandleSessionEnded();
		_colaFoodHandler.HandleSessionEnded();
	}
}
}