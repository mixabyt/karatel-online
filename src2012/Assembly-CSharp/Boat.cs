using UnityEngine;

public class Boat : EntityBase
{
	public const float MaxSpeed = 6f;

	public const float MaxBackSpeed = 1f;

	public const float AccelerateSpeed = 1f;

	public const float BackSpeed = 0.5f;

	public const float RotationAngleSpeed = 90f;

	public const float FrictionSpeed = 0.7f;

	public const float EmersionSpeed = 1.5f;

	private bool _InWater;

	private bool _UnderWater;

	private GameObject _Model;

	private Transform _PlayerPosition;

	private Transform _PlayerExit;

	private float _CurSpeed;

	private GameObject _Player;

	private CameraController _PlayerCameraController;

	private MouseLook _PlayerMouseLookHor;

	private Vector3 _AwakePosition = Vector3.zero;

	private Vector3 _Position = Vector3.zero;

	private Quaternion _Rotation = Quaternion.identity;

	public GameObject Player => _Player;

	public Vector3 AwakePosition
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _AwakePosition;
		}
	}

	public Boat()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
	}

	public static void KickPlayer(GameObject player)
	{
		if ((Object)(object)player == (Object)null)
		{
			return;
		}
		Boat[] array = Object.FindObjectsOfType(typeof(Boat)) as Boat[];
		Boat[] array2 = array;
		foreach (Boat boat in array2)
		{
			if ((Object)(object)boat._Player == (Object)(object)player)
			{
				boat.PlayerExit();
				break;
			}
		}
	}

	protected void Awake()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		_Model = ((Component)((Component)this).transform.FindChild("model")).gameObject;
		_PlayerPosition = ((Component)this).transform.FindChild("player_position");
		_PlayerExit = ((Component)this).transform.FindChild("player_exit");
		_AwakePosition = ((Component)this).transform.position;
		_Position = ((Component)this).transform.position;
		((Component)this).rigidbody.isKinematic = true;
	}

	protected override void Creation(object[] data)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		bool flag = WorldData.Instance.GetBlockType(BlockX, BlockY, (int)(((Component)this).transform.position.y - 0.01f)) == BlockType.Water;
		((Component)this).rigidbody.isKinematic = !flag;
	}

	protected override void PreviewCreation(object[] data)
	{
		Object.Destroy((Object)(object)((Component)((Component)this).transform.FindChild("colliders")).gameObject);
	}

	protected override void Destruction()
	{
		if ((Object)(object)_Player != (Object)null && (Object)(object)_Player == (Object)(object)WorldGameObject.INSTANCE.MainPlayer)
		{
			PlayerExit();
		}
	}

	private void FixedUpdate()
	{
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		if (IsPreview || !WorldGameObject.INSTANCE.IsWorldGenerated)
		{
			return;
		}
		if (_InWater)
		{
			bool flag = WorldData.Instance.GetBlockType(BlockX, BlockY, (int)((Component)this).transform.position.y) == BlockType.Water;
			if (!_UnderWater && flag)
			{
				_UnderWater = true;
			}
			else if (_UnderWater && !flag)
			{
				_UnderWater = false;
				if (!((Component)this).rigidbody.isKinematic)
				{
					((Component)this).transform.position = new Vector3(((Component)this).transform.position.x, (float)(int)((Component)this).transform.position.y, ((Component)this).transform.position.z);
					((Component)this).rigidbody.position = ((Component)this).transform.position;
					((Component)this).rigidbody.velocity = new Vector3(((Component)this).rigidbody.velocity.x, 0f, ((Component)this).rigidbody.velocity.z);
				}
			}
		}
		bool flag2 = WorldData.Instance.GetBlockType(BlockX, BlockY, (int)(((Component)this).transform.position.y - 0.01f)) == BlockType.Water;
		if (flag2 && !_InWater)
		{
			_InWater = true;
			_UnderWater = false;
			if (!((Component)this).rigidbody.isKinematic)
			{
				((Component)this).rigidbody.useGravity = false;
			}
		}
		else if (!flag2 && _InWater)
		{
			_InWater = false;
			_UnderWater = false;
			if (!((Component)this).rigidbody.isKinematic)
			{
				((Component)this).rigidbody.useGravity = true;
			}
		}
		if (!((Component)this).rigidbody.isKinematic && _InWater)
		{
			bool flag3 = false;
			if ((Object)(object)_Player != (Object)null && (Object)(object)_Player == (Object)(object)WorldGameObject.INSTANCE.MainPlayer)
			{
				if (Input.GetKey((KeyCode)119))
				{
					_CurSpeed += 1f * Time.fixedDeltaTime;
					flag3 = true;
				}
				if (Input.GetKey((KeyCode)115))
				{
					if (_CurSpeed > 0f)
					{
						_CurSpeed = Mathf.Lerp(_CurSpeed, 0f, Time.fixedDeltaTime * 2f);
						if (_CurSpeed < 0.1f)
						{
							_CurSpeed = 0f;
						}
					}
					else
					{
						_CurSpeed -= 0.5f * Time.fixedDeltaTime;
					}
					flag3 = true;
				}
				_CurSpeed = Mathf.Clamp(_CurSpeed, -1f, 6f);
			}
			if (!flag3)
			{
				_CurSpeed = Mathf.MoveTowards(_CurSpeed, 0f, 0.7f * Time.fixedDeltaTime);
				if (_CurSpeed < 0.1f && _CurSpeed > -0.1f)
				{
					_CurSpeed = 0f;
				}
			}
			if (_CurSpeed != 0f)
			{
				((Component)this).rigidbody.velocity = ((Component)this).transform.forward * _CurSpeed;
			}
			else if (((Component)this).rigidbody.velocity != Vector3.zero)
			{
				((Component)this).rigidbody.velocity = Vector3.zero;
			}
			if (((Component)this).rigidbody.angularVelocity != Vector3.zero)
			{
				((Component)this).rigidbody.angularVelocity = Vector3.zero;
			}
			if (_UnderWater)
			{
				((Component)this).rigidbody.velocity = new Vector3(((Component)this).rigidbody.velocity.x, 1.5f, ((Component)this).rigidbody.velocity.z);
			}
		}
		if (!((Component)this).rigidbody.isKinematic && (Object)(object)_Player != (Object)null && (Object)(object)_Player == (Object)(object)WorldGameObject.INSTANCE.MainPlayer)
		{
			float num = 0f;
			if (Input.GetKey((KeyCode)97))
			{
				num += -90f * Time.fixedDeltaTime;
			}
			if (Input.GetKey((KeyCode)100))
			{
				num += 90f * Time.fixedDeltaTime;
			}
			if (num != 0f)
			{
				((Component)this).transform.Rotate(0f, num, 0f, (Space)1);
				if (_PlayerCameraController.IsThirdPerson)
				{
					_PlayerMouseLookHor.RotationX += num;
				}
			}
		}
		FixWorldBounds();
		if ((Object)(object)_Player != (Object)null)
		{
			_Player.transform.position = _PlayerPosition.position;
		}
	}

	private void OnCollisionStay()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 velocity = ((Component)this).rigidbody.velocity;
		_CurSpeed = velocity.magnitude;
	}

	protected override void Updating()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		_ = photonView;
		if (1 == 0)
		{
			((Component)this).transform.position = Vector3.Lerp(((Component)this).transform.position, _Position, Time.deltaTime * 5f);
			((Component)this).transform.rotation = Quaternion.Lerp(((Component)this).transform.rotation, _Rotation, Time.deltaTime * 5f);
		}
		if (_InWater && !_UnderWater)
		{
			float z = _Model.transform.localEulerAngles.z;
			bool flag = (Object)(object)_Player != (Object)null && (Object)(object)_Player == (Object)(object)WorldGameObject.INSTANCE.MainPlayer;
			if (flag && Input.GetKey((KeyCode)97))
			{
				z = Mathf.MoveTowardsAngle(z, 15f, Time.deltaTime * 50f);
			}
			else
			{
				z = ((!flag || !Input.GetKey((KeyCode)100)) ? Mathf.MoveTowardsAngle(z, Mathf.PingPong(Time.time * 6f, 20f) - 10f, Time.deltaTime * 30f) : Mathf.MoveTowardsAngle(z, -15f, Time.deltaTime * 50f));
			}
			_Model.transform.localRotation = Quaternion.Euler(0f, 0f, z);
		}
		else
		{
			_Model.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		}
	}

	public override void OnButtonF(string playerName)
	{
		GameObject mainPlayer = WorldGameObject.INSTANCE.MainPlayer;
		if ((Object)(object)_Player != (Object)null)
		{
			if (!((Object)(object)_Player != (Object)(object)mainPlayer))
			{
				PlayerExit();
			}
		}
		else
		{
			KickPlayer(mainPlayer);
			PlayerEnter(mainPlayer);
		}
	}

	protected void PlayerEnter(GameObject player, bool others = false)
	{
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Expected Obj, but got Unknown
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		_Player = player;
		_Player.GetComponent<PlayerNetwork>().InVehicle = true;
		_PlayerCameraController = _Player.GetComponent<CameraController>();
		_PlayerMouseLookHor = _Player.GetComponent<MouseLook>();
		if ((Object)(object)_Player.GetComponent<CharacterController>() != (Object)null)
		{
			((Collider)_Player.GetComponent<CharacterController>()).enabled = false;
		}
		if ((Object)(object)_Player.GetComponent<PlayerMotor>() != (Object)null)
		{
			((Behaviour)_Player.GetComponent<PlayerMotor>()).enabled = false;
		}
		if ((Object)(object)_Player.GetComponent<PlayerInput>() != (Object)null)
		{
			_Player.GetComponent<PlayerInput>().SetMovement(move: false);
		}
		_Player.transform.position = _PlayerPosition.position;
		GameObject val = new GameObject("collider");
		val.transform.parent = ((Component)this).transform;
		CapsuleCollider val2 = val.AddComponent<CapsuleCollider>();
		val2.height = _Player.GetComponent<CharacterController>().height;
		val2.radius = _Player.GetComponent<CharacterController>().radius;
		val2.center = _Player.GetComponent<CharacterController>().center;
		val.transform.position = _Player.transform.position;
		val.transform.localScale = _Player.transform.localScale;
		if (!others)
		{
			((Component)this).rigidbody.isKinematic = false;
			((Component)this).rigidbody.useGravity = !_InWater && !_UnderWater;
			photonView.isForcedSending = 1;
			photonView.RPC("PlayerEnterOthers", PhotonTargets.Others, PhotonNetwork.player);
		}
	}

	[RPC]
	protected void PlayerEnterOthers(PhotonPlayer owner)
	{
		((Component)this).rigidbody.isKinematic = true;
		((Component)this).rigidbody.useGravity = false;
		photonView.isForcedSending = -1;
		PlayerEnter(WorldGameObject.INSTANCE.FindPlayer(owner).avatar, others: true);
	}

	private void PlayerExit(bool others = false)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		_Player.transform.position = ((Component)_PlayerExit).transform.position;
		if ((Object)(object)_Player.GetComponent<CharacterController>() != (Object)null)
		{
			((Collider)_Player.GetComponent<CharacterController>()).enabled = true;
		}
		if ((Object)(object)_Player.GetComponent<PlayerMotor>() != (Object)null)
		{
			((Behaviour)_Player.GetComponent<PlayerMotor>()).enabled = true;
		}
		if ((Object)(object)_Player.GetComponent<PlayerInput>() != (Object)null)
		{
			_Player.GetComponent<PlayerInput>().SetMovement(move: true);
		}
		Transform val = ((Component)this).transform.FindChild("collider");
		Object.Destroy((Object)(object)((Component)val).gameObject);
		_Player.GetComponent<PlayerNetwork>().InVehicle = false;
		_PlayerCameraController = null;
		_PlayerMouseLookHor = null;
		_Player = null;
		if (!others)
		{
			photonView.RPC("PlayerExitOthers", PhotonTargets.Others);
		}
	}

	[RPC]
	protected void PlayerExitOthers()
	{
		PlayerExit(others: true);
	}

	protected virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (stream.isWriting)
		{
			stream.SendNext(((Component)this).transform.position);
			stream.SendNext(((Component)this).transform.rotation);
		}
		else
		{
			_Position = (Vector3)stream.ReceiveNext();
			_Rotation = (Quaternion)stream.ReceiveNext();
		}
	}
}
