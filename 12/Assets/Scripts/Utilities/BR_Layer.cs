using UnityEngine;

public sealed class BR_Layer
{
	public static readonly BR_Layer Instance = new BR_Layer ();

	//Built in unity layers
	public const int Default = 0;
	public const int TransparentFX = 1;
	public const int IgnoreRaycast = 2;
	public const int Water = 4;

	//Standard Layers
	public const int MovableObject = 21;
	public const int Ragdoll = 22;
	public const int RemotePlayer = 23;
	public const int IgnoreAttacks = 24;
	public const int Enemy = 25;
	public const int Pickup = 26;
	public const int Trigger = 27;
	public const int MovingPlatform = 28;
	public const int Debris = 29;
	public const int LocalPlayer = 30;
	public const int Floor = 31;

	public static class Mask
	{
		//Add anything to ignore
		//**TODO** In Work

	}
	static BR_Layer()
	{
		Physics.IgnoreLayerCollision (LocalPlayer, Debris);
		Physics.IgnoreLayerCollision (Debris, Debris);
	}

	private BR_Layer(){}

	public static void Set(GameObject obj, int layer, bool recursive = false)
	{
		if (layer < 0 || layer > 31) 
		{
			Debug.LogError("BR_Layer: Attempted to set a layer id out of range [0, 31]");
			return;
		}

		obj.layer = layer;
		if (recursive) 
		{
			foreach(Transform t in obj.transform)
				Set (t.gameObject, layer, true);
		}
	}

	public static bool IsInMask(int layer, int layerMask)
	{
		return (layerMask & 1 << layer) == 0;
	}
}
