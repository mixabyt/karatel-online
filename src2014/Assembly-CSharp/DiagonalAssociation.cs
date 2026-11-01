public class DiagonalAssociation
{
	public int[] I = new int[3];

	public int[] J = new int[4];

	public int[] FlipJ = new int[4];

	public static DiagonalAssociation West = new DiagonalAssociation
	{
		I = new int[3] { 0, 2, 3 },
		J = new int[4] { 1, 2, 2, 3 },
		FlipJ = new int[4] { 0, 3, 1, 0 }
	};

	public static DiagonalAssociation East = new DiagonalAssociation
	{
		I = new int[3] { 1, 2, 3 },
		J = new int[4] { 2, 1, 0, 1 },
		FlipJ = new int[4] { 3, 0, 3, 2 }
	};

	public static DiagonalAssociation South = new DiagonalAssociation
	{
		I = new int[3] { 3, 0, 1 },
		J = new int[4] { 1, 2, 0, 3 },
		FlipJ = new int[4] { 0, 3, 0, 3 }
	};

	public static DiagonalAssociation North = new DiagonalAssociation
	{
		I = new int[3] { 2, 0, 1 },
		J = new int[4] { 2, 1, 1, 2 },
		FlipJ = new int[4] { 3, 0, 1, 2 }
	};

	public static DiagonalAssociation GetAssociation(BlockKind kind)
	{
		return kind switch
		{
			BlockKind.DiagonalEast => East, 
			BlockKind.DiagonalWest => West, 
			BlockKind.DiagonalNorth => North, 
			BlockKind.DiagonalSouth => South, 
			_ => null, 
		};
	}

	public static int[] GetUVNumber(BlockKind kind)
	{
		return kind switch
		{
			BlockKind.DiagonalEast => new int[2] { 2, 3 }, 
			BlockKind.DiagonalWest => new int[2] { 3, 2 }, 
			BlockKind.DiagonalSouth => new int[2] { 1, 0 }, 
			_ => new int[2] { 0, 1 }, 
		};
	}
}
