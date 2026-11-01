using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Boo.Lang;
using UnityEngine;

[Serializable]
public class DragRigidbody : MonoBehaviour
{
	[Serializable]
	[CompilerGenerated]
	internal sealed class _0024DragObject_002449 : GenericGenerator<object>
	{
		[Serializable]
		[CompilerGenerated]
		internal sealed class _0024 : GenericGeneratorEnumerator<object>, IEnumerator
		{
			internal float _0024oldDrag_002450;

			internal float _0024oldAngularDrag_002451;

			internal Camera _0024mainCamera_002452;

			internal Ray _0024ray_002453;

			internal float _0024distance_002454;

			internal DragRigidbody _0024self__002455;

			public _0024(float distance, DragRigidbody self_)
			{
				_0024distance_002454 = distance;
				_0024self__002455 = self_;
			}

			public override bool MoveNext()
			{
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
				int result;
				switch (_state)
				{
				default:
					_0024oldDrag_002450 = ((Joint)_0024self__002455.springJoint).connectedBody.drag;
					_0024oldAngularDrag_002451 = ((Joint)_0024self__002455.springJoint).connectedBody.angularDrag;
					((Joint)_0024self__002455.springJoint).connectedBody.drag = _0024self__002455.drag;
					((Joint)_0024self__002455.springJoint).connectedBody.angularDrag = _0024self__002455.angularDrag;
					_0024mainCamera_002452 = _0024self__002455.FindCamera();
					goto case 2;
				case 2:
					if (Input.GetMouseButton(0))
					{
						_0024ray_002453 = _0024mainCamera_002452.ScreenPointToRay(Input.mousePosition);
						((Component)_0024self__002455.springJoint).transform.position = _0024ray_002453.GetPoint(_0024distance_002454);
						result = (YieldDefault(2) ? 1 : 0);
						break;
					}
					if (Object.op_Implicit((Object)(object)((Joint)_0024self__002455.springJoint).connectedBody))
					{
						((Joint)_0024self__002455.springJoint).connectedBody.drag = _0024oldDrag_002450;
						((Joint)_0024self__002455.springJoint).connectedBody.angularDrag = _0024oldAngularDrag_002451;
						((Joint)_0024self__002455.springJoint).connectedBody = null;
					}
					YieldDefault(1);
					goto case 1;
				case 1:
					result = 0;
					break;
				}
				return (byte)result != 0;
			}
		}

		internal float _0024distance_002456;

		internal DragRigidbody _0024self__002457;

		public _0024DragObject_002449(float distance, DragRigidbody self_)
		{
			_0024distance_002456 = distance;
			_0024self__002457 = self_;
		}

		public override IEnumerator<object> GetEnumerator()
		{
			return new _0024(_0024distance_002456, _0024self__002457);
		}
	}

	public float spring;

	public float damper;

	public float drag;

	public float angularDrag;

	public float distance;

	public bool attachToCenterOfMass;

	private SpringJoint springJoint;

	public DragRigidbody()
	{
		spring = 50f;
		damper = 5f;
		drag = 10f;
		angularDrag = 5f;
		distance = 0.2f;
	}

	public override void Update()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected Obj, but got Unknown
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Expected Obj, but got Unknown
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		if (!Input.GetMouseButtonDown(0))
		{
			return;
		}
		Camera val = FindCamera();
		RaycastHit val2 = default;
		if (Physics.Raycast(val.ScreenPointToRay(Input.mousePosition), ref val2, 100f) && Object.op_Implicit((Object)(object)val2.rigidbody) && !val2.rigidbody.isKinematic)
		{
			if (!Object.op_Implicit((Object)(object)springJoint))
			{
				GameObject val3 = new GameObject("Rigidbody dragger");
				Component val4 = val3.AddComponent("Rigidbody");
				Rigidbody val5 = (Rigidbody)(object)((val4 is Rigidbody) ? val4 : null);
				springJoint = (SpringJoint)val3.AddComponent("SpringJoint");
				val5.isKinematic = true;
			}
			((Component)springJoint).transform.position = val2.point;
			if (attachToCenterOfMass)
			{
				Vector3 val6 = ((Component)this).transform.TransformDirection(val2.rigidbody.centerOfMass) + ((Component)val2.rigidbody).transform.position;
				val6 = ((Component)springJoint).transform.InverseTransformPoint(val6);
				((Joint)springJoint).anchor = val6;
			}
			else
			{
				((Joint)springJoint).anchor = Vector3.zero;
			}
			springJoint.spring = spring;
			springJoint.damper = damper;
			springJoint.maxDistance = distance;
			((Joint)springJoint).connectedBody = val2.rigidbody;
			((MonoBehaviour)this).StartCoroutine("DragObject", (object)val2.distance);
		}
	}

	public override IEnumerator DragObject(float distance)
	{
		return new _0024DragObject_002449(distance, this).GetEnumerator();
	}

	public override Camera FindCamera()
	{
		return (!Object.op_Implicit((Object)(object)((Component)this).camera)) ? Camera.main : ((Component)this).camera;
	}

	public override void Main()
	{
	}
}
