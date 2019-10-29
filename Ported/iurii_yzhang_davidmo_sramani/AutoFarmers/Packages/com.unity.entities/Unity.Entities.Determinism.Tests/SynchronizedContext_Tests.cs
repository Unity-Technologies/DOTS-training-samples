using NUnit.Framework;

namespace Unity.Entities.Determinism.Tests
{
    [DisableAutoCreation]
    class SynchronizedContext_TestSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
        }
    }
    
    [TestFixture]
    public class SynchronizedContext_Tests : PaddingMasks_TestFixture
    {
        const string worldName = "SynchronizedContext_TestWorld";

        [Test]
        public void SynchronizedContext_ContextFromWorld_IsNotValidInDifferentWorld()
        {
            using (var worldA = DeterminismTestUtility.CreateTestWorld_WithLocalToWorldComponents(worldName, 1))
            using (var worldB = DeterminismTestUtility.CreateTestWorld_WithLocalToWorldComponents(worldName, 1))
            {
                using (var context = SynchronizedContext.CreateFromWorld(worldA))
                {
                    Assert.IsFalse(context.IsValidIn(worldB));
                }
            }
        }

        [Test]
        public void SynchronizedContext_ContextFromWorld_IsValidInWorldAfterCreation()
        {
            using (var world = DeterminismTestUtility.CreateTestWorld_WithLocalToWorldComponents(worldName, 1))
            {
                using (var context = SynchronizedContext.CreateFromWorld(world))
                {
                    Assert.That(context.IsValidIn(world));
                }
            }
        }

        [Test]
        public void SynchronizedContext_ContextFromWorld_IsInvalidAfterEntityCreated()
        {
            using (var world = DeterminismTestUtility.CreateTestWorld_WithLocalToWorldComponents(worldName, 1))
            {
                using (var context = SynchronizedContext.CreateFromWorld(world))
                {
                    world.EntityManager.CreateEntity();
                    Assert.IsFalse(context.IsValidIn(world));
                }
            }
        }

        [Test]
        public void SynchronizedContext_ContextFromWorld_IsInvalidAfterSystemUpdate()
        {
            using (var world = DeterminismTestUtility.CreateTestWorld_WithLocalToWorldComponents(worldName, 1))
            {
                var system = world.CreateSystem<SynchronizedContext_TestSystem>();
                using (var context = SynchronizedContext.CreateFromWorld(world))
                {
                    system.Update();
                    Assert.IsFalse(context.IsValidIn(world));
                }
            }
        }
    }
}
