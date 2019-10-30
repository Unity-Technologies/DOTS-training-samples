using NUnit.Framework;
using Unity.Collections;

namespace Unity.Entities.Tests
{
    public abstract class EntityDifferTestFixture
    {
        ulong m_NextEntityGuidIndex;
        
        /// <summary>
        /// The previous <see cref="World.DefaultGameObjectInjectionWorld"/> to avoid breaking editor static functionality.
        /// </summary>
        World m_PreviousWorld;
        
        /// <summary>
        /// The source world which will be modified by the tests.
        /// </summary>
        protected World SrcWorld;
        
        /// <summary>
        /// The entity manager for the source world.
        /// </summary>
        protected EntityManager SrcEntityManager;
        
        /// <summary>
        /// The destination world which changes can be applied during tests.
        /// </summary>
        protected World DstWorld;
        
        /// <summary>
        /// The entity manager for the destination world.
        /// </summary>
        protected EntityManager DstEntityManager;
        
        [SetUp]
        public virtual void SetUp()
        {
            m_NextEntityGuidIndex = 1;
            m_PreviousWorld = World.DefaultGameObjectInjectionWorld;
            SrcWorld = new World(nameof(EntityDifferTests) + ".Source");
            SrcEntityManager = SrcWorld.EntityManager;
            DstWorld = new World(nameof(EntityPatcherTests) + ".Destination");
            DstEntityManager = DstWorld.EntityManager;
        }

        [TearDown]
        public void TearDown()
        {
            World.DefaultGameObjectInjectionWorld = m_PreviousWorld;
            SrcEntityManager.Debug.CheckInternalConsistency();
            SrcWorld.Dispose();
            DstEntityManager.Debug.CheckInternalConsistency();
            DstWorld.Dispose();
        }

        /// <summary>
        /// Creates a unique <see cref="EntityGuid"/> for the current test scope.
        /// </summary>
        /// <returns></returns>
        protected EntityGuid CreateEntityGuid()
        {
            m_NextEntityGuidIndex++;
            return new EntityGuid {a = m_NextEntityGuidIndex, b = ~m_NextEntityGuidIndex };
        }

        /// <summary>
        /// Pushes forward changes from the tracker to the applier.
        /// </summary>
        protected static void PushChanges(EntityManagerDiffer differ, EntityManager target, bool fastForward = true)
        {
            var options = EntityManagerDifferOptions.IncludeForwardChangeSet;

            if (fastForward)
            {
                options |= EntityManagerDifferOptions.FastForwardShadowWorld;
            }

            using (var changes = differ.GetChanges(options, Allocator.TempJob))
            {
                EntityPatcher.ApplyChangeSet(target, changes.ForwardChangeSet);
            }
        }
        
        protected static bool HasComponent<TComponentData>(EntityManager entityManager, EntityGuid entityGuid) 
            where TComponentData : struct, IComponentData
        {
            return entityManager.HasComponent<TComponentData>(GetEntity(entityManager, entityGuid));
        }

        protected static TComponentData GetComponentData<TComponentData>(EntityManager entityManager, EntityGuid entityGuid) 
            where TComponentData : struct, IComponentData
        {
            return entityManager.GetComponentData<TComponentData>(GetEntity(entityManager, entityGuid));
        }

        protected static void SetComponentData<TComponentData>(EntityManager entityManager, EntityGuid entityGuid, TComponentData data) 
            where TComponentData : struct, IComponentData
        {
            var entity = GetEntity(entityManager, entityGuid);
            entityManager.SetComponentData(entity, data);
        }

        protected static TComponentData GetSharedComponentData<TComponentData>(EntityManager entityManager, EntityGuid entityGuid) 
            where TComponentData : struct, ISharedComponentData
        {
            return entityManager.GetSharedComponentData<TComponentData>(GetEntity(entityManager, entityGuid));
        }

        protected static Entity GetEntity(EntityManager entityManager, EntityGuid entityGuid)
        {
            var entities = entityManager.GetAllEntities();

            foreach (var entity in entities)
            {
                if (entityManager.GetComponentData<EntityGuid>(entity).Equals(entityGuid))
                {
                    return entity;
                }
            }
            
            return Entity.Null;
        }

#if !UNITY_DISABLE_MANAGED_COMPONENTS
        protected static bool HasManagedComponent<TComponentData>(EntityManager entityManager, EntityGuid entityGuid)
            where TComponentData : class, IComponentData
        {
            return entityManager.HasComponent<TComponentData>(GetEntity(entityManager, entityGuid));
        }

        protected static TComponentData GetManagedComponentData<TComponentData>(EntityManager entityManager, EntityGuid entityGuid)
            where TComponentData : class, IComponentData
        {
            return entityManager.GetComponentData<TComponentData>(GetEntity(entityManager, entityGuid));
        }
        protected static void SetManagedComponentData<TComponentData>(EntityManager entityManager, EntityGuid entityGuid, TComponentData data)
            where TComponentData : class, IComponentData
        {
            var entity = GetEntity(entityManager, entityGuid);
            entityManager.SetComponentData(entity, data);
        }
#endif
    }
}
