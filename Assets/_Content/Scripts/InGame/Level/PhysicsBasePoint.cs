using UnityEngine;

namespace _Content.InGame.Level
{
	public class PhysicsBasePoint: MonoBehaviour
	{
		[SerializeField] private bool _useZAxisForRotation;
		[SerializeField] private Vector3 _offset;

		public bool UseZAxisForRotation => _useZAxisForRotation;
		public Vector3 Offset => _offset;
	}
}