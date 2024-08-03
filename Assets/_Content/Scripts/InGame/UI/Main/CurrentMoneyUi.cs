using System;
using _Content.Data;
using _Content.Events;
using Common.UI;
using DG.Tweening;
using TMPro;
using UnityEngine;
using EventHandler = Opsive.Shared.Events.EventHandler;

namespace _Content.InGame.UI.Main
{
	public class CurrentMoneyUi : UIViewWrapper
	{
		[SerializeField] private TextMeshProUGUI _coins;
		private int _currentCoins;
		private Tweener _coinsTween;


		private void OnEnable()
		{
			EventHandler.RegisterEvent(InGameEvents.CoinsChanged, OnCoinsChanged);
			UpdateInfo();
		}

		private void OnDisable()
		{
			EventHandler.UnregisterEvent(InGameEvents.CoinsChanged, OnCoinsChanged);
		}

		private void OnCoinsChanged()
		{
			UpdateInfo();
		}

		public override void ShowView(bool force = false)
		{
			base.ShowView(force);
			_coins.text = $"{PlayerData.Instance.CoinsCount}";
			UpdateInfo();
		}

		private void UpdateInfo()
		{
			var targetCoins = PlayerData.Instance.CoinsCount;
			if (_currentCoins != targetCoins)
			{
				var start = _currentCoins;
				if (_coinsTween != null && _coinsTween.IsActive())
					_coinsTween.Kill();

				_coinsTween = DOVirtual.Int(start, targetCoins, 1f, (x) =>
				{
					_currentCoins = x;
					_coins.text = $"{_currentCoins}";
				});
			}
		}
	}
}