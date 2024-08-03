using System;
using _Content.Appmetricas;
using _Content.Data;
using _Content.InGame.Managers;
using Common.UI;
using Doozy.Engine.UI;
using UnityEngine;
using UnityEngine.UI;

namespace _Content.InGame.UI.Misc
{
	public class RateController : UIViewWrapper
	{
		[SerializeField] private Color _disableColor;
		[SerializeField] private Image _rateButtonBackground;
		[SerializeField] private UIButton _rateUIButton;
		[SerializeField] private RateStar _firstStar;
		[SerializeField] private RateStar _secondStar;
		[SerializeField] private RateStar _thirdStar;
		[SerializeField] private RateStar _forthStar;
		[SerializeField] private RateStar _fifthStar;
		private int _lastRating;
		private Color _enabledBackgroundColor;
		private bool _rated;

		private void Awake()
		{
			_enabledBackgroundColor = _rateButtonBackground.color;
		}

		public void TryShowRateView()
		{
			if (!PlayerData.Instance.Rated && PlayerData.Instance.RateUsAttempts < 3 &&
			    PlayerData.Instance.LevelNumber > 3 && Time.unscaledTime > 60f)
			{
				var minutesFromLastRateUs = (DateTime.UtcNow - PlayerData.Instance.RateUsLastTime).TotalMinutes;
				var neededMinutes = PlayerData.Instance.RateUsAttempts < 1 ? 0 : 1440;
				if ((PlayerData.Instance.RateUsAttempts < 1) ||
				    minutesFromLastRateUs >= neededMinutes)
				{
#if UNITY_IOS
					Device.RequestStoreReview();
					PlayerData.Instance.RateUsAttempts = 3;
					PlayerData.Instance.Save();
#else
					GameManager.Instance.ShowRateView();
					//ShowView();
#endif
				}
			}
		}

		private void Update()
		{
			if (!_shown)
				return;

			if (_lastRating == 0)
			{
				_rateButtonBackground.color = _disableColor;
				_rateUIButton.DisableButton();
			}
			else
			{
				_rateButtonBackground.color = _enabledBackgroundColor;
				_rateUIButton.EnableButton();
			}
		}

		public override void ShowView(bool force = true)
		{
			base.ShowView(force);
			_firstStar.Initialize(this);
			_secondStar.Initialize(this);
			_thirdStar.Initialize(this);
			_forthStar.Initialize(this);
			_fifthStar.Initialize(this);

			PlayerData.Instance.RateUsLastTime = DateTime.UtcNow;
			PlayerData.Instance.RateUsAttempts++;
			PlayerData.Instance.Save();
			Time.timeScale = 0f;
			_lastRating = 0;
			_rated = false;
		}

		public void OnClickLater()
		{
			if (_rated)
				return;
			GameManager.Instance.HideRateView();
			//HideView();
			Time.timeScale = 1f;
			AppMetricaEvents.SendRateUs(0);
		}

		public void OnClickRate()
		{
			if (_rated)
				return;
			
			PlayerData.Instance.Rated = true;
			PlayerData.Instance.RateUsLastTime = DateTime.UtcNow;
			PlayerData.Instance.Save();
			AppMetricaEvents.SendRateUs(_lastRating);
			Time.timeScale = 1f;

			if (_lastRating == 5)
			{
#if UNITY_ANDROID
				Application.OpenURL("market://details?id=" + Application.identifier);
#elif UNITY_IOS
				Application.OpenURL("itms-apps://itunes.apple.com/app/idYOUR_ID");
#endif
			}

			_rated = true;
			GameManager.Instance.HideRateView();
			//HideView();
		}

		public void SetRating(int rating)
		{
			_firstStar.SetFilling(false);
			_secondStar.SetFilling(false);
			_thirdStar.SetFilling(false);
			_forthStar.SetFilling(false);
			_fifthStar.SetFilling(false);

			_lastRating = rating;

			if (rating > 0)
			{
				_firstStar.SetFilling(true);
			}

			if (rating > 1)
			{
				_secondStar.SetFilling(true);
			}

			if (rating > 2)
			{
				_thirdStar.SetFilling(true);
			}

			if (rating > 3)
			{
				_forthStar.SetFilling(true);
			}

			if (rating > 4)
			{
				_fifthStar.SetFilling(true);
			}
		}
	}
}