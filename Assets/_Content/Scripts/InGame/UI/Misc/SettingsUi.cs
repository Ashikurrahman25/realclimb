using _Content.Data;
using _Content.InGame.Managers;
using Common.UI;
using Doozy.Engine.Soundy;
using Lofelt.NiceVibrations;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Content.InGame.UI.Misc
{
	public class SettingsUi:UIViewWrapper
	{
		[SerializeField] private TextMeshProUGUI _soundText;
		[SerializeField] private TextMeshProUGUI _musicText;
		[SerializeField] private TextMeshProUGUI _vibrationText;
		[SerializeField] private Image _soundBackground;
		[SerializeField] private Image _musicBackground;
		[SerializeField] private Image _vibrationBackground;
		[SerializeField] private Color _enableColor;
		[SerializeField] private Color _disableColor;

		public override void ShowView(bool force = false)
		{
			base.ShowView(force);
			SetSound(PlayerData.Instance.SoundOn);
			SetMusic(PlayerData.Instance.MusicOn);
			SetVibration(PlayerData.Instance.VibrationOn, false);
		}

		public void ShowPrivacyPolicy()
		{
			Application.OpenURL("https://climbmaster.blogspot.com/p/privacy-policy.html");
		}

        public void ShowTermsConditiony()
        {
            Application.OpenURL("https://climbmaster.blogspot.com/p/terms-conditions.html");
        }

        public void OnCloseButtonClick()
		{
			GameManager.Instance.HideSettings();
		}

		public void ToggleVibration()
		{
			PlayerData.Instance.VibrationOn = !PlayerData.Instance.VibrationOn;
			PlayerData.Instance.Save();

			SetVibration(PlayerData.Instance.VibrationOn);
			/*AppmetricaEvents.SendSettingsEvent(PlayerData.Instance.MusicOn, PlayerData.Instance.SoundOn,
				PlayerData.Instance.VibrationOn);*/
		}

		public void ToggleSound()
		{
			PlayerData.Instance.SoundOn = !PlayerData.Instance.SoundOn;
			PlayerData.Instance.Save();

			SetSound(PlayerData.Instance.SoundOn);
			/*AppmetricaEvents.SendSettingsEvent(PlayerData.Instance.MusicOn, PlayerData.Instance.SoundOn,
				PlayerData.Instance.VibrationOn);*/
		}

		public void ToggleMusic()
		{
			PlayerData.Instance.MusicOn = !PlayerData.Instance.MusicOn;
			PlayerData.Instance.Save();

			SetMusic(PlayerData.Instance.MusicOn);
			/*AppmetricaEvents.SendSettingsEvent(PlayerData.Instance.MusicOn, PlayerData.Instance.SoundOn,
				PlayerData.Instance.VibrationOn);*/
		}

		public void SetMusic(bool on)
		{
			_musicText.text = $"Music: {(on ? "ON" : "OFF")}";
			_musicBackground.color = on ? _enableColor : _disableColor;
			SetMusicToManagers(on);
		}

		public static void SetMusicToManagers(bool on)
		{
			if (on)
			{
				//MasterAudio.UnmuteBus(SoundNames.Bus_Music);
				MMSoundManager.Instance.UnmuteMusic();
				MMSoundManager.Instance.SetVolumeMusic(1f);
			}
			else
			{
				//MasterAudio.MuteBus(SoundNames.Bus_Music);
				MMSoundManager.Instance.MuteMusic();
			}
		}

		public void SetSound(bool on)
		{
			_soundText.text = $"Sound: {(on ? "ON" : "OFF")}";
			_soundBackground.color = on ? _enableColor : _disableColor;
			SetSoundToManagers(on);
		}

		public static void SetSoundToManagers(bool on)
		{
			if (on)
			{
				//MasterAudio.UnmuteBus(SoundNames.Bus_Effects);
				SoundyManager.UnmuteAllControllers();
				MMSoundManager.Instance.UnmuteSfx();
				MMSoundManager.Instance.SetVolumeSfx(1f);
			}
			else
			{
				//MasterAudio.MuteBus(SoundNames.Bus_Effects);
				//MasterAudio.MuteBus(SoundNames.Bus_Music);
				SoundyManager.MuteAllControllers();
				MMSoundManager.Instance.MuteSfx();
			}
		}

		public void SetVibration(bool on, bool play = true)
		{
			_vibrationText.text = $"Vibration: {(on ? "ON" : "OFF")}";
			_vibrationBackground.color = on ? _enableColor : _disableColor;
			SetVibrationToManagers(on, play);
		}

		public static void SetVibrationToManagers(bool on, bool play = true)
		{
			HapticController.hapticsEnabled = on /*&& DeviceCapabilities.isVersionSupported*/;
			//MMVibrationManager.SetHapticsActive(on);
			if (on && play)
				HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
		}
	}
}