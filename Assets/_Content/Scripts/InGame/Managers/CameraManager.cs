using System;
using System.Collections.Generic;
using _Content.InGame.Level;
using Base;
using Cinemachine;
using UnityEngine;

namespace _Content.InGame.Managers
{
	public class CameraManager : Singleton<CameraManager>
	{
		[SerializeField] private SpriteRenderer _cameraBackground;
		[SerializeField] private float _yOffsetToStopFollowing = 5f;
		[SerializeField] private CinemachineVirtualCamera _gameplayCamera;
		[SerializeField] private CinemachineVirtualCamera _shopCamera;
		[SerializeField] private Transform _cameraPoint;

		private float _minimumCameraY;
		private Camera _camera;

		public Vector3 CameraPosition => _cameraPoint.position;

		private void Start()
		{
			_camera = Camera.main;
		}

		public void ShowShopCamera()
		{
			_gameplayCamera.Priority = 0;
			_shopCamera.Priority = 2;
		}

		public void ShowGameplayCamera()
		{
			_gameplayCamera.Priority = 2;
			_shopCamera.Priority = 0;
		}

		private void LateUpdate()
		{
			if (GameplayManager.Instance.State == GameplayManager.GameState.InProgress)
			{
				var characterCenter = GameplayManager.Instance.GetPlayersCenterPosition();

				if (characterCenter.y > _minimumCameraY)
				{
					_cameraPoint.position = characterCenter;
					SetMinimumCameraY(characterCenter.y);
				}
				else if (characterCenter.y > _minimumCameraY - _yOffsetToStopFollowing)
				{
					_cameraPoint.position = characterCenter;
				}
			}
		}

		public bool IsAnyMeshesOutOfCameraCameraView(List<Bounds> boundsList)
		{
			foreach (var bounds in boundsList)
			{
				if (!IsMeshInCameraView(bounds))
					return true;
			}

			return false;
		}

		public bool IsMeshInCameraView(Bounds bounds)
		{
			Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(_camera);
			return GeometryUtility.TestPlanesAABB(frustumPlanes, bounds);
		}

		public void ResetMinimumCameraY()
		{
			_minimumCameraY = -1000f;
		}

		public void SetMinimumCameraY(float minimumY)
		{
			_minimumCameraY = minimumY;
		}

		public void CameraOnGameOver()
		{
			_gameplayCamera.Follow = null;
		}

		public void SetCameraPosition(Vector3 position)
		{
			_gameplayCamera.Follow = _cameraPoint;
			_cameraPoint.position = position;
			ResetMinimumCameraY();
		}

		public void SetBiom(Bioms.BiomInfo currentBiom)
		{
			_cameraBackground.sprite = currentBiom.BiomSprite;
		}
	}
}