using UnityEngine;
using System.Collections.Generic;

namespace BallRunner.Manager
{
    public class PlatformManager : MonoBehaviour
    {
        #region Data
        public Transform floorPrefab;
        public Transform rampPrefab;
        private int numberOfObjects = 10;
        private float recycleOffset;

        private Vector3 startPosition;
        private Material[] materials;
        private PhysicMaterial[] physicMaterials;

        private Vector3 nextPosition;
        private Queue<Transform> objectQueue;
        #endregion

        public void Start()
        {
            floorPrefab = (Transform)Resources.Load("Prefab/Platform", typeof(Transform));
            rampPrefab = (Transform)Resources.Load("Prefab/Ramp", typeof(Transform));

            objectQueue = new Queue<Transform>(numberOfObjects);
            Debug.Log("Quing objects.");
            for (int i = 0; i < numberOfObjects; i++)
            {
                objectQueue.Enqueue((Transform)Instantiate(floorPrefab,
                  new Vector3(0.0f, 0.0f, -100.0f), Quaternion.identity));
            }
            Debug.Log("Object Queue");
        }

        void Update()
        {
            //if (objectQueue.Peek().localPosition.x + recycleOffset < player.distanceTraveled)
               // Recycle();
            if (Input.GetMouseButtonDown(0))
            {
                Recycle();            
            }

        }

        private void Recycle()
        {
            Vector3 position = nextPosition;
            //Add JumpBoost or SpeedBoost
            Transform o = objectQueue.Dequeue();
            o.localPosition = position;
            //Materials Random index and set
            objectQueue.Enqueue(o);

            nextPosition = new Vector3(
                nextPosition.x, nextPosition.y, nextPosition.z + 4);
        }

        private void PlatformStart()
        {
            nextPosition = startPosition;
            for (int i = 0; i < numberOfObjects; i++)
                Recycle();
        }
    }
}