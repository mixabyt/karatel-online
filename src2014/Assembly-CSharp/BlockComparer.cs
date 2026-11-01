public static class BlockComparer
{
	public static bool IsAir(this BlockType block)
	{
		return block == BlockType.Air;
	}

	public static bool IsGlass(this BlockType block)
	{
		return WorldGameObject.Instance.BlockParametrs[(uint)block].is_glass;
	}

	public static bool IsLand(this BlockType block)
	{
		return !block.IsAir() && !block.IsWater() && !block.IsGlass();
	}

	public static bool IsWater(this BlockType block)
	{
		return block == BlockType.Water;
	}

	public static bool IsDefault(this BlockKind block)
	{
		return block == BlockKind.Default;
	}

	public static bool IsDiagonal(this BlockKind block)
	{
		return block == BlockKind.DiagonalNorth || block == BlockKind.DiagonalSouth || block == BlockKind.DiagonalWest || block == BlockKind.DiagonalEast;
	}

	public static bool IsStair(this BlockKind block)
	{
		return block == BlockKind.StairNorth || block == BlockKind.StairSouth || block == BlockKind.StairWest || block == BlockKind.StairEast;
	}

	public static bool IsHalf(this BlockKind block)
	{
		return block == BlockKind.Half;
	}

	public static bool IsFence(this BlockKind block)
	{
		return block == BlockKind.Fence;
	}

	public static bool IsFlip(this BlockKind block)
	{
		return (int)block >= 128;
	}

	public static BlockKind DoFlip(this BlockKind block)
	{
		if ((int)block >= 128)
		{
			return block - 128;
		}
		return block + 128;
	}
}
