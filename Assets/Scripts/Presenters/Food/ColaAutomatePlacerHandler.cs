using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CookingPrototype.Kitchen.Handlers {
public class ColaAutomatePlacerHandler : BaseFoodPlacerHandler {
	public override event Func<Food, bool> OnFoodPlaced;
}
}
