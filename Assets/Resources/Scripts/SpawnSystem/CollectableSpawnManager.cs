using System;
using UnityEngine;
using Utilities;

namespace Platformer {
    public class CollectableSpawnManager : EntitySpawnManager {
        [SerializeField] private CollectableData[] collectableData;
        [SerializeField] private float spawnInterval = 1f;

        private EntitySpawner<Collectable> spawner;
        private CountdownTimer spawnTimer;
        private int counter;
        protected override void Awake() {
            base.Awake();

            spawner = new EntitySpawner<Collectable>(
                new EntityFactory<Collectable>(collectableData),
                spawnPointStrategy);
            
            spawnTimer = new CountdownTimer(spawnInterval);
            spawnTimer.OnTimerStop += () => {
                if (counter++ >= spawnPoints.Length) {
                    spawnTimer.Stop();
                    return;
                }
                Spawn();
                spawnTimer.Start();           
            };
        }

        private void Start() {
            spawnTimer.Start();       
        }

        private void Update() {
            spawnTimer.Tick(Time.deltaTime);
        }

        public override void Spawn() {
            spawner.Spawn();
        }
    }
}