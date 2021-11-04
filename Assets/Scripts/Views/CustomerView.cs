using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CookingPrototype.Kitchen.Views {

public class CustomerViewModel {
	public int Id { get; set; }
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

		[SerializeField]
		private Transform _ordersContainer;
		
		private readonly Dictionary<string,CustomerOrderView> _orders = new Dictionary<string,CustomerOrderView>();

		public async UniTaskVoid Repaint(CustomerViewModel customerViewModel) {
			_customerIcon.gameObject.SetActive(false);
			_customerIcon.sprite = await Resources.LoadAsync<Sprite>(customerViewModel.CustomerIconName) as Sprite;
			_customerIcon.gameObject.SetActive(true);
			
			_timerView.Init(customerViewModel.OrderInitialTime);
			CreateOrders(customerViewModel.OrdersViewsNames).Forget();
		}

		public void RepaintServedOrder(string orderModelName) {
			var orderView = _orders[orderModelName];
			orderView.DestroySelf();
			_orders.Remove(orderModelName);
		}

		public void RepaintTimer(float timeLeft) {
			_timerView.Repaint(timeLeft);
		}

		private async UniTaskVoid CreateOrders(IEnumerable<string> ordersViewsNames) {
			foreach ( var order in ordersViewsNames ) {
				var orderViewPrefab = await Resources.LoadAsync<GameObject>($"Prefabs/Orders/{order}") as CustomerOrderView;
				var go = Instantiate(orderViewPrefab, _ordersContainer);
				_orders.Add(order,go);
			}
		}
	}
}
