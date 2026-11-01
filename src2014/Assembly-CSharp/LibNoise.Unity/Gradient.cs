using System;
using System.Collections.Generic;
using UnityEngine;

namespace LibNoise.Unity;

public struct Gradient
{
	private List<KeyValuePair<double, Color>> m_data = new List<KeyValuePair<double, Color>>();

	private bool m_inverted;

	private static Gradient _empty;

	private static Gradient _terrain;

	private static Gradient _grayscale;

	public Color this[double position]
	{
		get
		{
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			for (num = 0; num < m_data.Count && !(position < m_data[num].Key); num++)
			{
			}
			int num2 = Mathf.Clamp(num - 1, 0, m_data.Count - 1);
			int num3 = Mathf.Clamp(num, 0, m_data.Count - 1);
			if (num2 == num3)
			{
				return m_data[num3].Value;
			}
			double key = m_data[num2].Key;
			double key2 = m_data[num3].Key;
			double num4 = (position - key) / (key2 - key);
			if (m_inverted)
			{
				num4 = 1.0 - num4;
				double num5 = key;
				key = key2;
				key2 = num5;
			}
			return Color.Lerp(m_data[num2].Value, m_data[num3].Value, (float)num4);
		}
		set
		{
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_data.Count; i++)
			{
				if (m_data[i].Key == position)
				{
					m_data.RemoveAt(i);
					break;
				}
			}
			m_data.Add(new KeyValuePair<double, Color>(position, value));
			m_data.Sort((KeyValuePair<double, Color> lhs, KeyValuePair<double, Color> rhs) => lhs.Key.CompareTo(rhs.Key));
		}
	}

	public bool IsInverted
	{
		get
		{
			return m_inverted;
		}
		set
		{
			m_inverted = value;
		}
	}

	public static Gradient Empty => _empty;

	public static Gradient Grayscale => _grayscale;

	public static Gradient Terrain => _terrain;

	public Gradient(Color color)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		m_data.Add(new KeyValuePair<double, Color>(-1.0, color));
		m_data.Add(new KeyValuePair<double, Color>(1.0, color));
		m_inverted = false;
	}

	public Gradient(Color start, Color end)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		m_data.Add(new KeyValuePair<double, Color>(-1.0, start));
		m_data.Add(new KeyValuePair<double, Color>(1.0, end));
		m_inverted = false;
	}

	static Gradient()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		_terrain.m_data = new List<KeyValuePair<double, Color>>();
		_terrain.m_data.Add(new KeyValuePair<double, Color>(-1.0, new Color(0f, 0f, 128f)));
		_terrain.m_data.Add(new KeyValuePair<double, Color>(-0.2, new Color(32f, 64f, 128f)));
		_terrain.m_data.Add(new KeyValuePair<double, Color>(-0.04, new Color(64f, 96f, 192f)));
		_terrain.m_data.Add(new KeyValuePair<double, Color>(-0.02, new Color(192f, 192f, 128f)));
		_terrain.m_data.Add(new KeyValuePair<double, Color>(0.0, new Color(0f, 192f, 0f)));
		_terrain.m_data.Add(new KeyValuePair<double, Color>(0.25, new Color(192f, 192f, 0f)));
		_terrain.m_data.Add(new KeyValuePair<double, Color>(0.5, new Color(160f, 96f, 64f)));
		_terrain.m_data.Add(new KeyValuePair<double, Color>(0.75, new Color(128f, 255f, 255f)));
		_terrain.m_data.Add(new KeyValuePair<double, Color>(1.0, new Color(255f, 255f, 255f)));
		_terrain.m_inverted = false;
		_grayscale.m_data = new List<KeyValuePair<double, Color>>();
		_grayscale.m_data.Add(new KeyValuePair<double, Color>(-1.0, Color.black));
		_grayscale.m_data.Add(new KeyValuePair<double, Color>(1.0, Color.white));
		_grayscale.m_inverted = false;
		_empty.m_data = new List<KeyValuePair<double, Color>>();
		_empty.m_data.Add(new KeyValuePair<double, Color>(-1.0, Color.clear));
		_empty.m_data.Add(new KeyValuePair<double, Color>(1.0, Color.clear));
		_empty.m_inverted = false;
	}

	public void Clear()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		m_data.Clear();
		m_data.Add(new KeyValuePair<double, Color>(0.0, Color.clear));
		m_data.Add(new KeyValuePair<double, Color>(1.0, Color.clear));
	}

	public void Invert()
	{
		throw new NotImplementedException();
	}
}
