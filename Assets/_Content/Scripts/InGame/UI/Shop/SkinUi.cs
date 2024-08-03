using System.Linq;
using _Content.Data;
using _Content.InGame.Characters;
using DG.Tweening;
using Doozy.Engine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Content.InGame.UI.Shop
{
	public class SkinUi : MonoBehaviour
	{
		[SerializeField] private Image _skinIcon;
		[SerializeField] private TextMeshProUGUI _costText;
		[SerializeField] private Image _buyButtonBackground;
		[SerializeField] private UIButton _buyButton;
		[SerializeField] private UIButton _chooseButton;
		[SerializeField] private Color _enableBuyColor;
		[SerializeField] private Color _disableBuyColor;
		[SerializeField] private Image _iconBackground;
		[SerializeField] private Color _selectedColor;
		[SerializeField] private Color _unselectedColor;

		private string _skinId;
		private int _cost;
		private SkinsShopUi _shopUi;

		public string SkinId => _skinId;
		public int Cost => _cost;

		public void Initialize(SkinsShopUi shopUi, Skins.Skin skin)
		{
			_shopUi = shopUi;
			_skinId = skin.SkinId;
			_cost = skin.Cost;
			_skinIcon.sprite = skin.Icon;
			_costText.text = $"{_cost}";
			_iconBackground.color = _unselectedColor;
			_chooseButton.DisableButton();
		}

		public void UpdateInfo()
		{
			if (string.IsNullOrEmpty(_skinId))
				return;
			
			if (PlayerData.Instance.UnlockedSkins.Any(s => s == _skinId))
			{
				_buyButton.gameObject.SetActive(false);
				_chooseButton.EnableButton();
			}
			else
			{
				_buyButton.gameObject.SetActive(true);
				_chooseButton.DisableButton();
			}

			if (PlayerData.Instance.CoinsCount >= _cost)
			{
				_buyButton.EnableButton();
				_buyButtonBackground.color = _enableBuyColor;
			}
			else
			{
				_buyButton.DisableButton();
				_buyButtonBackground.color = _disableBuyColor;
			}
		}

		public void OnChooseButton()
		{
			_shopUi.OnChooseSkin(this);
		}

		public void OnBuyButton()
		{
			_shopUi.BuySkin(this);
		}

		public void ResetSelection()
		{
			_iconBackground.color = _unselectedColor;
			transform.DOScale(Vector3.one, 0.3f);
		}

		public void Select(bool force)
		{
			_iconBackground.color = _selectedColor;
			if (!force)
			{
				transform.DOScale(Vector3.one * 1.1f, 0.3f);
			}
			else
			{
				transform.localScale = Vector3.one * 1.1f;
			}
		}
	}
}