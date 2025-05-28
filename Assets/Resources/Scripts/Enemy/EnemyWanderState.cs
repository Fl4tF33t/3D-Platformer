using UnityEngine;
using UnityEngine.AI;

namespace Platformer {
    public class EnemyWanderState : EnemyBaseState {
        private readonly float wanderRadius;
        private readonly Vector3 startPoint;
        
        public EnemyWanderState(Enemy enemy, NavMeshAgent agent, Animator animator, float wanderRadius) : base(enemy, agent, animator) {
            this.wanderRadius = wanderRadius;
            this.startPoint = enemy.transform.position;
        }

        public override void OnEnter() {
            animator.CrossFade(LocomotionHash, CROSS_FADE_DURATION);
        }
        
        public override void OnUpdate() {
            if (HasReachedDestination()) {
                Vector3 randomDestination = startPoint + (Random.insideUnitSphere * wanderRadius);
                NavMeshHit hit;
                NavMesh.SamplePosition(randomDestination, out hit, wanderRadius, 1);
                Vector3 finalDestination = hit.position;
                agent.SetDestination(finalDestination);
            }
            animator.SetFloat(Speed, agent.velocity.magnitude);
        }

        private bool HasReachedDestination() {
            return !agent.pathPending
                   && agent.remainingDistance <= agent.stoppingDistance
                   && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
        }
    }
}