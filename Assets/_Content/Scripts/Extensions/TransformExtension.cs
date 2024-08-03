using System.Collections.Generic;
using UnityEngine;

public static class TransformExtension
{
	private static Rect GetScreenCoordinates(RectTransform uiElement)
	{
		var worldCorners = new Vector3[4];
		uiElement.GetWorldCorners(worldCorners);
		var result = new Rect(
			worldCorners[0].x,
			worldCorners[0].y,
			worldCorners[2].x - worldCorners[0].x,
			worldCorners[2].y - worldCorners[0].y);
		return result;
	}

	public static Vector3 GetDestinationWorldPoint(this Transform destination, Camera camera, LayerMask groundMask)
	{
		var rectTransform = destination.GetComponent<RectTransform>();
		if (rectTransform != null)
		{
			return GetDestinationWorldPoint(rectTransform, camera, groundMask);
		}

		return destination.position;
	}

	public static Vector3 GetDestinationWorldPoint(this RectTransform rectTransform, Camera camera,
		LayerMask groundMask)
	{
		var screenCoords = GetScreenCoordinates(rectTransform);
		var ray = camera.ScreenPointToRay(screenCoords.center);
		if (Physics.Raycast(ray, out var hit, 10000f, groundMask))
		{
			return hit.point - ray.direction * 5f; // + Vector3.up * 10f;
		}

		return rectTransform.position;
	}

	public static Transform Clear(this Transform transform)
	{
		foreach (Transform child in transform)
		{
#if UNITY_EDITOR
			GameObject.DestroyImmediate(child.gameObject);
#else
			GameObject.Destroy(child.gameObject);
#endif
		}

		return transform;
	}

	public static Transform FindDeepChild(this Transform aParent, string aName)
	{
		Queue<Transform> queue = new Queue<Transform>();
		queue.Enqueue(aParent);
		while (queue.Count > 0)
		{
			var c = queue.Dequeue();
			if (c.name.StartsWith(aName))
				return c;
			foreach (Transform t in c)
				queue.Enqueue(t);
		}

		return null;
	}

	public static Transform FindDeepChildWithTag(this Transform aParent, string tag)
	{
		Queue<Transform> queue = new Queue<Transform>();
		queue.Enqueue(aParent);
		while (queue.Count > 0)
		{
			var c = queue.Dequeue();
			if (c.CompareTag(tag))
				return c;
			foreach (Transform t in c)
				queue.Enqueue(t);
		}

		return null;
	}

	public static T FindDeepParent<T>(this Transform aParent)
	{
		Queue<Transform> queue = new Queue<Transform>();
		queue.Enqueue(aParent);
		while (queue.Count > 0)
		{
			var c = queue.Dequeue();
			if (c.parent == null)
				return default(T);

			var comp = c.parent.GetComponent<T>();
			if (comp != null)
				return comp;

			queue.Enqueue(c.parent);
		}

		return default(T);
	}
}