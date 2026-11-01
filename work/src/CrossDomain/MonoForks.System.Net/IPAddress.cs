using System;
using System.Globalization;
using MonoForks.System.Net.Sockets;

namespace MonoForks.System.Net;

[Serializable]
public class IPAddress
{
	private long m_Address;

	private AddressFamily m_Family;

	private ushort[] m_Numbers;

	private long m_ScopeId;

	public static readonly IPAddress Any = new IPAddress(0L);

	public static readonly IPAddress Broadcast = Parse("255.255.255.255");

	public static readonly IPAddress Loopback = Parse("127.0.0.1");

	public static readonly IPAddress None = Parse("255.255.255.255");

	public static readonly IPAddress IPv6Any = ParseIPV6("::");

	public static readonly IPAddress IPv6Loopback = ParseIPV6("::1");

	public static readonly IPAddress IPv6None = ParseIPV6("::");

	private int m_HashCode;

	[Obsolete("This property is obsolete. Use GetAddressBytes.")]
	public long Address
	{
		get
		{
			if (m_Family != AddressFamily.InterNetwork)
			{
				throw new Exception("The attempted operation is not supported for the type of object referenced");
			}
			return m_Address;
		}
		set
		{
			if (m_Family != AddressFamily.InterNetwork)
			{
				throw new Exception("The attempted operation is not supported for the type of object referenced");
			}
			m_Address = value;
		}
	}

	internal long InternalIPv4Address => m_Address;

	public long ScopeId
	{
		get
		{
			if (m_Family != AddressFamily.InterNetworkV6)
			{
				throw new Exception("The attempted operation is not supported for the type of object referenced");
			}
			return m_ScopeId;
		}
		set
		{
			if (m_Family != AddressFamily.InterNetworkV6)
			{
				throw new Exception("The attempted operation is not supported for the type of object referenced");
			}
			m_ScopeId = value;
		}
	}

	public AddressFamily AddressFamily => m_Family;

	public IPAddress(long addr)
	{
		m_Address = addr;
		m_Family = AddressFamily.InterNetwork;
	}

	public IPAddress(byte[] address)
	{
		if (address == null)
		{
			throw new ArgumentNullException("address");
		}
		int num = address.Length;
		if (num != 16)
		{
			throw new ArgumentException("address");
		}
		if (num == 16)
		{
			m_Numbers = new ushort[8];
			Buffer.BlockCopy(address, 0, m_Numbers, 0, 16);
			m_Family = AddressFamily.InterNetworkV6;
			m_ScopeId = 0L;
		}
		else
		{
			m_Address = (uint)(address[3] << 24) + (address[2] << 16) + (address[1] << 8) + (int)address[0];
			m_Family = AddressFamily.InterNetwork;
		}
	}

	public IPAddress(byte[] address, long scopeId)
	{
		if (address == null)
		{
			throw new ArgumentNullException("address");
		}
		if (address.Length != 16)
		{
			throw new ArgumentException("address");
		}
		m_Numbers = new ushort[8];
		Buffer.BlockCopy(address, 0, m_Numbers, 0, 16);
		m_Family = AddressFamily.InterNetworkV6;
		m_ScopeId = scopeId;
	}

	internal IPAddress(ushort[] address, long scopeId)
	{
		m_Numbers = address;
		for (int i = 0; i < 8; i++)
		{
			m_Numbers[i] = (ushort)HostToNetworkOrder((short)m_Numbers[i]);
		}
		m_Family = AddressFamily.InterNetworkV6;
		m_ScopeId = scopeId;
	}

	private static short SwapShort(short number)
	{
		return (short)(((number >> 8) & 0xFF) | ((number << 8) & 0xFF00));
	}

	private static int SwapInt(int number)
	{
		return ((number >> 24) & 0xFF) | ((number >> 8) & 0xFF00) | ((number << 8) & 0xFF0000) | (number << 24);
	}

	private static long SwapLong(long number)
	{
		return ((number >> 56) & 0xFF) | ((number >> 40) & 0xFF00) | ((number >> 24) & 0xFF0000) | ((number >> 8) & 0xFF000000u) | ((number << 8) & 0xFF00000000L) | ((number << 24) & 0xFF0000000000L) | ((number << 40) & 0xFF000000000000L) | (number << 56);
	}

	public static short HostToNetworkOrder(short host)
	{
		if (!BitConverter.IsLittleEndian)
		{
			return host;
		}
		return SwapShort(host);
	}

	public static int HostToNetworkOrder(int host)
	{
		if (!BitConverter.IsLittleEndian)
		{
			return host;
		}
		return SwapInt(host);
	}

	public static long HostToNetworkOrder(long host)
	{
		if (!BitConverter.IsLittleEndian)
		{
			return host;
		}
		return SwapLong(host);
	}

	public static short NetworkToHostOrder(short network)
	{
		if (!BitConverter.IsLittleEndian)
		{
			return network;
		}
		return SwapShort(network);
	}

	public static int NetworkToHostOrder(int network)
	{
		if (!BitConverter.IsLittleEndian)
		{
			return network;
		}
		return SwapInt(network);
	}

	public static long NetworkToHostOrder(long network)
	{
		if (!BitConverter.IsLittleEndian)
		{
			return network;
		}
		return SwapLong(network);
	}

	public static IPAddress Parse(string ipString)
	{
		if (TryParse(ipString, out var address))
		{
			return address;
		}
		throw new FormatException("An invalid IP address was specified.");
	}

	internal static bool TryParse(string ipString, out IPAddress address)
	{
		if (ipString == null)
		{
			throw new ArgumentNullException("ipString");
		}
		if ((address = ParseIPV4(ipString)) == null && (address = ParseIPV6(ipString)) == null)
		{
			return false;
		}
		return true;
	}

	private static IPAddress ParseIPV4(string ip)
	{
		int num = ip.IndexOf(' ');
		if (num != -1)
		{
			ip = ip.Substring(0, num);
		}
		if (ip.Length == 0 || ip[ip.Length - 1] == '.')
		{
			return null;
		}
		string[] array = ip.Split('.');
		if (array.Length > 4)
		{
			return null;
		}
		try
		{
			long num2 = 0L;
			long num3 = 0L;
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				if (3 <= text.Length && text.Length <= 4 && text[0] == '0' && (text[1] == 'x' || text[1] == 'X'))
				{
					num3 = ((text.Length != 3) ? ((int)(byte)((Uri.FromHex(text[2]) << 4) | Uri.FromHex(text[3]))) : ((int)(byte)Uri.FromHex(text[2])));
				}
				else
				{
					if (text.Length == 0)
					{
						return null;
					}
					if (text[0] == '0')
					{
						num3 = 0L;
						for (int j = 1; j < text.Length; j++)
						{
							if ('0' <= text[j] && text[j] <= '7')
							{
								num3 = (num3 << 3) + (int)text[j] - 48;
								continue;
							}
							return null;
						}
					}
					else
					{
						num3 = long.Parse(text, NumberStyles.None);
					}
				}
				if (i == array.Length - 1)
				{
					i = 3;
				}
				else if (num3 > 255)
				{
					return null;
				}
				int num4 = 0;
				while (num3 > 0)
				{
					num2 |= (num3 & 0xFF) << (i - num4 << 3);
					num4++;
					num3 /= 256;
				}
			}
			return new IPAddress(num2);
		}
		catch (Exception)
		{
			return null;
		}
	}

	private static IPAddress ParseIPV6(string ip)
	{
		if (IPv6Address.TryParse(ip, out var result))
		{
			return new IPAddress(result.Address, result.ScopeId);
		}
		return null;
	}

	public byte[] GetAddressBytes()
	{
		if (m_Family == AddressFamily.InterNetworkV6)
		{
			byte[] array = new byte[16];
			Buffer.BlockCopy(m_Numbers, 0, array, 0, 16);
			return array;
		}
		return new byte[4]
		{
			(byte)(m_Address & 0xFF),
			(byte)((m_Address >> 8) & 0xFF),
			(byte)((m_Address >> 16) & 0xFF),
			(byte)(m_Address >> 24)
		};
	}

	public static bool IsLoopback(IPAddress addr)
	{
		if (addr.m_Family == AddressFamily.InterNetwork)
		{
			return (addr.m_Address & 0xFF) == 127;
		}
		for (int i = 0; i < 6; i++)
		{
			if (addr.m_Numbers[i] != 0)
			{
				return false;
			}
		}
		return NetworkToHostOrder((short)addr.m_Numbers[7]) == 1;
	}

	public override string ToString()
	{
		if (m_Family == AddressFamily.InterNetwork)
		{
			return ToString(m_Address);
		}
		ushort[] array = m_Numbers.Clone() as ushort[];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (ushort)NetworkToHostOrder((short)array[i]);
		}
		IPv6Address pv6Address = new IPv6Address(array);
		pv6Address.ScopeId = ScopeId;
		return pv6Address.ToString();
	}

	private static string ToString(long addr)
	{
		return (addr & 0xFF) + "." + ((addr >> 8) & 0xFF) + "." + ((addr >> 16) & 0xFF) + "." + ((addr >> 24) & 0xFF);
	}

	public override bool Equals(object other)
	{
		if (other is IPAddress)
		{
			IPAddress iPAddress = other as IPAddress;
			if (AddressFamily != iPAddress.AddressFamily)
			{
				return false;
			}
			if (AddressFamily == AddressFamily.InterNetwork)
			{
				return m_Address == iPAddress.m_Address;
			}
			ushort[] numbers = iPAddress.m_Numbers;
			for (int i = 0; i < 8; i++)
			{
				if (m_Numbers[i] != numbers[i])
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}

	public override int GetHashCode()
	{
		if (m_Family == AddressFamily.InterNetwork)
		{
			return (int)m_Address;
		}
		return Hash((m_Numbers[0] << 16) + m_Numbers[1], (m_Numbers[2] << 16) + m_Numbers[3], (m_Numbers[4] << 16) + m_Numbers[5], (m_Numbers[6] << 16) + m_Numbers[7]);
	}

	private static int Hash(int i, int j, int k, int l)
	{
		return i ^ ((j << 13) | (j >> 19)) ^ ((k << 26) | (k >> 6)) ^ ((l << 7) | (l >> 25));
	}
}
