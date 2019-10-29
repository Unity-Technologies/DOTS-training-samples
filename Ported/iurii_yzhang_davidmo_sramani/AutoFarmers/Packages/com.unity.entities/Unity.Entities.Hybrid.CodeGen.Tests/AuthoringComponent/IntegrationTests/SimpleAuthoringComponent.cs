using System;
using NUnit.Framework;
using Unity.Entities.CodeGen.Tests;
using Unity.Collections;
using Unity.Jobs;

namespace Unity.Entities.Hybrid.CodeGen.Tests
{
    [GenerateAuthoringComponent]
    public struct SimpleComponent : IComponentData
    {
        public float FloatValue;
        public int IntValue;
    }
    
    [TestFixture]
    public class SimpleAuthoringComponent : AuthoringComponentIntegrationTest
    {
        [Test]
        public void SimpleAuthoringComponentTest() => RunTest(typeof(SimpleComponent));
    }
}
