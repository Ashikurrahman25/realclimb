using System;
using _Content.InGame.Managers;
using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace _Content.InGame.Level
{
	public class PushingObstacle : MonoBehaviour
	{
		[SerializeField] private bool _dontWorkWhenAlreadyGrab;
		[SerializeField] private float _force;
		[SerializeField] private Vector2 _minMaxForce;
		[SerializeField] private bool _directionFromCenter;
		[SerializeField] private AnimationCurve _forceCurve;
		[SerializeField] private LayerMask _hitMask;
		[SerializeField] private Transform _modelTransform;
		[SerializeField] private MMF_Player _onCollisionFeedback;
		private float _lastCollisionTimer;

		private void Awake()
		{
			_onCollisionFeedback?.Initialization(gameObject);
		}

		private void Update()
		{
			if (_lastCollisionTimer > 0f)
				_lastCollisionTimer -= Time.deltaTime;
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (!_hitMask.Contains(collision.gameObject.layer))
				return;
			if (_lastCollisionTimer > 0f)
				return;
			OnCollision(collision);
		}
		
		private void OnCollisionStay(Collision collision)
		{
			if (!_hitMask.Contains(collision.gameObject.layer))
				return;
			if (_lastCollisionTimer > 0f)
				return;
			OnCollision(collision);
		}

		private void OnCollision(Collision collision)
		{
			var puppetController = GameplayManager.Instance.GetPuppetController();
			if (_dontWorkWhenAlreadyGrab && puppetController.IsGrabbing())
				return;
			
			var direction = Vector3.right;
			if (collision.contactCount > 0)
			{
				direction = collision.contacts[0].normal;
			}

			if (_directionFromCenter)
			{
				direction = GameplayManager.Instance.GetPlayersCenterPosition() - transform.position;
			}

			direction.z = 0f;
			direction.Normalize();

			var angle = Mathf.Clamp(Vector3.Angle(Vector3.up, direction), 0f, 90f);
			var force = Mathf.Lerp(_minMaxForce.x, _minMaxForce.y, _forceCurve.Evaluate(angle / 90f));
			puppetController.AddForceFromSource(direction, force);
			_lastCollisionTimer = 0.2f;

			_modelTransform.DOKill();
			_modelTransform.DOScale(Vector3.one * 0.8f, 0.1f)
				.OnComplete(() => _modelTransform.DOScale(Vector3.one, 0.1f));
			
			_onCollisionFeedback?.PlayFeedbacks();
		}
	}
}