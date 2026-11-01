using UnityEngine;

public class StairAssociation
{
	public class SideAssociation
	{
		public Vector3 v1 = Vector3.zero;

		public Vector3 v2 = Vector3.zero;

		public int[] num;

		public float[][] uvY = new float[2][];

		public float[][] uvX = new float[2][];

		public SideAssociation()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		}
	}

	public int[] Faces;

	public SideAssociation Forward;

	public SideAssociation Top;

	public SideAssociation Right;

	public SideAssociation Left;

	public static StairAssociation East = new StairAssociation
	{
		Faces = new int[6] { 0, 1, 2, 3, 4, 5 },
		Forward = new SideAssociation
		{
			v1 = new Vector3(0.5f, 0f, 0f),
			v2 = new Vector3(0f, 0f, 0.5f),
			num = new int[4] { 0, 3, 2, 1 },
			uvY = new float[2][]
			{
				new float[4] { 0.5f, 1f, 1f, 0.5f },
				new float[4] { 0f, 0.5f, 0.5f, 0f }
			}
		},
		Top = new SideAssociation
		{
			v1 = new Vector3(0f, 0f, -0.5f),
			v2 = new Vector3(-0.5f, 0f, 0f),
			num = new int[4] { 2, 3, 0, 1 },
			uvX = new float[2][]
			{
				new float[4] { 0f, 0f, 0.5f, 0.5f },
				new float[4] { 0.5f, 0.5f, 1f, 1f }
			}
		},
		Right = new SideAssociation
		{
			v1 = new Vector3(-0.5f, 0f, 0f),
			v2 = new Vector3(0f, 0f, -0.5f),
			num = new int[4] { 1, 0, 2, 3 },
			uvY = new float[2][]
			{
				new float[4] { 0f, 0.5f, 0.5f, 0f },
				null
			},
			uvX = new float[2][]
			{
				new float[4] { 0.5f, 0.5f, 1f, 1f },
				new float[4] { 0f, 0f, 0.5f, 0.5f }
			}
		},
		Left = new SideAssociation
		{
			v1 = new Vector3(-0.5f, 0f, 0f),
			v2 = new Vector3(0f, 0f, -0.5f),
			num = new int[4] { 2, 3, 1, 0 },
			uvY = new float[2][]
			{
				new float[4] { 0f, 0.5f, 0.5f, 0f },
				null
			},
			uvX = new float[2][]
			{
				new float[4] { 0f, 0f, 0.5f, 0.5f },
				new float[4] { 0.5f, 0.5f, 1f, 1f }
			}
		}
	};

	public static StairAssociation West = new StairAssociation
	{
		Faces = new int[6] { 1, 0, 3, 2, 4, 5 },
		Forward = new SideAssociation
		{
			v1 = new Vector3(-0.5f, 0f, 0f),
			v2 = new Vector3(0f, 0f, 0.5f),
			num = new int[4] { 0, 3, 2, 1 },
			uvY = new float[2][]
			{
				new float[4] { 0.5f, 1f, 1f, 0.5f },
				new float[4] { 0f, 0.5f, 0.5f, 0f }
			}
		},
		Top = new SideAssociation
		{
			v1 = new Vector3(0f, 0f, -0.5f),
			v2 = new Vector3(0.5f, 0f, 0f),
			num = new int[4] { 0, 1, 2, 3 },
			uvX = new float[2][]
			{
				new float[4] { 0.5f, 0.5f, 1f, 1f },
				new float[4] { 0f, 0f, 0.5f, 0.5f }
			}
		},
		Right = new SideAssociation
		{
			v1 = new Vector3(0.5f, 0f, 0f),
			v2 = new Vector3(0f, 0f, -0.5f),
			num = new int[4] { 1, 0, 2, 3 },
			uvY = new float[2][]
			{
				new float[4] { 0f, 0.5f, 0.5f, 0f },
				null
			},
			uvX = new float[2][]
			{
				new float[4] { 0.5f, 0.5f, 1f, 1f },
				new float[4] { 0f, 0f, 0.5f, 0.5f }
			}
		},
		Left = new SideAssociation
		{
			v1 = new Vector3(0.5f, 0f, 0f),
			v2 = new Vector3(0f, 0f, -0.5f),
			num = new int[4] { 2, 3, 1, 0 },
			uvY = new float[2][]
			{
				new float[4] { 0f, 0.5f, 0.5f, 0f },
				null
			},
			uvX = new float[2][]
			{
				new float[4] { 0f, 0f, 0.5f, 0.5f },
				new float[4] { 0.5f, 0.5f, 1f, 1f }
			}
		}
	};

	public static StairAssociation North = new StairAssociation
	{
		Faces = new int[6] { 3, 2, 1, 0, 4, 5 },
		Forward = new SideAssociation
		{
			v1 = new Vector3(0f, -0.5f, 0f),
			v2 = new Vector3(0f, 0f, 0.5f),
			num = new int[4] { 0, 3, 2, 1 },
			uvY = new float[2][]
			{
				new float[4] { 0.5f, 1f, 1f, 0.5f },
				new float[4] { 0f, 0.5f, 0.5f, 0f }
			}
		},
		Top = new SideAssociation
		{
			v1 = new Vector3(0f, 0f, -0.5f),
			v2 = new Vector3(0f, 0.5f, 0f),
			num = new int[4] { 0, 3, 1, 2 },
			uvY = new float[2][]
			{
				new float[4] { 0.5f, 1f, 1f, 0.5f },
				new float[4] { 0f, 0.5f, 0.5f, 0f }
			}
		},
		Right = new SideAssociation
		{
			v1 = new Vector3(0f, 0.5f, 0f),
			v2 = new Vector3(0f, 0f, -0.5f),
			num = new int[4] { 2, 3, 1, 0 },
			uvY = new float[2][]
			{
				new float[4] { 0f, 0.5f, 0.5f, 0f },
				null
			},
			uvX = new float[2][]
			{
				new float[4] { 0f, 0f, 0.5f, 0.5f },
				new float[4] { 0.5f, 0.5f, 1f, 1f }
			}
		},
		Left = new SideAssociation
		{
			v1 = new Vector3(0f, 0.5f, 0f),
			v2 = new Vector3(0f, 0f, -0.5f),
			num = new int[4] { 1, 0, 2, 3 },
			uvY = new float[2][]
			{
				new float[4] { 0f, 0.5f, 0.5f, 0f },
				null
			},
			uvX = new float[2][]
			{
				new float[4] { 0.5f, 0.5f, 1f, 1f },
				new float[4] { 0f, 0f, 0.5f, 0.5f }
			}
		}
	};

	public static StairAssociation South = new StairAssociation
	{
		Faces = new int[6] { 2, 3, 0, 1, 4, 5 },
		Forward = new SideAssociation
		{
			v1 = new Vector3(0f, 0.5f, 0f),
			v2 = new Vector3(0f, 0f, 0.5f),
			num = new int[4] { 0, 3, 2, 1 },
			uvY = new float[2][]
			{
				new float[4] { 0.5f, 1f, 1f, 0.5f },
				new float[4] { 0f, 0.5f, 0.5f, 0f }
			}
		},
		Top = new SideAssociation
		{
			v1 = new Vector3(0f, 0f, -0.5f),
			v2 = new Vector3(0f, -0.5f, 0f),
			num = new int[4] { 1, 2, 0, 3 },
			uvY = new float[2][]
			{
				new float[4] { 0f, 0.5f, 0.5f, 0f },
				new float[4] { 0.5f, 1f, 1f, 0.5f }
			}
		},
		Right = new SideAssociation
		{
			v1 = new Vector3(0f, -0.5f, 0f),
			v2 = new Vector3(0f, 0f, -0.5f),
			num = new int[4] { 2, 3, 1, 0 },
			uvY = new float[2][]
			{
				new float[4] { 0f, 0.5f, 0.5f, 0f },
				null
			},
			uvX = new float[2][]
			{
				new float[4] { 0f, 0f, 0.5f, 0.5f },
				new float[4] { 0.5f, 0.5f, 1f, 1f }
			}
		},
		Left = new SideAssociation
		{
			v1 = new Vector3(0f, -0.5f, 0f),
			v2 = new Vector3(0f, 0f, -0.5f),
			num = new int[4] { 1, 0, 2, 3 },
			uvY = new float[2][]
			{
				new float[4] { 0f, 0.5f, 0.5f, 0f },
				null
			},
			uvX = new float[2][]
			{
				new float[4] { 0.5f, 0.5f, 1f, 1f },
				new float[4] { 0f, 0f, 0.5f, 0.5f }
			}
		}
	};

	public BlockFaceStair this[int i] => (BlockFaceStair)Faces[i];

	static StairAssociation()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0506: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0668: Unknown result type (might be due to invalid IL or missing references)
		//IL_066d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0682: Unknown result type (might be due to invalid IL or missing references)
		//IL_0687: Unknown result type (might be due to invalid IL or missing references)
		//IL_0702: Unknown result type (might be due to invalid IL or missing references)
		//IL_0707: Unknown result type (might be due to invalid IL or missing references)
		//IL_071c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0721: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07db: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_093d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0942: Unknown result type (might be due to invalid IL or missing references)
		//IL_0957: Unknown result type (might be due to invalid IL or missing references)
		//IL_095c: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_09dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a96: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab5: Unknown result type (might be due to invalid IL or missing references)
	}

	public static StairAssociation GetAssociation(BlockKind kind)
	{
		return kind switch
		{
			BlockKind.StairEast => East, 
			BlockKind.StairWest => West, 
			BlockKind.StairNorth => North, 
			BlockKind.StairSouth => South, 
			_ => null, 
		};
	}
}
