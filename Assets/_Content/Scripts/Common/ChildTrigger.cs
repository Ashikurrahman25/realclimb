using System;
using UnityEngine;

namespace Common
{
	public class ChildTrigger: MonoBehaviour
	{
		public event Action<Collider> TriggerEntered;
		public event Action<Collider> TriggerStay;
		public event Action<Collider> TriggerExit;
		private void OnTriggerEnter(Collider other)
		{
			TriggerEntered?.Invoke(other);
		}

		private void OnTriggerStay(Collider other)
		{
			TriggerStay?.Invoke(other);
		}

		private void OnTriggerExit(Collider other)
		{
			TriggerExit?.Invoke(other);
		}
	}
}