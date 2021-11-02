using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CookingPrototype.Kitchen.Views {
public class TimerView : MonoBehaviour {
	[SerializeField]
	private Image _timerProgressBar;

	private float _initialTime;
	
	public void Init(float initialTime) {
		_initialTime = initialTime;
		_timerProgressBar.fillAmount = 1.0f;
	}

	public void Repaint(float leftTime) {
		_timerProgressBar.fillAmount = leftTime / _initialTime;
	}
}
}

