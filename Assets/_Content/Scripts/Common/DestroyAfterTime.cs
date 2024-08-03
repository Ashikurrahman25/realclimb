using UnityEngine;

namespace Common
{
	public class DestroyAfterTime: MonoBehaviour
	{
		[SerializeField] private float _time = 2f;
		private void Start()
		{
			Destroy(gameObject, _time);
		}
	}
}