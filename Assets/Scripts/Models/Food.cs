
using CookingPrototype.GameCore;

namespace CookingPrototype.Kitchen {
	public sealed class Food : IPrototype<Food> {
		public enum FoodStatus {
			Empty,
			Raw,
			Cooked,
			Overcooked
		}

		public string     Name      { get; }
		public FoodStatus CurStatus { get; private set; }

		public Food(string name, FoodStatus status = FoodStatus.Raw) {
			Name      = name;
			CurStatus = status;
		}

		public void CookStep() {
			switch ( CurStatus ) {
				case FoodStatus.Raw: {
					CurStatus = FoodStatus.Cooked;
					return;
				}
				case FoodStatus.Cooked: {
					CurStatus = FoodStatus.Overcooked;
					return;
				}
				default: {
					return;
				}
			}
		}

		public Food Clone() {
			return (Food)MemberwiseClone();
		}
	}
}
