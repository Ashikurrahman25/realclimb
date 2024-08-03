using System;
using System.Collections.Generic;
using DG.Tweening;
using RootMotion.Dynamics;
using UnityEngine;

namespace _Content.InGame.Characters
{
	public class CharacterMoveIndicator : MonoBehaviour
	{
		[SerializeField] private float _pointsZPosition;
		[SerializeField] private float _maxDistance;
		[SerializeField] private AnimationCurve _maxDistanceCurve;
		[SerializeField] private float _maxXDistance;
		[SerializeField] private PuppetController _puppetController;
		[SerializeField] private GameObject[] _points;
		[SerializeField] private AnimationCurve _forwardBonesCurve;
		[SerializeField] private AnimationCurve _sideBonesCurve;
		[SerializeField] private Material _idleMaterial;
		[SerializeField] private Material _enableMaterial;
		[SerializeField] private Material _disableMaterial;
		[SerializeField] private AnimationCurve _pointsDissapierCurve;
		private bool _shown;
		private Vector3 _lastDirection;
		private float _lastRatio;
		private List<Vector3> _initialScales;

		private void Awake()
		{
			_initialScales = new List<Vector3>();
			foreach (var point in _points)
			{
				_initialScales.Add(point.transform.localScale);
			}
			Hide(true);
		}

		private void LateUpdate()
		{
			if (!_shown)
				return;

			UpdatePoints();
		}

		public void UpdateDirectionAndRatio(Vector3 direction, float ratio)
		{
			_lastDirection = direction;
			_lastRatio = ratio;
		}

		private void UpdatePoints()
		{
			var direction = _lastDirection;
			var ratio = _lastRatio;
			var r = _maxDistanceCurve.Evaluate(ratio);
			var distance = Mathf.Lerp(0f, _maxDistance, r);
			var start = _puppetController.GetCenter() + direction * 1f;
			start.z = _pointsZPosition;
			var rotation = Quaternion.Euler(direction);
			var xDir = rotation * (_lastDirection.x >= 0f ? Vector3.right : Vector3.left);
			var xDif = Mathf.Lerp(0f, 1f, Mathf.Abs(_lastDirection.x)) * _maxXDistance;
			var xRatio = Mathf.Abs(direction.y);
			for (int i = 0; i < _points.Length; i++)
			{
				var t = i / (float)_points.Length;
				var boneDist = Mathf.Lerp(0f, distance, _forwardBonesCurve.Evaluate(t));
				var sideDist = Mathf.Lerp(0f, xDif, _sideBonesCurve.Evaluate(t));
				var position = start + direction * boneDist + (xDir * sideDist) * xRatio;
				_points[i].transform.position = position;
			}
		}

		public void Show()
		{
			for (int i = 0; i < _points.Length; i++)
			{
				var point = _points[i];
				var initialScale = _initialScales[i];
				var rend = point.GetComponent<MeshRenderer>();
				rend.material = _idleMaterial;
				point.transform.localScale = initialScale;
				point.SetActive(true);
			}

			UpdatePoints();
			_shown = true;
		}

		public void Hide(bool force)
		{
			if (force)
			{
				foreach (var point in _points)
				{
					point.SetActive(false);
				}
			}
			else
			{
				foreach (var point in _points)
				{
					var rend = point.GetComponent<MeshRenderer>();
					rend.material = _lastDirection.y < 0f ? _disableMaterial : _enableMaterial;
					point.transform.DOScale(Vector3.zero, 0.2f)
						.SetEase(_pointsDissapierCurve)
						.OnComplete(() => point.SetActive(false));
				}
			}

			_shown = false;
		}
	}
}