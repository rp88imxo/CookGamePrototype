using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CookingPrototype.Kitchen.Views {
	public class GameplayMainScreenView : View {
		[SerializeField]
		private CustomersViewPresenter _customersViewPresenter;

		[SerializeField]
		private FoodViewPresenter _foodViewPresenter;

		public FoodViewPresenter FoodViewPresenter => _foodViewPresenter;

		public CustomersViewPresenter CustomersViewPresenter => _customersViewPresenter;
	}
}

