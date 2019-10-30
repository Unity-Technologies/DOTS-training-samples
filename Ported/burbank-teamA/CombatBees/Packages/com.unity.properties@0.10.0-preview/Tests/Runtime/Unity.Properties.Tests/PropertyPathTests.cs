using System;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace Unity.Properties.Tests
{
    [TestFixture]
    internal class PropertyPathTests
    {
        [Test]
        public void CanConstructPropertyPathManually()
        {
            var propertyPath = new PropertyPath();
            Assert.That(propertyPath.PartsCount, Is.EqualTo(0));
            propertyPath.Push("Foo");
            Assert.That(propertyPath.PartsCount, Is.EqualTo(1));
            Assert.That(propertyPath[0].IsListItem, Is.EqualTo(false));
            Assert.That(propertyPath[0].Name, Is.EqualTo("Foo"));
            Assert.That(propertyPath[0].Index, Is.EqualTo(-1));
            
            propertyPath.Push("Bar", 5);
            Assert.That(propertyPath.PartsCount, Is.EqualTo(2));
            Assert.That(propertyPath[1].IsListItem, Is.EqualTo(true));
            Assert.That(propertyPath[1].Name, Is.EqualTo("Bar"));
            Assert.That(propertyPath[1].Index, Is.EqualTo(5));
            
            propertyPath.Push("Bee", PropertyPath.InvalidListIndex);
            Assert.That(propertyPath.PartsCount, Is.EqualTo(3));
            Assert.That(propertyPath[2].IsListItem, Is.EqualTo(false));
            Assert.That(propertyPath[2].Name, Is.EqualTo("Bee"));
            Assert.That(propertyPath[2].Index, Is.EqualTo(-1));
            
            Assert.That(propertyPath.ToString(), Is.EqualTo("Foo.Bar[5].Bee"));
            
            propertyPath.Pop();
            
            Assert.That(propertyPath.PartsCount, Is.EqualTo(2));
            Assert.That(propertyPath.ToString(), Is.EqualTo("Foo.Bar[5]"));
            
            propertyPath.Clear();
            
            Assert.That(propertyPath.PartsCount, Is.EqualTo(0));
            Assert.That(propertyPath.ToString(), Is.EqualTo(string.Empty));
        }

        [Test]
        [TestCase("")]
        [TestCase("Foo")]
        [TestCase("Foo[0]")]
        [TestCase("Foo[0].Bar")]
        [TestCase("Foo[0].Bar[1]")]
        [TestCase("Foo.Bar")]
        [TestCase("Foo.Bar[0]")]
        public void CanConstructAPropertyPathFromAString(string path)
        {
            Assert.That(() => CreateFromString(path), Throws.Nothing);
        }

        [Test]
        [TestCase("", 0)]
        [TestCase("Foo", 1)]
        [TestCase("Foo[0]", 1)]
        [TestCase("Foo[0].Bar", 2)]
        [TestCase("Foo[0].Bar[1]", 2)]
        [TestCase("Foo.Bar", 2)]
        [TestCase("Foo.Bar[0]", 2)]
        [TestCase("Foo.Foo.Foo.Foo.Foo", 5)]
        public void PropertyPathHasTheRightAmountOfParts(string path, int partCount)
        {
            var propertyPath = new PropertyPath(path);
            Assert.That(propertyPath.PartsCount, Is.EqualTo(partCount));
        }

        [Test]
        [TestCase("Foo[0]", 0)]
        [TestCase("Foo[1]", 1)]
        [TestCase("Foo.Bar[2]", 2)]
        [TestCase("Foo.Bar[12]", 12)]
        [TestCase("Foo[0].Foo[1].Foo[2].Foo[3].Foo[4]", 0, 1, 2, 3, 4)]
        public void PropertyPathMapsListIndicesCorrectly(string path, params int[] indices)
        {
            var propertyPath = new PropertyPath(path);
            var listIndex = 0;
            for (var i = 0; i < propertyPath.PartsCount; ++i)
            {
                var part = propertyPath[i];
                if (part.IsListItem)
                {
                    Assert.That(part.Index, Is.EqualTo(indices[listIndex]));
                    ++listIndex;
                }
            }
        }
        
        [Test]
        [TestCase("Foo[-1]")]
        [TestCase("Foo.Bar[-20]")]
        public void ThrowsWhenUsingNegativeIndices(string path)
        {
            Assert.That(() => CreateFromString(path), Throws.ArgumentException);
        }
        
        [Test]
        [TestCase("Foo[lol]")]
        [TestCase("Foo.Bar[\"one\"]")]
        public void ThrowsWhenUsingNonNumericIndices(string path)
        {
            Assert.That(() => CreateFromString(path), Throws.ArgumentException);
        }
        
        [Test]
        [TestCase("[0]")]
        [TestCase("[1].Bar")]
        public void ThrowsOnUsingIndicesAtPathRoot(string path)
        {
            Assert.That(() => CreateFromString(path), Throws.ArgumentException);
        }

        [Test]
        public void CanSetValueAtPath()
        {
            var container = new PropertyPathTestContainer();
            Assert.That(container.Float, Is.EqualTo(5.0f));
            Assert.That(container.Strings, Is.EqualTo(new []{"one", "two", "three"}));
            Assert.That(container.Nested.Int, Is.EqualTo(15));
            Assert.That(container.Nested.Doubles, Is.EqualTo(new []{1.0, 2.0, 3.0}));
            
            PropertyContainer.SetValueAtPath(ref container, new PropertyPath("Float"), 20.0f);
            Assert.That(container.Float, Is.EqualTo(20.0f));
            
            PropertyContainer.SetValueAtPath(ref container, new PropertyPath("Strings[1]"), "four");
            Assert.That(container.Strings, Is.EqualTo(new []{"one", "four", "three"}));
            
            PropertyContainer.SetValueAtPath(ref container, new PropertyPath("Nested.Int"), 5);
            Assert.That(container.Nested.Int, Is.EqualTo(5));
            
            PropertyContainer.SetValueAtPath(ref container, new PropertyPath("Nested.Doubles[2]"), 6.0);
            Assert.That(container.Nested.Doubles, Is.EqualTo(new []{1.0, 2.0, 6.0}));
        }
        
        [Test]
        public void CanTrySetValueAtPath()
        {
            var container = new PropertyPathTestContainer();
            Assert.That(container.Float, Is.EqualTo(5.0f));
            Assert.That(container.Strings, Is.EqualTo(new []{"one", "two", "three"}));
            Assert.That(container.Nested.Int, Is.EqualTo(15));
            Assert.That(container.Nested.Doubles, Is.EqualTo(new []{1.0, 2.0, 3.0}));
            
            Assert.That(PropertyContainer.TrySetValueAtPath(ref container, new PropertyPath("Float"), 20.0f), Is.EqualTo(true));
            Assert.That(container.Float, Is.EqualTo(20.0f));
            
            Assert.That(PropertyContainer.TrySetValueAtPath(ref container, new PropertyPath("Strings[1]"), "four"), Is.EqualTo(true));
            Assert.That(container.Strings, Is.EqualTo(new []{"one", "four", "three"}));
            
            Assert.That(PropertyContainer.TrySetValueAtPath(ref container, new PropertyPath("Nested.Int"), 5), Is.EqualTo(true));
            Assert.That(container.Nested.Int, Is.EqualTo(5));
            
            Assert.That(PropertyContainer.TrySetValueAtPath(ref container, new PropertyPath("Nested.Doubles[2]"), 6.0), Is.EqualTo(true));
            Assert.That(container.Nested.Doubles, Is.EqualTo(new []{1.0, 2.0, 6.0}));
        }
        
        [Test]
        public void CannotSetValueAtPath()
        {
            var container = new PropertyPathTestContainer();
            Assert.That(container.Float, Is.EqualTo(5.0f));
            Assert.That(container.Strings, Is.EqualTo(new []{"one", "two", "three"}));
            Assert.That(container.Nested.Int, Is.EqualTo(15));
            Assert.That(container.Nested.Doubles, Is.EqualTo(new []{1.0, 2.0, 3.0}));

            Assert.Throws<ArgumentException>(() => PropertyContainer.SetValueAtPath(ref container, new PropertyPath("Floats"), 20.0f));
            Assert.That(container.Float, Is.EqualTo(5.0f));
            
            Assert.Throws<InvalidCastException>(() => PropertyContainer.SetValueAtPath(ref container, new PropertyPath("Float"), VisitErrorCode.Ok));
            Assert.That(container.Float, Is.EqualTo(5.0f));

            Assert.Throws<ArgumentException>(() => PropertyContainer.SetValueAtPath(ref container, new PropertyPath("String[1]"), "four"));
            Assert.That(container.Strings, Is.EqualTo(new []{"one", "two", "three"}));
            
            Assert.Throws<InvalidCastException>(() => PropertyContainer.SetValueAtPath(ref container, new PropertyPath("Strings[1]"), VisitErrorCode.Ok));
            Assert.That(container.Strings, Is.EqualTo(new []{"one", "two", "three"}));
            
            Assert.Throws<ArgumentException>(() => PropertyContainer.SetValueAtPath(ref container, new PropertyPath("Nested.Isnt"), 5));
            Assert.That(container.Nested.Int, Is.EqualTo(15));
            
            Assert.Throws<InvalidCastException>(() => PropertyContainer.SetValueAtPath(ref container, new PropertyPath("Nested.Int"), VisitErrorCode.Ok));
            Assert.That(container.Nested.Int, Is.EqualTo(15));

            Assert.Throws<ArgumentException>(() => PropertyContainer.SetValueAtPath(ref container, new PropertyPath("Nested.Doublses[2]"), 6.0));
            Assert.That(container.Nested.Doubles, Is.EqualTo(new []{1.0, 2.0, 3.0}));
            
            Assert.Throws<InvalidCastException>(() => PropertyContainer.SetValueAtPath(ref container, new PropertyPath("Nested.Doubles[2]"), VisitErrorCode.Ok));
            Assert.That(container.Nested.Doubles, Is.EqualTo(new []{1.0, 2.0, 3.0}));
        }
        
        [Test]
        public void CannotTrySetValueAtPath()
        {
            var container = new PropertyPathTestContainer();
            Assert.That(container.Float, Is.EqualTo(5.0f));
            Assert.That(container.Strings, Is.EqualTo(new []{"one", "two", "three"}));
            Assert.That(container.Nested.Int, Is.EqualTo(15));
            Assert.That(container.Nested.Doubles, Is.EqualTo(new []{1.0, 2.0, 3.0}));

            Assert.That(PropertyContainer.TrySetValueAtPath(ref container, new PropertyPath("Floats"), 20.0f), Is.EqualTo(false));
            Assert.That(container.Float, Is.EqualTo(5.0f));
            
            Assert.That(PropertyContainer.TrySetValueAtPath(ref container, new PropertyPath("Float"), VisitErrorCode.Ok), Is.EqualTo(false));
            Assert.That(container.Float, Is.EqualTo(5.0f));

            Assert.That(PropertyContainer.TrySetValueAtPath(ref container, new PropertyPath("String[1]"), "four"), Is.EqualTo(false));
            Assert.That(container.Strings, Is.EqualTo(new []{"one", "two", "three"}));
            
            Assert.That(PropertyContainer.TrySetValueAtPath(ref container, new PropertyPath("Strings[1]"), VisitErrorCode.Ok), Is.EqualTo(false));
            Assert.That(container.Strings, Is.EqualTo(new []{"one", "two", "three"}));
            
            Assert.That(PropertyContainer.TrySetValueAtPath(ref container, new PropertyPath("Nested.Isnt"), 5), Is.EqualTo(false));
            Assert.That(container.Nested.Int, Is.EqualTo(15));
            
            Assert.That(PropertyContainer.TrySetValueAtPath(ref container, new PropertyPath("Nested.Int"), VisitErrorCode.Ok), Is.EqualTo(false));
            Assert.That(container.Nested.Int, Is.EqualTo(15));

            Assert.That(PropertyContainer.TrySetValueAtPath(ref container, new PropertyPath("Nested.Doublses[2]"), 6.0), Is.EqualTo(false));
            Assert.That(container.Nested.Doubles, Is.EqualTo(new []{1.0, 2.0, 3.0}));
            
            Assert.That(PropertyContainer.TrySetValueAtPath(ref container, new PropertyPath("Nested.Doubles[2]"), VisitErrorCode.Ok), Is.EqualTo(false));
            Assert.That(container.Nested.Doubles, Is.EqualTo(new []{1.0, 2.0, 3.0}));
        }
        
        [Test]
        public void CanGetValueAtPath()
        {
            var container = new PropertyPathTestContainer();
            Assert.That(container.Float, Is.EqualTo(5.0f));
            Assert.That(container.Strings, Is.EqualTo(new []{"one", "two", "three"}));
            Assert.That(container.Nested.Int, Is.EqualTo(15));
            Assert.That(container.Nested.Doubles, Is.EqualTo(new []{1.0, 2.0, 3.0}));

            container.Float = 20.0f;
            Assert.That(PropertyContainer.GetValueAtPath<PropertyPathTestContainer, float>(ref container, new PropertyPath("Float")), Is.EqualTo(container.Float));

            container.Strings[1] = "four";
            Assert.That(PropertyContainer.GetValueAtPath<PropertyPathTestContainer, string>(ref container, new PropertyPath("Strings[1]")), Is.EqualTo("four"));

            container.Nested.Int = 5;
            
            Assert.That(PropertyContainer.GetValueAtPath<PropertyPathTestContainer, int>(ref container, new PropertyPath("Nested.Int")), Is.EqualTo(container.Nested.Int));

            container.Nested.Doubles[2] = 6.0;
            Assert.That(PropertyContainer.GetValueAtPath<PropertyPathTestContainer, double>(ref container, new PropertyPath("Nested.Doubles[2]")), Is.EqualTo(6.0));
        }
        
        [Test]
        public void TryCanGetValueAtPath()
        {
            var container = new PropertyPathTestContainer();
            Assert.That(container.Float, Is.EqualTo(5.0f));
            Assert.That(container.Strings, Is.EqualTo(new []{"one", "two", "three"}));
            Assert.That(container.Nested.Int, Is.EqualTo(15));
            Assert.That(container.Nested.Doubles, Is.EqualTo(new []{1.0, 2.0, 3.0}));

            container.Float = 20.0f;
            {
                Assert.That(PropertyContainer.TryGetValueAtPath<PropertyPathTestContainer, float>(ref container, new PropertyPath("Float"), out var value), Is.EqualTo(true));
                Assert.That(value, Is.EqualTo(container.Float));
            }
            container.Strings[1] = "four";
            {
                Assert.That(PropertyContainer.TryGetValueAtPath<PropertyPathTestContainer, string>(ref container, new PropertyPath("Strings[1]"), out var value), Is.EqualTo(true));
                Assert.That(value, Is.EqualTo("four"));
            }
            container.Nested.Int = 5;
            {
                Assert.That(PropertyContainer.TryGetValueAtPath<PropertyPathTestContainer, int>(ref container, new PropertyPath("Nested.Int"), out var value), Is.EqualTo(true));
                Assert.That(value, Is.EqualTo(container.Nested.Int));
            }
            container.Nested.Doubles[2] = 6.0;
            {
                Assert.That(PropertyContainer.TryGetValueAtPath<PropertyPathTestContainer, double>(ref container, new PropertyPath("Nested.Doubles[2]"), out var value), Is.EqualTo(true));
                Assert.That(value, Is.EqualTo(6.0));
            }
        }
        
        [Test]
        public void CannotGetValueAtPath()
        {
            var container = new PropertyPathTestContainer();
            Assert.That(container.Float, Is.EqualTo(5.0f));
            Assert.That(container.Strings, Is.EqualTo(new []{"one", "two", "three"}));
            Assert.That(container.Nested.Int, Is.EqualTo(15));
            Assert.That(container.Nested.Doubles, Is.EqualTo(new []{1.0, 2.0, 3.0}));

            container.Float = 20.0f;
            Assert.Throws<ArgumentException>(() => PropertyContainer.GetValueAtPath<PropertyPathTestContainer, float>(ref container, new PropertyPath("Floatd")));
            Assert.Throws<InvalidCastException>(() => PropertyContainer.GetValueAtPath<PropertyPathTestContainer, VisitErrorCode>(ref container, new PropertyPath("Float")));

            container.Strings[1] = "four";
            Assert.Throws<ArgumentException>(() => PropertyContainer.GetValueAtPath<PropertyPathTestContainer, string>(ref container, new PropertyPath("String[1]")));
            Assert.Throws<InvalidCastException>(() => PropertyContainer.GetValueAtPath<PropertyPathTestContainer, VisitErrorCode>(ref container, new PropertyPath("Strings[1]")));

            container.Nested.Int = 5;
            
            Assert.Throws<ArgumentException>(() => PropertyContainer.GetValueAtPath<PropertyPathTestContainer, int>(ref container, new PropertyPath("Nested.Ints")));
            Assert.Throws<InvalidCastException>(() => PropertyContainer.GetValueAtPath<PropertyPathTestContainer, PropertyPathTests>(ref container, new PropertyPath("Nested.Int")));

            container.Nested.Doubles[2] = 6.0;
            Assert.Throws<ArgumentException>(() => PropertyContainer.GetValueAtPath<PropertyPathTestContainer, double>(ref container, new PropertyPath("Nested.Double[2]")));
            Assert.Throws<InvalidCastException>(() => PropertyContainer.GetValueAtPath<PropertyPathTestContainer, VisitErrorCode>(ref container, new PropertyPath("Nested.Doubles[2]")));
        }
        
        [Test]
        public void CannotTryGetValueAtPath()
        {
            var container = new PropertyPathTestContainer();
            Assert.That(container.Float, Is.EqualTo(5.0f));
            Assert.That(container.Strings, Is.EqualTo(new []{"one", "two", "three"}));
            Assert.That(container.Nested.Int, Is.EqualTo(15));
            Assert.That(container.Nested.Doubles, Is.EqualTo(new []{1.0, 2.0, 3.0}));

            container.Float = 20.0f;
            {
                Assert.That(PropertyContainer.TryGetValueAtPath<PropertyPathTestContainer, float>(ref container, new PropertyPath("Floats"), out var value), Is.EqualTo(false));
            }
            container.Strings[1] = "four";
            {
                Assert.That(PropertyContainer.TryGetValueAtPath<PropertyPathTestContainer, string>(ref container, new PropertyPath("String[1]"), out var value), Is.EqualTo(false));
            }
            container.Nested.Int = 5;
            {
                Assert.That(PropertyContainer.TryGetValueAtPath<PropertyPathTestContainer, int>(ref container, new PropertyPath("Nested.Ints"), out var value), Is.EqualTo(false));
            }
            container.Nested.Doubles[2] = 6.0;
            {
                Assert.That(PropertyContainer.TryGetValueAtPath<PropertyPathTestContainer, double>(ref container, new PropertyPath("Nested.Dobles[2]"), out var value), Is.EqualTo(false));
            }
        }

        [Test]
        public void CanGetCountAtPath()
        {
            var container = new PropertyPathTestContainer();
            container.Strings.Clear();
            container.Strings.AddRange(new []{"This", "Has", "Five", "Items", "!"});
            
            container.Nested.Doubles.Clear();
            container.Nested.Doubles.AddRange(new []{1.0, 2.0, 3.0});
            
            Assert.That(PropertyContainer.GetCountAtPath(ref container, new PropertyPath("Strings")), Is.EqualTo(5));
            Assert.That(PropertyContainer.GetCountAtPath(ref container, new PropertyPath("Nested.Doubles")), Is.EqualTo(3));
        }

        [Test]
        [TestCase("Strings[0]", false, -1)]
        [TestCase("Strings", true, 3)]
        [TestCase("Strins", false, -1)]
        [TestCase("Nested.VeryNested.Doubles", true, 5)]
        [TestCase("Nested.VeryNested.Doubes", false, -1)]
        [TestCase("MultiNested[1].VeryNested.Doubles", true, 5)]
        [TestCase("MultiNested[1].VeryNested.Doubes", false, -1)]
        public void TryCanGetCountAtPath(string path, bool expected, int expectedCount)
        {
            var container = new PropertyPathTestContainer();
            var propertyPath = new PropertyPath(path);
            Assert.That(PropertyContainer.TryGetCountAtPath(ref container, propertyPath, out var count), Is.EqualTo(expected));
            Assert.That(count == expectedCount);
        }
        
        [Test]
        public void CannotGetCountAtPath()
        {
            var container = new PropertyPathTestContainer();
            container.Strings.Clear();
            container.Strings.AddRange(new []{"This", "Has", "Five", "Items", "!"});
            
            container.Nested.Doubles.Clear();
            container.Nested.Doubles.AddRange(new []{1.0, 2.0, 3.0});
            
            Assert.Throws<ArgumentException>(() => PropertyContainer.GetCountAtPath(ref container, new PropertyPath("String")));
            Assert.Throws<ArgumentException>(() => PropertyContainer.GetCountAtPath(ref container, new PropertyPath("Nested.Double")));
        }

        [Test]
        public void CanSetCountAtPath()
        {
            var container = new PropertyPathTestContainer();
            container.Strings.Clear();
            container.Strings.AddRange(new []{"This", "Has", "Five", "Items", "!"});

            PropertyContainer.SetCountAtPath(ref container, new PropertyPath("Strings"), 3);
            Assert.That(container.Strings.Count, Is.EqualTo(3));
            Assert.That(container.Strings[0], Is.EqualTo("This"));
            Assert.That(container.Strings[1], Is.EqualTo("Has"));
            Assert.That(container.Strings[2], Is.EqualTo("Five"));
            
            PropertyContainer.SetCountAtPath(ref container, new PropertyPath("Strings"), 4);
            Assert.That(container.Strings.Count, Is.EqualTo(4));
            Assert.That(container.Strings[3], Is.EqualTo(null));
            
            container.Nested.Doubles.Clear();
            container.Nested.Doubles.AddRange(new []{1.0, 2.0, 3.0});
            PropertyContainer.SetCountAtPath(ref container, new PropertyPath("Nested.Doubles"), 4);
            Assert.That(container.Nested.Doubles.Count, Is.EqualTo(4));
            Assert.That(container.Nested.Doubles[0], Is.EqualTo(1.0));
            Assert.That(container.Nested.Doubles[1], Is.EqualTo(2.0));
            Assert.That(container.Nested.Doubles[2], Is.EqualTo(3.0));
            Assert.That(container.Nested.Doubles[3], Is.EqualTo(0.0));
            Assert.That(() => PropertyContainer.SetCountAtPath(ref container, new PropertyPath("Nested.Double"), 4), Throws.ArgumentException);
        }
        
        [Test]
        public void CannotSetCountAtPath()
        {
            var container = new PropertyPathTestContainer();
            container.Strings.Clear();
            container.Strings.AddRange(new []{"This", "Has", "Five", "Items", "!"});

            Assert.Throws<ArgumentException>(()=> PropertyContainer.SetCountAtPath(ref container, new PropertyPath("Strisngs"), 3));
            
            container.Nested.Doubles.Clear();
            container.Nested.Doubles.AddRange(new []{1.0, 2.0, 3.0});
            Assert.Throws<ArgumentException>(()=> PropertyContainer.SetCountAtPath(ref container, new PropertyPath("Nested.Double"), 4));
        }

        [Test]
        public void CanTrySetCountAtPath()
        {
            var container = new PropertyPathTestContainer();
            container.Strings.Clear();
            container.Strings.AddRange(new []{"This", "Has", "Five", "Items", "!"});

            Assert.That(PropertyContainer.TrySetCountAtPath(ref container, new PropertyPath("Strings"), 3), Is.EqualTo(true));
            Assert.That(container.Strings.Count, Is.EqualTo(3));
            Assert.That(container.Strings[0], Is.EqualTo("This"));
            Assert.That(container.Strings[1], Is.EqualTo("Has"));
            Assert.That(container.Strings[2], Is.EqualTo("Five"));
            
            Assert.That(PropertyContainer.TrySetCountAtPath(ref container, new PropertyPath("Strings"), 4), Is.EqualTo(true));
            Assert.That(container.Strings.Count, Is.EqualTo(4));
            Assert.That(container.Strings[3], Is.EqualTo(null));
            
            container.Nested.Doubles.Clear();
            container.Nested.Doubles.AddRange(new []{1.0, 2.0, 3.0});
            Assert.That(PropertyContainer.TrySetCountAtPath(ref container, new PropertyPath("Nested.Doubles"), 4), Is.EqualTo(true));
            Assert.That(container.Nested.Doubles.Count, Is.EqualTo(4));
            Assert.That(container.Nested.Doubles[0], Is.EqualTo(1.0));
            Assert.That(container.Nested.Doubles[1], Is.EqualTo(2.0));
            Assert.That(container.Nested.Doubles[2], Is.EqualTo(3.0));
            Assert.That(container.Nested.Doubles[3], Is.EqualTo(0.0));
        }
        
        [Test]
        public void CannotTrySetCountAtPath()
        {
            var container = new PropertyPathTestContainer();
            container.Strings.Clear();
            container.Strings.AddRange(new []{"This", "Has", "Five", "Items", "!"});

            Assert.That(PropertyContainer.TrySetCountAtPath(ref container, new PropertyPath("Strisngs"), 3), Is.EqualTo(false));
            
            container.Nested.Doubles.Clear();
            container.Nested.Doubles.AddRange(new []{1.0, 2.0, 3.0});
            Assert.That(PropertyContainer.TrySetCountAtPath(ref container, new PropertyPath("Nested.Double"), 4), Is.EqualTo(false));
        }
        
        [Test]
        [TestCase("Float", true)]
        [TestCase("Strings", true)]
        [TestCase("Nested", true)]
        [TestCase("Strings[2]", true)]
        [TestCase("Nested.Int", true)]
        [TestCase("Nested.Doubles", true)]
        [TestCase("Nested.Double", false)]
        [TestCase("Nasted", false)]
        [TestCase("float", false)]
        [TestCase("Strings[3]", false)]
        [TestCase("Nested.VeryNested.Byte", true)]
        [TestCase("Nested.VeryNested.Byte", true)]
        [TestCase("Nested.VeryNested.Doubles[4]", true)]
        [TestCase("Nested.VeryNested.Doubles", true)]
        [TestCase("Nested.VeryNested.Doubles[5]", false)]
        [TestCase("Nsted.VeryNested.Byte", false)]
        [TestCase("Nested.VerNested.Byte", false)]
        [TestCase("Nested.VeryNested.sByte", false)]
        public void CanVisitAtPath(string path, bool expected)
        {
            var container = new PropertyPathTestContainer();
            var visitor = new PathVisitor();
            var changeTracker = new ChangeTracker();
            var propertyPath = new PropertyPath(path);
            if (expected)
            {
                PropertyContainer.VisitAtPath(ref container, propertyPath, visitor, ref changeTracker);
                var lastPart = propertyPath[propertyPath.PartsCount - 1];
                var name = lastPart.IsListItem ? $"[{lastPart.Index}]" : lastPart.Name;
                Assert.That(visitor.Last == name);
            }
            else
            {
                Assert.Throws<ArgumentException>(() => PropertyContainer.VisitAtPath(ref container, propertyPath, visitor, ref changeTracker));
            }
        }
        
        [Test]
        [TestCase("Float", true)]
        [TestCase("Strings", true)]
        [TestCase("Nested", true)]
        [TestCase("Strings[2]", true)]
        [TestCase("Nested.Int", true)]
        [TestCase("Nested.Doubles", true)]
        [TestCase("Nested.Double", false)]
        [TestCase("Nasted", false)]
        [TestCase("float", false)]
        [TestCase("Strings[3]", false)]
        [TestCase("Nested.VeryNested.Byte", true)]
        [TestCase("Nested.VeryNested.Byte", true)]
        [TestCase("Nested.VeryNested.Doubles[4]", true)]
        [TestCase("Nested.VeryNested.Doubles", true)]
        [TestCase("Nested.VeryNested.Doubles[5]", false)]
        [TestCase("Nsted.VeryNested.Byte", false)]
        [TestCase("Nested.VerNested.Byte", false)]
        [TestCase("Nested.VeryNested.sByte", false)]
        public void CanTryVisitAtPath(string path, bool expected)
        {
            var container = new PropertyPathTestContainer();
            var visitor = new PathVisitor();
            var changeTracker = new ChangeTracker();
            var propertyPath = new PropertyPath(path);
            Assert.That(PropertyContainer.TryVisitAtPath(ref container, propertyPath, visitor, ref changeTracker), Is.EqualTo(expected));
            if (expected)
            {
                var lastPart = propertyPath[propertyPath.PartsCount - 1];
                var name = lastPart.IsListItem ? $"[{lastPart.Index}]" : lastPart.Name;
                Assert.That(visitor.Last == name);
            }
        }
        
        private static PropertyPath CreateFromString(string path)
        {
            return new PropertyPath(path);
        }

        private class PathVisitor : PropertyVisitor
        {
            public string Last;   
            
            protected override VisitStatus Visit<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref TValue value,
                ref ChangeTracker changeTracker)
            {
                try
                {
                    return base.Visit(property, ref container, ref value, ref changeTracker);
                }
                finally
                {
                    Last = property.GetName();
                }
            }

            protected override void EndCollection<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref TValue value,
                ref ChangeTracker changeTracker)
            {
                base.EndCollection(property, ref container, ref value, ref changeTracker);
                Last = property.GetName();
            }

            protected override void EndContainer<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref TValue value,
                ref ChangeTracker changeTracker)
            {
                base.EndContainer(property, ref container, ref value, ref changeTracker);
                Last = property.GetName();
            }
        }
    }
}