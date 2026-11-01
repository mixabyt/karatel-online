using UnityEngine;

[AddComponentMenu("NGUI/UI/Texture")]
[ExecuteInEditMode]
public class UITexture : UIWidget
{
	[SerializeField]
	[HideInInspector]
	private Rect mRect = new Rect(0f, 0f, 1f, 1f);

	[HideInInspector]
	[SerializeField]
	private Shader mShader;

	[HideInInspector]
	[SerializeField]
	private Texture mTexture;

	private Material mDynamicMat;

	private bool mCreatingMat;

	private int mPMA = -1;

	public Rect uvRect
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mRect;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (mRect != value)
			{
				mRect = value;
				MarkAsChanged();
			}
		}
	}

	public Shader shader
	{
		get
		{
			if ((Object)(object)mShader == (Object)null)
			{
				Material val = material;
				if ((Object)(object)val != (Object)null)
				{
					mShader = val.shader;
				}
				if ((Object)(object)mShader == (Object)null)
				{
					mShader = Shader.Find("Unlit/Texture");
				}
			}
			return mShader;
		}
		set
		{
			if ((Object)(object)mShader != (Object)(object)value)
			{
				mShader = value;
				Material val = material;
				if ((Object)(object)val != (Object)null)
				{
					val.shader = value;
				}
				mPMA = -1;
			}
		}
	}

	public bool hasDynamicMaterial => (Object)(object)mDynamicMat != (Object)null;

	public override bool keepMaterial => true;

	public override Material material
	{
		get
		{
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Expected Obj, but got Unknown
			if (!mCreatingMat && (Object)(object)mMat == (Object)null)
			{
				mCreatingMat = true;
				if ((Object)(object)mainTexture != (Object)null)
				{
					if ((Object)(object)mShader == (Object)null)
					{
						mShader = Shader.Find("Unlit/Texture");
					}
					mDynamicMat = new Material(mShader);
					((Object)mDynamicMat).hideFlags = (HideFlags)4;
					mDynamicMat.mainTexture = mainTexture;
					base.material = mDynamicMat;
					mPMA = 0;
				}
				mCreatingMat = false;
			}
			return mMat;
		}
		set
		{
			if ((Object)(object)mDynamicMat != (Object)(object)value && (Object)(object)mDynamicMat != (Object)null)
			{
				NGUITools.Destroy((Object)(object)mDynamicMat);
				mDynamicMat = null;
			}
			base.material = value;
			mPMA = -1;
		}
	}

	public bool premultipliedAlpha
	{
		get
		{
			if (mPMA == -1)
			{
				Material val = material;
				mPMA = (((Object)(object)val != (Object)null && (Object)(object)val.shader != (Object)null && ((Object)val.shader).name.Contains("Premultiplied")) ? 1 : 0);
			}
			return mPMA == 1;
		}
	}

	public override Texture mainTexture
	{
		get
		{
			return (!((Object)(object)mTexture != (Object)null)) ? base.mainTexture : mTexture;
		}
		set
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Expected Obj, but got Unknown
			if ((Object)(object)mPanel != (Object)null && (Object)(object)mMat != (Object)null)
			{
				mPanel.RemoveWidget(this);
			}
			if ((Object)(object)mMat == (Object)null)
			{
				mDynamicMat = new Material(shader);
				((Object)mDynamicMat).hideFlags = (HideFlags)4;
				mMat = mDynamicMat;
			}
			mPanel = null;
			mTex = value;
			mTexture = value;
			mMat.mainTexture = value;
			if (((Behaviour)this).enabled)
			{
				CreatePanel();
			}
		}
	}

	public UITexture()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
	}

	private void OnDestroy()
	{
		NGUITools.Destroy((Object)(object)mDynamicMat);
	}

	public override void MakePixelPerfect()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		Texture val = mainTexture;
		if ((Object)(object)val != (Object)null)
		{
			Vector3 localScale = cachedTransform.localScale;
			float num = val.width;
			Rect val2 = uvRect;
			localScale.x = num * val2.width;
			float num2 = val.height;
			Rect val3 = uvRect;
			localScale.y = num2 * val3.height;
			localScale.z = 1f;
			cachedTransform.localScale = localScale;
		}
		base.MakePixelPerfect();
	}

	public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		Color val = color;
		val.a *= mPanel.alpha;
		Color32 item = Color32.op_Implicit((!premultipliedAlpha) ? val : NGUITools.ApplyPMA(val));
		verts.Add(new Vector3(1f, 0f, 0f));
		verts.Add(new Vector3(1f, -1f, 0f));
		verts.Add(new Vector3(0f, -1f, 0f));
		verts.Add(new Vector3(0f, 0f, 0f));
		uvs.Add(new Vector2(mRect.xMax, mRect.yMax));
		uvs.Add(new Vector2(mRect.xMax, mRect.yMin));
		uvs.Add(new Vector2(mRect.xMin, mRect.yMin));
		uvs.Add(new Vector2(mRect.xMin, mRect.yMax));
		cols.Add(item);
		cols.Add(item);
		cols.Add(item);
		cols.Add(item);
	}
}
