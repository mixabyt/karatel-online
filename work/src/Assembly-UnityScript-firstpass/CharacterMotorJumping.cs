using System;
using UnityEngine;

[Serializable]
public class CharacterMotorJumping
{
	public bool enabled;

	public float baseHeight;

	public float extraHeight;

	public float perpAmount;

	public float steepPerpAmount;

	[NonSerialized]
	public bool jumping;

	[NonSerialized]
	public bool holdingJumpButton;

	[NonSerialized]
	public float lastStartTime;

	[NonSerialized]
	public float lastButtonDownTime;

	[NonSerialized]
	public Vector3 jumpDir;

	public CharacterMotorJumping()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		enabled = true;
		baseHeight = 1f;
		extraHeight = 4.1f;
		steepPerpAmount = 0.5f;
		lastButtonDownTime = -100f;
		jumpDir = Vector3.up;
	}
}
