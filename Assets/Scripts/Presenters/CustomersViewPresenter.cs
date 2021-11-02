using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CookingPrototype.GameCore;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CookingPrototype.Kitchen.Views {

public class CustomersViewPresenter : MonoBehaviour {
		[SerializeField]
		private CustomerView _customerViewPrefab;

		[SerializeField]
		private SpawnPlacesHandler _spawnPlacesHandler;

		private readonly List<CustomerView> _customerViews =
			new List<CustomerView>();
		
		public void AddCustomerViews(IEnumerable<CustomerViewModel> customerViewModels) {
			foreach ( var customerViewModel in customerViewModels ) {
				CreateCustomerViewModel(customerViewModel);
			}
		}

		public void AddCustomerView(CustomerViewModel customerViewModel) {
			CreateCustomerViewModel(customerViewModel);
		}

		private void CreateCustomerViewModel(CustomerViewModel customerViewModel) {
			var freeSpawnPoint = _spawnPlacesHandler.GetSpawnPoint();
			if ( freeSpawnPoint == null ) {
				Debug.LogError(
					"There are no any free spawn points left! Cant Add more customer views This shouldn't be happens!",
					this);
				return;
			}
			var go = Instantiate(_customerViewPrefab,freeSpawnPoint );
			
			go.Repaint(customerViewModel).Forget();
			_customerViews.Add(go);
		}

		private void Clear() {
			Utilities.Utils.ClearMonoBehaviourCollection(_customerViews);
		}
		
	}
}

