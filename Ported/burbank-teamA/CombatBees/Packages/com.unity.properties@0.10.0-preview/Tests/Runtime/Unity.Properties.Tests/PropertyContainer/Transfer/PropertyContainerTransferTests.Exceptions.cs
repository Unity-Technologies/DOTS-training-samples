using System;
using NUnit.Framework;

namespace Unity.Properties.Tests
{
    [TestFixture]
    partial class PropertyContainerTransferTests
    {
        [Test]
        public void PropertyContainer_Transfer_NullDstContainerShouldGiveUnderstandableErrorMessage()
        {
            var src = new CustomDataFoo();
            CustomDataFoo dst = null;

            var ex = Assert.Throws<ArgumentNullException>(() => PropertyContainer.Transfer(dst, src));
            Assert.That(ex.ParamName, Is.EqualTo("dstContainer"));
            Assert.That(ex.Message, Is.EqualTo("Value cannot be null." + Environment.NewLine +
                                               "Parameter name: dstContainer"));
        }

        [Test]
        public void PropertyContainer_Transfer_NullDstContainerByRefShouldGiveUnderstandableErrorMessage()
        {
            var src = new CustomDataFoo();
            CustomDataFoo dst = null;

            var ex = Assert.Throws<ArgumentNullException>(() => PropertyContainer.Transfer(ref dst, ref src));
            Assert.That(ex.ParamName, Is.EqualTo("dstContainer"));
            Assert.That(ex.Message, Is.EqualTo("Value cannot be null." + Environment.NewLine +
                                               "Parameter name: dstContainer"));
        }
    }
}