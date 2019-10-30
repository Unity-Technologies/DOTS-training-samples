using NUnit.Framework;
using Unity.Collections;

namespace Unity.Entities.Tests
{
    [TestFixture]
    internal sealed class HierarchyIntegrationTest : EntityDifferTestFixture
    {
        private struct HierarchyComponent : IComponentData
        {
            public Entity Parent;
            public Entity Child;
        }

        [Test]
        public void UndoRedoPrefabInstancesWithRelationship()
        {
            const int instanceCount = 10;
            var srcPrefabRootGuid = CreateEntityGuid();
            var srcPrefabLevel0Guid = CreateEntityGuid();
            var prefabLevel1Guid = CreateEntityGuid();

            const EntityManagerDifferOptions options = EntityManagerDifferOptions.IncludeForwardChangeSet | 
                                                       EntityManagerDifferOptions.FastForwardShadowWorld;

            using (var differ = new EntityManagerDiffer(SrcEntityManager, Allocator.TempJob))
            {
                // Create Root Prefab entity with a Guid component
                var srcPrefabRoot = SrcEntityManager.CreateEntity(typeof(HierarchyComponent), typeof(Prefab), typeof(LinkedEntityGroup));
                SrcEntityManager.AddComponentData(srcPrefabRoot, srcPrefabRootGuid);
                SrcEntityManager.GetBuffer<LinkedEntityGroup>(srcPrefabRoot).Add(srcPrefabRoot);

                PushChanges(differ, DstEntityManager);
                
                using (var changes = differ.GetChanges(options, Allocator.TempJob))
                {
                    EntityPatcher.ApplyChangeSet(DstEntityManager, changes.ForwardChangeSet);
                }
                
                // Instantiate root prefab in dst world 10 times
                var dstPrefabRoot = GetEntity(DstEntityManager, srcPrefabRootGuid);
                var dstInstanceRoots = new Entity[instanceCount];
                for (var i = 0; i != dstInstanceRoots.Length; i++)
                {
                    var dstInstanceRoot = DstEntityManager.Instantiate(dstPrefabRoot);
                    dstInstanceRoots[i] = dstInstanceRoot;
                    Assert.AreEqual(1, DstEntityManager.GetBuffer<LinkedEntityGroup>(dstInstanceRoot).Length);
                    Assert.AreEqual(dstInstanceRoot, DstEntityManager.GetBuffer<LinkedEntityGroup>(dstInstanceRoot)[0].Value);
                }

                // Add level 0 entity to the prefab
                var srcLevel0Prefab = SrcEntityManager.CreateEntity(typeof(HierarchyComponent), typeof(Prefab));
                SrcEntityManager.AddComponentData(srcLevel0Prefab, srcPrefabLevel0Guid);
                SrcEntityManager.GetBuffer<LinkedEntityGroup>(srcPrefabRoot).Add(srcLevel0Prefab);

                SrcEntityManager.SetComponentData(srcPrefabRoot, new HierarchyComponent {Parent = Entity.Null, Child = srcLevel0Prefab});
                SrcEntityManager.SetComponentData(srcLevel0Prefab, new HierarchyComponent {Parent = srcPrefabRoot, Child = Entity.Null});

                // Synchronize worlds, we now should have 10 instances in the world along with a level0 for each
                // and hierarchy matching the relationships create
                PushChanges(differ, DstEntityManager);
                
                for (var i = 0; i != dstInstanceRoots.Length; i++)
                {
                    var dstInstanceRoot = dstInstanceRoots[i];
                    var dstInstanceGroup = DstEntityManager.GetBuffer<LinkedEntityGroup>(dstInstanceRoot);
                    var dstInstanceLevel0 = dstInstanceGroup[1].Value;

                    Assert.AreEqual(2, dstInstanceGroup.Length);
                    Assert.AreEqual(dstInstanceRoot, dstInstanceGroup[0].Value);
                    Assert.AreEqual(dstInstanceLevel0, dstInstanceGroup[1].Value);

                    var hierarchyLevel0 = DstEntityManager.GetComponentData<HierarchyComponent>(dstInstanceLevel0);
                    var hierarchyRoot = DstEntityManager.GetComponentData<HierarchyComponent>(dstInstanceRoot);

                    Assert.AreEqual(dstInstanceRoot, hierarchyLevel0.Parent);
                    Assert.AreEqual(dstInstanceLevel0, hierarchyRoot.Child);
                }
                
                // Add level 1 entity to the prefab
                var srcLevel1Prefab = SrcEntityManager.CreateEntity(typeof(HierarchyComponent), typeof(Prefab));
                SrcEntityManager.AddComponentData(srcLevel1Prefab, prefabLevel1Guid);
                SrcEntityManager.GetBuffer<LinkedEntityGroup>(srcPrefabRoot).Add(srcLevel1Prefab);

                SrcEntityManager.SetComponentData(srcLevel0Prefab, new HierarchyComponent {Parent = srcPrefabRoot, Child = srcLevel1Prefab});
                SrcEntityManager.SetComponentData(srcLevel1Prefab, new HierarchyComponent {Parent = srcLevel0Prefab, Child = Entity.Null});

                // Synchronize worlds, we now should have 10 instances of level 1 linked to their matching level 0
                PushChanges(differ, DstEntityManager);

                for (var i = 0; i != dstInstanceRoots.Length; i++)
                {
                    var dstRootInstance = dstInstanceRoots[i];

                    var dstInstanceGroup = DstEntityManager.GetBuffer<LinkedEntityGroup>(dstRootInstance);
                    Assert.AreEqual(3, dstInstanceGroup.Length);
                    Assert.AreEqual(dstRootInstance, dstInstanceGroup[0].Value);
                    var dstInstanceLevel0 = dstInstanceGroup[1].Value;
                    var dstInstanceLevel1 = dstInstanceGroup[2].Value;

                    Assert.AreEqual(dstInstanceLevel0,DstEntityManager.GetComponentData<HierarchyComponent>(dstRootInstance).Child);
                    Assert.AreEqual(dstRootInstance,DstEntityManager.GetComponentData<HierarchyComponent>(dstInstanceLevel0).Parent);
                    Assert.AreEqual(dstInstanceLevel1,DstEntityManager.GetComponentData<HierarchyComponent>(dstInstanceLevel0).Child);
                    Assert.AreEqual(dstInstanceLevel0,DstEntityManager.GetComponentData<HierarchyComponent>(dstInstanceLevel1).Parent);
                }

                // Remove level 1 entity from the prefab
                SrcEntityManager.GetBuffer<LinkedEntityGroup>(srcPrefabRoot).RemoveAt(2);
                SrcEntityManager.DestroyEntity(srcLevel1Prefab);

                // Fix the hierarchy of level 0 to remove the link to level 1
                SrcEntityManager.SetComponentData(srcLevel0Prefab, new HierarchyComponent {Parent = srcPrefabRoot, Child = Entity.Null});

                // Synchronize worlds, destination world should no longer have instances of level 1
                PushChanges(differ, DstEntityManager);

                for (var i = 0; i != dstInstanceRoots.Length; i++)
                {
                    var dstRootInstance = dstInstanceRoots[i];

                    var dstInstanceGroup = DstEntityManager.GetBuffer<LinkedEntityGroup>(dstRootInstance);
                    Assert.AreEqual(2, dstInstanceGroup.Length);
                    Assert.AreEqual(dstRootInstance, dstInstanceGroup[0].Value);
                    var dstInstanceLevel1 = dstInstanceGroup[1].Value;

                    Assert.AreEqual(dstInstanceLevel1,DstEntityManager.GetComponentData<HierarchyComponent>(dstRootInstance).Child);
                    Assert.AreEqual(dstRootInstance,DstEntityManager.GetComponentData<HierarchyComponent>(dstInstanceLevel1).Parent);
                    Assert.AreEqual(Entity.Null,DstEntityManager.GetComponentData<HierarchyComponent>(dstInstanceLevel1).Child);
                }

                // Add again level 1 as a Child of level 0
                srcLevel1Prefab = SrcEntityManager.CreateEntity(typeof(HierarchyComponent), typeof(Prefab));
                SrcEntityManager.AddComponentData(srcLevel1Prefab, prefabLevel1Guid);
                SrcEntityManager.GetBuffer<LinkedEntityGroup>(srcPrefabRoot).Add(srcLevel1Prefab);
                SrcEntityManager.SetComponentData(srcLevel0Prefab, new HierarchyComponent {Parent = srcPrefabRoot, Child = srcLevel1Prefab});
                SrcEntityManager.SetComponentData(srcLevel1Prefab, new HierarchyComponent {Parent = srcLevel0Prefab, Child = Entity.Null});

                PushChanges(differ, DstEntityManager);

                for (var i = 0; i != dstInstanceRoots.Length; i++)
                {
                    var dstRootInstance = dstInstanceRoots[i];

                    var dstInstanceGroup = DstEntityManager.GetBuffer<LinkedEntityGroup>(dstRootInstance);
                    Assert.AreEqual(3, dstInstanceGroup.Length);
                    Assert.AreEqual(dstRootInstance, dstInstanceGroup[0].Value);
                    var dstInstanceLevel0 = dstInstanceGroup[1].Value;
                    var dstInstanceLevel1 = dstInstanceGroup[2].Value;

                    Assert.AreEqual(dstInstanceLevel0, DstEntityManager.GetComponentData<HierarchyComponent>(dstRootInstance).Child);
                    Assert.AreEqual(dstRootInstance, DstEntityManager.GetComponentData<HierarchyComponent>(dstInstanceLevel0).Parent);
                    Assert.AreEqual(dstInstanceLevel1, DstEntityManager.GetComponentData<HierarchyComponent>(dstInstanceLevel0).Child);
                    Assert.AreEqual(dstInstanceLevel0, DstEntityManager.GetComponentData<HierarchyComponent>(dstInstanceLevel1).Parent);
                }
            }
        }
    }
}