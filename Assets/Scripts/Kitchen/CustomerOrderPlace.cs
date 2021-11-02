using UnityEngine;

namespace CookingPrototype.Kitchen {
	public sealed class CustomerOrderPlace : MonoBehaviour {
		public OrderModel CurOrderModel { get; private set; } = null;

		public bool IsActive { get { return CurOrderModel != null; } }

		public void Init(OrderModel orderModel) {
			CurOrderModel = orderModel;
			gameObject.SetActive(true);
		}

		public void Complete() {
			CurOrderModel = null;
			gameObject.SetActive(false);
		}
	}
}
