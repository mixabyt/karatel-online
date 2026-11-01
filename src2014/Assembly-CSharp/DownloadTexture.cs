using System.Collections;
using UnityEngine;

[RequireComponent(typeof(UITexture))]
public class DownloadTexture : MonoBehaviour
{
	public string url = "http://www.tasharen.com/misc/logo.png";

	private Material mMat;

	private Texture2D mTex;

	private IEnumerator Start()
	{
		WWW www = new WWW(url);
		yield return www;
		mTex = www.texture;
		if ((Object)(object)mTex != (Object)null)
		{
			UITexture ut = ((Component)this).GetComponent<UITexture>();
			if ((Object)(object)ut.material == (Object)null)
			{
				mMat = new Material(Shader.Find("Unlit/Transparent Colored"));
			}
			else
			{
				mMat = new Material(ut.material);
			}
			ut.material = mMat;
			mMat.mainTexture = (Texture)(object)mTex;
			ut.MakePixelPerfect();
		}
		www.Dispose();
	}

	private void OnDestroy()
	{
		if ((Object)(object)mMat != (Object)null)
		{
			Object.Destroy((Object)(object)mMat);
		}
		if ((Object)(object)mTex != (Object)null)
		{
			Object.Destroy((Object)(object)mTex);
		}
	}
}
