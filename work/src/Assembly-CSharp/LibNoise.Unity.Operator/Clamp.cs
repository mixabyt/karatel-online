namespace LibNoise.Unity.Operator;

public class Clamp : ModuleBase
{
	private double m_min = -1.0;

	private double m_max = 1.0;

	public double Maximum
	{
		get
		{
			return m_max;
		}
		set
		{
			m_max = value;
		}
	}

	public double Minimum
	{
		get
		{
			return m_min;
		}
		set
		{
			m_min = value;
		}
	}

	public Clamp()
		: base(1)
	{
	}

	public Clamp(double min, double max, ModuleBase input)
		: base(1)
	{
		Minimum = min;
		Maximum = max;
		m_modules[0] = input;
	}

	public override double GetValue(double x, double y, double z)
	{
		if (m_min > m_max)
		{
			double min = m_min;
			m_min = m_max;
			m_max = min;
		}
		double value = m_modules[0].GetValue(x, y, z);
		if (value < m_min)
		{
			return m_min;
		}
		if (value > m_max)
		{
			return m_max;
		}
		return value;
	}
}
