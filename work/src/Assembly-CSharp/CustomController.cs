using System;
using UnityEngine;
using hd;

public class CustomController : MonoBehaviour
{
	private Vector3 m_Gravity = new Vector3(0f, -0.98f, 0f);

	private float m_BoxWidth = 0.5f;

	private Vector3 m_Velocity;

	private Vector3 m_Forward;

	public float Speed = 0.1f;

	public WorldGameObject World;

	private WorldData m_WorldData;

	public CustomController()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Start()
	{
		m_WorldData = World.WorldData;
	}

	private void Update()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Component)this).transform.position;
		m_Velocity = Vector3.zero;
		m_Forward = ((Component)this).transform.forward;
		if (Input.inputString.Contains("w"))
		{
			m_Velocity = m_Forward;
		}
		else if (Input.inputString.Contains("s"))
		{
			m_Velocity = -m_Forward;
		}
		m_Velocity += m_Gravity;
		if (CollidesWithY(position, m_Velocity.y, out var newVelocity))
		{
			m_Velocity.y = newVelocity;
		}
		Transform transform = ((Component)this).transform;
		transform.position += m_Velocity;
	}

	private bool CollidesWithX(Vector3 position, float movement, out float newVelocity)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		float num = Speed * (float)Math.Sign(movement);
		Vector3 val = new Vector3(position.x + num + m_BoxWidth, position.y, position.z);
		if (m_WorldData.GetBlockType((int)val.x, (int)val.y, (int)val.z) != BlockType.Air)
		{
			newVelocity = 0f;
			return true;
		}
		newVelocity = movement;
		return false;
	}

	private bool CollidesWithY(Vector3 position, float movement, out float newVelocity)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		float num = Speed * (float)Math.Sign(movement);
		Vector3 val = new Vector3(position.x, position.y + num + m_BoxWidth, position.z);
		Debug.Log("Checking " + val.x + ", " + val.z + ", " + val.y);
		BlockType blockType = m_WorldData.GetBlockType((int)val.x, (int)val.z, (int)val.y);
		if (blockType != BlockType.Air)
		{
			Debug.Log(string.Concat("Block ", blockType, " hit at ", val.x, ", ", val.z, ", ", val.y));
			newVelocity = 0f;
			return true;
		}
		newVelocity = movement;
		return false;
	}
}
