using System;
using System.Collections;
using System.Collections.Generic;
using CookingPrototype.GameCore;
using UnityEngine;

namespace CookingPrototype.Kitchen.Handlers {
public class ColaAutomatePlacerHandler : BaseFoodPlacerHandler {
	[SerializeField]
	private ColaFoodHandler _cookableFoodHandler;

	private void Awake() {
		_cookableFoodHandler.CookplaceStateUpdated += CookableFoodHandlerOnCookplaceStateUpdated;
	}

	private void OnDestroy() {
		_cookableFoodHandler.CookplaceStateUpdated -= CookableFoodHandlerOnCookplaceStateUpdated;
	}

	private void CookableFoodHandlerOnCookplaceStateUpdated() {
		if ( _cookableFoodHandler.HasFreePlaces ) {
			OnFoodPlaced?.Invoke(CurrentFood.Clone());
		}
	}

	public override event Func<Food, bool> OnFoodPlaced;
}
}
