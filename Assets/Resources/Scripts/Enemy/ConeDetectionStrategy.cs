using UnityEngine;
using Utilities;

namespace Platformer {
    public class ConeDetectionStrategy : IDetectionStrategy {
        private readonly float detectionAngle;
        private readonly float detectionRadius;
        private readonly float innerDetectionRadius;
        
        public ConeDetectionStrategy(float detectionAngle, float detectionRadius, float innerDetectionRadius) {
            this.detectionAngle = detectionAngle;
            this.detectionRadius = detectionRadius;
            this.innerDetectionRadius = innerDetectionRadius;
        }
        public bool Execute(Transform player, Transform detector, CountdownTimer timer) {
            if (timer.IsRunning) return false;
            
            Vector3 directionToPlayer = player.position - detector.position;
            float angleToPlayer = Vector3.Angle(directionToPlayer, Vector3.forward);

            if ((!(angleToPlayer < detectionAngle / 2f) || !(directionToPlayer.magnitude < detectionRadius)) 
                && !(directionToPlayer.magnitude < innerDetectionRadius)) {
                return false;
            }

            timer.Start();
            return true;
        }
    }
}