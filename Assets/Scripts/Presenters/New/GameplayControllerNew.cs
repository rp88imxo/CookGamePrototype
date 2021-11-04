using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CookingPrototype.Kitchen;
using CookingPrototype.Kitchen.Controllers;
using CookingPrototype.Kitchen.Views;
using CookingPrototype.Services;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CookingPrototype.Controllers {
public class GameplayControllerNew {
	private readonly CustomersControllerNew _customersControllerNew;
	private readonly FoodController _foodController;
	private readonly OrderGeneratorService _orderGeneratorService;

	private readonly GameplayMainScreenView _gameplayMainScreenView;
	private readonly GameplayTopUIView _gameplayTopUIView;
	private readonly GameplayResultWindowView _winWindowView;
	private readonly GameplayResultWindowView _loseWindowView;
	private readonly GameplayResultWindowView _startWindowView;
	
	public GameplayControllerNew(
		GameplayMainScreenView gameplayMainScreenView,
		CustomersControllerNew customersControllerNew,
		FoodController foodController,
		OrderGeneratorService orderGeneratorService) {
		_gameplayMainScreenView = gameplayMainScreenView;
		_customersControllerNew = customersControllerNew;
		_foodController = foodController;
		_orderGeneratorService = orderGeneratorService;

		_gameplayTopUIView =
			_gameplayMainScreenView.GameplayTopUIView;
		_startWindowView = _gameplayMainScreenView.StartWindowView;
		_loseWindowView = _gameplayMainScreenView.LoseWindowView;
		_winWindowView = _gameplayMainScreenView.WinWindowView;
	}

	public static event Action SessionEnded;

	public void Initialize() {
		_startWindowView.Init(StartGameButtonClickedCallback);
		_loseWindowView.Init(RestartButtonClickedCallback);
		_winWindowView.Init(CloseButtonClickedCallback);
	}

	public void InitGameSession() {
		_startWindowView.Show();
	}
	
	public void StartGameSession() {
		SessionEnded+= OnSessionEnded;

		// this config should be fetched from the server or from some other provider
		var customersConfig = new CustomersConfig {
			TotalCustomersNumber = 15,
			CustomerWaitTime = 18,
			CustomerSpawnTime = 3,
			MaxOrdersCount = 3
		};

		customersConfig.LevelOrders = GetLevelOrders(customersConfig);

		CustomersControllerNew.CustomerTaskCompleted +=
			CustomersControllerNewOnCustomerTaskCompleted;
		CustomersControllerNew.CustomerServed +=
			CustomersControllerNewOnCustomerServed;
		CustomersControllerNew.CustomerOrderServed +=
			CustomersControllerNewOnCustomerOrderServed;
		_customersControllerNew.InitGameSession(customersConfig);

		_foodController.InitGameSession();

		_gameplayTopUIView.Show();
	}

	#region UI_WINDOWS_CALLBACKS

	private void CloseButtonClickedCallback() {
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
			Application.Quit();
		#endif
	}

	private void RestartButtonClickedCallback() {
		StartGameSession();
	}

	private void StartGameButtonClickedCallback() {
		StartGameSession();
	}

	#endregion
	
	
	#region CUSTOMER_CONTROLLER_CALLBACKS

	private void CustomersControllerNewOnCustomerOrderServed(
		OrderModel obj) {
		_gameplayTopUIView.RepaintOrders(
			_customersControllerNew.TotalServedOrders,
			_customersControllerNew.TotalTargetOrders);
	}

	private void CustomersControllerNewOnCustomerServed(int servedCount,
		int totalNeeded) {
		_gameplayTopUIView.RepaintCustomers(totalNeeded - servedCount);
	}

	private void CustomersControllerNewOnCustomerTaskCompleted(bool taskSucceeded) {
		if ( taskSucceeded ) {
			_winWindowView.Repaint(_customersControllerNew.TotalServedOrders,
				_customersControllerNew.TotalTargetOrders);
		}
		else {
			_loseWindowView.Repaint(_customersControllerNew.TotalServedOrders,
				_customersControllerNew.TotalTargetOrders);
		}
		
		SessionEnded?.Invoke();
	}

	#endregion

	private void OnSessionEnded() {
		SessionEnded -= OnSessionEnded;
		
		CustomersControllerNew.CustomerTaskCompleted -=
			CustomersControllerNewOnCustomerTaskCompleted;
		CustomersControllerNew.CustomerServed -=
			CustomersControllerNewOnCustomerServed;
		CustomersControllerNew.CustomerOrderServed -=
			CustomersControllerNewOnCustomerOrderServed;
	}
	
	private List<OrderModel> GetLevelOrders(
		CustomersConfig customersConfig) {
		var t = Enumerable
			.Range(0,
				Random.Range(customersConfig.TotalCustomersNumber,
					customersConfig.TotalCustomersNumber
					* customersConfig.MaxOrdersCount
					+ 1))
			.Select(x
				=> _orderGeneratorService.GenerateRandomOrder())
			.ToList();

		return t;
	}
}
}