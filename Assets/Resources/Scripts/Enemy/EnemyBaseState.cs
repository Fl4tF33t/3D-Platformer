using UnityEngine;
using UnityEngine.AI;

namespace Platformer {
    public abstract class EnemyBaseState : IState {
        protected readonly Enemy enemy;
        protected readonly NavMeshAgent agent;
        protected readonly Animator animator;
        
        protected static readonly int LocomotionHash = Animator.StringToHash("Locomotion");
        protected static readonly int Speed = Animator.StringToHash("Speed");
        protected static readonly int AttackHash = Animator.StringToHash("Attack");
        
        protected const float CROSS_FADE_DURATION = 0.2f;

        protected EnemyBaseState(Enemy enemy, NavMeshAgent agent, Animator animator) {
            this.enemy = enemy;
            this.agent = agent;
            this.animator = animator;
        }
        public virtual void OnEnter() {
        }
        public virtual void OnUpdate() {
        }
        public virtual void OnFixedUpdate() {
        }
        public virtual void OnExit() {
        }
    }
}