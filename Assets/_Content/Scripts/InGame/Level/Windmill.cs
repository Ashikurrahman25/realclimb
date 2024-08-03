using UnityEngine;

namespace _Content.InGame.Level
{
	public class Windmill: MonoBehaviour
	{
		[SerializeField] private float _rotationSpeed;
		private Rigidbody _rigidbody;
		private float _currentAngle;

		private void Awake()
		{
			_rigidbody = GetComponent<Rigidbody>();
			_currentAngle = 0f;
		}

		private void FixedUpdate()
		{
			var deltaTime = Time.fixedDeltaTime;
			_currentAngle += _rotationSpeed * deltaTime;
			_rigidbody.MoveRotation(Quaternion.Euler(0f, 0f, _currentAngle));
		}
	}
}