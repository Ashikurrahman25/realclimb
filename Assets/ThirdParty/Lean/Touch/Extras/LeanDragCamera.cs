using UnityEngine;
using Lean.Common;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Touch
{
	/// <summary>This component allows you to move the current GameObject (e.g. Camera) based on finger drags and the specified ScreenDepth.</summary>
	[HelpURL(LeanTouch.HelpUrlPrefix + "LeanDragCamera")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Drag Camera")]
	public class LeanDragCamera : MonoBehaviour
	{
		public bool EnableDrag = true;
		public Vector2 MinMaxX;
		public Vector2 MinMaxZ;

		/// <summary>The method used to find fingers to use with this component. See LeanFingerFilter documentation for more information.</summary>
		public LeanFingerFilter Use = new LeanFingerFilter(true);

		/// <summary>The method used to find world coordinates from a finger. See LeanScreenDepth documentation for more information.</summary>
		public LeanScreenDepth ScreenDepth = new LeanScreenDepth(LeanScreenDepth.ConversionType.DepthIntercept);

		/// <summary>The movement speed will be multiplied by this.
		/// -1 = Inverted Controls.</summary>
		public float Sensitivity
		{
			set { sensitivity = value; }
			get { return sensitivity; }
		}

		[FSA("Sensitivity")] [SerializeField] private float sensitivity = 1.0f;

		/// <summary>If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.
		/// -1 = Instantly change.
		/// 1 = Slowly change.
		/// 10 = Quickly change.</summary>
		public float Damping
		{
			set { damping = value; }
			get { return damping; }
		}

		[FSA("Damping")] [FSA("Dampening")] [SerializeField]
		private float damping = -1.0f;

		/// <summary>This allows you to control how much momentum is retained when the dragging fingers are all released.
		/// NOTE: This requires <b>Damping</b> to be above 0.</summary>
		public float Inertia
		{
			set { inertia = value; }
			get { return inertia; }
		}

		[FSA("Inertia")] [SerializeField] [Range(0.0f, 1.0f)]
		private float inertia;

		[SerializeField] public Vector3 remainingDelta;
		private Camera _camera;

		/// <summary>This method moves the current GameObject to the center point of all selected objects.</summary>
		[ContextMenu("Move To Selection")]
		public virtual void MoveToSelection()
		{
			var center = default(Vector3);
			var count = 0;

			foreach (var selectable in LeanSelectable.Instances)
			{
				if (selectable.IsSelected == true)
				{
					center += selectable.transform.position;
					count += 1;
				}
			}

			if (count > 0)
			{
				var oldPosition = transform.localPosition;

				transform.position = center / count;

				remainingDelta += transform.localPosition - oldPosition;

				transform.localPosition = oldPosition;
			}
		}

		/// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually add a finger.</summary>
		public void AddFinger(LeanFinger finger)
		{
			Use.AddFinger(finger);
		}

		/// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually remove a finger.</summary>
		public void RemoveFinger(LeanFinger finger)
		{
			Use.RemoveFinger(finger);
		}

		/// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually remove all fingers.</summary>
		public void RemoveAllFingers()
		{
			Use.RemoveAllFingers();
		}

#if UNITY_EDITOR
		protected virtual void Reset()
		{
			Use.UpdateRequiredSelectable(gameObject);
		}
#endif

		protected virtual void Awake()
		{
			Use.UpdateRequiredSelectable(gameObject);
			_camera = Camera.main;
		}

		public virtual void LateUpdate()
		{
			var fingers = Use.UpdateAndGetFingers();
			if (!EnableDrag)
			{
				//fingers.ForEach(f => f.ScreenPosition = f.LastScreenPosition);
				return;
			}
			// Get the fingers we want to use

			var startPoint = LeanGesture.GetStartScreenCenter(fingers);
			// Get the last and current screen point of all fingers
			var lastScreenPoint = LeanGesture.GetLastScreenCenter(fingers);
			var screenPoint = LeanGesture.GetScreenCenter(fingers);
			
			// Get the world delta of them after conversion	
			var worldDelta = ScreenDepth.ConvertDelta(lastScreenPoint, screenPoint, gameObject);
			var absoluteWorldDelta = ScreenDepth.ConvertDelta(startPoint, screenPoint, gameObject);
			if (absoluteWorldDelta.magnitude < 0.2f)
				return;

			// Store the current position
			var oldPosition = transform.localPosition;

			// Pan the camera based on the world delta
			transform.position -= worldDelta * sensitivity;

			// Add to remainingDelta
			remainingDelta += transform.localPosition - oldPosition;

			// Get t value
			var factor = LeanHelper.GetDampenFactor(damping, Time.deltaTime);

			// Dampen remainingDelta
			var newRemainingDelta = Vector3.Lerp(remainingDelta, Vector3.zero, factor);

			// Shift this position by the change in delta
			var newPosition = oldPosition + remainingDelta - newRemainingDelta;
			newPosition = ClampNewPosition(newPosition);
			
			transform.localPosition = newPosition;

			if (fingers.Count == 0 && inertia > 0.0f && damping > 0.0f)
			{
				newRemainingDelta = Vector3.Lerp(newRemainingDelta, remainingDelta, inertia);
			}

			// Update remainingDelta with the dampened value
			remainingDelta = newRemainingDelta;
		}

		private Vector3 ClampNewPosition(Vector3 newPosition)
		{
			float verticalSeen = _camera.orthographicSize * 2.0f;
			float horizontalSeen = verticalSeen * Screen.width / Screen.height;
			var halfVertical = verticalSeen / 2f;
			var halfHorizontal = horizontalSeen / 2f;
			var angle = transform.rotation.eulerAngles.y;
			
			var minX = (newPosition + Vector3.left * halfHorizontal);
			var maxX = (newPosition + Vector3.right * halfHorizontal);
			var minZ = (newPosition + Vector3.back * halfVertical);
			var maxZ = (newPosition + Vector3.forward * halfVertical);
			
			var rotatedNewPosition = newPosition;
			if (maxX.x > MinMaxX.y)
			{
				rotatedNewPosition.x = MinMaxX.y - halfHorizontal;
			}
			else if (minX.x < MinMaxX.x)
			{
				rotatedNewPosition.x = MinMaxX.x + halfHorizontal;
			}

			if (maxZ.z > MinMaxZ.y)
			{
				rotatedNewPosition.z = MinMaxZ.y - halfVertical;
			}
			else if (minZ.z < MinMaxZ.x)
			{
				rotatedNewPosition.z = MinMaxZ.x + halfVertical;
			}

			newPosition = rotatedNewPosition;

			return newPosition;
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using TARGET = LeanDragCamera;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET), true)]
	public class LeanDragCamera_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt;
			TARGET[] tgts;
			GetTargets(out tgt, out tgts);

			Draw("MinMaxX");
			Draw("MinMaxZ");
			Draw("Use");
			Draw("ScreenDepth");
			Draw("sensitivity", "The movement speed will be multiplied by this.\n\n-1 = Inverted Controls.");
			Draw("damping",
				"If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.\n\n-1 = Instantly change.\n\n1 = Slowly change.\n\n10 = Quickly change.");
			Draw("inertia",
				"This allows you to control how much momentum is retained when the dragging fingers are all released.\n\nNOTE: This requires <b>Damping</b> to be above 0.");
		}
	}
}
#endif