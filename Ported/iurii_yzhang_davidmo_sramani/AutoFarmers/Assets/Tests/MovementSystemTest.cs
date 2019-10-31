using System.Collections;
using System.Collections.Generic;
using GameAI;
using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Tests;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class MovementSystemTest : ECSTestsFixture
    {
        [Test]
        public void MoveEntity()
        {
            BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();

            int2 position = new int2(0,0);
            int2 targetPosition = new int2(1, 1);
            var e = World.EntityManager.CreateEntity(typeof(AnimationCompleteTag), typeof(HasTarget), typeof(TilePositionable));
            World.EntityManager.SetComponentData(e, new HasTarget() { TargetPosition = targetPosition});
            World.EntityManager.SetComponentData(e, new TilePositionable() { Position = position});

            var movementSystem = World.GetOrCreateSystem<MovementSystem>();
            Assert.IsTrue(movementSystem.Enabled && movementSystem.ShouldRunSystem());
            movementSystem.Update();
            m_EntityCommandBufferSystem.Update();
            m_Manager.CompleteAllJobs();

            Assert.IsFalse(World.EntityManager.HasComponent(e, typeof(HasTarget)));

            TilePositionable value = World.EntityManager.GetComponentData<TilePositionable>(e);
            Assert.IsTrue(value.Position.x == targetPosition.x && value.Position.y == targetPosition.y );
        }
    }
}