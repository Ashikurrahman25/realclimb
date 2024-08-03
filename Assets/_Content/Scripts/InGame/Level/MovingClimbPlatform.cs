using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace _Content.InGame.Level
{
	public class MovingClimbPlatform : MonoBehaviour
	{
		[SerializeField] private float _speed;
		[SerializeField] private float _waitTime;
		[SerializeField] private List<Transform> _positions;
		[SerializeField] private bool _moveOnlyAfterGrabbing = true;
		[SerializeField] private bool _moveOnlyOneWay = true;

		[SerializeField] private Rigidbody _rigidbody;
		[Header("Visual")] [SerializeField] private Transform _start;
		[SerializeField] private Transform _end;
		[SerializeField] private Transform _center;
		[SerializeField] private Transform _grab;
		private int _currentPointsIndex;
		private float _waitTimer;

		public bool OnceGrabbed { get; set; }

		[Button()]
		public void AlignVisual()
		{
			if (_positions.Count < 2)
				return;

			var startPos = _positions[0].position;
			var endPos = _positions[1].position;
			var direction = endPos - startPos;
			direction.z = 0f;
			var angle = Vector3.SignedAngle(Vector3.right, direction, Vector3.forward);
			var endAngle = Vector3.SignedAngle(Vector3.left, -direction, Vector3.forward);
			//_start.position = startPos;
			_start.rotation = Quaternion.Euler(0f, 0f, angle);
			//_end.position = endPos;
			_end.rotation = Quaternion.Euler(0f, 0f, endAngle);
			_center.position = startPos + direction / 2f;
			_center.rotation = Quaternion.Euler(0f, 0f, angle);
			_grab.position = startPos;
		}
		
		private void Awake()
		{
			_currentPointsIndex = 0;
			if (_positions.Count > 1)
			{
				_currentPointsIndex = 1;
			}
		}

		private void FixedUpdate()
		{
			var deltaTime = Time.fixedDeltaTime;

			if (_waitTimer > 0f)
			{
				_waitTimer -= deltaTime;
				return;
			}

			if (!_moveOnlyAfterGrabbing || _moveOnlyAfterGrabbing && OnceGrabbed)
			{
				var needToChangeIIndex = false;
				var pos = _positions[_currentPointsIndex].position;
				var dif = pos - _rigidbody.position;
				dif.z = 0f;
				var distance = dif.magnitude;
				var nextDif = _speed * deltaTime;
				if (distance < nextDif)
				{
					nextDif = distance;
					needToChangeIIndex = true;
				}

				_rigidbody.MovePosition(_rigidbody.position + dif.normalized * nextDif);

				if (needToChangeIIndex)
				{
					ChangePointsIndex();
				}
			}
		}

		private void ChangePointsIndex()
		{
			_currentPointsIndex++;
			if (_currentPointsIndex >= _positions.Count)
			{
				if (_moveOnlyOneWay)
					_currentPointsIndex = _positions.Count - 1;
				else
					_currentPointsIndex = 0;
				
			}

			_waitTimer = _waitTime;
		}

		private void OnDrawGizmosSelected()
		{
			if (_positions != null && _positions.Count > 0)
			{
				Gizmos.color = Color.yellow;
				foreach (var position in _positions)
				{
					Gizmos.DrawSphere(position.position, 0.1f);
				}
			}
		}
	}
}