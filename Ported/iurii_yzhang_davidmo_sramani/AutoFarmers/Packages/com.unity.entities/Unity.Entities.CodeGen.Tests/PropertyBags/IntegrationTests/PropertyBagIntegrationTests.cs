using NUnit.Framework;
using Unity.Entities.CodeGen.Tests.PropertyBags.Infrastructure;

namespace Unity.Entities.CodeGen.Tests.PropertyBags
{
#if !NET_DOTS
    // Internally we won't actually produce PropertyBags for valuetype IComponentData (validated in XXXXXX)
    // but this test proves we do support generic PropertyBag generation which is still of interest
    [TestFixture]
    public class ValueTypeIComponentData : PropertyBagsIntegrationTest
    {
        [Test]
        public void ValueTypeIComponentDataTest() => RunTest(typeof(Types.ValueTypeIComponentData));
    }
    
    [TestFixture]
    public class ValueTypeSharedComponentData : PropertyBagsIntegrationTest
    {
        [Test]
        public void ValueTypeSharedComponentDataTest() => RunTest(typeof(Types.ValueTypeSharedComponentData));
    }

#if !UNITY_DISABLE_MANAGED_COMPONENTS
    [TestFixture]
    public class ManagedIComponentData : PropertyBagsIntegrationTest
    {
        [Test]
        public void ManagedIComponentDataTest() => RunTest(typeof(Types.ManagedIComponentData));
    }

    [TestFixture]
    public class MyComponent : PropertyBagsIntegrationTest
    {
        [Test]
        public void MyComponentTest() => RunTest(typeof(Some.Namespace.MyComponent));
    }
#endif
#endif
}