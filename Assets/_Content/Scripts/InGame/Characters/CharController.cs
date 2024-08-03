using System;
using RootMotion.Dynamics;
using UnityEngine;

namespace _Content.InGame.Characters
{
	public class CharController : MonoBehaviour
	{
		[SerializeField] private Rigidbody _connector;
		[SerializeField] private GameObject _rHand;
		[SerializeField] private Rigidbody _pelvisRb;
		[SerializeField] private float _force = 200f;
		private Collider[] _ragdollColliders;
		private Rigidbody[] _ragdollBodies;
		private bool _ragdollIsActive;

		private void Start()
		{
			_ragdollBodies = GetComponentsInChildren<Rigidbody>();
			_ragdollColliders = GetComponentsInChildren<Collider>();
			foreach (var rb in _ragdollBodies)
			{
				//rb.constraints = RigidbodyConstraints.FreezePositionZ;
			}

			ToggleRagdoll(false);
		}

		private void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				if (_ragdollIsActive)
				{
					var existingHj = _rHand.GetComponent<HingeJoint>();
					if (existingHj != null)
					{
						Destroy(existingHj);
					}
					else
					{
						var hj = _rHand.AddComponent<HingeJoint>();
						_connector.transform.position = _rHand.transform.position;
						hj.autoConfigureConnectedAnchor = false;
						hj.anchor = Vector3.zero;
						hj.connectedBody = _connector;
					}
				}
				else
				{
					ToggleRagdoll(true);
					_pelvisRb.AddForce(Vector3.up * _force);
				}

				foreach (var rb in _ragdollBodies)
				{
				}
				//_puppetMaster.muscles[0].broadcaster.Hit(1f,Vector3.up * _force, transform.position);
			}

			if (Mathf.Abs(_pelvisRb.velocity.z) > 0.001f)
			{
				var vel = _pelvisRb.velocity;
				vel.z = 0f;
				_pelvisRb.velocity = vel;
			}
		}

		private void ToggleRagdoll(bool state)
		{
			_ragdollIsActive = state;
			foreach (var rb in _ragdollBodies)
			{
				rb.isKinematic = !state;
			}

			foreach (var col in _ragdollColliders)
			{
				col.enabled = state;
			}
		}
	}
}