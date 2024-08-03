using _Content.InGame.UI.Main;
using _Content.InGame.UI.Misc;
using _Content.InGame.UI.Shop;
using Base;
using Common.UI;
using UnityEngine;

namespace _Content.InGame.Managers
{
	public class UIManager : Singleton<UIManager>
	{
		[SerializeField] private RectTransform _moneyEndPoint;
		[SerializeField] private RectTransform _keyEndPosition;
		[SerializeField] private PlayersInputUi _playersInputUi;
		[SerializeField] private CheatsUi _cheatsUi;
		[SerializeField] private SettingsButton _settingsButton;
		[SerializeField] private SettingsUi _settingsUi;
		[SerializeField] private UIViewWrapper _loadingUi;
		[SerializeField] private CurrentMoneyUi _currentMoneyUi;
		[SerializeField] private WinUi _winUi;
		[SerializeField] private FailedUi _failedUi;
		[SerializeField] private LevelProgressionUi _levelProgressionUi;
		[SerializeField] private BonusGameUi _bonusGameUi;
		[SerializeField] private UIViewWrapper _movingTutorial;
		[SerializeField] private SkinsShopUi _skinsShopUi;
		[SerializeField] private ChestUi _chestUi;
		[SerializeField] private SkinShopButtonUi _skinShopButtonUi;
		[SerializeField] private RateController _rateController;

		public RateController RateController => _rateController;
		public SkinsShopUi SkinsShopUi => _skinsShopUi;
		public ChestUi ChestUI => _chestUi;
		public SkinShopButtonUi SkinShopButtonUi => _skinShopButtonUi;
		public UIViewWrapper MovingTutorial => _movingTutorial;
		public BonusGameUi BonusGameUi => _bonusGameUi;
		public LevelProgressionUi LevelProgressionUi => _levelProgressionUi;
		public WinUi WinUi => _winUi;
		public CurrentMoneyUi CurrentMoneyUi => _currentMoneyUi;
		public FailedUi FailedUi => _failedUi;
		public UIViewWrapper LoadingUi => _loadingUi;
		public PlayersInputUi PlayersInputUi => _playersInputUi;

		public CheatsUi CheatsUi => _cheatsUi;
		public SettingsButton SettingsButton => _settingsButton;
		public SettingsUi SettingsUi => _settingsUi;
		public RectTransform MoneyEndPoint => _moneyEndPoint;
		public RectTransform KeyEndPosition=> _keyEndPosition;
	}
}