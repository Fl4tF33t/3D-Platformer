using UnityEngine;

namespace Platformer {
    public abstract class EntitySpawnManager : MonoBehaviour {
        [SerializeField] protected SpawnPointStrategyType spawnPointStrategyType = SpawnPointStrategyType.Linear;
        [SerializeField] protected Transform[] spawnPoints;
        protected ISpawnPointStrategy spawnPointStrategy;
        public enum SpawnPointStrategyType {
            Linear,
            Random
        }
        
        protected virtual void Awake() {
            spawnPointStrategy = spawnPointStrategyType switch {
                SpawnPointStrategyType.Linear => new LinearSpawnPoint(spawnPoints),
                SpawnPointStrategyType.Random => new RandomSpawnPoint(spawnPoints),
                _ => throw new System.Exception("Invalid SpawnPointStrategyType")
            };
        }

        public abstract void Spawn();
    }
}