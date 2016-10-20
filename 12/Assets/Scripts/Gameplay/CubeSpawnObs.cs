using UnityEngine;
using System.Collections.Generic;

namespace BallRunner.Manager
{
    public class CubeSpawnObs : MonoBehaviour
    {

        #region Data
        public Transform[] cubePrefabs;
        public int nObjects;
        private int nObjPerLine = 8;
        private int LaneAmountObj = 8;
        private float recycleOffset;

        private Vector3 startPosition;
        public Vector3 minGap, maxGap;
        public Vector3 randomVector;
        public float minZ, maxZ;
        public float minX, maxX;
        private Vector3 nextPosition;
        private List<Transform> ObjectQueue;
        int x;
        private bool notSpawn;
        public float YValueOnLane;
        #endregion

        // Use this for initialization
        public void Awake()
        {
            x = (int)this.transform.position.x;
            ObjectQueue = new List<Transform>();
            for (int i = 0; i < nObjects; i++)
            {
                ObjectQueue.Add(cubePrefabs[i]);
            }
            //minGap = new Vector3(-48.0f, 0.0f, 0.0f);
            //maxGap = new Vector3(14.0f, 0.0f, 50.0f);
            //startPosition = transform.position;
            //nextPosition = startPosition;
            nextPosition = new Vector3(0.0f, 0.0f, 0.0f);
            notSpawn = false;
        }

        private void Update()
        {

        }

        private void Recycle()
        {
            int RandomP = Random.Range(0, 100);
            if (RandomP > 60)
                return;
            
            int iRand = Random.Range(0, 10);
            Vector3 position = nextPosition;
            Debug.Log("Position = " + position);

            if (ObjectQueue[iRand] == null)
                iRand = 0;
            Transform o = (Transform)Instantiate(ObjectQueue[iRand],
                new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            nextPosition += new Vector3(Random.Range(minGap.x, maxGap.x), 0.0f, Random.Range(minGap.z, maxGap.z));

            Debug.Log("Next = " + nextPosition);
            //o.localPosition = nextPosition;
            //o.position = nextPosition;
            o.parent = gameObject.transform;
            o.localPosition = nextPosition;
            //o.localRotation = new Quaternion(0.0f, Random.Range(0, 30), 0.0f, 0.0f);
            if (nextPosition.x < minX)
                nextPosition.x = minX - maxGap.x;
            else if (nextPosition.x > maxX)
                nextPosition.x = minX - maxGap.x;
            if (nextPosition.z < minZ)
                nextPosition.z = minZ - maxGap.z;
            else if (nextPosition.z > maxZ)
                nextPosition.z = minZ - maxGap.z;
            
            
        }
        private void OnTriggerEnter(Collider other)
        {
            int rad = Random.Range(10, 50);
            if (other.tag != "Player" || notSpawn)
                return;
            else
            {
                for (int i = 0; i < 40; i++)
                {
                    Recycle();
                }
                notSpawn = true;
            }
        }
    }

}