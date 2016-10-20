using UnityEngine;
using System.Collections.Generic;
using BallRunner.Manager;

namespace BallRunner.Manager
{
    public class br_LaneManager : MonoBehaviour
    {
        public List<BaseLane> typeOfLane = new List<BaseLane>();
        public Transform blocks;
        public int numberOfObjects;
        public float recycleOffset;

        [SerializeField]
        private int CurrentIndex;

        public Vector3 startPosition;

        public float minY;
        public float maxY;
        private StatusCore core;
        //public PhysicMaterial[] physicsMaterials;

        [SerializeField]
        private Vector3 nextPosition;
        [SerializeField]
        private int randomNext;
        [SerializeField]
        private int iRand;
        private List<Transform> objectQueue;

        public bool spawnTrigger = false;
   
        public void Awake()
        {
            GameEventManager.GameStart += GameStart;
          //  GameEventManager.GameOver += GameOver;
            core = GameObject.FindGameObjectWithTag("Player").GetComponent<StatusCore>();
            objectQueue = new List<Transform>();
            for (int i = 0; i < numberOfObjects; i++)
            {
                //objectQueue.Add((Transform)Instantiate(typeOfLane[i].Prefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity));
                objectQueue.Add(typeOfLane[i].Prefab);
            }
            //Activate Once game starts.
            //enabled = false;
            Starting();
            enabled = true;
        }

        public void Update()
        {
            
            // if (objectQueue[CurrentIndex].localPosition.z)
            if (spawnTrigger)
            {
                Debug.Log("Recycling");
                Recycle();
                spawnTrigger = false;
            }
            
        }


        private void Recycle()
        {
            //Get the Type of Lane Solo or Base
            randomNext = Random.Range(0, 2);
            Debug.Log("Spawning Lane: " + objectQueue[iRand].name + ", Spawning at: " + nextPosition);
            Vector3 position = nextPosition;
            position.z += typeOfLane[iRand].position.z;
            Debug.Log("Setting o Transform.");
            Transform o = (Transform)Instantiate(objectQueue[iRand], new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);

            //Spawn the lane, Rotation in work
            o.localPosition = position;

            Debug.Log("Object has spawn.");

            //Give Boost or Stop
            int randLn = Random.Range(0, 2);
            randLn += 2;
            Debug.Log("Random Number: " + randLn);
            Transform k = (Transform)Instantiate(objectQueue[randLn], new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            position.z += typeOfLane[randLn].position.z + 186;
            Debug.Log("Spawning: " + objectQueue[randLn].name + ", spawning at: " + position);
            k.localPosition = position;
            Transform b = (Transform)Instantiate(blocks, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            b.localPosition = position;

            //NextPosition
            nextPosition += new Vector3(
                typeOfLane[iRand].position.x,
                typeOfLane[iRand].position.y,
                typeOfLane[iRand].position.z + 4);
            iRand = randomNext;
            Debug.Log("Next Lane: " + objectQueue[randomNext].name + ", Spawning at: " + nextPosition);

        }

        private void GameStart()
        {
            nextPosition = startPosition;
            for (int i = 0; i < numberOfObjects; i++)
                Recycle();
            Debug.Log("Game Start, Level has been loaded.");
            enabled = true;
        }
        /*
        private void GameOver()
        { enabled = false; }
        */
        private void Starting()
        {
            nextPosition = startPosition;
            FirstRecycle();
            Debug.Log("Game Start, Level has been loaded.");
        }

        private void FirstRecycle()
        {
            Vector3 position = nextPosition;
            Debug.Log("First");
            Transform o = (Transform)Instantiate(objectQueue[1], new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);


            o.localPosition = position;
            foreach (Transform c in o)
            {
                c.renderer.material = Resources.Load<Material>("Materials/" + "proto_blue");
            }
            Debug.Log("Object has spawn.");
            position = objectQueue[1].position;

            /*nextPosition += new Vector3(
                typeOfLane[0].position.x,
                typeOfLane[0].position.y,
                typeOfLane[0].position.z);*/
            nextPosition.z += position.z;

        }
    }
}