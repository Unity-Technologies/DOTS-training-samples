using System;
using NUnit.Framework;
using Unity.Entities.CodeGen.Tests;
using Unity.Collections;
using Unity.Jobs;

namespace Unity.Entities.Hybrid.CodeGen.Tests
{
    [GenerateAuthoringComponent]
    public struct Component : IComponentData
    {
        public Entity PrefabA;
        public Entity PrefabB;
        public float FloatValue;
        public int IntValue;
        public Entity PrefabC;
    }
    
    [TestFixture]
    public class AuthoringComponentWithReferencedPrefab : AuthoringComponentIntegrationTest
    {
        [Test]
        public void AuthoringComponentWithReferencedPrefabTest() => RunTest(typeof(Component));
    }
}
