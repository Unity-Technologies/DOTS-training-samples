using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Entities.Tests;

namespace Unity.Entities.PerformanceTests
{
    abstract class EntityDifferPerformanceTestFixture : EntityDifferTestFixture
    {
        /// <summary>
        /// Code to assign mock component based on the entity index.
        /// </summary>
        Dictionary<ComponentType, Action<EntityManager, Entity, int>> m_ComponentDataInitializer;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            
            m_ComponentDataInitializer = new Dictionary<ComponentType, Action<EntityManager, Entity, int>>
            {
                {
                    typeof(EcsTestData), (manager, entity, index) => { manager.SetComponentData(entity, new EcsTestData{value = index * 3}); }
                },
                {
                    typeof(EcsTestData2), (manager, entity, index) => { manager.SetComponentData(entity, new EcsTestData2{value0 = index, value1 = index * 2}); }
                },
                {
                    typeof(EcsTestSharedComp), (manager, entity, index) => { manager.SetSharedComponentData(entity, new EcsTestSharedComp{value = index / 31}); }
                }
            };
        }

        /// <summary>
        /// Creates the given number of entities with non-zero mock data.
        /// </summary>
        protected void CreateEntitiesWithMockComponentData(
            EntityManager entityManager,
            int count, 
            params ComponentType[] components)
        {
            var startIndex = entityManager.Debug.EntityCount;
            
            for (var i = 0; i < count; i++)
            {
                var entity = entityManager.CreateEntity();
                entityManager.AddComponentData(entity, CreateEntityGuid());

                foreach (var component in components)
                {
                    entityManager.AddComponent(entity, component);

                    if (null == m_ComponentDataInitializer)
                    {
                        continue;
                    }
                    
                    if (m_ComponentDataInitializer.TryGetValue(component, out var initializer))
                    {
                        initializer(entityManager, entity, startIndex + i);
                    }
                }
            }
        }
    }
}