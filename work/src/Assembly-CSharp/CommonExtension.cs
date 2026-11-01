using UnityEngine;

public static class CommonExtension
{
	private enum Side
	{
		West,
		East,
		North,
		South
	}

	public static BlockKind GetKind(this CommonBlockKind c, Vector3 dir)
	{
		Side side;
		if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
		{
			side = ((!(dir.x < 0f)) ? Side.East : Side.West);
		}
		else
		{
			side = ((!(dir.z < 0f)) ? Side.South : Side.North);
		}
		return c switch
		{
			CommonBlockKind.Half => BlockKind.Half, 
			CommonBlockKind.Fence => BlockKind.Fence, 
			CommonBlockKind.Diagonal => side switch
			{
				Side.East => BlockKind.DiagonalEast, 
				Side.West => BlockKind.DiagonalWest, 
				Side.North => BlockKind.DiagonalNorth, 
				_ => BlockKind.DiagonalSouth, 
			}, 
			CommonBlockKind.Stair => side switch
			{
				Side.East => BlockKind.StairEast, 
				Side.West => BlockKind.StairWest, 
				Side.North => BlockKind.StairNorth, 
				_ => BlockKind.StairSouth, 
			}, 
			_ => BlockKind.Default, 
		};
	}
}
