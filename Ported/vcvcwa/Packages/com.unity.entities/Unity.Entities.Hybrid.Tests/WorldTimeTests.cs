using System;
using NUnit.Framework;

namespace Unity.Entities.Tests
{
    public class WorldTimeTests : ECSTestsFixture
    {
        [UpdateInGroup(typeof(SimulationSystemGroup))]
        public class TimeCheckMatchesUnitySystem : ComponentSystem
        {
            public int updateCount;

            protected override void OnUpdate()
            {
                updateCount++;

                Assert.AreEqual(UnityEngine.Time.time, Time.ElapsedTime);
                Assert.AreEqual(UnityEngine.Time.deltaTime, Time.DeltaTime);
            }
        }

        [Test]
        public void WorldUnityDrivesTime()
        {
            var world = new World("World A");
            var sim = world.GetOrCreateSystem<SimulationSystemGroup>();
            var init = world.GetOrCreateSystem<InitializationSystemGroup>();

            var unityTimeSys = world.GetOrCreateSystem(typeof(UpdateWorldTimeSystem));
            init.AddSystemToUpdateList(unityTimeSys);

            var checkSys = world.GetOrCreateSystem<TimeCheckMatchesUnitySystem>();
            sim.AddSystemToUpdateList(checkSys);

            world.Update();
            Assert.AreEqual(1, checkSys.updateCount);
        }
    }
}
