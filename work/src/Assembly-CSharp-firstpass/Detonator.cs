using UnityEngine;

[AddComponentMenu("Detonator/Detonator")]
public class Detonator : MonoBehaviour
{
	private static float _baseSize = 30f;

	private static Color _baseColor = new Color(1f, 0.423f, 0f, 0.5f);

	private static float _baseDuration = 3f;

	public float size = 10f;

	public Color color = _baseColor;

	public bool explodeOnStart = true;

	public float duration = _baseDuration;

	public float detail = 1f;

	public float upwardsBias;

	public float destroyTime = 7f;

	public bool useWorldSpace = true;

	public Vector3 direction = Vector3.zero;

	public Material fireballAMaterial;

	public Material fireballBMaterial;

	public Material smokeAMaterial;

	public Material smokeBMaterial;

	public Material shockwaveMaterial;

	public Material sparksMaterial;

	public Material glowMaterial;

	public Material heatwaveMaterial;

	private Component[] components;

	private DetonatorFireball _fireball;

	private DetonatorSparks _sparks;

	private DetonatorShockwave _shockwave;

	private DetonatorSmoke _smoke;

	private DetonatorGlow _glow;

	private DetonatorLight _light;

	private DetonatorForce _force;

	private DetonatorHeatwave _heatwave;

	public bool autoCreateFireball = true;

	public bool autoCreateSparks = true;

	public bool autoCreateShockwave = true;

	public bool autoCreateSmoke = true;

	public bool autoCreateGlow = true;

	public bool autoCreateLight = true;

	public bool autoCreateForce = true;

	public bool autoCreateHeatwave;

	private float _lastExplosionTime = 1000f;

	private bool _firstComponentUpdate = true;

	private Component[] _subDetonators;

	public static Material defaultFireballAMaterial;

	public static Material defaultFireballBMaterial;

	public static Material defaultSmokeAMaterial;

	public static Material defaultSmokeBMaterial;

	public static Material defaultShockwaveMaterial;

	public static Material defaultSparksMaterial;

	public static Material defaultGlowMaterial;

	public static Material defaultHeatwaveMaterial;

	public Detonator()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
	}

	static Detonator()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Awake()
	{
		FillDefaultMaterials();
		components = ((Component)this).GetComponents(typeof(DetonatorComponent));
		Component[] array = components;
		for (int i = 0; i < array.Length; i++)
		{
			DetonatorComponent detonatorComponent = (DetonatorComponent)(object)array[i];
			if (detonatorComponent is DetonatorFireball)
			{
				_fireball = detonatorComponent as DetonatorFireball;
			}
			if (detonatorComponent is DetonatorSparks)
			{
				_sparks = detonatorComponent as DetonatorSparks;
			}
			if (detonatorComponent is DetonatorShockwave)
			{
				_shockwave = detonatorComponent as DetonatorShockwave;
			}
			if (detonatorComponent is DetonatorSmoke)
			{
				_smoke = detonatorComponent as DetonatorSmoke;
			}
			if (detonatorComponent is DetonatorGlow)
			{
				_glow = detonatorComponent as DetonatorGlow;
			}
			if (detonatorComponent is DetonatorLight)
			{
				_light = detonatorComponent as DetonatorLight;
			}
			if (detonatorComponent is DetonatorForce)
			{
				_force = detonatorComponent as DetonatorForce;
			}
			if (detonatorComponent is DetonatorHeatwave)
			{
				_heatwave = detonatorComponent as DetonatorHeatwave;
			}
		}
		if (!Object.op_Implicit((Object)(object)_fireball) && autoCreateFireball)
		{
			_fireball = ((Component)this).gameObject.AddComponent("DetonatorFireball") as DetonatorFireball;
			_fireball.Reset();
		}
		if (!Object.op_Implicit((Object)(object)_smoke) && autoCreateSmoke)
		{
			_smoke = ((Component)this).gameObject.AddComponent("DetonatorSmoke") as DetonatorSmoke;
			_smoke.Reset();
		}
		if (!Object.op_Implicit((Object)(object)_sparks) && autoCreateSparks)
		{
			_sparks = ((Component)this).gameObject.AddComponent("DetonatorSparks") as DetonatorSparks;
			_sparks.Reset();
		}
		if (!Object.op_Implicit((Object)(object)_shockwave) && autoCreateShockwave)
		{
			_shockwave = ((Component)this).gameObject.AddComponent("DetonatorShockwave") as DetonatorShockwave;
			_shockwave.Reset();
		}
		if (!Object.op_Implicit((Object)(object)_glow) && autoCreateGlow)
		{
			_glow = ((Component)this).gameObject.AddComponent("DetonatorGlow") as DetonatorGlow;
			_glow.Reset();
		}
		if (!Object.op_Implicit((Object)(object)_light) && autoCreateLight)
		{
			_light = ((Component)this).gameObject.AddComponent("DetonatorLight") as DetonatorLight;
			_light.Reset();
		}
		if (!Object.op_Implicit((Object)(object)_force) && autoCreateForce)
		{
			_force = ((Component)this).gameObject.AddComponent("DetonatorForce") as DetonatorForce;
			_force.Reset();
		}
		if (!Object.op_Implicit((Object)(object)_heatwave) && autoCreateHeatwave && SystemInfo.supportsImageEffects)
		{
			_heatwave = ((Component)this).gameObject.AddComponent("DetonatorHeatwave") as DetonatorHeatwave;
			_heatwave.Reset();
		}
		components = ((Component)this).GetComponents(typeof(DetonatorComponent));
	}

	private void FillDefaultMaterials()
	{
		if (!Object.op_Implicit((Object)(object)fireballAMaterial))
		{
			fireballAMaterial = DefaultFireballAMaterial();
		}
		if (!Object.op_Implicit((Object)(object)fireballBMaterial))
		{
			fireballBMaterial = DefaultFireballBMaterial();
		}
		if (!Object.op_Implicit((Object)(object)smokeAMaterial))
		{
			smokeAMaterial = DefaultSmokeAMaterial();
		}
		if (!Object.op_Implicit((Object)(object)smokeBMaterial))
		{
			smokeBMaterial = DefaultSmokeBMaterial();
		}
		if (!Object.op_Implicit((Object)(object)shockwaveMaterial))
		{
			shockwaveMaterial = DefaultShockwaveMaterial();
		}
		if (!Object.op_Implicit((Object)(object)sparksMaterial))
		{
			sparksMaterial = DefaultSparksMaterial();
		}
		if (!Object.op_Implicit((Object)(object)glowMaterial))
		{
			glowMaterial = DefaultGlowMaterial();
		}
		if (!Object.op_Implicit((Object)(object)heatwaveMaterial))
		{
			heatwaveMaterial = DefaultHeatwaveMaterial();
		}
	}

	private void Start()
	{
		if (explodeOnStart)
		{
			UpdateComponents();
			Explode();
		}
	}

	private void Update()
	{
		if (destroyTime > 0f && _lastExplosionTime + destroyTime <= Time.time)
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}

	private void UpdateComponents()
	{
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		if (_firstComponentUpdate)
		{
			Component[] array = components;
			for (int i = 0; i < array.Length; i++)
			{
				DetonatorComponent detonatorComponent = (DetonatorComponent)(object)array[i];
				detonatorComponent.Init();
				detonatorComponent.SetStartValues();
			}
			_firstComponentUpdate = false;
		}
		if (_firstComponentUpdate)
		{
			return;
		}
		Component[] array2 = components;
		for (int j = 0; j < array2.Length; j++)
		{
			DetonatorComponent detonatorComponent2 = (DetonatorComponent)(object)array2[j];
			if (detonatorComponent2.detonatorControlled)
			{
				detonatorComponent2.size = detonatorComponent2.startSize * (size / _baseSize);
				detonatorComponent2.timeScale = duration / _baseDuration;
				detonatorComponent2.detail = detonatorComponent2.startDetail * detail;
				detonatorComponent2.force = detonatorComponent2.startForce * (size / _baseSize) + direction * (size / _baseSize);
				detonatorComponent2.velocity = detonatorComponent2.startVelocity * (size / _baseSize) + direction * (size / _baseSize);
				detonatorComponent2.color = Color.Lerp(detonatorComponent2.startColor, color, color.a);
			}
		}
	}

	public void Explode()
	{
		_lastExplosionTime = Time.time;
		Component[] array = components;
		for (int i = 0; i < array.Length; i++)
		{
			DetonatorComponent detonatorComponent = (DetonatorComponent)(object)array[i];
			UpdateComponents();
			detonatorComponent.Explode();
		}
	}

	public void Reset()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		size = 10f;
		color = _baseColor;
		duration = _baseDuration;
		FillDefaultMaterials();
	}

	public static Material DefaultFireballAMaterial()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected Obj, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)defaultFireballAMaterial != (Object)null)
		{
			return defaultFireballAMaterial;
		}
		defaultFireballAMaterial = new Material(Shader.Find("Particles/Additive"));
		((Object)defaultFireballAMaterial).name = "FireballA-Default";
		Object val = Resources.Load("Detonator/Textures/Fireball");
		Texture2D mainTexture = (Texture2D)(object)((val is Texture2D) ? val : null);
		defaultFireballAMaterial.SetColor("_TintColor", Color.white);
		defaultFireballAMaterial.mainTexture = (Texture)(object)mainTexture;
		defaultFireballAMaterial.mainTextureScale = new Vector2(0.5f, 1f);
		return defaultFireballAMaterial;
	}

	public static Material DefaultFireballBMaterial()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected Obj, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)defaultFireballBMaterial != (Object)null)
		{
			return defaultFireballBMaterial;
		}
		defaultFireballBMaterial = new Material(Shader.Find("Particles/Additive"));
		((Object)defaultFireballBMaterial).name = "FireballB-Default";
		Object val = Resources.Load("Detonator/Textures/Fireball");
		Texture2D mainTexture = (Texture2D)(object)((val is Texture2D) ? val : null);
		defaultFireballBMaterial.SetColor("_TintColor", Color.white);
		defaultFireballBMaterial.mainTexture = (Texture)(object)mainTexture;
		defaultFireballBMaterial.mainTextureScale = new Vector2(0.5f, 1f);
		defaultFireballBMaterial.mainTextureOffset = new Vector2(0.5f, 0f);
		return defaultFireballBMaterial;
	}

	public static Material DefaultSmokeAMaterial()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected Obj, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)defaultSmokeAMaterial != (Object)null)
		{
			return defaultSmokeAMaterial;
		}
		defaultSmokeAMaterial = new Material(Shader.Find("Particles/Alpha Blended"));
		((Object)defaultSmokeAMaterial).name = "SmokeA-Default";
		Object val = Resources.Load("Detonator/Textures/Smoke");
		Texture2D mainTexture = (Texture2D)(object)((val is Texture2D) ? val : null);
		defaultSmokeAMaterial.SetColor("_TintColor", Color.white);
		defaultSmokeAMaterial.mainTexture = (Texture)(object)mainTexture;
		defaultSmokeAMaterial.mainTextureScale = new Vector2(0.5f, 1f);
		return defaultSmokeAMaterial;
	}

	public static Material DefaultSmokeBMaterial()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected Obj, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)defaultSmokeBMaterial != (Object)null)
		{
			return defaultSmokeBMaterial;
		}
		defaultSmokeBMaterial = new Material(Shader.Find("Particles/Alpha Blended"));
		((Object)defaultSmokeBMaterial).name = "SmokeB-Default";
		Object val = Resources.Load("Detonator/Textures/Smoke");
		Texture2D mainTexture = (Texture2D)(object)((val is Texture2D) ? val : null);
		defaultSmokeBMaterial.SetColor("_TintColor", Color.white);
		defaultSmokeBMaterial.mainTexture = (Texture)(object)mainTexture;
		defaultSmokeBMaterial.mainTextureScale = new Vector2(0.5f, 1f);
		defaultSmokeBMaterial.mainTextureOffset = new Vector2(0.5f, 0f);
		return defaultSmokeBMaterial;
	}

	public static Material DefaultSparksMaterial()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected Obj, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)defaultSparksMaterial != (Object)null)
		{
			return defaultSparksMaterial;
		}
		defaultSparksMaterial = new Material(Shader.Find("Particles/Additive"));
		((Object)defaultSparksMaterial).name = "Sparks-Default";
		Object val = Resources.Load("Detonator/Textures/GlowDot");
		Texture2D mainTexture = (Texture2D)(object)((val is Texture2D) ? val : null);
		defaultSparksMaterial.SetColor("_TintColor", Color.white);
		defaultSparksMaterial.mainTexture = (Texture)(object)mainTexture;
		return defaultSparksMaterial;
	}

	public static Material DefaultShockwaveMaterial()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected Obj, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)defaultShockwaveMaterial != (Object)null)
		{
			return defaultShockwaveMaterial;
		}
		defaultShockwaveMaterial = new Material(Shader.Find("Particles/Additive"));
		((Object)defaultShockwaveMaterial).name = "Shockwave-Default";
		Object val = Resources.Load("Detonator/Textures/Shockwave");
		Texture2D mainTexture = (Texture2D)(object)((val is Texture2D) ? val : null);
		defaultShockwaveMaterial.SetColor("_TintColor", new Color(0.1f, 0.1f, 0.1f, 1f));
		defaultShockwaveMaterial.mainTexture = (Texture)(object)mainTexture;
		return defaultShockwaveMaterial;
	}

	public static Material DefaultGlowMaterial()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected Obj, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)defaultGlowMaterial != (Object)null)
		{
			return defaultGlowMaterial;
		}
		defaultGlowMaterial = new Material(Shader.Find("Particles/Additive"));
		((Object)defaultGlowMaterial).name = "Glow-Default";
		Object val = Resources.Load("Detonator/Textures/Glow");
		Texture2D mainTexture = (Texture2D)(object)((val is Texture2D) ? val : null);
		defaultGlowMaterial.SetColor("_TintColor", Color.white);
		defaultGlowMaterial.mainTexture = (Texture)(object)mainTexture;
		return defaultGlowMaterial;
	}

	public static Material DefaultHeatwaveMaterial()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected Obj, but got Unknown
		if (SystemInfo.supportsImageEffects)
		{
			if ((Object)(object)defaultHeatwaveMaterial != (Object)null)
			{
				return defaultHeatwaveMaterial;
			}
			defaultHeatwaveMaterial = new Material(Shader.Find("HeatDistort"));
			((Object)defaultHeatwaveMaterial).name = "Heatwave-Default";
			Object val = Resources.Load("Detonator/Textures/Heatwave");
			Texture2D val2 = (Texture2D)(object)((val is Texture2D) ? val : null);
			defaultHeatwaveMaterial.SetTexture("_BumpMap", (Texture)(object)val2);
			return defaultHeatwaveMaterial;
		}
		return null;
	}
}
