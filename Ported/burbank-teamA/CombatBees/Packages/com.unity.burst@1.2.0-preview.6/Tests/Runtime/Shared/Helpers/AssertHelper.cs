using System;
using System.Numerics;
using NUnit.Framework;

namespace Burst.Compiler.IL.Tests.Helpers
{
    internal static class AssertHelper
    {
        /// <summary>
        /// AreEqual handling specially precision for float
        /// </summary>
        /// <param name="expected">The expected result</param>
        /// <param name="result">the actual result</param>
        public static void AreEqual(object expected, object result, int maxUlp)
        {
            if (expected is float && result is float)
            {
                var expectedF = (float)expected;
                var resultF = (float)result;
                int ulp;
                Assert.True(NearEqualFloat(expectedF, resultF, maxUlp, out ulp), $"Expected: {expectedF} != Result: {resultF}, ULPs: {ulp}");
                return;
            }

            if (expected is double && result is double)
            {
                var expectedF = (double)expected;
                var resultF = (double)result;
                long ulp;
                Assert.True(NearEqualDouble(expectedF, resultF, maxUlp, out ulp), $"Expected: {expectedF} != Result: {resultF}, ULPs: {ulp}");
                return;
            }

            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// The value for which all absolute numbers smaller than are considered equal to zero.
        /// </summary>
        public const float ZeroTolerance = 4 * float.Epsilon;

        /// <summary>
        /// The value for which all absolute numbers smaller than are considered equal to zero.
        /// </summary>
        public const double ZeroToleranceDouble = 4 * double.Epsilon;

        public static bool NearEqualFloat(float a, float b, int maxUlp, out int ulp)
        {
            ulp = 0;
            if (Math.Abs(a - b) < ZeroTolerance) return true;

            ulp = GetUlpFloatDistance(a, b);
            return ulp <= maxUlp;
        }

        public static unsafe int GetUlpFloatDistance(float a, float b)
        {
            // Save work if the floats are equal.
            // Also handles +0 == -0
            if (a == b)
            {
                return 0;
            }

            if (float.IsNaN(a) && float.IsNaN(b))
            {
                return 0;
            }

            if (float.IsInfinity(a) && float.IsInfinity(b))
            {
                return 0;
            }

            int aInt = *(int*)&a;
            int bInt = *(int*)&b;

            if ((aInt < 0) != (bInt < 0)) return int.MaxValue;

            // Because we would have an overflow below while trying to do -(int.MinValue)
            // We modify it here so that we don't overflow
            var ulp = (long)aInt - bInt;

            if (ulp <= int.MinValue) return int.MaxValue;
            if (ulp > int.MaxValue) return int.MaxValue;

            // We know for sure that numbers are in the range ]int.MinValue, int.MaxValue]
            return (int)Math.Abs(ulp);
        }

        public static bool NearEqualDouble(double a, double b, int maxUlp, out long ulp)
        {
            ulp = 0;
            if (Math.Abs(a - b) < ZeroTolerance) return true;

            ulp = GetUlpDoubleDistance(a, b);
            return ulp <= maxUlp;
        }

        private static readonly long LongMinValue = long.MinValue;
        private static readonly long LongMaxValue = long.MaxValue;

        public static unsafe long GetUlpDoubleDistance(double a, double b)
        {
            // Save work if the floats are equal.
            // Also handles +0 == -0
            if (a == b)
            {
                return 0;
            }

            if (double.IsNaN(a) && double.IsNaN(b))
            {
                return 0;
            }

            if (double.IsInfinity(a) && double.IsInfinity(b))
            {
                return 0;
            }

            long aInt = *(long*)&a;
            long bInt = *(long*)&b;

            if ((aInt < 0) != (bInt < 0)) return long.MaxValue;

            var ulp = aInt - bInt;

            if (ulp <= LongMinValue) return long.MaxValue;
            if (ulp > LongMaxValue) return long.MaxValue;

            return Math.Abs((long) ulp);
        }

        /// <summary>
        /// Determines whether the specified value is close to zero (0.0f).
        /// </summary>
        /// <param name="a">The floating value.</param>
        /// <returns><c>true</c> if the specified value is close to zero (0.0f); otherwise, <c>false</c>.</returns>
        public static bool IsZero(float a)
        {
            return Math.Abs(a) < ZeroTolerance;
        }

        /// <summary>
        /// Determines whether the specified value is close to zero (0.0f).
        /// </summary>
        /// <param name="a">The floating value.</param>
        /// <returns><c>true</c> if the specified value is close to zero (0.0f); otherwise, <c>false</c>.</returns>
        public static bool IsZero(double a)
        {
            return Math.Abs(a) < ZeroToleranceDouble;
        }
    }
}