using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CookingPrototype.Kitchen.Views {
public class FoodPlacerHandler : MonoBehaviour, IPointerUpHandler {
	[SerializeField]
	private Food.FoodStatus _foodStatus = Food.FoodStatus.Cooked;
	
	[SerializeField]
	private string _foodName;
	
	private Food _currentFood;

	private Func<Food, bool> _onFoodPlaceClicked;
	
	public void Init(Func<Food, bool> onFoodPlaceClickedCallback) {
		_onFoodPlaceClicked = onFoodPlaceClickedCallback;
		_currentFood = new Food(_foodName, _foodStatus);
	}

	public void Repaint(Food food) {
		_currentFood = food;
	}
	
	public void OnPointerUp(PointerEventData eventData) {
		_onFoodPlaceClicked?.Invoke(_currentFood);
	}
}

}
