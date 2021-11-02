using System.Collections;
using System.Collections.Generic;
using CookingPrototype.Kitchen.Views;
using CookingPrototype.Services;
using UnityEngine;

namespace CookingPrototype.Kitchen.Controllers {

public class CustomersConfig {
	public int CustomersTargetNumber { get; set; } = 15;
	public float CustomerWaitTime { get; set; }= 18f;
	public float CustomerSpawnTime{ get; set; } = 3f;
}

	public class CustomersControllerNew {
		
		// Should be injected via DI
		private readonly CustomersViewPresenter _customersViewPresenter;
		private readonly OrderGeneratorService _orderGeneratorService;

		private CustomersConfig _currentCustomersConfig;
		
		public CustomersControllerNew(CustomersViewPresenter customersViewPresenter, OrderGeneratorService orderGeneratorService) {
			_customersViewPresenter = customersViewPresenter;
			_orderGeneratorService = orderGeneratorService;
		}

		public void InitGameSession(CustomersConfig config) {
			_currentCustomersConfig = config;
		}
		
		
	}
}

