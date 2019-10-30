using System.Runtime.CompilerServices;
using Unity.Burst;

namespace Burst.Compiler.IL.Tests
{
    internal class Functions
    {
        [TestCompiler]
        public static int CheckFunctionCall()
        {
            return AnotherFunction();
        }

        private static int AnotherFunction()
        {
            return 150;
        }

        [TestCompiler(ExpectCompilerException = true, ExpectedDiagnosticId = DiagnosticId.ERR_UnableToAccessManagedMethod)]
        public static void Boxing()
        {
            var a = new CustomStruct();
            // This will box CustomStruct, so this method should fail when compiling
            a.GetType();
        }

        private struct CustomStruct
        {

        }

        public static int NotDiscardable()
        {
            return 3;
        }

        [BurstDiscard]
        public static void Discardable()
        {
        }

        [TestCompiler]
        public static int TestCallsOfDiscardedMethodRegression()
        {
            // The regression was that we would queue all calls of a method, but if we encountered a discardable one
            // We would stop visiting pending methods. This resulting in method bodies not being visited.
            Discardable();
            return NotDiscardable();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static int NoInline(int x)
        {
            return x;
        }

        [TestCompiler(42)]
        public static int TestNoInline(int x)
        {
            return NoInline(x);
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public static int NoOptimization(int x)
        {
            return x;
        }

        [TestCompiler(42)]
        public static int TestNoOptimization(int x)
        {
            return NoOptimization(x);
        }
    }
}
