namespace Unity.Properties
{
    /// <summary>
    /// @TODO codegen/type reg
    /// </summary>
    public static class CustomEquality
    {
        struct EqualityComparer<T>
        {
            public delegate bool EqualsDelegate(T lhs, T rhs);
            public new static EqualsDelegate Equals;
        }

        static CustomEquality()
        {
            EqualityComparer<char>.Equals = (lhs, rhs) => lhs == rhs;
            EqualityComparer<bool>.Equals = (lhs, rhs) => lhs == rhs;
            EqualityComparer<sbyte>.Equals = (lhs, rhs) => lhs == rhs;
            EqualityComparer<short>.Equals = (lhs, rhs) => lhs == rhs;
            EqualityComparer<int>.Equals = (lhs, rhs) => lhs == rhs;
            EqualityComparer<byte>.Equals = (lhs, rhs) => lhs == rhs;
            EqualityComparer<ushort>.Equals = (lhs, rhs) => lhs == rhs;
            EqualityComparer<uint>.Equals = (lhs, rhs) => lhs == rhs;
            EqualityComparer<ulong>.Equals = (lhs, rhs) => lhs == rhs;
            EqualityComparer<float>.Equals = (lhs, rhs) => lhs == rhs;
            EqualityComparer<double>.Equals = (lhs, rhs) => lhs == rhs;
            EqualityComparer<string>.Equals = (lhs, rhs) => lhs == rhs;
        }

        public static bool Equals<T>(T lhs, T rhs)
        {
            if (null == EqualityComparer<T>.Equals)
            {
                return System.Collections.Generic.EqualityComparer<T>.Default.Equals(lhs, rhs);
            }

            return EqualityComparer<T>.Equals(lhs, rhs);
        }
    }
}
