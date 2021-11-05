using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CookingPrototype.Kitchen.Views {
	public class CustomerOrderView : MonoBehaviour {
		
		[SerializeField] 
		private GameObject _root;
		
		public string OrderName { get; private set; }

		public void Init(string orderName) {
			OrderName = orderName;
		}
		
		public void Toggle(bool state) {
			_root.SetActive(state);
		}

		// We can add here some cool animations when the part of the order is completed
		public void DestroySelf() {
			Destroy(gameObject);
		}
	}
}

