using System;
using System.Collections;
using System.Collections.Generic;
using CookingPrototype.GameCore;
using CookingPrototype.Kitchen.Controllers;
using CookingPrototype.Kitchen.Handlers;
using CookingPrototype.Kitchen.Views;
using CookingPrototype.Services;
using UnityEngine;

namespace CookingPrototype.Controllers {

public class FoodController {

	private readonly CustomersControllerNew _customersControllerNew;

	private readonly OrderGeneratorService _orderGeneratorService;
	
	private readonly FoodViewPresenter _foodViewPresenter;

	public FoodController(GameplayMainScreenView gameplayMainScreenView,
		CustomersControllerNew customersControllerNew,OrderGeneratorService orderGeneratorService) {
		_foodViewPresenter = gameplayMainScreenView.FoodViewPresenter;
		_customersControllerNew = customersControllerNew;
		_orderGeneratorService = orderGeneratorService;
	}

	public void InitGameSession() {
		GameplayControllerNew.SessionEnded += GameplayControllerNewOnSessionEnded;

		#region DATA // move outside of this controller

		OrderAssemblyConfig orderAssemblyConfig = new OrderAssemblyConfig {
			TotalPlaces = 3
		};
		
		CookableFoodConfig cookableFoodConfigCola = new CookableFoodConfig {
			TotalPlaces = 3,
			CookTime = 5,
			OvercookTime = 0
		};
		
		CookableFoodConfig cookableFoodConfigBurger = new CookableFoodConfig {
			TotalPlaces = 3,
			CookTime = 5,
			OvercookTime = 7
		};
		
		CookableFoodConfig cookableFoodConfigHotDog = new CookableFoodConfig {
			TotalPlaces = 3,
			CookTime = 5,
			OvercookTime = 7
		};

		var allOrders = _orderGeneratorService.GetAllOrders();
		
		var data = new FoodData {
			ColaAssemblyData = new ColaAssemblyData {
				OrderAssemblyConfig = orderAssemblyConfig,
				CookableFoodConfig = cookableFoodConfigCola,
				PossibleOrders = allOrders.Clone()
			},
			BurgerData = new BurgerData {
				OrderAssemblyConfig = orderAssemblyConfig,
				CookableFoodConfig = cookableFoodConfigBurger,
				PossibleOrders = allOrders.Clone()
			},
			HotDogsData = new HotDogsData {
				OrderAssemblyConfig = orderAssemblyConfig,
				CookableFoodConfig = cookableFoodConfigHotDog,
				PossibleOrders = allOrders.Clone()
			}
		};

		#endregion
		
		_foodViewPresenter.InitGameSession(data, ONServeClickedCallback );
	}

	private void ONServeClickedCallback(List<string> arg, Action successCallback, Action failCallback) {
		var order = _orderGeneratorService.FindOrder(arg);
		if ( order== null ) {
			failCallback?.Invoke();
		}
		
		_customersControllerNew.ServeOrder(order,successCallback,failCallback);
	}

	private void GameplayControllerNewOnSessionEnded() {
		HandleSessionEnd();
	}

	private void HandleSessionEnd() {
		GameplayControllerNew.SessionEnded -= GameplayControllerNewOnSessionEnded;
		// Free Resources
		_foodViewPresenter.HandleSessionEnded();
	}
}
}

