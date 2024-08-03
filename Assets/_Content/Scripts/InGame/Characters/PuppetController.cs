using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Content.InGame.Level;
using _Content.InGame.Managers;
using MoreMountains.Feedbacks;
using NaughtyAttributes;
using RootMotion.Dynamics;
using Unity.VisualScripting;
using UnityEngine;

namespace _Content.InGame.Characters
{
	public class PuppetController : BehaviourBase
	{
		[Serializable]
		private class MuscleInfo
		{
			public float MuscleWeight;
			public float PinWeight;
		}

		private class ControllerInfo
		{
			public HandController Controller;
			public Rigidbody Point;
		}

		[SerializeField] private List<SkinnedMeshRenderer> _meshes;
		[SerializeField] private Rigidbody _rHand;
		[SerializeField] private Rigidbody _lHand;
		[SerializeField] private Rigidbody _rFoot;
		[SerializeField] private Rigidbody _lFoot;
		[SerializeField] private HandController _rHandController;
		[SerializeField] private HandController _lHandController;
		[SerializeField] private HandController _rFootController;
		[SerializeField] private HandController _lFootController;
		[SerializeField] private Vector2 _angularVelocityMinMax;
		[SerializeField] private Vector2 _forceMinMax;
		[SerializeField] private AnimationCurve _forceCurve;
		[SerializeField] private ForceMode _forceMode = ForceMode.VelocityChange;
		[SerializeField] private float _checkRadius = 0.1f;
		[SerializeField] private LayerMask _pointsMask;
		[SerializeField, ReadOnly] private bool _targetEnabled;
		[SerializeField, ReadOnly] private Vector3 _lastAngularVelocity;
		[Space] [SerializeField] private MMF_Player _onAddForceFeedback;
		[SerializeField] private MMF_Player _onGrabFeedback;

		private List<ControllerInfo> _controllerInfos = new List<ControllerInfo>();

		private RaycastHit[] _pointCheckResults = new RaycastHit[1];
		private float _timerToCheck;
		private bool _ragdollIsActive;
		private float _timer;
		private bool _endTriggered;
		private bool _firstInputHappened;
		private bool _canGrab;
		private MuscleInfo[] _musclesInfos;

		public List<SkinnedMeshRenderer> Meshes => _meshes;

		public bool FirstInputHappened
		{
			get { return _firstInputHappened; }
			set { _firstInputHappened = value; }
		}


		protected override void OnActivate()
		{
			_onAddForceFeedback?.Initialization(gameObject);
			_onGrabFeedback?.Initialization(gameObject);
			_canGrab = true;
			_targetEnabled = true;
			forceActive = true;
			_controllerInfos.Add(new ControllerInfo() { Controller = _lHandController, Point = _lHand });
			_controllerInfos.Add(new ControllerInfo() { Controller = _rHandController, Point = _rHand });
			_controllerInfos.Add(new ControllerInfo() { Controller = _lFootController, Point = _lFoot });
			_controllerInfos.Add(new ControllerInfo() { Controller = _rFootController, Point = _rFoot });
			SaveMuscleInfo();
			StopAllCoroutines();
		}

		private void SaveMuscleInfo()
		{
			_musclesInfos = new MuscleInfo[puppetMaster.muscles.Length];
			for (int i = 0; i < puppetMaster.muscles.Length; i++)
			{
				_musclesInfos[i] = new MuscleInfo()
				{
					MuscleWeight = puppetMaster.muscles[i].props.muscleWeight,
					PinWeight = puppetMaster.muscles[i].props.pinWeight
				};
				puppetMaster.muscles[i].props.muscleWeight = 1f;
			}
		}

		protected override void OnDeactivate()
		{
			forceActive = false;
		}

		public override void OnReactivate()
		{
			_targetEnabled = true;
		}

		protected override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			if (Input.GetMouseButtonDown(0))
			{
				if (!_ragdollIsActive)
				{
					ToggleRagdoll(true);
				}

				/*if (!_targetEnabled)
				{
					_rHandTarget.transform.position = _rHand.transform.position;
					_rHandTarget.connectedBody = _rHand;
					_targetEnabled = true;
				}*/
			}
		}

		protected override void OnFixedUpdate(float deltaTime)
		{
			base.OnFixedUpdate(deltaTime);
			if (_timerToCheck > 0f)
			{
				_timerToCheck -= deltaTime;
				return;
			}
			else
			{
				if (_canGrab)
					HandleGrabbing();
			}
		}

		private bool CheckGrabPoint(HandController controller, Rigidbody point)
		{
			var size = Physics.SphereCastNonAlloc(point.transform.position, _checkRadius, Vector3.forward,
				_pointCheckResults, 10f, _pointsMask);

			if (size > 0)
			{
				var col = _pointCheckResults[0].collider;
				var r = col.GetComponent<Rigidbody>();
				if (r != null)
				{

					var tr = r.transform;
					var movingPlatform = col.GetComponent<MovingClimbPlatform>() ??
					                     col.GetComponentInParent<MovingClimbPlatform>();
					var windmill = col.GetComponent<Windmill>() ??
					               col.GetComponentInParent<Windmill>();
					var physicsBasePoint = col.GetComponent<PhysicsBasePoint>();
					var freeGrabbingPoint = col.GetComponent<FreeGrabbingPoint>();
					var rope = col.GetComponentInParent<Rope>();
					var plank = col.GetComponentInParent<Plank>();
					if (IsNotGrabbed(tr))
					{

                        if (rope != null)
						{
                            rope.Grab();
						}

						if (plank != null)
						{
                            plank.Grab();
						}

						if (movingPlatform != null)
						{

                            movingPlatform.OnceGrabbed = true;
							DisableGrabbing();
							if (physicsBasePoint != null)
							{
								var joint = AddedJoint(physicsBasePoint.gameObject, point, Vector3.zero,
									physicsBasePoint.UseZAxisForRotation);
								controller.PhysicsTarget = joint;
								controller.forceWeight = 0f;
							}
							else
							{
								controller.forceWeight = 1f;
							}

							controller.target = tr;
							point.mass = 10f;
							EnableAbilityOfGrabbing(false);
							return false;
						}
						else if (windmill != null)
						{
                            DisableGrabbing();
							controller.forceWeight = 1f;
							controller.target = tr;
							point.mass = 10f;
							EnableAbilityOfGrabbing(false);
							_onGrabFeedback?.PlayFeedbacks();
							return false;
						}
						else
						{
							if (physicsBasePoint != null)
							{

                                DisableGrabbing();
								var joint = AddedJoint(physicsBasePoint.gameObject, point, Vector3.zero,
									physicsBasePoint.UseZAxisForRotation);
								controller.target = tr;
								controller.PhysicsTarget = joint;
								controller.forceWeight = 0f;
								EnableAbilityOfGrabbing(false);
								_onGrabFeedback?.PlayFeedbacks();
								return false;
							}
							else
							{
								controller.forceWeight = 1f;
							}

							if (freeGrabbingPoint != null)
							{

                                DisableGrabbing();
								var pos = freeGrabbingPoint.GetNearestPosition(point.position);
								var joint = AddedJoint(freeGrabbingPoint.gameObject, point, Vector3.zero, true);
								controller.PhysicsTarget = joint;
								controller.forceWeight = 0f;
								EnableAbilityOfGrabbing(false);
								controller.target = tr;
								point.mass = 10f;
								GameplayManager.Instance.WinGame();
								_onGrabFeedback?.PlayFeedbacks();
								return false;
							}

							controller.target = tr;
							point.mass = 10f;
						}

						if (!GlobalData.GAME_STARTTED && GlobalData.continueCount == 0)
						{
							GlobalData.GAME_STARTTED = true;
						}

						GlobalData.target = new Vector3( -controller.target.localPosition.x, controller.target.localPosition.y+1,0);
                        _onGrabFeedback?.PlayFeedbacks();
					}
				}
			}

			return true;
		}

		private ConfigurableJoint AddedJoint(GameObject physicsPoint, Rigidbody point)
		{
			return AddedJoint(physicsPoint, point, Vector3.zero, false);
		}

		private ConfigurableJoint AddedJoint(GameObject physicsPoint, Rigidbody point, Vector3 worldPositionAnchor,
			bool zAngularMotion = false)
		{
			var joint = physicsPoint.AddComponent<ConfigurableJoint>();
			joint.connectedBody = point;
			joint.autoConfigureConnectedAnchor = false;
			var anchor = GetAnchorForPoint(point);
			joint.connectedAnchor = anchor;
			//joint.connectedAnchor = worldPositionAnchor;

			joint.xMotion = ConfigurableJointMotion.Locked;
			joint.yMotion = ConfigurableJointMotion.Locked;
			joint.zMotion = ConfigurableJointMotion.Locked;
			joint.angularXMotion = zAngularMotion ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Free;
			joint.angularYMotion = ConfigurableJointMotion.Limited;
			joint.angularZMotion = zAngularMotion ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Limited;

			return joint;
		}

		private Vector3 GetAnchorForPoint(Rigidbody point)
		{
			if (point == _lFoot || point == _rFoot)
			{
				return new Vector3(0f, 0f, 0.13f);
			}

			if (point == _rHand)
				return new Vector3(-0.13f, 0f, 0f);

			return new Vector3(0.13f, 0f, 0f);
		}

		private bool IsNotGrabbed(Transform newTarget)
		{
			return _rHandController.target != newTarget && _lHandController.target != newTarget &&
			       _rFootController.target != newTarget && _lFootController.target != newTarget;
		}

		private void HandleGrabbing()
		{

            var hips = puppetMaster.muscles.FirstOrDefault(m => m.props.group == Muscle.Group.Hips);
			if (hips != null)
			{
				if (hips.rigidbody.velocity.y < 1f)
				{
					var controllers = _controllerInfos.OrderByDescending(ci => ci.Point.position.y).ToList();
					for (int i = 0; i < controllers.Count; i++)
					{
						var controller = controllers[i];
						if (controller.Controller.target == null)
						{
							var result = CheckGrabPoint(controller.Controller, controller.Point);
							if (!result)
								break;
						}
					}
				}
			}
		}

		public bool IsGrabbing()
		{
			return _rHandController.target != null || _lHandController.target != null ||
			       _rFootController.target != null || _lFootController.target != null;
		}

		public bool OnInput(Vector3 forceDirection, float ratio)
		{
			if (forceDirection.y <= 0f)
				return false;

			if (!_firstInputHappened)
			{
				ActivateRagdoll();
				_firstInputHappened = true;
			}

			DisableGrabbing();

			AddForce(forceDirection, ratio);
			EnableAbilityOfGrabbing(true);
			_onAddForceFeedback?.PlayFeedbacks();
			
			return true;
		}

		private void ActivateRagdoll()
		{
			for (int i = 0; i < puppetMaster.muscles.Length; i++)
			{
				puppetMaster.muscles[i].props.muscleWeight = _musclesInfos[i].MuscleWeight;
				puppetMaster.muscles[i].props.pinWeight = _musclesInfos[i].PinWeight;
			}
		}

		private void DisableGrabbing()
		{
			_rHandController.target = null;
			if (_rHandController.PhysicsTarget != null)
			{
				_rHandController.PhysicsTarget.connectedBody = null;
				Destroy(_rHandController.PhysicsTarget);
			}

			_rHandController.PhysicsTarget = null;

			_lHandController.target = null;
			if (_lHandController.PhysicsTarget != null)
			{
				_lHandController.PhysicsTarget.connectedBody = null;
				Destroy(_lHandController.PhysicsTarget);
			}

			_lHandController.PhysicsTarget = null;

			_rFootController.target = null;
			if (_rFootController.PhysicsTarget != null)
			{
				_rFootController.PhysicsTarget.connectedBody = null;
				Destroy(_rFootController.PhysicsTarget);
			}

			_rFootController.PhysicsTarget = null;

			_lFootController.target = null;
			if (_lFootController.PhysicsTarget != null)
			{
				_lFootController.PhysicsTarget.connectedBody = null;
				Destroy(_lFootController.PhysicsTarget);
			}

			_lFootController.PhysicsTarget = null;

			_timerToCheck = 0.5f;
		}

		private void AddForce(Vector3 forceDirection, float ratio = 1f)
		{
			var r = _forceCurve.Evaluate(ratio);
			var force = Mathf.Lerp(_forceMinMax.x, _forceMinMax.y, r);
			AddForce(forceDirection, force, ratio);
		}

		private void AddForce(Vector3 forceDirection, float force, float ratio = 1f)
		{
			_rHand.mass = 1;
			_lHand.mass = 1;
			_rFoot.mass = 1;
			_lFoot.mass = 1;
			foreach (Muscle m in puppetMaster.muscles)
			{
				m.rigidbody.velocity = Vector3.zero;
				m.rigidbody.angularVelocity = Vector3.zero;
				if (m.props.group == Muscle.Group.Hips)
				{
					var position = m.rigidbody.position;
					if (Mathf.Abs(forceDirection.x) > 0.3f)
					{
						forceDirection.x = Mathf.Sign(forceDirection.x) * 0.3f;
					}

					m.rigidbody.AddForceAtPosition(forceDirection * force, m.rigidbody.worldCenterOfMass, _forceMode);
				}
				/*if (m.props.group == Muscle.Group.Hips)
					m.rigidbody.AddTorque(0f, 0f, Random.Range(-130f, 130f));*/
			}

			_lastAngularVelocity = new Vector3(0f, 0f,
				-Mathf.Sign(forceDirection.x) * Mathf.Lerp(_angularVelocityMinMax.x, _angularVelocityMinMax.y,
					Mathf.Abs(forceDirection.x)));
		}

		private void ToggleRagdoll(bool state)
		{
			_ragdollIsActive = state;
		}

		protected override void OnLateUpdate(float deltaTime)
		{
			if (puppetMaster.muscles[0].state.mappingWeightMlp < 1f) return;
			if (puppetMaster.muscles[0].rigidbody.isKinematic) return;
			if (puppetMaster.isBlending) return;

			puppetMaster.targetRoot.position +=
				puppetMaster.muscles[0].transform.position - puppetMaster.muscles[0].target.position;
			//GroundTarget(raycastLayers);
		}

		public override void OnMuscleReconnected(Muscle m)
		{
			base.OnMuscleReconnected(m);

			m.state.pinWeightMlp = 0f;
			m.state.muscleWeightMlp = 1;
			m.state.muscleDamperMlp = 1;
			m.state.maxForceMlp = 1;
			m.state.mappingWeightMlp = 1f;
		}

		public Vector3 GetCenter()
		{
			var rigidbody = puppetMaster.muscles[0].rigidbody;
			var pos = rigidbody.position;
			pos += rigidbody.rotation * Vector3.up * 0.2f;
			return pos;
		}

		public void AddForceFromSource(Vector3 direction, float force)
		{
			DisableGrabbing();
			AddForce(direction, force, 1f);
		}

		public void EnableAbilityOfGrabbing(bool canGrab)
		{
			_canGrab = canGrab;
		}

		public void Teleport(Vector3 position)
		{
		}
	}
}