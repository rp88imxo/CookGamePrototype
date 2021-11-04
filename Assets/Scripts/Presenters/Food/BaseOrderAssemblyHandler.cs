using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CookingPrototype.Kitchen.Handlers {
public abstract class BaseOrderAssemblyHandler : MonoBehaviour {
	public abstract bool TryAddFoodComponent(Food food);
}
}
