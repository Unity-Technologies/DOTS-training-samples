using NUnit.Framework;

namespace Unity.Properties.Tests
{
    [TestFixture]
    partial class PropertyContainerTransferTests
    {
        [Test]
        public void PropertyContainer_Transfer_PrimitiveFields()
        {
            var src = new StructContainerWithPrimitives {Int32Value = 10};
            var dst = new StructContainerWithPrimitives {Int32Value = 20};

            using (var result = PropertyContainer.Transfer(ref dst, ref src))
            {
                Assert.That(result.Succeeded, Is.True);
                Assert.AreEqual(10, dst.Int32Value);
            }
        }
        
        [Test]
        public void PropertyContainer_Transfer_NestedPrimitiveFieldsWithDifferentContainerTypes()
        {
            var src = new StructContainerWithNestedStruct {Container = new StructContainerWithPrimitives {Int32Value = 10}};
            var dst = new ClassContainerWithNestedClass {Container = new ClassContainerWithPrimitives()};

            Assert.DoesNotThrow(() =>
            {
                using (var result = PropertyContainer.Transfer(ref dst, ref src))
                {
                    Assert.That(result.Succeeded, Is.True);
                }
            });

            Assert.That(dst.Container.Int32Value, Is.EqualTo(10));
        }
        
        [Test]
        public void PropertyContainer_Transfer_FlagsEnumFields()
        {
            var src = new StructContainerWithPrimitives {FlagsEnum = FlagsEnum.Value1 | FlagsEnum.Value4 };
            var dst = new StructContainerWithPrimitives {FlagsEnum = FlagsEnum.None};

            using (var result = PropertyContainer.Transfer(ref dst, ref src))
            {
                Assert.That(result.Succeeded, Is.True);
                Assert.AreEqual(FlagsEnum.Value1 | FlagsEnum.Value4, dst.FlagsEnum);
            }
        }
    }
}