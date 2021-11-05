using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CookingPrototype.Kitchen.Handlers {
public abstract class BaseOrderAssemblyHandler : MonoBehaviour {
	
	protected OrderAssemblyConfig _currentOrderAssemblyConfig;
	protected List<OrderModel> DefaultPossibleOrders;
	protected Action<List<string>, Action, Action> _onServeClicked;
	public abstract event Action OrderServed;

	public abstract bool TryAddFoodComponent(Food food);
	
	public virtual void Init(OrderAssemblyConfig orderAssemblyConfig,
		List<OrderModel> possibleOrders,
		Action<List<string>, Action, Action> onServeClickedCallback) {
		DefaultPossibleOrders = possibleOrders;
		_onServeClicked = onServeClickedCallback;
		_currentOrderAssemblyConfig = orderAssemblyConfig;
	}

	public abstract void HandleSessionEnded();
}
}
