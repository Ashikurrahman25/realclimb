using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public static class ArrayExtensions
{
	public static T MoveElement<T>(this List<T> items, int oldIndex, int newIndex)
	{
		T item = items[oldIndex];
		items.RemoveAt(oldIndex);
		items.Insert(newIndex, item);
		return item;
	}
	
	// Return a random item from an array.1
	public static T GetRandomElement<T>(this T[] items)
	{
		// Return a random item.
		return items[Random.Range(0, items.Length)];
	}

	// Return a random item from a list.
	public static T GetRandomElement<T>(this List<T> items)
	{
		// Return a random item.
		return items[Random.Range(0, items.Count)];
	}

	public static T PopRandomElement<T>(this List<T> items)
	{
		// Return a random item.
		var index = Random.Range(0, items.Count);
		var item = items[index];
		items.RemoveAt(index);
		return item;
	}

	public static T PopElementAtIndex<T>(this List<T> items, int index)
	{
		var item = items[index];
		items.RemoveAt(index);
		return item;
	}

	public static void Shuffle<T>(this IList<T> list)
	{
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = Random.Range(0, n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}

	public static void AddIfNotExist<T>(this List<T> source, T element)
	{
		if (!source.Contains(element))
			source.Add(element);
	}

	public static void AddIfNotExist<T>(this List<T> source, List<T> elements)
	{
		foreach (var element in elements)
		{
			if (!source.Contains(element))
				source.Add(element);
		}
	}

	public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
	{
		return source
			.Select((x, i) => new {Index = i, Value = x})
			.GroupBy(x => x.Index / chunkSize)
			.Select(x => x.Select(v => v.Value).ToList())
			.ToList();
	}
}