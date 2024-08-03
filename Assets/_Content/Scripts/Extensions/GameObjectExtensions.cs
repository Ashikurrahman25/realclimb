using System.Collections.Generic;
using UnityEngine;

namespace Extensions
{
	public static class GameObjectExtensions
	{
		private static Dictionary<GameObject, Dictionary<System.Type, object>> s_GameObjectThisOrParentComponentMap =
			new Dictionary<GameObject, Dictionary<System.Type, object>>();
		private static Dictionary<GameObject, Dictionary<System.Type, object>> s_GameObjectComponentMap =
			new Dictionary<GameObject, Dictionary<System.Type, object>>();

		private static Dictionary<GameObject, Dictionary<System.Type, object>> s_GameObjectParentComponentMap =
			new Dictionary<GameObject, Dictionary<System.Type, object>>();

		private static Dictionary<GameObject, Dictionary<System.Type, object>>
			s_GameObjectInactiveParentComponentMap = new Dictionary<GameObject, Dictionary<System.Type, object>>();

		private static Dictionary<GameObject, Dictionary<System.Type, object[]>> s_GameObjectComponentsMap =
			new Dictionary<GameObject, Dictionary<System.Type, object[]>>();

		private static Dictionary<GameObject, Dictionary<System.Type, object[]>> s_GameObjectParentComponentsMap =
			new Dictionary<GameObject, Dictionary<System.Type, object[]>>();

		public static T GetCachedComponentThisOrParent<T>(this GameObject gameObject)
		{
			Dictionary<System.Type, object> dictionary;
			if (GameObjectExtensions.s_GameObjectThisOrParentComponentMap.TryGetValue(gameObject, out dictionary))
			{
				object obj;
				if (dictionary.TryGetValue(typeof(T), out obj))
					return (T) obj;
			}
			else
			{
				dictionary = new Dictionary<System.Type, object>();
				GameObjectExtensions.s_GameObjectThisOrParentComponentMap.Add(gameObject, dictionary);
			}

			T component = gameObject.GetComponent<T>() ?? gameObject.GetComponentInParent<T>();;
			dictionary.Add(typeof(T), (object) component);
			return component;
		}
		
		public static T GetCachedComponent<T>(this GameObject gameObject)
		{
			Dictionary<System.Type, object> dictionary;
			if (GameObjectExtensions.s_GameObjectComponentMap.TryGetValue(gameObject, out dictionary))
			{
				object obj;
				if (dictionary.TryGetValue(typeof(T), out obj))
					return (T) obj;
			}
			else
			{
				dictionary = new Dictionary<System.Type, object>();
				GameObjectExtensions.s_GameObjectComponentMap.Add(gameObject, dictionary);
			}

			T component = gameObject.GetComponent<T>();
			dictionary.Add(typeof(T), (object) component);
			return component;
		}

		public static T GetCachedParentComponent<T>(this GameObject gameObject)
		{
			Dictionary<System.Type, object> dictionary;
			if (GameObjectExtensions.s_GameObjectParentComponentMap.TryGetValue(gameObject, out dictionary))
			{
				object obj;
				if (dictionary.TryGetValue(typeof(T), out obj))
					return (T) obj;
			}
			else
			{
				dictionary = new Dictionary<System.Type, object>();
				GameObjectExtensions.s_GameObjectParentComponentMap.Add(gameObject, dictionary);
			}

			T componentInParent = gameObject.GetComponentInParent<T>();
			dictionary.Add(typeof(T), (object) componentInParent);
			return componentInParent;
		}

		public static T[] GetCachedComponents<T>(this GameObject gameObject)
		{
			Dictionary<System.Type, object[]> dictionary;
			if (GameObjectExtensions.s_GameObjectComponentsMap.TryGetValue(gameObject, out dictionary))
			{
				object[] objArray;
				if (dictionary.TryGetValue(typeof(T), out objArray))
					return (object) objArray as T[];
			}
			else
			{
				dictionary = new Dictionary<System.Type, object[]>();
				GameObjectExtensions.s_GameObjectComponentsMap.Add(gameObject, dictionary);
			}

			T[] components = gameObject.GetComponents<T>();
			dictionary.Add(typeof(T), (object) components as object[]);
			return components;
		}

		public static T[] GetCachedParentComponents<T>(this GameObject gameObject)
		{
			Dictionary<System.Type, object[]> dictionary;
			if (GameObjectExtensions.s_GameObjectParentComponentsMap.TryGetValue(gameObject, out dictionary))
			{
				object[] objArray;
				if (dictionary.TryGetValue(typeof(T), out objArray))
					return (object) objArray as T[];
			}
			else
			{
				dictionary = new Dictionary<System.Type, object[]>();
				GameObjectExtensions.s_GameObjectParentComponentsMap.Add(gameObject, dictionary);
			}

			T[] componentsInParent = gameObject.GetComponentsInParent<T>();
			dictionary.Add(typeof(T), (object) componentsInParent as object[]);
			return componentsInParent;
		}

		public static T GetCachedInactiveComponentInParent<T>(this GameObject gameObject) where T : Component
		{
			Dictionary<System.Type, object> dictionary;
			if (GameObjectExtensions.s_GameObjectInactiveParentComponentMap.TryGetValue(gameObject, out dictionary))
			{
				object obj;
				if (dictionary.TryGetValue(typeof(T), out obj))
					return (T) obj;
			}
			else
			{
				dictionary = new Dictionary<System.Type, object>();
				GameObjectExtensions.s_GameObjectInactiveParentComponentMap.Add(gameObject, dictionary);
			}

			T obj1 = (T) null;
			Transform transform = gameObject.transform;
			while ((UnityEngine.Object) transform != (UnityEngine.Object) null &&
			       !((UnityEngine.Object) (obj1 = transform.GetComponent<T>()) != (UnityEngine.Object) null))
				transform = transform.parent;
			dictionary.Add(typeof(T), (object) obj1);
			return obj1;
		}
	}
}