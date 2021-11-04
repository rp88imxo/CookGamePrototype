﻿using System;
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
		Func<List<string>, bool> onServeClickedCallback) {
		base.InitGameSession(foodServeAssemblyData, onServeClickedCallback);
		
		_cookableFoodHandler.Init(foodServeAssemblyData.CookableFoodConfig,
			ONTryAddFoodComponentClickedCallback);
	}

	public void HandleSessionEnded() {
		_cookableFoodHandler.HandleSessionEnded();
	}
}
}