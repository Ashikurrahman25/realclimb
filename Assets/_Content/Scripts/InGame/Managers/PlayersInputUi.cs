using Common.UI;
using Doozy.Engine.Touchy;
using UnityEngine;

namespace _Content.InGame.Managers
{
	public class PlayersInputUi : UIViewWrapper
	{
		[SerializeField] private float _maxDelta = 500f;
		[SerializeField] private RectTransform _point1;
		[SerializeField] private RectTransform _dragTransform;
		[SerializeField] private bool _joystickLike;
		private Vector2 _lastDirection;
		private float _lastRatio;
		private float _lastAngle;

		public Vector2 LastDirection => _lastDirection;
		public float LastRatio => _lastRatio;

		public float LastAngle => _lastAngle;

		private void Update()
		{
			if (!_shown)
				return;

			if (TouchDetector.Instance.TouchInProgress)
			{
				var touchInfo = TouchDetector.Instance.CurrentTouchInfo;
				if (_joystickLike)
				{
					var startPosition = _canvas.ScreenToCanvasPosition(touchInfo.StartPosition);
					var currentPosition = _canvas.ScreenToCanvasPosition(touchInfo.CurrentTouchPosition);
					var dif = (currentPosition - startPosition);
					dif = Vector2.ClampMagnitude(dif, _maxDelta);
					var dir = dif.normalized;
					var angle = Vector2.SignedAngle(Vector2.up, dir);
					_dragTransform.rotation = Quaternion.Euler(0f, 0f, angle);
					var deltaSize = _dragTransform.sizeDelta;
					deltaSize.y = Mathf.Abs(dif.magnitude);
					_dragTransform.sizeDelta = deltaSize;
					_lastDirection = dir;
					_lastAngle = angle;
					_lastRatio = Mathf.Clamp01(deltaSize.y / _maxDelta);
					
					var startPositionWorld = GameplayManager.Instance.GetPlayersCenterPosition();
					var startPositionUI = WorldToUiPosition(startPositionWorld);
					_point1.anchoredPosition = startPositionUI;
				}
				else
				{
					var startPositionWorld = GameplayManager.Instance.GetPlayersCenterPosition();
					var startPosition = WorldToUiPosition(startPositionWorld);
					//var startPosition = touchInfo.StartPosition;
					_point1.anchoredPosition = startPosition;
					var currentPosition = touchInfo.CurrentTouchPosition;
					//currentPosition.y /= _canvas.scaleFactor;
					currentPosition = _canvas.ScreenToCanvasPosition(currentPosition);
					//currentPosition = transform.InverseTransformPoint(currentPosition);
					//Debug.Log($"CT3: {currentPosition}");
					var dif = (currentPosition - startPosition);
					dif = Vector2.ClampMagnitude(dif, _maxDelta);
					var dir = dif.normalized;
					var angle = Vector2.SignedAngle(Vector2.up, dir);
					_dragTransform.rotation = Quaternion.Euler(0f, 0f, angle);
					var deltaSize = _dragTransform.sizeDelta;
					deltaSize.y = Mathf.Abs(dif.magnitude);
					_dragTransform.sizeDelta = deltaSize;
					_lastAngle = angle;
					_lastDirection = dir;
					_lastRatio = Mathf.Clamp01(deltaSize.y / _maxDelta);
				}
			}
		}

		private Vector2 WorldToUiPosition(Vector3 worldPosition)
		{
			var screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPosition);
			screenPoint = _canvas.ScreenToCanvasPosition(screenPoint);
			
			return screenPoint;
		}

		public Rect GetScreenCoordinates(RectTransform uiElement)
		{
			var worldCorners = new Vector3[4];
			uiElement.GetWorldCorners(worldCorners);
			var result = new Rect(
				worldCorners[0].x,
				worldCorners[0].y,
				worldCorners[2].x - worldCorners[0].x,
				worldCorners[2].y - worldCorners[0].y);
			return result;
		}

		public override void ShowView(bool force = false)
		{
			base.ShowView(force);
			ShowImages();
			Update();
		}

		public override void HideView(bool force = false)
		{
			base.HideView(force);
			HideImages();
		}

		private void ShowImages()
		{
			_point1.gameObject.SetActive(true);
			_dragTransform.gameObject.SetActive(true);
			var delta = _dragTransform.sizeDelta;
			delta.y = 1f;
			_dragTransform.sizeDelta = delta;
		}

		private void HideImages()
		{
			_point1.gameObject.SetActive(false);
			_dragTransform.gameObject.SetActive(false);
		}
	}
}