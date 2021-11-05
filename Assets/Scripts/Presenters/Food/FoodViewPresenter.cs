using System;
using System.Collections;
using System.Collections.Generic;
using CookingPrototype.Kitchen.Handlers;
using UnityEngine;
using UnityEngine.Serialization;

namespace CookingPrototype.Kitchen.Views {
public class FoodData {
	public ColaAssemblyData ColaAssemblyData { get; set; }
	public BurgerData BurgerData { get; set; }
	public HotDogsData HotDogsData { get; set; }
}

public class FoodViewPresenter : View {
	[SerializeField]
	private ColaServeHandler _colaServeHandler;

	[SerializeField]
	private BurgersServeHandler _burgersServeHandler;

	[SerializeField]
	private HotDogsServeHandler _hotDogsServeHandler;

	private Action<List<string>, Action, Action> _onServeClickedCallback;

	public void Init() {
		Hide();
	}

	public void InitGameSession(FoodData foodData,
		Action<List<string>, Action, Action> onServeClickedCallback) {
		Show();
		_onServeClickedCallback = onServeClickedCallback;
		_colaServeHandler.InitGameSession(foodData.ColaAssemblyData,
			ONServeClickedCallback);
		_burgersServeHandler.InitGameSession(foodData.BurgerData,
			ONServeClickedCallback);
		_hotDogsServeHandler.InitGameSession(foodData.HotDogsData,
			ONServeClickedCallback);
	}

	public void HandleSessionEnded() {
		Hide();
		_colaServeHandler.HandleSessionEnded();
		_burgersServeHandler.HandleSessionEnded();
		_hotDogsServeHandler.HandleSessionEnded();
	}

	private void ONServeClickedCallback(List<string> obj, Action successCallback, Action failCallback) {
		_onServeClickedCallback?.Invoke(obj, successCallback, failCallback);
	}
}
}