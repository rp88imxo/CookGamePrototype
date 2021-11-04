using System;
using System.Collections;
using System.Collections.Generic;
using CookingPrototype.Controllers;
using CookingPrototype.Kitchen.Controllers;
using CookingPrototype.Kitchen.Views;
using CookingPrototype.Services;
using UnityEngine;

// Should be replaced with DI since i'm not sure we are allowed to use Zenject in test task i decided to manually resolve all dependencies
public class GameKernelInitializer : MonoBehaviour {
	[SerializeField]
	private GameplayMainScreenView _gameplayMainScreenView;

	private OrderGeneratorService _orderGeneratorService;
	
	private CustomersControllerNew _customersControllerNew;
	private GameplayControllerNew _gameplayControllerNew;
	private FoodController _foodController;
	
	private void Awake() 
	{
		_orderGeneratorService = new OrderGeneratorService();
		_customersControllerNew = new CustomersControllerNew(_orderGeneratorService,_gameplayMainScreenView);
		
		_foodController = new FoodController(_gameplayMainScreenView, _customersControllerNew, _orderGeneratorService);
		_gameplayControllerNew = new GameplayControllerNew(_gameplayMainScreenView, _customersControllerNew, _foodController, _orderGeneratorService);
		
		_orderGeneratorService.Initialize();
		_gameplayControllerNew.Initialize();
		
		_gameplayControllerNew.InitGameSession();
	}
	
}
