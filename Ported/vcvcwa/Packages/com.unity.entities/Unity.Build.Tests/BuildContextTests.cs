using NUnit.Framework;

namespace Unity.Build.Tests
{
    class BuildContextTests
    {
        class TestA { }
        class TestB { }

        [Test]
        public void CanSetContextObjectWithConstructor()
        {
            var context = new BuildContext(new TestA());
            Assert.IsNotNull(context.Get<TestA>());
        }

        [Test]
        public void CanSetContextWithObject()
        {
            var context = new BuildContext();
            context.Set(new TestA());
            Assert.IsNotNull(context.Get<TestA>());
        }

        [Test]
        public void SetWithExistingType()
        {
            var context = new BuildContext();
            var instance1 = new TestA();
            var instance2 = new TestA();
            context.Set(instance1);
            Assert.That(context.Get<TestA>(), Is.EqualTo(instance1));
            context.Set(instance2);
            Assert.That(context.Get<TestA>(), Is.EqualTo(instance2));
        }

        [Test]
        public void CanSetAndRemove()
        {
            var context = new BuildContext();
            context.Set(new TestA());
            Assert.IsNotNull(context.Get<TestA>());
            context.Remove<TestA>();
            Assert.IsNull(context.Get<TestA>());
        }

        [Test]
        public void GetOrCreateSucceedsWhenObjectNotPresent()
        {
            var context = new BuildContext();
            Assert.IsNotNull(context.GetOrCreate<TestA>());
        }

        [Test]
        public void GetFailsWhenObjectNotPresent()
        {
            var context = new BuildContext();
            Assert.IsNull(context.Get<TestA>());
        }

        [Test]
        public void GetOrCreateSucceedsWhenObjectPresent()
        {
            var context = new BuildContext();
            context.Set(new TestA());
            Assert.IsNotNull(context.GetOrCreate<TestA>());
        }

        [Test]
        public void SetAllAddsAllObjects()
        {
            var context = new BuildContext(new TestA(), new TestB());
            Assert.IsNotNull(context.Get<TestA>());
            Assert.IsNotNull(context.Get<TestB>());
        }

        [Test]
        public void SetAllSkipsNullObjects()
        {
            var context = new BuildContext(new TestA(), new TestB(), null);
            Assert.That(context.GetAll().Count, Is.EqualTo(2));
        }

        [Test]
        public void SetAllSkipsDuplicateObjects()
        {
            var context = new BuildContext(new TestA(), new TestB(), new TestA());
            Assert.That(context.GetAll().Count, Is.EqualTo(2));
        }
    }
}
