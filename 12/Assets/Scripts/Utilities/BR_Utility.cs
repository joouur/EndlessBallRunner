/////////////////////////////////////////////////////////////
///														  ///
/// BR_UTILITY.cs 	    								  ///
/// 													  ///
/// Description: Miscellaneous utlity functions			  ///
/// 													  ///
/////////////////////////////////////////////////////////////

using UnityEngine;
using System.Diagnostics;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;


public static class BR_Utility
{

	/// <summary>
	/// Activate the specified obj, by setting the bool activate.
	/// </summary>
	public static void Activate(GameObject obj, bool activate = true)
	{
		obj.SetActive (activate);
	}

	/// <summary>
	/// Determines if is active the specified obj.
	/// </summary>
	/// <returns><c>true</c> if is active the specified obj; otherwise, <c>false</c>.</returns>
	/// <param name="obj">Object.</param>
	public static bool IsActive(GameObject obj)
	{
		return obj.activeSelf;
	}

	/// <summary>
	/// Instantiate the specified original, for pool Manager to work.
	/// </summary>
	public static UnityEngine.Object Instantiate( UnityEngine.Object original)
	{
		return BR_Utility.Instantiate (original, Vector3.zero, Quaternion.identity);
	}

	public static UnityEngine.Object Instantiate (UnityEngine.Object original, Vector3 position, Quaternion rotation)
	{
		if (BR_PoolManager.Instance == null || !BR_PoolManager.Instance.enabled) 
			return GameObject.Instantiate (original, position, rotation);
		else
			return BR_GlobalEventReturn<UnityEngine.Object, Vector3, Quaternion, UnityEngine.Object>.Send ("BR_PoolManager Instantiate", original, position, rotation);
	}

	/// <summary>
	///Destroys Object for PoolManager to work
	/// </summary>
	public static void Destroy(UnityEngine.Object obj)
	{
		BR_Utility.Destroy (obj, 0);
	}

	public static void Destroy(UnityEngine.Object obj, float t)
	{
		if (BR_PoolManager.Instance == null || !BR_PoolManager.Instance.enabled)
			UnityEngine.Object.Destroy (obj, t);
		else
			BR_GlobalEvent<UnityEngine.Object, float>.Send ("BR_PoolManager Destroy", obj, t);
	}

}








