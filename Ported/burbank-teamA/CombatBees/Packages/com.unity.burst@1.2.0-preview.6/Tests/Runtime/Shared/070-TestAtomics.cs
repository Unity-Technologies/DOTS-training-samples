using System;
using System.Threading;

namespace Burst.Compiler.IL.Tests
{
    /// <summary>
    /// Tests of the <see cref="Interlocked"/> functions.
    /// </summary>
    internal class TestAtomics
    {
        [TestCompiler(1)]
        [TestCompiler(-1)]
        public static int test_atomic_increment_int(ref int value)
        {
            return Interlocked.Increment(ref value);
        }

        [TestCompiler(1L)]
        [TestCompiler(-1L)]
        public static long test_atomic_increment_long(ref long value)
        {
            return Interlocked.Increment(ref value);
        }

        [TestCompiler(1)]
        [TestCompiler(-1)]
        public static int test_atomic_add_int(ref int value)
        {
            return Interlocked.Add(ref value, 2);
        }

        [TestCompiler(1L)]
        [TestCompiler(-1L)]
        public static long test_atomic_add_long(ref long value)
        {
            return Interlocked.Add(ref value, 2);
        }

        [TestCompiler(1, 2, 1)]
        [TestCompiler(1, 10, 1)]
        [TestCompiler(1, 2, 2)]
        [TestCompiler(7, 2, 1)]
        [TestCompiler(7, 10, 1)]
        [TestCompiler(7, 2, 2)]
        public static int test_atomic_compare_and_exchange_int(ref int location, int value, int compareAnd)
        {
            return Interlocked.CompareExchange(ref location, value, compareAnd);
        }

        [TestCompiler(1L, 2L, 1L)]
        [TestCompiler(1L, 10L, 1L)]
        [TestCompiler(1L, 2L, 2L)]
        [TestCompiler(7L, 2L, 1L)]
        [TestCompiler(7L, 10L, 1L)]
        [TestCompiler(7L, 2L, 2L)]
        public static long test_atomic_compare_and_exchange_long(ref long location, long value, long compareAnd)
        {
            return Interlocked.CompareExchange(ref location, value, compareAnd);
        }

        [TestCompiler(1)]
        [TestCompiler(-1)]
        public static int test_atomic_decrement_int(ref int value)
        {
            return Interlocked.Decrement(ref value);
        }

        [TestCompiler(1L)]
        [TestCompiler(-1L)]
        public static long test_atomic_decrement_long(ref long value)
        {
            return Interlocked.Decrement(ref value);
        }

        [TestCompiler(1)]
        public static int test_atomic_exchange_int(ref int value)
        {
            return Interlocked.Exchange(ref value, 5);
        }

        [TestCompiler(1L)]
        public static long test_atomic_exchange_long(ref long value)
        {
            return Interlocked.Exchange(ref value, 5);
        }

        [TestCompiler(1)]
        public static IntPtr ExchangeIntPtr(IntPtr value)
        {
            return Interlocked.Exchange(ref value, new IntPtr(5));
        }

        [TestCompiler(1, 2, 1)]
        [TestCompiler(1, 10, 1)]
        [TestCompiler(1, 2, 2)]
        public static IntPtr CompareExchangeIntPtr(IntPtr location, IntPtr value, IntPtr compareAnd)
        {
            return Interlocked.CompareExchange(ref location, value, compareAnd);
        }

        [TestCompiler]
        public static void test_atomic_memorybarrier()
        {
            Interlocked.MemoryBarrier();
        }

        [TestCompiler(0)]
        public static int Case1111040(int val)
        {
            int test = val;
            Interlocked.Increment(ref test);
            Interlocked.Decrement(ref test);
            return test;
        }
    }
}
