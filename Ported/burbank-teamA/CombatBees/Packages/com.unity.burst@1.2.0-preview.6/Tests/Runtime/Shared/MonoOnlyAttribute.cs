using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace Burst.Compiler.IL.Tests
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class MonoOnlyAttribute : IgnoreAttribute, IApplyToTest
    {
#pragma warning disable CS0414
        public MonoOnlyAttribute(string reason) : base(reason)
        {
        }
#pragma warning restore CS0414

        void IApplyToTest.ApplyToTest(Test test)
        {
            if (Type.GetType("Mono.Runtime") == null)
            {
                base.ApplyToTest(test);
            }
        }
    }
}
