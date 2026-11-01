namespace LibNoise.Unity.Operator;

public class Subtract : ModuleBase
{
	public Subtract()
		: base(2)
	{
	}

	public Subtract(ModuleBase lhs, ModuleBase rhs)
		: base(2)
	{
		m_modules[0] = lhs;
		m_modules[1] = rhs;
	}

	public override double GetValue(double x, double y, double z)
	{
		return m_modules[0].GetValue(x, y, z) - m_modules[1].GetValue(x, y, z);
	}
}
