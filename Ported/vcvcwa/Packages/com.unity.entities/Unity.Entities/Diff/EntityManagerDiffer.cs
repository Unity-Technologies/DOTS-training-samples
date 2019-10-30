using System;
using Unity.Collections;

namespace Unity.Entities
{
    /// <summary>
    /// The <see cref="EntityManagerDiffer"/> is used to efficiently track changes to a given world over time.
    /// </summary>
    public struct EntityManagerDiffer : IDisposable
    {
        static EntityQueryDesc EntityGuidQueryDesc { get; } = new EntityQueryDesc
        {
            All = new ComponentType[]
            {
                typeof(EntityGuid)
            },
            Options = EntityQueryOptions.IncludeDisabled | EntityQueryOptions.IncludePrefab
        };
        
        World m_ShadowWorld;
        
        EntityManager m_SourceEntityManager;
        EntityManager m_ShadowEntityManager;
        EntityQueryDesc m_EntityQueryDesc;
        BlobAssetCache m_BlobAssetCache;

        internal EntityManager ShadowEntityManager => m_ShadowEntityManager;

        public EntityManagerDiffer(EntityManager sourceEntityManager, Allocator allocator, EntityQueryDesc entityQueryDesc = null)
        {
            m_SourceEntityManager = sourceEntityManager ?? throw new ArgumentNullException(nameof(sourceEntityManager));
            m_EntityQueryDesc = entityQueryDesc ?? EntityGuidQueryDesc;
            m_ShadowWorld = new World(sourceEntityManager.World.Name + " (Shadow)");
            m_ShadowEntityManager = m_ShadowWorld.EntityManager;
            m_BlobAssetCache = new BlobAssetCache(allocator);
        }

        public void Dispose()
        {
            m_SourceEntityManager = null;
            
            if (m_ShadowWorld != null && m_ShadowWorld.IsCreated)
                m_ShadowWorld.Dispose();
            
            m_BlobAssetCache.Dispose();
            m_ShadowWorld = null;
            m_ShadowEntityManager = null;
            m_EntityQueryDesc = null;
        }

        /// <summary>
        /// Generates a detailed change set for the world.
        /// All entities to be considered for diffing must have the <see cref="EntityGuid"/> component with a unique value.
        /// </summary>
        /// <remarks>
        /// The resulting <see cref="EntityChanges"/> must be disposed when no longer needed.
        /// </remarks>
        /// <param name="options">A set of options which can be toggled.</param>
        /// <param name="allocator">The allocator to use for the results object.</param>
        /// <returns>A set of changes for the world since the last fast-forward.</returns>
        public EntityChanges GetChanges(EntityManagerDifferOptions options, Allocator allocator)
        {
            #if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (null == m_SourceEntityManager || null == m_ShadowEntityManager)
                throw new ArgumentException($"The {nameof(EntityManagerDiffer)} has already been Disposed.");
            #endif
            
            var changes = EntityDiffer.GetChanges(
                srcEntityManager: m_SourceEntityManager,
                dstEntityManager: m_ShadowEntityManager,
                options,
                m_EntityQueryDesc,
                m_BlobAssetCache,
                allocator);

            return changes;
        }
    }
}