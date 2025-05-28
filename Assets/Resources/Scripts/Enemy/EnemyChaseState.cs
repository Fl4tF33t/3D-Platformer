using UnityEngine;
using UnityEngine.AI;

namespace Platformer {
    public class EnemyChaseState : EnemyBaseState {
        private readonly Transform player;
        public EnemyChaseState(Enemy enemy, NavMeshAgent agent, Animator animator, Transform player) : base(enemy, agent, animator) {
            this.player = player;
        }

        public override void OnEnter() {
            animator.CrossFade(LocomotionHash, CROSS_FADE_DURATION);
        }

        public override void OnUpdate() {
            agent.SetDestination(player.position);
            animator.SetFloat(Speed, agent.velocity.magnitude);
        }
    }
}