namespace LibNoise.Unity.Operator;

public class Select : ModuleBase
{
	private double m_fallOff;

	private double m_raw;

	private double m_min = -1.0;

	private double m_max = 1.0;

	public ModuleBase Controller
	{
		get
		{
			return m_modules[2];
		}
		set
		{
			m_modules[2] = value;
		}
	}

	public double FallOff
	{
		get
		{
			return m_fallOff;
		}
		set
		{
			double num = m_max - m_min;
			m_raw = value;
			m_fallOff = ((!(value > num / 2.0)) ? value : (num / 2.0));
		}
	}

	public double Maximum
	{
		get
		{
			return m_max;
		}
		set
		{
			m_max = value;
			FallOff = m_raw;
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
			FallOff = m_raw;
		}
	}

	public Select()
		: base(2)
	{
	}

	public Select(double min, double max, double fallOff, ModuleBase input, ModuleBase controller)
		: base(2)
	{
		m_modules[0] = input;
		m_modules[1] = controller;
		m_min = min;
		m_max = max;
		FallOff = fallOff;
	}

	public void SetBounds(double min, double max)
	{
		m_min = min;
		m_max = max;
		FallOff = m_fallOff;
	}

	public override double GetValue(double x, double y, double z)
	{
		double value = m_modules[2].GetValue(x, y, z);
		if (m_fallOff > 0.0)
		{
			if (value < m_min - m_fallOff)
			{
				return m_modules[0].GetValue(x, y, z);
			}
			if (value < m_min + m_fallOff)
			{
				double num = m_min - m_fallOff;
				double num2 = m_min + m_fallOff;
				double position = LibNoizeUtils.MapCubicSCurve((value - num) / (num2 - num));
				return LibNoizeUtils.InterpolateLinear(m_modules[0].GetValue(x, y, z), m_modules[1].GetValue(x, y, z), position);
			}
			if (value < m_max - m_fallOff)
			{
				return m_modules[1].GetValue(x, y, z);
			}
			if (value < m_max + m_fallOff)
			{
				double num3 = m_max - m_fallOff;
				double num4 = m_max + m_fallOff;
				double position = LibNoizeUtils.MapCubicSCurve((value - num3) / (num4 - num3));
				return LibNoizeUtils.InterpolateLinear(m_modules[1].GetValue(x, y, z), m_modules[0].GetValue(x, y, z), position);
			}
			return m_modules[0].GetValue(x, y, z);
		}
		if (value < m_min || value > m_max)
		{
			return m_modules[0].GetValue(x, y, z);
		}
		return m_modules[1].GetValue(x, y, z);
	}
}
