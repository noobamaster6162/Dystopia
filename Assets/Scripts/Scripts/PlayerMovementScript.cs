
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scenes.Scripts
{
    public class PlayerMovementScript : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private double playerSpeed = 1f;
        [SerializeField] private float sidewaysForce = 20f;
        [SerializeField] private float forwardForce = 20f;
        [SerializeField] private float jumpForce = 1f;
        [SerializeField] private float upwardForce = 15f;

        [Header("Player Settings")]
        private bool _jumped;
        private float _jumpForce = 10f;
        private float _acceleration = 1.25f;
        private bool _sprinted;
        private float _gravity = -9.8f; //acceleration due to gravity

        [SerializeField] private Rigidbody rigidBody;
        [SerializeField] public Transform player;

        public Transform stepLower;
        public Transform stepHigher;

        public RaycastHit PlayerLowerHalf;
        public RaycastHit PlayerUpperHalf;

        private LayerMask _layerMask;
        private Vector3 _moveDirection;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        private void Awake()
        {
            _layerMask = LayerMask.GetMask("Wall", "Character");
        }

        // Update is called once per frame
        void Update()
        {

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                _jumped = true;
            }

            _sprinted = Keyboard.current.leftShiftKey.isPressed;

        }

        //Contains major player movement
        private void FixedUpdate()
        {

            PlayerMovement();
            PlayerRayCast();

        }

        private void PlayerRayCast()
        {
            //Check if something is ahead of the player
            RaycastHit playerHit;

            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out playerHit, 5f, _layerMask))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward), Color.blue);
            }

            //For climbing stairs

            float stepOffset = 0.50f;

            bool lowerHit = Physics.Raycast(stepLower.position, transform.forward, out PlayerLowerHalf, stepOffset, _layerMask);
            bool upperHit = Physics.Raycast(stepHigher.position, transform.forward, out PlayerUpperHalf, stepOffset, _layerMask);

            if (lowerHit && !upperHit)
            {
                rigidBody.position += new Vector3(0f, upwardForce, 0f);
            }
        }

        //For player movement
        private void PlayerMovement()
        {

            Vector3 inputForce = Vector3.zero;

            if (Keyboard.current.wKey.isPressed) inputForce.z += 1f;
            if (Keyboard.current.aKey.isPressed) inputForce.x -= 1f;
            if (Keyboard.current.sKey.isPressed) inputForce.z -= 1f;
            if (Keyboard.current.dKey.isPressed) inputForce.x += 1f;

            _moveDirection = (player.forward * inputForce.z * forwardForce) + (player.right * inputForce.x * sidewaysForce);

            _moveDirection = _moveDirection.normalized * Mathf.Max(forwardForce, sidewaysForce);

            if (Keyboard.current.wKey.isPressed && _sprinted) _moveDirection *= _acceleration;

            if (_moveDirection != Vector3.zero) rigidBody.AddForce(_moveDirection, ForceMode.Acceleration);

            if (_jumped)
            {
                rigidBody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
                rigidBody.AddForce(Vector3.down * _gravity, ForceMode.Acceleration);
                _jumped = false;
            }

        }
    }
}
