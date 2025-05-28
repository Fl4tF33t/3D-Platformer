using System;
using UnityEngine;
using Utilities;

namespace Platformer {
    public class PlayerDetector : MonoBehaviour {
        [SerializeField] private float detectionAngle = 60f;
        [SerializeField] private float detectionRadius = 8f;
        [SerializeField] private float innerDetectionRadius = 2f;
        [SerializeField] private float detectionCoolDown = 1f;
        [SerializeField] private float attackRange = 1.5f;
        
        private CountdownTimer detectionTimer;
        public Transform Player { get; private set; }
        
        private IDetectionStrategy detectionStrategy;

        private void Start() {
            detectionTimer = new CountdownTimer(detectionCoolDown);
            Player = GameObject.FindGameObjectWithTag("Player").transform;
            detectionStrategy = new ConeDetectionStrategy(detectionAngle, detectionRadius, innerDetectionRadius);
        }

        private void Update() => detectionTimer.Tick(Time.deltaTime);
        public bool CanDetectPlayer() => detectionTimer.IsRunning || detectionStrategy.Execute(Player, transform, detectionTimer);
        public bool CanAttackPlayer() {
            Vector3 directionToPlayer = Player.position - transform.position;
            return directionToPlayer.magnitude < attackRange;
        }
        public void SetDetectionStrategy(IDetectionStrategy strategy) => detectionStrategy = strategy;
        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
            Gizmos.DrawWireSphere(transform.position, innerDetectionRadius);
            
            Vector3 forwardConeDirection = Quaternion.Euler(0, detectionAngle / 2, 0) * transform.forward * detectionRadius;
            Vector3 backwardConeDirection = Quaternion.Euler(0, -detectionAngle / 2, 0) * transform.forward * detectionRadius;
            
            Gizmos.DrawLine(transform.position, transform.position + forwardConeDirection);
            Gizmos.DrawLine(transform.position, transform.position + backwardConeDirection);
        }
    }
}