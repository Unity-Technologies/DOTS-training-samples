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
            var e = World.EntityManager.CreateEntity(typeof(AnimationCompleteTag), typeof(HasTarget), typeof(TilePositionable));
            World.EntityManager.SetComponentData(e, new HasTarget() { TargetPosition = new int2(1,1)});
            World.EntityManager.SetComponentData(e, new TilePositionable() { Position = new int2(0,0)});

            var movementSystem = World.GetOrCreateSystem<MovementSystem>();
            Assert.IsTrue(movementSystem.Enabled && movementSystem.ShouldRunSystem());
            movementSystem.Update();
            
            Assert.IsFalse(World.EntityManager.HasComponent<HasTarget>(e));

            TilePositionable value = World.EntityManager.GetComponentData<TilePositionable>(e);
            HasTarget target = World.EntityManager.GetComponentData<HasTarget>(e);
            Assert.IsTrue(value.Position.x == target.TargetPosition.x && value.Position.y == target.TargetPosition.y );
        }
    }
}