using System;
using System.Collections.Generic;
using System.Linq;
using _Content.Appmetricas;
using _Content.Data;
using _Content.Events;
using _Content.InGame.Characters;
using _Content.InGame.Level;
using Base;
using DG.Tweening;
using MoreMountains.Feedbacks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using EventHandler = Opsive.Shared.Events.EventHandler;

namespace _Content.InGame.Managers
{
	public class GameManager : Singleton<GameManager>
	{
		[SerializeField] private bool _showAllRigidbodies;
		[SerializeField] private int _levelsInProgression;
		[Expandable] [SerializeField] private Skins _skins;
		[Expandable] [SerializeField] private Bioms _bioms;
		[Expandable] [SerializeField] private LevelsList _levelsList;
		[SerializeField] private MMF_Player _musicFeedback;
		private LevelInfo _prevLevelInfo;
		private bool _unloadOperationCompleted;
		private bool _loadOperationCompleted;
		private bool _levelInitialized;
		private bool _lastRestartWasDefeat;

		public int LevelsInProgression => _levelsInProgression;
		public LevelInfo Level => _prevLevelInfo;

		public Bioms Bioms => _bioms;

		public void StartGame()
		{
			LoadCurrentLevelScene();
			UIManager.Instance.CheatsUi?.ShowView();
			UIManager.Instance.SettingsButton?.ShowView();
			_musicFeedback?.Initialization(gameObject);
			_musicFeedback?.PlayFeedbacks();
		}

		public void LoadCurrentLevelScene()
		{
			var levelId = GetCurrentLevelIndex();
			LoadLevelScene(levelId);
		}

		public int GetCurrentLevelIndex()
		{
			if (PlayerData.Instance.CurrentLevelsToPlay.Count <= 0)
			{
				if (PlayerData.Instance.LevelNumberThroughLoops < _levelsList.Levels.Count)
				{
					var toSkip = PlayerData.Instance.LevelNumberThroughLoops - 1;
					var toTake = Mathf.Min(_levelsInProgression, _levelsList.Levels.Count - toSkip);
					var levels = _levelsList.Levels.Skip(toSkip).Take(toTake).Select(l => l.LevelId).ToList();
					PlayerData.Instance.CurrentLevelsToPlay.Clear();
					PlayerData.Instance.CurrentLevelsToPlay.AddRange(levels);
					PlayerData.Instance.Save();
				}
				else
				{
					var levelsToRandom = _levelsList.Levels.ToList();
					var levelsIds = new List<int>();
					for (int i = 0; i < _levelsInProgression; i++)
					{
						levelsIds.Add(levelsToRandom.PopRandomElement().LevelId);
					}

					PlayerData.Instance.CurrentLevelsToPlay.Clear();
					PlayerData.Instance.CurrentLevelsToPlay.AddRange(levelsIds);
					PlayerData.Instance.Save();
				}

				if (PlayerData.Instance.RandomBioms)
				{
					PlayerData.Instance.CurrentBiomId = _bioms.BiomInfos.GetRandomElement().BiomId;
				}
				else
				{
					PlayerData.Instance.CurrentBiomId++;
					if (PlayerData.Instance.CurrentBiomId > _bioms.BiomInfos.Count)
					{
						PlayerData.Instance.CurrentBiomId = _bioms.BiomInfos.GetRandomElement().BiomId;
						PlayerData.Instance.RandomBioms = true;
					}
				}

				PlayerData.Instance.Save();
			}

			if (PlayerData.Instance.CurrentLevelsToPlay.Count > 0)
			{
				return PlayerData.Instance.CurrentLevelsToPlay[0];
			}
			else
			{
				return 1;
			}
		}

		private void LoadLevelScene(int levelId)
		{
			var levelInfo = _levelsList.Levels.FirstOrDefault(l => l.LevelId == levelId);

			if (levelInfo == null)
			{
				levelInfo = _levelsList.Levels.GetRandomElement();
			}

			if (_prevLevelInfo != null)
			{
				DeinitializeLevel();
			}

			_unloadOperationCompleted = false;
			_loadOperationCompleted = false;
			if (_prevLevelInfo != null)
			{
				var sceneName = _prevLevelInfo.SceneName;
				_prevLevelInfo = levelInfo;
				var unloadAsyncOperation = SceneManager.UnloadSceneAsync(sceneName);
				unloadAsyncOperation.completed += OnUnloadOperationCompleted;
			}
			else
			{
				_prevLevelInfo = levelInfo;
				_unloadOperationCompleted = true;
			}

			var loadAsyncOperation = SceneManager.LoadSceneAsync(levelInfo.SceneName, LoadSceneMode.Additive);
			loadAsyncOperation.allowSceneActivation = true;
			loadAsyncOperation.completed += OnLoadOperationComplete;
		}

		private void DeinitializeLevel()
		{
			EventHandler.ExecuteEvent(InGameEvents.DeinitializeLevel);
			GameplayManager.Instance.DeinitializeLevel();
			_levelInitialized = false;
			//FreezingManager.Instance.Deinitialize();
		}

		private void OnLoadOperationComplete(AsyncOperation obj)
		{
			_loadOperationCompleted = true;
			if (_unloadOperationCompleted)
				SetCurrentLevelSceneActive();
		}

		private void OnUnloadOperationCompleted(AsyncOperation obj)
		{
			_unloadOperationCompleted = true;
			if (_loadOperationCompleted)
				SetCurrentLevelSceneActive();
		}

		private void SetCurrentLevelSceneActive()
		{
			var currentBiom = GetBiom();
			CameraManager.Instance.SetBiom(currentBiom);
			GameplayManager.Instance.InitializeLevel(_prevLevelInfo, OnLevelInitialized);
		}

		private Bioms.BiomInfo GetBiom()
		{
			var biomInfo = _bioms.BiomInfos.FirstOrDefault(b => b.BiomId == PlayerData.Instance.CurrentBiomId);
			if (biomInfo == null)
			{
				biomInfo = _bioms.BiomInfos.FirstOrDefault();
			}

			return biomInfo;
		}

		public void OnLevelInitialized()
		{
			UIManager.Instance.LoadingUi?.HideViewAfterShowingComplete();
			/*UIManager.Instance.MovementTutorUi.ShowView();
			UIManager.Instance.LevelInfoUi.ShowView();*/
			UIManager.Instance.SkinShopButtonUi.ShowView();
			UIManager.Instance.CurrentMoneyUi.ShowView();
			UIManager.Instance.LevelProgressionUi.ShowView();
			UIManager.Instance.MovingTutorial.ShowView();

			PlayerData.Instance.LevelCount++;
			PlayerData.Instance.Save();

			AppMetricaEvents.SendLevelStartEvent(PlayerData.Instance.LevelNumber, 
				PlayerData.Instance.LevelCount, _prevLevelInfo?.LevelName ?? "", PlayerData.Instance.LevelLoop);
			_levelInitialized = true;
			
			if (!_lastRestartWasDefeat)
				DOVirtual.DelayedCall(0.5f, UIManager.Instance.RateController.TryShowRateView);
		}

		public void RestartAfterDefeat()
		{
			_lastRestartWasDefeat = true;
			AppMetricaEvents.SendLevelEndEvent(PlayerData.Instance.LevelNumber, PlayerData.Instance.LevelCount,
				_prevLevelInfo?.LevelName, PlayerData.Instance.LevelLoop, LevelResult.Lose, Mathf.RoundToInt(GameplayManager.Instance.GameTimer),
				GameplayManager.Instance.GetProgress(), GameplayManager.Instance.CurrentLevelKeys > 0);

			/*AppMetricaEvents.SendLevelEndEvent(PlayerData.Instance.LevelNumber, PlayerData.Instance.LevelCount,
				_prevLevelInfo?.LevelCaption, 1, LevelResult.Lose, Mathf.RoundToInt(GameplayManager.Instance.GameTimer),
				EnemyManager.Instance.GetLevelProgressForAnalytics());*/

			UIManager.Instance.FailedUi.HideView();
			UIManager.Instance.LoadingUi.ShowView();
			DOVirtual.DelayedCall(0.5f, LoadCurrentLevelScene);
			//LoadCurrentLevelScene();
		}

		public void RestartAfterWinning()
		{
			_lastRestartWasDefeat = false;
			AppMetricaEvents.SendLevelEndEvent(PlayerData.Instance.LevelNumber, PlayerData.Instance.LevelCount,
				_prevLevelInfo?.LevelName, PlayerData.Instance.LevelLoop, LevelResult.Win, Mathf.RoundToInt(GameplayManager.Instance.GameTimer),
				100, GameplayManager.Instance.CurrentLevelKeys > 0);

			PlayerData.Instance.LevelNumber++;
			PlayerData.Instance.LevelNumberThroughLoops++;

			var currentLevel = PlayerData.Instance.CurrentLevelsToPlay[0];
			PlayerData.Instance.CurrentLevelsToPlay.RemoveAt(0);
			PlayerData.Instance.PlayedLevels.Add(currentLevel);
			if (PlayerData.Instance.CurrentLevelsToPlay.Count == 0 && PlayerData.Instance.LevelLoop < 1 &&
			    PlayerData.Instance.LevelNumberThroughLoops > _levelsList.Levels.Count)
			{
				PlayerData.Instance.LevelLoop++;
			}

			PlayerData.Instance.Save();

			UIManager.Instance.WinUi.HideView();
			UIManager.Instance.BonusGameUi.HideView();
			UIManager.Instance.LoadingUi.ShowView();
			DOVirtual.DelayedCall(0.5f, LoadCurrentLevelScene);
		}

		public void ClearData()
		{
			PlayerData.NewSave();
			LoadCurrentLevelScene();
		}

		public void HideSettings()
		{
			UIManager.Instance.SettingsUi.HideView();
			UIManager.Instance.SettingsButton.ShowView();
			UIManager.Instance.SkinShopButtonUi.ShowView();
			GameplayManager.Instance.GameInProgress = true;
		}

		public void ShowSettings()
		{
			UIManager.Instance.SettingsUi.ShowView();
			UIManager.Instance.SettingsButton.HideView();
			UIManager.Instance.SkinShopButtonUi.HideView();
			GameplayManager.Instance.GameInProgress = false;
			//GameplayManager.Instance.ResetInput();
		}

		public void ShowBonusGame()
		{
			UIManager.Instance.BonusGameUi.ShowView();
			UIManager.Instance.WinUi.HideView();
		}

		public void SkipLevel()
		{
			RestartAfterWinning();
		}

		private void OnDrawGizmos()
		{
#if UNITY_EDITOR
			if (_showAllRigidbodies)
			{
				var rigidbodies = FindObjectsOfType<Rigidbody>();
				Gizmos.color = Color.yellow;

				foreach (var r in rigidbodies)
				{
					Gizmos.DrawSphere(r.position, 0.1f);
				}
			}
#endif
		}

		public Skins.Skin GetSkin(string skinId)
		{
			var skin = _skins.SkinsList.FirstOrDefault(s => s.SkinId == skinId);
			if (skin == null)
				skin = _skins.SkinsList.FirstOrDefault();

			return skin;
		}

		public Skins.Skin GetCurrentSkin()
		{
			return GetSkin(PlayerData.Instance.CurrentSkinId);
		}

		[Button()]
		public void ShowSkinShop()
		{
			GameplayManager.Instance.DisableGame();
			UIManager.Instance.SettingsButton.HideView();
			UIManager.Instance.MovingTutorial.HideView();
			UIManager.Instance.LevelProgressionUi.HideView();
			UIManager.Instance.SkinShopButtonUi.HideView();
			UIManager.Instance.SkinsShopUi.ShowView();
			CameraManager.Instance.ShowShopCamera();
		}

		public void HideSkinShop()
		{
			GameplayManager.Instance.EnableGame();
			UIManager.Instance.SkinsShopUi.HideView();
			UIManager.Instance.SkinShopButtonUi.ShowView();
			UIManager.Instance.SettingsButton.ShowView();
			UIManager.Instance.MovingTutorial.ShowView();
			UIManager.Instance.LevelProgressionUi.ShowView();
			CameraManager.Instance.ShowGameplayCamera();
		}

        [Button()]
        public void ShowChestUI()
        {
            GameplayManager.Instance.DisableGame();
            UIManager.Instance.SettingsButton.HideView();
            UIManager.Instance.MovingTutorial.HideView();
            UIManager.Instance.LevelProgressionUi.HideView();
            UIManager.Instance.SkinShopButtonUi.HideView();
            UIManager.Instance.ChestUI.ShowView();
            //CameraManager.Instance.ShowShopCamera();
        }

        public void HideChestUI()
        {
            GameplayManager.Instance.EnableGame();
            UIManager.Instance.ChestUI.HideView();
            UIManager.Instance.SkinShopButtonUi.ShowView();
            UIManager.Instance.SettingsButton.ShowView();
            UIManager.Instance.MovingTutorial.ShowView();
            UIManager.Instance.LevelProgressionUi.ShowView();
            //CameraManager.Instance.ShowGameplayCamera();
        }

        public List<Skins.Skin> GetAllSkins()
		{
			return _skins.SkinsList.ToList();
		}

		public bool HasSkinToBuy()
		{
			return _skins.SkinsList.Any(s =>
				!PlayerData.Instance.UnlockedSkins.Contains(s.SkinId) && s.Cost <= PlayerData.Instance.CoinsCount);
		}

		public void ShowRateView()
		{
			UIManager.Instance.RateController.ShowView();
			GameplayManager.Instance.GameInProgress = false;
		}

		public void HideRateView()
		{
			UIManager.Instance.RateController.HideView();
			GameplayManager.Instance.GameInProgress = true;
		}
	}
}