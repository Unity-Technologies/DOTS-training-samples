using System;
using NUnit.Framework;

namespace Unity.Properties.Tests
{
    class TypeConstructionTests
    {
        [Test]
        public void CanBeConstructedTests()
        {
            Assert.That(TypeConstruction.CanBeConstructed<IConstructInterface>(), Is.False);
            Assert.That(TypeConstruction.CanBeConstructed<AbstractConstructibleBaseType>(), Is.False);
            Assert.That(TypeConstruction.CanBeConstructed<ConstructibleBaseType>(), Is.True);
            Assert.That(TypeConstruction.CanBeConstructed<ConstructibleDerivedType>(), Is.True);
            Assert.That(TypeConstruction.CanBeConstructed<NonConstructibleDerivedType>(), Is.False);
            Assert.That(TypeConstruction.CanBeConstructed<NoConstructorType>(), Is.True);
            Assert.That(TypeConstruction.CanBeConstructed<ParameterLessConstructorType>(), Is.True);
            Assert.That(TypeConstruction.CanBeConstructed<ParameterConstructorType>(), Is.False);
            Assert.That(TypeConstruction.CanBeConstructed<ScriptableObjectType>(), Is.True);
        }
        
        [Test]
        public void CanBeConstructedTestsFromDerivedTypes()
        {
            Assert.That(TypeConstruction.CanBeConstructedFromDerivedType<IConstructInterface>(), Is.True);
            Assert.That(TypeConstruction.CanBeConstructedFromDerivedType<AbstractConstructibleBaseType>(), Is.True);
            Assert.That(TypeConstruction.CanBeConstructedFromDerivedType<ConstructibleBaseType>(), Is.True);
            Assert.That(TypeConstruction.CanBeConstructedFromDerivedType<ConstructibleDerivedType>(), Is.False);
            Assert.That(TypeConstruction.CanBeConstructedFromDerivedType<NonConstructibleDerivedType>(), Is.False);
            Assert.That(TypeConstruction.CanBeConstructedFromDerivedType<NoConstructorType>(), Is.False);
            Assert.That(TypeConstruction.CanBeConstructedFromDerivedType<ParameterLessConstructorType>(), Is.False);
            Assert.That(TypeConstruction.CanBeConstructedFromDerivedType<ParameterConstructorType>(), Is.False);
            Assert.That(TypeConstruction.CanBeConstructedFromDerivedType<ScriptableObjectType>(), Is.False);
        }

        [Test]
        public void ReturnsAnActualInstanceTests()
        {
            {
                var instance = TypeConstruction.Construct<ConstructibleBaseType>();
                Assert.That(instance, Is.Not.Null);
                Assert.That(instance.Value, Is.EqualTo(25.0f));
            }

            {
                var instance = TypeConstruction.Construct<ConstructibleDerivedType>();
                Assert.That(instance, Is.Not.Null);
                Assert.That(instance.Value, Is.EqualTo(25.0f));
                Assert.That(instance.SubValue, Is.EqualTo(50.0f));
            }
            
            {
                var instance = TypeConstruction.Construct<ConstructibleDerivedType>();
                Assert.That(instance, Is.Not.Null);
                Assert.That(instance.Value, Is.EqualTo(25.0f));
                Assert.That(instance.SubValue, Is.EqualTo(50.0f));
            }

            {
                var instance = TypeConstruction.Construct<ScriptableObjectType>();
                Assert.That(instance, Is.Not.Null);
                Assert.That(instance, Is.Not.False);
            }
        }
        
        [Test]
        public void ReturnsAnActualInstanceOfDerivedTypeTests()
        {
            {
                var instance = TypeConstruction.Construct<ConstructibleBaseType>(typeof(ConstructibleDerivedType));
                Assert.That(instance, Is.Not.Null);
                Assert.That(instance, Is.TypeOf<ConstructibleDerivedType>());
                Assert.That(instance.Value, Is.EqualTo(25.0f));
                Assert.That((instance as ConstructibleDerivedType).SubValue, Is.EqualTo(50.0f));
            }

            {
                var instance = TypeConstruction.Construct<ConstructibleDerivedType>();
                Assert.That(instance, Is.Not.Null);
                Assert.That(instance.Value, Is.EqualTo(25.0f));
                Assert.That(instance.SubValue, Is.EqualTo(50.0f));
            }
            
            {
                var instance = TypeConstruction.Construct<ConstructibleDerivedType>();
                Assert.That(instance, Is.Not.Null);
                Assert.That(instance.Value, Is.EqualTo(25.0f));
                Assert.That(instance.SubValue, Is.EqualTo(50.0f));
            }
        }

        [Test]
        public void ConstructionOfInvalidTypeThrows()
        {
            Assert.Throws<InvalidOperationException>(() => TypeConstruction.Construct<IConstructInterface>());
            Assert.Throws<InvalidOperationException>(() => TypeConstruction.Construct<AbstractConstructibleBaseType>());
            Assert.Throws<InvalidOperationException>(() => TypeConstruction.Construct<NonConstructibleDerivedType>());
            Assert.Throws<InvalidOperationException>(() => TypeConstruction.Construct<ParameterConstructorType>());
        }
        
        [Test]
        public void ConstructionOfInvalidDerivedTypeThrows()
        {
            Assert.Throws<ArgumentException>(() => TypeConstruction.Construct<IConstructInterface>(typeof(NonConstructibleDerivedType)));
            Assert.Throws<ArgumentException>(() => TypeConstruction.Construct<IConstructInterface>(typeof(ParameterLessConstructorType)));
        }

        [Test]
        public void CanSetExplicitConstruction()
        {
            Assert.That(TypeConstruction.CanBeConstructed<ParameterConstructorType>(), Is.False);
            TypeConstruction.SetExplicitConstructionMethod(ExplicitConstruction);
            Assert.That(TypeConstruction.CanBeConstructed<ParameterConstructorType>(), Is.True);
            {
                var instance = TypeConstruction.Construct<ParameterConstructorType>();
                Assert.That(instance, Is.Not.Null);
                Assert.That(instance.Value, Is.EqualTo(10.0f));
            }
            
            TypeConstruction.UnsetExplicitConstructionMethod(ExplicitConstruction);
            Assert.That(TypeConstruction.CanBeConstructed<ParameterConstructorType>(), Is.False);
        }

        [Test]
        public void TryConstruct_DoesNotThrow()
        {
            Assert.That(TypeConstruction.TryConstruct<ParameterLessConstructorType>(out var _), Is.True);
            Assert.That(TypeConstruction.TryConstruct<ParameterConstructorType>(out var _), Is.False);
        }

        [Test]
        public void TryConstruct_DerivedType_DoesNotThrow()
        {
            Assert.That(TypeConstruction.TryConstruct<ConstructibleBaseType>(typeof(ConstructibleDerivedType), out var _), Is.True);
            Assert.That(TypeConstruction.TryConstruct<ConstructibleBaseType>(typeof(NonConstructibleDerivedType), out var _), Is.False);
        }

        private static ParameterConstructorType ExplicitConstruction()
        {
            return new ParameterConstructorType(10.0f);
        }
    }
}
