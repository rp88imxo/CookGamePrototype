using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CookingPrototype.Kitchen.Handlers {
public abstract class BaseOrderAssemblyHandler : MonoBehaviour {
	
	protected OrderAssemblyConfig _currentOrderAssemblyConfig;
	protected List<OrderModel> DefaultPossibleOrders;
	protected Func<List<string>, bool> _onServeClicked;
	
	public abstract bool TryAddFoodComponent(Food food);
	
	public virtual void Init(OrderAssemblyConfig orderAssemblyConfig,
		List<OrderModel> possibleOrders,
		Func<List<string>, bool> onServeClickedCallback) {
		DefaultPossibleOrders = possibleOrders;
		_onServeClicked = onServeClickedCallback;
		_currentOrderAssemblyConfig = orderAssemblyConfig;
	}
}
}
