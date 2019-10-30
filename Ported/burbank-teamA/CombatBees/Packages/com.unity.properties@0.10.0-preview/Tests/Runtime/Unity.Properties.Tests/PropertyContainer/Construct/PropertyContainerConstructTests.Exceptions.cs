using System;
using NUnit.Framework;

namespace Unity.Properties.Tests
{
    [TestFixture]
    partial class PropertyContainerConstructTests
    {
        [Test]
        public void PropertyContainer_Construct_NullSrcContainerShouldGiveUnderstandableErrorMessage()
        {
            var src = (CustomDataFoo) null;
            var dst = new CustomDataFoo();

            var ex = Assert.Throws<ArgumentNullException>(() => PropertyContainer.Construct(ref dst, ref src));
            Assert.That(ex.ParamName, Is.EqualTo("srcContainer"));
            Assert.That(ex.Message, Is.EqualTo("Value cannot be null." + Environment.NewLine + "Parameter name: srcContainer"));
        }
    }
}