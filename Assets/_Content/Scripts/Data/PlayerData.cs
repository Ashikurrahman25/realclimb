using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using _Content.Events;
using Extensions;
using UnityEngine;
using EventHandler = Opsive.Shared.Events.EventHandler;

namespace _Content.Data
{
	public class PlayerData
	{
		private const string DefaultSkinName = "default";
		private static PlayerData _instance;

		public static PlayerData Instance
		{
			get
			{
				if (_instance == null)
				{
					Create();

					//if we create the PlayerData, mean it's the very first call, so we use that to init the database
					//this allow to always init the database at the earlier we can, i.e. the start screen if started normally on device
					//or the Loadout screen if testing in editor
				}

				return _instance;
			}
		}


		protected string SaveFile = "";
		static int _version = 1;
		public bool PrivacyAndTermsAccepted = false;
		public bool VibrationOn = true;
		public bool SoundOn = true;
		public bool MusicOn = true;
		public int LevelCount;
		public int LevelLoop;
		public int RateUsAttempts = 0;
		public DateTime RateUsLastTime = DateTime.UtcNow;
		public bool Rated = false;
		public int LevelNumber = 1;
		public int TutorialStage = 1;
		public DateTime LastSavedTime;
		public int InterstitialsShown = 0;
		public int CoinsCount = 0;
        public int LevelClaim = 0;
        public int LevelNumberThroughLoops;
		public int KeysCount;
		public int CurrentBiomId = 0;
		public List<int> CurrentLevelsToPlay = new List<int>();
		public List<int> PlayedLevels = new List<int>();
		public bool RandomBioms = false;
		public List<string> UnlockedSkins = new List<string>() { DefaultSkinName };
		public string CurrentSkinId = DefaultSkinName;

		private float _timeToSave;
		private bool _savingInProgress;

		public DateTime StartTime;
		public DateTime LastTimeBeforeStart;
		public bool DisableAds = false;
		public float LastTimeBuiltTileForAd;
		public int LastGameSeconds;
		private float _lastSaveTime;

		public bool InterstitialWithoutOffer { get; set; } = true;
		public bool IAPBuyButtonWasPressed { get; set; }

		public int GetGameSeconds()
		{
			return LastGameSeconds + Mathf.FloorToInt(Time.unscaledTime);
		}

		public static void Create()
		{
			if (_instance == null)
			{
				_instance = new PlayerData();

				//if we create the PlayerData, mean it's the very first call, so we use that to init the database
				//this allow to always init the database at the earlier we can, i.e. the start screen if started normally on device
				//or the Loadout screen if testing in editor
			}

			_instance.SaveFile = Application.persistentDataPath + "/save.bin";
			if (File.Exists(_instance.SaveFile))
			{
				// If we have a save, we read it.

				_instance.Read();
			}
			else
			{
				// If not we create one with default data.
				NewSave();
			}
		}

		public long GetSaveFileLength()
		{
			if (File.Exists(SaveFile))
			{
				var fileInfo = new System.IO.FileInfo(SaveFile);
				return fileInfo.Length;
			}

			return 0;
		}

		public static void NewSave()
		{
			_instance.VibrationOn = true;
			_instance.SoundOn = true;
			_instance.MusicOn = true;
			_instance.PrivacyAndTermsAccepted = false;
			_instance.LevelCount = 0;
			_instance.LevelLoop = 0;
			_instance.Rated = false;
			_instance.RateUsAttempts = 0;
			_instance.RateUsLastTime = DateTime.UtcNow;
			_instance.LevelNumber = 1;
			_instance.TutorialStage = 1;
			_instance.LastSavedTime = DateTime.UtcNow;
			_instance.InterstitialsShown = 0;
			_instance.CoinsCount = 0;
			_instance.LevelClaim = 0;
			_instance.LevelNumberThroughLoops = 1;
			_instance.KeysCount = 0;
			_instance.CurrentBiomId = 0;
			_instance.CurrentLevelsToPlay = new List<int>();
			_instance.PlayedLevels = new List<int>();
			_instance.RandomBioms = false;
			_instance.UnlockedSkins = new List<string>();
			_instance.UnlockedSkins.Add(DefaultSkinName);
			_instance.CurrentSkinId = DefaultSkinName;

			_instance.Save(true);
		}

		public PlayerData GetNewSave()
		{
			var playerData = new PlayerData();
			playerData.VibrationOn = true;
			playerData.SoundOn = true;
			playerData.MusicOn = true;
			playerData.PrivacyAndTermsAccepted = false;
			playerData.LevelCount = 0;
			playerData.LevelLoop = 0;
			playerData.Rated = false;
			playerData.RateUsAttempts = 0;
			playerData.RateUsLastTime = DateTime.UtcNow;
			playerData.LevelNumber = 1;
			playerData.TutorialStage = 1;
			playerData.LastSavedTime = DateTime.UtcNow;
			playerData.InterstitialsShown = 0;
			playerData.CoinsCount = 0;
            playerData.LevelClaim = 0;
            playerData.LevelNumberThroughLoops = 1;
			playerData.KeysCount = 0;
			playerData.CurrentBiomId = 0;
			playerData.CurrentLevelsToPlay.Clear();
			playerData.PlayedLevels.Clear();
			playerData.RandomBioms = false;
			playerData.UnlockedSkins.Clear();
			playerData.UnlockedSkins.Add(DefaultSkinName);

			playerData.CurrentSkinId = DefaultSkinName;

			return playerData;
		}

		public void Read()
		{
			BinaryReader r = null;
			try
			{
				r = new BinaryReader(new FileStream(SaveFile, FileMode.Open));
				var playerData = ProcessPlayerData(r.ReadAllBytes());
				r.Close();

				ApplyPlayerData(playerData);
			}
			catch (Exception e)
			{
				r.Close();
				NewSave();
			}
		}

		public void Save(bool force = false)
		{
			_timeToSave = Time.time + 0.3f;
			if (!_savingInProgress)
			{
				CoroutineHandler.StartStaticCoroutine(SaveCoroutine(force));
			}
		}

		private void SaveInternal()
		{
			BinaryWriter w = null;
			try
			{
				LastSavedTime = DateTime.UtcNow;
				_lastSaveTime = Time.time;
				w = new BinaryWriter(new FileStream(SaveFile, FileMode.OpenOrCreate, FileAccess.ReadWrite,
					FileShare.Delete));
				w.Write(GetBinaryData());
				w.Close();
			}
			catch (IOException e)
			{
				if (w != null)
					w.Close();
				if (File.Exists(SaveFile))
					File.Delete(SaveFile);
				Save();
			}
		}

		private IEnumerator SaveCoroutine(bool force)
		{
			_savingInProgress = true;
			if (!force)
			{
				while (_timeToSave > Time.time && Time.time - _lastSaveTime < 2f)
					yield return null;
			}

			SaveInternal();
			_savingInProgress = false;
		}

		private void ApplyPlayerData(PlayerData playerData)
		{
			VibrationOn = playerData.VibrationOn;
			SoundOn = playerData.SoundOn;
			MusicOn = playerData.MusicOn;
			PrivacyAndTermsAccepted = playerData.PrivacyAndTermsAccepted;
			LevelCount = playerData.LevelCount;
			LevelLoop = playerData.LevelLoop;
			Rated = playerData.Rated;
			RateUsAttempts = playerData.RateUsAttempts;
			RateUsLastTime = playerData.RateUsLastTime;
			LevelNumber = playerData.LevelNumber;
			TutorialStage = playerData.TutorialStage;
			LastSavedTime = playerData.LastSavedTime;
			InterstitialsShown = playerData.InterstitialsShown;
			CoinsCount = playerData.CoinsCount;
			LevelClaim = playerData.LevelClaim;
			LevelNumberThroughLoops = playerData.LevelNumberThroughLoops;
			KeysCount = playerData.KeysCount;
			CurrentBiomId = playerData.CurrentBiomId;
			CurrentLevelsToPlay.Clear();
			CurrentLevelsToPlay.AddRange(playerData.CurrentLevelsToPlay);
			PlayedLevels.Clear();
			PlayedLevels.AddRange(playerData.PlayedLevels);
			RandomBioms = playerData.RandomBioms;
			UnlockedSkins.Clear();
			UnlockedSkins.AddRange(playerData.UnlockedSkins);
			CurrentSkinId = playerData.CurrentSkinId;
		}

		public PlayerData ProcessPlayerData(byte[] data)
		{
			using (var ms = new MemoryStream(data))
			using (var r = new BinaryReader(ms))
			{
				int ver = r.ReadInt32();
				if (ver > _version)
				{
					throw new Exception();
				}

				var pd = GetNewSave();
				pd.VibrationOn = r.ReadBoolean();
				pd.SoundOn = r.ReadBoolean();
				pd.MusicOn = r.ReadBoolean();
				pd.PrivacyAndTermsAccepted = r.ReadBoolean();
				pd.LevelCount = r.ReadInt32();
				pd.LevelLoop = r.ReadInt32();
				pd.Rated = r.ReadBoolean();
				pd.RateUsAttempts = r.ReadInt32();
				var longData = r.ReadInt64();
				var date = DateTime.FromBinary(longData);
				pd.RateUsLastTime = date;
				pd.LevelNumber = r.ReadInt32();
				pd.TutorialStage = r.ReadInt32();

				longData = r.ReadInt64();
				date = DateTime.FromBinary(longData);
				pd.LastSavedTime = date;
				pd.InterstitialsShown = r.ReadInt32();
				pd.CoinsCount = r.ReadInt32();
				pd.LevelClaim = r.ReadInt32();
				pd.LevelNumberThroughLoops = r.ReadInt32();
				pd.KeysCount = r.ReadInt32();
				pd.CurrentBiomId = r.ReadInt32();

				var count = r.ReadInt32();
				for (int i = 0; i < count; i++)
				{
					var levelId = r.ReadInt32();
					pd.CurrentLevelsToPlay.Add(levelId);
				}

				count = r.ReadInt32();
				for (int i = 0; i < count; i++)
				{
					var levelId = r.ReadInt32();
					pd.PlayedLevels.Add(levelId);
				}

				pd.RandomBioms = r.ReadBoolean();

				count = r.ReadInt32();
				for (int i = 0; i < count; i++)
				{
					var skinId = r.ReadString();
					pd.UnlockedSkins.Add(skinId);
				}

				pd.CurrentSkinId = r.ReadString();

				return pd;
			}
		}

		public byte[] GetBinaryData()
		{
			using (var ms = new MemoryStream())
			using (var w = new BinaryWriter(ms))
			{
				w.Write(_version);

				w.Write(VibrationOn);
				w.Write(SoundOn);
				w.Write(MusicOn);
				w.Write(PrivacyAndTermsAccepted);
				w.Write(LevelCount);
				w.Write(LevelLoop);
				w.Write(Rated);
				w.Write(RateUsAttempts);
				w.Write(RateUsLastTime.ToBinary());
				w.Write(LevelNumber);
				w.Write(TutorialStage);

				w.Write(LastSavedTime.ToBinary());
				w.Write(InterstitialsShown);
				w.Write(CoinsCount);
				w.Write(LevelClaim);
				w.Write(LevelNumberThroughLoops);
				w.Write(KeysCount);
				w.Write(CurrentBiomId);

				w.Write(CurrentLevelsToPlay.Count);
				for (int i = 0; i < CurrentLevelsToPlay.Count; i++)
				{
					w.Write(CurrentLevelsToPlay[i]);
				}

				w.Write(PlayedLevels.Count);
				for (int i = 0; i < PlayedLevels.Count; i++)
				{
					w.Write(PlayedLevels[i]);
				}

				w.Write(RandomBioms);

				w.Write(UnlockedSkins.Count);
				for (int i = 0; i < UnlockedSkins.Count; i++)
				{
					w.Write(UnlockedSkins[i]);
				}

				w.Write(CurrentSkinId);

				return ms.ToArray();
			}
		}

		public int GetCoinsCount()
		{
			return CoinsCount;
		}

        public int GetLevelClaimCount()
        {
            return LevelClaim;
        }

        public void AddCoins(int count)
		{
			CoinsCount += count;
			Save();
			EventHandler.ExecuteEvent(InGameEvents.CoinsChanged);
		}

        public void AddLvlClaim(int count)
        {
            LevelClaim += count;
            Save();
        }

        public void AddKeys(int count)
		{
			KeysCount += count;
			Save();
			EventHandler.ExecuteEvent(InGameEvents.KeysChanged);
		}

		public void UnlockSkin(string skinId)
		{
			UnlockedSkins.AddIfNotExist(skinId);
			EventHandler.ExecuteEvent(InGameEvents.SkinUnlocked);
		}
	}
}