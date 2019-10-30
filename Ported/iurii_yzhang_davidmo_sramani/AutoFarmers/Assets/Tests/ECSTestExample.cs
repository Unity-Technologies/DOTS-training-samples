using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Tests;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class ECSTestExample : ECSTestsFixture
    {
        [Test]
        public void AddComponentEmptyNativeArray()
        {
            var array = new NativeArray<Entity>(0, Allocator.Temp);
            m_Manager.AddComponent(array, typeof(EcsTestData));
            array.Dispose();
        }
    }
}
