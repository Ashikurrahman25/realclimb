using System;
using System.Collections;
using _Content.InGame.Managers;
using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace _Content.InGame.Collectables
{
	public class Coin : MonoBehaviour
	{
		[SerializeField] private MMF_Player _onCollectFeedback;
		[SerializeField] private float _rotateTime = 2f;
		[SerializeField] private LayerMask _collectLayers;
		private bool _collected;

		private void Awake()
		{
			_onCollectFeedback?.Initialization(gameObject);
		}

		private void OnEnable()
		{
			transform.DOLocalRotate(new Vector3(0f, 360f, 0f), _rotateTime, RotateMode.FastBeyond360)
				.SetLoops(-1, LoopType.Incremental);
		}

		private void OnDisable()
		{
			transform.DOKill();
		}

		private void OnTriggerEnter(Collider other)
		{
			if (_collected)
				return;

			if (!_collectLayers.Contains(other.gameObject.layer))
				return;

			Collect();
		}

		public void Collect()
		{
			_collected = true;
			GameplayManager.Instance.AddCoinsForLevelReward();
			GameplayManager.Instance.AddCoinsToData(1);
			StartCoroutine(DestroyAfterFrame());
			_onCollectFeedback?.PlayFeedbacks();
		}

		private IEnumerator DestroyAfterFrame()
		{
			yield return null;
			Destroy(gameObject);
		}
	}
}