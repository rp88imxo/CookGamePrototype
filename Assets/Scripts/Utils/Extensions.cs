using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CookingPrototype.Utilities {
	
	public static class Extensions
	{
		public static bool AnyNonAlloc<TObject>(this List<TObject> source, Func<TObject, bool> pred) {
			foreach (var o in source)
			{
				if (pred(o))
					return true;
			}
			return false;
		}
	}

}
