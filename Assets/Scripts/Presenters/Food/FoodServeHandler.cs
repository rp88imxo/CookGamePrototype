using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CookingPrototype.Kitchen.Handlers {
public class FoodServeAssemblyData {
	public OrderAssemblyConfig OrderAssemblyConfig { get; set; }
	public CookableFoodConfig CookableFoodConfig { get; set; }
	public List<OrderModel> PossibleOrders { get; set; }
}

public abstract class FoodServeHandler<T> : MonoBehaviour
	where T : FoodServeAssemblyData {
	[SerializeField]
	protected BaseOrderAssemblyHandler _orderAssemblyHandler;

	[SerializeField]
	protected List<FoodPlacerHandler> _foodPlacerHandlers;

	public virtual void InitGameSession(T foodServeAssemblyData,
		Func<List<string>, bool> onServeClickedCallback) {
		_orderAssemblyHandler.Init(
			foodServeAssemblyData.OrderAssemblyConfig,
			foodServeAssemblyData.PossibleOrders,
			onServeClickedCallback);

		foreach ( var foodPlacer in _foodPlacerHandlers ) {
			foodPlacer.Init();
			foodPlacer.OnFoodPlaced +=
				ONTryAddFoodComponentClickedCallback;
		}
	}

	public virtual void HandleSessionEnded() {
		foreach ( var foodPlacer in _foodPlacerHandlers ) {
			foodPlacer.OnFoodPlaced -=
				ONTryAddFoodComponentClickedCallback;
		}

		_orderAssemblyHandler.HandleSessionEnded();
		_foodPlacerHandlers.ForEach(x => x.HandleSessionEnded());
	}

	protected virtual bool
		ONTryAddFoodComponentClickedCallback(Food arg) {
		return _orderAssemblyHandler.TryAddFoodComponent(arg);
	}
}
}