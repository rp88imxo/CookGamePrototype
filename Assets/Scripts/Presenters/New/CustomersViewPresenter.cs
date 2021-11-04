using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CookingPrototype.GameCore;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CookingPrototype.Kitchen.Views {

public class CustomersViewPresenter : View {
		[SerializeField]
		private CustomerView _customerViewPrefab;

		[SerializeField]
		private SpawnPlacesHandler _spawnPlacesHandler;

		public void InitGameSession() {
			Clear();
		}
		
		public bool HasFreeSpawnPoint
			=> _spawnPlacesHandler.HasAnyFreeSpawnPoint;

		private readonly Dictionary<int, CustomerView> _customerViews =
			new Dictionary<int, CustomerView>();
		
		public void AddCustomerViews(IEnumerable<CustomerViewModel> customerViewModels) {
			foreach ( var customerViewModel in customerViewModels ) {
				CreateCustomerViewModel(customerViewModel);
			}
		}

		public void AddCustomerView(CustomerViewModel customerViewModel) {
			CreateCustomerViewModel(customerViewModel);
		}

		public void RemoveCustomerViewModelById(int obj) {
			// No need to check if we actually have a key since we don't expect to miss it
			var c = _customerViews[obj];
			_customerViews.Remove(obj);
			Destroy(c.gameObject);
		}

		public void RepaintCustomerViewModelTimerById(int arg1Id,
			TimeSpan timeSpan) {
			var c = _customerViews[arg1Id];
			c.RepaintTimer((float)timeSpan.TotalSeconds);
		}

		public void ServeOrderByName(int customerId,
			string orderModelName) {
			var c = _customerViews[customerId];
			c.RepaintServedOrder(orderModelName);
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
			_customerViews.Add(customerViewModel.Id,go);
		}

		private void Clear() {
			foreach (var customer in _customerViews.Values)
			{
				Destroy(customer.gameObject);
			}
			
			_customerViews.Clear();
		}

	
}
}

