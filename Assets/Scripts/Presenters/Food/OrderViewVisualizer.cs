using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;

namespace CookingPrototype.Kitchen.Views {

[Serializable]
public class OrderComponentsVisualizer{
	[SerializeField]
	private string _foodComponentName;
	[SerializeField]
	private GameObject _foodSprite;

	public string FoodComponentName => _foodComponentName;
	public GameObject FoodSprite => _foodSprite;
}

public class OrderViewVisualizer : MonoBehaviour
{
	[SerializeField]
	private List<OrderComponentsVisualizer> _foodStatusVisualizers;
	
	public void Repaint(List<string> foodComponents) {
		if ( foodComponents == null ) {
			return;
		}
		_foodStatusVisualizers.ForEach(x => 
			x.FoodSprite.SetActive(foodComponents.Contains(x.FoodComponentName)));
	}
}
}

