using System;
using System.Xml.Serialization;
using UnityEngine;

namespace LibNoise.Unity;

public class Noise2D : IDisposable
{
	public const double South = -90.0;

	public const double North = 90.0;

	public const double West = -180.0;

	public const double East = 180.0;

	public const double AngleMin = -180.0;

	public const double AngleMax = 180.0;

	public const double Left = -1.0;

	public const double Right = 1.0;

	public const double Top = -1.0;

	public const double Bottom = 1.0;

	private int m_width;

	private int m_height;

	private float m_borderValue = float.NaN;

	private float[,] m_data;

	private ModuleBase m_generator;

	[NonSerialized]
	[XmlIgnore]
	private bool m_disposed;

	public float this[int x, int y]
	{
		get
		{
			if (x < 0 && x >= m_width)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (y < 0 && y >= m_height)
			{
				throw new ArgumentOutOfRangeException();
			}
			return m_data[x, y];
		}
		set
		{
			if (x < 0 && x >= m_width)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (y < 0 && y >= m_height)
			{
				throw new ArgumentOutOfRangeException();
			}
			m_data[x, y] = value;
		}
	}

	public float Border
	{
		get
		{
			return m_borderValue;
		}
		set
		{
			m_borderValue = value;
		}
	}

	public ModuleBase Generator
	{
		get
		{
			return m_generator;
		}
		set
		{
			m_generator = value;
		}
	}

	public int Height => m_height;

	public int Width => m_width;

	public bool IsDisposed => m_disposed;

	protected Noise2D()
	{
	}

	public Noise2D(int size)
		: this(size, size, null)
	{
	}

	public Noise2D(int size, ModuleBase generator)
		: this(size, size, generator)
	{
	}

	public Noise2D(int width, int height)
		: this(width, height, null)
	{
	}

	public Noise2D(int width, int height, ModuleBase generator)
	{
		m_generator = generator;
		m_width = width;
		m_height = height;
		m_data = new float[width, height];
	}

	public void Clear()
	{
		Clear(0f);
	}

	public void Clear(float value)
	{
		for (int i = 0; i < m_width; i++)
		{
			for (int j = 0; j < m_height; j++)
			{
				m_data[i, j] = value;
			}
		}
	}

	private double GenerateCylindrical(double angle, double height)
	{
		double x = Math.Cos(angle * (Math.PI / 180.0));
		double z = Math.Sin(angle * (Math.PI / 180.0));
		return m_generator.GetValue(x, height, z);
	}

	public void GenerateCylindrical(double angleMin, double angleMax, double heightMin, double heightMax)
	{
		if (angleMax <= angleMin || heightMax <= heightMin || m_generator == null)
		{
			throw new ArgumentException();
		}
		double num = angleMax - angleMin;
		double num2 = heightMax - heightMin;
		double num3 = num / (double)m_width;
		double num4 = num2 / (double)m_height;
		double num5 = angleMin;
		double num6 = heightMin;
		for (int i = 0; i < m_width; i++)
		{
			num6 = heightMin;
			for (int j = 0; j < m_height; j++)
			{
				m_data[i, j] = (float)GenerateCylindrical(num5, num6);
				num6 += num4;
			}
			num5 += num3;
		}
	}

	private double GeneratePlanar(double x, double y)
	{
		return m_generator.GetValue(x, 0.0, y);
	}

	public void GeneratePlanar(double left, double right, double top, double bottom)
	{
		GeneratePlanar(left, right, top, bottom, seamless: false);
	}

	public void GeneratePlanar(double left, double right, double top, double bottom, bool seamless)
	{
		if (right <= left || bottom <= top || m_generator == null)
		{
			throw new ArgumentException();
		}
		double num = right - left;
		double num2 = bottom - top;
		double num3 = num / (double)m_width;
		double num4 = num2 / (double)m_height;
		double num5 = left;
		double num6 = top;
		float num7 = 0f;
		for (int i = 0; i < m_width; i++)
		{
			num6 = top;
			for (int j = 0; j < m_height; j++)
			{
				if (!seamless)
				{
					num7 = (float)GeneratePlanar(num5, num6);
				}
				else
				{
					double a = GeneratePlanar(num5, num6);
					double b = GeneratePlanar(num5 + num, num6);
					double a2 = GeneratePlanar(num5, num6 + num2);
					double b2 = GeneratePlanar(num5 + num, num6 + num2);
					double position = 1.0 - (num5 - left) / num;
					double position2 = 1.0 - (num6 - top) / num2;
					double a3 = LibNoizeUtils.InterpolateLinear(a, b, position);
					double b3 = LibNoizeUtils.InterpolateLinear(a2, b2, position);
					num7 = (float)LibNoizeUtils.InterpolateLinear(a3, b3, position2);
				}
				m_data[i, j] = num7;
				num6 += num4;
			}
			num5 += num3;
		}
	}

	private double GenerateSpherical(double lat, double lon)
	{
		double num = Math.Cos(Math.PI / 180.0 * lat);
		return m_generator.GetValue(num * Math.Cos(Math.PI / 180.0 * lon), Math.Sin(Math.PI / 180.0 * lat), num * Math.Sin(Math.PI / 180.0 * lon));
	}

	public void GenerateSpherical(double south, double north, double west, double east)
	{
		if (east <= west || north <= south || m_generator == null)
		{
			throw new ArgumentException();
		}
		double num = east - west;
		double num2 = north - south;
		double num3 = num / (double)m_width;
		double num4 = num2 / (double)m_height;
		double num5 = west;
		double num6 = south;
		for (int i = 0; i < m_width; i++)
		{
			num6 = south;
			for (int j = 0; j < m_height; j++)
			{
				m_data[i, j] = (float)GenerateSpherical(num6, num5);
				num6 += num4;
			}
			num5 += num3;
		}
	}

	public Texture2D GetNormalMap(float scale)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected Obj, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		Texture2D val = new Texture2D(m_width, m_height);
		Color[] array = new Color[m_width * m_height];
		for (int i = 0; i < m_height; i++)
		{
			for (int j = 0; j < m_width; j++)
			{
				Vector3 zero = Vector3.zero;
				Vector3 zero2 = Vector3.zero;
				Vector3 val2 = default;
				if (j > 0 && i > 0 && j < m_width - 1 && i < m_height - 1)
				{
					zero = new Vector3((m_data[j - 1, i] - m_data[j + 1, i]) / 2f * scale, 0f, 1f);
					zero2 = new Vector3(0f, (m_data[j, i - 1] - m_data[j, i + 1]) / 2f * scale, 1f);
					val2 = zero + zero2;
					val2.Normalize();
					Vector3 zero3 = Vector3.zero;
					zero3.x = (val2.x + 1f) / 2f;
					zero3.y = (val2.y + 1f) / 2f;
					zero3.z = (val2.z + 1f) / 2f;
					ref Color reference = ref array[j + i * m_height];
					reference = new Color(zero3.x, zero3.y, zero3.z);
				}
				else
				{
					zero = new Vector3(0f, 0f, 1f);
					zero2 = new Vector3(0f, 0f, 1f);
					val2 = zero + zero2;
					val2.Normalize();
					Vector3 zero4 = Vector3.zero;
					zero4.x = (val2.x + 1f) / 2f;
					zero4.y = (val2.y + 1f) / 2f;
					zero4.z = (val2.z + 1f) / 2f;
					ref Color reference2 = ref array[j + i * m_height];
					reference2 = new Color(zero4.x, zero4.y, zero4.z);
				}
			}
		}
		val.SetPixels(array);
		return val;
	}

	public Texture2D GetTexture()
	{
		return GetTexture(Gradient.Grayscale);
	}

	public Texture2D GetTexture(Gradient gradient)
	{
		return GetTexture(ref gradient);
	}

	public Texture2D GetTexture(ref Gradient gradient)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected Obj, but got Unknown
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		Texture2D val = new Texture2D(m_width, m_height);
		Color[] array = new Color[m_width * m_height];
		int num = 0;
		for (int i = 0; i < m_height; i++)
		{
			int num2 = 0;
			while (num2 < m_width)
			{
				float num3 = 0f;
				num3 = ((float.IsNaN(m_borderValue) || (num2 != 0 && num2 != m_width - 1 && i != 0 && i != m_height - 1)) ? m_data[num2, i] : m_borderValue);
				ref Color reference = ref array[num];
				reference = gradient[num3];
				num2++;
				num++;
			}
		}
		val.SetPixels(array);
		return val;
	}

	public void Dispose()
	{
		if (!m_disposed)
		{
			m_disposed = Disposing();
		}
		GC.SuppressFinalize(this);
	}

	protected virtual bool Disposing()
	{
		if (m_data != null)
		{
			m_data = null;
		}
		m_width = 0;
		m_height = 0;
		return true;
	}
}
