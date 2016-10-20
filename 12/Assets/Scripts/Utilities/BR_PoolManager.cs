/////////////////////////////////////////////////////////////
///														  ///
/// BR_POOLMANAGER.cs 									  ///
/// 													  ///
/// Description: Class that manages the pooling objects   ///
/// 		only occur if the object has the script in    ///
/// 		the scene and enabled. Overrides              ///
/// 		object.Instantiate and object.Destroy.        ///
/// 													  ///
/////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BR_PoolManager : MonoBehaviour 
{

	public class BR_CustomPoolObject
	{
		public GameObject Prefab = null;	//Prefab for check on Pool
		public int Buffer = 15;				// Amount of objects to instantiate at Start
		public int MaxAmount = 25;			// Max Amount of objects that will be Pooled
	}

	public int MaxAmount = 25;				// Max amount that will be pooled, otherwise send to CustomPrefab
	public bool PoolOnDestroy = true;		// If an object is not pooled, but destroyed, it´ll be added to pool
	public List<GameObject> IgnoredPrefabs = new List<GameObject>();					//Prefabs List will not be Pooled
	public List<BR_CustomPoolObject> CustomPrefabs = new List<BR_CustomPoolObject>();	//Specify settings for specific prefabs by adding

	protected Transform m_Transform; 		// The transform of the object. Used for Parenting
	protected Dictionary<string, List<Object>> m_AvailableObjects = new Dictionary<string, List<Object>>();  // Available objects
	protected Dictionary <string, List<Object>> m_UsedObjects = new Dictionary<string, List<Object>>();		 // Objects currently on use

	/// <summary>
	/// Instance of the Script in the object
	/// </summary>
	protected static BR_PoolManager m_Instance = null;
	public static BR_PoolManager Instance { get { return m_Instance; } }

	/// <summary>
	/// Sets the Instance and Transform of the object in Awake Function
	/// </summary>
	protected virtual void Awake()
	{
		m_Instance = this;
		m_Transform = transform;
	}


	protected virtual void Start()
	{
		foreach (BR_CustomPoolObject obj in CustomPrefabs)
			AddObjects (obj.Prefab, Vector3.zero, Quaternion.identity, obj.Buffer);
	}

	/// <summary>
	/// Registers the pooling Events
	/// </summary>
	protected virtual void OnEnable()
	{
		BR_GlobalEventReturn<Object, Vector3, Quaternion, Object>.Register ("BR_PoolManager Instantiate", InstantiateInternal);
		BR_GlobalEvent<Object, float>.Register ("BR_PoolManager Destroy", DestroyInternal);
	}

	/// <summary>
	/// Unregisters the pooling Events.
	/// </summary>
	protected virtual void OnDisable()
	{
		BR_GlobalEventReturn<Object, Vector3, Quaternion, Object>.Unregister ("BR_PoolManager Instantiate", InstantiateInternal);
		BR_GlobalEvent<Object, float>.Unregister ("BR_PoolManager Destroy", DestroyInternal);
	}

	/// <summary>
	/// Adds the object(S) for Party Pooling.
	/// </summary>
	public virtual void AddObjects(Object obj, Vector3 position, Quaternion rotation, int amount = 1)
	{
		//Error Handling
		if (obj == null)
			return;

		// Add to available objects dictionary if it doesn´t exist
		if (!m_AvailableObjects.ContainsKey (obj.name)) 
		{
			m_AvailableObjects.Add (obj.name, new List<Object> ());
			m_UsedObjects.Add(obj.name, new List<Object>());
		}

		for (int i = 0; i < amount; i++) 
		{
			GameObject newObj = GameObject.Instantiate(obj, position, rotation) as GameObject;
			newObj.name = obj.name;
			newObj.transform.parent = m_Transform;
			BR_Utility.Activate(newObj, false);
			m_AvailableObjects[obj.name].Add (newObj);

		}
	}

	protected virtual Object InstantiateInternal(Object original, Vector3 position, Quaternion rotation)
	{

		//Check for ignored List if it is, Instantiate a new Object
		if (IgnoredPrefabs.FirstOrDefault (obj => obj.name == original.name || obj.name == original.name + "(Clone)") != null)
			return Object.Instantiate (original, position, rotation) as Object;

		GameObject go = null;
		List<Object> availableObjects = null;
		List<Object> usedObjects = null;

		// Check if this object is already being pooled
		if (m_AvailableObjects.TryGetValue (original.name, out availableObjects)) 
		{
		Retry:
				m_UsedObjects.TryGetValue(original.name, out usedObjects);

			// Check if the object has reach max amount
			int objectCount = availableObjects.Count + usedObjects.Count;
			if(CustomPrefabs.FirstOrDefault(obj => obj.Prefab.name == original.name) == null && objectCount < MaxAmount && availableObjects.Count == 0)
				AddObjects(original, position, rotation);

			//if no objects are available, get a used object and retry
			if(availableObjects.Count == 0)
			{
				go = usedObjects.FirstOrDefault() as GameObject;

				if(go == null)
				{
					usedObjects.Remove (go);
					goto Retry;
				}

				//Deavtivate objecte
				BR_Utility.Activate(go, false);
				//remove the object from used object list
				usedObjects.Remove(go);

				//add it to the available objects
				availableObjects.Add (go);

				//And try and instantiate again
				goto Retry;
			}

			//Get the first available object
			go = availableObjects.FirstOrDefault() as GameObject;

			//check if object still exist
			if(go == null)
			{
				availableObjects.Remove(go);
				goto Retry;
			}

			//Set the position and rotation
			go.transform.position = position;
			go.transform.rotation = rotation;

			//Remove from available
			availableObjects.Remove (go);

			//Add to the used object list
			usedObjects.Add (go);

			//Activate the object
			BR_Utility.Activate(go);

			return go;
		}

		//Add a new object if this type isnt being pooled
		AddObjects (original, position, rotation);

		return InstantiateInternal (original, position, rotation);
	}

	protected virtual void DestroyInternal(Object obj, float t)
	{
		if (obj == null)
			return;

		//Check if this prefab is in the ignore list or if it is not pooled and destroy it 
		if (IgnoredPrefabs.FirstOrDefault (o => o.name == obj.name || o.name == obj.name + "(Clone)") != null || (!m_AvailableObjects.ContainsKey (obj.name) && !PoolOnDestroy)) 
		{
			Object.Destroy (obj, t);
			return;
		}

		//Handle Timed Destroy
		if(t != 0)
		{
			BR_Timer.In (t, delegate { DestroyInternal(obj, 0); });
			return;
		}

		//Not being pooled add it
		if (!m_AvailableObjects.ContainsKey (obj.name)) 
		{
			AddObjects(obj, Vector3.zero, Quaternion.identity);
			return;
		}

		List<Object> availableObjects = null;
		List<Object> usedObjects = null;
		m_AvailableObjects.TryGetValue (obj.name, out availableObjects);
		m_UsedObjects.TryGetValue (obj.name, out usedObjects);

		GameObject go = usedObjects.FirstOrDefault (o => o.GetInstanceID () == obj.GetInstanceID ()) as GameObject;

		if (go == null)
			return;

		go.transform.parent = m_Transform;

		//Disable the object 
		BR_Utility.Activate(go, false);

		usedObjects.Remove (go);
		availableObjects.Add (go);
		
	} 
}
