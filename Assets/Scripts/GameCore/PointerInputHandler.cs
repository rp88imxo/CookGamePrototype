using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;


namespace CookingPrototype.GameCore {
public class PointerInputHandler : MonoBehaviour, IPointerUpHandler {
	
	[SerializeField]
	private int _neededPointerTaps;
	
	[SerializeField]
	private float _timeBetweenTaps = 0.5f;

	private Action _taped;

	private int _currentTapCount;
	private float _currentTime;
	
	public void Init(Action tapCallback) {
		_taped = tapCallback;
	}
	
	public void OnPointerUp(PointerEventData eventData) {
		
		if ( ++_currentTapCount == _neededPointerTaps) {
			_taped?.Invoke();
		}

		_currentTime = 0f;
	}

	private void Update() {
		_currentTime += Time.deltaTime;
		if ( _currentTime >  _timeBetweenTaps) {
			_currentTapCount = 0;
		}
	}
	
	
}
}

