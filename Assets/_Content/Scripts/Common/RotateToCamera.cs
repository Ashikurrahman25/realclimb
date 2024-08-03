using UnityEngine;

namespace Common
{
	public class RotateToCamera: MonoBehaviour
	{
		private Camera _camera;

		private void Start()
		{
			_camera = Camera.main;
		}
		private void Update()
		{
			if (_camera != null)
			{
				var dif = transform.position - _camera.transform.position;
				dif.y = 0f;
				dif.Normalize();
				transform.forward = dif;
			}
		}
	}
}