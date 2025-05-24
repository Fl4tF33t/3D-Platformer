using System;
using Unity.Cinemachine;
using UnityEngine;
using Utilities;

namespace Platformer {
    public class PlayerController : MonoBehaviour {
        [Header("References")] 
        [SerializeField] private InputReader inputReader;

        [Header("Settings")]
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float rotationSpeed = 15f;
        [SerializeField] private float smoothTime = 0.2f;

        private const float ZERO_F = 0f;
        private CharacterController characterController;
        private Animator animator;
        private Transform mainCamera;
        
        private float currentSpeed;
        private float velocity;
        
        //Animator properties
        static readonly int Speed = Animator.StringToHash("Speed");

        private void Awake() {
            mainCamera = Camera.main.transform;
            characterController = gameObject.GetOrAdd<CharacterController>();
            animator = gameObject.GetOrAdd<Animator>();
        }

        private void Start() => inputReader.Enable();

        private void Update() {
            HandleMovement();
            UpdateAnimator();
        }

        private void UpdateAnimator() {
            animator.SetFloat(Speed, currentSpeed);
        }

        private void HandleMovement() {
            Vector3 movementDirection = new Vector3(inputReader.Direction.x, 0, inputReader.Direction.y);
            // Rotate movement direction to match the camera rotation
            Vector3 adjustedDirection = Quaternion.AngleAxis(mainCamera.eulerAngles.y, Vector3.up) * movementDirection;
            
            if (adjustedDirection.magnitude > ZERO_F) {
                HandleRotation(adjustedDirection);
                HandleCharacterController(adjustedDirection);
                SmoothSpeed(adjustedDirection.magnitude);
            } 
            else SmoothSpeed(ZERO_F);
        }
        private void HandleCharacterController(Vector3 adjustedDirection) {
            // Move the character
            Vector3 adjustedMovement = adjustedDirection * (moveSpeed * Time.deltaTime);;
            characterController.Move(adjustedMovement);
        }
        private void HandleRotation(Vector3 adjustedDirection) {
            Quaternion targetRotation = Quaternion.LookRotation(adjustedDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.LookAt(transform.position + adjustedDirection);
        }
        private void SmoothSpeed (float targetSpeed) => currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref velocity, smoothTime);
    }
}