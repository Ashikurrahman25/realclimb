using RootMotion.Dynamics;
using UnityEngine;

namespace _Content.InGame.Characters
{
	public class HandController : MonoBehaviour
	{
		public ConfigurableJoint PhysicsTarget;
		public Transform target;
		[Range(0f, 1f)] public float forceWeight = 1f;
		[Range(0f, 1f)] public float torqueWeight = 1f;
		public bool useTargetVelocity = true;

		private Rigidbody r;
		public Vector3 lastTargetPos;
		private Quaternion lastTargetRot = Quaternion.identity;

		/// <summary>
		/// Call this after target has been teleported
		/// </summary>
		public void OnTargetTeleported()
		{
			if (target != null)
			{
				lastTargetPos = GetTargetPosition();
				lastTargetRot = target.rotation;
			}
		}

		private void Start()
		{
			r = GetComponent<Rigidbody>();
			OnTargetTeleported();
		}

		private void FixedUpdate()
		{
			if (target == null)
				return;
			
			Vector3 targetVelocity = Vector3.zero;
			Vector3 targetAngularVelocity = Vector3.zero;

			// Calculate target velocity and angular velocity
			if (useTargetVelocity)
			{
				targetVelocity = (GetTargetPosition() - lastTargetPos) / Time.deltaTime;

				targetAngularVelocity = PhysXTools.GetAngularVelocity(lastTargetRot, target.rotation, Time.deltaTime);
			}

			lastTargetPos = GetTargetPosition();
			lastTargetRot = target.rotation;

			// Force
			Vector3 force = PhysXTools.GetLinearAcceleration(r.position, GetTargetPosition());
			force += targetVelocity;
			force -= r.velocity;
			if (r.useGravity) force -= Physics.gravity * Time.deltaTime;
			force *= forceWeight;
			r.AddForce(force, ForceMode.VelocityChange);

			// Torque
			Vector3 torque = PhysXTools.GetAngularAcceleration(r.rotation, target.rotation);
			torque += targetAngularVelocity;
			torque -= r.angularVelocity;
			torque *= torqueWeight;
			r.AddTorque(torque, ForceMode.VelocityChange);
		}
		
		private Vector3 GetTargetPosition()
		{
			return target.position - Vector3.forward * 0.2f;
		}
	}
}