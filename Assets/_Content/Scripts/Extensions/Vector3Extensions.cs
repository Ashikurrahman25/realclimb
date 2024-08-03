using UnityEngine;

namespace Extensions
{
	public static class Vector3Extensions
	{
		public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
		{
			Vector3 AB = b - a;
			Vector3 AV = value - a;
			return Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB);
		}
		
		public static bool InsideCircle(this Vector3 point, Vector3 center, float radius)
		{
			return Vector3.Distance(point, center) <= radius;
		}

		public static Vector2 RotateVector(this Vector2 vector, float angle)
		{
			float radian = angle * Mathf.Deg2Rad;
			float x = vector.x * Mathf.Cos(radian) - vector.y * Mathf.Sin(radian);
			float y = vector.x * Mathf.Sin(radian) + vector.y * Mathf.Cos(radian);
			return new Vector2(x, y);
		}
		
		public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
			return RotatePointAroundPivot(point, pivot, Quaternion.Euler(angles));
		}
 
		public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation) {
			return rotation * (point - pivot) + pivot;
		}
	}
}