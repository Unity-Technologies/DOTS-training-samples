using Unity.Collections;

namespace Unity.Entities
{
    /// <summary>
    /// Internal structure to pass params easier.
    ///
    /// This structure will hold all the relevant information for a world to compare.
    /// </summary>
    internal readonly unsafe struct WorldState
    {
        public readonly EntityManager EntityManager;
        
        public EntityComponentStore* EntityComponentStore => EntityManager.EntityComponentStore;
        public ManagedComponentStore ManagedComponentStore => EntityManager.ManagedComponentStore;
        
        /// <summary>
        /// A set of all entities in the world to consider for the diff.
        ///
        /// @NOTE This only includes entities that have `potentially` changed.
        /// </summary>
        public readonly NativeArray<EntityInChunkWithComponent<EntityGuid>> Entities;

        public WorldState(
            EntityManager entityManager, 
            NativeArray<EntityInChunkWithComponent<EntityGuid>> entities)
        {
            EntityManager = entityManager;
            Entities = entities;
        }
    }
}