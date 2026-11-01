using UnityEngine;

public class SecuredValue<T>
{
	private T _Value;

	private string _Hash;

	public T Value
	{
		get
		{
			if (!IsSecured())
			{
				Debug.Log((object)"Secure value failed, disconnecting.");
				PhotonNetwork.Disconnect();
				return default;
			}
			return _Value;
		}
	}

	public SecuredValue()
	{
		_Value = default;
		_Hash = GetHash();
	}

	public SecuredValue(T value)
	{
		_Value = value;
		_Hash = GetHash();
	}

	public bool IsSecured()
	{
		return _Hash == GetHash();
	}

	private string GetHash()
	{
		string text = _Value.ToString();
		return ("507" + text.Substring(0, text.Length / 2) + "430" + text.Substring(text.Length / 2)).GetHashCode().ToString();
	}

	public static implicit operator T(SecuredValue<T> v)
	{
		return v.Value;
	}

	public static implicit operator SecuredValue<T>(T v)
	{
		return new SecuredValue<T>(v);
	}
}
