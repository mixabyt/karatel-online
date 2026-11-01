namespace LibNoise.Unity.Operator;

public class Invert : ModuleBase
{
	public Invert()
		: base(1)
	{
	}

	public Invert(ModuleBase input)
		: base(1)
	{
		m_modules[0] = input;
	}

	public override double GetValue(double x, double y, double z)
	{
		return 0.0 - m_modules[0].GetValue(x, y, z);
	}
}
