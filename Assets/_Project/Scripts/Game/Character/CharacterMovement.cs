using System.Globalization;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;


namespace Character
{
    [RequireComponent(typeof(NetworkTransform))]
    [RequireComponent(typeof(CharacterController))]
    public class CharacterMovement : NetworkBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField]
        private float moveSpeed = 5f;
        [SerializeField]
        private float jumpForce = 1.5f;
        [SerializeField]
        private float mouseSensitivity = 2f;
        private float gravity = -9.81f;


        [Header("References")]
        [SerializeField]
        private Camera playerCamera;
        [SerializeField]
        private CharacterController controller;
        [SerializeField]
        private LayerMask groundLayer;
        private float clampAngle = 85f;

        private Vector3 velocity;
        private float verticalRotation = 0f;

        private void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

        }

        private void Update()
        {
            if (IsOwner)
            {
                HandleMouseLook();
                HandleMovement();
                HandleJump();

            }
            else
            {
                playerCamera.gameObject.SetActive(false);
                enabled = false;
            }
        }

        private void HandleMouseLook()
        {
            if (!GameClientsNerworkInfo.Singleton.CharacterPermissions.HasPermission(CharacterPermissionsType.CameraRotate))
                return;

            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, -clampAngle, clampAngle);

            playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }
        private void HandleMovement()
        {
            if (!GameClientsNerworkInfo.Singleton.CharacterPermissions.HasPermission(CharacterPermissionsType.Movement))
                return;

            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            Vector3 move = transform.right * moveX + transform.forward * moveZ;
            controller.Move(move * moveSpeed * Time.deltaTime);
        }

        private void HandleJump()
        {
            if (!GameClientsNerworkInfo.Singleton.CharacterPermissions.HasPermission(CharacterPermissionsType.Jump))
                return;

            if (IsGroundedCustom() && velocity.y < 0)
                velocity.y = -2f;

            if (Input.GetButtonDown("Jump") && IsGroundedCustom())
                velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
        bool IsGroundedCustom()
        {
            return Physics.Raycast(transform.position, Vector3.down, out _, 1.1f);
        }
    }
}