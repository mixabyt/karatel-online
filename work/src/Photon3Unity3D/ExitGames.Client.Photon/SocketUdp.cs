using System;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Threading;

namespace ExitGames.Client.Photon;

internal class SocketUdp : IPhotonSocket
{
	private Socket sock;

	private readonly object syncer = new object();

	public SocketUdp(PeerBase npeer)
		: base(npeer)
	{
		if (ReportDebugOfLevel(DebugLevel.ALL))
		{
			Listener.DebugReturn(DebugLevel.ALL, "CSharpSocket: UDP, Unity3d.");
		}
		Protocol = ConnectionProtocol.Udp;
		PollReceive = false;
	}

	public override bool Connect()
	{
		lock (syncer)
		{
			if (!base.Connect())
			{
				return false;
			}
			State = PhotonSocketState.Connecting;
			Thread thread = new Thread(DnsAndConnect);
			thread.Name = "photon dns thread";
			thread.IsBackground = true;
			thread.Start();
			return true;
		}
	}

	public override bool Disconnect()
	{
		if (ReportDebugOfLevel(DebugLevel.INFO))
		{
			EnqueueDebugReturn(DebugLevel.INFO, "CSharpSocket.Disconnect()");
		}
		State = PhotonSocketState.Disconnecting;
		lock (syncer)
		{
			if (sock != null)
			{
				try
				{
					sock.Close();
				}
				catch (Exception ex)
				{
					EnqueueDebugReturn(DebugLevel.INFO, "Exception in Disconnect(): " + ex);
				}
				sock = null;
			}
		}
		State = PhotonSocketState.Disconnected;
		return true;
	}

	public override PhotonSocketError Send(byte[] data, int length)
	{
		lock (syncer)
		{
			if (!sock.Connected)
			{
				return PhotonSocketError.Skipped;
			}
			try
			{
				sock.Send(data, 0, length, SocketFlags.None);
			}
			catch
			{
				return PhotonSocketError.Exception;
			}
		}
		return PhotonSocketError.Success;
	}

	public override PhotonSocketError Receive(out byte[] data)
	{
		data = null;
		return PhotonSocketError.NoData;
	}

	internal void DnsAndConnect()
	{
		try
		{
			lock (syncer)
			{
				sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				IPAddress ipAddress = IPhotonSocket.GetIpAddress(ServerAddress);
				sock.Connect(ipAddress, ServerPort);
				State = PhotonSocketState.Connected;
			}
		}
		catch (SecurityException ex)
		{
			if (ReportDebugOfLevel(DebugLevel.ERROR))
			{
				Listener.DebugReturn(DebugLevel.ERROR, "Connect() failed: " + ex.ToString());
			}
			HandleException(StatusCode.SecurityExceptionOnConnect);
			return;
		}
		catch (Exception ex2)
		{
			if (ReportDebugOfLevel(DebugLevel.ERROR))
			{
				Listener.DebugReturn(DebugLevel.ERROR, "Connect() failed: " + ex2.ToString());
			}
			HandleException(StatusCode.ExceptionOnConnect);
			return;
		}
		Thread thread = new Thread(ReceiveLoop);
		thread.Name = "photon receive thread";
		thread.IsBackground = true;
		thread.Start();
	}

	public void ReceiveLoop()
	{
		byte[] array = new byte[MTU];
		while (State == PhotonSocketState.Connected)
		{
			try
			{
				int length = sock.Receive(array);
				HandleReceivedDatagram(array, length, willBeReused: true);
			}
			catch (Exception ex)
			{
				if (State != PhotonSocketState.Disconnecting && State != PhotonSocketState.Disconnected)
				{
					if (ReportDebugOfLevel(DebugLevel.ERROR))
					{
						EnqueueDebugReturn(DebugLevel.ERROR, string.Concat("Receive issue. State: ", State, " Exception: ", ex));
					}
					HandleException(StatusCode.ExceptionOnReceive);
				}
			}
		}
		Disconnect();
	}
}
