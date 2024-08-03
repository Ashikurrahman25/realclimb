using _Content.Data;
using _Content.InGame.Manager;
using _Content.InGame.Managers;
using Common.UI;
using DG.Tweening;
using Doozy.Engine.UI;
using TMPro;
using UnityEngine;

namespace _Content.InGame.UI.Main
{
	public class WinUi : UIViewWrapper
	{
		[SerializeField] private UIButton _claimButton;
        [SerializeField] private UIButton _claim2XButton;

        [SerializeField] private TextMeshProUGUI _coinsCountText;
		[SerializeField] private GameObject _keysObject;
		[SerializeField] private RectTransform _particlsStartPosition;
		[SerializeField] private RectTransform _keyParticlesStartPosition;
		private int _reward;

		public override void ShowView(bool force = false)
		{
			base.ShowView(force);
			UpdateInfo();
		}

		private void UpdateInfo()
		{
			_reward = GameplayManager.Instance.GetRewardForLevel();
			_coinsCountText.text = $"+{_reward}";
			_keysObject.SetActive(GameplayManager.Instance.CurrentLevelKeys > 0);
			_claimButton.EnableButton();
            _claim2XButton.EnableButton();

        }

		public void OnClaimButton()
		{
            _claimButton.DisableButton();
            UiParticleSystem.Instance.StartCoinParticles(_particlsStartPosition.position,
                UIManager.Instance.MoneyEndPoint.position);
            DOVirtual.DelayedCall(1f, () => { GameplayManager.Instance.AddCoinsToData(_reward); });
            if (GameplayManager.Instance.CurrentLevelKeys > 0)
            {
                UiParticleSystem.Instance.StartKeyParticle(_keyParticlesStartPosition.position,
                    UIManager.Instance.KeyEndPosition.position);
                DOVirtual.DelayedCall(1f, () => { GameplayManager.Instance.AddKeyToSave(); });
            }

            if (PlayerData.Instance.CurrentLevelsToPlay.Count == 1)
            {
                DOVirtual.DelayedCall(2f, () => { GameManager.Instance.ShowBonusGame(); });
            }
            else
            {
                DOVirtual.DelayedCall(2f, () => { GameManager.Instance.RestartAfterWinning(); });
            }
          
		}

        public void OnClaim2XButton()
        {
            AdsController.instance.ShowRewardedAds(() =>
            {
                _claim2XButton.DisableButton();
                UiParticleSystem.Instance.StartCoinParticles(_particlsStartPosition.position,
                    UIManager.Instance.MoneyEndPoint.position);
                DOVirtual.DelayedCall(1f, () => { GameplayManager.Instance.AddCoinsToData(_reward * 2); });
                if (GameplayManager.Instance.CurrentLevelKeys > 0)
                {
                    UiParticleSystem.Instance.StartKeyParticle(_keyParticlesStartPosition.position,
                        UIManager.Instance.KeyEndPosition.position);
                    DOVirtual.DelayedCall(1f, () => { GameplayManager.Instance.AddKeyToSave(); });
                }

                if (PlayerData.Instance.CurrentLevelsToPlay.Count == 1)
                {
                    DOVirtual.DelayedCall(2f, () => { GameManager.Instance.ShowBonusGame(); });
                }
                else
                {
                    DOVirtual.DelayedCall(2f, () => { GameManager.Instance.RestartAfterWinning(); });
                }
            });

        }
    }
}