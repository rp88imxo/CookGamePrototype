using System;
using System.Collections.Generic;
using CookingPrototype.GameCore;
using CookingPrototype.Kitchen.Handlers;
using UnityEngine;

namespace CookingPrototype.Kitchen.Views {
public class OrderDataViewModel {
	public List<string> FoodComponents { get; set; }
}

public class OrderView : MonoBehaviour {
	[SerializeField]
	private OrderViewVisualizer _orderViewVisualizer;

	[SerializeField]
	private PointerInputHandler _oneTapHandler;

	[SerializeField]
	private PointerInputHandler _doubleTapHandler;

	private Action<OrderModelHandler> _onServeClicked;
	private Action<OrderModelHandler> _onTrashClicked;
	private OrderModelHandler _associatedModelHandler;

	public void Init(OrderModelHandler associatedModelHandler,
		Action<OrderModelHandler> onServeClicked,
		Action<OrderModelHandler> onTrashClicked) {
		_associatedModelHandler = associatedModelHandler;

		_onServeClicked = onServeClicked;
		_onTrashClicked = onTrashClicked;

		if ( _oneTapHandler != null ) {
			_oneTapHandler.Init(TapCallback);
		}

		if ( _doubleTapHandler != null ) {
			_doubleTapHandler.Init(DoubleTapCallback);
		}
	}

	public void Repaint(OrderDataViewModel orderDataViewModel) {
		_orderViewVisualizer.Repaint(orderDataViewModel.FoodComponents);
	}

	public void DestroySelf() {
		Destroy(gameObject);
	}

	#region TAP_CALLBACKS

	private void DoubleTapCallback() {
		_onTrashClicked?.Invoke(_associatedModelHandler);
	}

	private void TapCallback() {
		_onServeClicked?.Invoke(_associatedModelHandler);
	}

	#endregion
}
}