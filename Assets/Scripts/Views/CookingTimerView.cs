using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CookingPrototype.Kitchen.Views {

[Serializable]
public class CookingTimerViewModel {
	[SerializeField]
	private Food.FoodStatus _foodState;

	[SerializeField]
	private GameObject _rootImages;
	
	[SerializeField]
	private Image _timerImage;

	public Food.FoodStatus FoodStatus => _foodState;

	public Image TimerImage => _timerImage;

	public GameObject RootImages => _rootImages;
}

public class CookingTimerView : MonoBehaviour {
	[SerializeField]
	private List<CookingTimerViewModel> _cookingTimerViewModels;

	private float _initialTime;

	private CookingTimerViewModel _currentTimer;
	
	public void Init(float initialTime, Food.FoodStatus foodStatus) {
		_initialTime = initialTime;
		_cookingTimerViewModels.ForEach(x => 
			{
				if ( x.FoodStatus == foodStatus ) {
					x.RootImages.SetActive(true);
					_currentTimer = x;
				}
				else {
					x.RootImages.SetActive(false);
				}
				
				x.TimerImage.fillAmount = 1.0f; 
			}
		);
	}

	public void Repaint(float leftTime) {
		_currentTimer.TimerImage.fillAmount = 1f - leftTime / _initialTime;
	}

	public void ToggleTimer(bool state) {
		gameObject.SetActive(state);
	}
}
}

