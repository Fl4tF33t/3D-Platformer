using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static PlayerInputAction;

namespace Platformer
{
    [CreateAssetMenu(fileName = "InputAction", menuName = "ScriptableObject/InputAction")]
    public class InputReader : ScriptableObject, IPlayerActions {
        public event UnityAction<Vector2> Move = delegate { };
        public event Action<Vector2> onMove;
        public event UnityAction<Vector2, bool> Look = delegate { };
        public event Action<bool> onMouseControlCamera;
        public event UnityAction EnableMouseControlCamera = delegate { };
        public event UnityAction DisableMouseControlCamera = delegate { };
        public event UnityAction<bool> Jump = delegate { };

        public Vector3 Direction => playerInputAction.Player.Move.ReadValue<Vector2>();
        private PlayerInputAction playerInputAction;
        public void Enable() => playerInputAction.Enable();
        private void OnEnable() {
            if (playerInputAction == null) {
                playerInputAction = new PlayerInputAction();
                playerInputAction.Player.SetCallbacks(this);
            }
        }

        private void OnDisable() {
            playerInputAction.Disable();
        }

        public void OnMove(InputAction.CallbackContext context) {
            Move.Invoke(context.ReadValue<Vector2>());
            onMove?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnLook(InputAction.CallbackContext context) {
            Look.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context));
        }

        private bool IsDeviceMouse(InputAction.CallbackContext context) =>
            context.control.device.name == "Mouse";

        public void OnFire(InputAction.CallbackContext context) {
            //null
        }

        public void OnMouseCameraController(InputAction.CallbackContext context) {
            switch (context.phase) {
                case InputActionPhase.Started:
                    onMouseControlCamera?.Invoke(true);
                    EnableMouseControlCamera.Invoke();
                    break;
                case InputActionPhase.Canceled:
                    onMouseControlCamera?.Invoke(false);
                    DisableMouseControlCamera.Invoke();
                    break;
                default:
                    break;
            }
        }

        public void OnJump(InputAction.CallbackContext context) {
            switch (context.phase) {
                case InputActionPhase.Started:
                    Jump.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Jump.Invoke(false);   
                    break;
                default:
                    break;
            }        
        }
    }
}
