using System;
using Boo.Lang.Runtime;
using UnityEngine;

[Serializable]
public class GUITestScript : MonoBehaviour
{
	public bool doWindow0;

	public bool doWindow1;

	public bool doWindow2;

	public bool doWindow3;

	public bool doWindow4;

	private object leafOffset;

	private object frameOffset;

	private object skullOffset;

	private object RibbonOffsetX;

	private object FrameOffsetX;

	private object SkullOffsetX;

	private object RibbonOffsetY;

	private object FrameOffsetY;

	private object SkullOffsetY;

	private object WSwaxOffsetX;

	private object WSwaxOffsetY;

	private object WSribbonOffsetX;

	private object WSribbonOffsetY;

	private object spikeCount;

	public GUISkin mySkin;

	private Rect windowRect0;

	private Rect windowRect1;

	private Rect windowRect2;

	private Rect windowRect3;

	private Vector2 scrollPosition;

	private float HroizSliderValue;

	private float VertSliderValue;

	private bool ToggleBTN;

	private string NecroText;

	public GUITestScript()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		doWindow0 = true;
		doWindow1 = true;
		doWindow2 = true;
		doWindow3 = true;
		doWindow4 = true;
		windowRect0 = new Rect(500f, 140f, 350f, 510f);
		windowRect1 = new Rect(380f, 40f, 262f, 420f);
		windowRect2 = new Rect(700f, 40f, 306f, 480f);
		windowRect3 = new Rect(0f, 40f, 350f, 500f);
		HroizSliderValue = 0.5f;
		VertSliderValue = 0.5f;
		NecroText = "This started as a question... How flexible is the built in GUI in unity? The answer... pretty damn flexible! At first I wasn\ufffdt so sure; it seemed no one ever used it to make a non OS style GUI at least not a publicly available one. So I decided I couldn\ufffdt be sure until I tried to develop a full GUI, Long story short Necromancer was the result and is now available to the general public, free for comercial and non-comercial use. I only ask that if you add something Share it.   Credits to Kevin King for the fonts.";
	}

	public override void AddSpikes(object winX)
	{
		spikeCount = Mathf.Floor(RuntimeServices.UnboxSingle(RuntimeServices.InvokeBinaryOperator("op_Subtraction", winX, 152))) / 22f;
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label(string.Empty, GUIStyle.op_Implicit("SpikeLeft"), new GUILayoutOption[0]);
		for (int i = 0; RuntimeServices.ToBool(RuntimeServices.InvokeBinaryOperator("op_LessThan", i, spikeCount)); i++)
		{
			GUILayout.Label(string.Empty, GUIStyle.op_Implicit("SpikeMid"), new GUILayoutOption[0]);
		}
		GUILayout.Label(string.Empty, GUIStyle.op_Implicit("SpikeRight"), new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
	}

	public override void FancyTop(object topX)
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		leafOffset = RuntimeServices.InvokeBinaryOperator("op_Subtraction", RuntimeServices.InvokeBinaryOperator("op_Division", topX, 2), 64);
		frameOffset = RuntimeServices.InvokeBinaryOperator("op_Subtraction", RuntimeServices.InvokeBinaryOperator("op_Division", topX, 2), 27);
		skullOffset = RuntimeServices.InvokeBinaryOperator("op_Subtraction", RuntimeServices.InvokeBinaryOperator("op_Division", topX, 2), 20);
		GUI.Label(new Rect(RuntimeServices.UnboxSingle(leafOffset), 18f, 0f, 0f), string.Empty, GUIStyle.op_Implicit("GoldLeaf"));
		GUI.Label(new Rect(RuntimeServices.UnboxSingle(frameOffset), 3f, 0f, 0f), string.Empty, GUIStyle.op_Implicit("IconFrame"));
		GUI.Label(new Rect(RuntimeServices.UnboxSingle(skullOffset), 12f, 0f, 0f), string.Empty, GUIStyle.op_Implicit("Skull"));
	}

	public override void WaxSeal(object x, object y)
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		WSwaxOffsetX = RuntimeServices.InvokeBinaryOperator("op_Subtraction", x, 120);
		WSwaxOffsetY = RuntimeServices.InvokeBinaryOperator("op_Subtraction", y, 115);
		WSribbonOffsetX = RuntimeServices.InvokeBinaryOperator("op_Subtraction", x, 114);
		WSribbonOffsetY = RuntimeServices.InvokeBinaryOperator("op_Subtraction", y, 83);
		GUI.Label(new Rect(RuntimeServices.UnboxSingle(WSribbonOffsetX), RuntimeServices.UnboxSingle(WSribbonOffsetY), 0f, 0f), string.Empty, GUIStyle.op_Implicit("RibbonBlue"));
		GUI.Label(new Rect(RuntimeServices.UnboxSingle(WSwaxOffsetX), RuntimeServices.UnboxSingle(WSwaxOffsetY), 0f, 0f), string.Empty, GUIStyle.op_Implicit("WaxSeal"));
	}

	public override void DeathBadge(object x, object y)
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		RibbonOffsetX = x;
		FrameOffsetX = RuntimeServices.InvokeBinaryOperator("op_Addition", x, 3);
		SkullOffsetX = RuntimeServices.InvokeBinaryOperator("op_Addition", x, 10);
		RibbonOffsetY = RuntimeServices.InvokeBinaryOperator("op_Addition", y, 22);
		FrameOffsetY = y;
		SkullOffsetY = RuntimeServices.InvokeBinaryOperator("op_Addition", y, 9);
		GUI.Label(new Rect(RuntimeServices.UnboxSingle(RibbonOffsetX), RuntimeServices.UnboxSingle(RibbonOffsetY), 0f, 0f), string.Empty, GUIStyle.op_Implicit("RibbonRed"));
		GUI.Label(new Rect(RuntimeServices.UnboxSingle(FrameOffsetX), RuntimeServices.UnboxSingle(FrameOffsetY), 0f, 0f), string.Empty, GUIStyle.op_Implicit("IconFrame"));
		GUI.Label(new Rect(RuntimeServices.UnboxSingle(SkullOffsetX), RuntimeServices.UnboxSingle(SkullOffsetY), 0f, 0f), string.Empty, GUIStyle.op_Implicit("Skull"));
	}

	public override void DoMyWindow0(int windowID)
	{
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		AddSpikes(windowRect0.width);
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.Space(8f);
		GUILayout.Label(string.Empty, GUIStyle.op_Implicit("Divider"), new GUILayoutOption[0]);
		GUILayout.Label("Standard Label", new GUILayoutOption[0]);
		GUILayout.Label("Short Label", GUIStyle.op_Implicit("ShortLabel"), new GUILayoutOption[0]);
		GUILayout.Label(string.Empty, GUIStyle.op_Implicit("Divider"), new GUILayoutOption[0]);
		GUILayout.Button("Standard Button", new GUILayoutOption[0]);
		GUILayout.Button("Short Button", GUIStyle.op_Implicit("ShortButton"), new GUILayoutOption[0]);
		GUILayout.Label(string.Empty, GUIStyle.op_Implicit("Divider"), new GUILayoutOption[0]);
		ToggleBTN = GUILayout.Toggle(ToggleBTN, "This is a Toggle Button", new GUILayoutOption[0]);
		GUILayout.Label(string.Empty, GUIStyle.op_Implicit("Divider"), new GUILayoutOption[0]);
		GUILayout.Box("This is a textbox\n this can be expanded by using \\n", new GUILayoutOption[0]);
		GUILayout.TextField("This is a textfield\n You cant see this text!!", new GUILayoutOption[0]);
		GUILayout.TextArea("This is a textArea\n this can be expanded by using \\n", new GUILayoutOption[0]);
		GUILayout.EndVertical();
		GUI.DragWindow(new Rect(0f, 0f, 10000f, 10000f));
	}

	public override void DoMyWindow1(int windowID)
	{
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		AddSpikes(windowRect1.width);
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.Label(string.Empty, GUIStyle.op_Implicit("Divider"), new GUILayoutOption[0]);
		GUILayout.Label("Plain Text", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		GUILayout.Label("Italic Text", GUIStyle.op_Implicit("ItalicText"), new GUILayoutOption[0]);
		GUILayout.Label("Light Text", GUIStyle.op_Implicit("LightText"), new GUILayoutOption[0]);
		GUILayout.Label("Bold Text", GUIStyle.op_Implicit("BoldText"), new GUILayoutOption[0]);
		GUILayout.Label("Disabled Text", GUIStyle.op_Implicit("DisabledText"), new GUILayoutOption[0]);
		GUILayout.Label("Cursed Text", GUIStyle.op_Implicit("CursedText"), new GUILayoutOption[0]);
		GUILayout.Label("Legendary Text", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[0]);
		GUILayout.Label("Outlined Text", GUIStyle.op_Implicit("OutlineText"), new GUILayoutOption[0]);
		GUILayout.Label("Italic Outline Text", GUIStyle.op_Implicit("ItalicOutlineText"), new GUILayoutOption[0]);
		GUILayout.Label("Light Outline Text", GUIStyle.op_Implicit("LightOutlineText"), new GUILayoutOption[0]);
		GUILayout.Label("Bold Outline Text", GUIStyle.op_Implicit("BoldOutlineText"), new GUILayoutOption[0]);
		GUILayout.EndVertical();
		GUI.DragWindow(new Rect(0f, 0f, 10000f, 10000f));
	}

	public override void DoMyWindow2(int windowID)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		AddSpikes(windowRect2.width);
		GUILayout.Space(8f);
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, true, true, new GUILayoutOption[0]);
		GUILayout.Label(NecroText, GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		GUILayout.EndScrollView();
		GUILayout.EndHorizontal();
		GUILayout.Space(8f);
		HroizSliderValue = GUILayout.HorizontalSlider(HroizSliderValue, 0f, 1.1f, new GUILayoutOption[0]);
		VertSliderValue = GUILayout.VerticalSlider(VertSliderValue, 0f, 1.1f, new GUILayoutOption[1] { GUILayout.Height(70f) });
		DeathBadge(200, 350);
		GUILayout.EndVertical();
		GUI.DragWindow(new Rect(0f, 0f, 10000f, 10000f));
	}

	public override void DoMyWindow3(int windowID)
	{
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		AddSpikes(windowRect3.width);
		FancyTop(windowRect0.width);
		GUILayout.Space(8f);
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.Label("Necromancer", new GUILayoutOption[0]);
		GUILayout.Label(string.Empty, GUIStyle.op_Implicit("Divider"), new GUILayoutOption[0]);
		GUILayout.Label("Necromancer is a free to use GUI for the unity community. this skin can be used in commercial and non-commercial products.", GUIStyle.op_Implicit("LightText"), new GUILayoutOption[0]);
		GUILayout.Label(string.Empty, GUIStyle.op_Implicit("Divider"), new GUILayoutOption[0]);
		GUILayout.Space(8f);
		doWindow0 = GUILayout.Toggle(doWindow0, "Standard Components", new GUILayoutOption[0]);
		doWindow1 = GUILayout.Toggle(doWindow1, "Text Examples", new GUILayoutOption[0]);
		doWindow2 = GUILayout.Toggle(doWindow2, "Sliders", new GUILayoutOption[0]);
		GUILayout.Space(8f);
		GUILayout.Label(string.Empty, GUIStyle.op_Implicit("Divider"), new GUILayoutOption[0]);
		GUILayout.Label("Please read through the source of this script to see", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("how to use special ", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		GUILayout.Label("Components ", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[0]);
		GUILayout.Label("and ", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		GUILayout.Label("Functions ", GUIStyle.op_Implicit("CursedText"), new GUILayoutOption[0]);
		GUILayout.Label("!", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		GUILayout.Label(string.Empty, GUIStyle.op_Implicit("Divider"), new GUILayoutOption[0]);
		GUILayout.Space(26f);
		GUILayout.Label("Created By Jason Wentzel 2011", GUIStyle.op_Implicit("SingleQuotes"), new GUILayoutOption[0]);
		GUILayout.EndVertical();
		WaxSeal(windowRect3.width, windowRect3.height);
		GUI.DragWindow(new Rect(0f, 0f, 10000f, 10000f));
	}

	public override void OnGUI()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected Obj, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Expected Obj, but got Unknown
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Expected Obj, but got Unknown
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Expected Obj, but got Unknown
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		GUI.skin = mySkin;
		if (doWindow0)
		{
			windowRect0 = GUI.Window(0, windowRect0, (WindowFunction)DoMyWindow0, string.Empty);
		}
		GUI.BeginGroup(new Rect(0f, 0f, 100f, 100f));
		GUI.EndGroup();
		if (doWindow1)
		{
			windowRect1 = GUI.Window(1, windowRect1, (WindowFunction)DoMyWindow1, string.Empty);
		}
		GUI.BeginGroup(new Rect(0f, 0f, 100f, 100f));
		GUI.EndGroup();
		if (doWindow2)
		{
			windowRect2 = GUI.Window(2, windowRect2, (WindowFunction)DoMyWindow2, string.Empty);
		}
		GUI.BeginGroup(new Rect(0f, 0f, 100f, 100f));
		GUI.EndGroup();
		if (doWindow3)
		{
			windowRect3 = GUI.Window(3, windowRect3, (WindowFunction)DoMyWindow3, string.Empty);
		}
		GUI.BeginGroup(new Rect(0f, 0f, 100f, 100f));
		GUI.EndGroup();
	}

	public override void Main()
	{
	}
}
