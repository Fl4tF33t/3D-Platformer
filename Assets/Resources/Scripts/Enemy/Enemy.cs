using System;
using UnityEngine;
using UnityEngine.AI;
using Utilities;

namespace Platformer
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(PlayerDetector))]
    public class Enemy : Entity {
        [SerializeField] Animator animator;
        
        [Header("Wander Settings")]
        [SerializeField] private float wanderRadius = 10f;

        [Header("AttackSettings")] 
        [SerializeField] private float attackRate = 1f;
        
        private NavMeshAgent agent;
        private StateMachine stateMachine;
        private PlayerDetector playerDetector;

        private CountdownTimer attackTimer;
        private void Awake() {
            agent = GetComponent<NavMeshAgent>();
            playerDetector = GetComponent<PlayerDetector>();
        }

        private void Start() {
            attackTimer = new CountdownTimer(attackRate);
            
            stateMachine = new StateMachine();
            
            EnemyWanderState wanderState = new EnemyWanderState(this, agent, animator, wanderRadius);
            EnemyChaseState chaseState = new EnemyChaseState(this, agent, animator, playerDetector.Player);
            EnemyAttackState attackState = new EnemyAttackState(this, agent, animator, playerDetector.Player);

            At(wanderState, chaseState, new FuncPredicate(() => playerDetector.CanDetectPlayer()));
            At(chaseState, wanderState, new FuncPredicate(() => !playerDetector.CanDetectPlayer()));
            At(chaseState, attackState, new FuncPredicate(() => playerDetector.CanAttackPlayer()));
            At(attackState, chaseState, new FuncPredicate(() => !playerDetector.CanAttackPlayer()));
            
            stateMachine.SetState(wanderState);
        }
        
        private void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        private void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);
        
        private void Update() {
            stateMachine.Update();
            attackTimer.Tick(Time.deltaTime);
        }

        private void FixedUpdate() {
            stateMachine.FixedUpdate();
        }

        public void Attack() {
            if (attackTimer.IsRunning) return;
            attackTimer.Start();
        }
    }
}
