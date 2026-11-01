using UnityEngine;

public class CubeDamage : MonoBehaviour
{
	public Texture2D[] frames;

	public int cur_frame;

	private void Start()
	{
		Reset();
	}

	public void Reset()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		cur_frame = 0;
		((Component)this).renderer.material.shader = Shader.Find("Transparent/Diffuse");
		((Component)this).renderer.material.color = Color.white;
		((Component)this).renderer.material.mainTexture = (Texture)(object)frames[cur_frame];
	}

	public void SetFrame(int frame)
	{
		if (frame > frames.Length - 1)
		{
			frame = frames.Length - 1;
		}
		if (frame < 0)
		{
			frame = 0;
		}
		cur_frame = frame;
		((Component)this).renderer.material.mainTexture = (Texture)(object)frames[cur_frame];
	}
}
