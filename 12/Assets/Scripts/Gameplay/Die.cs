using UnityEngine;
using System.Collections;
using BallRunner.Player;

public class Die : MonoBehaviour {

    StatusCore stats;
	// Use this for initialization
	void Start () {

        stats = GameObject.FindGameObjectWithTag("Player").GetComponent<StatusCore>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.transform.position = stats.startPosition;
            stats.Change();
            stats.deaths++;
        }
    }
}
