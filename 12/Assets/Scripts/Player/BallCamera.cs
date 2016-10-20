using UnityEngine;
using System.Collections;

public class BallCamera : MonoBehaviour {

	public GameObject target;
	public Vector3 offset;
	Vector3 targetPos;
	// Use this for initialization
	void Start () 
	{
		offset = transform.position - target.transform.position;
	}

	// Update is called once per frame
	void LateUpdate () {
		if (target)
		{

			transform.position = target.transform.position + offset;
		}
	}
}
