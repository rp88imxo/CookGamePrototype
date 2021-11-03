using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;

namespace CookingPrototype.Kitchen.Views {

public enum FoodViewState {
	Empty,
	Raw,
	Cooked,
	Overcooked
}

[Serializable]
public class FoodStatusVisualizer{
	[SerializeField]
	private Food.FoodStatus _foodState;
	[SerializeField]
	private GameObject _foodSprite;

	public Food.FoodStatus FoodState => _foodState;
	public GameObject FoodSprite => _foodSprite;
}

public class FoodView : MonoBehaviour {
	[SerializeField]
	private Food.FoodStatus _defaultState;

	[SerializeField]
	private List<FoodStatusVisualizer> _foodStatusVisualizers;
	
	public void Repaint(Food.FoodStatus foodViewState) {
		_foodStatusVisualizers.ForEach(x => {
			if ( x.FoodSprite != null ) {
				x.FoodSprite.SetActive(x.FoodState == foodViewState);
			}
		});

	}

	#if UNITY_EDITOR
	[UsedImplicitly]
	[Button("Set Default State")]
	private void SetDefaultState() {
		Repaint(_defaultState);
	}
	#endif
	
}
}

