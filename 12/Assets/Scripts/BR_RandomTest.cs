using UnityEngine;
using System.Collections;

public class BR_RandomTest : MonoBehaviour {

    [SerializeField]
    int rTextureID;
	void Start () 
    {
        rTextureID = Random.Range(0, 9);
	}
	
	void Update () 
    {
        Debug.Log("Random number: " + rTextureID); 
	
	}
}
