using UnityEngine;

[AddComponentMenu("NGUI/UI/Panel")]
[ExecuteInEditMode]
public class UIPanel : MonoBehaviour
{
	public enum DebugInfo
	{
		None,
		Gizmos,
		Geometry
	}

	public delegate void OnChangeDelegate();

	public OnChangeDelegate onChange;

	public bool showInPanelTool = true;

	public bool generateNormals;

	public bool depthPass;

	public bool widgetsAreStatic;

	public bool cullWhileDragging;

	[HideInInspector]
	public Matrix4x4 worldToLocal = Matrix4x4.identity;

	[SerializeField]
	[HideInInspector]
	private float mAlpha = 1f;

	[HideInInspector]
	[SerializeField]
	private DebugInfo mDebugInfo = DebugInfo.Gizmos;

	[SerializeField]
	[HideInInspector]
	private UIDrawCall.Clipping mClipping;

	[HideInInspector]
	[SerializeField]
	private Vector4 mClipRange = Vector4.zero;

	[SerializeField]
	[HideInInspector]
	private Vector2 mClipSoftness = new Vector2(40f, 40f);

	private BetterList<UIWidget> mWidgets = new BetterList<UIWidget>();

	private BetterList<Material> mChanged = new BetterList<Material>();

	private BetterList<UIDrawCall> mDrawCalls = new BetterList<UIDrawCall>();

	private BetterList<Vector3> mVerts = new BetterList<Vector3>();

	private BetterList<Vector3> mNorms = new BetterList<Vector3>();

	private BetterList<Vector4> mTans = new BetterList<Vector4>();

	private BetterList<Vector2> mUvs = new BetterList<Vector2>();

	private BetterList<Color32> mCols = new BetterList<Color32>();

	private GameObject mGo;

	private Transform mTrans;

	private Camera mCam;

	private int mLayer = -1;

	private bool mDepthChanged;

	private float mCullTime;

	private float mUpdateTime;

	private float mMatrixTime;

	private static float[] mTemp = new float[4];

	private Vector2 mMin = Vector2.zero;

	private Vector2 mMax = Vector2.zero;

	private UIPanel[] mChildPanels;

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

	public float alpha
	{
		get
		{
			return mAlpha;
		}
		set
		{
			float num = Mathf.Clamp01(value);
			if (mAlpha != num)
			{
				mAlpha = num;
				for (int i = 0; i < mDrawCalls.size; i++)
				{
					UIDrawCall uIDrawCall = mDrawCalls[i];
					MarkMaterialAsChanged(uIDrawCall.material, sort: false);
				}
				for (int j = 0; j < mWidgets.size; j++)
				{
					mWidgets[j].MarkAsChangedLite();
				}
			}
		}
	}

	public DebugInfo debugInfo
	{
		get
		{
			return mDebugInfo;
		}
		set
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			if (mDebugInfo != value)
			{
				mDebugInfo = value;
				BetterList<UIDrawCall> betterList = drawCalls;
				HideFlags hideFlags = (HideFlags)((mDebugInfo != DebugInfo.Geometry) ? 13 : 12);
				int i = 0;
				for (int size = betterList.size; i < size; i++)
				{
					UIDrawCall uIDrawCall = betterList[i];
					GameObject gameObject = ((Component)uIDrawCall).gameObject;
					NGUITools.SetActiveSelf(gameObject, state: false);
					((Object)gameObject).hideFlags = hideFlags;
					NGUITools.SetActiveSelf(gameObject, state: true);
				}
			}
		}
	}

	public UIDrawCall.Clipping clipping
	{
		get
		{
			return mClipping;
		}
		set
		{
			if (mClipping != value)
			{
				mClipping = value;
				mMatrixTime = 0f;
				UpdateDrawcalls();
			}
		}
	}

	public Vector4 clipRange
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mClipRange;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			if (mClipRange != value)
			{
				mCullTime = ((mCullTime != 0f) ? (Time.realtimeSinceStartup + 0.15f) : 0.001f);
				mClipRange = value;
				mMatrixTime = 0f;
				UpdateDrawcalls();
			}
		}
	}

	public Vector2 clipSoftness
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mClipSoftness;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (mClipSoftness != value)
			{
				mClipSoftness = value;
				UpdateDrawcalls();
			}
		}
	}

	public BetterList<UIWidget> widgets => mWidgets;

	public BetterList<UIDrawCall> drawCalls
	{
		get
		{
			int num = mDrawCalls.size;
			while (num > 0)
			{
				UIDrawCall uIDrawCall = mDrawCalls[--num];
				if ((Object)(object)uIDrawCall == (Object)null)
				{
					mDrawCalls.RemoveAt(num);
				}
			}
			return mDrawCalls;
		}
	}

	public UIPanel()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
	}

	public void SetAlphaRecursive(float val, bool rebuildList)
	{
		if (rebuildList || mChildPanels == null)
		{
			mChildPanels = ((Component)this).GetComponentsInChildren<UIPanel>(true);
		}
		int i = 0;
		for (int num = mChildPanels.Length; i < num; i++)
		{
			mChildPanels[i].alpha = val;
		}
	}

	private bool IsVisible(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		UpdateTransformMatrix();
		a = worldToLocal.MultiplyPoint3x4(a);
		b = worldToLocal.MultiplyPoint3x4(b);
		c = worldToLocal.MultiplyPoint3x4(c);
		d = worldToLocal.MultiplyPoint3x4(d);
		mTemp[0] = a.x;
		mTemp[1] = b.x;
		mTemp[2] = c.x;
		mTemp[3] = d.x;
		float num = Mathf.Min(mTemp);
		float num2 = Mathf.Max(mTemp);
		mTemp[0] = a.y;
		mTemp[1] = b.y;
		mTemp[2] = c.y;
		mTemp[3] = d.y;
		float num3 = Mathf.Min(mTemp);
		float num4 = Mathf.Max(mTemp);
		if (num2 < mMin.x)
		{
			return false;
		}
		if (num4 < mMin.y)
		{
			return false;
		}
		if (num > mMax.x)
		{
			return false;
		}
		if (num3 > mMax.y)
		{
			return false;
		}
		return true;
	}

	public bool IsVisible(Vector3 worldPos)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (mAlpha < 0.001f)
		{
			return false;
		}
		if (mClipping == UIDrawCall.Clipping.None)
		{
			return true;
		}
		UpdateTransformMatrix();
		Vector3 val = worldToLocal.MultiplyPoint3x4(worldPos);
		if (val.x < mMin.x)
		{
			return false;
		}
		if (val.y < mMin.y)
		{
			return false;
		}
		if (val.x > mMax.x)
		{
			return false;
		}
		if (val.y > mMax.y)
		{
			return false;
		}
		return true;
	}

	public bool IsVisible(UIWidget w)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		if (mAlpha < 0.001f)
		{
			return false;
		}
		if (!((Behaviour)w).enabled || !NGUITools.GetActive(w.cachedGameObject) || w.alpha < 0.001f)
		{
			return false;
		}
		if (mClipping == UIDrawCall.Clipping.None)
		{
			return true;
		}
		Vector2 relativeSize = w.relativeSize;
		Vector2 val = Vector2.Scale(w.pivotOffset, relativeSize);
		Vector2 val2 = val;
		val.x += relativeSize.x;
		val.y -= relativeSize.y;
		Transform val3 = w.cachedTransform;
		Vector3 a = val3.TransformPoint(Vector2.op_Implicit(val));
		Vector3 b = val3.TransformPoint(Vector2.op_Implicit(new Vector2(val.x, val2.y)));
		Vector3 c = val3.TransformPoint(Vector2.op_Implicit(new Vector2(val2.x, val.y)));
		Vector3 d = val3.TransformPoint(Vector2.op_Implicit(val2));
		return IsVisible(a, b, c, d);
	}

	public void MarkMaterialAsChanged(Material mat, bool sort)
	{
		if ((Object)(object)mat != (Object)null)
		{
			if (sort)
			{
				mDepthChanged = true;
			}
			if (!mChanged.Contains(mat))
			{
				mChanged.Add(mat);
			}
		}
	}

	public void AddWidget(UIWidget w)
	{
		if ((Object)(object)w != (Object)null && !mWidgets.Contains(w))
		{
			mWidgets.Add(w);
			if (!mChanged.Contains(w.material))
			{
				mChanged.Add(w.material);
			}
			mDepthChanged = true;
		}
	}

	public void RemoveWidget(UIWidget w)
	{
		if ((Object)(object)w != (Object)null && (Object)(object)w != (Object)null && mWidgets.Remove(w) && (Object)(object)w.material != (Object)null)
		{
			mChanged.Add(w.material);
		}
	}

	private UIDrawCall GetDrawCall(Material mat, bool createIfMissing)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected Obj, but got Unknown
		int i = 0;
		for (int size = drawCalls.size; i < size; i++)
		{
			UIDrawCall uIDrawCall = drawCalls.buffer[i];
			if ((Object)(object)uIDrawCall.material == (Object)(object)mat)
			{
				return uIDrawCall;
			}
		}
		UIDrawCall uIDrawCall2 = null;
		if (createIfMissing)
		{
			GameObject val = new GameObject("_UIDrawCall [" + ((Object)mat).name + "]");
			Object.DontDestroyOnLoad((Object)(object)val);
			val.layer = cachedGameObject.layer;
			uIDrawCall2 = val.AddComponent<UIDrawCall>();
			uIDrawCall2.material = mat;
			mDrawCalls.Add(uIDrawCall2);
		}
		return uIDrawCall2;
	}

	private void Awake()
	{
		mGo = ((Component)this).gameObject;
		mTrans = ((Component)this).transform;
	}

	private void Start()
	{
		mLayer = mGo.layer;
		UICamera uICamera = UICamera.FindCameraForLayer(mLayer);
		mCam = ((!((Object)(object)uICamera != (Object)null)) ? NGUITools.FindCameraForLayer(mLayer) : uICamera.cachedCamera);
	}

	private void OnEnable()
	{
		int num = 0;
		while (num < mWidgets.size)
		{
			UIWidget uIWidget = mWidgets.buffer[num];
			if ((Object)(object)uIWidget != (Object)null)
			{
				MarkMaterialAsChanged(uIWidget.material, sort: true);
				num++;
			}
			else
			{
				mWidgets.RemoveAt(num);
			}
		}
	}

	private void OnDisable()
	{
		int num = mDrawCalls.size;
		while (num > 0)
		{
			UIDrawCall uIDrawCall = mDrawCalls.buffer[--num];
			if ((Object)(object)uIDrawCall != (Object)null)
			{
				NGUITools.DestroyImmediate((Object)(object)((Component)uIDrawCall).gameObject);
			}
		}
		mDrawCalls.Release();
		mChanged.Release();
	}

	private void UpdateTransformMatrix()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		if (mUpdateTime != 0f && mMatrixTime == mUpdateTime)
		{
			return;
		}
		mMatrixTime = mUpdateTime;
		worldToLocal = cachedTransform.worldToLocalMatrix;
		if (mClipping != UIDrawCall.Clipping.None)
		{
			Vector2 val = new Vector2(mClipRange.z, mClipRange.w);
			if (val.x == 0f)
			{
				val.x = ((!((Object)(object)mCam == (Object)null)) ? mCam.pixelWidth : ((float)Screen.width));
			}
			if (val.y == 0f)
			{
				val.y = ((!((Object)(object)mCam == (Object)null)) ? mCam.pixelHeight : ((float)Screen.height));
			}
			val *= 0.5f;
			mMin.x = mClipRange.x - val.x;
			mMin.y = mClipRange.y - val.y;
			mMax.x = mClipRange.x + val.x;
			mMax.y = mClipRange.y + val.y;
		}
	}

	public void UpdateDrawcalls()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Invalid comparison between Unknown and I4
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Invalid comparison between Unknown and I4
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Invalid comparison between Unknown and I4
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		Vector4 val = Vector4.zero;
		if (mClipping != UIDrawCall.Clipping.None)
		{
			val = new Vector4(mClipRange.x, mClipRange.y, mClipRange.z * 0.5f, mClipRange.w * 0.5f);
		}
		if (val.z == 0f)
		{
			val.z = (float)Screen.width * 0.5f;
		}
		if (val.w == 0f)
		{
			val.w = (float)Screen.height * 0.5f;
		}
		RuntimePlatform platform = Application.platform;
		if ((int)platform == 2 || (int)platform == 5 || (int)platform == 7)
		{
			val.x -= 0.5f;
			val.y += 0.5f;
		}
		Transform val2 = cachedTransform;
		int i = 0;
		for (int size = mDrawCalls.size; i < size; i++)
		{
			UIDrawCall uIDrawCall = mDrawCalls.buffer[i];
			uIDrawCall.clipping = mClipping;
			uIDrawCall.clipRange = val;
			uIDrawCall.clipSoftness = mClipSoftness;
			uIDrawCall.depthPass = depthPass && mClipping == UIDrawCall.Clipping.None;
			Transform transform = ((Component)uIDrawCall).transform;
			transform.position = val2.position;
			transform.rotation = val2.rotation;
			transform.localScale = val2.lossyScale;
		}
	}

	private void Fill(Material mat)
	{
		int num = 0;
		while (num < mWidgets.size)
		{
			UIWidget uIWidget = mWidgets.buffer[num];
			if ((Object)(object)uIWidget == (Object)null)
			{
				mWidgets.RemoveAt(num);
				continue;
			}
			if ((Object)(object)uIWidget.material == (Object)(object)mat && uIWidget.isVisible)
			{
				if (!((Object)(object)uIWidget.panel == (Object)(object)this))
				{
					mWidgets.RemoveAt(num);
					continue;
				}
				if (generateNormals)
				{
					uIWidget.WriteToBuffers(mVerts, mUvs, mCols, mNorms, mTans);
				}
				else
				{
					uIWidget.WriteToBuffers(mVerts, mUvs, mCols, null, null);
				}
			}
			num++;
		}
		if (mVerts.size > 0)
		{
			UIDrawCall drawCall = GetDrawCall(mat, createIfMissing: true);
			drawCall.depthPass = depthPass && mClipping == UIDrawCall.Clipping.None;
			drawCall.Set(mVerts, (!generateNormals) ? null : mNorms, (!generateNormals) ? null : mTans, mUvs, mCols);
		}
		else
		{
			UIDrawCall drawCall2 = GetDrawCall(mat, createIfMissing: false);
			if ((Object)(object)drawCall2 != (Object)null)
			{
				mDrawCalls.Remove(drawCall2);
				NGUITools.DestroyImmediate((Object)(object)((Component)drawCall2).gameObject);
			}
		}
		mVerts.Clear();
		mNorms.Clear();
		mTans.Clear();
		mUvs.Clear();
		mCols.Clear();
	}

	private void LateUpdate()
	{
		mUpdateTime = Time.realtimeSinceStartup;
		UpdateTransformMatrix();
		if (mLayer != cachedGameObject.layer)
		{
			mLayer = mGo.layer;
			UICamera uICamera = UICamera.FindCameraForLayer(mLayer);
			mCam = ((!((Object)(object)uICamera != (Object)null)) ? NGUITools.FindCameraForLayer(mLayer) : uICamera.cachedCamera);
			SetChildLayer(cachedTransform, mLayer);
			int i = 0;
			for (int size = drawCalls.size; i < size; i++)
			{
				((Component)mDrawCalls.buffer[i]).gameObject.layer = mLayer;
			}
		}
		bool forceVisible = !cullWhileDragging && (clipping == UIDrawCall.Clipping.None || mCullTime > mUpdateTime);
		int j = 0;
		for (int size2 = mWidgets.size; j < size2; j++)
		{
			UIWidget uIWidget = mWidgets[j];
			if (uIWidget.UpdateGeometry(this, forceVisible) && !mChanged.Contains(uIWidget.material))
			{
				mChanged.Add(uIWidget.material);
			}
		}
		if (mChanged.size != 0 && onChange != null)
		{
			onChange();
		}
		if (mDepthChanged)
		{
			mDepthChanged = false;
			mWidgets.Sort(UIWidget.CompareFunc);
		}
		int k = 0;
		for (int size3 = mChanged.size; k < size3; k++)
		{
			Fill(mChanged.buffer[k]);
		}
		UpdateDrawcalls();
		mChanged.Clear();
	}

	public void Refresh()
	{
		UIWidget[] componentsInChildren = ((Component)this).GetComponentsInChildren<UIWidget>();
		int i = 0;
		for (int num = componentsInChildren.Length; i < num; i++)
		{
			componentsInChildren[i].Update();
		}
		LateUpdate();
	}

	public Vector3 CalculateConstrainOffset(Vector2 min, Vector2 max)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		float num = clipRange.z * 0.5f;
		float num2 = clipRange.w * 0.5f;
		Vector2 minRect = new Vector2(min.x, min.y);
		Vector2 maxRect = new Vector2(max.x, max.y);
		Vector2 minArea = new Vector2(clipRange.x - num, clipRange.y - num2);
		Vector2 maxArea = new Vector2(clipRange.x + num, clipRange.y + num2);
		if (clipping == UIDrawCall.Clipping.SoftClip)
		{
			minArea.x += clipSoftness.x;
			minArea.y += clipSoftness.y;
			maxArea.x -= clipSoftness.x;
			maxArea.y -= clipSoftness.y;
		}
		return Vector2.op_Implicit(NGUIMath.ConstrainRect(minRect, maxRect, minArea, maxArea));
	}

	public bool ConstrainTargetToBounds(Transform target, ref Bounds targetBounds, bool immediate)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = CalculateConstrainOffset(Vector2.op_Implicit(targetBounds.min), Vector2.op_Implicit(targetBounds.max));
		if (val.magnitude > 0f)
		{
			if (immediate)
			{
				target.localPosition += val;
				targetBounds.center += val;
				SpringPosition component = ((Component)target).GetComponent<SpringPosition>();
				if ((Object)(object)component != (Object)null)
				{
					((Behaviour)component).enabled = false;
				}
			}
			else
			{
				SpringPosition springPosition = SpringPosition.Begin(((Component)target).gameObject, target.localPosition + val, 13f);
				springPosition.ignoreTimeScale = true;
				springPosition.worldSpace = false;
			}
			return true;
		}
		return false;
	}

	public bool ConstrainTargetToBounds(Transform target, bool immediate)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Bounds targetBounds = NGUIMath.CalculateRelativeWidgetBounds(cachedTransform, target);
		return ConstrainTargetToBounds(target, ref targetBounds, immediate);
	}

	private static void SetChildLayer(Transform t, int layer)
	{
		for (int i = 0; i < t.childCount; i++)
		{
			Transform child = t.GetChild(i);
			if ((Object)(object)((Component)child).GetComponent<UIPanel>() == (Object)null)
			{
				if ((Object)(object)((Component)child).GetComponent<UIWidget>() != (Object)null)
				{
					((Component)child).gameObject.layer = layer;
				}
				SetChildLayer(child, layer);
			}
		}
	}

	public static UIPanel Find(Transform trans, bool createIfMissing)
	{
		Transform val = trans;
		UIPanel uIPanel = null;
		while ((Object)(object)uIPanel == (Object)null && (Object)(object)trans != (Object)null)
		{
			uIPanel = ((Component)trans).GetComponent<UIPanel>();
			if ((Object)(object)uIPanel != (Object)null || (Object)(object)trans.parent == (Object)null)
			{
				break;
			}
			trans = trans.parent;
		}
		if (createIfMissing && (Object)(object)uIPanel == (Object)null && (Object)(object)trans != (Object)(object)val)
		{
			uIPanel = ((Component)trans).gameObject.AddComponent<UIPanel>();
			SetChildLayer(uIPanel.cachedTransform, uIPanel.cachedGameObject.layer);
		}
		return uIPanel;
	}

	public static UIPanel Find(Transform trans)
	{
		return Find(trans, createIfMissing: true);
	}
}
