using System;
using System.Collections;
using System.Collections.Generic;
using CookingPrototype.Kitchen.Handlers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CookingPrototype.Kitchen.Handlers {
public class FoodPlacerHandler : BaseFoodPlacerHandler, IPointerUpHandler {
	public override event Func<Food, bool> OnFoodPlaced;

	public void HandleSessionEnded() {
		Debug.Log($"Food placer handler {name} freed resources!");
	}

	public void OnPointerUp(PointerEventData eventData) {
		OnFoodPlaced?.Invoke(CurrentFood.Clone());
	}
}

}
