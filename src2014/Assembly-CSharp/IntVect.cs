using UnityEngine;

public struct IntVect(int x, int y, int z)
{
	private readonly int x = x;

	private readonly int y = y;

	private readonly int z = z;

	public static IntVect Zero = new IntVect(0, 0, 0);

	public static IntVect One = new IntVect(1, 1, 1);

	public static IntVect Right = new IntVect(1, 0, 0);

	public static IntVect Forward = new IntVect(0, 1, 0);

	public static IntVect Up = new IntVect(0, 0, 1);

	public int X => x;

	public int Y => y;

	public int Z => z;

	public override bool Equals(object obj)
	{
		IntVect intVect = (IntVect)obj;
		return intVect.X == x && intVect.Y == y && intVect.Z == z;
	}

	public override int GetHashCode()
	{
		return x ^ y ^ z;
	}

	public override string ToString()
	{
		return $"({x}, {y}, {z})";
	}

	public IntVect MultiplyComponent(IntVect right)
	{
		return new IntVect(x * right.x, y * right.y, z * right.z);
	}

	public bool CompareOr(IntVect right)
	{
		return x == right.x || y == right.y || z == right.z;
	}

	public static bool operator ==(IntVect left, IntVect right)
	{
		return left.x == right.x && left.y == right.y && left.z == right.z;
	}

	public static bool operator !=(IntVect left, IntVect right)
	{
		return !(left == right);
	}

	public static IntVect operator -(IntVect left, IntVect right)
	{
		return new IntVect(left.x - right.x, left.y - right.y, left.z - right.z);
	}

	public static IntVect operator -(IntVect a)
	{
		return new IntVect(-a.x, -a.y, -a.z);
	}

	public static IntVect operator +(IntVect left, IntVect right)
	{
		return new IntVect(left.x + right.x, left.y + right.y, left.z + right.z);
	}

	public static IntVect operator *(IntVect a, int d)
	{
		return new IntVect(a.x * d, a.y * d, a.z * d);
	}

	public static implicit operator Vector3(IntVect v)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3((float)v.x, (float)v.y, (float)v.z);
	}
}
