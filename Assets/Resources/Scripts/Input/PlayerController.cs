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
        [SerializeField] private float jumpCooldown = 0.5f;
        [SerializeField] private float gravityMultiplier = 3f;
        
        [Header("Dash Settings")]
        [SerializeField] private float dashForce = 12f;
        [SerializeField] private float dashDuration = 0.5f;
        [SerializeField] private float dashCooldown = 1.5f;
        
        private const float ZERO_F = 0f;
        private Rigidbody rigidBody;
        private GroundChecker groundChecker;
        private Animator animator;
        private Transform mainCamera;
        
        private Vector3 movement;
        private float currentSpeed;
        private float velocity;
        private float jumpVelocity;
        private float dashVelocity = 1f;

        private List<Timer> timers;
        private CountdownTimer jumpTimer;
        private CountdownTimer jumpCooldownTimer;
        private CountdownTimer dashTimer;
        private CountdownTimer dashCooldownTimer;
        
        //Animator properties
        static readonly int Speed = Animator.StringToHash("Speed");
        
        private StateMachine stateMachine;

        private void Awake() {
            mainCamera = Camera.main.transform;
            rigidBody = gameObject.GetOrAdd<Rigidbody>();
            groundChecker = gameObject.GetOrAdd<GroundChecker>();
            animator = gameObject.GetOrAdd<Animator>();
            
            // Setup timers
            jumpTimer = new CountdownTimer(jumpDuration);
            jumpCooldownTimer = new CountdownTimer(jumpCooldown);
            dashTimer = new CountdownTimer(dashDuration);
            dashCooldownTimer = new CountdownTimer(dashCooldown);
            
            jumpTimer.OnTimerStart += () => jumpVelocity = jumpForce;
            jumpTimer.OnTimerStop += () => jumpCooldownTimer.Start();
            dashTimer.OnTimerStart += () => dashVelocity = dashForce;
            dashTimer.OnTimerStop += () => {
                dashVelocity = 1f;
                dashCooldownTimer.Start();
            };
            
            timers = new List<Timer>(4) { jumpTimer, jumpCooldownTimer, dashTimer, dashCooldownTimer };
            
            // Setup state machine
            stateMachine = new StateMachine();
            
            // Declare states
            LocomotionState locomotionState = new LocomotionState(this, animator);
            JumpState jumpState = new JumpState(this, animator);
            DashState dashState = new DashState(this, animator);
            
            // Define Transitions
            At(locomotionState, jumpState, new FuncPredicate(() => jumpTimer.IsRunning));
            At(locomotionState, dashState, new FuncPredicate(() => dashTimer.IsRunning));
            Any(locomotionState, new FuncPredicate(() => !jumpTimer.IsRunning && !dashTimer.IsRunning && groundChecker.IsGrounded));
            
            // Set the initial state
            stateMachine.SetState(locomotionState);
        }

        private void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        private void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        private void OnEnable() {
            inputReader.Jump += OnJump;
            inputReader.Dash += OnDash;
        }

        private void OnDisable() {
            inputReader.Jump -= OnJump;
            inputReader.Dash -= OnDash;
        }

        private void OnDash(bool performed) {   
            if (performed && !dashTimer.IsRunning && !dashCooldownTimer.IsRunning) {
                dashTimer.Start();
            } else if (!performed && dashTimer.IsRunning) {
                dashTimer.Stop();
            }
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
            stateMachine.Update();

            HandleTimers();
            UpdateAnimator();
        }
        private void FixedUpdate() {
            stateMachine.FixedUpdate();
        }

        private void HandleTimers() {
            foreach (var timer in timers) {
                timer.Tick(Time.deltaTime);
            }
        }

        private void UpdateAnimator() {
            animator.SetFloat(Speed, currentSpeed);
        }

        public void HandleJump() {
            // If not jumping and grounded
            if (!jumpTimer.IsRunning && groundChecker.IsGrounded) {
                jumpVelocity = ZERO_F;
                return;
            }
            
            // If jumping or falling, calculate velocity
            if (!jumpTimer.IsRunning) 
                jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
            
            // Apply physics
            rigidBody.linearVelocity = new Vector3(rigidBody.linearVelocity.x, jumpVelocity, rigidBody.linearVelocity.z);
        }

        public void HandleMovement() {
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
            Vector3 adjustedVelocity = adjustedDirection * (moveSpeed * dashVelocity * Time.fixedDeltaTime);;
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