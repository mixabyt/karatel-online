namespace LibNoise.Unity.Operator;

public class Multiply : ModuleBase
{
	public Multiply()
		: base(2)
	{
	}

	public Multiply(ModuleBase lhs, ModuleBase rhs)
		: base(2)
	{
		m_modules[0] = lhs;
		m_modules[1] = rhs;
	}

	public override double GetValue(double x, double y, double z)
	{
		return m_modules[0].GetValue(x, y, z) * m_modules[1].GetValue(x, y, z);
	}
}
