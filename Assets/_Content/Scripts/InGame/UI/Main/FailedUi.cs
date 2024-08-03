using _Content.InGame.Manager;
using _Content.InGame.Managers;
using Common.UI;
using DG.Tweening;
using Doozy.Engine.UI;
using TMPro;
using UnityEngine;

namespace _Content.InGame.UI.Main
{
	public class FailedUi : UIViewWrapper
	{
		[SerializeField] private UIButton _restartButton;
        [SerializeField] private UIButton _continueButton;

        [SerializeField] private TextMeshProUGUI _rewardText;
		[SerializeField] private RectTransform _particlsStartPosition;
		private int _reward;

		public override void ShowView(bool force = false)
		{
			base.ShowView(force);
			UpdateInfo();
		}

		private void UpdateInfo()
		{
			_reward = 25;
			_restartButton.EnableButton();

			if(GlobalData.continueCount != 0 || !GlobalData.GAME_STARTTED)
				_continueButton.DisableButton();
			else
				_continueButton.EnableButton();

            _rewardText.text = $"+{_reward}";
		}

		public void Restart()
		{
			_restartButton.DisableButton();
			UiParticleSystem.Instance.StartCoinParticles(_particlsStartPosition.position,
				UIManager.Instance.MoneyEndPoint.position);
			DOVirtual.DelayedCall(1f, () => { GameplayManager.Instance.AddCoinsToData(_reward); });
			DOVirtual.DelayedCall(2f, () => { GameManager.Instance.RestartAfterDefeat(); });

			GlobalData.GAME_STARTTED = false;
			GlobalData.continueCount = 0;
		}

        public void Continue()
        {
            _continueButton.DisableButton();
			AdsController.instance.ShowRewardedAds(() =>
			{
                UiParticleSystem.Instance.StartCoinParticles(_particlsStartPosition.position,
                UIManager.Instance.MoneyEndPoint.position);
                DOVirtual.DelayedCall(1f, () => { GameplayManager.Instance.AddCoinsToData(_reward*2); });
                DOVirtual.DelayedCall(2f, () => { GameplayManager.Instance.ContinueGame(); });
               
			});
        }
    }
}