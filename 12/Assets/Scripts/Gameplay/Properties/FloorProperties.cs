using UnityEngine;
using System.Collections;

namespace BallRunner.Manager
{
    public class FloorProperties : MonoBehaviour
    {
        #region Data
        Vector3 startPosition;
        bool direction = true;
        bool canMove = false;
        bool positiveDirection = true;
        int dir = 0;
        public int fuzzyLogic = 0;

        public Vector3 currentPosition;
        private float moveAmount = 12;
        private float moveSpeed = 2;
        private float delay = 2;
        private static Vector3 movingDirection;

        #endregion

        // Use this for initialization
        void Start()
        {

            fuzzyLogic = Random.Range(0, 100);
            #region Activate Object
            currentPosition = gameObject.transform.localPosition;
            if (currentPosition.z >= 100)
            {
                if (fuzzyLogic > 75)
                    gameObject.SetActive(false);

            }
            else if (currentPosition.z >= 60)
            {
                if (fuzzyLogic > 85)
                    gameObject.SetActive(false);

            }
            else if (currentPosition.z >= 25)
            {
                if (fuzzyLogic > 92)
                    gameObject.SetActive(false);

            }
            else
            {
                if (fuzzyLogic > 95)
                    gameObject.SetActive(false);

            }
            #endregion
            if (gameObject.activeInHierarchy)
            {
                int k = Random.Range(0, 100);
                if (k > 98)
                {
                    canMove = true;
                    startPosition = transform.position;
                    int rand = Random.Range(0, 6);
                    if (rand >= 2)
                    {
                        dir = 1;
                        positiveDirection = true;
                    }
                    else
                    {
                        dir = -1;
                        positiveDirection = false;
                    }
                    transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ
                        | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                    movingDirection = new Vector3(0.0f, dir * moveSpeed, 0.0f);
                }
            }
        }

        private void Update()
        {
            if (canMove)
            {
                if (positiveDirection)
                {
                    if (direction)
                    {
                        transform.position = new Vector3(transform.position.x,
                            transform.position.y + moveSpeed * Time.deltaTime,
                            transform.position.z);
                    }
                    else
                    {
                        transform.position = new Vector3(transform.position.x,
                            transform.position.y - moveSpeed * Time.deltaTime,
                            transform.position.z);
                    }
                    if (transform.position.y >= startPosition.y + moveAmount)
                    {
                        transform.position = new Vector3(transform.position.x,
                            startPosition.y + moveAmount, transform.position.z);
                        direction = false;
                        canMove = false;
                        Invoke("MovingCooldown", delay);
                        dir = -1;
                    }
                    else if (transform.position.y < startPosition.y)
                    {
                        transform.position = new Vector3(transform.position.x,
                            startPosition.y, transform.position.z);
                        direction = true;
                        canMove = false;
                        Invoke("MovingCooldown", delay);
                        dir = 1;
                    }
                }   // If Positive Direction
                else
                {
                    if (direction)
                    {
                        transform.position = new Vector3(transform.position.x,
                            transform.position.y - moveSpeed * Time.deltaTime,
                            transform.position.z);
                    }
                    else
                    {
                        transform.position = new Vector3(transform.position.x,
                            transform.position.y + moveSpeed * Time.deltaTime,
                            transform.position.z);
                    }
                    if (transform.position.y <= startPosition.y - moveAmount)
                    {
                        transform.position = new Vector3(transform.position.x,
                            startPosition.y - moveAmount, transform.position.z);
                        direction = false;
                        canMove = false;
                        Invoke("MovingCooldown", delay);
                        dir = 1;
                    }
                    else if (transform.position.y > startPosition.y)
                    {
                        transform.position = new Vector3(transform.position.x,
                            startPosition.y, transform.position.z);
                        direction = true;
                        canMove = false;
                        Invoke("MovingCooldown", delay);
                        dir = -1;
                    }

                } // else
                movingDirection = new Vector3(0.0f, dir * moveSpeed, 0.0f);
            } // if canMove
            else
                movingDirection = new Vector3(0, 0, 0);
        }

        #region Helper Functions
        private void MovingCooldown()
        { canMove = true; }
        #endregion

    }
}