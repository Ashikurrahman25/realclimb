using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Content.Data;
using _Content.InGame.Characters;
using _Content.InGame.Level;
using Base;
using Doozy.Engine.Touchy;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace _Content.InGame.Managers
{
	public class GameplayManager : Singleton<GameplayManager>
	{
		public enum BonusRewardType
		{
			Coin
		}

		[Serializable]
		public class BonusReward
		{
			public BonusRewardType RewardType;
			public Sprite Icon;
			public int Count;
		}

		public enum GameState
		{
			WaitForStart,
			InProgress,
			GameOver,
			Win
		}

		[SerializeField] private int _levelReward = 50;
		[SerializeField] private MMF_Player _onWinFeedback;
		[SerializeField] private MMF_Player _onLoseFeedback;
		[SerializeField] private GameObject _characterPrefab;
		[SerializeField] private float _timeBeforeReload = 3f;
		[SerializeField] private List<BonusReward> _bonusGameRewards;
		[SerializeField] private Collider _groundCollider;
		private PuppetController _puppetBehavior;
		private CharacterMoveIndicator _moveIndicator;

		public GameState State;
		private Transform _charPositionPoint;
		private PlayersInput _playersInput;
		private int _currentLevelKeys;
		private SkinHandler _skinHandler;
		private bool _enableInput;
		private int _currentLevelCoins;
		private Helicopter _helicopter;
		private float _endY;
		private float _startY;
		public bool GameInProgress { get; set; }

		public int CurrentLevelKeys => _currentLevelKeys;
		public float GameTimer { get; set; }

		public Transform puppetMaster, fullChar, playerInput;

		protected override void OnAwake()
		{
			base.OnAwake();
			_onWinFeedback?.Initialization(gameObject);
			_onLoseFeedback?.Initialization(gameObject);
		}

		private void Update()
		{
			if (State == GameState.GameOver || State == GameState.Win)
				return;
			if (_puppetBehavior == null)
				return;

			if (State == GameState.InProgress && _puppetBehavior.FirstInputHappened)
			{
				GameTimer += Time.deltaTime;
			}
			
			if (!_enableInput)
				return;
			
			HandlePlayersInput();
			if (_puppetBehavior.IsGrabbing())
			{
				CameraManager.Instance.ResetMinimumCameraY();
			}
			else
			{
				if (_puppetBehavior.FirstInputHappened && CameraManager.Instance.IsAnyMeshesOutOfCameraCameraView(
					    _puppetBehavior.Meshes.Select(m => m.bounds)
						    .ToList()))
				{
					GameOver();
				}
			}
		}

		public void InitializeLevel(LevelInfo prevLevelInfo, Action onLevelInitialized)
		{
			_currentLevelKeys = 0;

			_groundCollider.enabled = true;
			var charPositionGo = GameObject.FindWithTag("Character Position");
			_charPositionPoint = charPositionGo.transform;

			var charGo = Instantiate(_characterPrefab, _charPositionPoint.position, Quaternion.Euler(0f, 180f, 0f));
			_puppetBehavior = charGo.GetComponentInChildren<PuppetController>();
			_moveIndicator = charGo.GetComponentInChildren<CharacterMoveIndicator>();
			_playersInput = charGo.GetComponentInChildren<PlayersInput>();
			_skinHandler = charGo.GetComponentInChildren<SkinHandler>();
			State = GameState.InProgress;

			CameraManager.Instance.SetCameraPosition(_charPositionPoint.position);
			_puppetBehavior.Teleport(_charPositionPoint.position);

			UIManager.Instance.PlayersInputUi.ShowView();
			var skin = GameManager.Instance.GetCurrentSkin();
			_skinHandler.SetSkin(skin);
			onLevelInitialized();
			_currentLevelCoins = 0;
			GameTimer = 0f;
			_helicopter = FindObjectOfType<Helicopter>();

			_startY = GetPlayersCenterPosition().y;
			_endY = _helicopter.transform.position.y;
			_enableInput = true;

			puppetMaster = charGo.transform.GetChild(1).GetChild(0);
			fullChar = charGo.transform.GetChild(2);
            playerInput = charGo.transform.GetChild(4);
			GlobalData.target = charGo.transform.position;
		}

        public void DeinitializeLevel()
		{
			State = GameState.WaitForStart;
			UIManager.Instance.PlayersInputUi.HideView();
			Destroy(_puppetBehavior.transform.parent.gameObject);
			_puppetBehavior = null;
			_moveIndicator = null;
			_playersInput = null;
			_skinHandler = null;
			_helicopter = null;
		}

		private void GameOver()
		{
            _enableInput = false;
            State = GameState.GameOver;
			_puppetBehavior.EnableAbilityOfGrabbing(false);
			StartCoroutine(ShowView());
			AdsController.instance.ShowInterstitialAds();
			//Implementation.Instance.ShowInterstitial();
		}

		private IEnumerator ShowView()
		{
			_onLoseFeedback?.PlayFeedbacks();
			yield return new WaitForSeconds(_timeBeforeReload);
			UIManager.Instance.FailedUi.ShowView();
            puppetMaster.gameObject.SetActive(false);

        }

		private IEnumerator HideVide()
		{
			yield return new WaitForSeconds(.5f);
            UIManager.Instance.FailedUi.HideView();
            State = GameState.InProgress;
            _enableInput = true;
            UIManager.Instance.LoadingUi.HideView();

        }

        public void ContinueGame()
		{
            

            fullChar.localPosition = GlobalData.target;
			puppetMaster.localPosition = GlobalData.target;
			playerInput.localPosition = GlobalData.target;
            puppetMaster.gameObject.SetActive(true);

			GlobalData.continueCount++;
            _puppetBehavior.EnableAbilityOfGrabbing(true);
			StartCoroutine(HideVide());
        }

		private void HandlePlayersInput()
		{
			_playersInput.transform.position = GetPlayersCenterPosition();
			var hasTouch = TouchDetector.Instance.TouchInProgress;
			
			if (hasTouch && (!_puppetBehavior.FirstInputHappened || _puppetBehavior.IsGrabbing()) &&
			    !UIManager.Instance.PlayersInputUi.IsShown)
			{
				UIManager.Instance.PlayersInputUi.ShowView();
				_playersInput.Show();
				var lastDirection = UIManager.Instance.PlayersInputUi.LastDirection;
				var forceDirection = new Vector3(-lastDirection.x, -lastDirection.y, 0f);
				_moveIndicator.UpdateDirectionAndRatio(forceDirection, UIManager.Instance.PlayersInputUi.LastRatio);
				_moveIndicator.Show();
			}
			else if (!hasTouch && UIManager.Instance.PlayersInputUi.IsShown)
			{
				UIManager.Instance.PlayersInputUi.HideView();
				_playersInput.Hide();
				_moveIndicator.Hide(false);
			}

			if (hasTouch && (!_puppetBehavior.FirstInputHappened || _puppetBehavior.IsGrabbing()))
			{
				var touchInfo = TouchDetector.Instance.CurrentTouchInfo;
				var lastDirection = UIManager.Instance.PlayersInputUi.LastDirection;
				var ratio = UIManager.Instance.PlayersInputUi.LastRatio;
				if (!_puppetBehavior.FirstInputHappened && ratio < 0.3f)
					return;
				
				var forceDirection = new Vector3(-lastDirection.x, -lastDirection.y, 0f);
				_moveIndicator.UpdateDirectionAndRatio(forceDirection, UIManager.Instance.PlayersInputUi.LastRatio);
				if (touchInfo.Touch.phase == TouchPhase.Ended)
				{
					var result = _puppetBehavior.OnInput(forceDirection, UIManager.Instance.PlayersInputUi.LastRatio);
					if (result && UIManager.Instance.LevelProgressionUi.IsShown)
					{
						UIManager.Instance.LevelProgressionUi.HideView();
						UIManager.Instance.MovingTutorial.HideView();
						UIManager.Instance.SkinShopButtonUi.HideView();
						_groundCollider.enabled = false;
					}
				}
			}
		}

		public Vector3 GetPlayersCenterPosition()
		{
			return _puppetBehavior.GetCenter();
		}

		public PuppetController GetPuppetController()
		{
			return _puppetBehavior;
		}

		public void AddCoinsForLevelReward()
		{
			_currentLevelCoins++;
		}

		public void AddCoinsToData(int count)
		{
			PlayerData.Instance.AddCoins(count);
		}

		public void WinGame()
		{
			State = GameState.Win;
			StartCoroutine(StartHelicopter());
			//Implementation.Instance.ShowInterstitial();
		}

		private IEnumerator StartHelicopter()
		{
			yield return new WaitForSeconds(0.5f);
			CameraManager.Instance.CameraOnGameOver();
			if (_helicopter != null)
			{

				_helicopter.GoAway();
			}

			
			yield return new WaitForSeconds(1f);
			_onWinFeedback?.PlayFeedbacks();
			yield return new WaitForSeconds(1f);
            AdsController.instance.ShowInterstitialAds();
            UIManager.Instance.WinUi.ShowView();
		}

		public int GetRewardForLevel()
		{
			return _levelReward + _currentLevelCoins;
		}

		public void AddKeyToSave()
		{
			var count = _currentLevelKeys;
			_currentLevelKeys = 0;
			PlayerData.Instance.AddKeys(count);
		}

		public void AddKey(int count)
		{
			_currentLevelKeys += count;
		}

		public List<BonusReward> GetRewardsForBonusGame()
		{
			return _bonusGameRewards.ToList();
		}

		public void DisableGame()
		{
			_enableInput = false;
			UIManager.Instance.PlayersInputUi.HideView();
			_playersInput.Hide();
			_moveIndicator.Hide(false);
		}

		public void EnableGame()
		{
			_enableInput = true;
			_puppetBehavior.FirstInputHappened = false;
		}

		public int GetProgress()
		{
			var value = Mathf.Clamp01(Mathf.InverseLerp(_startY, _endY, GetPlayersCenterPosition().y));
			return Mathf.RoundToInt(value * 100);
		}
	}
}