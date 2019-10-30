using System;
using NUnit.Framework;
using Unity.Collections;

namespace Unity.Properties.Reflection.Tests
{
    [TestFixture]
    class ReflectedPropertyBagTests
    {
#pragma warning disable 649
        struct SimpleContainerWithPrivateFields
        {
            [Property] int m_Int32Value;
            int m_HiddenInt32Value;
            
            public int Int32Value => m_Int32Value;
        }

        struct ContainerWithProperties
        {
            [Property] public int IntProperty { get; set; }
            public int HiddenInt32Property { get; }
        }

        struct ContainerWithSpecialAccess
        {
            public int m_ReadWriteField;
            [ReadOnly] public int m_ExplicitReadOnlyField;
            [Property] public int ReadWriteProperty { get; set; }
            [Property] public int ImplicitReadOnlyProperty { get; }
        }
#pragma warning restore 649

        [Test]
        public void ReflectedPropertyBag_SetValue_PrivateFields()
        {
            PropertyBagResolver.Register(new ReflectedPropertyBagProvider().Generate<SimpleContainerWithPrivateFields>());
            var instance = default(SimpleContainerWithPrivateFields);

            PropertyContainer.SetValue(ref instance, "m_Int32Value", 10);

            Assert.That(instance.Int32Value, Is.EqualTo(10));
                
            Assert.Throws<InvalidOperationException>(() =>
            {
                PropertyContainer.SetValue(ref instance, "m_HiddenInt32Value", 10);
            });
        }
        
        [Test]
        public void ReflectedPropertyBag_SetValue_CSharpProperties()
        {
            PropertyBagResolver.Register(new ReflectedPropertyBagProvider().Generate<ContainerWithProperties>());
            var instance = default(ContainerWithProperties);

            PropertyContainer.SetValue(ref instance, "IntProperty", 10);

            Assert.That(instance.IntProperty, Is.EqualTo(10));
                
            Assert.Throws<InvalidOperationException>(() =>
            {
                PropertyContainer.SetValue(ref instance, "HiddenInt32Property", 10);
            });
        }
        
        [Test]
        public void ReflectedPropertyBag_SetValue_ReadOnly()
        {
            PropertyBagResolver.Register(new ReflectedPropertyBagProvider().Generate<ContainerWithSpecialAccess>());
            var instance = default(ContainerWithSpecialAccess);

            Assert.DoesNotThrow(() =>
            {
                PropertyContainer.SetValue(ref instance, "m_ReadWriteField", 10);
            });
            
            Assert.Throws<InvalidOperationException>(() =>
            {
                PropertyContainer.SetValue(ref instance, "m_ExplicitReadOnlyField", 10);
            });
            
            Assert.DoesNotThrow(() =>
            {
                PropertyContainer.SetValue(ref instance, "ReadWriteProperty", 10);
            });
            
            Assert.Throws<InvalidOperationException>(() =>
            {
                PropertyContainer.SetValue(ref instance, "ImplicitReadOnlyProperty", 10);
            });
        }
    }
}