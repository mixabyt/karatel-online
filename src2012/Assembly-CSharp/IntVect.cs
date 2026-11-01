public struct IntVect(int x, int y, int z)
{
	private readonly int m_X = x;

	private readonly int m_Y = y;

	private readonly int m_Z = z;

	public int X => m_X;

	public int Y => m_Y;

	public int Z => m_Z;

	public override bool Equals(object obj)
	{
		IntVect intVect = (IntVect)obj;
		return intVect.X == m_X && intVect.Y == m_Y && intVect.Z == m_Z;
	}

	public override int GetHashCode()
	{
		return m_X ^ m_Y ^ m_Z;
	}

	public static bool operator ==(IntVect one, IntVect other)
	{
		return one.X == other.X && one.Y == other.Y && one.Z == other.Z;
	}

	public static bool operator !=(IntVect one, IntVect other)
	{
		return !(one == other);
	}
}
