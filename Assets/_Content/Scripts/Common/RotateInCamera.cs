using System;
using UnityEngine;

namespace Common
{
	public class RotateInCamera: MonoBehaviour
	{
		[SerializeField] private bool _onlyY;
		private Camera _camera;

		private void OnEnable()
		{
			_camera = Camera.main;
			OnCameraChanged();
		}

		private void LateUpdate()
		{
			OnCameraChanged();
		}

		private void OnCameraChanged()
		{
			if (_onlyY)
			{
				var angles = transform.rotation.eulerAngles;
				var cameraAngles = _camera.transform.rotation.eulerAngles;
				angles.y = cameraAngles.y;
				transform.rotation = Quaternion.Euler(angles);
			}
			else
			{
				transform.forward = _camera.transform.forward;
			}
		}
	}
}