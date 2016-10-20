using UnityEngine;
using BallRunner.Manager;

namespace BallRunner.Player
{
    public class PlayerController : MonoBehaviour
    {
        /*
        #region Data
        public static float distanceTraveled;

        private float acceleration;
        [SerializeField]
        private float speed;
        [SerializeField]
        private float maxSpeed;
        private float gameOverY;
        private static int boosts;

        private Vector3 startPosition;
        private Vector3 boostVelocity;
        private Vector3 jumpVelocity;

        [SerializeField]
        private bool _isGrounded;
        public bool isGrounded
        {
            get { return _isGrounded; }
            set { _isGrounded = value; }
        }

        public float curreSpeed;
        #endregion
        // Use this for initialization
        void Start()
        {
            jumpVelocity = new Vector3(0.0f, 8.0f, 0.0f);
            gameOverY = 12.0f;
            acceleration = .025f;
            maxSpeed = 30;

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetButtonDown("Jump"))
            {
                if (isGrounded)
                {
                    rigidbody.AddForce(jumpVelocity, ForceMode.VelocityChange);
                    isGrounded = false;
                }
            }

            distanceTraveled = transform.localPosition.z;

            if (transform.localPosition.y < gameOverY)
            { GameEventManager.TriggerGameOver(); }
            curreSpeed = rigidbody.velocity.z;
        }

        private void FixedUpdate()
        {
            float h = Input.GetAxis("Horizontal");

            Vector3 movement = new Vector3(h, 0.0f, speed);
            if (isGrounded)
            {
                if (Mathf.Abs(rigidbody.velocity.magnitude) > maxSpeed)
                    rigidbody.velocity = rigidbody.velocity.normalized * maxSpeed;
                else
                    rigidbody.AddForce(movement * speed);   
            }
            if (speed <= maxSpeed)
                speed += acceleration;
        }
        #region Helper Functions
        void OnCollisionEnter()
        { isGrounded = true; }

        void OnCollisionExit()
        { isGrounded = false; }


        #endregion
         */
        [SerializeField]
        private float movePower = 5;
        [SerializeField]
        private bool useTorque = true;
        [SerializeField]
        private float MaxAngularVelocity = 25;
        [SerializeField]
        private float jumpPower = 2;

        private const float groundRay = 1f;
        private Rigidbody rigidBody;

        private Vector3 move;

        private Transform cam; 
        private Vector3 camForward;
        private bool jump = false;
        private void Start()
        {
            rigidBody = GetComponent<Rigidbody>();
            GetComponent<Rigidbody>().maxAngularVelocity = MaxAngularVelocity;
        }

        bool effing;
        private void Awake()
        {
            if (Camera.main != null)
            {
                cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Ball needs a Camera tagged \"MainCamera\", for camera-relative controls.");
                // we use world-relative controls in this case, which may not be what the user wants, but hey, we warned them!
            }
            effing = true;  
 
        }

        public void Update()
        {
            float h = Input.GetAxis("Horizontal");
            float v = 0.5f;
            jump = Input.GetButton("Jump");

            if (cam != null)
            {
               camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
                move = (v * camForward + h * cam.right).normalized;
            }
            else
                move = (v * Vector3.forward + h * Vector3.right).normalized;

            /*
            if (effing)
            {
                Change();
                effing = false;
                Invoke("ef", 1.0f);
            }
             * */
        }

        public void FixedUpdate()
        {
            Move(move, jump);
            jump = false;
        }

        public void Move(Vector3 m, bool j)
        {
            if (useTorque)
                rigidBody.AddTorque(new Vector3(m.z, 0.0f, -m.x) * movePower);
            else
                rigidBody.AddForce(m * movePower);

            if (Physics.Raycast(transform.position, -Vector3.up, groundRay) && j)
                rigidBody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
        public void Change()
        {
            Color newColor = new Color(Random.value, Random.value, Random.value, 1.0f);

            // apply it on current object's material
            renderer.material.color = newColor;
        }
        private void ef()
        {
            effing = true;
        }
    }
}