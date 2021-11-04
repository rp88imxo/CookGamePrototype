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
		var orderModelHandler = _orderViews.Keys.FirstOrDefault();
		ONServeClicked(orderModelHandler);
	}

	private void CreateViews() {
		foreach ( var spawnPoint in _spawnPlacesHandler
			.GetAllFreeSpawnPoints() ) {
			var modelHandler =
				new OrderModelHandler(DefaultPossibleOrders,
					ONOrderUpdatedCallback);
			var go = Instantiate(_orderViewPrefab, spawnPoint);
			go.Init(modelHandler, null, null);
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
	
	private void ONServeClicked(OrderModelHandler orderModelHandler) {
		var res= _onServeClicked?.Invoke(orderModelHandler.CurOrder);
		if ( res.HasValue && res.Value ) {
			RemoveView(orderModelHandler);
		}
	}
	
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