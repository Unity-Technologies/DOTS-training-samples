using NUnit.Framework;
using Unity.Collections;

namespace Unity.Entities.Tests
{
    [TestFixture]
    sealed class EntityPatcherTests : EntityDifferTestFixture
    {
        [Test]
        public void EntityPatcher_ApplyChanges_NoChanges()
        {
            using (var differ = new EntityManagerDiffer(SrcEntityManager, Allocator.TempJob))
            {
                PushChanges(differ, DstEntityManager);

                Assert.AreEqual(0, DstEntityManager.Debug.EntityCount);
            }
        }

        [Test]
        public void EntityPatcher_ApplyChanges_CreateEntityWithTestData()
        {
            using (var differ = new EntityManagerDiffer(SrcEntityManager, Allocator.TempJob))
            {
                var entity = SrcEntityManager.CreateEntity(typeof(EntityGuid), typeof(EcsTestData));

                var entityGuid = CreateEntityGuid();

                SrcEntityManager.SetComponentData(entity, entityGuid);
                SrcEntityManager.SetComponentData(entity, new EcsTestData { value = 9 });

                PushChanges(differ, DstEntityManager);

                Assert.AreEqual(1, DstEntityManager.Debug.EntityCount);
                Assert.AreEqual(9, GetComponentData<EcsTestData>(DstEntityManager, entityGuid).value);

                // Mutate some component data.
                SrcEntityManager.SetComponentData(entity, entityGuid);
                SrcEntityManager.SetComponentData(entity, new EcsTestData { value = 10 });

                PushChanges(differ, DstEntityManager);

                Assert.AreEqual(1, DstEntityManager.Debug.EntityCount);
                Assert.AreEqual(10, GetComponentData<EcsTestData>(DstEntityManager, entityGuid).value);

                // Destroy the entity
                SrcEntityManager.DestroyEntity(entity);
                PushChanges(differ, DstEntityManager);
                Assert.AreEqual(0, DstEntityManager.Debug.EntityCount);
            }
        }

        [Test]
        public void EntityPatcher_ApplyChanges_CreateEntityWithPrefabComponent()
        {
            using (var differ = new EntityManagerDiffer(SrcEntityManager, Allocator.TempJob))
            {
                var entityGuid = CreateEntityGuid();
                var entity = SrcEntityManager.CreateEntity(typeof(EntityGuid), typeof(Prefab));
                SrcEntityManager.SetComponentData(entity, entityGuid);
                PushChanges(differ, DstEntityManager);
                Assert.IsTrue(HasComponent<Prefab>(DstEntityManager, entityGuid));
            }
        }

        [Test]
        public void EntityPatcher_ApplyChanges_CreateEntityWithDisabledComponent()
        {
            using (var differ = new EntityManagerDiffer(SrcEntityManager, Allocator.TempJob))
            {
                var entityGuid = CreateEntityGuid();
                var entity = SrcEntityManager.CreateEntity(typeof(EntityGuid), typeof(Disabled));
                SrcEntityManager.SetComponentData(entity, entityGuid);
                PushChanges(differ, DstEntityManager);
                Assert.IsTrue(HasComponent<Disabled>(DstEntityManager, entityGuid));
            }
        }

        [Test]
        public void EntityPatcher_ApplyChanges_CreateEntityWithPrefabAndDisabledComponent()
        {
            using (var differ = new EntityManagerDiffer(SrcEntityManager, Allocator.TempJob))
            {
                var entityGuid = CreateEntityGuid();
                var entity = SrcEntityManager.CreateEntity(typeof(EntityGuid), typeof(Prefab), typeof(Disabled));
                SrcEntityManager.SetComponentData(entity, entityGuid);
                PushChanges(differ, DstEntityManager);
                Assert.IsTrue(HasComponent<Prefab>(DstEntityManager, entityGuid));
                Assert.IsTrue(HasComponent<Disabled>(DstEntityManager, entityGuid));
            }
        }

        [Test]
        public void EntityPatcher_ApplyChanges_RemapEntityReferences()
        {
            using (var differ = new EntityManagerDiffer(SrcEntityManager, Allocator.TempJob))
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

                PushChanges(differ, DstEntityManager);

                Assert.AreEqual(GetEntity(DstEntityManager, entityGuid1), GetComponentData<EcsTestDataEntity>(DstEntityManager, entityGuid0).value1);
                Assert.AreEqual(GetEntity(DstEntityManager, entityGuid0), GetComponentData<EcsTestDataEntity>(DstEntityManager, entityGuid1).value1);
            }
        }

        [Test]
        [Ignore("Not currently supported")]
        public void EntityPatcher_ApplyChanges_RemapEntityReferencesInSharedComponents()
        {
            using (var differ = new EntityManagerDiffer(SrcEntityManager, Allocator.TempJob))
            {
                // Create extra entity to make sure test doesn't accidentally succeed with no remapping
                SrcEntityManager.CreateEntity();

                var entityGuid0 = CreateEntityGuid();
                var entityGuid1 = CreateEntityGuid();

                var e0 = SrcEntityManager.CreateEntity(typeof(EntityGuid), typeof(EcsTestSharedCompEntity));
                var e1 = SrcEntityManager.CreateEntity(typeof(EntityGuid), typeof(EcsTestSharedCompEntity));

                SrcEntityManager.SetComponentData(e0, entityGuid0);
                SrcEntityManager.SetComponentData(e1, entityGuid1);

                SrcEntityManager.SetSharedComponentData(e0, new EcsTestSharedCompEntity { value = e1 });
                SrcEntityManager.SetSharedComponentData(e1, new EcsTestSharedCompEntity { value = e0 });

                PushChanges(differ, DstEntityManager);

                Assert.AreEqual(GetEntity(DstEntityManager, entityGuid1), GetSharedComponentData<EcsTestSharedCompEntity>(DstEntityManager, entityGuid0).value);
                Assert.AreEqual(GetEntity(DstEntityManager, entityGuid0), GetSharedComponentData<EcsTestSharedCompEntity>(DstEntityManager, entityGuid1).value);
            }
        }

        [Test]
        public void EntityPatcher_ApplyChanges_UnidentifiedEntityReferenceBecomesNull()
        {
            using (var differ = new EntityManagerDiffer(SrcEntityManager, Allocator.TempJob))
            {
                // Create extra entity to make sure test doesn't accidentally succeed with no remapping
                SrcEntityManager.CreateEntity();

                // Create a standalone entity with no entityGuid. This means the change tracking should NOT resolve it.
                var missing = SrcEntityManager.CreateEntity();

                var entityGuid = CreateEntityGuid();
                var entity = SrcEntityManager.CreateEntity();
                SrcEntityManager.AddComponentData(entity, entityGuid);
                SrcEntityManager.AddComponentData(entity, new EcsTestDataEntity {value1 = missing});

                PushChanges(differ, DstEntityManager);

                // Missing entity has no entityGuid, so the reference becomes null.
                Assert.AreEqual(Entity.Null, GetComponentData<EcsTestDataEntity>(DstEntityManager, entityGuid).value1);
            }
        }

        [Test]
        public void EntityPatcher_ApplyChanges_AddComponent()
        {
            using (var differ = new EntityManagerDiffer(SrcEntityManager, Allocator.TempJob))
            {
                var entityGuid = CreateEntityGuid();
                var entity = SrcEntityManager.CreateEntity();
                SrcEntityManager.AddComponentData(entity, entityGuid);
                SrcEntityManager.AddComponentData(entity, new EcsTestData { value = 9 });

                PushChanges(differ, DstEntityManager);

                // Add a component in the source world.
                SrcEntityManager.AddComponentData(entity, new EcsTestData2(10));

                // Mutate the dst world
                SetComponentData(DstEntityManager, entityGuid, new EcsTestData(-1));

                PushChanges(differ, DstEntityManager);

                // Both changes should be present in the output
                Assert.AreEqual(10, GetComponentData<EcsTestData2>(DstEntityManager, entityGuid).value0);
                Assert.AreEqual(-1, GetComponentData<EcsTestData>(DstEntityManager, entityGuid).value);
            }
        }

        [Test]
        public void EntityPatcher_ApplyChanges_RemoveComponent()
        {
            using (var differ = new EntityManagerDiffer(SrcEntityManager, Allocator.TempJob))
            {
                var entityGuid = CreateEntityGuid();
                var entity = SrcEntityManager.CreateEntity();
                SrcEntityManager.AddComponentData(entity, entityGuid);
                SrcEntityManager.AddComponentData(entity, new EcsTestData { value = 9 });
                SrcEntityManager.AddComponentData(entity, new EcsTestData2(7));

                PushChanges(differ, DstEntityManager);

                SrcEntityManager.RemoveComponent<EcsTestData>(entity);
                SetComponentData(DstEntityManager, entityGuid, new EcsTestData2(-1));

                PushChanges(differ, DstEntityManager);

                Assert.IsFalse(HasComponent<EcsTestData>(DstEntityManager, entityGuid));
                Assert.AreEqual(-1, GetComponentData<EcsTestData2>(DstEntityManager, entityGuid).value0);
            }
        }

#if !NET_DOTS
        [Test]
        public unsafe void EntityPatcher_ApplyChanges_CreateSharedComponent()
        {
            const int count = 3;

            using (var differ = new EntityManagerDiffer(SrcEntityManager, Allocator.TempJob))
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

                PushChanges(differ, DstEntityManager);

                for (var i = 0; i != count; i++)
                {
                    var sharedData = GetSharedComponentData<EcsTestSharedComp>(DstEntityManager, entityGuids[i]);
                    Assert.AreEqual(i * 2, sharedData.value);
                }
            }
        }

        [Test]
        public void EntityPatcher_ApplyChanges_ChangeSharedComponent()
        {
            using (var differ = new EntityManagerDiffer(SrcEntityManager, Allocator.TempJob))
            {
                var entityGuid = CreateEntityGuid();
                var entity = SrcEntityManager.CreateEntity();
                SrcEntityManager.AddComponentData(entity, entityGuid);
                SrcEntityManager.AddComponent<EcsTestSharedComp>(entity);

                PushChanges(differ, DstEntityManager);
                var dstEntity = GetEntity(DstEntityManager, entityGuid);
                Assert.AreEqual(0, DstEntityManager.GetSharedComponentDataIndex<EcsTestSharedComp>(dstEntity));
                Assert.AreEqual(0, DstEntityManager.GetSharedComponentData<EcsTestSharedComp>(dstEntity).value);

                SrcEntityManager.SetSharedComponentData(entity, new EcsTestSharedComp {value = 2});
                PushChanges(differ, DstEntityManager);
                Assert.AreEqual(2, DstEntityManager.GetSharedComponentData<EcsTestSharedComp>(dstEntity).value);

                SrcEntityManager.SetSharedComponentData(entity, new EcsTestSharedComp {value = 3});
                PushChanges(differ, DstEntityManager);
                Assert.AreEqual(3, DstEntityManager.GetSharedComponentData<EcsTestSharedComp>(dstEntity).value);

                SrcEntityManager.SetSharedComponentData(entity, new EcsTestSharedComp {value = 0});
                PushChanges(differ, DstEntityManager);
                Assert.AreEqual(0, DstEntityManager.GetSharedComponentDataIndex<EcsTestSharedComp>(dstEntity));
                Assert.AreEqual(0, DstEntityManager.GetSharedComponentData<EcsTestSharedComp>(dstEntity).value);
            }
        }

#endif

        [Test]
        public void EntityPatcher_ApplyChanges_ChangeAppliesToAllPrefabInstances([Values] bool prefabTag)
        {
            using (var differ = new EntityManagerDiffer(SrcEntityManager, Allocator.TempJob))
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
                PushChanges(differ, DstEntityManager);

                var dstPrefab = GetEntity(DstEntityManager, entityGuid);

                // Spawn some more instances of this thing in the dst world.
                var dstInstance0 = DstEntityManager.Instantiate(dstPrefab);
                var dstInstance1 = DstEntityManager.Instantiate(dstPrefab);

                // Mutate the original prefab in the src world.
                SrcEntityManager.SetComponentData(prefab, new EcsTestData(10));

                // Sync to the dst world.
                PushChanges(differ, DstEntityManager);

                // The changes should be propagated to all instances.
                Assert.AreEqual(10, DstEntityManager.GetComponentData<EcsTestData>(dstPrefab).value);
                Assert.AreEqual(10, DstEntityManager.GetComponentData<EcsTestData>(dstInstance0).value);
                Assert.AreEqual(10, DstEntityManager.GetComponentData<EcsTestData>(dstInstance1).value);
            }
        }

        [Test]
        public void EntityPatcher_ApplyChanges_CreateDynamicBuffer([Values(1, 100)] int bufferLength)
        {
            using (var differ = new EntityManagerDiffer(SrcEntityManager, Allocator.TempJob))
            {
                var entityGuid = CreateEntityGuid();
                var entity = SrcEntityManager.CreateEntity();

                SrcEntityManager.AddComponentData(entity, entityGuid);
                var buffer = SrcEntityManager.AddBuffer<EcsIntElement>(entity);

                for (var i = 0; i < bufferLength; ++i)
                {
                    buffer.Add(new EcsIntElement {Value = i});
                }

                PushChanges(differ, DstEntityManager);

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
        public void EntityPatcher_ApplyChanges_DebugNames(string srcName)
        {
            using (var differ = new EntityManagerDiffer(SrcEntityManager, Allocator.TempJob))
            {
                var entityGuid = CreateEntityGuid();
                var entity = SrcEntityManager.CreateEntity();
                SrcEntityManager.AddComponentData(entity, entityGuid);
                SrcEntityManager.SetName(entity, srcName);

                PushChanges(differ, DstEntityManager);

                var dstEntity = GetEntity(DstEntityManager, entityGuid);
                var dstName = DstEntityManager.GetName(dstEntity);

                Assert.AreEqual(srcName, dstName);
            }
        }

#endif

        [Test]
        public void EntityPatcher_ApplyChanges_EntityPatchWithMissingTargetDoesNotThrow()
        {
            using (var differ = new EntityManagerDiffer(SrcEntityManager, Allocator.TempJob))
            {
                var entityGuid0 = CreateEntityGuid();
                var entity0 = SrcEntityManager.CreateEntity();
                SrcEntityManager.AddComponentData(entity0, entityGuid0);

                var entityGuid1 = CreateEntityGuid();
                var entity1 = SrcEntityManager.CreateEntity();
                SrcEntityManager.AddComponentData(entity1, entityGuid1);

                PushChanges(differ, DstEntityManager);

                // Create a component with an entity reference
                SrcEntityManager.AddComponentData(entity1, new EcsTestDataEntity {value1 = entity0});

                using (var changes = differ.GetChanges(EntityManagerDifferOptions.IncludeForwardChangeSet, Allocator.TempJob))
                {
                    var forward = changes.ForwardChangeSet;

                    Assert.That(forward.EntityReferenceChanges.Length, Is.EqualTo(1));

                    // Destroy the entity we should patch
                    SrcEntityManager.DestroyEntity(entity1);

                    Assert.DoesNotThrow(() => { EntityPatcher.ApplyChangeSet(DstEntityManager, forward); });
                }
            }
        }

        [Test]
        public void EntityPatcher_ApplyChanges_EntityPatchWithMissingValueDoesNotThrow()
        {
            using (var differ = new EntityManagerDiffer(SrcEntityManager, Allocator.TempJob))
            {
                var entityGuid0 = CreateEntityGuid();
                var entity0 = SrcEntityManager.CreateEntity();
                SrcEntityManager.AddComponentData(entity0, entityGuid0);

                var entityGuid1 = CreateEntityGuid();
                var entity1 = SrcEntityManager.CreateEntity();
                SrcEntityManager.AddComponentData(entity1, entityGuid1);

                PushChanges(differ, DstEntityManager);

                // Create a component with an entity reference
                SrcEntityManager.AddComponentData(entity1, new EcsTestDataEntity {value1 = entity0});

                using (var changes = differ.GetChanges(EntityManagerDifferOptions.IncludeForwardChangeSet, Allocator.TempJob))
                {
                    var forward = changes.ForwardChangeSet;

                    Assert.That(forward.EntityReferenceChanges.Length, Is.EqualTo(1));

                    // Destroy the entity the patch references
                    SrcEntityManager.DestroyEntity(entity0);

                    Assert.DoesNotThrow(() => { EntityPatcher.ApplyChangeSet(DstEntityManager, forward); });
                }
            }
        }

        [Test]
        public void EntityPatcher_ApplyChanges_EntityPatchWithAmbiguousValueDoesNotThrow()
        {
            using (var differ = new EntityManagerDiffer(SrcEntityManager, Allocator.TempJob))
            {
                var entityGuid0 = CreateEntityGuid();
                var entity0 = SrcEntityManager.CreateEntity();
                SrcEntityManager.AddComponentData(entity0, entityGuid0);

                var entityGuid1 = CreateEntityGuid();
                var entity1 = SrcEntityManager.CreateEntity();
                SrcEntityManager.AddComponentData(entity1, entityGuid1);

                // Create a component with an entity reference
                PushChanges(differ, DstEntityManager);

                // Create a patch
                SrcEntityManager.AddComponentData(entity1, new EcsTestDataEntity {value1 = entity0});

                using (var changes = differ.GetChanges(EntityManagerDifferOptions.IncludeForwardChangeSet, Allocator.TempJob))
                {
                    var forward = changes.ForwardChangeSet;

                    Assert.That(forward.EntityReferenceChanges.Length, Is.EqualTo(1));

                    // Create a new entity in the dst world with the same ID the patch value points to.
                    var dstEntity0 = DstEntityManager.CreateEntity();
                    DstEntityManager.AddComponentData(dstEntity0, entityGuid0);

                    Assert.DoesNotThrow(() => { EntityPatcher.ApplyChangeSet(DstEntityManager, forward); });
                }
            }
        }

        [Test]
        public void EntityPatcher_ApplyChanges_EntityPatchWithAmbiguousTargetDoesNotThrow()
        {
            using (var differ = new EntityManagerDiffer(SrcEntityManager, Allocator.TempJob))
            {
                var entityGuid0 = CreateEntityGuid();
                var entity0 = SrcEntityManager.CreateEntity();
                SrcEntityManager.AddComponentData(entity0, entityGuid0);

                var entityGuid1 = CreateEntityGuid();
                var entity1 = SrcEntityManager.CreateEntity();
                SrcEntityManager.AddComponentData(entity1, entityGuid1);

                PushChanges(differ, DstEntityManager);

                // Create a patch
                SrcEntityManager.AddComponentData(entity1, new EcsTestDataEntity {value1 = entity0});

                using (var changes = differ.GetChanges(EntityManagerDifferOptions.IncludeForwardChangeSet, Allocator.TempJob))
                {
                    var forward = changes.ForwardChangeSet;

                    Assert.That(forward.EntityReferenceChanges.Length, Is.EqualTo(1));

                    // Create a new entity in the dst world with the same ID the patch target.
                    var dstEntity0 = DstEntityManager.CreateEntity();
                    DstEntityManager.AddComponentData(dstEntity0, entityGuid1);

                    Assert.DoesNotThrow(() => { EntityPatcher.ApplyChangeSet(DstEntityManager, forward); });
                }
            }
        }

        [Test]
        public void EntityPatcher_ApplyChanges_NewEntityIsReplicatedIntoExistingPrefabInstances([Values(1, 10)] int instanceCount)
        {
            using (var differ = new EntityManagerDiffer(SrcEntityManager, Allocator.TempJob))
            {
                var rootEntityGuid = CreateEntityGuid();
                var childEntityGuid = CreateEntityGuid();
                
                var srcRootEntity = SrcEntityManager.CreateEntity(typeof(EcsTestDataEntity), typeof(Prefab), typeof(LinkedEntityGroup));
                
                SrcEntityManager.AddComponentData(srcRootEntity, rootEntityGuid);
                SrcEntityManager.GetBuffer<LinkedEntityGroup>(srcRootEntity).Add(srcRootEntity);
                    
                PushChanges(differ, DstEntityManager);
                
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
                
                PushChanges(differ, DstEntityManager);
                
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
        public void EntityPatcher_ApplyChanges_WithChunkData()
        {
            using (var differ = new EntityManagerDiffer(SrcEntityManager, Allocator.TempJob))
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
                    SrcEntityManager.AddComponentData(entity, new EcsTestData(1));
                    SrcEntityManager.AddChunkComponentData<EcsTestData2>(entity);
                    SrcEntityManager.SetChunkComponentData(SrcEntityManager.GetChunk(entity), new EcsTestData2(3));

                    PushChanges(differ, DstEntityManager);

                    dstRootEntity = GetEntity(DstEntityManager, guid);
                    Assert.AreEqual(1, DstEntityManager.GetComponentData<EcsTestData>(dstRootEntity).value);
                    Assert.IsTrue(DstEntityManager.HasChunkComponent<EcsTestData2>(dstRootEntity));
                    Assert.AreEqual(0, DstEntityManager.GetChunkComponentData<EcsTestData2>(dstRootEntity).value0);
                    Assert.AreEqual(1, DstEntityManager.CreateEntityQuery(typeof(ChunkHeader)).CalculateEntityCount());
                }

                // Changing Chunk component creates no diff
                {
                    SrcEntityManager.SetChunkComponentData(SrcEntityManager.GetChunk(entity), new EcsTestData2(7));
                    using (var changes = differ.GetChanges(EntityManagerDifferOptions.Default, Allocator.Temp))
                    {
                        Assert.IsFalse(changes.AnyChanges);
                    }
                }

                // Removing chunk component, removes chunk component again
                {
                    SrcEntityManager.RemoveChunkComponent<EcsTestData2>(entity);
                    PushChanges(differ, DstEntityManager);
                    Assert.IsFalse(DstEntityManager.HasChunkComponent<EcsTestData2>(dstRootEntity));
                    Assert.AreEqual(0, DstEntityManager.CreateEntityQuery(typeof(ChunkHeader)).CalculateEntityCount());
                }
            }
        }

#if !UNITY_DISABLE_MANAGED_COMPONENTS
        [Test]
        public void EntityPatcher_ApplyChanges_WithChunkData_ManagedComponents()
        {
            using (var differ = new EntityManagerDiffer(SrcEntityManager, Allocator.TempJob))
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
                    SrcEntityManager.AddComponentData(entity, new EcsTestData(1));
                    SrcEntityManager.AddChunkComponentData<EcsTestData2>(entity);
                    SrcEntityManager.SetChunkComponentData(SrcEntityManager.GetChunk(entity), new EcsTestData2(3));
                    SrcEntityManager.AddChunkComponentData<EcsTestManagedComponent>(entity);
                    SrcEntityManager.SetChunkComponentData(SrcEntityManager.GetChunk(entity), new EcsTestManagedComponent() { value = "SomeString" });

                    PushChanges(differ, DstEntityManager);

                    dstRootEntity = GetEntity(DstEntityManager, guid);
                    Assert.AreEqual(1, DstEntityManager.GetComponentData<EcsTestData>(dstRootEntity).value);
                    Assert.IsTrue(DstEntityManager.HasChunkComponent<EcsTestData2>(dstRootEntity));
                    Assert.IsTrue(DstEntityManager.HasChunkComponent<EcsTestManagedComponent>(dstRootEntity));
                    Assert.AreEqual(0, DstEntityManager.GetChunkComponentData<EcsTestData2>(dstRootEntity).value0);
                    Assert.AreEqual(null, DstEntityManager.GetChunkComponentData<EcsTestManagedComponent>(dstRootEntity));
                    Assert.AreEqual(1, DstEntityManager.CreateEntityQuery(typeof(ChunkHeader)).CalculateEntityCount());
                }

                // Changing Chunk component creates no diff
                {
                    SrcEntityManager.SetChunkComponentData(SrcEntityManager.GetChunk(entity), new EcsTestData2(7));
                    SrcEntityManager.SetChunkComponentData(SrcEntityManager.GetChunk(entity), new EcsTestManagedComponent() { value = "SomeOtherString" });
                    using (var changes = differ.GetChanges(EntityManagerDifferOptions.Default, Allocator.Temp))
                    {
                        Assert.IsFalse(changes.AnyChanges);
                    }
                }

                // Removing chunk component, removes chunk component again
                {
                    SrcEntityManager.RemoveChunkComponent<EcsTestData2>(entity);
                    SrcEntityManager.RemoveChunkComponent<EcsTestManagedComponent>(entity);
                    PushChanges(differ, DstEntityManager);
                    Assert.IsFalse(DstEntityManager.HasChunkComponent<EcsTestData2>(dstRootEntity));
                    Assert.IsFalse(DstEntityManager.HasChunkComponent<EcsTestManagedComponent>(dstRootEntity));
                    Assert.AreEqual(0, DstEntityManager.CreateEntityQuery(typeof(ChunkHeader)).CalculateEntityCount());
                }
            }
        }

        [Test]
        public void EntityPatcher_ApplyChanges_CreateEntityWithTestData_ManagedComponents()
        {
            using (var differ = new EntityManagerDiffer(SrcEntityManager, Allocator.TempJob))
            {
                var entity = SrcEntityManager.CreateEntity(typeof(EntityGuid), typeof(EcsTestData), typeof(EcsTestManagedComponent));

                var entityGuid = CreateEntityGuid();

                SrcEntityManager.SetComponentData(entity, entityGuid);
                SrcEntityManager.SetComponentData(entity, new EcsTestData { value = 9 });
                SrcEntityManager.SetComponentData(entity, new EcsTestManagedComponent { value = "SomeString" });

                PushChanges(differ, DstEntityManager);

                Assert.AreEqual(1, DstEntityManager.Debug.EntityCount);
                Assert.AreEqual(9, GetComponentData<EcsTestData>(DstEntityManager, entityGuid).value);
                Assert.AreEqual("SomeString", GetManagedComponentData<EcsTestManagedComponent>(DstEntityManager, entityGuid).value);

                // Mutate some component data.
                SrcEntityManager.SetComponentData(entity, entityGuid);
                SrcEntityManager.SetComponentData(entity, new EcsTestData { value = 10 });
                SrcEntityManager.SetComponentData(entity, new EcsTestManagedComponent { value = "SomeOtherString" });

                PushChanges(differ, DstEntityManager);

                Assert.AreEqual(1, DstEntityManager.Debug.EntityCount);
                Assert.AreEqual(10, GetComponentData<EcsTestData>(DstEntityManager, entityGuid).value);
                Assert.AreEqual("SomeOtherString", GetManagedComponentData<EcsTestManagedComponent>(DstEntityManager, entityGuid).value);

                // Destroy the entity
                SrcEntityManager.DestroyEntity(entity);

                PushChanges(differ, DstEntityManager);

                Assert.AreEqual(0, DstEntityManager.Debug.EntityCount);
            }
        }

        [Test]
        [Ignore("Not currently supported")]
        public void EntityPatcher_ApplyChanges_RemapEntityReferencesInManagedComponents()
        {
            using (var differ = new EntityManagerDiffer(SrcEntityManager, Allocator.TempJob))
            {
                // Create extra entity to make sure test doesn't accidentally succeed with no remapping
                SrcEntityManager.CreateEntity();

                var entityGuid0 = CreateEntityGuid();
                var entityGuid1 = CreateEntityGuid();

                var e0 = SrcEntityManager.CreateEntity(typeof(EntityGuid), typeof(EcsTestManagedDataEntity));
                var e1 = SrcEntityManager.CreateEntity(typeof(EntityGuid), typeof(EcsTestManagedDataEntity));

                SrcEntityManager.SetComponentData(e0, entityGuid0);
                SrcEntityManager.SetComponentData(e1, entityGuid1);

                SrcEntityManager.SetComponentData(e0, new EcsTestManagedDataEntity { value1 = e1 });
                SrcEntityManager.SetComponentData(e1, new EcsTestManagedDataEntity { value1 = e0 });

                PushChanges(differ, DstEntityManager);

                Assert.AreEqual(GetEntity(DstEntityManager, entityGuid1), GetManagedComponentData<EcsTestManagedDataEntity>(DstEntityManager, entityGuid0).value1);
                Assert.AreEqual(GetEntity(DstEntityManager, entityGuid0), GetManagedComponentData<EcsTestManagedDataEntity>(DstEntityManager, entityGuid1).value1);
            }
        }

        [Test]
        [StandaloneFixme] // No support for PinGCObject
        public void EntityPatcher_ApplyChanges_RemapEntityReferencesInManagedComponentCollection()
        {
            using (var differ = new EntityManagerDiffer(SrcEntityManager, Allocator.TempJob))
            {
                // Create extra entity to make sure test doesn't accidentally succeed with no remapping
                SrcEntityManager.CreateEntity();

                var entityGuid0 = CreateEntityGuid();
                var entityGuid1 = CreateEntityGuid();

                var e0 = SrcEntityManager.CreateEntity(typeof(EntityGuid), typeof(EcsTestManagedDataEntityCollection));
                var e1 = SrcEntityManager.CreateEntity(typeof(EntityGuid), typeof(EcsTestManagedDataEntityCollection));

                SrcEntityManager.SetComponentData(e0, entityGuid0);
                SrcEntityManager.SetComponentData(e1, entityGuid1);

                SrcEntityManager.SetComponentData(e0, new EcsTestManagedDataEntityCollection(new string[] { e1.ToString() }, new Entity[] { e1, e1, e1 }));
                SrcEntityManager.SetComponentData(e1, new EcsTestManagedDataEntityCollection(new string[] { e0.ToString() }, new Entity[] { e0, e0, e0 }));

                PushChanges(differ, DstEntityManager);

                var c0 = GetManagedComponentData<EcsTestManagedDataEntityCollection>(DstEntityManager, entityGuid0);
                var c1 = GetManagedComponentData<EcsTestManagedDataEntityCollection>(DstEntityManager, entityGuid1);
                Assert.AreEqual(GetEntity(DstEntityManager, entityGuid1), c0.value1[0]);
                Assert.AreEqual(GetEntity(DstEntityManager, entityGuid1), c0.value1[1]);
                Assert.AreEqual(GetEntity(DstEntityManager, entityGuid1), c0.value1[2]);
                Assert.AreEqual(GetEntity(DstEntityManager, entityGuid0), c1.value1[0]);
                Assert.AreEqual(GetEntity(DstEntityManager, entityGuid0), c1.value1[1]);
                Assert.AreEqual(GetEntity(DstEntityManager, entityGuid0), c1.value1[2]);
            }
        }

        [Test]
        public void EntityPatcher_ApplyChanges_AddComponent_ManagedComponents()
        {
            using (var differ = new EntityManagerDiffer(SrcEntityManager, Allocator.TempJob))
            {
                var entityGuid = CreateEntityGuid();
                var entity = SrcEntityManager.CreateEntity();
                SrcEntityManager.AddComponentData(entity, entityGuid);
                SrcEntityManager.AddComponentData(entity, new EcsTestData { value = 9 });
                SrcEntityManager.AddComponentData(entity, new EcsTestManagedComponent { value = "SomeString" });

                PushChanges(differ, DstEntityManager);

                // Add a component in the source world.
                SrcEntityManager.AddComponentData(entity, new EcsTestData2(10));
                SrcEntityManager.AddComponentData(entity, new EcsTestManagedComponent2() { value = "SomeOtherString" });

                // Mutate the dst world
                SetComponentData(DstEntityManager, entityGuid, new EcsTestData(-1));
                SetManagedComponentData(DstEntityManager, entityGuid, new EcsTestManagedComponent() { value = "YetAnotherString" });

                PushChanges(differ, DstEntityManager);

                // Both changes should be present in the output
                Assert.AreEqual(10, GetComponentData<EcsTestData2>(DstEntityManager, entityGuid).value0);
                Assert.AreEqual(-1, GetComponentData<EcsTestData>(DstEntityManager, entityGuid).value);
                Assert.AreEqual("SomeOtherString", GetManagedComponentData<EcsTestManagedComponent2>(DstEntityManager, entityGuid).value);
                Assert.AreEqual("YetAnotherString", GetManagedComponentData<EcsTestManagedComponent>(DstEntityManager, entityGuid).value);
            }
        }

        [Test]
        public void EntityPatcher_ApplyChanges_RemoveComponent_ManagedComponents()
        {
            using (var differ = new EntityManagerDiffer(SrcEntityManager, Allocator.TempJob))
            {
                var entityGuid = CreateEntityGuid();
                var entity = SrcEntityManager.CreateEntity();
                SrcEntityManager.AddComponentData(entity, entityGuid);
                SrcEntityManager.AddComponentData(entity, new EcsTestData { value = 9 });
                SrcEntityManager.AddComponentData(entity, new EcsTestData2(7));
                SrcEntityManager.AddComponentData(entity, new EcsTestManagedComponent { value = "SomeString" });
                SrcEntityManager.AddComponentData(entity, new EcsTestManagedComponent2 { value = "SomeOtherString" });

                PushChanges(differ, DstEntityManager);

                SrcEntityManager.RemoveComponent<EcsTestData>(entity);
                SrcEntityManager.RemoveComponent<EcsTestManagedComponent>(entity);
                SetComponentData(DstEntityManager, entityGuid, new EcsTestData2(-1));
                SetManagedComponentData(DstEntityManager, entityGuid, new EcsTestManagedComponent2() { value = "YetAnotherString" });

                PushChanges(differ, DstEntityManager);

                Assert.IsFalse(HasComponent<EcsTestData>(DstEntityManager, entityGuid));
                Assert.IsFalse(HasManagedComponent<EcsTestManagedComponent>(DstEntityManager, entityGuid));
                Assert.AreEqual(-1, GetComponentData<EcsTestData2>(DstEntityManager, entityGuid).value0);
                Assert.AreEqual("YetAnotherString", GetManagedComponentData<EcsTestManagedComponent2>(DstEntityManager, entityGuid).value);
            }
        }

#endif
        [Test]
        public void EntityPatcher_ApplyChanges_BlobAssets_CreateEntityWithBlobAssetReference()
        {
            using (var differ = new EntityManagerDiffer(SrcEntityManager, Allocator.TempJob))
            {
                var blobAssetReference = BlobAssetReference<int>.Create(11);
                var entity = SrcEntityManager.CreateEntity(typeof(EntityGuid), typeof(EcsTestDataBlobAssetRef));
                var entityGuid = CreateEntityGuid();

                SrcEntityManager.SetComponentData(entity, entityGuid);
                SrcEntityManager.SetComponentData(entity, new EcsTestDataBlobAssetRef { value = blobAssetReference });

                PushChanges(differ, DstEntityManager);

                Assert.AreEqual(1, DstEntityManager.Debug.EntityCount);
                Assert.AreEqual(11, GetComponentData<EcsTestDataBlobAssetRef>(DstEntityManager, entityGuid).value.Value);
            }
        }
    }
}
