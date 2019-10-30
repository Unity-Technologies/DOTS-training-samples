using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Unity.Properties.Tests
{
    [TestFixture]
    class PropertyContainerValueTests
    {
        [SetUp]
        public void SetUp()
        {
            TestData.InitializePropertyBags();
        }

        [Test]
        public void PropertyContainer_SetValue_Primitives()
        {
            var container = new TestPrimitiveContainer();

            PropertyContainer.SetValue(ref container, nameof(TestPrimitiveContainer.Int32Value), 10);
            PropertyContainer.SetValue(ref container, nameof(TestPrimitiveContainer.Float32Value), 2.5f);

            Assert.AreEqual(10, container.Int32Value);
            Assert.AreEqual(2.5f, container.Float32Value);
        }

        [Test]
        public void PropertyContainer_SetValue_Enums_Direct()
        {
            var container = new TestPrimitiveContainer();

            PropertyContainer.SetValue(ref container, nameof(TestPrimitiveContainer.FlagsEnum), FlagsEnum.Value1 | FlagsEnum.Value4);
            PropertyContainer.SetValue(ref container, nameof(TestPrimitiveContainer.UnorderedIntEnum), UnorderedIntEnum.Value4);

            Assert.AreEqual(FlagsEnum.Value1 | FlagsEnum.Value4, container.FlagsEnum);
            Assert.AreEqual(UnorderedIntEnum.Value4, container.UnorderedIntEnum);
        }

        [Test]
        public void PropertyContainer_SetValue_Enums_As_Int()
        {
            var container = new TestPrimitiveContainer();

            PropertyContainer.SetValue(ref container, nameof(TestPrimitiveContainer.FlagsEnum), (int)(FlagsEnum.Value1 | FlagsEnum.Value4));
            PropertyContainer.SetValue(ref container, nameof(TestPrimitiveContainer.UnorderedIntEnum), (int)UnorderedIntEnum.Value4);

            Assert.AreEqual(FlagsEnum.Value1 | FlagsEnum.Value4, container.FlagsEnum);
            Assert.AreEqual(UnorderedIntEnum.Value4, container.UnorderedIntEnum);
        }
        
        [Test]
        public void PropertyContainer_SetValue_Enums_As_NarrowingConversion()
        {
            var container = new TestPrimitiveContainer();

            PropertyContainer.SetValue(ref container, nameof(TestPrimitiveContainer.FlagsEnum), (ulong)(FlagsEnum.Value1 | FlagsEnum.Value4));
            PropertyContainer.SetValue(ref container, nameof(TestPrimitiveContainer.UnorderedIntEnum), (ulong)UnorderedIntEnum.Value4);

            PropertyContainer.SetValue(ref container, nameof(TestPrimitiveContainer.SmallEnum), (ulong)(SmallEnum.Value1 | SmallEnum.Value2));
            
            Assert.AreEqual(FlagsEnum.Value1 | FlagsEnum.Value4, container.FlagsEnum);
            Assert.AreEqual(UnorderedIntEnum.Value4, container.UnorderedIntEnum);
        }
        
        [Test]
        public void PropertyContainer_SetValue_Enums_As_Byte()
        {
            var container = new TestPrimitiveContainer();

            PropertyContainer.SetValue(ref container, nameof(TestPrimitiveContainer.SmallEnum), (byte)(SmallEnum.Value1 | SmallEnum.Value2));

            Assert.AreEqual(SmallEnum.Value1 | SmallEnum.Value2, container.SmallEnum);
        }

        [Test]
        public void PropertyContainer_SetValue_Enums_As_String()
        {
            var container = new TestPrimitiveContainer();

            PropertyContainer.SetValue(ref container, nameof(TestPrimitiveContainer.FlagsEnum), FlagsEnum.Value2.ToString());

            Assert.AreEqual(FlagsEnum.Value2, container.FlagsEnum);
        }

        [Test]
        public void PropertyContainer_SetValue_Enums_As_String_ThrowsWhenStringNotParsed()
        {
            var container = new TestPrimitiveContainer();

            Assert.Throws<InvalidOperationException>(() => PropertyContainer.SetValue(ref container, nameof(TestPrimitiveContainer.FlagsEnum), "NotAValidValue"));

            Assert.AreEqual(FlagsEnum.None, container.FlagsEnum);
        }

        [Test]
        public void PropertyContainer_SetValue_NestedContainer()
        {
            var container = new TestNestedContainer();

            PropertyContainer.SetValue(ref container, nameof(TestNestedContainer.TestPrimitiveContainer), new TestPrimitiveContainer { Int32Value = 42 });

            Assert.AreEqual(42, container.TestPrimitiveContainer.Int32Value);
        }
        
        [Test]
        public void PropertyContainer_SetValue_Collection()
        {
            var container = new TestArrayContainer();

            PropertyContainer.SetValue(ref container, nameof(TestArrayContainer.Int32Array), new [] { 4, 5, 6 });

            Assert.AreEqual(3, container.Int32Array.Length);
        }
        
        [Test]
        public void PropertyContainer_SetValue_Conversion_DoesNotThrow()
        {
            var container = new TestPrimitiveContainer();

            Assert.DoesNotThrow(() =>
            {
                PropertyContainer.SetValue(ref container, nameof(TestPrimitiveContainer.Int32Value), 10.5f);
            });

            Assert.AreEqual(10, container.Int32Value);
        }

        struct NotSupportedType
        {
        }
        
        [Test]
        public void PropertyContainer_SetValue_Conversion_Throws()
        {
            var container = new TestPrimitiveContainer();
            var value = new NotSupportedType();

            Assert.Throws<InvalidOperationException>(() =>
            {
                PropertyContainer.SetValue(ref container, nameof(TestPrimitiveContainer.Int32Value), value);
            });
        }
        
        [Test]
        public void PropertyContainer_SetValue_InvalidName_Throws()
        {
            var container = new TestPrimitiveContainer();

            Assert.Throws<InvalidOperationException>(() =>
            {
                PropertyContainer.SetValue(ref container, "test", 10);
            });
        }
        
        [Test]
        [TestCase(0, 1, true)]
        [TestCase(2, 3, true)]
        [TestCase(4, 4, false)]
        [TestCase(0, 0, false)]
        public void PropertyContainer_SetValue_ChangeTracker(int start, int end, bool expected)
        {
            var container = new TestPrimitiveContainer
            {
                Int32Value = start
            };
            
            var changeTracker = new ChangeTracker();
            PropertyContainer.SetValue(ref container, nameof(TestPrimitiveContainer.Int32Value), end, ref changeTracker);
            Assert.AreEqual(expected, changeTracker.IsChanged());
        }
        
        [Test]
        public void PropertyContainer_SetValue_InvalidPropertyBag_Throws()
        { 
            Assert.Throws<InvalidOperationException>(() =>
            {
                var container = 10;
                PropertyContainer.SetValue(ref container, "test", 10);
            });
        }
        
        [Test]
        public void PropertyContainer_GetValue_Primitives()
        {
            var container = new TestPrimitiveContainer
            {
                Int32Value = 42, 
                Float64Value = 12345.678
            };
            
            Assert.AreEqual(42, PropertyContainer.GetValue<TestPrimitiveContainer, int>(ref container, nameof(TestPrimitiveContainer.Int32Value)));
            Assert.AreEqual(12345.678, PropertyContainer.GetValue<TestPrimitiveContainer, double>(ref container, nameof(TestPrimitiveContainer.Float64Value)));
        }
        
        [Test]
        public void PropertyContainer_GetValue_NestedContainer()
        {
            var container = new TestNestedContainer
            {
                TestPrimitiveContainer = new TestPrimitiveContainer
                {
                    Int32Value = 42
                }
            };

            var value = PropertyContainer.GetValue<TestNestedContainer, TestPrimitiveContainer>(ref container, nameof(TestNestedContainer.TestPrimitiveContainer));
            
            Assert.AreEqual(42, value.Int32Value);
        }
        
        [Test]
        public void PropertyContainer_GetValue_Conversion_DoesNotThrow()
        {
            var container = new TestPrimitiveContainer
            {
                Float64Value = 12345.678
            };
            
            var value = PropertyContainer.GetValue<TestPrimitiveContainer, int>(ref container, nameof(TestPrimitiveContainer.Float64Value));
            
            Assert.AreEqual(12345, value);
        }
        
        [Test]
        public void PropertyContainer_GetCount()
        {
            var container = new TestListContainer
            {
                Int32List = new List<int>
                {
                    1, 2, 3, 4, 5
                }
            };

            Assert.That(PropertyContainer.GetCount(ref container, nameof(TestListContainer.Int32List)), Is.EqualTo(5));
        }
        
        [Test]
        public void PropertyContainer_SetCount()
        {
            var container = new TestListContainer
            {
                Int32List = new List<int>
                {
                    1, 2, 3, 4, 5
                }
            };
            
            PropertyContainer.SetCount(ref container, nameof(TestListContainer.Int32List), 10);
            Assert.That(container.Int32List.Count, Is.EqualTo(10));
        }
    }
}