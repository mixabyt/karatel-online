using System;

public class RandomAdv
{
	private const ulong N = 624uL;

	private const ulong M = 397uL;

	private const ulong MATRIX_A = 2567483615uL;

	private const ulong UPPER_MASK = 2147483648uL;

	private const ulong LOWER_MASK = 2147483647uL;

	private const uint DEFAULT_SEED = 4357u;

	private static ulong[] mt = new ulong[625];

	private static ulong mti = 625uL;

	public RandomAdv()
	{
		ulong[] init_key = new ulong[4] { 291uL, 564uL, 837uL, 1110uL };
		ulong key_length = 4uL;
		init_by_array(init_key, key_length);
	}

	private void init_genrand(ulong s)
	{
		mt[0] = s & 0xFFFFFFFFu;
		for (mti = 1uL; mti < 624; mti++)
		{
			mt[mti] = 1812433253 * (mt[mti - 1] ^ (mt[mti - 1] >> 30)) + mti;
			mt[mti] &= 4294967295uL;
		}
	}

	public void init_by_array(ulong[] init_key, ulong key_length)
	{
		init_genrand(19650218uL);
		ulong num = 1uL;
		ulong num2 = 0uL;
		for (ulong num3 = ((624 <= key_length) ? key_length : 624); num3 != 0; num3--)
		{
			mt[num] = (mt[num] ^ ((mt[num - 1] ^ (mt[num - 1] >> 30)) * 1664525)) + init_key[num2] + num2;
			mt[num] &= 4294967295uL;
			num++;
			num2++;
			if (num >= 624)
			{
				mt[0] = mt[623];
				num = 1uL;
			}
			if (num2 >= key_length)
			{
				num2 = 0uL;
			}
		}
		for (ulong num3 = 623uL; num3 != 0; num3--)
		{
			mt[num] = (mt[num] ^ ((mt[num - 1] ^ (mt[num - 1] >> 30)) * 1566083941)) - num;
			mt[num] &= 4294967295uL;
			num++;
			if (num >= 624)
			{
				mt[0] = mt[623];
				num = 1uL;
			}
		}
		mt[0] = 2147483648uL;
	}

	public long genrand_int31()
	{
		return (long)(genrand_int32() >> 1);
	}

	public double genrand_real1()
	{
		return (double)genrand_int32() * 2.3283064370807974E-10;
	}

	public double genrand_real2()
	{
		return (double)genrand_int32() * 2.3283064365386963E-10;
	}

	public double genrand_real3()
	{
		return ((double)genrand_int32() + 0.5) * 2.3283064365386963E-10;
	}

	public double genrand_res53()
	{
		ulong num = genrand_int32() >> 5;
		ulong num2 = genrand_int32() >> 6;
		return ((double)num * 67108864.0 + (double)num2) * 1.1102230246251565E-16;
	}

	public ulong genrand_int32()
	{
		ulong num = 0uL;
		ulong[] array = new ulong[2] { 0uL, 2567483615uL };
		if (mti >= 624)
		{
			if (mti == 625)
			{
				init_genrand(5489uL);
			}
			ulong num2;
			for (num2 = 0uL; num2 < 227; num2++)
			{
				num = (mt[num2] & 0x80000000u) | (mt[num2 + 1] & 0x7FFFFFFF);
				mt[num2] = mt[num2 + 397] ^ (num >> 1) ^ array[num & 1];
			}
			for (; num2 < 623; num2++)
			{
				num = (mt[num2] & 0x80000000u) | (mt[num2 + 1] & 0x7FFFFFFF);
				mt[num2] = mt[num2 - 227] ^ (num >> 1) ^ array[num & 1];
			}
			num = (mt[623] & 0x80000000u) | (mt[0] & 0x7FFFFFFF);
			mt[623] = mt[396] ^ (num >> 1) ^ array[num & 1];
			mti = 0uL;
		}
		num = mt[mti++];
		num ^= num >> 11;
		num ^= (num << 7) & 0x9D2C5680u;
		num ^= (num << 15) & 0xEFC60000u;
		return num ^ (num >> 18);
	}

	public int RandomRange(int lo, int hi)
	{
		return Math.Abs((int)genrand_int32() % (hi - lo + 1)) + lo;
	}
}
