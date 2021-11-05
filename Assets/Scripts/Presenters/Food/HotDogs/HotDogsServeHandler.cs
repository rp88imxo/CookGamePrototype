using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CookingPrototype.Kitchen.Handlers {

public class HotDogsData : FoodServeAssemblyData {
}

public class HotDogsServeHandler : FoodServeHandler<HotDogsData> {
	[SerializeField]
	private CookableFoodHandler _cookableFoodHandler;

	public override void InitGameSession(
		HotDogsData foodServeAssemblyData,
		Func<List<string>, bool> onServeClickedCallback) {
		base.InitGameSession(foodServeAssemblyData,
			onServeClickedCallback);

		_cookableFoodHandler.Init(
			foodServeAssemblyData.CookableFoodConfig,
			ONTryAddFoodComponentClickedCallback);
	}

	public override void HandleSessionEnded() {
		base.HandleSessionEnded();
		_cookableFoodHandler.HandleSessionEnded();
	}
}
}