namespace Platformer {
    public class EntitySpawner<T> where T : Entity {
        private IEntityFactory<T> factory;
        private ISpawnPointStrategy spawnPointStrategy;
        
        public EntitySpawner(IEntityFactory<T> factory, ISpawnPointStrategy spawnPointStrategy) {
            this.factory = factory;
            this.spawnPointStrategy = spawnPointStrategy;
        }

        public T Spawn() {
            return factory.Create(spawnPointStrategy.GetSpawnPosition());
        }
    }
}