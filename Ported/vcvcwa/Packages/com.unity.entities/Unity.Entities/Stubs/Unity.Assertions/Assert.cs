using System.Diagnostics;

namespace Unity.Assertions
{
    // TODO: provide an implementation of Unity.Assertions.Assert that does not rely on UnityEngine.
    [DebuggerStepThrough]
    static class Assert
    {
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsTrue(bool condition)
        {
            if (condition)
                return;

            UnityEngine.Assertions.Assert.IsTrue(condition);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsTrue(bool condition, string message)
        {
            if (condition)
                return;

            UnityEngine.Assertions.Assert.IsTrue(condition, message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsFalse(bool condition)
        {
            if (!condition)
                return;

            UnityEngine.Assertions.Assert.IsFalse(condition);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsFalse(bool condition, string message)
        {
            if (!condition)
                return;

            UnityEngine.Assertions.Assert.IsFalse(condition, message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNull<T>(T value) where T : class
        {
            #if NET_DOTS
            IsTrue(ReferenceEquals(value, null));
            #else
            UnityEngine.Assertions.Assert.IsNull(value);
            #endif
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNull<T>(T value, string message) where T : class
        {
            #if NET_DOTS
            IsTrue(ReferenceEquals(value, null), message);
            #else
            UnityEngine.Assertions.Assert.IsNull(value, message);
            #endif
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNotNull<T>(T value) where T : class
        {
            #if NET_DOTS
            IsFalse(ReferenceEquals(value, null));
            #else
            UnityEngine.Assertions.Assert.IsNotNull(value);
            #endif
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNotNull<T>(T value, string message) where T : class
        {
            #if NET_DOTS
            IsFalse(ReferenceEquals(value, null), message);
            #else
            UnityEngine.Assertions.Assert.IsNotNull(value, message);
            #endif
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AreApproximatelyEqual(float expected, float actual)
        {
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(expected, actual);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AreApproximatelyEqual(float expected, float actual, string message)
        {
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(expected, actual, message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AreApproximatelyEqual(float expected, float actual, float tolerance)
        {
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(expected, actual, tolerance);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AreEqual<T>(T expected, T actual)
        {
            UnityEngine.Assertions.Assert.AreEqual(expected, actual);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AreEqual<T>(T expected, T actual, string message)
        {
            #if NET_DOTS
            UnityEngine.Assertions.Assert.AreEqual(expected, actual);
            #else
            UnityEngine.Assertions.Assert.AreEqual(expected, actual, message);
            #endif
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AreNotEqual<T>(T expected, T actual)
        {
            UnityEngine.Assertions.Assert.AreNotEqual(expected, actual);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AreNotEqual<T>(T expected, T actual, string message)
        {
            #if NET_DOTS
            UnityEngine.Assertions.Assert.AreNotEqual(expected, actual);
            #else
            UnityEngine.Assertions.Assert.AreNotEqual(expected, actual, message);
            #endif
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AreEqual(int expected, int actual)
        {
            if (expected == actual)
                return;

            UnityEngine.Assertions.Assert.AreEqual(expected, actual);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AreNotEqual(int expected, int actual)
        {
            if (expected != actual)
                return;

            UnityEngine.Assertions.Assert.AreNotEqual(expected, actual);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AreEqual(bool expected, bool actual)
        {
            if (expected == actual)
                return;

            UnityEngine.Assertions.Assert.AreEqual(expected, actual);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AreNotEqual(bool expected, bool actual)
        {
            if (expected != actual)
                return;

            UnityEngine.Assertions.Assert.AreNotEqual(expected, actual);
        }
    }
}
