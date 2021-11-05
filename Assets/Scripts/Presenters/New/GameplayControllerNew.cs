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
	public static event Action SessionEnded;
	
	private readonly CustomersControllerNew _customersControllerNew;
	private readonly FoodController _foodController;
	private readonly OrderGeneratorService _orderGeneratorService;

	private readonly GameplayMainScreenView _gameplayMainScreenView;
	private readonly GameplayTopUIView _gameplayTopUIView;
	private readonly GameplayResultWindowView _winWindowView;
	private readonly GameplayResultWindowView _loseWindowView;
	private readonly GameplayResultWindowView _startWindowView;

	private CustomersConfig _customersConfig;
	
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
	
	public void Initialize() {
		_startWindowView.Init(StartGameButtonClickedCallback);
		_loseWindowView.Init(RestartButtonClickedCallback);
		_winWindowView.Init(CloseButtonClickedCallback);
	}

	public void InitGameSession() {
		
		// should be fetched from some data provider in dev stage
		_customersConfig = new CustomersConfig {
			TotalCustomersNumber = 15,
			CustomerWaitTime = 18,
			CustomerSpawnTime = 3,
			MaxOrdersCount = 3,
			AddedTimeOnServedOrder = 6f
		};
		
		_customersConfig.LevelOrders = GetLevelOrders(_customersConfig);
		
		_startWindowView.Repaint(0,  _customersConfig.NeededFoodsInOrderToBeServed);
	}
	
	public void StartGameSession() {
		SessionEnded+= OnSessionEnded;
		
		CustomersControllerNew.CustomerTaskCompleted +=
			CustomersControllerNewOnCustomerTaskCompleted;
		CustomersControllerNew.CustomerGenerated +=
			CustomersControllerNewOnCustomerGenerated;
		CustomersControllerNew.CustomerOrderServed +=
			CustomersControllerNewOnCustomerOrderServed;
		
		_customersControllerNew.InitGameSession(_customersConfig);
		_foodController.InitGameSession();
		
		_gameplayTopUIView.RepaintCustomers(_customersConfig.TotalCustomersNumber);
		_gameplayTopUIView.RepaintOrders(0, _customersConfig.NeededFoodsInOrderToBeServed);
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

	private void CustomersControllerNewOnCustomerGenerated() {
		_gameplayTopUIView.RepaintCustomers(_customersControllerNew.CustomersLeft);
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
		CustomersControllerNew.CustomerGenerated -=
			CustomersControllerNewOnCustomerGenerated;
		CustomersControllerNew.CustomerOrderServed -=
			CustomersControllerNewOnCustomerOrderServed;
		
		_gameplayTopUIView.Hide();
	}
	
	private List<List<OrderModel>> GetLevelOrders(
		CustomersConfig customersConfig) {
		
		var t = Enumerable
			.Range(0, customersConfig.TotalCustomersNumber)
			.Select(x=> Enumerable
				.Range(0,
					Random.Range(1, customersConfig.MaxOrdersCount + 1))
				.Select(y
					=> _orderGeneratorService.GenerateRandomOrder())
				.ToList())
			.ToList();

		return t;
	}
}
}