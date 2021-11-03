using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CookingPrototype.GameCore;
using CookingPrototype.Kitchen.Views;
using UnityEngine;

namespace CookingPrototype.Kitchen.Handlers {

public class OrderAssemblyConfig {
	public int TotalPlaces { get; set; }
}

public class OrderModelHandler {

	private List<OrderModel> _possibleOrders = new List<OrderModel>();
	private List<string> _curOrder = new List<string>();

	private Action<OrderModelHandler> _onOrderUpdated;

	public OrderModelHandler(IEnumerable<OrderModel> possibleOrders,
		Action<OrderModelHandler> onOrderUpdatedCallback) {
		_possibleOrders.AddRange(possibleOrders);
		_onOrderUpdated = onOrderUpdatedCallback;
	}

	private bool CanAddFood(Food food) {
		if ( _curOrder.Contains(food.Name) ) {
			return false;
		}

		return _possibleOrders
			.SelectMany(order
				=> order.Foods.Where(x => x.Name == food.Name))
			.Any(orderFood => string.IsNullOrEmpty(orderFood.Needs)
				|| _curOrder.Contains(orderFood.Needs));
	}

	private void UpdatePossibleOrders() {
		var ordersToRemove = _possibleOrders.Where(order
				=> order.Foods.Count(x
					=> x.Name == _curOrder[_curOrder.Count - 1])
				== 0)
			.ToList();

		_possibleOrders = _possibleOrders.Except(ordersToRemove).ToList();
	}

	public bool TryPlaceFood(Food food) {
		if ( !CanAddFood(food) ) {
			return false;
		}

		_curOrder.Add(food.Name);
		UpdatePossibleOrders();
		_onOrderUpdated?.Invoke(this);
		return true;
	}
	
}

public class OrderAssemblyHandler : MonoBehaviour {
	[SerializeField]
	private OrderView _orderViewPrefab;
	
	[SerializeField]
	private List<Transform> _spawnPlaces;

	[SerializeField]
	private SpawnPlacesHandler _burgerSpawnPlacesHandler;
	
	private OrderAssemblyConfig _currentOrderAssemblyConfig;
	
	private Action<OrderModel> _onServeClicked;
	
	public void Init(OrderAssemblyConfig orderAssemblyConfig, Action<OrderModel> onServeClickedCallback) {
		_onServeClicked = onServeClickedCallback;
		_currentOrderAssemblyConfig = orderAssemblyConfig;
		
		_spawnPlaces.ForEach(x => x.gameObject.SetActive(false));
		var totalActivePlaces = _spawnPlaces
			.Take(_currentOrderAssemblyConfig.TotalPlaces)
			.ToList();
		totalActivePlaces.ForEach(x => x.gameObject.SetActive(true));
		_burgerSpawnPlacesHandler.AddSpawnPoints(totalActivePlaces);
	}

	public bool TryAddFoodComponent(Food food) {
		
	}
	
}
}

