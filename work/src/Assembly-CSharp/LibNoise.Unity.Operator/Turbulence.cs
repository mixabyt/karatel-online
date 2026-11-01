using LibNoise.Unity.Generator;

namespace LibNoise.Unity.Operator;

public class Turbulence : ModuleBase
{
	private const double X0 = 6207.0 / 32768.0;

	private const double Y0 = 16281.0 / 16384.0;

	private const double Z0 = 31337.0 / 65536.0;

	private const double X1 = 26519.0 / 65536.0;

	private const double Y1 = 1133.0 / 4096.0;

	private const double Z1 = 60493.0 / 65536.0;

	private const double X2 = 13455.0 / 16384.0;

	private const double Y2 = 11213.0 / 65536.0;

	private const double Z2 = 44845.0 / 65536.0;

	private double m_power = 1.0;

	private Perlin m_xDistort;

	private Perlin m_yDistort;

	private Perlin m_zDistort;

	public double Frequency
	{
		get
		{
			return m_xDistort.Frequency;
		}
		set
		{
			m_xDistort.Frequency = value;
			m_yDistort.Frequency = value;
			m_zDistort.Frequency = value;
		}
	}

	public double Power
	{
		get
		{
			return m_power;
		}
		set
		{
			m_power = value;
		}
	}

	public int Roughness
	{
		get
		{
			return m_xDistort.OctaveCount;
		}
		set
		{
			m_xDistort.OctaveCount = value;
			m_yDistort.OctaveCount = value;
			m_zDistort.OctaveCount = value;
		}
	}

	public int Seed
	{
		get
		{
			return m_xDistort.Seed;
		}
		set
		{
			m_xDistort.Seed = value;
			m_yDistort.Seed = value + 1;
			m_zDistort.Seed = value + 2;
		}
	}

	public Turbulence()
		: base(1)
	{
		m_xDistort = new Perlin();
		m_yDistort = new Perlin();
		m_zDistort = new Perlin();
	}

	public Turbulence(double power, ModuleBase input)
		: this(new Perlin(), new Perlin(), new Perlin(), power, input)
	{
	}

	public Turbulence(Perlin x, Perlin y, Perlin z, double power, ModuleBase input)
		: base(1)
	{
		m_xDistort = x;
		m_yDistort = y;
		m_zDistort = z;
		m_modules[0] = input;
		Power = power;
	}

	public override double GetValue(double x, double y, double z)
	{
		double x2 = x + m_xDistort.GetValue(x + 6207.0 / 32768.0, y + 16281.0 / 16384.0, z + 31337.0 / 65536.0) * m_power;
		double y2 = y + m_yDistort.GetValue(x + 26519.0 / 65536.0, y + 1133.0 / 4096.0, z + 60493.0 / 65536.0) * m_power;
		double z2 = z + m_zDistort.GetValue(x + 13455.0 / 16384.0, y + 11213.0 / 65536.0, z + 44845.0 / 65536.0) * m_power;
		return m_modules[0].GetValue(x2, y2, z2);
	}
}
