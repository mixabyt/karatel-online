using System;
using UnityEngine;

[Serializable]
public class CharacterMotorMovement
{
	public float maxForwardSpeed;

	public float maxSidewaysSpeed;

	public float maxBackwardsSpeed;

	public AnimationCurve slopeSpeedMultiplier;

	public float maxGroundAcceleration;

	public float maxAirAcceleration;

	public float gravity;

	public float maxFallSpeed;

	[NonSerialized]
	public CollisionFlags collisionFlags;

	[NonSerialized]
	public Vector3 velocity;

	[NonSerialized]
	public Vector3 frameVelocity;

	[NonSerialized]
	public Vector3 hitPoint;

	[NonSerialized]
	public Vector3 lastHitPoint;

	public CharacterMotorMovement()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected Obj, but got Unknown
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		maxForwardSpeed = 10f;
		maxSidewaysSpeed = 10f;
		maxBackwardsSpeed = 10f;
		slopeSpeedMultiplier = new AnimationCurve(new Keyframe[3]
		{
			new Keyframe(-90f, 1f),
			new Keyframe(0f, 1f),
			new Keyframe(90f, 0f)
		});
		maxGroundAcceleration = 30f;
		maxAirAcceleration = 20f;
		gravity = 10f;
		maxFallSpeed = 20f;
		frameVelocity = Vector3.zero;
		hitPoint = Vector3.zero;
		lastHitPoint = new Vector3(float.PositiveInfinity, 0f, 0f);
	}
}
