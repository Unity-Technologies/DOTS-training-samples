using System.Collections;
using System.Collections.Generic;
using GameAI;
using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Tests;
using UnityEngine;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;

namespace Tests
{
    public class ECSTestExample : ECSTestsFixture
    {
        [Test]
        public void RenderingUnityCreateCorrectEntities()
        {
            var go = new GameObject();
            go.AddComponent<RenderingUnity>().Initialize();
            
            var ru = RenderingUnity.instance;

            Assert.IsNotNull(ru, "RenderingUnity.instance == null");
            Assert.IsNotNull(m_Manager, "m_Manager == null");

            Assert.IsTrue(ru.CreateGround(m_Manager) != Entity.Null, "Ground entity creation failed");
            Assert.IsTrue(ru.CreateStone(m_Manager) != Entity.Null, "Stone entity creation failed");
            Assert.IsTrue(ru.CreatePlant(m_Manager) != Entity.Null, "Plant entity creation failed");
        }
        
        [Test]
        public void WorldCreationIsCalledOnlyOnce()
        {
            var wc = World.GetOrCreateSystem<WorldCreatorSystem>();
            Assert.IsTrue(wc.Enabled && wc.ShouldRunSystem());
            wc.Update();
            
            Assert.IsFalse(wc.Enabled && wc.ShouldRunSystem());
            wc.Update();
            Assert.IsFalse(wc.Enabled && wc.ShouldRunSystem());
            wc.Update();
            
            WorldCreatorSystem.ResetExecuteOnceTag(m_Manager);
            Assert.IsTrue(wc.Enabled && wc.ShouldRunSystem());
            wc.Update();
        }
        
        [Test]
        public void RenderConvertionIsCalledOnlyOnce()
        {
            var wc = World.GetOrCreateSystem<RenderingMapInit>();
            Assert.IsTrue(wc.Enabled && wc.ShouldRunSystem());
            wc.Update();
            
            Assert.IsFalse(wc.Enabled && wc.ShouldRunSystem());
            wc.Update();
            Assert.IsFalse(wc.Enabled && wc.ShouldRunSystem());
            wc.Update();
            
            RenderingMapInit.ResetExecuteOnceTag(m_Manager);
            Assert.IsTrue(wc.Enabled && wc.ShouldRunSystem());
            wc.Update();
        }
    }
}
