namespace LibNoise.Unity.Generator;

public class Const : ModuleBase
{
	private double m_value;

	public double Value
	{
		get
		{
			return m_value;
		}
		set
		{
			m_value = value;
		}
	}

	public Const()
		: base(0)
	{
	}

	public Const(double value)
		: base(0)
	{
		Value = value;
	}

	public override double GetValue(double x, double y, double z)
	{
		return m_value;
	}
}
