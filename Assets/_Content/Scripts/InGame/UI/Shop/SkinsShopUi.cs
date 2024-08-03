using System.Collections.Generic;
using System.Linq;
using _Content.Appmetricas;
using _Content.Data;
using _Content.Events;
using _Content.InGame.Managers;
using Common.UI;
using UnityEngine;
using EventHandler = Opsive.Shared.Events.EventHandler;

namespace _Content.InGame.UI.Shop
{
	public class SkinsShopUi : UIViewWrapper
	{
		[SerializeField] private List<SkinUi> _skinUis;
		private SkinUi _currentSkinUi;

		private void OnEnable()
		{
			EventHandler.RegisterEvent(InGameEvents.CoinsChanged, OnCoinsChanged);
		}

		private void OnDisable()
		{
			EventHandler.UnregisterEvent(InGameEvents.CoinsChanged, OnCoinsChanged);
		}

		private void OnCoinsChanged()
		{
			UpdateSkinsInformation();
		}

		private void Start()
		{
			Initialize();
		}

		private void Initialize()
		{
			var skins = GameManager.Instance.GetAllSkins();
			for (int i = 0; i < _skinUis.Count; i++)
			{
				var skinUi = _skinUis[i];
				if (i < skins.Count)
				{
					var skin = skins[i];
					skinUi.Initialize(this, skin);
				}
				else
				{
					skinUi.gameObject.SetActive(false);
				}
			}

			var currentSkinId = PlayerData.Instance.CurrentSkinId;
			var currentSkinUi = _skinUis.FirstOrDefault(s => s.SkinId == currentSkinId);
			if (currentSkinUi != null)
			{
				OnChooseSkin(currentSkinUi, true);
			}

			UpdateSkinsInformation();
		}

		public override void ShowView(bool force = false)
		{
			base.ShowView(force);
			UpdateSkinsInformation();
		}

		private void UpdateSkinsInformation()
		{
			_skinUis.ForEach(s => s.UpdateInfo());
		}

		public void CloseButton()
		{
			GameManager.Instance.HideSkinShop();
		}

		public void BuySkin(SkinUi skinUi)
		{
			var skin = GameManager.Instance.GetSkin(skinUi.SkinId);
			if (PlayerData.Instance.CoinsCount < skin.Cost)
				return;

			PlayerData.Instance.AddCoins(-skin.Cost);
			PlayerData.Instance.UnlockSkin(skinUi.SkinId);
			AppMetricaEvents.SkinUnlock(skinUi.SkinId);
			skinUi.OnChooseButton();		
			UpdateSkinsInformation();
		}

		public void OnChooseSkin(SkinUi skinUi, bool force = false)
		{
			if (_currentSkinUi != null)
			{
				_currentSkinUi.ResetSelection();
			}

			_currentSkinUi = skinUi;
			PlayerData.Instance.CurrentSkinId = skinUi.SkinId;
			PlayerData.Instance.Save();
			EventHandler.ExecuteEvent(InGameEvents.CurrentSkinChanged);
			_currentSkinUi.Select(force);
			
			UpdateSkinsInformation();
		}
	}
}