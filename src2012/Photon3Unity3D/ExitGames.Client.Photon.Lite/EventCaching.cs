namespace ExitGames.Client.Photon.Lite;

public enum EventCaching : byte
{
	DoNotCache,
	MergeCache,
	ReplaceCache,
	RemoveCache,
	AddToRoomCache,
	AddToRoomCacheGlobal,
	RemoveFromRoomCache,
	RemoveFromRoomCacheForActorsLeft
}
