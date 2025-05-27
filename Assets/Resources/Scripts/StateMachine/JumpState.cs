using UnityEngine;

namespace Platformer {
    public class JumpState : BaseState {
        public JumpState(PlayerController player, Animator animator) : base(player, animator) {
        }

        public override void OnEnter() {
            animator.CrossFade(JumpHash, CROSS_FADE_DURATION);
        }

        public override void OnFixedUpdate() {
            player.HandleJump();
            player.HandleMovement();
        }
        
        public override void OnExit() {
            
        }
    }
}