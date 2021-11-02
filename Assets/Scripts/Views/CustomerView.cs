using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CookingPrototype.Kitchen.Views {

public class CustomerViewModel {
	// Should be replaced with assetReference after Prototype stage
	public string CustomerIconName { get; set; }
	public float OrderInitialTime { get; set; }

	public List<string> OrdersViewsNames { get; set; }
}

	public class CustomerView : MonoBehaviour 
	{
		[SerializeField]
		private Image _customerIcon;

		[SerializeField]
		private TimerView _timerView;
		
		private readonly List<OrderView> _orders = new List<OrderView>();

		public void Repaint(CustomerViewModel customerViewModel) {
			
			//_customerIcon.sprite = Resources.LoadAsync<Sprite>(customerViewModel.CustomerIconName);
			_timerView.Init(customerViewModel.OrderInitialTime);
		}

		public void RepaintTimer(float timeLeft) {
			_timerView.Repaint(timeLeft);
		}
	}
}
