using UnityEngine;

namespace Platformer {
    public class LocomotionState : BaseState {
        public LocomotionState(PlayerController player, Animator animator) : base(player, animator) {
        }

        public override void OnEnter() {
            animator.CrossFade(LocomotionHash, CROSS_FADE_DURATION);
        }

        public override void OnFixedUpdate() {
            player.HandleMovement();
        }
        public override void OnExit() {
            
        }
    }
}