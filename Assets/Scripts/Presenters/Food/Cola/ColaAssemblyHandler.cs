using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CookingPrototype.GameCore;
using CookingPrototype.Kitchen.Views;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CookingPrototype.Kitchen.Handlers {
public class ColaAssemblyHandler
	: BaseOrderAssemblyHandler {
	[SerializeField]
	private OrderView _orderViewPrefab;

	[SerializeField]
	private List<Transform> _spawnPlaces;

	[SerializeField]
	private SpawnPlacesHandler _spawnPlacesHandler;

	[SerializeField]
	private PointerInputHandler _serveColaInput;

	public override event Action OrderServed;

	private readonly Dictionary<OrderModelHandler, OrderView> _orderViews
		=
		new Dictionary<OrderModelHandler, OrderView>();

	public override void Init(OrderAssemblyConfig orderAssemblyConfig,
		List<OrderModel> possibleOrders,
		Func<List<string>, bool> onServeClickedCallback) {
		base.Init(orderAssemblyConfig,
			possibleOrders,
			onServeClickedCallback);

		_serveColaInput.Init(_serveZoneTapHandler);
		_spawnPlaces.ForEach(x => x.gameObject.SetActive(false));
		var totalActivePlaces = _spawnPlaces
			.Take(_currentOrderAssemblyConfig.TotalPlaces)
			.ToList();
		totalActivePlaces.ForEach(x => x.gameObject.SetActive(true));
		_spawnPlacesHandler.AddSpawnPoints(totalActivePlaces);

		CreateViews();
	}

	public void _serveZoneTapHandler() {
		var orderModelHandler =
			_orderViews.Keys.FirstOrDefault(x => x.CurOrder.Count > 0);
		if ( orderModelHandler != null ) {
			ONServeClicked(orderModelHandler);
		}
	}

	private void CreateViews() {
		foreach ( var spawnPoint in _spawnPlacesHandler
			.GetAllFreeSpawnPoints() ) {
			var modelHandler =
				new OrderModelHandler(DefaultPossibleOrders,
					ONOrderUpdatedCallback);
			var go = Instantiate(_orderViewPrefab, spawnPoint);
			go.Init(modelHandler, null, null);
			go.Repaint(new OrderDataViewModel());

			_orderViews.Add(modelHandler, go);
		}
	}

	private void ONOrderUpdatedCallback(OrderModelHandler obj) {
		var view = _orderViews[obj];
		view.Repaint(new OrderDataViewModel {
			FoodComponents = obj.CurOrder
		});
	}

	private void ONServeClicked(OrderModelHandler orderModelHandler) {
		var res = _onServeClicked?.Invoke(orderModelHandler.CurOrder);
		if ( res.HasValue && res.Value ) {
			RemoveView(orderModelHandler);
			OrderServed?.Invoke();
		}
	}

	private void RemoveView(OrderModelHandler orderModelHandler) {
		var orderView = _orderViews[orderModelHandler];
		orderView.Repaint(new OrderDataViewModel());
		orderModelHandler.Reset(DefaultPossibleOrders);
	}


	public override bool TryAddFoodComponent(Food food) {
		return _orderViews.Keys.Any(orderModelHandler
			=> orderModelHandler.TryPlaceFood(food));
	}

	public override void HandleSessionEnded() {
		foreach ( var keyValuePair in _orderViews ) {
			Destroy(keyValuePair.Value.gameObject);
		}

		_spawnPlacesHandler.RemoveAllPoints();
		_orderViews.Clear();
	}
}
}