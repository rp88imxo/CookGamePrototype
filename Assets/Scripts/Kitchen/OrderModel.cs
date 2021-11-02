using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CookingPrototype.Kitchen {
	public sealed class OrderModel : IEquatable<OrderModel> {
		public class OrderFood {
			public string Name  { get; } = null;
			public string Needs { get; } = null;

			public OrderFood(string name, string needs) {
				Name  = name;
				Needs = needs;
			}
		}

		public readonly string               Name;
		public ReadOnlyCollection<OrderFood> Foods { get { return _foods.AsReadOnly(); } }

		List<OrderFood> _foods;

		public OrderModel(string name, List<OrderFood> foods) {
			Name   = name;
			_foods = foods;
		}

		public bool Equals(OrderModel other)
		{
			if (ReferenceEquals(null, other)) {
				return false;
			}

			if (ReferenceEquals(this, other)) {
				return true;
			}

			return Name == other.Name;
		}

		public override bool Equals(object obj)
		{
			return ReferenceEquals(this, obj) || obj is OrderModel other && Equals(other);
		}

		public override int GetHashCode()
		{
			return (Name != null ? Name.GetHashCode() : 0);
		}
	}
}
