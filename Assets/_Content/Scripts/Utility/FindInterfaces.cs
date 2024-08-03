using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Root.Utility
{
	public static class FindInterfaces
	{
		public static List<T> Find<T>()
		{
			var result = new List<T>();
			GameObject[] rootGos = SceneManager.GetActiveScene().GetRootGameObjects();
			foreach (var rootGo in rootGos)
			{
				result.AddRange(Find<T>(rootGo));
			}

			return result;
		}

		public static List<T> Find<T>(GameObject rootGameObject)
		{
			return rootGameObject.GetComponentsInChildren<T>().ToList();
		}
	}
}