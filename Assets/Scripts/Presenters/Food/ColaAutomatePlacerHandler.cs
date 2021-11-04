using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CookingPrototype.Kitchen.Handlers {
public class ColaAutomatePlacerHandler : BaseFoodPlacerHandler {
	public override event Func<Food, bool> OnFoodPlaced;
	
	public void Init(Func<Food, bool> foodPlacedCallback) {
		OnFoodPlaced += foodPlacedCallback;
		CurrentFood = new Food(_foodName, _foodStatus);
	}
}
}
