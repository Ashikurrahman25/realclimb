using System.Collections;
using _Content.InGame.Managers;
using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace _Content.InGame.Collectables
{
	public class Key: MonoBehaviour
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
			transform.rotation = Quaternion.Euler(0f, 0f, -10f);
			transform.DORotate(new Vector3(0f, 0f, 10f), _rotateTime, RotateMode.Fast)
				.SetLoops(-1, LoopType.Yoyo);
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
			GameplayManager.Instance.AddKey(1);
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