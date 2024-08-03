using System;
using System.Collections.Generic;
using _Content.Data;
using _Content.Events;
using _Content.InGame.Manager;
using _Content.InGame.Managers;
using Common.UI;
using DG.Tweening;
using Doozy.Engine.UI;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using EventHandler = Opsive.Shared.Events.EventHandler;

namespace _Content.InGame.UI.Main
{
	public class BonusGameUi : UIViewWrapper
	{
		[SerializeField] private TextMeshProUGUI _keysCount;
		[SerializeField] private List<BonusChestUi> _bonusChestUis;
		[SerializeField] private UIButton _claimButton;
        [SerializeField] private UIButton _claim2xButton;

        [SerializeField] private MMF_Player _onChestOpen;
		[SerializeField] private MMF_Player _onClickFeedback;
		[Header("Keys")] [SerializeField] private Transform _startKeyPoint;

		protected override void OnAwake()
		{
			base.OnAwake();
			_onChestOpen?.Initialization(gameObject);
			_onClickFeedback?.Initialization(gameObject);
		}

		private void OnEnable()
		{
			EventHandler.RegisterEvent(InGameEvents.KeysChanged, OnKeysChanged);
			OnKeysChanged();
		}

		private void OnDisable()
		{
			EventHandler.UnregisterEvent(InGameEvents.KeysChanged, OnKeysChanged);
		}

		private void OnKeysChanged()
		{
			_keysCount.text = $"{PlayerData.Instance.KeysCount}";
		}

		public override void ShowView(bool force = false)
		{
			base.ShowView(force);
			PlayerData.Instance.AddKeys(2);
			_claimButton.gameObject.SetActive(false);
			_claim2xButton.gameObject.SetActive(false);
			_claimButton.EnableButton();
			_claim2xButton.EnableButton();

            OnKeysChanged();
			var rewards = GameplayManager.Instance.GetRewardsForBonusGame();
			foreach (var chest in _bonusChestUis)
			{
				var reward = rewards.PopRandomElement();
				chest.Initialize(reward.RewardType, reward.Icon, reward.Count);
			}
		}

		public void OpenChest(BonusChestUi bonusChestUi)
		{
			if (PlayerData.Instance.KeysCount <= 0)
				return;
			_onClickFeedback?.PlayFeedbacks();
			PlayerData.Instance.AddKeys(-1);
			UiParticleSystem.Instance.StartKeyParticle(_startKeyPoint.position, bonusChestUi.transform.position);
			DOVirtual.DelayedCall(1f, () =>
			{
				_onChestOpen?.PlayFeedbacks();
				ChestOpen(bonusChestUi);
			});
		}

		private void ChestOpen(BonusChestUi bonusChestUi)
		{
			bonusChestUi.Open();
			if (PlayerData.Instance.KeysCount <= 0)
			{
				_claimButton.gameObject.SetActive(true);
				_claim2xButton.gameObject.SetActive(true);
			}
		}

		public void OnClaimButton()
		{
			_claimButton.DisableButton();
			foreach (var bonusChestUi in _bonusChestUis)
			{
				if (bonusChestUi.Opened)
				{
					if (bonusChestUi.RewardType == GameplayManager.BonusRewardType.Coin)
					{
						UiParticleSystem.Instance.StartCoinParticles(bonusChestUi.transform.position,
							UIManager.Instance.MoneyEndPoint.position, 5);
						DOVirtual.DelayedCall(1f, () => GameplayManager.Instance.AddCoinsToData(bonusChestUi.RewardCount));
					}
				}
			}

			DOVirtual.DelayedCall(2f, () => GameManager.Instance.RestartAfterWinning());
		}

		public void Claim2x()
		{
            _claimButton.DisableButton();

			AdsController.instance.ShowRewardedAds(() =>
			{
				foreach (var bonusChestUi in _bonusChestUis)
				{
					if (bonusChestUi.Opened)
					{
						if (bonusChestUi.RewardType == GameplayManager.BonusRewardType.Coin)
						{
							UiParticleSystem.Instance.StartCoinParticles(bonusChestUi.transform.position,
								UIManager.Instance.MoneyEndPoint.position, 5);
							DOVirtual.DelayedCall(1f, () => GameplayManager.Instance.AddCoinsToData(bonusChestUi.RewardCount*5));
						}
					}
				}

				DOVirtual.DelayedCall(2f, () => GameManager.Instance.RestartAfterWinning());
			});
           
        }
	}
}