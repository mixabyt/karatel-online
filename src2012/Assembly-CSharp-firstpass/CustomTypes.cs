using System;
using System.IO;
using ExitGames.Client.Photon;
using UnityEngine;

internal static class CustomTypes
{
	internal static void Register()
	{
		PhotonPeer.RegisterType(typeof(Vector2), 87, SerializeVector2, DeserializeVector2);
		PhotonPeer.RegisterType(typeof(Vector3), 86, SerializeVector3, DeserializeVector3);
		PhotonPeer.RegisterType(typeof(Transform), 84, SerializeTransform, DeserializeTransform);
		PhotonPeer.RegisterType(typeof(Quaternion), 81, SerializeQuaternion, DeserializeQuaternion);
		PhotonPeer.RegisterType(typeof(PhotonPlayer), 80, SerializePhotonPlayer, DeserializePhotonPlayer);
		PhotonPeer.RegisterType(typeof(PhotonViewID), 73, SerializePhotonViewID, DeserializePhotonViewID);
	}

	private static byte[] SerializeTransform(object customobject)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected Obj, but got Unknown
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		Transform val = (Transform)customobject;
		return Protocol.Serialize(new Vector3[2] { val.position, val.eulerAngles });
	}

	private static object DeserializeTransform(byte[] serializedcustomobject)
	{
		return Protocol.Deserialize(serializedcustomobject);
	}

	private static byte[] SerializeVector3(object customobject)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = (Vector3)customobject;
		int targetOffset = 0;
		byte[] array = new byte[12];
		Protocol.Serialize(val.x, array, ref targetOffset);
		Protocol.Serialize(val.y, array, ref targetOffset);
		Protocol.Serialize(val.z, array, ref targetOffset);
		return array;
	}

	private static object DeserializeVector3(byte[] bytes)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default;
		int offset = 0;
		Protocol.Deserialize(out val.x, bytes, ref offset);
		Protocol.Deserialize(out val.y, bytes, ref offset);
		Protocol.Deserialize(out val.z, bytes, ref offset);
		return val;
	}

	private static byte[] SerializeVector2(object customobject)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = (Vector2)customobject;
		MemoryStream memoryStream = new MemoryStream(8);
		memoryStream.Write(BitConverter.GetBytes(val.x), 0, 4);
		memoryStream.Write(BitConverter.GetBytes(val.y), 0, 4);
		return memoryStream.ToArray();
	}

	private static object DeserializeVector2(byte[] bytes)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		return (object)new Vector2
		{
			x = BitConverter.ToSingle(bytes, 0),
			y = BitConverter.ToSingle(bytes, 4)
		};
	}

	private static byte[] SerializeQuaternion(object obj)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = (Quaternion)obj;
		MemoryStream memoryStream = new MemoryStream(12);
		memoryStream.Write(BitConverter.GetBytes(val.w), 0, 4);
		memoryStream.Write(BitConverter.GetBytes(val.x), 0, 4);
		memoryStream.Write(BitConverter.GetBytes(val.y), 0, 4);
		memoryStream.Write(BitConverter.GetBytes(val.z), 0, 4);
		return memoryStream.ToArray();
	}

	private static object DeserializeQuaternion(byte[] bytes)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		return (object)new Quaternion
		{
			w = BitConverter.ToSingle(bytes, 0),
			x = BitConverter.ToSingle(bytes, 4),
			y = BitConverter.ToSingle(bytes, 8),
			z = BitConverter.ToSingle(bytes, 12)
		};
	}

	private static byte[] SerializePhotonPlayer(object customobject)
	{
		int iD = ((PhotonPlayer)customobject).ID;
		return BitConverter.GetBytes(iD);
	}

	private static object DeserializePhotonPlayer(byte[] bytes)
	{
		int key = BitConverter.ToInt32(bytes, 0);
		if (PhotonNetwork.networkingPeer.mActors.ContainsKey(key))
		{
			return PhotonNetwork.networkingPeer.mActors[key];
		}
		return null;
	}

	private static byte[] SerializePhotonViewID(object customobject)
	{
		int iD = ((PhotonViewID)customobject).ID;
		return BitConverter.GetBytes(iD);
	}

	private static object DeserializePhotonViewID(byte[] bytes)
	{
		int num = BitConverter.ToInt32(bytes, 0);
		int iD = num % PhotonNetwork.MAX_VIEW_IDS;
		int num2 = num / PhotonNetwork.MAX_VIEW_IDS;
		PhotonPlayer owner = null;
		if (num2 > 0)
		{
			owner = PhotonPlayer.Find(num2);
		}
		return new PhotonViewID(iD, owner);
	}
}
