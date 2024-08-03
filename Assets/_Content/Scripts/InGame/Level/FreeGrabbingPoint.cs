using UnityEngine;

namespace _Content.InGame.Level
{
	public class FreeGrabbingPoint: MonoBehaviour
	{
		[SerializeField] private Vector3 _offset;

		public Vector3 GetNearestPosition(Vector3 grabPosition)
		{
			var position = grabPosition;
			grabPosition.y = transform.position.y;
			grabPosition += _offset;

			return grabPosition;
		}
	}
}