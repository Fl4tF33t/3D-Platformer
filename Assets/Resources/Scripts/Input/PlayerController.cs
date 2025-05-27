using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Platformer {
    public class PlayerController : MonoBehaviour {
        [Header("References")] 
        [SerializeField] private InputReader inputReader;

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float rotationSpeed = 15f;
        [SerializeField] private float smoothTime = 0.2f;
        
        [Header("Jump Settings")]
        [SerializeField] private float jumpForce = 10f;
        [SerializeField] private float jumpDuration = 0.5f;
        [SerializeField] private float jumpCooldown = 0f;
        [SerializeField] private float jumpHeightMax = 2f;
        [SerializeField] private float gravityMultiplier = 3f;
        
        private const float ZERO_F = 0f;
        private Rigidbody rigidBody;
        private GroundChecker groundChecker;
        private Animator animator;
        private Transform mainCamera;
        
        private float currentSpeed;
        private float velocity;
        private Vector3 movement;
        private float jumpVelocity;

        private List<Timer> timers;
        private CountdownTimer jumpTimer;
        private CountdownTimer jumpCooldownTimer;
        
        //Animator properties
        static readonly int Speed = Animator.StringToHash("Speed");

        private void Awake() {
            mainCamera = Camera.main.transform;
            rigidBody = gameObject.GetOrAdd<Rigidbody>();
            groundChecker = gameObject.GetOrAdd<GroundChecker>();
            animator = gameObject.GetOrAdd<Animator>();
            
            // Setup timers
            jumpTimer = new CountdownTimer(jumpDuration);
            jumpCooldownTimer = new CountdownTimer(jumpCooldown);
            timers = new List<Timer>(2) { jumpTimer, jumpCooldownTimer };
            jumpTimer.OnTimerStop += () => jumpCooldownTimer.Start();
        }

        private void OnEnable() {
            inputReader.Jump += OnJump;
        }

        private void OnDisable() {
            inputReader.Jump -= OnJump;
        }

        private void OnJump(bool performed) {
            if (performed && !jumpTimer.IsRunning && !jumpCooldownTimer.IsRunning && groundChecker.IsGrounded) {
                jumpTimer.Start();
            } else if (!performed && jumpTimer.IsRunning) {
                jumpTimer.Stop();
            }
        }

        private void Start() => inputReader.Enable();

        private void Update() {
            movement = new Vector3(inputReader.Direction.x, 0, inputReader.Direction.y);

            HandleTimers();
            UpdateAnimator();
        }
        private void FixedUpdate() {
            HandleMovement();
            HandleJump();
        }

        private void HandleTimers() {
            foreach (var timer in timers) {
                timer.Tick(Time.deltaTime);
            }
        }

        private void UpdateAnimator() {
            animator.SetFloat(Speed, currentSpeed);
        }
        private void HandleJump() {
            // If not jumping and grounded
            if (!jumpTimer.IsRunning && groundChecker.IsGrounded) {
                jumpVelocity = ZERO_F;
                jumpTimer.Stop();
                return;
            }
            
            // If jumping or falling, calculate velocity
            if (jumpTimer.IsRunning) {
                // progress point, initial jump velocity burst
                float launchPoint = 0.9f;
                if (jumpTimer.Progress > launchPoint) {
                    // Velocity to reach max height (v = sqrt(2gh))
                    jumpVelocity = Mathf.Sqrt(2 * jumpHeightMax * Mathf.Abs(Physics.gravity.y));
                } else {
                    // Linear interpolation between initial velocity and max height
                    jumpVelocity += (1 - jumpTimer.Progress) * jumpForce * Time.fixedDeltaTime;
                }
            } else {
                jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
            }
            
            // Apply physics
            rigidBody.linearVelocity = new Vector3(rigidBody.linearVelocity.x, jumpVelocity, rigidBody.linearVelocity.z);
        }

        private void HandleMovement() {
            // Rotate movement direction to match the camera rotation
            Vector3 adjustedDirection = Quaternion.AngleAxis(mainCamera.eulerAngles.y, Vector3.up) * movement;
            
            if (adjustedDirection.magnitude > ZERO_F) {
                HandleRotation(adjustedDirection);
                HandleHorizontalMovement(adjustedDirection);
                SmoothSpeed(adjustedDirection.magnitude);
            } 
            else SmoothSpeed(ZERO_F);
        }
        private void HandleHorizontalMovement(Vector3 adjustedDirection) {
            // Move the character
            Vector3 adjustedVelocity = adjustedDirection * (moveSpeed * Time.fixedDeltaTime);;
            rigidBody.linearVelocity = new Vector3(adjustedVelocity.x, rigidBody.linearVelocity.y, adjustedVelocity.z);
        }
        private void HandleRotation(Vector3 adjustedDirection) {
            Quaternion targetRotation = Quaternion.LookRotation(adjustedDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.LookAt(transform.position + adjustedDirection);
        }
        private void SmoothSpeed (float targetSpeed) => currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref velocity, smoothTime);
    }
}