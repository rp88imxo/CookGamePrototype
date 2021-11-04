﻿using System;
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

	public List<string> CurOrder => new List<string>(_curOrder);

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

public class OrderAssemblyHandler : BaseOrderAssemblyHandler {
	[SerializeField]
	private OrderView _orderViewPrefab;

	[SerializeField]
	private List<Transform> _spawnPlaces;

	[SerializeField]
	private SpawnPlacesHandler _spawnPlacesHandler;

	private readonly Dictionary<OrderModelHandler, OrderView> _orderViews
		=
		new Dictionary<OrderModelHandler, OrderView>();

	public override void Init(OrderAssemblyConfig orderAssemblyConfig,
		List<OrderModel> possibleOrders,
		Func<List<string>, bool> onServeClickedCallback) {
		base.Init(orderAssemblyConfig,
			possibleOrders,
			onServeClickedCallback);

		_spawnPlaces.ForEach(x => x.gameObject.SetActive(false));
		var totalActivePlaces = _spawnPlaces
			.Take(_currentOrderAssemblyConfig.TotalPlaces)
			.ToList();
		totalActivePlaces.ForEach(x => x.gameObject.SetActive(true));
		_spawnPlacesHandler.AddSpawnPoints(totalActivePlaces);

		CreateViews();
	}

	private void CreateViews() {
		foreach ( var spawnPoint in _spawnPlacesHandler
			.GetAllFreeSpawnPoints() ) {
			var modelHandler =
				new OrderModelHandler(DefaultPossibleOrders,
					ONOrderUpdatedCallback);
			var go = Instantiate(_orderViewPrefab, spawnPoint);
			go.Init(modelHandler, ONServeClicked, ONTrashClicked);
			go.Repaint(new OrderDataViewModel {FoodComponents = null});

			_orderViews.Add(modelHandler, go);
		}
	}

	private void ONOrderUpdatedCallback(OrderModelHandler obj) {
		var view = _orderViews[obj];
		view.Repaint(new OrderDataViewModel {
			FoodComponents = obj.CurOrder
		});
	}

	#region ORDER_VIEW_CALLBACKS

	private void ONTrashClicked(OrderModelHandler orderModelHandler) {
		RemoveView(orderModelHandler);
	}

	private void ONServeClicked(OrderModelHandler orderModelHandler) {
	var res= _onServeClicked?.Invoke(orderModelHandler.CurOrder);
	if ( res.HasValue && res.Value ) {
		RemoveView(orderModelHandler);
	}
	}

	#endregion

	private void RemoveView(OrderModelHandler orderModelHandler) {
		var orderView = _orderViews[orderModelHandler];
		orderView.DestroySelf();
		_orderViews.Remove(orderModelHandler);
	}
	
	public override bool TryAddFoodComponent(Food food) {
		return _orderViews.Keys.Any(orderModelHandler
			=> orderModelHandler.TryPlaceFood(food));
	}
}
}