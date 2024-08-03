using System;
using MoreMountains.Feedbacks;
using NaughtyAttributes;
using UnityEngine;

namespace _Content.InGame.Level
{
	public class Rope : MonoBehaviour
	{
		[SerializeField] private Joint _endConnector;
		[SerializeField] private bool _destroyEndConnectorGo;
		[SerializeField] private MMF_Player _onFirstGrabFeedback;

		private void Awake()
		{
			_onFirstGrabFeedback?.Initialization(gameObject);
		}

		public void Grab()
		{
			if (_endConnector != null)
			{
				_endConnector.connectedBody = null;
				Destroy(_endConnector);
				_endConnector = null;
				if (_destroyEndConnectorGo)
					Destroy(_endConnector.gameObject);

				_onFirstGrabFeedback?.PlayFeedbacks();
			}
		}

		[Button()]
		public void Simulate()
		{
			Physics.autoSimulation = false;
			Physics.Simulate(Time.fixedDeltaTime);
			Physics.autoSimulation = true;
		}
	}
}