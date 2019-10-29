using NUnit.Framework;
using Unity.Collections;
using Unity.Entities.Tests;
using Unity.PerformanceTesting;
using Unity.Transforms;

namespace Unity.Entities.PerformanceTests
{
    [Category("Performance")]
    public sealed unsafe class ChangeHierarchyPerformanceTests : ECSTestsFixture
    {

        public void UpdateTransforms()
        {
            World.GetOrCreateSystem<EndFrameParentSystem>().Update();
            World.GetOrCreateSystem<EndFrameCompositeRotationSystem>().Update();
            World.GetOrCreateSystem<EndFrameCompositeScaleSystem>().Update();
            World.GetOrCreateSystem<EndFrameParentScaleInverseSystem>().Update();
            World.GetOrCreateSystem<EndFrameTRSToLocalToWorldSystem>().Update();
            World.GetOrCreateSystem<EndFrameTRSToLocalToParentSystem>().Update();
            World.GetOrCreateSystem<EndFrameLocalToParentSystem>().Update();
            World.GetOrCreateSystem<EndFrameWorldToLocalSystem>().Update();
            
            // Force complete so that main thread (tests) can have access to direct editing.
            m_Manager.CompleteAllJobs();             
        }
        
    [Test, Performance]
        public void ChangeParents()
        {
            var rootArchetype = m_Manager.CreateArchetype(typeof(LocalToWorld));
            var childArchetype = m_Manager.CreateArchetype(typeof(LocalToWorld), typeof(LocalToParent), typeof(Parent), typeof(Prefab));

            var root0 = m_Manager.CreateEntity(rootArchetype);
            var root1 = m_Manager.CreateEntity(rootArchetype);
            var childPrefab = m_Manager.CreateEntity(childArchetype);
            var children = new NativeArray<Entity>(10000, Allocator.Persistent);
            m_Manager.Instantiate(childPrefab, children);

            var rootIndex = 0;
            
            Measure.Method(() =>
                {
                    UpdateTransforms();
                })
                .SetUp(() =>
                {
                    var parent = (rootIndex == 0) ? root0 : root1;
                    for (int i = 0; i < children.Length; i++)
                        m_Manager.SetComponentData(children[i], new Parent { Value = parent });
                    rootIndex ^= 1;
                })
                .CleanUp(() =>
                {
                })
                .Run();

            children.Dispose();
        }
    }
}
