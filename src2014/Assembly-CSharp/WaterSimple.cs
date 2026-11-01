using UnityEngine;

[ExecuteInEditMode]
public class WaterSimple : MonoBehaviour
{
	private int uvAnimationTileX = 1;

	private int uvAnimationTileY = 51;

	private float framesPerSecond = 10f;

	private void Update()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		int num = (int)(Time.time * framesPerSecond);
		num %= uvAnimationTileX * uvAnimationTileY;
		Vector2 val = new Vector2(1f / (float)uvAnimationTileX, 1f / (float)uvAnimationTileY);
		int num2 = num % uvAnimationTileX;
		int num3 = num / uvAnimationTileX;
		Vector2 val2 = new Vector2((float)num2 * val.x, 1f - val.y - (float)num3 * val.y);
		((Component)this).renderer.material.SetTextureOffset("_MainTex", val2);
		((Component)this).renderer.material.SetTextureScale("_MainTex", val);
	}
}
