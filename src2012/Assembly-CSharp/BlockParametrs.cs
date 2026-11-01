using System;
using UnityEngine;

[Serializable]
public class BlockParametrs
{
	public BlockType BType;

	public string name;

	public string about;

	public EntityType dropEntity;

	public Texture2D icon;

	public int life_index;

	public bool is_glass;

	public ResourceLoader.eSounds FootEffect;

	public ResourceLoader.eSounds HitEffect;

	public ResourceLoader.eSounds DestroyEffect;
}
