using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Base;
using DG.Tweening;
using UnityEngine;

namespace _Content.InGame.Manager
{
	public class UiParticleSystem : Singleton<UiParticleSystem>
	{
		[SerializeField] private RectTransform _partentRectTransform;
		[SerializeField] private int _particlesCount;
		private List<RectTransform> _coinParticles;
		private List<RectTransform> _keyParticles;
		[SerializeField] private RectTransform _coinParticleUIPrefabs;
		[SerializeField] private RectTransform _keyParticleUIPrefabs;
		[SerializeField] private AnimationCurve _scaleCurve;
		[SerializeField] private float _moveTime;
		private int _rewardToAdd;

		private void Awake()
		{
			_keyParticles = new List<RectTransform>();
			for (int i = 0; i < 10; i++)
			{
				var res = Instantiate(_keyParticleUIPrefabs, _partentRectTransform);
				res.gameObject.SetActive(false);
				_keyParticles.Add(res);
			}
			
			_coinParticles = new List<RectTransform>();
			for (int i = 0; i < _particlesCount * 3; i++)
			{
				var res = Instantiate(_coinParticleUIPrefabs, _partentRectTransform);
				res.gameObject.SetActive(false);
				_coinParticles.Add(res);
			}
		}

		public void StartKeyParticle(Vector2 startPoint, Vector2 endPoint)
		{
			StartCoroutine(StartCoinParticleCorutine(_keyParticles, startPoint, endPoint, 1));
		}

		public void StartCoinParticles(Vector2 startPoint, Vector2 endPoint)
		{
			StartCoroutine(StartCoinParticleCorutine(_coinParticles, startPoint, endPoint, _particlesCount));
		}
		public void StartCoinParticles(Vector2 startPoint, Vector2 endPoint, int count)
		{
			StartCoroutine(StartCoinParticleCorutine(_coinParticles, startPoint, endPoint, count));
		}

		private IEnumerator StartCoinParticleCorutine(List<RectTransform> pool, Vector2 startPoint, Vector2 endPoint, int count)
		{
			var availableParticles = pool.Where(p => !p.gameObject.activeSelf).ToList();
			for (int i = 0; i < count; i++)
			{
				if (i < availableParticles.Count)
				{
					var coin = availableParticles[i];
					StartParticle(coin, startPoint, endPoint);
					yield return new WaitForSeconds(0.05f);
				}
			}
		}

		private void StartParticle(RectTransform coin, Vector2 startPoint, Vector2 endPosition)
		{
			coin.position = startPoint;
			var coinStartPos = startPoint +
			                   new Vector2(Random.Range(-1f, 1f), Random.Range(0f, 1f)) * (Screen.width / 8f);

			coin.gameObject.SetActive(true);
			coin.localScale = Vector3.zero;
			var seqBefore = DOTween.Sequence();
			seqBefore.Append(coin.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack))
				.Insert(0f, coin.DOMove(coinStartPos, 0.2f).SetEase(Ease.InOutCirc))
				.OnComplete(() =>
				{
					var duration = Random.Range(_moveTime, _moveTime + 0.3f);
					var seq = DOTween.Sequence();
					seq.Append(coin.DOMove(endPosition, duration).SetEase(Ease.InBack))
						.Insert(0f, coin.DOScale(Vector3.zero, duration).SetEase(_scaleCurve))
						.OnComplete(() => CoinAtTheEndPosition(coin));
				});
		}

		private void CoinAtTheEndPosition(RectTransform coin)
		{
			if (_rewardToAdd > 0)
			{
				/*PlayerData.Instance.Money += _rewardToAdd;
				PlayerData.Instance.Save();
				EventHandler.ExecuteEvent(InGameEvents.OnCoinsUIUpdated);*/
				_rewardToAdd = 0;
			}

			coin.gameObject.SetActive(false);
		}
	}
}