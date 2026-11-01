using ExitGames.Client.Photon;
using UnityEngine;

internal static class CustomTypes
{
	internal static void Register()
	{
		PhotonPeer.RegisterType(typeof(Vector2), 87, SerializeVector2, DeserializeVector2);
		PhotonPeer.RegisterType(typeof(Vector3), 86, SerializeVector3, DeserializeVector3);
		PhotonPeer.RegisterType(typeof(Quaternion), 81, SerializeQuaternion, DeserializeQuaternion);
		PhotonPeer.RegisterType(typeof(PhotonPlayer), 80, SerializePhotonPlayer, DeserializePhotonPlayer);
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
		byte[] array = new byte[8];
		int targetOffset = 0;
		Protocol.Serialize(val.x, array, ref targetOffset);
		Protocol.Serialize(val.y, array, ref targetOffset);
		return array;
	}

	private static object DeserializeVector2(byte[] bytes)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = default;
		int offset = 0;
		Protocol.Deserialize(out val.x, bytes, ref offset);
		Protocol.Deserialize(out val.y, bytes, ref offset);
		return val;
	}

	private static byte[] SerializeQuaternion(object obj)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = (Quaternion)obj;
		byte[] array = new byte[16];
		int targetOffset = 0;
		Protocol.Serialize(val.w, array, ref targetOffset);
		Protocol.Serialize(val.x, array, ref targetOffset);
		Protocol.Serialize(val.y, array, ref targetOffset);
		Protocol.Serialize(val.z, array, ref targetOffset);
		return array;
	}

	private static object DeserializeQuaternion(byte[] bytes)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = default;
		int offset = 0;
		Protocol.Deserialize(out val.w, bytes, ref offset);
		Protocol.Deserialize(out val.x, bytes, ref offset);
		Protocol.Deserialize(out val.y, bytes, ref offset);
		Protocol.Deserialize(out val.z, bytes, ref offset);
		return val;
	}

	private static byte[] SerializePhotonPlayer(object customobject)
	{
		int iD = ((PhotonPlayer)customobject).ID;
		byte[] array = new byte[4];
		int targetOffset = 0;
		Protocol.Serialize(iD, array, ref targetOffset);
		return array;
	}

	private static object DeserializePhotonPlayer(byte[] bytes)
	{
		int offset = 0;
		Protocol.Deserialize(out int value, bytes, ref offset);
		if (PhotonNetwork.networkingPeer.mActors.ContainsKey(value))
		{
			return PhotonNetwork.networkingPeer.mActors[value];
		}
		return null;
	}
}
