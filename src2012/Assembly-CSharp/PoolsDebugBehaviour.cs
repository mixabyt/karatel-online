using UnityEngine;

public class PoolsDebugBehaviour : MonoBehaviour
{
	private string text = string.Empty;

	private float nextUpdateTime;

	public void OnGUI()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time > nextUpdateTime)
		{
			nextUpdateTime = Time.time + 1f;
			UpdateText();
		}
		GUI.Label(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), text);
	}

	private void UpdateText()
	{
		text = $"PChunkList pool size: {PChunkList.pool.AcquiredCount} {PChunkList.pool.ReleasedCount} {PChunkList.pool.PoolCount}\n{PMeshData.GetDebugInfo()}";
	}
}
