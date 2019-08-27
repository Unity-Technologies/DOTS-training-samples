using NUnit.Framework;
using Unity.Collections;

namespace Unity.Entities.Tests
{
    /// <summary>
    /// Tests for the stateful <see cref="EntityManagerDiffer"/>
    /// </summary>
    [TestFixture]
    internal sealed class EntityManagerPatcherTests : EntityManagerDiffTestFixture
    {
        [Test]
        public void EntityManagerPatcher_ApplyChanges_NoChanges()
        {
            using (var differ = new EntityManagerDiffer(SrcWorld, Allocator.TempJob))
            using (var patcher = new EntityManagerPatcher(DstWorld, Allocator.TempJob))
            {
                PushChanges(differ, patcher);

                Assert.AreEqual(0, DstEntityManager.Debug.EntityCount);
            }
        }

        [Test]
        public void EntityManagerPatcher_ApplyChanges_CreateEntityWithTestData()
        {
            using (var differ = new EntityManagerDiffer(SrcWorld, Allocator.TempJob))
            using (var patcher = new EntityManagerPatcher(DstWorld, Allocator.TempJob))
            {
                var entity = SrcEntityManager.CreateEntity(typeof(EntityGuid), typeof(EcsTestData));

                var entityGuid = CreateEntityGuid();

                SrcEntityManager.SetComponentData(entity, entityGuid);
                SrcEntityManager.SetComponentData(entity, new EcsTestData {value = 9});

                PushChanges(differ, patcher);

                Assert.AreEqual(1, DstEntityManager.Debug.EntityCount);
                Assert.AreEqual(9, GetComponentData<EcsTestData>(DstEntityManager, entityGuid).value);

                // Mutate some component data.
                SrcEntityManager.SetComponentData(entity, entityGuid);
                SrcEntityManager.SetComponentData(entity, new EcsTestData {value = 10});

                PushChanges(differ, patcher);

                Assert.AreEqual(1, DstEntityManager.Debug.EntityCount);
                Assert.AreEqual(10, GetComponentData<EcsTestData>(DstEntityManager, entityGuid).value);

                // Destroy the entity
                SrcEntityManager.DestroyEntity(entity);

                PushChanges(differ, patcher);

                Assert.AreEqual(0, DstEntityManager.Debug.EntityCount);
            }
        }

        [Test]
        public void EntityManagerPatcher_ApplyChanges_CreateEntityWithPrefabComponent()
        {
            using (var differ = new EntityManagerDiffer(SrcWorld, Allocator.TempJob))
            using (var patcher = new EntityManagerPatcher(DstWorld, Allocator.TempJob))
            {
                var entityGuid = CreateEntityGuid();
                var entity = SrcEntityManager.CreateEntity(typeof(EntityGuid), typeof(Prefab));
                SrcEntityManager.SetComponentData(entity, entityGuid);
                PushChanges(differ, patcher);
                Assert.IsTrue(HasComponent<Prefab>(DstEntityManager, entityGuid));
            }
        }

        [Test]
        public void EntityManagerPatcher_ApplyChanges_CreateEntityWithDisabledComponent()
        {
            using (var differ = new EntityManagerDiffer(SrcWorld, Allocator.TempJob))
            using (var patcher = new EntityManagerPatcher(DstWorld, Allocator.TempJob))
            {
                var entityGuid = CreateEntityGuid();
                var entity = SrcEntityManager.CreateEntity(typeof(EntityGuid), typeof(Disabled));
                SrcEntityManager.SetComponentData(entity, entityGuid);
                PushChanges(differ, patcher);
                Assert.IsTrue(HasComponent<Disabled>(DstEntityManager, entityGuid));
            }
        }

        [Test]
        public void EntityManagerPatcher_ApplyChanges_CreateEntityWithPrefabAndDisabledComponent()
        {
            using (var differ = new EntityManagerDiffer(SrcWorld, Allocator.TempJob))
            using (var patcher = new EntityManagerPatcher(DstWorld, Allocator.TempJob))
            {
                var entityGuid = CreateEntityGuid();
                var entity = SrcEntityManager.CreateEntity(typeof(EntityGuid), typeof(Prefab), typeof(Disabled));
                SrcEntityManager.SetComponentData(entity, entityGuid);
                PushChanges(differ, patcher);
                Assert.IsTrue(HasComponent<Prefab>(DstEntityManager, entityGuid));
                Assert.IsTrue(HasComponent<Disabled>(DstEntityManager, entityGuid));
            }
        }

        [Test]
        public void EntityManagerPatcher_ApplyChanges_RemapEntityReferences()
        {
            using (var differ = new EntityManagerDiffer(SrcWorld, Allocator.TempJob))
            using (var patcher = new EntityManagerPatcher(DstWorld, Allocator.TempJob))
            {
                // Create extra entity to make sure test doesn't accidentally succeed with no remapping
                SrcEntityManager.CreateEntity();

                var entityGuid0 = CreateEntityGuid();
                var entityGuid1 = CreateEntityGuid();

                var e0 = SrcEntityManager.CreateEntity(typeof(EntityGuid), typeof(EcsTestDataEntity));
                var e1 = SrcEntityManager.CreateEntity(typeof(EntityGuid), typeof(EcsTestDataEntity));

                SrcEntityManager.SetComponentData(e0, entityGuid0);
                SrcEntityManager.SetComponentData(e1, entityGuid1);

                SrcEntityManager.SetComponentData(e0, new EcsTestDataEntity {value1 = e1});
                SrcEntityManager.SetComponentData(e1, new EcsTestDataEntity {value1 = e0});

                PushChanges(differ, patcher);

                Assert.AreEqual(GetEntity(DstEntityManager, entityGuid1), GetComponentData<EcsTestDataEntity>(DstEntityManager, entityGuid0).value1);
                Assert.AreEqual(GetEntity(DstEntityManager, entityGuid0), GetComponentData<EcsTestDataEntity>(DstEntityManager, entityGuid1).value1);
            }
        }

        [Test]
        public void EntityManagerPatcher_ApplyChanges_UnidentifiedEntityReferenceBecomesNull()
        {
            using (var differ = new EntityManagerDiffer(SrcWorld, Allocator.TempJob))
            using (var patcher = new EntityManagerPatcher(DstWorld, Allocator.TempJob))
            {
                // Create extra entity to make sure test doesn't accidentally succeed with no remapping
                SrcEntityManager.CreateEntity();

                // Create a standalone entity with no entityGuid. This means the change tracking should NOT resolve it.
                var missing = SrcEntityManager.CreateEntity();

                var entityGuid = CreateEntityGuid();
                var entity = SrcEntityManager.CreateEntity();
                SrcEntityManager.AddComponentData(entity, entityGuid);
                SrcEntityManager.AddComponentData(entity, new EcsTestDataEntity {value1 = missing});

                PushChanges(differ, patcher);

                // Missing entity has no entityGuid, so the reference becomes null.
                Assert.AreEqual(Entity.Null, GetComponentData<EcsTestDataEntity>(DstEntityManager, entityGuid).value1);
            }
        }

        [Test]
        public void EntityManagerPatcher_ApplyChanges_AddComponent()
        {
            using (var differ = new EntityManagerDiffer(SrcWorld, Allocator.TempJob))
            using (var patcher = new EntityManagerPatcher(DstWorld, Allocator.TempJob))
            {
                var entityGuid = CreateEntityGuid();
                var entity = SrcEntityManager.CreateEntity();
                SrcEntityManager.AddComponentData(entity, entityGuid);
                SrcEntityManager.AddComponentData(entity, new EcsTestData {value = 9});

                PushChanges(differ, patcher);

                // Add a component in the source world.
                SrcEntityManager.AddComponentData(entity, new EcsTestData2(10));

                // Mutate the dst world
                SetComponentData(DstEntityManager, entityGuid, new EcsTestData(-1));

                PushChanges(differ, patcher);

                // Both changes should be present in the output
                Assert.AreEqual(10, GetComponentData<EcsTestData2>(DstEntityManager, entityGuid).value0);
                Assert.AreEqual(-1, GetComponentData<EcsTestData>(DstEntityManager, entityGuid).value);
            }
        }

        [Test]
        public void EntityManagerPatcher_ApplyChanges_RemoveComponent()
        {
            using (var differ = new EntityManagerDiffer(SrcWorld, Allocator.TempJob))
            using (var patcher = new EntityManagerPatcher(DstWorld, Allocator.TempJob))
            {
                var entityGuid = CreateEntityGuid();
                var entity = SrcEntityManager.CreateEntity();
                SrcEntityManager.AddComponentData(entity, entityGuid);
                SrcEntityManager.AddComponentData(entity, new EcsTestData {value = 9});
                SrcEntityManager.AddComponentData(entity, new EcsTestData2(7));

                PushChanges(differ, patcher);

                SrcEntityManager.RemoveComponent<EcsTestData>(entity);
                SetComponentData(DstEntityManager, entityGuid, new EcsTestData2(-1));

                PushChanges(differ, patcher);

                Assert.IsFalse(HasComponent<EcsTestData>(DstEntityManager, entityGuid));
                Assert.AreEqual(-1, GetComponentData<EcsTestData2>(DstEntityManager, entityGuid).value0);
            }
        }

#if !NET_DOTS
        [Test]
        public unsafe void EntityManagerPatcher_ApplyChanges_CreateSharedComponent()
        {
            const int count = 3;

            using (var differ = new EntityManagerDiffer(SrcWorld, Allocator.TempJob))
            using (var patcher = new EntityManagerPatcher(DstWorld, Allocator.TempJob))
            {
                var entityGuids = stackalloc EntityGuid[count]
                {
                    CreateEntityGuid(),
                    CreateEntityGuid(),
                    CreateEntityGuid()
                };

                for (var i = 0; i != count; i++)
                {
                    var entity = SrcEntityManager.CreateEntity();
                    SrcEntityManager.AddComponentData(entity, entityGuids[i]);
                    SrcEntityManager.AddSharedComponentData(entity, new EcsTestSharedComp {value = i * 2});
                }

                PushChanges(differ, patcher);

                for (var i = 0; i != count; i++)
                {
                    var sharedData = GetSharedComponentData<EcsTestSharedComp>(DstEntityManager, entityGuids[i]);
                    Assert.AreEqual(i * 2, sharedData.value);
                }
            }
        }

        [Test]
        public void EntityManagerPatcher_ApplyChanges_ChangeSharedComponent()
        {
            using (var differ = new EntityManagerDiffer(SrcWorld, Allocator.TempJob))
            using (var patcher = new EntityManagerPatcher(DstWorld, Allocator.TempJob))
            {
                var entityGuid = CreateEntityGuid();
                var entity = SrcEntityManager.CreateEntity();
                SrcEntityManager.AddComponentData(entity, entityGuid);
                SrcEntityManager.AddComponent<EcsTestSharedComp>(entity);

                PushChanges(differ, patcher);
                var dstEntity = GetEntity(DstEntityManager, entityGuid);
                Assert.AreEqual(0, DstEntityManager.GetSharedComponentDataIndex<EcsTestSharedComp>(dstEntity));
                Assert.AreEqual(0, DstEntityManager.GetSharedComponentData<EcsTestSharedComp>(dstEntity).value);

                SrcEntityManager.SetSharedComponentData(entity, new EcsTestSharedComp {value = 2});
                PushChanges(differ, patcher);
                Assert.AreEqual(2, DstEntityManager.GetSharedComponentData<EcsTestSharedComp>(dstEntity).value);

                SrcEntityManager.SetSharedComponentData(entity, new EcsTestSharedComp {value = 3});
                PushChanges(differ, patcher);
                Assert.AreEqual(3, DstEntityManager.GetSharedComponentData<EcsTestSharedComp>(dstEntity).value);

                SrcEntityManager.SetSharedComponentData(entity, new EcsTestSharedComp {value = 0});
                PushChanges(differ, patcher);
                Assert.AreEqual(0, DstEntityManager.GetSharedComponentDataIndex<EcsTestSharedComp>(dstEntity));
                Assert.AreEqual(0, DstEntityManager.GetSharedComponentData<EcsTestSharedComp>(dstEntity).value);
            }
        }
#endif

        [Test]
        public void EntityManagerPatcher_ApplyChanges_ChangeAppliesToAllPrefabInstances([Values] bool prefabTag)
        {
            using (var differ = new EntityManagerDiffer(SrcWorld, Allocator.TempJob))
            using (var patcher = new EntityManagerPatcher(DstWorld, Allocator.TempJob))
            {
                // Create a prefab in the source world.
                var entityGuid = CreateEntityGuid();
                var prefab = SrcEntityManager.CreateEntity();
                SrcEntityManager.AddComponentData(prefab, entityGuid);
                SrcEntityManager.AddComponentData(prefab, new EcsTestData());

                if (prefabTag)
                {
                    SrcEntityManager.AddComponentData(prefab, new Prefab());
                }

                // Sync to the dst world. At this point the dst world will have a single entity.
                PushChanges(differ, patcher);

                var dstPrefab = GetEntity(DstEntityManager, entityGuid);

                // Spawn some more instances of this thing in the dst world.
                var dstInstance0 = DstEntityManager.Instantiate(dstPrefab);
                var dstInstance1 = DstEntityManager.Instantiate(dstPrefab);

                // Mutate the original prefab in the src world.
                SrcEntityManager.SetComponentData(prefab, new EcsTestData(10));

                // Sync to the dst world.
                PushChanges(differ, patcher);

                // The changes should be propagated to all instances.
                Assert.AreEqual(10, DstEntityManager.GetComponentData<EcsTestData>(dstPrefab).value);
                Assert.AreEqual(10, DstEntityManager.GetComponentData<EcsTestData>(dstInstance0).value);
                Assert.AreEqual(10, DstEntityManager.GetComponentData<EcsTestData>(dstInstance1).value);
            }
        }

        [Test]
        public void EntityManagerPatcher_ApplyChanges_CreateDynamicBuffer([Values(1, 100)] int bufferLength)
        {
            using (var differ = new EntityManagerDiffer(SrcWorld, Allocator.TempJob))
            using (var patcher = new EntityManagerPatcher(DstWorld, Allocator.TempJob))
            {
                var entityGuid = CreateEntityGuid();
                var entity = SrcEntityManager.CreateEntity();

                SrcEntityManager.AddComponentData(entity, entityGuid);
                var buffer = SrcEntityManager.AddBuffer<EcsIntElement>(entity);

                for (var i = 0; i < bufferLength; ++i)
                {
                    buffer.Add(new EcsIntElement {Value = i});
                }

                PushChanges(differ, patcher);

                var dstEntity = GetEntity(DstEntityManager, entityGuid);
                var dstBuffer = DstEntityManager.GetBuffer<EcsIntElement>(dstEntity);

                Assert.AreEqual(bufferLength, dstBuffer.Length);
                for (var i = 0; i != dstBuffer.Length; i++)
                {
                    Assert.AreEqual(i, dstBuffer[i].Value);
                }
            }
        }

#if UNITY_EDITOR
        [Test]
        [TestCase("Manny")]
        [TestCase("Moe")]
        [TestCase("Jack")]
        public void EntityManagerPatcher_ApplyChanges_DebugNames(string srcName)
        {
            using (var differ = new EntityManagerDiffer(SrcWorld, Allocator.TempJob))
            using (var patcher = new EntityManagerPatcher(DstWorld, Allocator.TempJob))
            {
                var entityGuid = CreateEntityGuid();
                var entity = SrcEntityManager.CreateEntity();
                SrcEntityManager.AddComponentData(entity, entityGuid);
                SrcEntityManager.SetName(entity, srcName);

                PushChanges(differ, patcher);

                var dstEntity = GetEntity(DstEntityManager, entityGuid);
                var dstName = DstEntityManager.GetName(dstEntity);

                Assert.AreEqual(srcName, dstName);
            }
        }
#endif

        [Test]
        public void EntityManagerPatcher_ApplyChanges_EntityPatchWithMissingTargetDoesNotThrow()
        {
            using (var differ = new EntityManagerDiffer(SrcWorld, Allocator.TempJob))
            using (var patcher = new EntityManagerPatcher(DstWorld, Allocator.TempJob))
            {
                var entityGuid0 = CreateEntityGuid();
                var entity0 = SrcEntityManager.CreateEntity();
                SrcEntityManager.AddComponentData(entity0, entityGuid0);

                var entityGuid1 = CreateEntityGuid();
                var entity1 = SrcEntityManager.CreateEntity();
                SrcEntityManager.AddComponentData(entity1, entityGuid1);

                PushChanges(differ, patcher);

                // Create a component with an entity reference
                SrcEntityManager.AddComponentData(entity1, new EcsTestDataEntity {value1 = entity0});

                using (var changes = differ.GetChanges(EntityManagerDifferOptions.IncludeForwardChangeSet, Allocator.TempJob))
                {
                    var forward = changes.ForwardChangeSet;

                    Assert.That(forward.EntityPatches.Length, Is.EqualTo(1));

                    // Destroy the entity we should patch
                    SrcEntityManager.DestroyEntity(entity1);

                    Assert.DoesNotThrow(() => { patcher.ApplyChangeSet(forward); });
                }
            }
        }


        [Test]
        public void EntityManagerPatcher_ApplyChanges_EntityPatchWithMissingValueDoesNotThrow()
        {
            using (var differ = new EntityManagerDiffer(SrcWorld, Allocator.TempJob))
            using (var patcher = new EntityManagerPatcher(DstWorld, Allocator.TempJob))
            {
                var entityGuid0 = CreateEntityGuid();
                var entity0 = SrcEntityManager.CreateEntity();
                SrcEntityManager.AddComponentData(entity0, entityGuid0);

                var entityGuid1 = CreateEntityGuid();
                var entity1 = SrcEntityManager.CreateEntity();
                SrcEntityManager.AddComponentData(entity1, entityGuid1);

                PushChanges(differ, patcher);

                // Create a component with an entity reference
                SrcEntityManager.AddComponentData(entity1, new EcsTestDataEntity {value1 = entity0});

                using (var changes = differ.GetChanges(EntityManagerDifferOptions.IncludeForwardChangeSet, Allocator.TempJob))
                {
                    var forward = changes.ForwardChangeSet;

                    Assert.That(forward.EntityPatches.Length, Is.EqualTo(1));

                    // Destroy the entity the patch references
                    SrcEntityManager.DestroyEntity(entity0);

                    Assert.DoesNotThrow(() => { patcher.ApplyChangeSet(forward); });
                }
            }
        }

        [Test]
        public void EntityManagerPatcher_ApplyChanges_EntityPatchWithAmbiguousValueDoesNotThrow()
        {
            using (var differ = new EntityManagerDiffer(SrcWorld, Allocator.TempJob))
            using (var patcher = new EntityManagerPatcher(DstWorld, Allocator.TempJob))
            {
                var entityGuid0 = CreateEntityGuid();
                var entity0 = SrcEntityManager.CreateEntity();
                SrcEntityManager.AddComponentData(entity0, entityGuid0);

                var entityGuid1 = CreateEntityGuid();
                var entity1 = SrcEntityManager.CreateEntity();
                SrcEntityManager.AddComponentData(entity1, entityGuid1);

                // Create a component with an entity reference
                PushChanges(differ, patcher);

                // Create a patch
                SrcEntityManager.AddComponentData(entity1, new EcsTestDataEntity {value1 = entity0});

                using (var changes = differ.GetChanges(EntityManagerDifferOptions.IncludeForwardChangeSet, Allocator.TempJob))
                {
                    var forward = changes.ForwardChangeSet;

                    Assert.That(forward.EntityPatches.Length, Is.EqualTo(1));

                    // Create a new entity in the dst world with the same ID the patch value points to.
                    var dstEntity0 = DstEntityManager.CreateEntity();
                    DstEntityManager.AddComponentData(dstEntity0, entityGuid0);

                    Assert.DoesNotThrow(() => { patcher.ApplyChangeSet(forward); });
                }
            }
        }


        [Test]
        public void EntityManagerPatcher_ApplyChanges_EntityPatchWithAmbiguousTargetDoesNotThrow()
        {
            using (var differ = new EntityManagerDiffer(SrcWorld, Allocator.TempJob))
            using (var patcher = new EntityManagerPatcher(DstWorld, Allocator.TempJob))
            {
                var entityGuid0 = CreateEntityGuid();
                var entity0 = SrcEntityManager.CreateEntity();
                SrcEntityManager.AddComponentData(entity0, entityGuid0);

                var entityGuid1 = CreateEntityGuid();
                var entity1 = SrcEntityManager.CreateEntity();
                SrcEntityManager.AddComponentData(entity1, entityGuid1);

                PushChanges(differ, patcher);

                // Create a patch
                SrcEntityManager.AddComponentData(entity1, new EcsTestDataEntity {value1 = entity0});

                using (var changes = differ.GetChanges(EntityManagerDifferOptions.IncludeForwardChangeSet, Allocator.TempJob))
                {
                    var forward = changes.ForwardChangeSet;

                    Assert.That(forward.EntityPatches.Length, Is.EqualTo(1));

                    // Create a new entity in the dst world with the same ID the patch target.
                    var dstEntity0 = DstEntityManager.CreateEntity();
                    DstEntityManager.AddComponentData(dstEntity0, entityGuid1);

                    Assert.DoesNotThrow(() => { patcher.ApplyChangeSet(forward); });
                }
            }
        }

        [Test]
        public void EntityManagerPatcher_ApplyChanges_NewEntityIsReplicatedIntoExistingPrefabInstances([Values(1, 10)] int instanceCount)
        {
            using (var differ = new EntityManagerDiffer(SrcWorld, Allocator.TempJob))
            using (var patcher = new EntityManagerPatcher(DstWorld, Allocator.TempJob))
            {
                var rootEntityGuid = CreateEntityGuid();
                var childEntityGuid = CreateEntityGuid();
                
                var srcRootEntity = SrcEntityManager.CreateEntity(typeof(EcsTestDataEntity), typeof(Prefab), typeof(LinkedEntityGroup));
                
                SrcEntityManager.AddComponentData(srcRootEntity, rootEntityGuid);
                SrcEntityManager.GetBuffer<LinkedEntityGroup>(srcRootEntity).Add(srcRootEntity);
                    
                PushChanges(differ, patcher);
                
                var dstRootEntity = GetEntity(DstEntityManager, rootEntityGuid);
                
                // Instantiate root in dst world
                var dstRootInstances = new Entity[instanceCount];
                for (var i = 0; i != dstRootInstances.Length; i++)
                {
                    var dstRootInstance = DstEntityManager.Instantiate(dstRootEntity);
                    dstRootInstances[i] = dstRootInstance;
                    Assert.AreEqual(1, DstEntityManager.GetBuffer<LinkedEntityGroup>(dstRootInstance).Length);
                    Assert.AreEqual(dstRootInstance, DstEntityManager.GetBuffer<LinkedEntityGroup>(dstRootInstance)[0].Value);
                }
                
                // Add a new entity into the prefab
                var srcChildEntity = SrcEntityManager.CreateEntity(typeof(EcsTestDataEntity), typeof(Prefab));
                SrcEntityManager.AddComponentData(srcChildEntity, childEntityGuid);
                SrcEntityManager.GetBuffer<LinkedEntityGroup>(srcRootEntity).Add(srcChildEntity);

                SrcEntityManager.SetComponentData(srcRootEntity, new EcsTestDataEntity {value1 = srcChildEntity});
                SrcEntityManager.SetComponentData(srcChildEntity, new EcsTestDataEntity {value1 = srcRootEntity});
                
                PushChanges(differ, patcher);
                
                for (var i = 0; i != dstRootInstances.Length; i++)
                {
                    var dstRootInstance = dstRootInstances[i];

                    var dstInstanceGroup = DstEntityManager.GetBuffer<LinkedEntityGroup>(dstRootInstance);
                    Assert.AreEqual(2, dstInstanceGroup.Length);
                    Assert.AreEqual(dstRootInstance, dstInstanceGroup[0].Value);
                    var dstChildInstance = dstInstanceGroup[1].Value;

                    Assert.IsTrue(DstEntityManager.HasComponent<Prefab>(dstRootEntity));
                    Assert.IsFalse(DstEntityManager.HasComponent<Prefab>(dstRootInstance));
                    Assert.IsFalse(DstEntityManager.HasComponent<Prefab>(dstChildInstance));

                    Assert.AreEqual(dstRootInstance, DstEntityManager.GetComponentData<EcsTestDataEntity>(dstChildInstance).value1);
                    Assert.AreEqual(dstChildInstance, DstEntityManager.GetComponentData<EcsTestDataEntity>(dstRootInstance).value1);
                }
            }
        }

        [Test]
        public void EntityManagerPatcher_ApplyChanges_WithChunkData()
        {
            using (var differ = new EntityManagerDiffer(SrcWorld, Allocator.TempJob))
            using (var patcher = new EntityManagerPatcher(DstWorld, Allocator.TempJob))
            {
                var guid = CreateEntityGuid();
                var entity = SrcEntityManager.CreateEntity();
                Entity dstRootEntity;
                // Chunk component is added but no values are copied
                // Because chunks are generally caches and thus must be rebuildable automatically.
                // They are also likely a totally different set of chunks.
                // Diff & Patch is generally working against entities not on chunk level
                {
                    SrcEntityManager.AddComponentData(entity, guid);
                    SrcEntityManager.AddComponentData(entity, new EcsTestData(1 ));
                    SrcEntityManager.AddChunkComponentData<EcsTestData2>(entity);
                    SrcEntityManager.SetChunkComponentData(SrcEntityManager.GetChunk(entity), new EcsTestData2 (3));

                    PushChanges(differ, patcher);
                    
                    dstRootEntity = GetEntity(DstEntityManager, guid);
                    Assert.AreEqual(1, DstEntityManager.GetComponentData<EcsTestData>(dstRootEntity).value);
                    Assert.IsTrue(DstEntityManager.HasChunkComponent<EcsTestData2>(dstRootEntity));
                    Assert.AreEqual(0, DstEntityManager.GetChunkComponentData<EcsTestData2>(dstRootEntity).value0);
                    Assert.AreEqual(1, DstEntityManager.CreateEntityQuery(typeof(ChunkHeader)).CalculateEntityCount());
                }
                
                // Changing Chunk component creates no diff
                {
                    SrcEntityManager.SetChunkComponentData(SrcEntityManager.GetChunk(entity), new EcsTestData2 (7));
                    using (var changes = differ.GetChanges(EntityManagerDifferOptions.Default, Allocator.Temp))
                    {
                        Assert.IsFalse(changes.AnyChanges);
                    }
                }
                
                // Removing chunk component, removes chunk component again
                {
                    SrcEntityManager.RemoveChunkComponent<EcsTestData2>(entity);
                    PushChanges(differ, patcher);
                    Assert.IsFalse(DstEntityManager.HasChunkComponent<EcsTestData2>(dstRootEntity));
                    Assert.AreEqual(0, DstEntityManager.CreateEntityQuery(typeof(ChunkHeader)).CalculateEntityCount());
                }
            }
        }
    }
}