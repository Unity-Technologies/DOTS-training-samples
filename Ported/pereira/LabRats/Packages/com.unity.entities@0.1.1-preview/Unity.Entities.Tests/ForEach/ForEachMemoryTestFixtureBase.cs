#if false // !UNITY_DOTSPLAYER
using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Unity.Entities.Tests.ForEach
{
    class ForEachMemoryTestFixtureBase : EntityQueryBuilderTestFixture
    {
        protected int m_Field;
        protected static int s_Static;

        [SetUp]
        public void SetUp()
        {
            m_Field = 3;
            s_Static = 4;
        }

        [TearDown]
        public new void TearDown()
        {
            // this code is necessary to make our tests re-runnable without requiring a domain reload.
            // lambdas only provide code, and we're operating with delegates in ForEach, which require
            // an alloc to create. the c# compiler generates code to alloc the delegate on first use,
            // and then stores it in a static for later reuse. we need to clear those statics in order
            // to rerun the tests.

            var cache = GetType().GetNestedType("<>c", BindingFlags.NonPublic);
            if (cache != null)
            {
                foreach (var field in cache
                    .GetFields(BindingFlags.Static | BindingFlags.Public)
                    .Where(f => typeof(MulticastDelegate).IsAssignableFrom(f.FieldType)))
                {
                    field.SetValue(null, null);
                }
            }
        }

        protected delegate void TestDelegate(int i);

        protected void TestInvoke(TestDelegate op)
        {
            // simulate more than one iteration
            op(1);
            op(1);
        }
    }
}
#endif // !UNITY_DOTSPLAYER
