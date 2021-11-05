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

	[SerializeField]
	private List<GameObject> _extraObjects;

	public void SetActive(bool isActive) {
		_foodSprite.SetActive(isActive);
		_extraObjects.ForEach(x=> x.SetActive(isActive));
	}
	
	public List<GameObject> ExtraObjects => _extraObjects;
	public string FoodComponentName => _foodComponentName;
	public GameObject FoodSprite => _foodSprite;
}

public class OrderViewVisualizer : MonoBehaviour
{
	[SerializeField]
	private List<OrderComponentsVisualizer> _foodStatusVisualizers;
	
	public void Repaint(List<string> foodComponents) {
		if ( foodComponents == null ) {
			_foodStatusVisualizers.ForEach(x
				=> 
			{
				x.SetActive(false);
			});
			return;
		}
		
		_foodStatusVisualizers.ForEach(x => 
			x.SetActive(foodComponents.Contains(x.FoodComponentName)));
	}
}
}

