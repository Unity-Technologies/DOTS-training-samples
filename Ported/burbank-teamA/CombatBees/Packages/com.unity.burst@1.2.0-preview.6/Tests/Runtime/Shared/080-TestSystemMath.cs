using System;
using Burst.Compiler.IL.Tests.Helpers;
using NUnit.Framework;

namespace Burst.Compiler.IL.Tests
{
    /// <summary>
    /// Tests of the <see cref="System.Math"/> functions.
    /// </summary>
    internal class TestSystemMath
    {
        // TODO: Make all these tests args more automatic and shared with ranges, inf, nan...etc.

        [TestCompiler(DataRange.Standard)]
        public static double TestCos(float value)
        {
            return Math.Cos(value);
        }

        [TestCompiler(DataRange.Standard)]
        public static double TestSin(float value)
        {
            return Math.Sin(value);
        }

        [TestCompiler(DataRange.Standard)]
        public static float TestTan(float value)
        {
            return (float) Math.Tan(value);
        }

        [TestCompiler(DataRange.Standard11)]
        public static double TestAcos(float value)
        {
            return Math.Acos(value);
        }

        [TestCompiler(DataRange.Standard11)]
        public static double TestAsin(float value)
        {
            return Math.Asin(value);
        }

        [TestCompiler(DataRange.Standard11)]
        public static float TestAtan(float value)
        {
            return (float)Math.Atan(value);
        }

        [TestCompiler(DataRange.Standard)]
        public static double TestCosh(float value)
        {
            return Math.Cosh(value);
        }

        [TestCompiler(DataRange.Standard)]
        public static double TestSinh(float value)
        {
            return Math.Sinh(value);
        }

        [TestCompiler(DataRange.Standard)]
        public static float TestTanh(float value)
        {
            return (float)Math.Tanh(value);
        }

        [TestCompiler(DataRange.StandardPositive)]
        public static double TestSqrt(float value)
        {
            return Math.Sqrt(value);
        }

        [TestCompiler(DataRange.StandardPositive & ~DataRange.Zero)]
        public static double TestLog(float value)
        {
            return Math.Log(value);
        }

        [TestCompiler(DataRange.StandardPositive & ~DataRange.Zero)]
        public static double TestLog10(float value)
        {
            return Math.Log10(value);
        }

        [TestCompiler(DataRange.StandardPositive)]
        public static double TestExp(float value)
        {
            return Math.Exp(value);
        }

        [TestCompiler(DataRange.Standard & ~(DataRange.Zero|DataRange.NaN), DataRange.Standard)]
        [TestCompiler(DataRange.Standard & ~DataRange.Zero, DataRange.Standard & ~DataRange.Zero)]
        public static double TestPow(float value, float power)
        {
            return Math.Pow(value, power);
        }

        [TestCompiler(DataRange.Standard)]
        public static sbyte TestAbsSByte(sbyte value)
        {
            return Math.Abs(value);
        }

        [TestCompiler(DataRange.Standard)]
        public static short TestAbsShort(short value)
        {
            return Math.Abs(value);
        }

        [TestCompiler(DataRange.Standard)]
        public static int TestAbsInt(int value)
        {
            return Math.Abs(value);
        }

        [TestCompiler(DataRange.Standard)]
        public static long TestAbsLong(long value)
        {
            return Math.Abs(value);
        }

        [TestCompiler(DataRange.Standard)]
        public static float TestAbsFloat(float value)
        {
            return Math.Abs(value);
        }

        [TestCompiler(DataRange.Standard)]
        public static double TestAbsDouble(double value)
        {
            return Math.Abs(value);
        }

        [TestCompiler(DataRange.Standard, DataRange.Standard)]
        public static int TestMaxInt(int left, int right)
        {
            return Math.Max(left, right);
        }

        [TestCompiler(DataRange.Standard, DataRange.Standard)]
        public static int TestMinInt(int left, int right)
        {
            return Math.Min(left, right);
        }

        [TestCompiler(DataRange.Standard, DataRange.Standard)]
        public static double TestMaxDouble(double left, double right)
        {
            return Math.Max(left, right);
        }

        [TestCompiler(DataRange.Standard, DataRange.Standard)]
        public static double TestMinDouble(double left, double right)
        {
            return Math.Min(left, right);
        }

        [TestCompiler(DataRange.Standard)]
        public static int TestSignInt(int value)
        {
            return Math.Sign(value);
        }

        [TestCompiler(DataRange.Standard & ~DataRange.NaN)]
        public static int TestSignFloat(float value)
        {
            return Math.Sign(value);
        }

        [TestCompiler(float.NaN, ExpectedException = typeof(ArithmeticException))]
        [MonoOnly(".NET CLR does not support burst.abort correctly")]
        public static int TestSignException(float value)
        {
            return Math.Sign(value);
        }

        [TestCompiler(DataRange.Standard & ~DataRange.NaN)]
        public static int TestSignDouble(double value)
        {
            return Math.Sign(value);
        }

        [TestCompiler(DataRange.Standard)]
        public static double TestCeilingDouble(double value)
        {
            return Math.Ceiling(value);
        }

        [TestCompiler(DataRange.Standard)]
        public static double TestFloorDouble(double value)
        {
            return Math.Floor(value);
        }

        [TestCompiler(DataRange.Standard)]
        public static double TestRoundDouble(double value)
        {
            return Math.Round(value);
        }

        [TestCompiler(DataRange.Standard)]
        public static double TestTruncateDouble(double value)
        {
            return Math.Truncate(value);
        }

        [TestCompiler(DataRange.Standard, DataRange.Standard & ~DataRange.Zero)]
        public static int TestDivRemInt(int a, int b)
        {
            int remResult;
            var divResult = Math.DivRem(a, b, out remResult);
            return divResult + remResult * 7;
        }

        [TestCompiler(DataRange.Standard, DataRange.Standard)]
        [TestCompiler(int.MaxValue, DataRange.Standard)]
        public static long TestBigMulInt(int a, int b)
        {
            return Math.BigMul(a, b);
        }

        [TestCompiler(DataRange.Standard & ~DataRange.Zero, DataRange.Standard & ~DataRange.Zero)]
        public static double TestLogWithBaseDouble(double a, double newBase)
        {
            return Math.Log(a, newBase);
        }

        //[TestCompiler(1.0, 2.0)]
        //[TestCompiler(10.0, 3.0)]
        //[TestCompiler(15.0, 4.0)]
        //[Ignore("Not yet supported")]
        //public static double TestIEEERemainder(double a, double newBase)
        //{
        //    return Math.IEEERemainder(a, newBase);
        //}

        [TestCompiler(DataRange.Standard)]
        public static bool TestIsNanDouble(double a)
        {
            return double.IsNaN(a);
        }

        [TestCompiler(DataRange.Standard)]
        public static bool TestIsNanFloat(float a)
        {
            return float.IsNaN(a);
        }

        [TestCompiler(DataRange.Standard)]
        public static bool TestIsInfinityDouble(double a)
        {
            return double.IsInfinity(a);
        }

        [TestCompiler(DataRange.Standard)]
        public static bool TestIsInfinityFloat(float a)
        {
            return float.IsInfinity(a);
        }
    }
}
