using _Content.Events;
using _Content.InGame.Managers;
using Common.UI;
using Opsive.Shared.Events;
using UnityEngine;

namespace _Content.InGame.UI.Shop
{
	public class SkinShopButtonUi: UIViewWrapper
	{
		[SerializeField] private CanvasGroup _hasSkinToUnlockIcon;
		private void OnEnable()
		{
			EventHandler.RegisterEvent(InGameEvents.CoinsChanged, OnCoinsChanged);
			UpdateCanBuySkinIcon();
		}

		private void OnDisable()
		{
			EventHandler.UnregisterEvent(InGameEvents.CoinsChanged, OnCoinsChanged);
		}

		private void OnCoinsChanged()
		{
			UpdateCanBuySkinIcon();
		}

		private void UpdateCanBuySkinIcon()
		{
			var show = GameManager.Instance.HasSkinToBuy();
			_hasSkinToUnlockIcon.alpha = show ? 1f : 0f;
		}

		public void ShowShop()
		{
			GameManager.Instance.ShowSkinShop();
		}

        public void ShowGift()
        {
			GameManager.Instance.ShowChestUI();
        }
    }
}