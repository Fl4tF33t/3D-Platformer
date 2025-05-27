using UnityEngine;

namespace Platformer {
    public class LinearSpawnPoint : ISpawnPointStrategy {
        private int index = 0;
        private Transform[] spawnPoints;
        
        public LinearSpawnPoint(Transform[] spawnPoints) => this.spawnPoints = spawnPoints;
        
        public Transform GetSpawnPosition() {
            Transform spawnPoint = spawnPoints[index];
            index = (index + 1) % spawnPoints.Length;
            return spawnPoint;
        }
    }
}