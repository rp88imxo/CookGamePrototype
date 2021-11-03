using System;
using System.Collections;
using System.Collections.Generic;
using CookingPrototype.GameCore;
using CookingPrototype.Kitchen.Handlers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CookingPrototype.Kitchen.Views {
public class CookingFoodViewModel {
	public Food.FoodStatus FoodViewState { get; set; }
	public float CookTime { get; set; } // Cook or overcook time
}

// Composition for base food view and timer
public class CookingFoodView : MonoBehaviour {
	[SerializeField]
	private FoodView _foodView;

	[SerializeField]
	private CookingTimerView _cookingTimerView;

	[SerializeField]
	private PointerInputHandler _oneTapHandler;

	[SerializeField]
	private PointerInputHandler _doubleTapHandler;

	private FoodViewModelHandler _associatedFoodViewModelHandler;

	private Action<FoodViewModelHandler> _onServeClicked;
	private Action<FoodViewModelHandler> _onTrashClicked;
	
	public void Init(FoodViewModelHandler foodViewModelHandler, Action<FoodViewModelHandler> onServeClicked, Action<FoodViewModelHandler> onTrashClicked) {
		_onServeClicked = onServeClicked;
		_onTrashClicked = onTrashClicked;
		
		_oneTapHandler.Init(TapCallback);
		_doubleTapHandler.Init(DoubleTapCallback);
		_associatedFoodViewModelHandler = foodViewModelHandler;
	}

	public void DestroySelf() {
		// here we can add some animations...
		Destroy(gameObject);
	}
	
	#region TAP_CALLBACKS

	private void DoubleTapCallback() {
		_onTrashClicked?.Invoke(_associatedFoodViewModelHandler);
	}

	private void TapCallback() {
		_onServeClicked?.Invoke(_associatedFoodViewModelHandler);
	}

	#endregion
	
	public void Repaint(CookingFoodViewModel cookingFoodViewModel) {
		_foodView.Repaint(cookingFoodViewModel.FoodViewState);
		InitTimer(cookingFoodViewModel.CookTime,
			cookingFoodViewModel.FoodViewState);
	}
	
	#region TIMER

	public void ToogleTimer(bool state) {
		_cookingTimerView.ToggleTimer(state);	
	}
	public void InitTimer(float initialTime, Food.FoodStatus foodStatus) {
		_cookingTimerView.Init(initialTime, foodStatus);
	}
	
	public void RepaintTimer(float timeLeft) {
		_cookingTimerView.Repaint(timeLeft);
	}

	#endregion

	
}
}