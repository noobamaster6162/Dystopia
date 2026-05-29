
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scenes.Scripts
{
    public class MouseMovement : MonoBehaviour
    {

        [Header("Settings")] [SerializeField] public float mouseSensitivity = 1f;
        [SerializeField] public Transform playerBody;
        [SerializeField] public Camera mainCamera;
        [SerializeField] public Transform cameraSocket;

        public InputActionReference lookAction;
        private float _rotationX;
        private float _rotationY;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            mainCamera.transform.position = cameraSocket.position;
            //mainCamera.transform.position = playerBody.position;

            HandleRotation();
        }

        void OnEnable() => lookAction.action.Enable();
        void OnDisable() => lookAction.action.Disable();

        private void HandleRotation()
        {
            Vector2 lookInput = lookAction.action.ReadValue<Vector2>();

            float mouseX = lookInput.x * mouseSensitivity;
            float mouseY = lookInput.y * mouseSensitivity;

            _rotationX -= mouseY;
            _rotationX = Mathf.Clamp(_rotationX, -90f, 90f);

            _rotationY += mouseX;

            cameraSocket.localRotation = Quaternion.Euler(_rotationX, _rotationY, 0f);
            playerBody.localRotation = Quaternion.Euler(0f, _rotationY, 0f);
            mainCamera.transform.rotation = cameraSocket.rotation;
        }
    }
}
