using Unity.Cinemachine;
using UnityEngine;

namespace Platformer {
    public class CameraManager : MonoBehaviour {
        [Header("References")]
        [SerializeField] private InputReader inputReader;
        [SerializeField] private CinemachineCamera freeLookVCam;

        private InputAxisControllerBase<CinemachineInputAxisController.Reader> inputAxisVCam;

        private void Awake() {
            inputAxisVCam = freeLookVCam.GetComponent<InputAxisControllerBase<CinemachineInputAxisController.Reader>>();
        }

        private void OnEnable() {
            inputReader.onMouseControlCamera += SetCameraMovementLock;
        }
        private void OnDisable() {
            inputReader.onMouseControlCamera -= SetCameraMovementLock;
        }
        private void SetCameraMovementLock(bool value) {
            Cursor.visible = !value;
            foreach (var controller in inputAxisVCam.Controllers) {
                if (controller.Input == null) continue;
                controller.Enabled = value;
            }
        }
    }
}