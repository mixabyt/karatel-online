using UnityEngine;

public class BlockUVCoordinates
{
	private readonly Rect[] m_BlockFaceUvCoordinates = new Rect[3];

	public Rect[] BlockFaceUvCoordinates => m_BlockFaceUvCoordinates;

	public BlockUVCoordinates(Rect topUvCoordinates, Rect sideUvCoordinates, Rect bottomUvCoordinates)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		BlockFaceUvCoordinates[0] = topUvCoordinates;
		BlockFaceUvCoordinates[1] = sideUvCoordinates;
		BlockFaceUvCoordinates[2] = bottomUvCoordinates;
	}
}
