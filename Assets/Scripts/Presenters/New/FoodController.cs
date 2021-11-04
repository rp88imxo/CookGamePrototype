using System.Collections;
using System.Collections.Generic;
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

		#region DATA

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
			OvercookTime = 0
		};
		
		_orderGeneratorService.GenerateRandomOrder()
		
		var data = new FoodData {
			ColaAssemblyData = new ColaAssemblyData {
				OrderAssemblyConfig = orderAssemblyConfig,
				CookableFoodConfig = cookableFoodConfigCola,
				PossibleOrders = 
			},
			BurgerData = new BurgerData {
				OrderAssemblyConfig = orderAssemblyConfig,
				CookableFoodConfig = cookableFoodConfigBurger,
				PossibleOrders = 
			}
		};

		#endregion
		
		_foodViewPresenter.InitGameSession(data, ONServeClickedCallback );
	}

	private bool ONServeClickedCallback(List<string> arg) {
		
		return _customersControllerNew.ServeOrder();
	}

	private void GameplayControllerNewOnSessionEnded() {
		
	}

	public void HandleSessionEnd() {
		GameplayControllerNew.SessionEnded -= GameplayControllerNewOnSessionEnded;
		// Free Resources
		_foodViewPresenter.HandleSessionEnded();
	}
}
}

