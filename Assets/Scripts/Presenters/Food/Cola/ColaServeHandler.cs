using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CookingPrototype.Kitchen.Handlers {
public class ColaAssemblyData : FoodServeAssemblyData { }

public class ColaServeHandler : FoodServeHandler<ColaAssemblyData> {
	[SerializeField]
	private CookableFoodHandler _cookableFoodHandler;

	public override void InitGameSession(ColaAssemblyData foodServeAssemblyData,
		Func<List<string>, bool> onServeClickedCallback) {
		base.InitGameSession(foodServeAssemblyData, onServeClickedCallback);

		_cookableFoodHandler.Init(
			foodServeAssemblyData.CookableFoodConfig,
			ONTryAddFoodComponentClickedCallback);
	}

	public void HandleSessionEnded() {
		_cookableFoodHandler.HandleSessionEnded();
	}
}
}