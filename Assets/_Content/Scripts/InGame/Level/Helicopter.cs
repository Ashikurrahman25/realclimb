using DG.Tweening;
using MoreMountains.Feedbacks;
using NaughtyAttributes;
using UnityEngine;

namespace _Content.InGame.Level
{
	public class Helicopter: MonoBehaviour
	{
		[SerializeField] private Transform _mainRotor;
		[SerializeField] private Transform _ladder;
		[SerializeField] private float _angleToGo;
		[SerializeField] private float _speed;
		[SerializeField] private float _floatingSpeed;
		[SerializeField] private Vector2 _floatingInitialPositionRange;
		[SerializeField] private MMF_Player _onWinFeedback;
		private Rigidbody _rigidbody;
		private Vector3 _initialPosition;
		private Vector3 _targetPosition;
		private bool _goAway;

		private void Start()
		{_onWinFeedback?.Initialization(gameObject);
			_rigidbody = GetComponent<Rigidbody>();
			_ladder.parent = null;
			_mainRotor.DORotate(new Vector3(0f, -360f, 0f), .5f, RotateMode.FastBeyond360)
				.SetEase(Ease.Linear)
				.SetLoops(-1, LoopType.Incremental);

			_initialPosition = _rigidbody.position;
			FindNewPosition();
		}

		private void FixedUpdate()
		{
			if (_goAway)
				return;
			var dif = _floatingSpeed * Time.fixedDeltaTime;
			var dir = _targetPosition - _rigidbody.position;
			dir.z = 0f;
			dir.Normalize();
			var distance = Vector3.Distance(_rigidbody.position, _targetPosition);
			if (distance > dif && dir != Vector3.zero)
			{
				_rigidbody.MovePosition(_rigidbody.position + dir * dif);
			}
			else
			{
				_rigidbody.MovePosition(_rigidbody.position + dir * distance);
				FindNewPosition();
			}
		}

		private void FindNewPosition()
		{
			_targetPosition = _initialPosition + (Vector3)Random.insideUnitCircle.normalized * Random.Range(_floatingInitialPositionRange.x, _floatingInitialPositionRange.y);
		}


		[Button()]
		public void GoAway()
		{
			_goAway = true;
			_rigidbody.DOKill();
			//_rigidbody.velocity = new Vector3(1f, 1f, 0f) * _speed;
			_rigidbody.DOMove(transform.position + new Vector3(1f, 1f, 0f) * 100f, _speed)
				.SetSpeedBased(true);
			_rigidbody.DORotate(new Vector3(0f, 0f, _angleToGo), 0.5f, RotateMode.FastBeyond360);
			_onWinFeedback.PlayFeedbacks();
		}
	}
}