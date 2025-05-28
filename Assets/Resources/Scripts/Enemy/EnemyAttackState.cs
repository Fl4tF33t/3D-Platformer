using UnityEngine;
using UnityEngine.AI;

namespace Platformer {
    public class EnemyAttackState : EnemyBaseState {
        private readonly Transform player;
        public EnemyAttackState(Enemy enemy, NavMeshAgent agent, Animator animator, Transform player) : base(enemy, agent, animator) {
            this.player = player;
        }

        public override void OnEnter() {
            animator.CrossFade(AttackHash, CROSS_FADE_DURATION);
        }
        
        public override void OnUpdate() {
            agent.SetDestination(player.position);
            enemy.Attack();
        }
    }
}