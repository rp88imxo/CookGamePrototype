using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CookingPrototype.Kitchen.Views {
public class FoodPlaceNew : MonoBehaviour, IPointerUpHandler {
	
	private Food _currentFood;

	private Action<Food> _onFoodPlaceClicked;
	
	public void Init(Action<Food> onFoodPlaceClickedCallback) {
		_onFoodPlaceClicked = onFoodPlaceClickedCallback;
	}

	public void Repaint(Food food) {
		_currentFood = food;
	}
	
	public void OnPointerUp(PointerEventData eventData) {
		_onFoodPlaceClicked?.Invoke(_currentFood);
	}
}

}
