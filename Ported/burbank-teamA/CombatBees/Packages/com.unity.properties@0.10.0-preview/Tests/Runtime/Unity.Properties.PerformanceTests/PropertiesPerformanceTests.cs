using NUnit.Framework;
using Unity.PerformanceTesting;
using Unity.Properties.Tests;

namespace Unity.Properties.PerformanceTests
{
    [TestFixture]
    [Category("Performance")]
    internal class PropertiesPerformanceTests
    {
        [SetUp]
        public void SetUp()
        {
            PropertyBagResolver.Register(new TestNestedContainerPropertyBag());
            PropertyBagResolver.Register(new TestPrimitiveContainerPropertyBag());
            PropertyBagResolver.Register(new TestArrayContainerPropertyBag());
            PropertyBagResolver.Register(new CustomDataFooPropertyBag());
            PropertyBagResolver.Register(new CustomDataBarPropertyBag());
        }


#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        [TestCase(1000)]
        public void PerformanceTest_Properties_Transfer_Interface(int count)
        {
            var src = new TestInterfaceContainer
            {
                CustomData = new CustomDataFoo
                {
                    Test = 10,
                    Foo = 42
                }
            };

            var dst = new TestInterfaceContainer
            {
                CustomData = new CustomDataBar
                {
                    Test = 20,
                    Bar = -1
                }
            };

            Measure.Method(() =>
                   {
                       PropertyContainer.Transfer(ref dst, ref src).Dispose();
                   })
                   .Definition("PropertyContainerTransfer")
                   .WarmupCount(1)
                   .MeasurementCount(100)
                   .GC()
                   .Run();
        }




#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        [TestCase(100000)]
        public void PerformanceTest_Properties_Transfer_Primitives(int count)
        {
            var src = new TestPrimitiveContainer()
            {
            };

            var dst = new TestPrimitiveContainer
            {
            };

            Measure.Method(() =>
                   {
                       PropertyContainer.Transfer(ref dst, ref src).Dispose();
                   })
                   .Definition("PropertyContainerTransfer")
                   .WarmupCount(1)
                   .MeasurementCount(100)
                   .GC()
                   .Run();
        }
    }
}
