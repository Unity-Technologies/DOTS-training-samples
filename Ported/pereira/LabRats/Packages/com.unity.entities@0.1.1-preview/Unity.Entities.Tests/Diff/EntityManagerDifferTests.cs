using NUnit.Framework;
using Unity.Collections;

namespace Unity.Entities.Tests
{
    /// <summary>
    /// Tests for the stateful <see cref="EntityManagerDiffer"/>
    /// </summary>
    [TestFixture]
    internal sealed class EntityManagerDifferTests : EntityManagerDiffTestFixture
    {
        [Test]
        public void EntityManagerDiffer_GetChanges_NoChanges()
        {
            using (var differ = new EntityManagerDiffer(SrcWorld, Allocator.TempJob))
            {
                using (var changes = differ.GetChanges(EntityManagerDifferOptions.Default, Allocator.Temp))
                {
                    Assert.IsFalse(changes.AnyChanges);
                }
            }
        }

        /// <summary>
        /// Generates a change set over the world and efficiently updates the internal shadow world.
        /// </summary>
        [Test]
        public void EntityManagerDiffer_GetChanges_CreateEntityAndSetComponentData_WithFastForward()
        {
            using (var differ = new EntityManagerDiffer(SrcWorld, Allocator.TempJob))
            {
                var entity = SrcEntityManager.CreateEntity(typeof(EntityGuid), typeof(EcsTestData));

                var entityGuid = CreateEntityGuid();
                
                SrcEntityManager.SetComponentData(entity, entityGuid);
                SrcEntityManager.SetComponentData(entity, new EcsTestData {value = 9});

                const EntityManagerDifferOptions options = EntityManagerDifferOptions.IncludeForwardChangeSet |
                                                           EntityManagerDifferOptions.IncludeReverseChangeSet |
                                                           EntityManagerDifferOptions.FastForwardShadowWorld;
                
                using (var changes = differ.GetChanges(options, Allocator.Temp))
                {
                    // Forward changes is all changes needed to go from the shadow state to the current state.
                    Assert.IsTrue(changes.HasForwardChangeSet);
                    Assert.AreEqual(0, changes.ForwardChangeSet.DestroyedEntityCount);
                    Assert.AreEqual(1, changes.ForwardChangeSet.CreatedEntityCount);
                    Assert.AreEqual(2, changes.ForwardChangeSet.AddComponents.Length);
                    Assert.AreEqual(2, changes.ForwardChangeSet.SetComponents.Length);
                        
                    // Reverse changes is all changes needed to go from the current state back to the last shadow state. (i.e. Undo)
                    Assert.IsTrue(changes.HasReverseChangeSet);
                    Assert.AreEqual(1, changes.ReverseChangeSet.DestroyedEntityCount);
                    Assert.AreEqual(0, changes.ReverseChangeSet.CreatedEntityCount);
                    Assert.AreEqual(0, changes.ReverseChangeSet.AddComponents.Length);
                    Assert.AreEqual(0, changes.ReverseChangeSet.SetComponents.Length);
                }
                
                // The inner shadow world was updated during the last call which means no new changes should be found.
                using (var changes = differ.GetChanges(options, Allocator.Temp))
                {
                    Assert.IsFalse(changes.AnyChanges);
                }
            }
        } 
        
        /// <summary>
        /// Generates a change set over the world without updating the shadow world.
        /// </summary>
        [Test]
        public void EntityManagerDiffer_GetChanges_CreateEntityAndSetComponentData_WithoutFastForward()
        {
            using (var differ = new EntityManagerDiffer(SrcWorld, Allocator.TempJob))
            {
                var entity = SrcEntityManager.CreateEntity(typeof(EntityGuid), typeof(EcsTestData));
                
                SrcEntityManager.SetComponentData(entity, CreateEntityGuid());
                SrcEntityManager.SetComponentData(entity, new EcsTestData {value = 9});

                const EntityManagerDifferOptions options = EntityManagerDifferOptions.IncludeForwardChangeSet |
                                                           EntityManagerDifferOptions.IncludeReverseChangeSet;
                
                using (var changes = differ.GetChanges(options, Allocator.Temp))
                {
                    // ForwardChanges defines all operations needed to go from the shadow state to the current state.
                    Assert.IsTrue(changes.HasForwardChangeSet);
                    Assert.AreEqual(0, changes.ForwardChangeSet.DestroyedEntityCount);
                    Assert.AreEqual(1, changes.ForwardChangeSet.CreatedEntityCount);
                    Assert.AreEqual(2, changes.ForwardChangeSet.AddComponents.Length);
                    Assert.AreEqual(2, changes.ForwardChangeSet.SetComponents.Length);
                        
                    // ReverseChanges defines all operations needed to go from the current state back to the last shadow state. (i.e. Undo)
                    Assert.IsTrue(changes.HasReverseChangeSet);
                    Assert.AreEqual(1, changes.ReverseChangeSet.DestroyedEntityCount);
                    Assert.AreEqual(0, changes.ReverseChangeSet.CreatedEntityCount);
                    Assert.AreEqual(0, changes.ReverseChangeSet.AddComponents.Length);
                    Assert.AreEqual(0, changes.ReverseChangeSet.SetComponents.Length);
                }
                
                // Since we did not fast forward the inner shadow world. We should be able to generate the exact same changes again.
                using (var changes = differ.GetChanges(options, Allocator.Temp))
                {
                    Assert.IsTrue(changes.HasForwardChangeSet);
                    Assert.AreEqual(0, changes.ForwardChangeSet.DestroyedEntityCount);
                    Assert.AreEqual(1, changes.ForwardChangeSet.CreatedEntityCount);
                    Assert.AreEqual(2, changes.ForwardChangeSet.AddComponents.Length);
                    Assert.AreEqual(2, changes.ForwardChangeSet.SetComponents.Length);
                        
                    Assert.IsTrue(changes.HasReverseChangeSet);
                    Assert.AreEqual(1, changes.ReverseChangeSet.DestroyedEntityCount);
                    Assert.AreEqual(0, changes.ReverseChangeSet.CreatedEntityCount);
                    Assert.AreEqual(0, changes.ReverseChangeSet.AddComponents.Length);
                    Assert.AreEqual(0, changes.ReverseChangeSet.SetComponents.Length);
                }
            }
        }

        [Test]
        public void EntityManagerDiffer_GetChanges_CreateEntityAndSetComponentData_IncrementalChanges()
        {
            using (var tracker = new EntityManagerDiffer(SrcWorld, Allocator.TempJob))
            {
                var entityGuid = CreateEntityGuid();
                var entity = SrcEntityManager.CreateEntity(typeof(EntityGuid), typeof(EcsTestData));
                
                SrcEntityManager.SetComponentData(entity, entityGuid);
                SrcEntityManager.SetComponentData(entity, new EcsTestData {value = 9});
                
                using (var changes = tracker.GetChanges(EntityManagerDifferOptions.Default, Allocator.Temp))
                {
                    Assert.IsTrue(changes.AnyChanges);
                }
                
                // Mutate some component data.
                SrcEntityManager.SetComponentData(entity, entityGuid);
                SrcEntityManager.SetComponentData(entity, new EcsTestData {value = 10});
                
                // The entityGuid value is the same so it should not be picked up during change tracking.
                // We should only see the one data change.
                using (var changes = tracker.GetChanges(EntityManagerDifferOptions.Default, Allocator.Temp))
                {
                    // The ForwardChangeSet will contain a set value 10
                    Assert.IsTrue(changes.HasForwardChangeSet);
                    Assert.AreEqual(0, changes.ForwardChangeSet.DestroyedEntityCount);
                    Assert.AreEqual(0, changes.ForwardChangeSet.CreatedEntityCount);
                    Assert.AreEqual(1, changes.ForwardChangeSet.SetComponents.Length);
                    Assert.AreEqual(0, changes.ForwardChangeSet.AddComponents.Length);
                    
                    // The ReverseChangeSet will contain a set value 9
                    Assert.IsTrue(changes.HasReverseChangeSet);
                    Assert.AreEqual(0, changes.ReverseChangeSet.DestroyedEntityCount);
                    Assert.AreEqual(0, changes.ReverseChangeSet.CreatedEntityCount);
                    Assert.AreEqual(1, changes.ReverseChangeSet.SetComponents.Length);
                    Assert.AreEqual(0, changes.ReverseChangeSet.AddComponents.Length);
                }
                
                SrcEntityManager.DestroyEntity(entity);

                using (var changes = tracker.GetChanges(EntityManagerDifferOptions.Default, Allocator.Temp))
                {
                    Assert.IsTrue(changes.HasForwardChangeSet);
                    Assert.AreEqual(1, changes.ForwardChangeSet.DestroyedEntityCount);
                    Assert.AreEqual(0, changes.ForwardChangeSet.CreatedEntityCount);
                    Assert.AreEqual(0, changes.ForwardChangeSet.SetComponents.Length);
                    Assert.AreEqual(0, changes.ForwardChangeSet.RemoveComponents.Length);
                        
                    // In this case the ReverseChangeSet should describe how to get this entity back in it's entirety
                    Assert.IsTrue(changes.HasReverseChangeSet);
                    Assert.AreEqual(0, changes.ReverseChangeSet.DestroyedEntityCount);
                    Assert.AreEqual(1, changes.ReverseChangeSet.CreatedEntityCount);
                    Assert.AreEqual(2, changes.ReverseChangeSet.AddComponents.Length);
                    Assert.AreEqual(2, changes.ReverseChangeSet.SetComponents.Length);
                }
            }
        }
            
        [Test]
        public void EntityManagerDiffer_GetChanges_CreateEntityAndSetSharedComponentData_IncrementalChanges()
        {
            using (var tracker = new EntityManagerDiffer(SrcWorld, Allocator.TempJob))
            {
                var entity = SrcEntityManager.CreateEntity(typeof(EntityGuid), typeof(EcsTestSharedComp));
                
                SrcEntityManager.SetComponentData(entity, CreateEntityGuid());
                SrcEntityManager.SetSharedComponentData(entity, new EcsTestSharedComp {value = 2});

                using (var changes = tracker.GetChanges(EntityManagerDifferOptions.Default, Allocator.Temp))
                {
                    Assert.IsTrue(changes.HasForwardChangeSet);
                    Assert.AreEqual(1, changes.ForwardChangeSet.CreatedEntityCount);
                    Assert.AreEqual(2, changes.ForwardChangeSet.AddComponents.Length);
                    Assert.AreEqual(1, changes.ForwardChangeSet.SetComponents.Length);
                    Assert.AreEqual(1, changes.ForwardChangeSet.SetSharedComponents.Length);
                }
            }
        }
            
        [Test]
        public void EntityManagerDiffer_GetChanges_DuplicateEntityGuidThrows()
        {
            using (var tracker = new EntityManagerDiffer(SrcWorld, Allocator.TempJob))
            {
                var entityGuid = CreateEntityGuid();
                
                var entity0 = SrcEntityManager.CreateEntity(typeof(EntityGuid));
                var entity1 = SrcEntityManager.CreateEntity(typeof(EntityGuid));
                
                SrcEntityManager.SetComponentData(entity0, entityGuid);
                SrcEntityManager.SetComponentData(entity1, entityGuid);

                Assert.Throws<DuplicateEntityGuidException>(() =>
                {
                    using (tracker.GetChanges(EntityManagerDifferOptions.Default, Allocator.Temp))
                    {
                    }
                });
            }
        }
        
        
        [Test]
        public void EntityManagerDiffer_GetChanges_IgnoresSystemState()
        {
            using (var tracker = new EntityManagerDiffer(SrcWorld, Allocator.TempJob))
            {
                var entityGuid = CreateEntityGuid();
                var entity = SrcEntityManager.CreateEntity();
                SrcEntityManager.AddComponentData(entity, entityGuid);
                
                using (var changes = tracker.GetChanges(EntityManagerDifferOptions.Default, Allocator.Temp)) {}

                SrcEntityManager.AddComponentData(entity, new EcsState1 {Value = 9});
                using (var changes = tracker.GetChanges(EntityManagerDifferOptions.Default, Allocator.Temp))
                {
                    Assert.IsFalse(changes.AnyChanges);
                }

                SrcEntityManager.SetComponentData(entity, new EcsState1 {Value = 10});
                using (var changes = tracker.GetChanges(EntityManagerDifferOptions.Default, Allocator.Temp))
                {
                    Assert.IsFalse(changes.AnyChanges);
                }
                
                // NOTE: the system state component being copied to shadow world is not required by the public API.
                //       This is simply defining the expected internal behaviour.
                Assert.AreEqual(10, tracker.ShadowEntityManager.GetComponentData<EcsState1>(entity).Value);
            }
        }
        
        
        [Test]
        public void EntityManagerDiffer_AddingZeroSizeComponentToWholeChunk()
        {
            using (var tracker = new EntityManagerDiffer(SrcWorld, Allocator.TempJob))
            {
                for (int i = 0; i != 10; i++)
                {
                    var entityGuid = CreateEntityGuid();
                    var entity = SrcEntityManager.CreateEntity();
                    SrcEntityManager.AddComponentData(entity, entityGuid);
                }
                
                using (var changes = tracker.GetChanges(EntityManagerDifferOptions.Default, Allocator.Temp)) {}

                SrcEntityManager.AddSharedComponentData(SrcEntityManager.UniversalQuery, new SharedData1(9));
                
                using (var changes = tracker.GetChanges(EntityManagerDifferOptions.Default, Allocator.Temp))
                {
                    Assert.IsTrue(changes.AnyChanges);
                    Assert.AreEqual(10, changes.ForwardChangeSet.AddComponents.Length);
                }
            }
        }
    }
}