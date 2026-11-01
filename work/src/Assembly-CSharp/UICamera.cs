using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("NGUI/UI/Camera")]
public class UICamera : MonoBehaviour
{
	public enum ClickNotification
	{
		None,
		Always,
		BasedOnDelta
	}

	public class MouseOrTouch
	{
		public Vector2 pos;

		public Vector2 delta;

		public Vector2 totalDelta;

		public Camera pressedCam;

		public GameObject current;

		public GameObject pressed;

		public GameObject dragged;

		public float clickTime;

		public ClickNotification clickNotification = ClickNotification.Always;

		public bool touchBegan = true;

		public bool pressStarted;

		public bool dragStarted;
	}

	private class Highlighted
	{
		public GameObject go;

		public int counter;
	}

	public delegate void OnCustomInput();

	public bool debug;

	public bool useMouse = true;

	public bool useTouch = true;

	public bool allowMultiTouch = true;

	public bool useKeyboard = true;

	public bool useController = true;

	public bool stickyPress = true;

	public LayerMask eventReceiverMask = LayerMask.op_Implicit(-1);

	public bool clipRaycasts = true;

	public float tooltipDelay = 1f;

	public bool stickyTooltip = true;

	public float mouseDragThreshold = 4f;

	public float mouseClickThreshold = 10f;

	public float touchDragThreshold = 40f;

	public float touchClickThreshold = 40f;

	public float rangeDistance = -1f;

	public string scrollAxisName = "Mouse ScrollWheel";

	public string verticalAxisName = "Vertical";

	public string horizontalAxisName = "Horizontal";

	public KeyCode submitKey0 = (KeyCode)13;

	public KeyCode submitKey1 = (KeyCode)330;

	public KeyCode cancelKey0 = (KeyCode)27;

	public KeyCode cancelKey1 = (KeyCode)331;

	public static OnCustomInput onCustomInput;

	public static bool showTooltips = true;

	public static Vector2 lastTouchPosition = Vector2.zero;

	public static RaycastHit lastHit;

	public static UICamera current = null;

	public static Camera currentCamera = null;

	public static int currentTouchID = -1;

	public static MouseOrTouch currentTouch = null;

	public static bool inputHasFocus = false;

	public static GameObject genericEventHandler;

	public static GameObject fallThrough;

	private static List<UICamera> mList = new List<UICamera>();

	private static List<Highlighted> mHighlighted = new List<Highlighted>();

	private static GameObject mSel = null;

	private static MouseOrTouch[] mMouse = new MouseOrTouch[3]
	{
		new MouseOrTouch(),
		new MouseOrTouch(),
		new MouseOrTouch()
	};

	private static GameObject mHover;

	private static MouseOrTouch mController = new MouseOrTouch();

	private static float mNextEvent = 0f;

	private static Dictionary<int, MouseOrTouch> mTouches = new Dictionary<int, MouseOrTouch>();

	private GameObject mTooltip;

	private Camera mCam;

	private LayerMask mLayerMask;

	private float mTooltipTime;

	private bool mIsEditor;

	public static bool isDragging = false;

	public static GameObject hoveredObject;

	private bool handlesEvents => (Object)(object)eventHandler == (Object)(object)this;

	public Camera cachedCamera
	{
		get
		{
			if ((Object)(object)mCam == (Object)null)
			{
				mCam = ((Component)this).camera;
			}
			return mCam;
		}
	}

	public static GameObject selectedObject
	{
		get
		{
			return mSel;
		}
		set
		{
			if (!((Object)(object)mSel != (Object)(object)value))
			{
				return;
			}
			if ((Object)(object)mSel != (Object)null)
			{
				UICamera uICamera = FindCameraForLayer(mSel.layer);
				if ((Object)(object)uICamera != (Object)null)
				{
					current = uICamera;
					currentCamera = uICamera.mCam;
					Notify(mSel, "OnSelect", false);
					if (uICamera.useController || uICamera.useKeyboard)
					{
						Highlight(mSel, highlighted: false);
					}
					current = null;
				}
			}
			mSel = value;
			if (!((Object)(object)mSel != (Object)null))
			{
				return;
			}
			UICamera uICamera2 = FindCameraForLayer(mSel.layer);
			if ((Object)(object)uICamera2 != (Object)null)
			{
				current = uICamera2;
				currentCamera = uICamera2.mCam;
				if (uICamera2.useController || uICamera2.useKeyboard)
				{
					Highlight(mSel, highlighted: true);
				}
				Notify(mSel, "OnSelect", true);
				current = null;
			}
			else
			{
				Debug.Log((object)("The fuck? " + mList.Count));
			}
		}
	}

	public static int touchCount
	{
		get
		{
			int num = 0;
			for (int i = 0; i < mTouches.Count; i++)
			{
				if ((Object)(object)mTouches[i].pressed != (Object)null)
				{
					num++;
				}
			}
			for (int j = 0; j < mMouse.Length; j++)
			{
				if ((Object)(object)mMouse[j].pressed != (Object)null)
				{
					num++;
				}
			}
			if ((Object)(object)mController.pressed != (Object)null)
			{
				num++;
			}
			return num;
		}
	}

	public static int dragCount
	{
		get
		{
			int num = 0;
			for (int i = 0; i < mTouches.Count; i++)
			{
				if ((Object)(object)mTouches[i].dragged != (Object)null)
				{
					num++;
				}
			}
			for (int j = 0; j < mMouse.Length; j++)
			{
				if ((Object)(object)mMouse[j].dragged != (Object)null)
				{
					num++;
				}
			}
			if ((Object)(object)mController.dragged != (Object)null)
			{
				num++;
			}
			return num;
		}
	}

	public static Camera mainCamera
	{
		get
		{
			UICamera uICamera = eventHandler;
			return (!((Object)(object)uICamera != (Object)null)) ? null : uICamera.cachedCamera;
		}
	}

	public static UICamera eventHandler
	{
		get
		{
			for (int i = 0; i < mList.Count; i++)
			{
				UICamera uICamera = mList[i];
				if (!((Object)(object)uICamera == (Object)null) && ((Behaviour)uICamera).enabled && NGUITools.GetActive(((Component)uICamera).gameObject))
				{
					return uICamera;
				}
			}
			return null;
		}
	}

	public UICamera()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
	}

	static UICamera()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
	}

	private void OnApplicationQuit()
	{
		mHighlighted.Clear();
	}

	private static int CompareFunc(UICamera a, UICamera b)
	{
		if (a.cachedCamera.depth < b.cachedCamera.depth)
		{
			return 1;
		}
		if (a.cachedCamera.depth > b.cachedCamera.depth)
		{
			return -1;
		}
		return 0;
	}

	public static bool Raycast(Vector3 inPos, ref RaycastHit hit)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < mList.Count; i++)
		{
			UICamera uICamera = mList[i];
			if (!((Behaviour)uICamera).enabled || !NGUITools.GetActive(((Component)uICamera).gameObject))
			{
				continue;
			}
			currentCamera = uICamera.cachedCamera;
			Vector3 val = currentCamera.ScreenToViewportPoint(inPos);
			if (val.x < 0f || val.x > 1f || val.y < 0f || val.y > 1f)
			{
				continue;
			}
			Ray val2 = currentCamera.ScreenPointToRay(inPos);
			int num = currentCamera.cullingMask & LayerMask.op_Implicit(uICamera.eventReceiverMask);
			float num2 = ((!(uICamera.rangeDistance > 0f)) ? (currentCamera.farClipPlane - currentCamera.nearClipPlane) : uICamera.rangeDistance);
			if (uICamera.clipRaycasts)
			{
				RaycastHit[] array = Physics.RaycastAll(val2, num2, num);
				if (array.Length > 1)
				{
					Array.Sort(array, (RaycastHit r1, RaycastHit r2) => r1.distance.CompareTo(r2.distance));
					int num3 = 0;
					for (int num4 = array.Length; num3 < num4; num3++)
					{
						if (IsVisible(ref array[num3]))
						{
							hit = array[num3];
							return true;
						}
					}
				}
				else if (array.Length == 1 && IsVisible(ref array[0]))
				{
					hit = array[0];
					return true;
				}
			}
			else if (Physics.Raycast(val2, ref hit, num2, num))
			{
				return true;
			}
		}
		return false;
	}

	private static bool IsVisible(ref RaycastHit hit)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		UIPanel uIPanel = NGUITools.FindInParents<UIPanel>(((Component)hit.collider).gameObject);
		if ((Object)(object)uIPanel == (Object)null || uIPanel.IsVisible(hit.point))
		{
			return true;
		}
		return false;
	}

	public static UICamera FindCameraForLayer(int layer)
	{
		int num = 1 << layer;
		for (int i = 0; i < mList.Count; i++)
		{
			UICamera uICamera = mList[i];
			Camera val = uICamera.cachedCamera;
			if ((Object)(object)val != (Object)null && (val.cullingMask & num) != 0)
			{
				return uICamera;
			}
		}
		return null;
	}

	private static int GetDirection(KeyCode up, KeyCode down)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (Input.GetKeyDown(up))
		{
			return 1;
		}
		if (Input.GetKeyDown(down))
		{
			return -1;
		}
		return 0;
	}

	private static int GetDirection(KeyCode up0, KeyCode up1, KeyCode down0, KeyCode down1)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (Input.GetKeyDown(up0) || Input.GetKeyDown(up1))
		{
			return 1;
		}
		if (Input.GetKeyDown(down0) || Input.GetKeyDown(down1))
		{
			return -1;
		}
		return 0;
	}

	private static int GetDirection(string axis)
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (mNextEvent < realtimeSinceStartup)
		{
			float axis2 = Input.GetAxis(axis);
			if (axis2 > 0.75f)
			{
				mNextEvent = realtimeSinceStartup + 0.25f;
				return 1;
			}
			if (axis2 < -0.75f)
			{
				mNextEvent = realtimeSinceStartup + 0.25f;
				return -1;
			}
		}
		return 0;
	}

	public static bool IsHighlighted(GameObject go)
	{
		int num = mHighlighted.Count;
		while (num > 0)
		{
			Highlighted highlighted = mHighlighted[--num];
			if ((Object)(object)highlighted.go == (Object)(object)go)
			{
				return true;
			}
		}
		return false;
	}

	private static void Highlight(GameObject go, bool highlighted)
	{
		if (!((Object)(object)go != (Object)null))
		{
			return;
		}
		int num = mHighlighted.Count;
		while (num > 0)
		{
			Highlighted highlighted2 = mHighlighted[--num];
			if (highlighted2 == null || (Object)(object)highlighted2.go == (Object)null)
			{
				mHighlighted.RemoveAt(num);
			}
			else if ((Object)(object)highlighted2.go == (Object)(object)go)
			{
				if (highlighted)
				{
					highlighted2.counter++;
				}
				else if (--highlighted2.counter < 1)
				{
					mHighlighted.Remove(highlighted2);
					Notify(go, "OnHover", false);
				}
				return;
			}
		}
		if (highlighted)
		{
			Highlighted highlighted3 = new Highlighted();
			highlighted3.go = go;
			highlighted3.counter = 1;
			mHighlighted.Add(highlighted3);
			Notify(go, "OnHover", true);
		}
	}

	public static void Notify(GameObject go, string funcName, object obj)
	{
		if ((Object)(object)go != (Object)null)
		{
			go.SendMessage(funcName, obj, (SendMessageOptions)1);
			if ((Object)(object)genericEventHandler != (Object)null && (Object)(object)genericEventHandler != (Object)(object)go)
			{
				genericEventHandler.SendMessage(funcName, obj, (SendMessageOptions)1);
			}
		}
	}

	public static MouseOrTouch GetTouch(int id)
	{
		MouseOrTouch value = null;
		if (!mTouches.TryGetValue(id, out value))
		{
			value = new MouseOrTouch();
			value.touchBegan = true;
			mTouches.Add(id, value);
		}
		return value;
	}

	public static void RemoveTouch(int id)
	{
		mTouches.Remove(id);
	}

	private void Awake()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Invalid comparison between Unknown and I4
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Invalid comparison between Unknown and I4
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Invalid comparison between Unknown and I4
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Invalid comparison between Unknown and I4
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Invalid comparison between Unknown and I4
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Invalid comparison between Unknown and I4
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		mList.Add(this);
		cachedCamera.eventMask = 0;
		if ((int)Application.platform == 11 || (int)Application.platform == 8)
		{
			useMouse = false;
			useTouch = true;
			if ((int)Application.platform == 8)
			{
				useKeyboard = false;
				useController = false;
			}
		}
		else if ((int)Application.platform == 9 || (int)Application.platform == 10)
		{
			useMouse = false;
			useTouch = false;
			useKeyboard = false;
			useController = true;
		}
		else if ((int)Application.platform == 7 || (int)Application.platform == 0)
		{
			mIsEditor = true;
		}
		mMouse[0].pos.x = Input.mousePosition.x;
		mMouse[0].pos.y = Input.mousePosition.y;
		lastTouchPosition = mMouse[0].pos;
		if (LayerMask.op_Implicit(eventReceiverMask) == -1)
		{
			eventReceiverMask = LayerMask.op_Implicit(cachedCamera.cullingMask);
		}
	}

	private void OnDestroy()
	{
		mList.Remove(this);
	}

	private void OnEnable()
	{
		mList.Sort(CompareFunc);
	}

	private void FixedUpdate()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (useMouse && Application.isPlaying && handlesEvents)
		{
			hoveredObject = ((!Raycast(Input.mousePosition, ref lastHit)) ? fallThrough : ((Component)lastHit.collider).gameObject);
			if ((Object)(object)hoveredObject == (Object)null)
			{
				hoveredObject = genericEventHandler;
			}
			for (int i = 0; i < 3; i++)
			{
				mMouse[i].current = hoveredObject;
			}
		}
	}

	private void Update()
	{
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		if (!Application.isPlaying || !handlesEvents)
		{
			return;
		}
		current = this;
		if (useMouse || (useTouch && mIsEditor))
		{
			ProcessMouse();
		}
		if (useTouch)
		{
			ProcessTouches();
		}
		if (onCustomInput != null)
		{
			onCustomInput();
		}
		if (useMouse && (Object)(object)mSel != (Object)null && (((int)cancelKey0 != 0 && Input.GetKeyDown(cancelKey0)) || ((int)cancelKey1 != 0 && Input.GetKeyDown(cancelKey1))))
		{
			selectedObject = null;
		}
		if ((Object)(object)mSel != (Object)null)
		{
			string text = Input.inputString;
			if (useKeyboard && Input.GetKeyDown((KeyCode)127))
			{
				text += "\b";
			}
			if (text.Length > 0)
			{
				if (!stickyTooltip && (Object)(object)mTooltip != (Object)null)
				{
					ShowTooltip(val: false);
				}
				Notify(mSel, "OnInput", text);
			}
		}
		else
		{
			inputHasFocus = false;
		}
		if ((Object)(object)mSel != (Object)null)
		{
			ProcessOthers();
		}
		if (useMouse && (Object)(object)mHover != (Object)null)
		{
			float axis = Input.GetAxis(scrollAxisName);
			if (axis != 0f)
			{
				Notify(mHover, "OnScroll", axis);
			}
			if (showTooltips && mTooltipTime != 0f && (mTooltipTime < Time.realtimeSinceStartup || Input.GetKey((KeyCode)304) || Input.GetKey((KeyCode)303)))
			{
				mTooltip = mHover;
				ShowTooltip(val: true);
			}
		}
		current = null;
	}

	public void ProcessMouse()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		bool flag = useMouse && Time.timeScale < 0.9f;
		if (!flag)
		{
			for (int i = 0; i < 3; i++)
			{
				if (Input.GetMouseButton(i) || Input.GetMouseButtonUp(i))
				{
					flag = true;
					break;
				}
			}
		}
		mMouse[0].pos = Vector2.op_Implicit(Input.mousePosition);
		mMouse[0].delta = mMouse[0].pos - lastTouchPosition;
		bool flag2 = mMouse[0].pos != lastTouchPosition;
		lastTouchPosition = mMouse[0].pos;
		if (flag)
		{
			hoveredObject = ((!Raycast(Input.mousePosition, ref lastHit)) ? fallThrough : ((Component)lastHit.collider).gameObject);
			if ((Object)(object)hoveredObject == (Object)null)
			{
				hoveredObject = genericEventHandler;
			}
			mMouse[0].current = hoveredObject;
		}
		for (int j = 1; j < 3; j++)
		{
			mMouse[j].pos = mMouse[0].pos;
			mMouse[j].delta = mMouse[0].delta;
			mMouse[j].current = mMouse[0].current;
		}
		bool flag3 = false;
		for (int k = 0; k < 3; k++)
		{
			if (Input.GetMouseButton(k))
			{
				flag3 = true;
				break;
			}
		}
		if (flag3)
		{
			mTooltipTime = 0f;
		}
		else if (useMouse && flag2 && (!stickyTooltip || (Object)(object)mHover != (Object)(object)mMouse[0].current))
		{
			if (mTooltipTime != 0f)
			{
				mTooltipTime = Time.realtimeSinceStartup + tooltipDelay;
			}
			else if ((Object)(object)mTooltip != (Object)null)
			{
				ShowTooltip(val: false);
			}
		}
		if (useMouse && !flag3 && (Object)(object)mHover != (Object)null && (Object)(object)mHover != (Object)(object)mMouse[0].current)
		{
			if ((Object)(object)mTooltip != (Object)null)
			{
				ShowTooltip(val: false);
			}
			Highlight(mHover, highlighted: false);
			mHover = null;
		}
		if (useMouse)
		{
			for (int l = 0; l < 3; l++)
			{
				bool mouseButtonDown = Input.GetMouseButtonDown(l);
				bool mouseButtonUp = Input.GetMouseButtonUp(l);
				currentTouch = mMouse[l];
				currentTouchID = -1 - l;
				if (mouseButtonDown)
				{
					currentTouch.pressedCam = currentCamera;
				}
				else if ((Object)(object)currentTouch.pressed != (Object)null)
				{
					currentCamera = currentTouch.pressedCam;
				}
				ProcessTouch(mouseButtonDown, mouseButtonUp);
			}
			currentTouch = null;
		}
		if (useMouse && !flag3 && (Object)(object)mHover != (Object)(object)mMouse[0].current)
		{
			mTooltipTime = Time.realtimeSinceStartup + tooltipDelay;
			mHover = mMouse[0].current;
			Highlight(mHover, highlighted: true);
		}
	}

	public void ProcessTouches()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Invalid comparison between Unknown and I4
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Invalid comparison between Unknown and I4
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < Input.touchCount; i++)
		{
			Touch touch = Input.GetTouch(i);
			currentTouchID = ((!allowMultiTouch) ? 1 : touch.fingerId);
			currentTouch = GetTouch(currentTouchID);
			bool flag = (int)touch.phase == 0 || currentTouch.touchBegan;
			bool flag2 = (int)touch.phase == 4 || (int)touch.phase == 3;
			currentTouch.touchBegan = false;
			if (flag)
			{
				currentTouch.delta = Vector2.zero;
			}
			else
			{
				currentTouch.delta = touch.position - currentTouch.pos;
			}
			currentTouch.pos = touch.position;
			hoveredObject = ((!Raycast(Vector2.op_Implicit(currentTouch.pos), ref lastHit)) ? fallThrough : ((Component)lastHit.collider).gameObject);
			if ((Object)(object)hoveredObject == (Object)null)
			{
				hoveredObject = genericEventHandler;
			}
			currentTouch.current = hoveredObject;
			lastTouchPosition = currentTouch.pos;
			if (flag)
			{
				currentTouch.pressedCam = currentCamera;
			}
			else if ((Object)(object)currentTouch.pressed != (Object)null)
			{
				currentCamera = currentTouch.pressedCam;
			}
			if (touch.tapCount > 1)
			{
				currentTouch.clickTime = Time.realtimeSinceStartup;
			}
			ProcessTouch(flag, flag2);
			if (flag2)
			{
				RemoveTouch(currentTouchID);
			}
			currentTouch = null;
			if (!allowMultiTouch)
			{
				break;
			}
		}
	}

	public void ProcessOthers()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		currentTouchID = -100;
		currentTouch = mController;
		inputHasFocus = (Object)(object)mSel != (Object)null && (Object)(object)mSel.GetComponent<UIInput>() != (Object)null;
		bool flag = ((int)submitKey0 != 0 && Input.GetKeyDown(submitKey0)) || ((int)submitKey1 != 0 && Input.GetKeyDown(submitKey1));
		bool flag2 = ((int)submitKey0 != 0 && Input.GetKeyUp(submitKey0)) || ((int)submitKey1 != 0 && Input.GetKeyUp(submitKey1));
		if (flag || flag2)
		{
			currentTouch.current = mSel;
			ProcessTouch(flag, flag2);
			currentTouch.current = null;
		}
		int num = 0;
		int num2 = 0;
		if (useKeyboard)
		{
			if (inputHasFocus)
			{
				num += GetDirection((KeyCode)273, (KeyCode)274);
				num2 += GetDirection((KeyCode)275, (KeyCode)276);
			}
			else
			{
				num += GetDirection((KeyCode)119, (KeyCode)273, (KeyCode)115, (KeyCode)274);
				num2 += GetDirection((KeyCode)100, (KeyCode)275, (KeyCode)97, (KeyCode)276);
			}
		}
		if (useController)
		{
			if (!string.IsNullOrEmpty(verticalAxisName))
			{
				num += GetDirection(verticalAxisName);
			}
			if (!string.IsNullOrEmpty(horizontalAxisName))
			{
				num2 += GetDirection(horizontalAxisName);
			}
		}
		if (num != 0)
		{
			Notify(mSel, "OnKey", (object)(KeyCode)((num <= 0) ? 274 : 273));
		}
		if (num2 != 0)
		{
			Notify(mSel, "OnKey", (object)(KeyCode)((num2 <= 0) ? 276 : 275));
		}
		if (useKeyboard && Input.GetKeyDown((KeyCode)9))
		{
			Notify(mSel, "OnKey", (object)(KeyCode)9);
		}
		if ((int)cancelKey0 != 0 && Input.GetKeyDown(cancelKey0))
		{
			Notify(mSel, "OnKey", (object)(KeyCode)27);
		}
		if ((int)cancelKey1 != 0 && Input.GetKeyDown(cancelKey1))
		{
			Notify(mSel, "OnKey", (object)(KeyCode)27);
		}
		currentTouch = null;
	}

	public void ProcessTouch(bool pressed, bool unpressed)
	{
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		bool flag = currentTouch == mMouse[0] || currentTouch == mMouse[1] || currentTouch == mMouse[2];
		float num = ((!flag) ? touchDragThreshold : mouseDragThreshold);
		float num2 = ((!flag) ? touchClickThreshold : mouseClickThreshold);
		if (pressed)
		{
			if ((Object)(object)mTooltip != (Object)null)
			{
				ShowTooltip(val: false);
			}
			currentTouch.pressStarted = true;
			Notify(currentTouch.pressed, "OnPress", false);
			currentTouch.pressed = currentTouch.current;
			currentTouch.dragged = currentTouch.current;
			currentTouch.clickNotification = ((!flag) ? ClickNotification.Always : ClickNotification.BasedOnDelta);
			currentTouch.totalDelta = Vector2.zero;
			currentTouch.dragStarted = false;
			Notify(currentTouch.pressed, "OnPress", true);
			if ((Object)(object)currentTouch.pressed != (Object)(object)mSel)
			{
				if ((Object)(object)mTooltip != (Object)null)
				{
					ShowTooltip(val: false);
				}
				selectedObject = null;
			}
		}
		else
		{
			if (!stickyPress && !unpressed && currentTouch.pressStarted && (Object)(object)currentTouch.pressed != (Object)(object)hoveredObject)
			{
				isDragging = true;
				Notify(currentTouch.pressed, "OnPress", false);
				currentTouch.pressed = hoveredObject;
				Notify(currentTouch.pressed, "OnPress", true);
				isDragging = false;
			}
			if ((Object)(object)currentTouch.pressed != (Object)null)
			{
				float magnitude = currentTouch.delta.magnitude;
				if (magnitude != 0f)
				{
					MouseOrTouch mouseOrTouch = currentTouch;
					mouseOrTouch.totalDelta += currentTouch.delta;
					magnitude = currentTouch.totalDelta.magnitude;
					if (!currentTouch.dragStarted && num < magnitude)
					{
						currentTouch.dragStarted = true;
						currentTouch.delta = currentTouch.totalDelta;
					}
					if (currentTouch.dragStarted)
					{
						if ((Object)(object)mTooltip != (Object)null)
						{
							ShowTooltip(val: false);
						}
						isDragging = true;
						bool flag2 = currentTouch.clickNotification == ClickNotification.None;
						Notify(currentTouch.dragged, "OnDrag", currentTouch.delta);
						isDragging = false;
						if (flag2)
						{
							currentTouch.clickNotification = ClickNotification.None;
						}
						else if (currentTouch.clickNotification == ClickNotification.BasedOnDelta && num2 < magnitude)
						{
							currentTouch.clickNotification = ClickNotification.None;
						}
					}
				}
			}
		}
		if (!unpressed)
		{
			return;
		}
		currentTouch.pressStarted = false;
		if ((Object)(object)mTooltip != (Object)null)
		{
			ShowTooltip(val: false);
		}
		if ((Object)(object)currentTouch.pressed != (Object)null)
		{
			Notify(currentTouch.pressed, "OnPress", false);
			if (useMouse && (Object)(object)currentTouch.pressed == (Object)(object)mHover)
			{
				Notify(currentTouch.pressed, "OnHover", true);
			}
			if ((Object)(object)currentTouch.dragged == (Object)(object)currentTouch.current || (currentTouch.clickNotification != ClickNotification.None && currentTouch.totalDelta.magnitude < num))
			{
				if ((Object)(object)currentTouch.pressed != (Object)(object)mSel)
				{
					mSel = currentTouch.pressed;
					Notify(currentTouch.pressed, "OnSelect", true);
				}
				else
				{
					mSel = currentTouch.pressed;
				}
				if (currentTouch.clickNotification != ClickNotification.None)
				{
					float realtimeSinceStartup = Time.realtimeSinceStartup;
					Notify(currentTouch.pressed, "OnClick", null);
					if (currentTouch.clickTime + 0.35f > realtimeSinceStartup)
					{
						Notify(currentTouch.pressed, "OnDoubleClick", null);
					}
					currentTouch.clickTime = realtimeSinceStartup;
				}
			}
			else
			{
				Notify(currentTouch.current, "OnDrop", currentTouch.dragged);
			}
		}
		currentTouch.dragStarted = false;
		currentTouch.pressed = null;
		currentTouch.dragged = null;
	}

	public void ShowTooltip(bool val)
	{
		mTooltipTime = 0f;
		Notify(mTooltip, "OnTooltip", val);
		if (!val)
		{
			mTooltip = null;
		}
	}
}
