using _Content.InGame.Managers;
using DG.Tweening;
using Doozy.Engine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Content.InGame.UI.Main
{
	public class BonusChestUi : MonoBehaviour
	{
		[SerializeField] private Transform _body;
		[SerializeField] private UIButton _button;
		[SerializeField] private Image _rewardImage;
		[SerializeField] private TextMeshProUGUI _rewardCountText;

		private int _rewardCount;
		private BonusGameUi _bonusGame;
		private bool _opened;
		private GameplayManager.BonusRewardType _rewardType;

		public GameplayManager.BonusRewardType RewardType => _rewardType;
		public int RewardCount => _rewardCount;

		public bool Opened => _opened;

		private void Awake()
		{
			_bonusGame = GetComponentInParent<BonusGameUi>();
		}

		public void Initialize(GameplayManager.BonusRewardType rewardRewardType, Sprite rewardIcon, int rewardCount)
		{
			_opened = false;
			_body.localScale = Vector3.one;
			_button.EnableButton();
			_rewardType = rewardRewardType;
			_rewardImage.sprite = rewardIcon;
			_rewardCount = rewardCount;
			_rewardCountText.text = $"{rewardCount}";
		}

		public void OnButtonClick()
		{
			_button.DisableButton();
			_bonusGame.OpenChest(this);
		}

		public void Open()
		{
			_body.DOScale(Vector3.zero, .4f)
				.SetEase(Ease.InBack);
			_opened = true;
		}
	}
}