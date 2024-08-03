using NaughtyAttributes;
using UnityEngine;

namespace _Content.InGame.Level
{
	public class Plank: MonoBehaviour
	{
		[SerializeField] private FixedJoint _fixedJoint;
		
		public void Grab()
		{
			if (_fixedJoint != null)
			{
				_fixedJoint.breakForce = -1f;
				_fixedJoint.breakTorque = 0f;
			}

			Debug.Log("Grabbing!! ");
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