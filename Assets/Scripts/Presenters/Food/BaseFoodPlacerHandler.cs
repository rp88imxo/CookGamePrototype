using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CookingPrototype.Kitchen.Handlers {
public abstract class BaseFoodPlacerHandler : MonoBehaviour
{
	[SerializeField]
	protected Food.FoodStatus _foodStatus = Food.FoodStatus.Cooked;
	
	[SerializeField]
	protected string _foodName;
	
	public abstract event Func<Food, bool> OnFoodPlaced;
	
	protected Food CurrentFood;

	public void Init() {
		CurrentFood = new Food(_foodName, _foodStatus);
	}
}
}

