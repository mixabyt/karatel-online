using System;
using UnityEngine;

namespace LibNoise.Unity.Generator;

public class Billow : ModuleBase
{
	private double m_frequency = 1.0;

	private double m_lacunarity = 2.0;

	private QualityMode m_quality = QualityMode.Medium;

	private int m_octaveCount = 6;

	private double m_persistence = 0.5;

	private int m_seed;

	public double Frequency
	{
		get
		{
			return m_frequency;
		}
		set
		{
			m_frequency = value;
		}
	}

	public double Lacunarity
	{
		get
		{
			return m_lacunarity;
		}
		set
		{
			m_lacunarity = value;
		}
	}

	public QualityMode Quality
	{
		get
		{
			return m_quality;
		}
		set
		{
			m_quality = value;
		}
	}

	public int OctaveCount
	{
		get
		{
			return m_octaveCount;
		}
		set
		{
			m_octaveCount = Mathf.Clamp(value, 1, 30);
		}
	}

	public double Persistence
	{
		get
		{
			return m_persistence;
		}
		set
		{
			m_persistence = value;
		}
	}

	public int Seed
	{
		get
		{
			return m_seed;
		}
		set
		{
			m_seed = value;
		}
	}

	public Billow()
		: base(0)
	{
	}

	public Billow(double frequency, double lacunarity, double persistence, int octaves, int seed, QualityMode quality)
		: base(0)
	{
		Frequency = frequency;
		Lacunarity = lacunarity;
		OctaveCount = octaves;
		Persistence = persistence;
		Seed = seed;
		Quality = quality;
	}

	public override double GetValue(double x, double y, double z)
	{
		double num = 0.0;
		double num2 = 0.0;
		double num3 = 1.0;
		x *= m_frequency;
		y *= m_frequency;
		z *= m_frequency;
		for (int i = 0; i < m_octaveCount; i++)
		{
			double x2 = LibNoizeUtils.MakeInt32Range(x);
			double y2 = LibNoizeUtils.MakeInt32Range(y);
			double z2 = LibNoizeUtils.MakeInt32Range(z);
			long seed = (m_seed + i) & 0xFFFFFFFFu;
			num2 = LibNoizeUtils.GradientCoherentNoise3D(x2, y2, z2, seed, m_quality);
			num2 = 2.0 * Math.Abs(num2) - 1.0;
			num += num2 * num3;
			x *= m_lacunarity;
			y *= m_lacunarity;
			z *= m_lacunarity;
			num3 *= m_persistence;
		}
		return num + 0.5;
	}
}
