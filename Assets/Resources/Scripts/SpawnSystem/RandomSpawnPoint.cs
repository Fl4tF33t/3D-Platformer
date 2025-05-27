using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Platformer {
    public class RandomSpawnPoint : ISpawnPointStrategy {
        private List<Transform> unusedSpawnPoints;
        private Transform[] spawnPoints;
        
        public RandomSpawnPoint(Transform[] spawnPoints) { 
            this.spawnPoints = spawnPoints;
            unusedSpawnPoints = new List<Transform>(spawnPoints);
        }
        
        public Transform GetSpawnPosition() {
            if (!unusedSpawnPoints.Any()) {
                unusedSpawnPoints = new List<Transform>(spawnPoints);
            }
            int index = Random.Range(0, unusedSpawnPoints.Count);
            Transform spawnPoint = unusedSpawnPoints[index];
            unusedSpawnPoints.RemoveAt(index);
            return spawnPoint;
        }
        
    }
}