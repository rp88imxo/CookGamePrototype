﻿using System;
using System.Collections;
using System.Collections.Generic;
using CookingPrototype.Kitchen.Handlers;
using UnityEngine;
using UnityEngine.Serialization;

namespace CookingPrototype.Kitchen.Views {

public class FoodData {
	public ColaAssemblyData ColaAssemblyData { get; set; }
	public BurgerData BurgerData { get; set; }
}

public class FoodViewPresenter : View {
	[SerializeField]
	private ColaServeHandler _colaServeHandler;
	
	[SerializeField]
	private BurgersServeHandler _burgersServeHandler;

	// TODO: Add an hot dog handler

	private Func<List<string>, bool> _onServeClickedCallback;
	
	public void Init() {
		Hide();
	}

	public void InitGameSession(FoodData foodData, Func<List<string>, bool> onServeClickedCallback) {
		Show();
		_onServeClickedCallback = onServeClickedCallback;
		_colaServeHandler.InitGameSession(foodData.ColaAssemblyData, ONServeClickedCallback);
		_burgersServeHandler.InitGameSession(foodData.BurgerData,
			ONServeClickedCallback);
	}

	public void HandleSessionEnded() {
		Hide();
		_colaServeHandler.HandleSessionEnded();
		_burgersServeHandler.HandleSessionEnded();
	}
	
	private bool ONServeClickedCallback(List<string> obj) {
		var res = _onServeClickedCallback?.Invoke(obj);
		return res ?? false;
	}
}
}
