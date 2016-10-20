using UnityEngine;
using System.Collections;
using BallRunner.Manager;

public class hitterOnBases : MonoBehaviour {

    public br_LaneManager manager;
    public bool nothingToSee;
    void Awake()
    {
        manager = GameObject.FindGameObjectWithTag("GM").GetComponent<br_LaneManager>();
        nothingToSee = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !nothingToSee)
        {
            manager.spawnTrigger = true;
            nothingToSee = true;
        }
    }
}
