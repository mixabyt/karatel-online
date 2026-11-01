using UnityEngine;

public class Torch : EntityBase
{
	protected override void Creation(object[] data)
	{
		SetLight();
	}

	private void SetLight()
	{
		World.Instance.Lighting.AddLight(BlockX, BlockY, BlockZ, recalculateLighting: true);
	}

	[RPC]
	private void RemoveLight()
	{
		World.Instance.Lighting.RemoveLight(BlockX, BlockY, BlockZ, recalculateLighting: true);
	}

	public override void SelfDelete()
	{
		photonView.RPC("RemoveLight", PhotonTargets.All);
		base.SelfDelete();
	}
}
