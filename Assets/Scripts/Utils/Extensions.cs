using System;
using System.Collections;
using System.Collections.Generic;
using CookingPrototype.Kitchen;
using CookingPrototype.Kitchen.Views;
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

public static class Utils {
	public static void ClearMonoBehaviourCollection<T>(ICollection<T> list)
		where T : MonoBehaviour
	{
		if (list.Count == 0)
			return;
        
		foreach (var monoBehaviour in list)
			UnityEngine.Object.Destroy(monoBehaviour.gameObject);
        
		list.Clear();
	}
	
	public static TimeSpan ToTimeSpanSeconds(this float value)
		=> TimeSpan.FromSeconds(value);
}

}
