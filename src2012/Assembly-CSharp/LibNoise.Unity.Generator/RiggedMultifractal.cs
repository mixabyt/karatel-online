using System;
using UnityEngine;

namespace LibNoise.Unity.Generator;

public class RiggedMultifractal : ModuleBase
{
	private double m_frequency = 1.0;

	private double m_lacunarity = 2.0;

	private QualityMode m_quality = QualityMode.Medium;

	private int m_octaveCount = 6;

	private int m_seed;

	private double[] m_weights = new double[30];

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
			UpdateWeights();
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

	public RiggedMultifractal()
		: base(0)
	{
		UpdateWeights();
	}

	public RiggedMultifractal(double frequency, double lacunarity, int octaves, int seed, QualityMode quality)
		: base(0)
	{
		Frequency = frequency;
		Lacunarity = lacunarity;
		OctaveCount = octaves;
		Seed = seed;
		Quality = quality;
	}

	private void UpdateWeights()
	{
		double num = 1.0;
		for (int i = 0; i < 30; i++)
		{
			m_weights[i] = Math.Pow(num, -1.0);
			num *= m_lacunarity;
		}
	}

	public override double GetValue(double x, double y, double z)
	{
		x *= m_frequency;
		y *= m_frequency;
		z *= m_frequency;
		double num = 0.0;
		double num2 = 0.0;
		double num3 = 1.0;
		double num4 = 1.0;
		double num5 = 2.0;
		for (int i = 0; i < m_octaveCount; i++)
		{
			double x2 = LibNoizeUtils.MakeInt32Range(x);
			double y2 = LibNoizeUtils.MakeInt32Range(y);
			double z2 = LibNoizeUtils.MakeInt32Range(z);
			long seed = (m_seed + i) & 0x7FFFFFFF;
			num = LibNoizeUtils.GradientCoherentNoise3D(x2, y2, z2, seed, m_quality);
			num = Math.Abs(num);
			num = num4 - num;
			num *= num;
			num *= num3;
			num3 = num * num5;
			if (num3 > 1.0)
			{
				num3 = 1.0;
			}
			if (num3 < 0.0)
			{
				num3 = 0.0;
			}
			num2 += num * m_weights[i];
			x *= m_lacunarity;
			y *= m_lacunarity;
			z *= m_lacunarity;
		}
		return num2 * 1.25 - 1.0;
	}
}
