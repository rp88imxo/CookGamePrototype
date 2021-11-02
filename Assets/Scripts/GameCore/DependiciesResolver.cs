using System;
using System.Collections;
using System.Collections.Generic;
using CookingPrototype.Kitchen.Controllers;
using CookingPrototype.Kitchen.Views;
using CookingPrototype.Services;
using UnityEngine;

public class DependiciesResolver : MonoBehaviour {
	[SerializeField]
	private GameplayMainScreenView _gameplayMainScreenView;

	private CustomersControllerNew _customersControllerNew;
	
	private void Awake() 
	{
		var orderGeneratorService = new OrderGeneratorService();
		_customersControllerNew = new CustomersControllerNew(orderGeneratorService,_gameplayMainScreenView);
		
	}
}
