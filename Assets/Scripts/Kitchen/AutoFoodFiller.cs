using System;
using UnityEngine;

using System.Collections.Generic;

namespace CookingPrototype.Kitchen {
	public sealed class AutoFoodFiller : MonoBehaviour {
		public string                  FoodName = null;
		public List<AbstractFoodPlace> Places   = new List<AbstractFoodPlace>();

		private Food _food;
		
		private void Start() {
			_food = new Food(FoodName);
		}

		void Update() {
			foreach ( var place in Places ) {
				place.TryPlaceFood(_food);
			}
		}
	}
}
