using System;
using System.Collections;
using System.Collections.Generic;
using CookingPrototype.GameCore;
using CookingPrototype.Kitchen.Handlers;
using UnityEngine;

namespace CookingPrototype.Kitchen.Views {

public class BurgerOrderViewModel {
	public List<string> FoodComponents { get; set; }
}

public class OrderView : MonoBehaviour {
	[SerializeField]
	private OrderViewVisualizer _orderViewVisualizer;
	
	[SerializeField]
	private PointerInputHandler _oneTapHandler;

	[SerializeField]
	private PointerInputHandler _doubleTapHandler;
	
	private Action _onServeClicked;
	private Action _onTrashClicked;
	
	public void Init(Action onServeClicked, Action onTrashClicked) {
		_onServeClicked = onServeClicked;
		_onTrashClicked = onTrashClicked;
		
		_oneTapHandler.Init(TapCallback);
		_doubleTapHandler.Init(DoubleTapCallback);
	}
	
	public void Repaint(BurgerOrderViewModel burgerOrderViewModel) {
		_orderViewVisualizer.Repaint(burgerOrderViewModel.FoodComponents);
	}
	
	public void DestroySelf() {
		Destroy(gameObject);
	}
	
	#region TAP_CALLBACKS

	private void DoubleTapCallback() {
		_onTrashClicked?.Invoke();
	}

	private void TapCallback() {
		_onServeClicked?.Invoke();
	}

	#endregion
}

}
