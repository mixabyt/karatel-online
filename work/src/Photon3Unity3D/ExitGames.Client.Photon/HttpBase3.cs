#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace ExitGames.Client.Photon;

internal class HttpBase3 : PeerBase
{
	public delegate int GetLocalMsTimestampDelegate();

	internal enum MessageType
	{
		CONNECT,
		DISCONNECT,
		NORMAL
	}

	private class AsyncRequestState
	{
		private Stopwatch Watch;

		public MessageType type;

		public int rerequested = 0;

		public bool restarting = false;

		public int id = 0;

		private int ElapsedTime => (int)Watch.ElapsedMilliseconds;

		public WWW Request { get; set; }

		public byte[] OutgoingData { get; set; }

		public bool IsDisconnect => type == MessageType.DISCONNECT;

		public bool IsTimedOut => ElapsedTime > 10000;

		public AsyncRequestState()
		{
			Watch = new Stopwatch();
			Watch.Start();
		}

		public void Abort()
		{
			Request.Dispose();
			Request = null;
		}
	}

	internal const int TCP_HEADER_LEN = 7;

	private const int MAX_GROSS_ERRORS = 3;

	private static readonly byte[] pingData = new byte[5] { 240, 0, 0, 0, 0 };

	private string HttpPeerID;

	private int _challengId = 0;

	private string UrlParameters;

	private long lastPingTimeStamp;

	private List<byte[]> incomingList = new List<byte[]>();

	private int _lastSendAck = 0;

	private bool _sendAck = false;

	private InvocationCache _invocationCache = new InvocationCache();

	private int _minConnectionsCount = 2;

	private int _maxConnectionsCount = 4;

	private static Random _rnd = new Random();

	private LinkedList<AsyncRequestState> _requestCache = new LinkedList<AsyncRequestState>();

	private int _stateIdBase = -1;

	private MemoryStream outgoingStream = null;

	private int _grossErrorCount = 0;

	internal GetLocalMsTimestampDelegate GetLocalMsTimestamp = () => Environment.TickCount;

	internal static readonly byte[] messageHeader = new byte[9] { 251, 0, 0, 0, 0, 0, 0, 243, 2 };

	public override string PeerID => HttpPeerID;

	public bool UseGet { get; set; }

	public int currentRequestCount => _requestCache.Count;

	public int totalRequestCount => _stateIdBase;

	internal override int QueuedIncomingCommandsCount => 0;

	internal override int QueuedOutgoingCommandsCount => 0;

	internal HttpBase3()
	{
		PeerBase.peerCount++;
		_challengId = _rnd.Next();
		InitOnce();
	}

	internal HttpBase3(IPhotonPeerListener listener)
		: this()
	{
		Listener = listener;
	}

	internal void Request(byte[] data, string urlParamter)
	{
		Request(data, urlParamter, MessageType.NORMAL);
	}

	internal void Request(byte[] data, string urlParameter, MessageType type)
	{
		int id = Interlocked.Increment(ref _stateIdBase);
		_addAckId(ref urlParameter);
		AsyncRequestState asyncRequestState = new AsyncRequestState();
		asyncRequestState.OutgoingData = data;
		asyncRequestState.type = type;
		asyncRequestState.id = id;
		AsyncRequestState asyncRequestState2 = asyncRequestState;
		_requestCache.AddLast(asyncRequestState2);
		Request(asyncRequestState2, urlParameter);
	}

	private void Request(AsyncRequestState state, string urlParamter)
	{
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Expected Obj, but got Unknown
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Expected Obj, but got Unknown
		urlParamter = ((!UseGet) ? (urlParamter + $"&seq={state.id}") : (urlParamter + $"&seq={state.id}&data={Convert.ToBase64String(state.OutgoingData, Base64FormattingOptions.None)}"));
		if (state.type == MessageType.DISCONNECT)
		{
			urlParamter += "&dis";
		}
		if ((int)debugOut >= 5)
		{
			Listener.DebugReturn(DebugLevel.ALL, $"url for request is {urlParamter}");
		}
		string text = ServerAddress + urlParamter + HttpUrlParameters;
		if (UseGet)
		{
			state.Request = new WWW(text);
		}
		else
		{
			state.Request = new WWW(text, GetPOSTRequestData(state));
		}
	}

	private void _addAckId(ref string urlParamter)
	{
		if (_sendAck)
		{
			_sendAck = false;
			urlParamter = urlParamter + "&ack=" + _lastSendAck;
			if ((int)debugOut >= 5)
			{
				Listener.DebugReturn(DebugLevel.ALL, $"ack sent for id {_lastSendAck}, pid={HttpPeerID}, cid={_challengId}");
			}
		}
	}

	private static void writeLength(byte[] target, int value, int targetOffset)
	{
		target[targetOffset++] = (byte)(value >> 24);
		target[targetOffset++] = (byte)(value >> 16);
		target[targetOffset++] = (byte)(value >> 8);
		target[targetOffset++] = (byte)value;
	}

	private byte[] GetPOSTRequestData(AsyncRequestState state)
	{
		return state.OutgoingData;
	}

	private static int _getStatusCodeFromResponse(byte[] response, HttpBase3 peer)
	{
		int num = 0;
		if (response.Length >= 4)
		{
			num |= response[0] << 24;
			num |= response[1] << 16;
			num |= response[2] << 8;
			num |= response[3];
		}
		return num;
	}

	private void _webExceptionHandler(AsyncRequestState state, WWW request)
	{
		if (state.IsDisconnect)
		{
			return;
		}
		if ((peerConnectionState != ConnectionStateValue.Disconnecting || peerConnectionState != ConnectionStateValue.Disconnected) && !state.restarting && (int)debugOut >= 1)
		{
			Listener.DebugReturn(DebugLevel.ERROR, $"Request {state.id} for pid={HttpPeerID} cid={_challengId} failed with error: {request.error}");
		}
		if (peerConnectionState == ConnectionStateValue.Connecting)
		{
			EnqueueErrorDisconnect(StatusCode.ExceptionOnConnect);
		}
		else
		{
			if (peerConnectionState != ConnectionStateValue.Connected)
			{
				return;
			}
			int responseStatus = getResponseStatus(request);
			if (gotIgnoreStatus(responseStatus))
			{
				if ((int)debugOut >= 5)
				{
					Listener.DebugReturn(DebugLevel.ALL, "got statues which we ignore");
				}
			}
			else
			{
				EnqueueErrorDisconnect(StatusCode.DisconnectByServer);
			}
		}
	}

	private static bool gotIgnoreStatus(int responseStatus)
	{
		return responseStatus == 404;
	}

	private int getResponseStatus(WWW request)
	{
		int result = -1;
		if (request.error != null)
		{
			int num = request.error.IndexOf(' ');
			if (num != -1)
			{
				string text = request.error.Substring(0, num + 1);
				text.Trim();
				int.TryParse(text, out result);
			}
			return result;
		}
		Listener.DebugReturn(DebugLevel.WARNING, $"failed to parse status {result}");
		return -1;
	}

	private void GetResponse(AsyncRequestState state)
	{
		WWW request = state.Request;
		if (request.error != null)
		{
			_webExceptionHandler(state, request);
			return;
		}
		try
		{
			if (request.bytes != null && request.bytes.Length > 0)
			{
				ReceiveIncomingCommands(request.bytes, request.bytes.Length);
			}
			Interlocked.Exchange(ref _grossErrorCount, 0);
		}
		catch (Exception ex)
		{
			Listener.DebugReturn(DebugLevel.ERROR, string.Format("exception: msg - {0}\n Stack\n{1}", ex.StackTrace));
			return;
		}
		_checkAckCondition();
		state.Request.Dispose();
		state.OutgoingData = null;
	}

	private void _checkAckCondition()
	{
		if (_requestCache.Count != 0 && _stateIdBase - _maxConnectionsCount - _lastSendAck > 5)
		{
			int num = _requestCache.Min((AsyncRequestState request) => request.id);
			if (num != int.MaxValue && num - _lastSendAck > 5)
			{
				_lastSendAck = num - 2;
				_sendAck = true;
			}
		}
	}

	private void CancelRequests()
	{
		foreach (AsyncRequestState item in _requestCache)
		{
			item.Abort();
		}
		_requestCache.Clear();
	}

	private void Reset()
	{
		_lastSendAck = 0;
		_stateIdBase = -1;
		_sendAck = false;
		_invocationCache.Reset();
		HttpPeerID = "";
		_requestCache.Clear();
	}

	internal override bool Connect(string serverAddress, string appID)
	{
		if (peerConnectionState != ConnectionStateValue.Disconnected)
		{
			if ((int)debugOut >= 2)
			{
				Listener.DebugReturn(DebugLevel.WARNING, "Connect() called while peerConnectionState != Disconnected. Nothing done.");
			}
			return false;
		}
		if (string.IsNullOrEmpty(serverAddress) || !serverAddress.StartsWith("http", ignoreCase: true, null))
		{
			Listener.DebugReturn(DebugLevel.ERROR, "Connect() with RHTTP failed. ServerAddress must include 'http://' or 'https://' prefix. Was: " + serverAddress);
			return false;
		}
		outgoingStream = new MemoryStream(PeerBase.outgoingStreamBufferSize);
		Reset();
		peerConnectionState = ConnectionStateValue.Connecting;
		ServerAddress = serverAddress;
		UrlParameters = "?init&cid=";
		UrlParameters += _challengId;
		if (appID == null)
		{
			appID = "NUnit";
		}
		UrlParameters = UrlParameters + "&app=" + appID;
		UrlParameters += "&clientversion=3.2.2.2";
		UrlParameters += "&protocol=GpBinaryV16";
		lastPingTimeStamp = GetLocalMsTimestamp();
		Request(null, UrlParameters, MessageType.CONNECT);
		return true;
	}

	internal override void Disconnect()
	{
		if (peerConnectionState != ConnectionStateValue.Disconnected && peerConnectionState != ConnectionStateValue.Disconnecting)
		{
			peerConnectionState = ConnectionStateValue.Disconnecting;
			CancelRequests();
			Request(null, UrlParameters, MessageType.DISCONNECT);
			Disconnected();
		}
	}

	private void EnqueueErrorDisconnect(StatusCode statusCode)
	{
		lock (this)
		{
			if (peerConnectionState != ConnectionStateValue.Connected && peerConnectionState != ConnectionStateValue.Connecting)
			{
				return;
			}
			peerConnectionState = ConnectionStateValue.Disconnecting;
		}
		if ((int)debugOut >= 3)
		{
			Listener.DebugReturn(DebugLevel.INFO, $"pid={HttpPeerID} cid={_challengId} is disconnected");
		}
		EnqueueStatusCallback(statusCode);
		EnqueueActionForDispatch(() =>
		{
			Disconnected();
		});
	}

	internal void Disconnected()
	{
		InitPeerBase();
		Listener.OnStatusChanged(StatusCode.Disconnect);
	}

	internal override void InitPeerBase()
	{
		peerConnectionState = ConnectionStateValue.Disconnected;
	}

	internal override void FetchServerTimestamp()
	{
	}

	internal override bool DispatchIncomingCommands()
	{
		CheckRequests();
		return DispatchIncommingActions();
	}

	private bool CheckRequests()
	{
		LinkedListNode<AsyncRequestState> linkedListNode = _requestCache.First;
		while (linkedListNode != null)
		{
			LinkedListNode<AsyncRequestState> linkedListNode2 = linkedListNode;
			linkedListNode = linkedListNode.Next;
			if (linkedListNode2.Value.Request.isDone)
			{
				GetResponse(linkedListNode2.Value);
				_requestCache.Remove(linkedListNode2);
			}
			else if (linkedListNode2.Value.IsTimedOut)
			{
				AsyncRequestState value = linkedListNode2.Value;
				value.Abort();
				if ((int)debugOut >= 2)
				{
					Listener.DebugReturn(DebugLevel.WARNING, $"Request {value.id} for pid={HttpPeerID} cid={_challengId} aborted by timeout");
				}
				EnqueueErrorDisconnect(StatusCode.TimeoutDisconnect);
				_requestCache.Remove(linkedListNode2);
				return true;
			}
		}
		return true;
	}

	private bool DispatchIncommingActions()
	{
		lock (ActionQueue)
		{
			while (ActionQueue.Count > 0)
			{
				MyAction myAction = ActionQueue.Dequeue();
				myAction();
			}
		}
		byte[] array;
		lock (incomingList)
		{
			if (incomingList.Count <= 0)
			{
				return false;
			}
			array = incomingList[0];
			incomingList.RemoveAt(0);
		}
		ByteCountCurrentDispatch = array.Length + 3;
		if (array.Length < 2)
		{
			Listener.DebugReturn(DebugLevel.WARNING, $"message has length less then 2. data {SupportClass.ByteArrayToString(array)}");
		}
		return DeserializeMessageAndCallback(array);
	}

	private void _sendPing()
	{
		if (GetLocalMsTimestamp() - lastPingTimeStamp > timePingInterval)
		{
			lastPingTimeStamp = GetLocalMsTimestamp();
			int targetOffset = 1;
			Protocol.Serialize(SupportClass.GetTickCount(), pingData, ref targetOffset);
			Request(pingData, UrlParameters, MessageType.NORMAL);
		}
		int num = _minConnectionsCount - _requestCache.Count;
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				Request(null, UrlParameters, MessageType.NORMAL);
			}
		}
	}

	internal override bool SendOutgoingCommands()
	{
		if (peerConnectionState != ConnectionStateValue.Connected)
		{
			return false;
		}
		if (_requestCache.Count >= _maxConnectionsCount)
		{
			return false;
		}
		if (outgoingStream.Length > 0)
		{
			Request(outgoingStream.ToArray(), UrlParameters);
			outgoingStream.SetLength(0L);
			outgoingStream.Position = 0L;
		}
		_sendPing();
		return false;
	}

	private static int _readMessageHeader(BinaryReader br)
	{
		int result = (br.ReadByte() << 24) | (br.ReadByte() << 16) | (br.ReadByte() << 8) | br.ReadByte();
		byte b = br.ReadByte();
		byte b2 = br.ReadByte();
		return result;
	}

	internal override void ReceiveIncomingCommands(byte[] inBuff, int dataLen)
	{
		if (peerConnectionState == ConnectionStateValue.Connecting)
		{
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(inBuff, 4, 4);
			}
			HttpPeerID = BitConverter.ToInt32(inBuff, 4).ToString();
			UrlParameters = "?pid=" + HttpPeerID + "&cid=" + _challengId;
			peerConnectionState = ConnectionStateValue.Connected;
			EnqueueActionForDispatch(InitCallback);
		}
		else
		{
			timestampOfLastReceive = GetLocalMsTimestamp();
			bytesIn += inBuff.Length + 7;
			Array.Reverse(inBuff, 0, 4);
			BinaryReader br = new BinaryReader(new MemoryStream(inBuff));
			int invocationId = br.ReadInt32();
			_invocationCache.Invoke(invocationId, () =>
			{
				_parseMessage(inBuff, br);
			});
		}
	}

	private void _parseMessage(byte[] inBuff, BinaryReader br)
	{
		int num = inBuff.Length;
		using (br)
		{
			using Stream stream = br.BaseStream;
			while (stream.Position != stream.Length)
			{
				switch (br.ReadByte())
				{
				case 251:
				{
					int num2 = _readMessageHeader(br) - 7;
					if (num2 == -1)
					{
						if ((int)debugOut >= 1)
						{
							Listener.DebugReturn(DebugLevel.ERROR, $"Invalid message header for pid={HttpPeerID} cid={_challengId} and message {SupportClass.ByteArrayToString(inBuff)}");
						}
						return;
					}
					Debug.Assert(num2 >= 2);
					byte[] item = br.ReadBytes(num2);
					if (num2 < 2)
					{
						Listener.DebugReturn(DebugLevel.WARNING, $"data len is to small. data {SupportClass.ByteArrayToString(inBuff)}");
					}
					lock (incomingList)
					{
						incomingList.Add(item);
						if (incomingList.Count % warningSize == 0)
						{
							EnqueueStatusCallback(StatusCode.QueueIncomingReliableWarning);
						}
					}
					break;
				}
				case 240:
					ReadPingResponse(br);
					break;
				default:
					Listener.DebugReturn(DebugLevel.WARNING, "Unknow response from server");
					break;
				}
			}
		}
	}

	private void ReadPingResponse(BinaryReader br)
	{
		int num = (br.ReadByte() << 24) | (br.ReadByte() << 16) | (br.ReadByte() << 8) | br.ReadByte();
		int num2 = (br.ReadByte() << 24) | (br.ReadByte() << 16) | (br.ReadByte() << 8) | br.ReadByte();
		lastRoundTripTime = SupportClass.GetTickCount() - num2;
		if (!serverTimeOffsetIsAvailable)
		{
			roundTripTime = lastRoundTripTime;
		}
		UpdateRoundTripTimeAndVariance(lastRoundTripTime);
		if (!serverTimeOffsetIsAvailable)
		{
			serverTimeOffset = num + (lastRoundTripTime >> 1) - SupportClass.GetTickCount();
			serverTimeOffsetIsAvailable = true;
		}
	}

	internal override bool EnqueueOperation(Dictionary<byte, object> parameters, byte opCode, bool sendReliable, byte channelId, bool encrypted, EgMessageType messageType)
	{
		if (peerConnectionState != ConnectionStateValue.Connected)
		{
			if ((int)debugOut >= 1)
			{
				Listener.DebugReturn(DebugLevel.ERROR, "Cannot send op: Not connected. PeerState: " + peerConnectionState);
			}
			Listener.OnStatusChanged(StatusCode.SendError);
			return false;
		}
		byte[] array = SerializeOperationToMessage(opCode, parameters, messageType, encrypted);
		if (array == null)
		{
			return false;
		}
		outgoingStream.Write(array, 0, array.Length);
		return true;
	}

	internal override byte[] SerializeOperationToMessage(byte opCode, Dictionary<byte, object> parameters, EgMessageType messageType, bool encrypt)
	{
		byte[] array;
		lock (SerializeMemStream)
		{
			SerializeMemStream.Position = 0L;
			SerializeMemStream.SetLength(0L);
			if (!encrypt)
			{
				SerializeMemStream.Write(messageHeader, 0, messageHeader.Length);
			}
			Protocol.SerializeOperationRequest(SerializeMemStream, opCode, parameters, setType: false);
			if (encrypt)
			{
				byte[] data = SerializeMemStream.ToArray();
				data = CryptoProvider.Encrypt(data);
				SerializeMemStream.Position = 0L;
				SerializeMemStream.SetLength(0L);
				SerializeMemStream.Write(messageHeader, 0, messageHeader.Length);
				SerializeMemStream.Write(data, 0, data.Length);
			}
			array = SerializeMemStream.ToArray();
		}
		if (messageType != EgMessageType.Operation)
		{
			array[messageHeader.Length - 1] = (byte)messageType;
		}
		if (encrypt)
		{
			array[messageHeader.Length - 1] = (byte)(array[messageHeader.Length - 1] | 0x80);
		}
		int targetOffset = 1;
		Protocol.Serialize(array.Length, array, ref targetOffset);
		return array;
	}

	internal override void StopConnection()
	{
		throw new NotImplementedException();
	}
}
