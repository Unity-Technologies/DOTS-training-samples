using NUnit.Framework;

namespace Unity.Properties.Tests
{
    [TestFixture]
    partial class PropertyContainerTransferTests
    {
        [Test]
        public void PropertyContainer_Transfer_StructWithInterface()
        {
            var src = (IContainer) new StructContainerWithInterface
            {
                X = 1,
                Y = 3,
                Z = 10
            };
            var dst = (IContainer) new StructContainerWithInterface();

            using (var result = PropertyContainer.Transfer(ref dst, ref src))
            {
                Assert.That(result.Succeeded, Is.True);
                Assert.That(((StructContainerWithInterface) dst).X, Is.EqualTo(1));
                Assert.That(((StructContainerWithInterface) dst).Y, Is.EqualTo(3));
                Assert.That(((StructContainerWithInterface) dst).Z, Is.EqualTo(10));
            }
        }
    }
}