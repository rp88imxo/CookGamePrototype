using System.Collections.Generic;
using System.Linq;

namespace CookingPrototype.GameCore {
public interface IPrototype<out T>
{
	T Clone();
}

public static class PrototypeExtension
{
	public static IEnumerable<T> Clone<T>(this IEnumerable<T> list)
		where T : IPrototype<T>
	{
		return list.Select(item => item.Clone()).ToList();
	}
    
	public static List<T> Clone<T>(this List<T> list)
		where T : IPrototype<T>
	{
		return list.Select(item => item.Clone()).ToList();
	}
    
	public static Dictionary<TKey, TValue> Clone<TKey, TValue>(
		this Dictionary<TKey, TValue> dictionary)
		where TValue : IPrototype<TValue>
	{
		var slots = new Dictionary<TKey, TValue>();

		foreach (var slot in dictionary)
		{
			slots[slot.Key] = slot.Value.Clone();
		}

		return slots;
	}
}

}

