using System;
using System.Collections;
using _Content.Data;
using _Content.InGame.UI.Misc;
using Common.UI;
using Doozy.Engine.Touchy;
//using Facebook.Unity;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace _Content.InGame.Managers
{
	public class Bootstrapper: MonoBehaviour
	{
		[SerializeField] private UIViewWrapper _loading;
		private void Awake()
		{
			Application.targetFrameRate = 60;
			
			//if (!FB.IsInitialized)
			//{
			//	// Initialize the Facebook SDK
			//	FB.Init(InitCallback, OnHideUnity);
			//}
			//else
			//{
			//	// Already initialized, signal an app activation App Event
			//	FB.ActivateApp();
			//}
			
			PlayerData.Instance.LastTimeBeforeStart = PlayerData.Instance.LastSavedTime;
			PlayerData.Instance.StartTime = DateTime.UtcNow;
		}
		
		private void OnApplicationPause(bool pauseStatus)
		{
			if (!pauseStatus)
			{
				//app resume
				//if (FB.IsInitialized)
				//{
				//	FB.ActivateApp();
				//}
				//else
				//{
				//	FB.Init(() => { FB.ActivateApp(); });
				//}

				TouchDetector.Instance.DetectTouch();
			}
		}

		private void OnApplicationFocus(bool focusStatus)
		{
			if (focusStatus)
				SetSettings();
		}

		private void InitCallback()
		{
			//if (FB.IsInitialized)
			//	FB.ActivateApp();
			//else
			//	Debug.Log("Failed to Initialize the Facebook SDK");
		}

		private void OnHideUnity(bool isGameShown)
		{
			if (!isGameShown)
			{
				// Pause the game - we will need to hide
				Time.timeScale = 0;
			}
			else
			{
				// Resume the game - we're getting focus again
				Time.timeScale = 1;
			}
		}
		
		private void OnEnable()
		{
			SetRenderScale();
			PrivacyAndTermsAccepted(false);
			//AppLovinManager.Instance.InitializeAds(ShowPrivacyAndTerms, ContinueWithoutGdpr);
			if (_loading != null)
				_loading.ShowView(true);
		}

		private void SetRenderScale()
		{
			var pipelineAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
			if (pipelineAsset != null)
			{
				pipelineAsset.renderScale = 1f;
			}

			//DynamicResolutionScaling.Instance.RecalcRenderScaleAfterTime(2f);
		}
		
		public void PrivacyAndTermsAccepted(bool buttonClicked)
		{
			if (buttonClicked && !PlayerData.Instance.PrivacyAndTermsAccepted)
			{
				PlayerData.Instance.PrivacyAndTermsAccepted = true;
				PlayerData.Instance.Save();
			}
			
			StartCoroutine(StartGame());
		}

		private IEnumerator StartGame()
		{
			yield return new WaitForSeconds(0.1f);
			SetSettings();
			yield return new WaitForSeconds(0.4f);
			GameManager.Instance.StartGame();
		}

		private void SetSettings()
		{
			SettingsUi.SetMusicToManagers(PlayerData.Instance.MusicOn);
			SettingsUi.SetSoundToManagers(PlayerData.Instance.SoundOn);
			SettingsUi.SetVibrationToManagers(PlayerData.Instance.VibrationOn, false);
		}

		public static void ShowPrivacy()
		{
			Application.OpenURL("https://devgame.me/policy");
		}
	}
}