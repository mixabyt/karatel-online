using System;
using UnityEngine;

public abstract class UIWidget : MonoBehaviour
{
	public enum Pivot
	{
		TopLeft,
		Top,
		TopRight,
		Left,
		Center,
		Right,
		BottomLeft,
		Bottom,
		BottomRight
	}

	[SerializeField]
	[HideInInspector]
	protected Material mMat;

	[SerializeField]
	[HideInInspector]
	protected Texture mTex;

	[HideInInspector]
	[SerializeField]
	private Color mColor = Color.white;

	[SerializeField]
	[HideInInspector]
	private Pivot mPivot = Pivot.Center;

	[HideInInspector]
	[SerializeField]
	private int mDepth;

	protected GameObject mGo;

	protected Transform mTrans;

	protected UIPanel mPanel;

	protected bool mChanged = true;

	protected bool mPlayMode = true;

	private Vector3 mDiffPos;

	private Quaternion mDiffRot;

	private Vector3 mDiffScale;

	private Matrix4x4 mLocalToPanel;

	private bool mVisibleByPanel = true;

	private float mLastAlpha;

	private UIGeometry mGeom = new UIGeometry();

	private bool mForceVisible;

	private Vector3 mOldV0;

	private Vector3 mOldV1;

	public bool isVisible => mVisibleByPanel && finalAlpha > 0.001f;

	public Color color
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mColor;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			if (!mColor.Equals((object)value))
			{
				mColor = value;
				mChanged = true;
			}
		}
	}

	public float alpha
	{
		get
		{
			return mColor.a;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			Color val = mColor;
			val.a = value;
			color = val;
		}
	}

	public float finalAlpha
	{
		get
		{
			if ((Object)(object)mPanel == (Object)null)
			{
				CreatePanel();
			}
			return (!((Object)(object)mPanel != (Object)null)) ? mColor.a : (mColor.a * mPanel.alpha);
		}
	}

	public Pivot pivot
	{
		get
		{
			return mPivot;
		}
		set
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			if (mPivot != value)
			{
				Vector3 val = NGUIMath.CalculateWidgetCorners(this)[0];
				mPivot = value;
				mChanged = true;
				Vector3 val2 = NGUIMath.CalculateWidgetCorners(this)[0];
				Transform val3 = cachedTransform;
				Vector3 position = val3.position;
				float z = val3.localPosition.z;
				position.x += val.x - val2.x;
				position.y += val.y - val2.y;
				cachedTransform.position = position;
				position = cachedTransform.localPosition;
				position.x = Mathf.Round(position.x);
				position.y = Mathf.Round(position.y);
				position.z = z;
				cachedTransform.localPosition = position;
			}
		}
	}

	public int depth
	{
		get
		{
			return mDepth;
		}
		set
		{
			if (mDepth != value)
			{
				mDepth = value;
				if ((Object)(object)mPanel != (Object)null)
				{
					mPanel.MarkMaterialAsChanged(material, sort: true);
				}
			}
		}
	}

	public Vector2 pivotOffset
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			Vector2 zero = Vector2.zero;
			Vector4 val = relativePadding;
			Pivot pivot = this.pivot;
			switch (pivot)
			{
			case Pivot.Top:
			case Pivot.Center:
			case Pivot.Bottom:
				zero.x = (val.x - val.z - 1f) * 0.5f;
				break;
			case Pivot.TopRight:
			case Pivot.Right:
			case Pivot.BottomRight:
				zero.x = -1f - val.z;
				break;
			default:
				zero.x = val.x;
				break;
			}
			switch (pivot)
			{
			case Pivot.Left:
			case Pivot.Center:
			case Pivot.Right:
				zero.y = (val.w - val.y + 1f) * 0.5f;
				break;
			case Pivot.BottomLeft:
			case Pivot.Bottom:
			case Pivot.BottomRight:
				zero.y = 1f + val.w;
				break;
			default:
				zero.y = 0f - val.y;
				break;
			}
			return zero;
		}
	}

	public GameObject cachedGameObject
	{
		get
		{
			if ((Object)(object)mGo == (Object)null)
			{
				mGo = ((Component)this).gameObject;
			}
			return mGo;
		}
	}

	public Transform cachedTransform
	{
		get
		{
			if ((Object)(object)mTrans == (Object)null)
			{
				mTrans = ((Component)this).transform;
			}
			return mTrans;
		}
	}

	public virtual Material material
	{
		get
		{
			return mMat;
		}
		set
		{
			if ((Object)(object)mMat != (Object)(object)value)
			{
				if ((Object)(object)mMat != (Object)null && (Object)(object)mPanel != (Object)null)
				{
					mPanel.RemoveWidget(this);
				}
				mPanel = null;
				mMat = value;
				mTex = null;
				if ((Object)(object)mMat != (Object)null)
				{
					CreatePanel();
				}
			}
		}
	}

	public virtual Texture mainTexture
	{
		get
		{
			Material val = material;
			if ((Object)(object)val != (Object)null)
			{
				if ((Object)(object)val.mainTexture != (Object)null)
				{
					mTex = val.mainTexture;
				}
				else if ((Object)(object)mTex != (Object)null)
				{
					if ((Object)(object)mPanel != (Object)null)
					{
						mPanel.RemoveWidget(this);
					}
					mPanel = null;
					mMat.mainTexture = mTex;
					if (((Behaviour)this).enabled)
					{
						CreatePanel();
					}
				}
			}
			return mTex;
		}
		set
		{
			Material val = material;
			if (!((Object)(object)val == (Object)null) && !((Object)(object)val.mainTexture != (Object)(object)value))
			{
				return;
			}
			if ((Object)(object)mPanel != (Object)null)
			{
				mPanel.RemoveWidget(this);
			}
			mPanel = null;
			mTex = value;
			val = material;
			if ((Object)(object)val != (Object)null)
			{
				val.mainTexture = value;
				if (((Behaviour)this).enabled)
				{
					CreatePanel();
				}
			}
		}
	}

	public UIPanel panel
	{
		get
		{
			if ((Object)(object)mPanel == (Object)null)
			{
				CreatePanel();
			}
			return mPanel;
		}
		set
		{
			mPanel = value;
		}
	}

	public virtual Vector2 relativeSize
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return Vector2.one;
		}
	}

	public virtual Vector4 relativePadding
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return Vector4.zero;
		}
	}

	public virtual Vector4 border
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return Vector4.zero;
		}
	}

	public virtual bool keepMaterial => false;

	public virtual bool pixelPerfectAfterResize => false;

	protected UIWidget()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
	}

	public static BetterList<UIWidget> Raycast(GameObject root, Vector2 mousePos)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		BetterList<UIWidget> betterList = new BetterList<UIWidget>();
		UICamera uICamera = UICamera.FindCameraForLayer(root.layer);
		if ((Object)(object)uICamera != (Object)null)
		{
			Camera cachedCamera = uICamera.cachedCamera;
			UIWidget[] componentsInChildren = root.GetComponentsInChildren<UIWidget>();
			foreach (UIWidget uIWidget in componentsInChildren)
			{
				Vector3[] worldPoints = NGUIMath.CalculateWidgetCorners(uIWidget);
				if (NGUIMath.DistanceToRectangle(worldPoints, mousePos, cachedCamera) == 0f)
				{
					betterList.Add(uIWidget);
				}
			}
			betterList.Sort((UIWidget w1, UIWidget w2) => w2.mDepth.CompareTo(w1.mDepth));
		}
		return betterList;
	}

	public static int CompareFunc(UIWidget left, UIWidget right)
	{
		if (left.mDepth > right.mDepth)
		{
			return 1;
		}
		if (left.mDepth < right.mDepth)
		{
			return -1;
		}
		return 0;
	}

	public void MarkAsChangedLite()
	{
		mChanged = true;
	}

	public virtual void MarkAsChanged()
	{
		mChanged = true;
		if ((Object)(object)mPanel != (Object)null && ((Behaviour)this).enabled && NGUITools.GetActive(((Component)this).gameObject) && !Application.isPlaying && (Object)(object)material != (Object)null)
		{
			mPanel.AddWidget(this);
			CheckLayer();
		}
	}

	public void CreatePanel()
	{
		if ((Object)(object)mPanel == (Object)null && ((Behaviour)this).enabled && NGUITools.GetActive(((Component)this).gameObject) && (Object)(object)material != (Object)null)
		{
			mPanel = UIPanel.Find(cachedTransform);
			if ((Object)(object)mPanel != (Object)null)
			{
				CheckLayer();
				mPanel.AddWidget(this);
				mChanged = true;
			}
		}
	}

	public void CheckLayer()
	{
		if ((Object)(object)mPanel != (Object)null && ((Component)mPanel).gameObject.layer != ((Component)this).gameObject.layer)
		{
			Debug.LogWarning((object)"You can't place widgets on a layer different than the UIPanel that manages them.\nIf you want to move widgets to a different layer, parent them to a new panel instead.", (Object)(object)this);
			((Component)this).gameObject.layer = ((Component)mPanel).gameObject.layer;
		}
	}

	[Obsolete("Use ParentHasChanged() instead")]
	public void CheckParent()
	{
		ParentHasChanged();
	}

	public void ParentHasChanged()
	{
		if (!((Object)(object)mPanel != (Object)null))
		{
			return;
		}
		UIPanel uIPanel = UIPanel.Find(cachedTransform);
		if ((Object)(object)mPanel != (Object)(object)uIPanel)
		{
			mPanel.RemoveWidget(this);
			if (!keepMaterial || Application.isPlaying)
			{
				material = null;
			}
			mPanel = null;
			CreatePanel();
		}
	}

	protected virtual void Awake()
	{
		mGo = ((Component)this).gameObject;
		mPlayMode = Application.isPlaying;
	}

	protected virtual void OnEnable()
	{
		mChanged = true;
		if (!keepMaterial)
		{
			mMat = null;
			mTex = null;
		}
		mPanel = null;
	}

	private void Start()
	{
		OnStart();
		CreatePanel();
	}

	public virtual void Update()
	{
		if ((Object)(object)mPanel == (Object)null)
		{
			CreatePanel();
		}
	}

	private void OnDisable()
	{
		if (!keepMaterial)
		{
			material = null;
		}
		else if ((Object)(object)mPanel != (Object)null)
		{
			mPanel.RemoveWidget(this);
		}
		mPanel = null;
	}

	private void OnDestroy()
	{
		if ((Object)(object)mPanel != (Object)null)
		{
			mPanel.RemoveWidget(this);
			mPanel = null;
		}
	}

	public bool UpdateGeometry(UIPanel p, bool forceVisible)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)material != (Object)null && (Object)(object)p != (Object)null)
		{
			mPanel = p;
			bool flag = false;
			float num = finalAlpha;
			bool flag2 = num > 0.001f;
			bool flag3 = forceVisible || mVisibleByPanel;
			if (cachedTransform.hasChanged)
			{
				mTrans.hasChanged = false;
				if (!mPanel.widgetsAreStatic)
				{
					Vector2 val = relativeSize;
					Vector2 val2 = pivotOffset;
					Vector4 val3 = relativePadding;
					float num2 = val2.x * val.x - val3.x;
					float num3 = val2.y * val.y + val3.y;
					float num4 = num2 + val.x + val3.x + val3.z;
					float num5 = num3 - val.y - val3.y - val3.w;
					mLocalToPanel = p.worldToLocal * cachedTransform.localToWorldMatrix;
					flag = true;
					Vector3 val4 = new Vector3(num2, num3, 0f);
					Vector3 val5 = new Vector3(num4, num5, 0f);
					val4 = mLocalToPanel.MultiplyPoint3x4(val4);
					val5 = mLocalToPanel.MultiplyPoint3x4(val5);
					if (Vector3.SqrMagnitude(mOldV0 - val4) > 1E-06f || Vector3.SqrMagnitude(mOldV1 - val5) > 1E-06f)
					{
						mChanged = true;
						mOldV0 = val4;
						mOldV1 = val5;
					}
				}
				if (flag2 || mForceVisible != forceVisible)
				{
					mForceVisible = forceVisible;
					flag3 = forceVisible || mPanel.IsVisible(this);
				}
			}
			else if (flag2 && mForceVisible != forceVisible)
			{
				mForceVisible = forceVisible;
				flag3 = mPanel.IsVisible(this);
			}
			if (mVisibleByPanel != flag3)
			{
				mVisibleByPanel = flag3;
				mChanged = true;
			}
			if (mVisibleByPanel && mLastAlpha != num)
			{
				mChanged = true;
			}
			mLastAlpha = num;
			if (mChanged)
			{
				mChanged = false;
				if (isVisible)
				{
					mGeom.Clear();
					OnFill(mGeom.verts, mGeom.uvs, mGeom.cols);
					if (mGeom.hasVertices)
					{
						Vector3 val6 = Vector2.op_Implicit(pivotOffset);
						Vector2 val7 = relativeSize;
						val6.x *= val7.x;
						val6.y *= val7.y;
						if (!flag)
						{
							mLocalToPanel = p.worldToLocal * cachedTransform.localToWorldMatrix;
						}
						mGeom.ApplyOffset(val6);
						mGeom.ApplyTransform(mLocalToPanel);
					}
					return true;
				}
				if (mGeom.hasVertices)
				{
					mGeom.Clear();
					return true;
				}
			}
		}
		return false;
	}

	public void WriteToBuffers(BetterList<Vector3> v, BetterList<Vector2> u, BetterList<Color32> c, BetterList<Vector3> n, BetterList<Vector4> t)
	{
		mGeom.WriteToBuffers(v, u, c, n, t);
	}

	public virtual void MakePixelPerfect()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		Vector3 localScale = cachedTransform.localScale;
		int num = Mathf.RoundToInt(localScale.x);
		int num2 = Mathf.RoundToInt(localScale.y);
		localScale.x = num;
		localScale.y = num2;
		localScale.z = 1f;
		Vector3 localPosition = cachedTransform.localPosition;
		localPosition.z = Mathf.RoundToInt(localPosition.z);
		if (num % 2 == 1 && (pivot == Pivot.Top || pivot == Pivot.Center || pivot == Pivot.Bottom))
		{
			localPosition.x = Mathf.Floor(localPosition.x) + 0.5f;
		}
		else
		{
			localPosition.x = Mathf.Round(localPosition.x);
		}
		if (num2 % 2 == 1 && (pivot == Pivot.Left || pivot == Pivot.Center || pivot == Pivot.Right))
		{
			localPosition.y = Mathf.Ceil(localPosition.y) - 0.5f;
		}
		else
		{
			localPosition.y = Mathf.Round(localPosition.y);
		}
		cachedTransform.localPosition = localPosition;
		cachedTransform.localScale = localScale;
	}

	protected virtual void OnStart()
	{
	}

	public virtual void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
	}
}
