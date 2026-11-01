namespace LibNoise.Unity.Operator;

public class ScaleBias : ModuleBase
{
	private double m_scale = 1.0;

	private double m_bias;

	public double Bias
	{
		get
		{
			return m_bias;
		}
		set
		{
			m_bias = value;
		}
	}

	public double Scale
	{
		get
		{
			return m_scale;
		}
		set
		{
			m_scale = value;
		}
	}

	public ScaleBias()
		: base(1)
	{
	}

	public ScaleBias(double scale, double bias, ModuleBase input)
		: base(1)
	{
		m_modules[0] = input;
		Bias = bias;
		Scale = scale;
	}

	public override double GetValue(double x, double y, double z)
	{
		return m_modules[0].GetValue(x, y, z) * m_scale + m_bias;
	}
}
