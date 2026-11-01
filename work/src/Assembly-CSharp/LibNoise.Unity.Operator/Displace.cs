namespace LibNoise.Unity.Operator;

public class Displace : ModuleBase
{
	public ModuleBase X
	{
		get
		{
			return m_modules[1];
		}
		set
		{
			m_modules[1] = value;
		}
	}

	public ModuleBase Y
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

	public ModuleBase Z
	{
		get
		{
			return m_modules[3];
		}
		set
		{
			m_modules[3] = value;
		}
	}

	public Displace()
		: base(4)
	{
	}

	public Displace(ModuleBase input, ModuleBase x, ModuleBase y, ModuleBase z)
		: base(4)
	{
		m_modules[0] = input;
		m_modules[1] = x;
		m_modules[2] = y;
		m_modules[3] = z;
	}

	public override double GetValue(double x, double y, double z)
	{
		double x2 = x + m_modules[1].GetValue(x, y, z);
		double y2 = y + m_modules[1].GetValue(x, y, z);
		double z2 = z + m_modules[1].GetValue(x, y, z);
		return m_modules[0].GetValue(x2, y2, z2);
	}
}
