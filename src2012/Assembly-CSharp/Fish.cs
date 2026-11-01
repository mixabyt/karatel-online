using System;
using UnityEngine;

public class Fish : EntityBase
{
	public const int MaxFindPosIteration = 10;

	public const int FindPosRadius = 3;

	public GameObject FishObj;

	private Vector3 _LastPos = Vector3.zero;

	private Vector3 _NewPos;

	private BlockType _CurCube;

	private Random _Random = new Random();

	public Fish()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
	}

	protected override void Creation(object[] data)
	{
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		BlockType blockType = WorldData.Instance.GetBlockType(BlockX, BlockY, BlockZ - 1);
		BlockType blockType2 = WorldData.Instance.GetBlockType(BlockX, BlockY, BlockZ);
		_ = photonView;
		if (1 == 0)
		{
			if (blockType2 != BlockType.Water)
			{
				FishObj.animation.Play("breathing");
			}
			((MonoBehaviour)this).Invoke("TestCurentCube", 1f);
		}
		else if (blockType2 == BlockType.Water)
		{
			OnFinishDiving();
			((MonoBehaviour)this).Invoke("TestCurentCube", 1f);
		}
		else if (blockType == BlockType.Water)
		{
			Vector3 val = new Vector3(((Component)this).gameObject.transform.position.x, ((Component)this).gameObject.transform.position.y - 0.6f, ((Component)this).gameObject.transform.position.z);
			iTween.MoveTo(((Component)this).gameObject, iTween.Hash("easetype", iTween.EaseType.easeInOutQuad, "position", val, "time", 2f));
			((MonoBehaviour)this).Invoke("OnFinishDiving", 2f);
			((MonoBehaviour)this).Invoke("TestCurentCube", 1f);
		}
		else
		{
			FishObj.animation.Play("breathing");
		}
	}

	protected override void PreviewCreation(object[] data)
	{
		FishObj.animation.Play("breathing");
	}

	public override void OnLeftMouseHit(string playerName)
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		_ = photonView;
		if (true)
		{
			if (_CurCube != BlockType.Water)
			{
				base.OnLeftMouseHit(playerName);
				return;
			}
			iTween.Stop(((Component)this).gameObject);
			((MonoBehaviour)this).CancelInvoke("OnFinishDiving");
			OnScared(WorldGameObject.INSTANCE.MainPlayer.transform.position);
		}
		else
		{
			photonView.RPC("OnScared", PhotonTargets.MasterClient, WorldGameObject.INSTANCE.MainPlayer.transform.position);
		}
	}

	public void TestCurentCube()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		_CurCube = WorldData.Instance.GetBlockType(BlockX, BlockY, BlockZ);
		_ = photonView;
		if (1 == 0)
		{
			return;
		}
		if (_CurCube != BlockType.Water)
		{
			iTween.Stop(((Component)this).gameObject);
			((MonoBehaviour)this).CancelInvoke("OnFinishDiving");
			_NewPos = Vector3.zero;
			int num = (int)((Component)this).gameObject.transform.position.x;
			int num2 = (int)((Component)this).gameObject.transform.position.y;
			int num3 = (int)((Component)this).gameObject.transform.position.z;
			if (WorldData.Instance.GetBlockType(num, num3 - 1, num2) == BlockType.Water)
			{
				_NewPos = new Vector3((float)num + 0.5f, (float)num2, (float)(num3 - 1) + 0.5f);
				_LastPos = ((Component)this).gameObject.transform.position;
			}
			else if (WorldData.Instance.GetBlockType(num, num3 + 1, num2) == BlockType.Water)
			{
				_NewPos = new Vector3((float)num + 0.5f, (float)num2, (float)(num3 + 1) + 0.5f);
				_LastPos = ((Component)this).gameObject.transform.position;
			}
			else if (WorldData.Instance.GetBlockType(num - 1, num3, num2) == BlockType.Water)
			{
				_NewPos = new Vector3((float)(num - 1) + 0.5f, (float)num2, (float)num3 + 0.5f);
				_LastPos = ((Component)this).gameObject.transform.position;
			}
			else if (WorldData.Instance.GetBlockType(num + 1, num3, num2) == BlockType.Water)
			{
				_NewPos = new Vector3((float)(num + 1) + 0.5f, (float)num2, (float)num3 + 0.5f);
				_LastPos = ((Component)this).gameObject.transform.position;
			}
			else if (WorldData.Instance.GetBlockType(num, num3, num2 - 1) == BlockType.Water)
			{
				_NewPos = new Vector3((float)num + 0.5f, (float)(num2 - 1), (float)num3 + 0.5f);
				_LastPos = ((Component)this).gameObject.transform.position;
			}
			else if (WorldData.Instance.GetBlockType(num, num3, num2 + 1) == BlockType.Water)
			{
				_NewPos = new Vector3((float)num + 0.5f, (float)(num2 + 1), (float)num3 + 0.5f);
				_LastPos = ((Component)this).gameObject.transform.position;
			}
			if (_NewPos != Vector3.zero)
			{
				photonView.RPC("MoveTo", PhotonTargets.All, _NewPos, 1f, 1f, true);
				((MonoBehaviour)this).Invoke("OnFinishDiving", 1f);
			}
			else
			{
				((MonoBehaviour)this).Invoke("OnFinishDiving", 2f);
			}
		}
		((MonoBehaviour)this).Invoke("TestCurentCube", 1f);
	}

	public void OnFinishDiving()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		_CurCube = WorldData.Instance.GetBlockType((int)((Component)this).gameObject.transform.position.x, (int)((Component)this).gameObject.transform.position.z, (int)((Component)this).gameObject.transform.position.y);
		if (_CurCube == BlockType.Water)
		{
			Vector3 positionToMove = GetPositionToMove();
			if (positionToMove == Vector3.zero)
			{
				((MonoBehaviour)this).Invoke("OnFinishDiving", 2f);
				return;
			}
			Vector3 val = positionToMove - ((Component)this).gameObject.transform.position;
			float magnitude = val.magnitude;
			photonView.RPC("MoveTo", PhotonTargets.All, positionToMove, 3f, magnitude * 4f, false);
			((MonoBehaviour)this).Invoke("OnFinishDiving", magnitude * 4f);
		}
	}

	[RPC]
	public void MoveTo(Vector3 moveTo, float lookTime, float time, bool easeTypeOut)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		iTween.Stop(((Component)this).gameObject);
		((MonoBehaviour)this).CancelInvoke("OnFinishDiving");
		iTween.EaseType easeType = ((!easeTypeOut) ? iTween.EaseType.easeInOutQuart : iTween.EaseType.easeOutExpo);
		iTween.MoveTo(((Component)this).gameObject, iTween.Hash("looktarget", moveTo, "axis", "y", "looktime", lookTime, "easetype", easeType, "position", moveTo, "time", time));
	}

	[RPC]
	public void OnScared(Vector3 myPos)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		if (_CurCube == BlockType.Water)
		{
			iTween.Stop(((Component)this).gameObject);
			((MonoBehaviour)this).CancelInvoke("OnFinishDiving");
			Vector3 positionToMove = GetPositionToMove();
			Vector3 positionToMove2 = GetPositionToMove();
			Vector3 positionToMove3 = GetPositionToMove();
			float num;
			if (positionToMove != Vector3.zero)
			{
				Vector3 val = positionToMove - myPos;
				num = val.magnitude;
			}
			else
			{
				num = 0f;
			}
			float num2 = num;
			float num3;
			if (positionToMove2 != Vector3.zero)
			{
				Vector3 val2 = positionToMove2 - myPos;
				num3 = val2.magnitude;
			}
			else
			{
				num3 = 0f;
			}
			float num4 = num3;
			float num5;
			if (positionToMove3 != Vector3.zero)
			{
				Vector3 val3 = positionToMove3 - myPos;
				num5 = val3.magnitude;
			}
			else
			{
				num5 = 0f;
			}
			float num6 = num5;
			Vector3 val4 = Vector3.zero;
			if (num2 > num4 && num2 > num6)
			{
				val4 = positionToMove;
			}
			else if (num4 > num2 && num4 > num6)
			{
				val4 = positionToMove2;
			}
			else if (num6 > num2 && num6 > num4)
			{
				val4 = positionToMove3;
			}
			if (val4 != Vector3.zero)
			{
				float num7 = Mathf.Max(Mathf.Max(num2, num4), num6);
				photonView.RPC("MoveTo", PhotonTargets.All, val4, 1f, num7, true);
				((MonoBehaviour)this).Invoke("OnFinishDiving", num7);
			}
			else
			{
				((MonoBehaviour)this).Invoke("OnFinishDiving", 2f);
			}
		}
	}

	private bool IsPointInMap(Vector3 pos)
	{
		if (pos.y < 1f)
		{
			return false;
		}
		if (pos.y > 125f)
		{
			return false;
		}
		if (pos.x < 16f)
		{
			return false;
		}
		if (pos.z < 16f)
		{
			return false;
		}
		if (pos.x > (float)WorldData.Instance.GetMaxBlockX())
		{
			return false;
		}
		if (pos.z > (float)WorldData.Instance.GetMaxBlockY())
		{
			return false;
		}
		return true;
	}

	private Vector3 GetPositionToMove()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 10; i++)
		{
			int num = _Random.Next((int)((Component)this).gameObject.transform.position.x - 3, (int)((Component)this).gameObject.transform.position.x + 3);
			int num2 = _Random.Next((int)((Component)this).gameObject.transform.position.y - 3, (int)((Component)this).gameObject.transform.position.y + 3);
			int num3 = _Random.Next((int)((Component)this).gameObject.transform.position.z - 3, (int)((Component)this).gameObject.transform.position.z + 3);
			if (IsPointInMap(new Vector3((float)num, (float)num2, (float)num3)) && WorldData.Instance.GetBlockType(num, num3, num2) == BlockType.Water)
			{
				_NewPos = new Vector3((float)num + 0.5f, (float)num2, (float)num3 + 0.5f);
				if (_LastPos != _NewPos)
				{
					_LastPos = ((Component)this).gameObject.transform.position;
					return _NewPos;
				}
			}
		}
		int num4 = (int)((Component)this).gameObject.transform.position.x;
		int num5 = (int)((Component)this).gameObject.transform.position.y;
		int num6 = (int)((Component)this).gameObject.transform.position.z;
		if (WorldData.Instance.GetBlockType(num4, num6 - 1, num5) == BlockType.Water)
		{
			_NewPos = new Vector3((float)num4 + 0.5f, (float)num5, (float)(num6 - 1) + 0.5f);
			if (_LastPos != _NewPos)
			{
				_LastPos = ((Component)this).gameObject.transform.position;
				return _NewPos;
			}
		}
		if (WorldData.Instance.GetBlockType(num4, num6 + 1, num5) == BlockType.Water)
		{
			_NewPos = new Vector3((float)num4 + 0.5f, (float)num5, (float)(num6 + 1) + 0.5f);
			if (_LastPos != _NewPos)
			{
				_LastPos = ((Component)this).gameObject.transform.position;
				return _NewPos;
			}
		}
		if (WorldData.Instance.GetBlockType(num4 - 1, num6, num5) == BlockType.Water)
		{
			_NewPos = new Vector3((float)(num4 - 1) + 0.5f, (float)num5, (float)num6 + 0.5f);
			if (_LastPos != _NewPos)
			{
				_LastPos = ((Component)this).gameObject.transform.position;
				return _NewPos;
			}
		}
		if (WorldData.Instance.GetBlockType(num4 + 1, num6, num5) == BlockType.Water)
		{
			_NewPos = new Vector3((float)(num4 + 1) + 0.5f, (float)num5, (float)num6 + 0.5f);
			if (_LastPos != _NewPos)
			{
				_LastPos = ((Component)this).gameObject.transform.position;
				return _NewPos;
			}
		}
		if (WorldData.Instance.GetBlockType(num4, num6, num5 - 1) == BlockType.Water)
		{
			_NewPos = new Vector3((float)num4 + 0.5f, (float)(num5 - 1), (float)num6 + 0.5f);
			if (_LastPos != _NewPos)
			{
				_LastPos = ((Component)this).gameObject.transform.position;
				return _NewPos;
			}
		}
		if (WorldData.Instance.GetBlockType(num4, num6, num5 + 1) == BlockType.Water)
		{
			_NewPos = new Vector3((float)num4 + 0.5f, (float)(num5 + 1), (float)num6 + 0.5f);
			if (_LastPos != _NewPos)
			{
				_LastPos = ((Component)this).gameObject.transform.position;
				return _NewPos;
			}
		}
		return Vector3.zero;
	}
}
