using System.Collections.Generic;
using UnityEngine;

public class Rail : EntityBase
{
	public GameObject RailTwoStrong;

	public GameObject RailTwoCurve;

	public GameObject RailThree;

	public GameObject RailFour;

	public GameObject RailElevation;

	private static List<Rail> _AttachedRails = new List<Rail>();

	private static Transform _AttachedRailsGroup = null;

	private GameObject _Model;

	private Collider _ModelCollider;

	private List<Rail> _Siblings;

	private int _ValidSiblingBits = 4095;

	private int _PlaneIndex = -1;

	private int _PlaneBlockX;

	private int _PlaneBlockY;

	private int _PlaneBlockZ;

	private static Vector3[] _PlanePosition = new Vector3[6]
	{
		new Vector3(0f, 0.01f, 0f),
		new Vector3(0f, 0.99f, 0f),
		new Vector3(-0.49f, 0.49f, 0f),
		new Vector3(0.49f, 0.49f, 0f),
		new Vector3(0f, 0.49f, -0.49f),
		new Vector3(0f, 0.49f, 0.49f)
	};

	private static Vector3[] _PlaneRotation = new Vector3[6]
	{
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 0f, 180f),
		new Vector3(0f, 0f, -90f),
		new Vector3(0f, 0f, 90f),
		new Vector3(90f, 0f, 0f),
		new Vector3(-90f, 0f, 0f)
	};

	private static int[,] _PlaneDirections = new int[6, 6]
	{
		{ -1, -1, 0, 2, 1, 3 },
		{ -1, -1, 2, 0, 1, 3 },
		{ 2, 0, -1, -1, 1, 3 },
		{ 0, 2, -1, -1, 1, 3 },
		{ 3, 1, 0, 2, -1, -1 },
		{ 1, 3, 0, 2, -1, -1 }
	};

	private bool _IsTransition;

	public int PlaneIndex => _PlaneIndex;

	public static Vector3[] PlanePosition => _PlanePosition;

	public static Vector3[] PlaneRotation => _PlaneRotation;

	public bool IsTransition => _IsTransition;

	static Rail()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
	}

	public static void ClearAttachedRails()
	{
		_AttachedRails.Clear();
	}

	public static Rail Find(int blockX, int blockY, int blockZ)
	{
		foreach (Rail attachedRail in _AttachedRails)
		{
			if (attachedRail.BlockX == blockX && attachedRail.BlockY == blockY && attachedRail.BlockZ == blockZ)
			{
				return attachedRail;
			}
		}
		return null;
	}

	public static Rail Find(Collider collider)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		foreach (Rail attachedRail in _AttachedRails)
		{
			Bounds bounds = attachedRail._ModelCollider.bounds;
			if (bounds.Intersects(collider.bounds))
			{
				return attachedRail;
			}
		}
		return null;
	}

	public float GetRotationToRail(Rail rail)
	{
		if (_PlaneIndex == rail._PlaneIndex)
		{
			if (_PlaneBlockX == rail._PlaneBlockX)
			{
				return (_PlaneBlockY < rail._PlaneBlockY) ? 180 : 0;
			}
			return (_PlaneBlockX >= rail._PlaneBlockX) ? 90 : 270;
		}
		int num = _PlaneDirections[_PlaneIndex, rail._PlaneIndex];
		int blockX = rail.BlockX;
		int blockY = rail.BlockY;
		int blockZ = rail.BlockZ;
		SetPlaneBlock(_PlaneIndex, ref blockX, ref blockY, ref blockZ);
		if (blockZ < _PlaneBlockZ)
		{
			num = (num + 2) % 4;
		}
		return num switch
		{
			0 => 90, 
			1 => 0, 
			2 => 270, 
			_ => 180, 
		};
	}

	private void Attach()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		if (((Component)this).transform.position.y % 1f < 0.1f)
		{
			_PlaneIndex = 0;
		}
		else if (((Component)this).transform.position.y % 1f > 0.9f)
		{
			_PlaneIndex = 1;
		}
		else if (((Component)this).transform.position.x % 1f < 0.1f)
		{
			_PlaneIndex = 2;
		}
		else if (((Component)this).transform.position.x % 1f > 0.9f)
		{
			_PlaneIndex = 3;
		}
		else if (((Component)this).transform.position.z % 1f < 0.1f)
		{
			_PlaneIndex = 4;
		}
		else
		{
			if (!(((Component)this).transform.position.z % 1f > 0.1f))
			{
				Debug.Log((object)("Wrong rail position! " + ((Component)this).transform.position));
				return;
			}
			_PlaneIndex = 5;
		}
		((Component)this).transform.position = new Vector3((float)BlockX + 0.5f, (float)BlockZ, (float)BlockY + 0.5f);
		((Component)this).transform.rotation = Quaternion.identity;
		((Component)this).transform.Translate(_PlanePosition[_PlaneIndex], (Space)0);
		((Component)this).transform.Rotate(_PlaneRotation[_PlaneIndex], (Space)0);
		_PlaneBlockX = BlockX;
		_PlaneBlockY = BlockY;
		_PlaneBlockZ = BlockZ;
		SetPlaneBlock(_PlaneIndex, ref _PlaneBlockX, ref _PlaneBlockY, ref _PlaneBlockZ);
		foreach (Rail attachedRail in _AttachedRails)
		{
			if (IsSibling(attachedRail))
			{
				SetSibling(attachedRail, add: true);
				attachedRail.SetSibling(this, add: true);
			}
		}
		if ((Object)(object)_Model == (Object)null)
		{
			SetModel(RailTwoStrong);
		}
		_AttachedRails.Add(this);
		if ((Object)(object)_AttachedRailsGroup == (Object)null)
		{
			_AttachedRailsGroup = new GameObject("Rails").transform;
		}
		((Component)this).transform.parent = _AttachedRailsGroup;
		Trolley.OnRailAttached(this);
	}

	[RPC]
	private void Detach()
	{
		if (_Siblings != null)
		{
			foreach (Rail sibling in _Siblings)
			{
				sibling.SetSibling(this, add: false);
			}
			_Siblings = null;
		}
		_AttachedRails.Remove(this);
		((Component)this).transform.parent = null;
		Trolley.OnRailDetached(this);
	}

	public override void SelfDelete()
	{
		photonView.RPC("Detach", PhotonTargets.All);
		base.SelfDelete();
	}

	public Rail GetNext(Vector3 direction, float closestAngle)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if (_Siblings != null)
		{
			Rail rail = null;
			float num = 0f;
			{
				foreach (Rail sibling in _Siblings)
				{
					Vector3 val = ((Component)sibling).transform.position - ((Component)this).transform.position;
					float num2 = Vector3.Angle(val, direction);
					if (Vector3.Dot(Vector3.Cross(val, direction), ((Component)this).transform.up) < 0f)
					{
						num2 = 360f - num2;
					}
					if (num2 > 225f || num2 < 135f)
					{
						float num3 = Mathf.Abs(Mathf.DeltaAngle(num2, closestAngle));
						if ((Object)(object)rail == (Object)null || num3 < num)
						{
							rail = sibling;
							num = num3;
						}
					}
				}
				return rail;
			}
		}
		return null;
	}

	public Rail GetDown()
	{
		if (_Siblings != null)
		{
			foreach (Rail sibling in _Siblings)
			{
				if (sibling.BlockZ < BlockZ || (sibling.BlockZ == BlockZ && sibling._IsTransition))
				{
					return sibling;
				}
			}
		}
		return null;
	}

	public static void SetPlaneBlock(int planeIndex, ref int blockX, ref int blockY, ref int blockZ)
	{
		int num = blockX;
		int num2 = blockY;
		int num3 = blockZ;
		switch (planeIndex)
		{
		case 1:
			blockX = WorldData.Instance.WidthInBlocks - num;
			blockZ = WorldData.Instance.DepthInBlocks - num3;
			break;
		case 2:
			blockX = WorldData.Instance.DepthInBlocks - num3;
			blockZ = num;
			break;
		case 3:
			blockX = num3;
			blockZ = WorldData.Instance.WidthInBlocks - num;
			break;
		case 4:
			blockY = WorldData.Instance.DepthInBlocks - num3;
			blockZ = num2;
			break;
		case 5:
			blockY = num3;
			blockZ = WorldData.Instance.HeightInBlocks - num2;
			break;
		}
	}

	protected override void Creation(object[] data)
	{
		Attach();
	}

	protected override void Updating()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (_Siblings == null)
		{
			return;
		}
		foreach (Rail sibling in _Siblings)
		{
			Debug.DrawLine(((Component)this).transform.position, ((Component)sibling).transform.position, Color.yellow);
		}
	}

	private bool IsSibling(Rail rail)
	{
		if (_PlaneIndex == rail._PlaneIndex)
		{
			int num = rail._PlaneBlockX - _PlaneBlockX;
			int num2 = rail._PlaneBlockY - _PlaneBlockY;
			int num3 = rail._PlaneBlockZ - _PlaneBlockZ;
			int num4 = Mathf.Abs(num);
			int num5 = Mathf.Abs(num2);
			int num6 = Mathf.Abs(num3);
			if (num4 > 1 || num5 > 1 || num6 > 1 || num4 + num5 != 1)
			{
				return false;
			}
			int num7 = num switch
			{
				-1 => 0, 
				0 => (num2 == -1) ? 1 : 3, 
				_ => 2, 
			} + (num3 + 1) * 4;
			int num8 = 11 - num7 - ((num7 % 2 == 0) ? 1 : (-1));
			return ((_ValidSiblingBits >> num7) & 1) != 0 && ((rail._ValidSiblingBits >> num8) & 1) != 0;
		}
		int blockX = rail.BlockX;
		int blockY = rail.BlockY;
		int blockZ = rail.BlockZ;
		SetPlaneBlock(_PlaneIndex, ref blockX, ref blockY, ref blockZ);
		int num9 = blockX - _PlaneBlockX;
		int num10 = blockY - _PlaneBlockY;
		int num11 = blockZ - _PlaneBlockZ;
		if (num9 == 0 && num10 == 0 && num11 == 1)
		{
			int num12 = _PlaneDirections[_PlaneIndex, rail._PlaneIndex];
			if (num12 == -1)
			{
				return false;
			}
			num12 += 8;
			int num13 = _PlaneDirections[rail._PlaneIndex, _PlaneIndex] + 8;
			return ((_ValidSiblingBits >> num12) & 1) != 0 && ((rail._ValidSiblingBits >> num13) & 1) != 0;
		}
		if (Mathf.Abs(num9) + Mathf.Abs(num10) == 1 && num11 == -1)
		{
			int num14 = _PlaneDirections[_PlaneIndex, rail._PlaneIndex];
			if (num14 == -1)
			{
				return false;
			}
			num14 = (num14 + 2) % 4;
			int num15 = (_PlaneDirections[rail._PlaneIndex, _PlaneIndex] + 2) % 4;
			return ((_ValidSiblingBits >> num14) & 1) != 0 && ((rail._ValidSiblingBits >> num15) & 1) != 0;
		}
		return false;
	}

	private void SetSibling(Rail rail, bool add)
	{
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		if (add)
		{
			if (_Siblings == null)
			{
				_Siblings = new List<Rail>();
			}
			_Siblings.Add(rail);
		}
		else
		{
			_Siblings.Remove(rail);
			if (_Siblings.Count == 0)
			{
				_Siblings = null;
			}
		}
		int num = 0;
		int num2 = 0;
		if (_Siblings != null)
		{
			foreach (Rail sibling in _Siblings)
			{
				bool flag = false;
				int num5;
				if (sibling._PlaneIndex == _PlaneIndex)
				{
					int num3 = sibling._PlaneBlockX - _PlaneBlockX;
					int num4 = sibling._PlaneBlockY - _PlaneBlockY;
					num5 = num3 switch
					{
						-1 => 0, 
						0 => (num4 == -1) ? 1 : 3, 
						_ => 2, 
					};
					flag = _PlaneBlockZ < sibling._PlaneBlockZ;
				}
				else
				{
					num5 = _PlaneDirections[_PlaneIndex, sibling._PlaneIndex];
					int blockX = sibling.BlockX;
					int blockY = sibling.BlockY;
					int blockZ = sibling.BlockZ;
					SetPlaneBlock(_PlaneIndex, ref blockX, ref blockY, ref blockZ);
					flag = _PlaneBlockZ < blockZ;
					if (blockZ < _PlaneBlockZ)
					{
						num5 = (num5 + 2) % 4;
					}
				}
				int num6 = 0;
				if (flag)
				{
					num6 |= 1 << (((num5 + 2) % 4) & 0x1F);
					num6 |= 1 << (((num5 + 2) % 4 + 4) & 0x1F);
				}
				else
				{
					num6 |= 1 << (((num5 + 2) % 4) & 0x1F);
					num6 |= 1 << (((num5 + 2) % 4 + 4) & 0x1F);
					num6 |= 1 << (((num5 + 2) % 4 + 8) & 0x1F);
					num6 |= 1 << (((num5 + 1) % 4) & 0x1F);
					num6 |= 1 << (((num5 + 3) % 4) & 0x1F);
					num6 |= 1 << (((num5 + 1) % 4 + 4) & 0x1F);
					num6 |= 1 << (((num5 + 3) % 4 + 4) & 0x1F);
				}
				num |= num6 ^ 0xFFF;
				num2 |= 1 << num5;
				if (flag)
				{
					num2 |= 1 << ((num5 + 4) & 0x1F);
				}
			}
		}
		_ValidSiblingBits = num ^ 0xFFF;
		if (_Siblings != null)
		{
			GameObject val = null;
			Vector3 val2 = Vector3.zero;
			if (_Siblings.Count == 1)
			{
				val = RailTwoStrong;
				val2 = new Vector3(0f, (float)(((num2 & 1) != 0) ? 90 : (((num2 & 2) == 0) ? (((num2 & 4) == 0) ? 180 : 270) : 0)), 0f);
				if ((num2 & 0xF0) != 0)
				{
					val = RailElevation;
					_IsTransition = true;
				}
			}
			else if (_Siblings.Count == 2)
			{
				if ((num2 & 5) == 5 || (num2 & 0xA) == 10)
				{
					if ((num2 & 0xF0) != 0)
					{
						num2 = (num2 >> 4) | (num2 & 0xF0);
					}
					val = RailTwoStrong;
					val2 = new Vector3(0f, (float)(((num2 & 1) != 0) ? 90 : (((num2 & 2) == 0) ? (((num2 & 4) == 0) ? 180 : 270) : 0)), 0f);
					if ((num2 & 0xF0) != 0)
					{
						val = RailElevation;
						_IsTransition = true;
					}
				}
				else
				{
					val = RailTwoCurve;
					val2 = new Vector3(0f, (float)(((num2 & 3) != 3) ? (((num2 & 6) == 6) ? 270 : (((num2 & 0xC) != 12) ? 90 : 180)) : 0), 0f);
				}
			}
			else if (_Siblings.Count == 3)
			{
				int num7 = 0;
				if ((num2 & 1) == 0)
				{
					num7 = 270;
				}
				else if ((num2 & 2) == 0)
				{
					num7 = 180;
				}
				else if ((num2 & 4) == 0)
				{
					num7 = 90;
				}
				val = RailThree;
				val2 = new Vector3(0f, (float)num7, 0f);
			}
			else
			{
				val = RailFour;
			}
			SetModel(val);
			val2.y -= 90f;
			_Model.transform.localRotation = Quaternion.Euler(val2);
		}
		else
		{
			SetModel(RailTwoStrong);
		}
	}

	private void SetModel(GameObject model)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)_Model != (Object)null)
		{
			Object.Destroy((Object)(object)_Model);
		}
		Object val = Object.Instantiate((Object)(object)model, ((Component)this).transform.position, ((Component)this).transform.rotation);
		_Model = (GameObject)(object)((val is GameObject) ? val : null);
		_Model.transform.parent = ((Component)this).transform;
		_Model.GetComponentInChildren<EntityBaseChild>().Parent = this;
		_ModelCollider = _Model.GetComponentInChildren<Collider>();
		UpdateTextures();
		TimeOfDay.Affect(_Model);
	}

	public override void OnButtonE(string playerName)
	{
		if (Creator == playerName || Level.Instance.IsAdministrator())
		{
			SelfDelete();
		}
		else
		{
			Chat.SendInfo(ProfileINI.nickname + Localize.GetText("CANT_TAKE_AND_DESTROY_ITEMS"), forAll: false);
		}
	}
}
